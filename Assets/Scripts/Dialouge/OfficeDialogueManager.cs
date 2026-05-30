using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

[System.Serializable]
public class DialogueLine
{
    public bool isPlayer;
    [TextArea(2, 5)]
    public string dialogueText;
}

public class OfficeDialogueManager : MonoBehaviour
{
    public static OfficeDialogueManager Instance;

    [Header("Player Panel Settings")]
    public GameObject playerPanel;
    public TextMeshProUGUI playerDialogueText;

    [Header("NPC Panel Settings")]
    public GameObject npcPanel;
    public TextMeshProUGUI npcNameText;
    public TextMeshProUGUI npcDialogueText;
    public SpriteRenderer npcPortraitRenderer;

    [Header("Options Panel Settings")]
    public GameObject optionsPanel;
    public Button option1Button;
    public TextMeshProUGUI option1Text;
    public Button option2Button;
    public TextMeshProUGUI option2Text;

    [Header("Typing Settings")]
    public float charDelay = 0.05f;

    private DialogueLine[] currentLines;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private System.Action onComplete;

    // Store the NPC details for the whole conversation
    private string currentNpcName;
    private Sprite currentNpcPortrait;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // Notice we added npcName and npcPortrait to the start of this method!
    public void StartConversation(DialogueLine[] lines, string npcName, Sprite npcPortrait, System.Action onFinish = null)
    {
        currentLines = lines;
        currentNpcName = npcName;
        currentNpcPortrait = npcPortrait;
        currentLineIndex = 0;
        onComplete = onFinish;

        playerPanel.SetActive(false);
        npcPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);

        DisplayCurrentLine();
    }

    private void DisplayCurrentLine()
    {
        DialogueLine line = currentLines[currentLineIndex];

        if (line.isPlayer)
        {
            npcPanel.SetActive(false);
            playerPanel.SetActive(true);
            if (npcPortraitRenderer != null) npcPortraitRenderer.gameObject.SetActive(false);
            StartCoroutine(TypeLine(playerDialogueText, line.dialogueText));
        }
        else
        {
            playerPanel.SetActive(false);
            npcPanel.SetActive(true);

            // Use the stored name and portrait!
            npcNameText.text = currentNpcName;
            if (npcPortraitRenderer != null)
            {
                npcPortraitRenderer.gameObject.SetActive(true);
                if (currentNpcPortrait != null) npcPortraitRenderer.sprite = currentNpcPortrait;
            }

            StartCoroutine(TypeLine(npcDialogueText, line.dialogueText));
        }
    }

    private IEnumerator TypeLine(TextMeshProUGUI targetText, string text)
    {
        isTyping = true;
        targetText.text = "";

        foreach (char c in text)
        {
            targetText.text += c;
            yield return new WaitForSeconds(charDelay);
        }
        isTyping = false;
    }

    private void Update()
    {
        if (currentLines == null || currentLines.Length == 0) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                DialogueLine line = currentLines[currentLineIndex];
                if (line.isPlayer) playerDialogueText.text = line.dialogueText;
                else npcDialogueText.text = line.dialogueText;
                isTyping = false;
            }
            else
            {
                currentLineIndex++;
                if (currentLineIndex < currentLines.Length)
                {
                    DisplayCurrentLine();
                }
                else
                {
                    playerPanel.SetActive(false);
                    npcPanel.SetActive(false);
                    if (npcPortraitRenderer != null) npcPortraitRenderer.gameObject.SetActive(false);
                    currentLines = null;
                    onComplete?.Invoke();
                }
            }
        }
    }

    public void ShowOptions(string opt1, UnityAction action1, string opt2, UnityAction action2)
    {
        optionsPanel.SetActive(true);

        option1Text.text = opt1;
        option1Button.onClick.RemoveAllListeners();
        option1Button.onClick.AddListener(action1);
        option1Button.onClick.AddListener(CloseOptions);

        option2Text.text = opt2;
        option2Button.onClick.RemoveAllListeners();
        option2Button.onClick.AddListener(action2);
        option2Button.onClick.AddListener(CloseOptions);
    }

    private void CloseOptions()
    {
        optionsPanel.SetActive(false);
        FindObjectOfType<PlayerController>().enabled = true;
    }
}
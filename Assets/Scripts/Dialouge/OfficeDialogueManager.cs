using UnityEngine;
using TMPro;
using System.Collections;

[System.Serializable]
public class DialogueLine
{
    public bool isPlayer; // Check this if the player is speaking
    public string characterName; // "Ahmet", "Selin", or "Kaan"
    [TextArea(2, 5)]
    public string dialogueText;
    public Sprite characterPortrait; // Drag the character's face sprite here
}

public class OfficeDialogueManager : MonoBehaviour
{
    [Header("Player Panel Settings")]
    public GameObject playerPanel;
    public TextMeshProUGUI playerDialogueText;

    [Header("NPC Panel Settings")]
    public GameObject npcPanel;
    public TextMeshProUGUI npcNameText;
    public TextMeshProUGUI npcDialogueText;
    public SpriteRenderer npcPortraitRenderer; // Updated to SpriteRenderer!

    [Header("Typing Settings")]
    public float charDelay = 0.05f;

    private DialogueLine[] currentLines;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private System.Action onComplete;

    public void StartConversation(DialogueLine[] lines, System.Action onFinish = null)
    {
        currentLines = lines;
        currentLineIndex = 0;
        onComplete = onFinish;

        // Hide both panels initially
        playerPanel.SetActive(false);
        npcPanel.SetActive(false);

        DisplayCurrentLine();
    }

    private void DisplayCurrentLine()
    {
        DialogueLine line = currentLines[currentLineIndex];

        if (line.isPlayer)
        {
            npcPanel.SetActive(false);
            playerPanel.SetActive(true);

            // Hide the NPC portrait when the player is talking
            if (npcPortraitRenderer != null) npcPortraitRenderer.gameObject.SetActive(false);

            StartCoroutine(TypeLine(playerDialogueText, line.dialogueText));
        }
        else
        {
            playerPanel.SetActive(false);
            npcPanel.SetActive(true);
            npcNameText.text = line.characterName;

            // Show and update the NPC portrait
            if (npcPortraitRenderer != null)
            {
                npcPortraitRenderer.gameObject.SetActive(true);
                if (line.characterPortrait != null)
                {
                    npcPortraitRenderer.sprite = line.characterPortrait;
                }
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
        // Only progress if a conversation is active
        if (currentLines == null || currentLines.Length == 0) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                // Instantly finish typing
                StopAllCoroutines();
                DialogueLine line = currentLines[currentLineIndex];

                if (line.isPlayer) playerDialogueText.text = line.dialogueText;
                else npcDialogueText.text = line.dialogueText;

                isTyping = false;
            }
            else
            {
                // Move to next line
                currentLineIndex++;
                if (currentLineIndex < currentLines.Length)
                {
                    DisplayCurrentLine();
                }
                else
                {
                    // End conversation
                    playerPanel.SetActive(false);
                    npcPanel.SetActive(false);

                    if (npcPortraitRenderer != null) npcPortraitRenderer.gameObject.SetActive(false);

                    currentLines = null;
                    onComplete?.Invoke();
                }
            }
        }
    }
}
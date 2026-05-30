using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public float charDelay = 0.05f;

    private string[] lines;
    private int currentLine = 0;
    private bool isTyping = false;
    private System.Action onComplete;

    public void StartDialogue(string[] dialogueLines, System.Action onFinish = null)
    {
        lines = dialogueLines;
        currentLine = 0;
        onComplete = onFinish;
        dialoguePanel.SetActive(true);
        StartCoroutine(TypeLine());
    }

    private IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char c in lines[currentLine])
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(charDelay);
        }
        isTyping = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = lines[currentLine];
                isTyping = false;
            }
            else
            {
                currentLine++;
                if (currentLine < lines.Length)
                {
                    StartCoroutine(TypeLine());
                }
                else
                {
                    dialoguePanel.SetActive(false);
                    onComplete?.Invoke();
                }
            }
        }
    }
}

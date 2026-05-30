using UnityEngine;

public class OfficeEntranceDialogue : MonoBehaviour
{
    public OfficeDialogueManager dialogueManager;

    // This allows you to create the conversation visually in Unity!
    public DialogueLine[] entranceConversation;

    private void Start()
    {
        // Freeze player movement
        GetComponent<PlayerController>().enabled = false;

        // Start the conversation
        dialogueManager.StartConversation(entranceConversation, OnEntranceDialogueDone);
    }

    private void OnEntranceDialogueDone()
    {
        // Unfreeze player movement
        GetComponent<PlayerController>().enabled = true;
    }
}
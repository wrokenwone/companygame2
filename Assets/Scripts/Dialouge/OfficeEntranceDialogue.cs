using UnityEngine;

public class OfficeEntranceDialogue : MonoBehaviour
{
    public DialogueLine[] entranceConversation;

    private void Start()
    {
        GetComponent<PlayerController>().enabled = false;

        // Pass empty string and null since no NPC is involved
        OfficeDialogueManager.Instance.StartConversation(entranceConversation, "", null, OnEntranceDialogueDone);
    }

    private void OnEntranceDialogueDone()
    {
        GetComponent<PlayerController>().enabled = true;
    }
}
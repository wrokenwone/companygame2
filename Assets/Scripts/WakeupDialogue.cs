using UnityEngine;

public class WakeupDialogue : MonoBehaviour
{
    public DialogueManager dialogueManager;

    private string[] wakeupLines = {
        "Bugün...",
        "...aynı.",
        "Her sabah aynı tavan."
    };

    private void Start()
    {
        dialogueManager.StartDialogue(wakeupLines, OnWakeupDone);
    }

    private void OnWakeupDone()
    {
        // Diyalog bitti, karakter hareket edebilir
        GetComponent<PlayerController>().enabled = true;
    }
}
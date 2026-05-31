using UnityEngine;

// This custom class keeps the Inspector perfectly organized!
[System.Serializable]

public enum MinigameType { Photo, Form, Coffee, Meeting, Printer }
[System.Serializable]
public class NPCTask
{
    public string taskName = "Görev Adı";
    public MinigameType minigameType; // NEW: Dropdown to select the minigame!

    [Header("Dialogues")]
    public DialogueLine[] introDialogue;
    public DialogueLine[] rejectDialogue;
    public DialogueLine[] successDialogue;
    public DialogueLine[] failDialogue;

    [Header("Choice Settings")]
    public string acceptButtonText = "Kabul Et";
    public string rejectButtonText = "Reddet";
}

public class NPCInteract : MonoBehaviour
{
    private enum NPCState { DoingTasks, TrustPhase, Done }
    private NPCState currentState = NPCState.DoingTasks;

    [Header("NPC Identity")]
    public string npcName;
    public Sprite npcPortrait;
    public GameObject ePrompt;

    [Header("NPC's Tasks")]
    [Tooltip("Change the size to 1 or 2 depending on how many tasks this NPC has")]
    public NPCTask[] tasks;
    private int currentTaskIndex = 0; // Tracks which task we are currently on

    [Header("Trust System - Intro")]
    public DialogueLine[] trustIntroDialogue;

    [Header("Trust Option 1")]
    public string trustOption1Text = "Me?gulüm";
    public bool option1GainsTrust = false;
    public DialogueLine[] trustOption1Dialogue;

    [Header("Trust Option 2")]
    public string trustOption2Text = "Dinliyorum";
    public bool option2GainsTrust = true;
    public DialogueLine[] trustOption2Dialogue;

    private bool playerNearby = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && currentState != NPCState.Done)
        {
            playerNearby = true;
            if (ePrompt != null) ePrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            if (ePrompt != null) ePrompt.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E) && currentState != NPCState.Done)
        {
            if (ePrompt != null) ePrompt.SetActive(false);
            FindObjectOfType<PlayerController>().enabled = false;

            if (currentState == NPCState.DoingTasks)
            {
                // Start the dialogue for whichever task we are currently on
                NPCTask currentTask = tasks[currentTaskIndex];
                OfficeDialogueManager.Instance.StartConversation(currentTask.introDialogue, npcName, npcPortrait, ShowTaskOptions);
            }
            else if (currentState == NPCState.TrustPhase)
            {
                currentState = NPCState.Done; // Lock them out completely
                OfficeDialogueManager.Instance.StartConversation(trustIntroDialogue, npcName, npcPortrait, ShowTrustOptions);
            }
        }
    }

    // --- PHASE 1: TASK LOGIC ---

    private void ShowTaskOptions()
    {
        NPCTask currentTask = tasks[currentTaskIndex];
        OfficeDialogueManager.Instance.ShowOptions(
            currentTask.acceptButtonText, AcceptTask,
            currentTask.rejectButtonText, RejectTask
        );
    }

    private void AcceptTask()
    {
        NPCTask currentTask = tasks[currentTaskIndex];

        if (currentTask.minigameType == MinigameType.Photo)
        {
            PhotoMinigame.Instance.OpenMinigame(this);
        }
        else if (currentTask.minigameType == MinigameType.Coffee) // NEW COFFEE LOGIC
        {
            CoffeeMinigame.Instance.OpenMinigame(this);
        }
    }

    public void OnMinigameComplete(bool wasSuccessful)
    {
        NPCTask currentTask = tasks[currentTaskIndex];

        if (wasSuccessful)
            OfficeDialogueManager.Instance.StartConversation(currentTask.successDialogue, npcName, npcPortrait, OnTaskFinishedSuccessfully);
        else
            OfficeDialogueManager.Instance.StartConversation(currentTask.failDialogue, npcName, npcPortrait, OnTaskFinishedFailed);
    }

    private void OnTaskFinishedSuccessfully()
    {
        GameManager.Instance.CompleteTask(true);
        AdvanceToNextPhase();
    }

    private void OnTaskFinishedFailed()
    {
        GameManager.Instance.CompleteTask(false);
        AdvanceToNextPhase();
    }

    private void RejectTask()
    {
        NPCTask currentTask = tasks[currentTaskIndex];
        OfficeDialogueManager.Instance.StartConversation(currentTask.rejectDialogue, npcName, npcPortrait, OnRejectFinished);
    }

    private void OnRejectFinished()
    {
        GameManager.Instance.CompleteTask(false);
        AdvanceToNextPhase();
    }

    private void AdvanceToNextPhase()
    {
        // Move to the next task
        currentTaskIndex++;

        // If we ran out of tasks, move to the Trust Phase!
        if (currentTaskIndex >= tasks.Length)
        {
            currentState = NPCState.TrustPhase;
        }

        FindObjectOfType<PlayerController>().enabled = true;
    }

    // --- PHASE 2: TRUST LOGIC ---

    private void ShowTrustOptions()
    {
        OfficeDialogueManager.Instance.ShowOptions(trustOption1Text, ChooseOption1, trustOption2Text, ChooseOption2);
    }

    private void ChooseOption1()
    {
        GameManager.Instance.SetTrust(npcName, option1GainsTrust);
        OfficeDialogueManager.Instance.StartConversation(trustOption1Dialogue, npcName, npcPortrait, EndTrustPhase);
    }

    private void ChooseOption2()
    {
        GameManager.Instance.SetTrust(npcName, option2GainsTrust);
        OfficeDialogueManager.Instance.StartConversation(trustOption2Dialogue, npcName, npcPortrait, EndTrustPhase);
    }

    private void EndTrustPhase()
    {
        FindObjectOfType<PlayerController>().enabled = true;
    }
}
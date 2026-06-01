using UnityEngine;

public enum MinigameType { Photo, Form, Coffee, Meeting, Printer }

[System.Serializable]
public class NPCTask
{
    public string taskName = "Görev Adı";
    public MinigameType minigameType;

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
    private int currentTaskIndex = 0;

    [Header("Trust System - Intro")]
    public DialogueLine[] trustIntroDialogue;

    [Header("Trust Option 1")]
    public string trustOption1Text = "Meşgulüm";
    public bool option1GainsTrust = false;
    public DialogueLine[] trustOption1Dialogue;

    [Header("Trust Option 2")]
    public string trustOption2Text = "Dinliyorum";
    public bool option2GainsTrust = true;
    public DialogueLine[] trustOption2Dialogue;

    private bool playerNearby = false;

    // Memory variables for reporting back
    private bool isWaitingForReport = false;
    private bool lastMinigameSuccess = false;

    // NEW: Global lock so all NPCs know if someone is waiting for a report!
    public static NPCInteract NPCWaitingForReport = null;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // NEW: If ANY NPC is waiting for a report, and it isn't ME, ignore the player!
        if (NPCWaitingForReport != null && NPCWaitingForReport != this) return;

        if (PrinterMinigame.Instance != null && PrinterMinigame.Instance.isWaitingForPlayer) return;
        if (CoffeeMinigame.Instance != null && CoffeeMinigame.Instance.isWaitingForPlayer) return;
        if (MeetingMinigame.Instance != null && MeetingMinigame.Instance.isWaitingForPlayer) return;

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
        // NEW: If ANY NPC is waiting for a report, and it isn't ME, ignore the player!
        if (NPCWaitingForReport != null && NPCWaitingForReport != this) return;

        if (PrinterMinigame.Instance != null && PrinterMinigame.Instance.isWaitingForPlayer) return;
        if (CoffeeMinigame.Instance != null && CoffeeMinigame.Instance.isWaitingForPlayer) return;
        if (MeetingMinigame.Instance != null && MeetingMinigame.Instance.isWaitingForPlayer) return;

        if (playerNearby && Input.GetKeyDown(KeyCode.E) && currentState != NPCState.Done)
        {
            if (ePrompt != null) ePrompt.SetActive(false);
            FindAnyObjectByType<PlayerController>().enabled = false;

            // NEW LOGIC: Is the NPC waiting for you to report back?
            if (isWaitingForReport)
            {
                isWaitingForReport = false; // Reset the memory
                NPCWaitingForReport = null; // NEW: The report is delivered, unlock the other NPCs!
                ReportBackToSelin();
            }
            else if (currentState == NPCState.DoingTasks)
            {
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

        if (currentTask.minigameType == MinigameType.Photo) PhotoMinigame.Instance.OpenMinigame(this);
        else if (currentTask.minigameType == MinigameType.Coffee) CoffeeMinigame.Instance.OpenMinigame(this);
        else if (currentTask.minigameType == MinigameType.Form) FormMinigame.Instance.OpenMinigame(this);
        else if (currentTask.minigameType == MinigameType.Meeting) MeetingMinigame.Instance.OpenMinigame(this);
        else if (currentTask.minigameType == MinigameType.Printer) PrinterMinigame.Instance.OpenMinigame(this);
    }

    public void OnMinigameComplete(bool wasSuccessful)
    {
        lastMinigameSuccess = wasSuccessful;
        NPCTask currentTask = tasks[currentTaskIndex];

        // physical walk-back tasks
        if (currentTask.minigameType == MinigameType.Coffee ||
            currentTask.minigameType == MinigameType.Printer ||
            currentTask.minigameType == MinigameType.Meeting)
        {
            // 1. Memorize the result
            isWaitingForReport = true;

            // NEW: Lock out every other NPC in the game!
            NPCWaitingForReport = this;

            // 2. Turn the player's movement script BACK ON so they can walk back to Selin!
            PlayerController player = FindAnyObjectByType<PlayerController>();
            if (player != null) player.enabled = true;
        }
        else
        {
            // For Photo and Form: Instantly play the success/fail dialogue!
            isWaitingForReport = false;
            ReportBackToSelin();
        }
    }

    private void ReportBackToSelin()
    {
        NPCTask currentTask = tasks[currentTaskIndex];

        if (lastMinigameSuccess)
        {
            OfficeDialogueManager.Instance.StartConversation(currentTask.successDialogue, npcName, npcPortrait, OnTaskFinishedSuccessfully);
        }
        else
        {
            OfficeDialogueManager.Instance.StartConversation(currentTask.failDialogue, npcName, npcPortrait, OnTaskFinishedFailed);
        }
    }

   private void OnTaskFinishedSuccessfully()
    {
        // Tell the GameManager you got a success!
        if (GameManager.Instance != null) GameManager.Instance.AddSuccessfulTask();

        AdvanceToNextPhase();
    }

    private void OnTaskFinishedFailed()
    {
        // Removed the old GameManager call. AdvanceToNextPhase handles progression now!
        AdvanceToNextPhase();
    }

    private void RejectTask()
    {
        NPCTask currentTask = tasks[currentTaskIndex];
        OfficeDialogueManager.Instance.StartConversation(currentTask.rejectDialogue, npcName, npcPortrait, OnRejectFinished);
    }

    private void OnRejectFinished()
    {
        // Removed the old GameManager call. AdvanceToNextPhase handles progression now!
        AdvanceToNextPhase();
    }

    private void AdvanceToNextPhase()
    {
        currentTaskIndex++;

        // If we ran out of tasks, move to the Trust Phase!
        if (currentTaskIndex >= tasks.Length)
        {
            // NEW: Only report this the very first time we enter the Trust Phase
            if (currentState != NPCState.TrustPhase && currentState != NPCState.Done)
            {
                currentState = NPCState.TrustPhase;

                // Tell the Manager this NPC's tasks are done!
                if (GameManager.Instance != null) GameManager.Instance.ReportTaskPhaseComplete();
            }
        }

        FindAnyObjectByType<PlayerController>().enabled = true;
    }

    // --- PHASE 2: TRUST LOGIC ---

    private void ShowTrustOptions()
    {
        OfficeDialogueManager.Instance.ShowOptions(trustOption1Text, ChooseOption1, trustOption2Text, ChooseOption2);
    }

    private void ChooseOption1()
    {
        if (GameManager.Instance != null) GameManager.Instance.SetTrust(npcName, option1GainsTrust);
        OfficeDialogueManager.Instance.StartConversation(trustOption1Dialogue, npcName, npcPortrait, EndTrustPhase);
    }

    private void ChooseOption2()
    {
        if (GameManager.Instance != null) GameManager.Instance.SetTrust(npcName, option2GainsTrust);
        OfficeDialogueManager.Instance.StartConversation(trustOption2Dialogue, npcName, npcPortrait, EndTrustPhase);
    }

    private void EndTrustPhase()
    {
        FindAnyObjectByType<PlayerController>().enabled = true;
    }
}
using UnityEngine;

public class PrinterMinigame : MonoBehaviour
{
    public static PrinterMinigame Instance;

    [Header("Interaction Settings")]
    public GameObject ePrompt;
    public SpriteRenderer printerRenderer;
    public Sprite normalSprite;
    public Sprite happySprite;
    public Sprite sadSprite;

    [Header("Dialogue - Intro")]
    public string printerName = "Printer";
    public Sprite printerPortrait;
    public DialogueLine[] introDialogue;

    [Header("Dialogue - Branch 1")]
    public string option1Text = "Please, I'm busy.";
    public string option2Text = "How are you?";
    public DialogueLine[] failChoiceDialogue;
    public DialogueLine[] option2Dialogue;

    [Header("Dialogue - Branch 2")]
    public string option3Text = "I'm listening.";
    public DialogueLine[] option3Dialogue;

    [Header("Dialogue - Branch 3")]
    public string option4Text = "Let me give you an advertisement.";
    public DialogueLine[] option4Dialogue;

    [Header("Dialogue - Outcomes")]
    public DialogueLine[] successOutcomeDialogue;
    public DialogueLine[] failOutcomeDialogue;

    [HideInInspector] public bool isWaitingForPlayer = false;
    private bool playerNearby = false;
    private NPCInteract requestingNPC;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        if (ePrompt != null) ePrompt.SetActive(false);
    }

    public void OpenMinigame(NPCInteract npc)
    {
        requestingNPC = npc;
        isWaitingForPlayer = true;

        if (printerRenderer != null && normalSprite != null)
            printerRenderer.sprite = normalSprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isWaitingForPlayer)
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
        if (playerNearby && isWaitingForPlayer && Input.GetKeyDown(KeyCode.E))
        {
            if (ePrompt != null) ePrompt.SetActive(false);
            FindAnyObjectByType<PlayerController>().enabled = false;

            OfficeDialogueManager.Instance.StartConversation(introDialogue, printerName, printerPortrait, ShowFirstOptions);
        }
    }

    private void ShowFirstOptions()
    {
        OfficeDialogueManager.Instance.ShowOptions(option1Text, ChooseFailPath, option2Text, ChooseOption2);
    }

    private void ChooseFailPath()
    {
        OfficeDialogueManager.Instance.StartConversation(failChoiceDialogue, printerName, printerPortrait, PlayFailOutcome);
    }

    private void ChooseOption2()
    {
        OfficeDialogueManager.Instance.StartConversation(option2Dialogue, printerName, printerPortrait, ShowSecondOptions);
    }

    private void ShowSecondOptions()
    {
        OfficeDialogueManager.Instance.ShowOptions(option3Text, ChooseOption3, "", null);
    }

    private void ChooseOption3()
    {
        OfficeDialogueManager.Instance.StartConversation(option3Dialogue, printerName, printerPortrait, ShowThirdOptions);
    }

    private void ShowThirdOptions()
    {
        OfficeDialogueManager.Instance.ShowOptions(option4Text, ChooseOption4, "", null);
    }

    private void ChooseOption4()
    {
        OfficeDialogueManager.Instance.StartConversation(option4Dialogue, printerName, printerPortrait, PlaySuccessOutcome);
    }

    private void PlaySuccessOutcome()
    {
        if (printerRenderer != null && happySprite != null) printerRenderer.sprite = happySprite;
        OfficeDialogueManager.Instance.StartConversation(successOutcomeDialogue, printerName, printerPortrait, OnSuccessFinished);
    }

    private void PlayFailOutcome()
    {
        if (printerRenderer != null && sadSprite != null) printerRenderer.sprite = sadSprite;
        OfficeDialogueManager.Instance.StartConversation(failOutcomeDialogue, printerName, printerPortrait, OnFailFinished);
    }

    private void OnSuccessFinished()
    {
        isWaitingForPlayer = false;

        if (requestingNPC != null) requestingNPC.OnMinigameComplete(true);
    }

    private void OnFailFinished()
    {
        isWaitingForPlayer = false;

        if (requestingNPC != null) requestingNPC.OnMinigameComplete(false);
    }
}
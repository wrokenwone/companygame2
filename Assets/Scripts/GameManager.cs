using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Elements")]
    public GameObject endDayButton;

    [Header("Game Progress")]
    public int totalNPCsInLevel = 3;
    private int npcsFinishedWithTasks = 0;

    [Header("Trust System")]
    public int totalTrustScore = 0;

    [Header("Ending Dialogues")]
    public string bossName = "Selin";
    public Sprite bossPortrait;
    public DialogueLine[] goodEndingDialogue;
    public DialogueLine[] badEndingDialogue;

    [Header("Cinematic Settings")]
    public Transform bossObject; // Assign Selin's physical object here
    public Transform playerObject; // Assign the Player here
    public float bossWalkSpeed = 3f;

    [Header("Final Interaction Triggers")]
    public EndingTrigger doorExitTrigger;
    public EndingTrigger deskSitTrigger;

    private bool isGoodEndingSequence = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        if (endDayButton != null) endDayButton.SetActive(false);
    }

    public void ReportTaskPhaseComplete()
    {
        npcsFinishedWithTasks++;
        if (npcsFinishedWithTasks >= totalNPCsInLevel)
        {
            if (endDayButton != null) endDayButton.SetActive(true);
        }
    }

    public void SetTrust(string npcName, bool gainedTrust)
    {
        if (gainedTrust) totalTrustScore++;
    }

    public void OnEndDayButtonClicked()
    {
        if (endDayButton != null) endDayButton.SetActive(false);

        // 1. Freeze the player
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null) player.enabled = false;

        // 2. Start the dramatic walk sequence!
        StartCoroutine(BossWalkToPlayerRoutine());
    }

    private IEnumerator BossWalkToPlayerRoutine()
    {
        // NEW: Turn the hidden cinematic boss ON before she starts walking!
        if (bossObject != null) bossObject.gameObject.SetActive(true);

        // Slide the boss towards the player until she is 1.5 units away
        while (Vector2.Distance(bossObject.position, playerObject.position) > 1.5f)
        {
            bossObject.position = Vector2.MoveTowards(bossObject.position, playerObject.position, bossWalkSpeed * Time.deltaTime);
            yield return null; // Wait for the next frame
        }

        // Figure out which ending we got
        isGoodEndingSequence = totalTrustScore >= (totalNPCsInLevel / 2f);

        // Start the dialogue!
        if (isGoodEndingSequence)
        {
            OfficeDialogueManager.Instance.StartConversation(goodEndingDialogue, bossName, bossPortrait, OnEndingDialogueFinished);
        }
        else
        {
            OfficeDialogueManager.Instance.StartConversation(badEndingDialogue, bossName, bossPortrait, OnEndingDialogueFinished);
        }
    }

    private void OnEndingDialogueFinished()
    {
        // Unfreeze the player so they can walk to the final trigger!
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null) player.enabled = true;

        // NEW: Unlock the correct physical trigger in the room!
        if (isGoodEndingSequence)
        {
            if (doorExitTrigger != null) doorExitTrigger.isUnlocked = true;
        }
        else
        {
            if (deskSitTrigger != null) deskSitTrigger.isUnlocked = true;
        }
    }
}
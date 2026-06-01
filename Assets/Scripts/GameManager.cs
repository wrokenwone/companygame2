using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[System.Serializable]
public class NPCCompanion
{
    public string npcName;
    public GameObject mainNPC;
    public GameObject cinematicNPC;

    [Header("Pathfinding")]
    [Tooltip("Place an empty GameObject in the aisle outside their desk so they don't walk through walls!")]
    public Transform aisleWaypoint;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Elements")]
    public GameObject endDayButton;

    [Header("Game Progress")]
    public int totalNPCsInLevel = 3;
    private int npcsFinishedWithTasks = 0;
    public int successfulTasks = 0; // NEW: Tracks your minigame wins!

    [Header("Trust System & Companions")]
    public int totalTrustScore = 0;
    public List<NPCCompanion> companionsList;
    private Dictionary<string, bool> npcTrustRecords = new Dictionary<string, bool>();

    [Header("Ending Dialogues")]
    public string bossName = "Selin";
    public Sprite bossPortrait;
    public DialogueLine[] goodEndingDialogue;
    public DialogueLine[] badEndingDialogue;

    [Header("Cinematic Settings")]
    public Transform bossObject;
    public Transform playerObject;
    public float walkSpeed = 3f;

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
        if (!npcTrustRecords.ContainsKey(npcName))
        {
            npcTrustRecords.Add(npcName, gainedTrust);
        }
        else
        {
            npcTrustRecords[npcName] = gainedTrust;
        }

        if (gainedTrust) totalTrustScore++;
    }

    public void OnEndDayButtonClicked()
    {
        if (endDayButton != null) endDayButton.SetActive(false);

        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null) player.enabled = false;

        StartCoroutine(BossWalkToPlayerRoutine());
    }

    private IEnumerator BossWalkToPlayerRoutine()
    {
        if (bossObject != null) bossObject.gameObject.SetActive(true);

        while (Vector2.Distance(bossObject.position, playerObject.position) > 1.5f)
        {
            bossObject.position = Vector2.MoveTowards(bossObject.position, playerObject.position, walkSpeed * Time.deltaTime);
            yield return null;
        }

        isGoodEndingSequence = totalTrustScore <= 2;

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
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null) player.enabled = true;

        if (isGoodEndingSequence)
        {
            if (doorExitTrigger != null) doorExitTrigger.isUnlocked = true;
        }
        else
        {
            if (deskSitTrigger != null) deskSitTrigger.isUnlocked = true;
        }
    }

    public void StartCompanionsExit(Vector3 doorPosition, string sceneToLoad)
    {
        StartCoroutine(CompanionsExitRoutine(doorPosition, sceneToLoad));
    }

    private IEnumerator CompanionsExitRoutine(Vector3 doorPosition, string sceneToLoad)
    {
        foreach (NPCCompanion companion in companionsList)
        {
            if (npcTrustRecords.ContainsKey(companion.npcName) && npcTrustRecords[companion.npcName] == true)
            {
                if (companion.mainNPC != null) companion.mainNPC.SetActive(false);

                if (companion.cinematicNPC != null)
                {
                    companion.cinematicNPC.SetActive(true);

                    // NEW: 1. Walk to the aisle waypoint first (to get out of the cubicle)
                    if (companion.aisleWaypoint != null)
                    {
                        while (Vector2.Distance(companion.cinematicNPC.transform.position, companion.aisleWaypoint.position) > 0.1f)
                        {
                            companion.cinematicNPC.transform.position = Vector2.MoveTowards(companion.cinematicNPC.transform.position, companion.aisleWaypoint.position, walkSpeed * Time.deltaTime);
                            yield return null;
                        }
                    }

                    // NEW: 2. Now walk horizontally/vertically to the door!
                    while (Vector2.Distance(companion.cinematicNPC.transform.position, doorPosition) > 0.1f)
                    {
                        companion.cinematicNPC.transform.position = Vector2.MoveTowards(companion.cinematicNPC.transform.position, doorPosition, walkSpeed * Time.deltaTime);
                        yield return null;
                    }

                    companion.cinematicNPC.SetActive(false);
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }

        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(sceneToLoad);
    }
    // NEW: Called by NPCInteract when you win a minigame!
    public void AddSuccessfulTask()
    {
        successfulTasks++;
    }
}
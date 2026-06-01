using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class MeetingMinigame : MonoBehaviour
{
    public static MeetingMinigame Instance;

    [Header("Interaction Settings")]
    public GameObject ePrompt;
    [HideInInspector] public bool isWaitingForPlayer = false;
    private bool playerNearby = false;

    [Header("UI Elements")]
    public GameObject minigamePanel;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI promptText;

    [Header("Directional Buttons")]
    public Button downButton;
    public Button upButton;
    public Button leftButton;
    public Button rightButton;

    [Header("Player Visuals")]
    public GameObject playerNormal;
    public GameObject playerNod;

    [Header("Text Customization")]
    public string meetingStartedText = "Toplantı Başladı!";
    public string nodPromptText = "KAFA SALLA!";

    [Header("Timing Settings")]
    public float totalMeetingTime = 30f;
    public float minPromptDelay = 3f;
    public float maxPromptDelay = 5f;
    public float reactionTime = 1.5f;

    [Header("Win Conditions")]
    public int totalPromptsToShow = 5;
    public int requiredNodsToWin = 3;

    private NPCInteract requestingNPC;
    private bool isPlaying = false;

    private float gameTimer = 0f;
    private float nextPromptTimer = 0f;
    private float activePromptTimer = 0f;

    private int successfulNods = 0;
    private int promptsShown = 0;
    private bool isPromptActive = false;

    private bool isIntroPhase = false;
    private float introTimer = 2f;

    private Coroutine nodAnimationCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        if (downButton != null) downButton.onClick.AddListener(OnNodInput);
        if (upButton != null) upButton.onClick.AddListener(OnWrongInput);
        if (leftButton != null) leftButton.onClick.AddListener(OnWrongInput);
        if (rightButton != null) rightButton.onClick.AddListener(OnWrongInput);

        minigamePanel.SetActive(false);
        if (ePrompt != null) ePrompt.SetActive(false);
    }

    // Called by Selin. Turns the computer "on" and waits for the player!
    public void OpenMinigame(NPCInteract npc)
    {
        requestingNPC = npc;
        isWaitingForPlayer = true;
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
        // 1. Check for player pressing 'E'
        if (playerNearby && isWaitingForPlayer && Input.GetKeyDown(KeyCode.E))
        {
            if (ePrompt != null) ePrompt.SetActive(false);
            FindAnyObjectByType<PlayerController>().enabled = false; // Freeze player

            StartMeetingPanel();
        }

        // 2. The minigame logic (only runs when the panel is open!)
        if (!isPlaying) return;

        if (isIntroPhase)
        {
            introTimer -= Time.deltaTime;
            if (introTimer <= 0)
            {
                isIntroPhase = false;
                nextPromptTimer = Random.Range(minPromptDelay, maxPromptDelay);
            }
            return;
        }

        gameTimer += Time.deltaTime;

        int currentMinute = Mathf.FloorToInt(gameTimer);
        if (currentMinute > totalMeetingTime) currentMinute = (int)totalMeetingTime;
        timeText.text = $"{currentMinute}min/{totalMeetingTime}min";

        if (gameTimer >= totalMeetingTime)
        {
            EndMeeting();
            return;
        }

        if (!isPromptActive && promptsShown < totalPromptsToShow)
        {
            nextPromptTimer -= Time.deltaTime;
            if (nextPromptTimer <= 0)
            {
                TriggerPrompt();
            }
        }

        if (isPromptActive)
        {
            activePromptTimer -= Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.DownArrow)) OnNodInput();
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                OnWrongInput();
            }
            else if (activePromptTimer <= 0)
            {
                ClearPrompt();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.DownArrow)) OnNodInput();
        }
    }

    // Opens the UI panel after the player presses E
    private void StartMeetingPanel()
    {
        gameTimer = 0f;
        successfulNods = 0;
        promptsShown = 0;
        isPromptActive = false;

        playerNormal.SetActive(true);
        playerNod.SetActive(false);
        promptText.text = "";

        isIntroPhase = true;
        introTimer = 2f;
        timeText.text = meetingStartedText;

        isPlaying = true;
        ScreenFader.Instance.FadeToPanel(minigamePanel);
    }

    public void OnNodInput()
    {
        if (!isPlaying || isIntroPhase) return;

        if (isPromptActive)
        {
            successfulNods++;
            ClearPrompt();
            TriggerNodAnimation();
        }
        else
        {
            TriggerNodAnimation();
        }
    }

    public void OnWrongInput()
    {
        if (!isPlaying || isIntroPhase) return;

        if (isPromptActive)
        {
            ClearPrompt();
        }
    }

    private void TriggerPrompt()
    {
        isPromptActive = true;
        promptsShown++;
        activePromptTimer = reactionTime;
        promptText.text = nodPromptText;
    }

    private void ClearPrompt()
    {
        isPromptActive = false;
        promptText.text = "";
        nextPromptTimer = Random.Range(minPromptDelay, maxPromptDelay);
    }

    private void TriggerNodAnimation()
    {
        if (nodAnimationCoroutine != null) StopCoroutine(nodAnimationCoroutine);
        nodAnimationCoroutine = StartCoroutine(NodRoutine());
    }

    private IEnumerator NodRoutine()
    {
        playerNormal.SetActive(false);
        playerNod.SetActive(true);
        yield return new WaitForSeconds(0.4f);
        playerNormal.SetActive(true);
        playerNod.SetActive(false);
    }

    private void EndMeeting()
    {
        isPlaying = false;
        promptText.text = "Toplantı Bitti!";

        bool isSuccess = (successfulNods >= requiredNodsToWin);

        ScreenFader.Instance.FadeClosePanel(minigamePanel, () =>
        {
            isWaitingForPlayer = false; // Turn off the waiting state
            if (requestingNPC != null) requestingNPC.OnMinigameComplete(isSuccess);
        });
    }
}
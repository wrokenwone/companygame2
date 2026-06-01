using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class CoffeeMinigame : MonoBehaviour
{
    public static CoffeeMinigame Instance;

    [Header("Interaction Settings")]
    public GameObject ePrompt; // The "Press E" visual
    [HideInInspector] public bool isWaitingForPlayer = false;
    private bool playerNearby = false;

    [Header("UI Elements")]
    public GameObject minigamePanel;
    public Slider coffeeSlider;
    public Button pourButton;

    [Header("Sprite Swap Settings")]
    public SpriteRenderer coffeeCupRenderer;
    public Sprite defaultCupSprite;
    public Sprite filledCupSprite;

    [Header("End Settings")]
    public float delayAfterPour = 2f;
    public string nextSceneToLoad = "";

    [Header("Game Settings (0 to 1 Scale)")]
    public float fillSpeed = 1.5f;
    public float targetValue = 0.7f;
    public float tolerance = 0.05f;

    private bool isPlaying = false;
    private float direction = 1f;
    private NPCInteract requestingNPC;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        if (pourButton != null) pourButton.onClick.AddListener(OnPourClicked);
        minigamePanel.SetActive(false);
        if (ePrompt != null) ePrompt.SetActive(false);
    }

    // This is called by Selin. It no longer opens the panel immediately!
    // Instead, it turns the machine "on" and waits for the player.
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

            StartCoffeePanel();
        }

        // 2. The Slider logic (only runs when the panel is open!)
        if (!isPlaying) return;

        coffeeSlider.value += fillSpeed * direction * Time.deltaTime;

        if (coffeeSlider.value >= 1f) { coffeeSlider.value = 1f; direction = -1f; }
        else if (coffeeSlider.value <= 0f) { coffeeSlider.value = 0f; direction = 1f; }
    }

    // Opens the actual UI panel once the player presses E
    private void StartCoffeePanel()
    {
        if (coffeeCupRenderer != null && defaultCupSprite != null)
            coffeeCupRenderer.sprite = defaultCupSprite;

        coffeeSlider.minValue = 0f;
        coffeeSlider.maxValue = 1f;
        coffeeSlider.value = 0f;
        direction = 1f;
        isPlaying = true;

        ScreenFader.Instance.FadeToPanel(minigamePanel);
    }

    private void OnPourClicked()
    {
        if (!isPlaying) return;
        isPlaying = false;

        if (coffeeCupRenderer != null && filledCupSprite != null)
            coffeeCupRenderer.sprite = filledCupSprite;

        bool isSuccess = Mathf.Abs(coffeeSlider.value - targetValue) <= tolerance;

        StartCoroutine(FinishMinigameRoutine(isSuccess));
    }

    private IEnumerator FinishMinigameRoutine(bool isSuccess)
    {
        yield return new WaitForSeconds(delayAfterPour);

        ScreenFader.Instance.FadeClosePanel(minigamePanel, () =>
        {
            isWaitingForPlayer = false; // Turn off the waiting state

            if (!string.IsNullOrEmpty(nextSceneToLoad))
            {
                SceneManager.LoadScene(nextSceneToLoad);
            }
            else
            {
                // This tells Selin the task is done, freezing the memory so you have to walk back to her!
                if (requestingNPC != null) requestingNPC.OnMinigameComplete(isSuccess);
            }
        });
    }
}
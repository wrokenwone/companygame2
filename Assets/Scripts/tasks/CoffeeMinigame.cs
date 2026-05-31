using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class CoffeeMinigame : MonoBehaviour
{
    public static CoffeeMinigame Instance;

    [Header("UI Elements")]
    public GameObject minigamePanel;
    public Slider coffeeSlider;
    public Button pourButton;

    [Header("Sprite Swap Settings")]
    public SpriteRenderer coffeeCupRenderer; // CHANGED: Now accepts your 2D SpriteRenderer!
    public Sprite defaultCupSprite; // The empty cup image file
    public Sprite filledCupSprite; // The full cup image file

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
    }

    public void OpenMinigame(NPCInteract npc)
    {
        requestingNPC = npc;

        // Reset the image to the empty cup when the game opens
        if (coffeeCupRenderer != null && defaultCupSprite != null)
            coffeeCupRenderer.sprite = defaultCupSprite;

        coffeeSlider.minValue = 0f;
        coffeeSlider.maxValue = 1f;
        coffeeSlider.value = 0f;
        direction = 1f;
        isPlaying = true;

        ScreenFader.Instance.FadeToPanel(minigamePanel);
    }

    private void Update()
    {
        if (!isPlaying) return;

        coffeeSlider.value += fillSpeed * direction * Time.deltaTime;

        if (coffeeSlider.value >= 1f) { coffeeSlider.value = 1f; direction = -1f; }
        else if (coffeeSlider.value <= 0f) { coffeeSlider.value = 0f; direction = 1f; }
    }

    private void OnPourClicked()
    {
        if (!isPlaying) return;
        isPlaying = false;

        // Swap the sprite to the filled cup!
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
            if (!string.IsNullOrEmpty(nextSceneToLoad))
            {
                SceneManager.LoadScene(nextSceneToLoad);
            }
            else
            {
                if (requestingNPC != null) requestingNPC.OnMinigameComplete(isSuccess);
            }
        });
    }
}
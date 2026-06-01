using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FormMinigame : MonoBehaviour
{
    public static FormMinigame Instance;

    [Header("UI Elements")]
    public GameObject minigamePanel; // The main "Form_panel"
    public Transform formObject; // The paper that moves
    public Button[] optionButtons; // Drag your 4 Option buttons here

    [Header("Animation Settings")]
    [Tooltip("How far down it starts, and how far up it leaves (in Unity units/pixels)")]
    public float slideDistance = 15f;
    public float slideDuration = 0.6f;

    [Header("Game Settings")]
    public int correctOptionIndex = 2; // Matches "Option A (right one)" if it's the 3rd item

    private NPCInteract requestingNPC;
    private Vector3 onScreenPosition; // Remembers where the center is
    private bool isAnimating = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        // Memorize where the form is supposed to rest on screen
        if (formObject != null)
        {
            onScreenPosition = formObject.localPosition;
        }

        // Hook up the buttons automatically
        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i;
            optionButtons[i].onClick.AddListener(() => OnOptionClicked(index));
        }

        minigamePanel.SetActive(false); // Hide on start
    }

    public void OpenMinigame(NPCInteract npc)
    {
        requestingNPC = npc;

        // Re-enable all buttons so they can be clicked
        foreach (Button btn in optionButtons) btn.interactable = true;

        // Start the slide IN animation
        StartCoroutine(SlideRoutine(slidingIn: true, isSuccess: false, closeAtEnd: false));
    }

    private void OnOptionClicked(int index)
    {
        if (isAnimating) return; // Prevent clicking while it's moving

        // Lock all buttons so the player can't click twice
        foreach (Button btn in optionButtons) btn.interactable = false;

        bool isSuccess = (index == correctOptionIndex);

        // Start the slide OUT animation
        StartCoroutine(SlideRoutine(slidingIn: false, isSuccess: isSuccess, closeAtEnd: true));
    }

    private IEnumerator SlideRoutine(bool slidingIn, bool isSuccess, bool closeAtEnd)
    {
        isAnimating = true;

        if (slidingIn) minigamePanel.SetActive(true);

        // Calculate start and end points mathematically
        Vector3 startPos = slidingIn ? onScreenPosition + (Vector3.down * slideDistance) : onScreenPosition;
        Vector3 endPos = slidingIn ? onScreenPosition : onScreenPosition + (Vector3.up * slideDistance);

        float t = 0;
        while (t < slideDuration)
        {
            t += Time.deltaTime;

            // SmoothStep makes the slide slow down gently at the end like a real paper!
            float smoothedTime = Mathf.SmoothStep(0, 1, t / slideDuration);
            formObject.localPosition = Vector3.Lerp(startPos, endPos, smoothedTime);

            yield return null;
        }

        // Lock perfectly into place
        formObject.localPosition = endPos;
        isAnimating = false;

        if (closeAtEnd)
        {
            minigamePanel.SetActive(false);
            if (requestingNPC != null) requestingNPC.OnMinigameComplete(isSuccess);
        }
    }
}
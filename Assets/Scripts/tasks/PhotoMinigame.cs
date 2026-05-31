using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement; // Added for scene loading

public class PhotoMinigame : MonoBehaviour
{
    public static PhotoMinigame Instance;

    [Header("UI Elements")]
    public GameObject minigamePanel;
    public Transform selectionBorder;
    public Button confirmButton;

    [Header("Game Settings")]
    public Button[] photoButtons;
    public int correctPhotoIndex = 2;

    [Header("End Settings")]
    [Tooltip("How many seconds to wait after clicking confirm before fading out")]
    public float delayAfterConfirm = 1f;
    [Tooltip("Type a scene name to go there. Leave EMPTY to return to the Office.")]
    public string nextSceneToLoad = "";

    private int selectedIndex = -1;
    private NPCInteract requestingNPC;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < photoButtons.Length; i++)
        {
            int index = i;
            photoButtons[i].onClick.AddListener(() => SelectPhoto(index));
        }

        minigamePanel.SetActive(false);
    }

    public void OpenMinigame(NPCInteract npc)
    {
        requestingNPC = npc;
        selectedIndex = -1;

        selectionBorder.gameObject.SetActive(false);
        if (confirmButton != null) confirmButton.interactable = false;

        // FADE IN INSTEAD OF POPPING IN!
        ScreenFader.Instance.FadeToPanel(minigamePanel);
    }

    public void SelectPhoto(int index)
    {
        selectedIndex = index;
        selectionBorder.gameObject.SetActive(true);

        selectionBorder.position = photoButtons[index].transform.position;

        if (confirmButton != null) confirmButton.interactable = true;
    }

    public void OnConfirmClicked()
    {
        // Prevent them from clicking confirm multiple times during the fade!
        if (confirmButton != null) confirmButton.interactable = false;

        bool isSuccess = (selectedIndex == correctPhotoIndex);

        // Start the fade out sequence
        StartCoroutine(FinishMinigameRoutine(isSuccess));
    }

    private IEnumerator FinishMinigameRoutine(bool isSuccess)
    {
        // 1. Wait a moment so they can see their final choice locked in
        yield return new WaitForSeconds(delayAfterConfirm);

        // 2. Fade to black, close panel, and run the ending logic
        ScreenFader.Instance.FadeClosePanel(minigamePanel, () =>
        {
            // 3. Go to next scene if one is typed in the inspector
            if (!string.IsNullOrEmpty(nextSceneToLoad))
            {
                SceneManager.LoadScene(nextSceneToLoad);
            }
            // 4. Otherwise, continue the normal quest logic
            else
            {
                if (requestingNPC != null) requestingNPC.OnMinigameComplete(isSuccess);
            }
        });
    }
}
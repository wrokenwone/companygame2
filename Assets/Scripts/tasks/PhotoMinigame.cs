using UnityEngine;
using UnityEngine.UI;

public class PhotoMinigame : MonoBehaviour
{
    public static PhotoMinigame Instance;

    [Header("UI Elements")]
    public GameObject minigamePanel;
    public Transform selectionBorder; // CHANGED: Now accepts a standard 2D Transform!
    public Button confirmButton;

    [Header("Game Settings")]
    public Button[] photoButtons;
    public int correctPhotoIndex = 2;

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
    }

    public void OpenMinigame(NPCInteract npc)
    {
        requestingNPC = npc;
        selectedIndex = -1;

        selectionBorder.gameObject.SetActive(false);
        if (confirmButton != null) confirmButton.interactable = false;

        minigamePanel.SetActive(true);
    }

    public void SelectPhoto(int index)
    {
        selectedIndex = index;
        selectionBorder.gameObject.SetActive(true);

        // CHANGED: Now grabs the standard position of the clicked photo!
        selectionBorder.position = photoButtons[index].transform.position;

        if (confirmButton != null) confirmButton.interactable = true;
    }

    public void OnConfirmClicked()
    {
        minigamePanel.SetActive(false);

        bool isSuccess = (selectedIndex == correctPhotoIndex);

        if (requestingNPC != null)
        {
            requestingNPC.OnMinigameComplete(isSuccess);
        }
    }
}
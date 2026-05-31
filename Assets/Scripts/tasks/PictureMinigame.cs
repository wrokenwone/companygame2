using UnityEngine;
using UnityEngine.UI;

public class PictureMinigame : MonoBehaviour
{
    public static PictureMinigame Instance;

    [Header("UI Elements")]
    public GameObject minigamePanel; // The main "Pictures" panel
    public RectTransform selectionBorder; // The yellow outline image
    public Button confirmButton; // The bottom button

    [Header("Game Settings")]
    public Button[] pictureButtons; // Drag the 5 pictures here
    public int correctPictureIndex = 2; // If the 3rd picture is correct, this is 2 (starts at 0)

    private int selectedIndex = -1;
    private NPCInteract requestingNPC;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        // Automatically set up the click events for all 5 pictures
        for (int i = 0; i < pictureButtons.Length; i++)
        {
            int index = i; // Save a local copy for the button memory
            pictureButtons[i].onClick.AddListener(() => SelectPicture(index));
        }
    }

    // Called by the NPCInteract script when the player clicks "Yes"
    public void OpenMinigame(NPCInteract npc)
    {
        requestingNPC = npc;
        selectedIndex = -1; // Reset selection

        selectionBorder.gameObject.SetActive(false); // Hide border at start
        confirmButton.interactable = false; // Cannot click confirm yet

        minigamePanel.SetActive(true); // Show the panel
    }

    public void SelectPicture(int index)
    {
        selectedIndex = index;
        selectionBorder.gameObject.SetActive(true);

        // Move the yellow border perfectly on top of the clicked picture
        selectionBorder.position = pictureButtons[index].GetComponent<RectTransform>().position;

        // Allow the player to click the confirm button now
        confirmButton.interactable = true;
    }

    // Put this on the Confirm Button's OnClick event in the Inspector!
    public void OnConfirmClicked()
    {
        minigamePanel.SetActive(false); // Close the minigame

        bool isSuccess = (selectedIndex == correctPictureIndex);

        // Tell the NPC who started this game whether the player won or lost!
        if (requestingNPC != null)
        {
            requestingNPC.OnMinigameComplete(isSuccess);
        }
    }
}
using UnityEngine;

public class FolderClickHandler : MonoBehaviour
{
    public GameObject menuPanel;

    public void OpenMenu()
    {
        if (menuPanel != null)
            menuPanel.SetActive(!menuPanel.activeSelf);
    }
}
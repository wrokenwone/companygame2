using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject menuPanel;

    public void NewGame()
    {
        SceneManager.LoadScene("HomeWakeup");
    }

    public void LoadGame()
    {
        Debug.Log("Load Game - henüz yapılmadı");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
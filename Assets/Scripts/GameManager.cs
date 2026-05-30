using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [HideInInspector] public int successfulTasks = 0;
    [HideInInspector] public int totalTasksCompleted = 0;

    // --- NEW: Trust System Variables ---
    [HideInInspector] public bool ahmetTrusted = false;
    [HideInInspector] public bool selinTrusted = false;
    [HideInInspector] public bool kaanTrusted = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void CompleteTask(bool wasSuccessful)
    {
        totalTasksCompleted++;

        if (wasSuccessful) successfulTasks++;

        if (totalTasksCompleted >= 5)
        {
            if (successfulTasks >= 3) SceneManager.LoadScene("Ending_Good");
            else SceneManager.LoadScene("Ending_Bad");
        }
    }

    // --- NEW: Function to set trust based on NPC name ---
    public void SetTrust(string npcName, bool isTrusted)
    {
        if (npcName == "Ahmet") ahmetTrusted = isTrusted;
        else if (npcName == "Selin") selinTrusted = isTrusted;
        else if (npcName == "Kaan") kaanTrusted = isTrusted;

        Debug.Log(npcName + " trust set to: " + isTrusted);
    }
}
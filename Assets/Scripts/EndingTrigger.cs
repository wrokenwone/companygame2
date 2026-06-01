using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndingTrigger : MonoBehaviour
{
    [Header("Interaction Settings")]
    public GameObject ePrompt;
    public string sceneToLoad;

    [Header("Ending Type")]
    [Tooltip("Check this box ONLY for the Door object!")]
    public bool isDoorExit = false;
    [Tooltip("Check this box ONLY for the Desk object!")]
    public bool snapToCenter = false;

    [HideInInspector] public bool isUnlocked = false;
    private bool playerNearby = false;

    private void Start()
    {
        if (ePrompt != null) ePrompt.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isUnlocked) return;

        if (other.CompareTag("Player"))
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
        if (!isUnlocked) return;

        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (ePrompt != null) ePrompt.SetActive(false);

            PlayerController player = FindAnyObjectByType<PlayerController>();
            if (player != null) player.enabled = false;

            if (isDoorExit)
            {
                // GOOD ENDING: Hide the player immediately, and start the NPC walk!
                if (player != null) player.gameObject.SetActive(false);
                GameManager.Instance.StartCompanionsExit(this.transform.position, sceneToLoad);
            }
            else
            {
                // BAD ENDING: Snap to the desk and load the scene
                if (snapToCenter && player != null)
                {
                    player.transform.position = this.transform.position;
                }
                StartCoroutine(WaitAndLoadScene());
            }
        }
    }

    private IEnumerator WaitAndLoadScene()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(sceneToLoad);
    }
}
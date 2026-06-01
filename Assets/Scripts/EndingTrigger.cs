using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndingTrigger : MonoBehaviour
{
    [Header("Interaction Settings")]
    public GameObject ePrompt;
    public string sceneToLoad;
    public bool snapToCenter = false;

    // NEW: A lock so the player can't interact early!
    [HideInInspector] public bool isUnlocked = false;

    private bool playerNearby = false;

    private void Start()
    {
        if (ePrompt != null) ePrompt.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Don't show the E prompt if the ending hasn't happened yet!
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
        // Ignore key presses if it is locked!
        if (!isUnlocked) return;

        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (ePrompt != null) ePrompt.SetActive(false);

            PlayerController player = FindAnyObjectByType<PlayerController>();
            if (player != null) player.enabled = false;

            if (snapToCenter && player != null)
            {
                player.transform.position = this.transform.position;
            }

            // Start the dramatic pause before loading
            StartCoroutine(WaitAndLoadScene());
        }
    }

    private IEnumerator WaitAndLoadScene()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(sceneToLoad);
    }
}
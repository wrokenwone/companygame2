using UnityEngine;
using UnityEngine.SceneManagement;

public class PCInteract : MonoBehaviour
{
    public GameObject ePrompt;
    public GameObject pcPanel;
    public GameObject nextDayPanel;
    public GameObject player;
    public Sprite sittingSprite;
    private Sprite originalSprite;
    private SpriteRenderer playerSR;
    private bool playerNearby = false;
    private bool applied = false;

    private void Start()
    {
        playerSR = player.GetComponent<SpriteRenderer>();
        originalSprite = playerSR.sprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            ePrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            if (ePrompt != null) ePrompt.SetActive(false);
            if (playerSR != null) playerSR.sprite = originalSprite;
        }
    }

   private void Update()
{
    if (this == null || !gameObject.activeInHierarchy) return;
    
    if (playerNearby && Input.GetKeyDown(KeyCode.E) && !applied)
    {
        player.GetComponent<YSort>().enabled = false;
        playerSR.sortingOrder = 3;
        player.transform.position = new Vector3(4.54f, -1.45f, 0f);
        playerSR.sprite = sittingSprite;
        pcPanel.SetActive(true);
    }
}

    public void Apply()
    {
        pcPanel.SetActive(false);
        applied = true;
        nextDayPanel.SetActive(true);
        Invoke("GoToOffice", 2f);
    }

    private void GoToOffice()
    {
        SceneManager.LoadScene("Office");

    }
    private void OnDestroy()
{
    CancelInvoke();
}
}
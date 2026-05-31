using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;

    [Tooltip("Drag a full-screen black UI Image here")]
    public Image fadePanel;
    public float fadeDuration = 0.5f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // Use this to OPEN a minigame
    public void FadeToPanel(GameObject targetPanel)
    {
        StartCoroutine(FadeRoutine(targetPanel, true));
    }

    // Use this to CLOSE a minigame, and run code after the fade finishes
    public void FadeClosePanel(GameObject targetPanel, System.Action onComplete = null)
    {
        StartCoroutine(FadeRoutine(targetPanel, false, onComplete));
    }

    private IEnumerator FadeRoutine(GameObject panel, bool opening, System.Action onComplete = null)
    {
        fadePanel.gameObject.SetActive(true);
        fadePanel.transform.SetAsLastSibling(); // Forces the black screen to the very front

        // 1. Fade to Black
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadePanel.color = new Color(0, 0, 0, t / fadeDuration);
            yield return null;
        }

        // 2. Turn the Minigame Panel ON or OFF while the screen is pitch black
        if (panel != null) panel.SetActive(opening);

        // 3. Fade back to Clear
        t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadePanel.color = new Color(0, 0, 0, Mathf.SmoothStep(1, 0, t / fadeDuration));
            yield return null;
        }

        fadePanel.color = new Color(0, 0, 0, 0);
        fadePanel.gameObject.SetActive(false);

        // 4. Tell the minigame that the transition is completely finished
        onComplete?.Invoke();
    }
}
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeEffect : MonoBehaviour
{
    public Image fadePanel;
    public GameObject pcScreenPanel;
    public float fadeDuration = 1f;

    public void FadeToPC()
    {
        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        // Kararma
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadePanel.color = new Color(0, 0, 0, t / fadeDuration);
            yield return null;
        }

        // PC ekranı aç
        pcScreenPanel.SetActive(true);
    fadePanel.transform.SetAsLastSibling();

        // Aydınlanma
        t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadePanel.color = new Color(0, 0, 0, Mathf.SmoothStep(1, 0, t / fadeDuration));
            yield return null;
        }

        fadePanel.color = new Color(0, 0, 0, 0);
fadePanel.gameObject.SetActive(false);
    }
}
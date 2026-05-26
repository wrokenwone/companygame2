using UnityEngine;
using UnityEngine.SceneManagement;

public class FiredScene : MonoBehaviour
{
    public float waitTime = 3f;
    private float timer;

    private void Start()
    {
        timer = waitTime;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            SceneManager.LoadScene("Glitch");
        }
    }
}
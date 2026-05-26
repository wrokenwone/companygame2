using UnityEngine;

public class YSort : MonoBehaviour
{
    private SpriteRenderer sr;
    public int offset = 0;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        sr.sortingOrder = (int)(-transform.position.y * 100) + offset;
    }
}
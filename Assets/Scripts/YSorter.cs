using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class YSorter : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [Tooltip("Check this if the object never moves (like a desk or wall) to save performance!")]
    public bool isStaticObject = false;

    [Tooltip("Add a little extra to the sorting order if needed")]
    public int sortingOffset = 0;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // If it's a desk or machine, sort it once and stop calculating to save memory
        if (isStaticObject)
        {
            UpdateSortingOrder();
        }
    }

    private void Update()
    {
        // If it's a character that walks around, constantly update their sorting order!
        if (!isStaticObject)
        {
            UpdateSortingOrder();
        }
    }

    private void UpdateSortingOrder()
    {
        // Multiply by -100 so that the lower you are on the Y axis, the higher your sorting number becomes!
        spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -100f) + sortingOffset;
    }
}
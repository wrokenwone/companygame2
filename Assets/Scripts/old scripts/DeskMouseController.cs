using UnityEngine;

public class DeskMouseController : MonoBehaviour
{
    public float followSpeed = 0.05f;
    public float movementScale = 0.002f;

    private Vector3 lastMousePos;

    private void Start()
    {
        lastMousePos = Input.mousePosition;
    }

    private void Update()
    {
        Vector3 delta = Input.mousePosition - lastMousePos;
        lastMousePos = Input.mousePosition;

        Vector3 targetPos = transform.position + new Vector3(delta.x, delta.y, 0) * movementScale;
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed);
    }
}
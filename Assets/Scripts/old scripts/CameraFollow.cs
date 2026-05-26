using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player;
    public float minX = -8.20f;
    public float maxX = 28.97f;

    private void Update()
    {
        if (player == null) return;
        float clampedX = Mathf.Clamp(player.transform.position.x, minX, maxX);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }
}
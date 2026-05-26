using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public Sprite[] walkFrames;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private float animTimer;
    private int currentFrame;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        rb.linearVelocity = new Vector2(x, y) * moveSpeed;

        if (x != 0 || y != 0)
        {
            animTimer += Time.deltaTime;
            if (animTimer > 0.2f)
            {
                animTimer = 0;
                currentFrame = (currentFrame + 1) % walkFrames.Length;
                sr.sprite = walkFrames[currentFrame];
            }

            if (x < 0) sr.flipX = true;
            else if (x > 0) sr.flipX = false;
        }
    }
}
using UnityEngine;

public class Controller : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public LayerMask groundLayer;
    private Rigidbody2D rb;
    private bool isGrounded;
    private Collider2D coll;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);
        isGrounded = Physics2D.IsTouchingLayers(coll, groundLayer);

        if (Input.GetButtonDown("Jump") && isGrounded)
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
    }
}
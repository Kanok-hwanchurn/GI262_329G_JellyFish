using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private StatSystem stats;

    private Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        stats = GetComponent<StatSystem>();
    }   

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        float x = 0f;
        float y = 0f;

        if (kb.wKey.isPressed || kb.upArrowKey.isPressed) y += 1f;
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed) y -= 1f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) x += 1f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) x -= 1f;

        moveInput = new Vector2(x, y).normalized;

        if (x > 0.01f)
            sprite.flipX = false;
        else if (x < -0.01f)
            sprite.flipX = true;

        if (kb.spaceKey.wasPressedThisFrame && stats != null)
        {
            stats.TakeDamage(3f);
        }
    }

    void FixedUpdate()
    {
        float speed = (stats != null) ? stats.moveSpeed : 5f;
        rb.linearVelocity = moveInput * speed;
    }
}

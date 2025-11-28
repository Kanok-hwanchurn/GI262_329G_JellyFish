using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private StatSystem stats;
    private Transform target;   

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>(); 
        stats = GetComponent<StatSystem>();
    }

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
    }

    void FixedUpdate()
    {
        if (target == null || stats == null || rb == null)
        {
            if (rb != null) rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 dir = ((Vector2)target.position - rb.position).normalized;

        rb.linearVelocity = dir * stats.moveSpeed;

        if (sprite != null)
        {
            if (dir.x > 0.01f)
                sprite.flipX = false;
            else if (dir.x < -0.01f)
                sprite.flipX = true;
        }
    }
}

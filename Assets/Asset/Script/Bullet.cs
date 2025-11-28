using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 3f;
    public float damage = 3f;

    private Vector2 direction;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Vector2 dir, float dmg)
    {
        direction = dir.normalized;
        damage = dmg;
        Destroy(gameObject, lifeTime);
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        rb.linearVelocity = direction * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            return;

        StatSystem targetStats = other.GetComponent<StatSystem>();

        if (targetStats != null)
        {
            targetStats.TakeDamage(damage);
            Destroy(gameObject);      
        }

    }
}

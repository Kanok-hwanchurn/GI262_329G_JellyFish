using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public float contactCooldown = 0.5f; 

    private StatSystem myStats;
    private float timer = 0f;

    void Awake()
    {
        myStats = GetComponent<StatSystem>();
    }

    void Update()
    {
        if (timer > 0f)
            timer -= Time.deltaTime;
    }


    void OnCollisionStay2D(Collision2D collision)
    {
        TryDoDamage(collision.gameObject);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        TryDoDamage(other.gameObject);
    }

    void TryDoDamage(GameObject other)
    {
        if (timer > 0f) return;

        StatSystem targetStats = other.GetComponent<StatSystem>();
        if (targetStats != null && !targetStats.isEnemy) 
        {
            float dmg = (myStats != null) ? myStats.attack : 1f;
            targetStats.TakeDamage(dmg);
            timer = contactCooldown;
        }
    }
}

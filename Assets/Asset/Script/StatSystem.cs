using UnityEngine;

public class StatSystem : MonoBehaviour
{
    [Header("Level / EXP")]
    public int level = 1;           
    public int currentXP = 0;       
    public int xpToNextLevel = 10;  
    [Header("Main Stats")]
    public float maxHealth = 10f;   
    public float regenHP = 0.5f;    
    public float attack = 2f;       
    public float defense = 0f;     
    public float moveSpeed = 5f;    

    [Header("Runtime")]
    public float currentHealth;     

    [Header("Enemy Settings")]
    public bool isEnemy = false;    
    public int xpReward = 5;        

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        RegenerateHealth();
    }

    void RegenerateHealth()
    {
        if (currentHealth <= 0f) return;
        if (currentHealth >= maxHealth) return;
        if (regenHP <= 0f) return;

        currentHealth += regenHP * Time.deltaTime;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        float finalDamage = Mathf.Max(damage - defense, 1f);

        currentHealth -= finalDamage;
        Debug.Log($"{gameObject.name} took {finalDamage} damage. HP = {currentHealth}/{maxHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} died");

        if (isEnemy)
        {
            if (xpReward > 0)
            {
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    StatSystem playerStats = player.GetComponent<StatSystem>();
                    if (playerStats != null)
                    {
                        playerStats.GainXP(xpReward);
                    }
                }
            }

            Destroy(gameObject);
        }
        else
        {
            GameManager gm = Object.FindFirstObjectByType<GameManager>();
            if (gm != null)
            {
                gm.PlayerDied();
            }

            Destroy(gameObject);
        }
    }

    public void GainXP(int amount)
    {
        currentXP += amount;
        Debug.Log($"{gameObject.name} gained {amount} XP. ({currentXP}/{xpToNextLevel})");

        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;

        maxHealth += 2f;
        attack += 1f;
        moveSpeed += 0.2f;
        regenHP += 0.1f;

        currentHealth = maxHealth; 

        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.3f);

        Debug.Log($"{gameObject.name} LEVEL UP! → Level {level}");

        if (!isEnemy)
        {
            LevelUpManager lm = Object.FindFirstObjectByType<LevelUpManager>();
            if (lm != null)
            {
                lm.OnPlayerLevelUp(this);
            }
        }
    }
}

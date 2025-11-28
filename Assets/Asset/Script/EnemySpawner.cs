using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy")]
    public GameObject enemyPrefab;

    [Header("Arena / Spawn Area")]
    public BoxCollider2D arenaBounds;      
    public float spawnRadius = 8f;         
    public float minDistanceFromPlayer = 3f;
    public float innerMargin = 0.5f;

    [Header("Wave Settings")]
    public int firstWaveCount = 5;         
    public int addPerWave = 3;             
    public float timeBetweenWaves = 3f;    
    public float spawnInterval = 0.3f;     

    [Header("Enemy Level Scaling (ตามเลเวลผู้เล่น)")]
    public int baseEnemyLevel = 1;                 
    public int playerLevelsPerEnemyLevel = 3;      
    public float hpPerEnemyLevelPercent = 0.3f;    
    public float attackPerEnemyLevelPercent = 0.2f;
    public float defensePerEnemyLevel = 0.2f;      
    public float moveSpeedPerEnemyLevel = 0.1f;    

    private Transform player;
    private StatSystem playerStats;

    private int currentWave = 0;
    private int enemiesToSpawn;
    private int enemiesSpawned;
    private int enemiesAlive;

    private float spawnTimer;
    private float waveBreakTimer;
    private bool waveRunning;

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerStats = playerObj.GetComponent<StatSystem>();
        }

        StartNextWave();
    }

    void Update()
    {
        if (player == null || enemyPrefab == null)
            return;

        if (waveRunning)
        {
            if (enemiesSpawned < enemiesToSpawn)
            {
                spawnTimer -= Time.deltaTime;
                if (spawnTimer <= 0f)
                {
                    SpawnEnemy();
                    spawnTimer = spawnInterval;
                }
            }

            if (enemiesSpawned >= enemiesToSpawn && enemiesAlive <= 0)
            {
                waveRunning = false;
                waveBreakTimer = timeBetweenWaves;
                Debug.Log($"Wave {currentWave} cleared!");
            }
        }
        else
        {
            waveBreakTimer -= Time.deltaTime;
            if (waveBreakTimer <= 0f)
            {
                StartNextWave();
            }
        }
    }

    void StartNextWave()
    {
        currentWave++;
        enemiesToSpawn = firstWaveCount + addPerWave * (currentWave - 1);
        enemiesSpawned = 0;
        enemiesAlive = 0;
        waveRunning = true;
        spawnTimer = 0f;

        Debug.Log($"Start Wave {currentWave} | To spawn: {enemiesToSpawn}");
    }

    void SpawnEnemy()
    {
        if (player == null) return;

        Vector2 spawnPos = GetSpawnPosition();
        GameObject enemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        enemiesSpawned++;
        enemiesAlive++;

        EnemyLifeHook hook = enemyObj.AddComponent<EnemyLifeHook>();
        hook.spawner = this;

        ApplyEnemyScaling(enemyObj);
    }

    Vector2 GetSpawnPosition()
    {
        if (arenaBounds != null)
        {
            Bounds b = arenaBounds.bounds;
            Vector2 result = player.position;

            int safety = 0;
            while (Vector2.Distance(result, player.position) < minDistanceFromPlayer && safety < 30)
            {
                float x = Random.Range(b.min.x + innerMargin, b.max.x - innerMargin);
                float y = Random.Range(b.min.y + innerMargin, b.max.y - innerMargin);
                result = new Vector2(x, y);
                safety++;
            }

            return result;
        }
        else
        {
            Vector2 dir = Random.insideUnitCircle.normalized;
            Vector2 pos = (Vector2)player.position + dir * spawnRadius;
            return pos;
        }
    }

    void ApplyEnemyScaling(GameObject enemyObj)
    {
        StatSystem enemyStats = enemyObj.GetComponent<StatSystem>();
        if (enemyStats == null)
            return;

        int pLevel = (playerStats != null) ? Mathf.Max(playerStats.level, 1) : 1;

        int enemyLevel = baseEnemyLevel;
        if (playerLevelsPerEnemyLevel > 0)
        {
            enemyLevel += Mathf.FloorToInt((pLevel - 1) / (float)playerLevelsPerEnemyLevel);
        }

        enemyStats.level = enemyLevel;

        int extraLevels = Mathf.Max(enemyLevel - baseEnemyLevel, 0);

        if (extraLevels > 0)
        {
            float hpMul = 1f + hpPerEnemyLevelPercent * extraLevels;
            float atkMul = 1f + attackPerEnemyLevelPercent * extraLevels;

            enemyStats.maxHealth *= hpMul;
            enemyStats.currentHealth = enemyStats.maxHealth;
            enemyStats.attack *= atkMul;
            enemyStats.defense += defensePerEnemyLevel * extraLevels;
            enemyStats.moveSpeed += moveSpeedPerEnemyLevel * extraLevels;
        }

        Debug.Log($"Spawn enemy Lvl {enemyLevel} (player lvl {pLevel})");
    }

    public void OnEnemyDied()
    {
        enemiesAlive--;
        if (enemiesAlive < 0)
            enemiesAlive = 0;
    }

    public int GetCurrentWave()
    {
        return currentWave;
    }
}

using UnityEngine;

public class EnemyLifeHook : MonoBehaviour
{
    [HideInInspector] public EnemySpawner spawner;

    void OnDestroy()
    {
        if (spawner != null && Application.isPlaying)
        {
            spawner.OnEnemyDied();
        }
    }
}

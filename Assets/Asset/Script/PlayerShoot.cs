using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;      
    public float fireRate = 4f;      

    private float fireCooldown = 0f;
    private StatSystem stats;

    void Awake()
    {
        stats = GetComponent<StatSystem>();
    }

    void Update()
    {
        fireCooldown -= Time.deltaTime;

        var mouse = Mouse.current;
        if (mouse == null) return;

        if (mouse.leftButton.isPressed && fireCooldown <= 0f)
        {
            ShootTowardMouse();
            fireCooldown = 1f / fireRate;
        }
    }

    void ShootTowardMouse()
    {
        if (bulletPrefab == null || firePoint == null) return;

        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.z = 0f;

        Vector2 dir = (worldPos - firePoint.position).normalized;

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            float dmg = (stats != null) ? stats.attack : 3f;
            bullet.Init(dir, dmg);
        }
    }
}

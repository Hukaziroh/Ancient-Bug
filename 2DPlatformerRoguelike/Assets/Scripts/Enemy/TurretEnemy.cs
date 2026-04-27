using UnityEngine;
using System.Collections;

public class TurretEnemy : MonoBehaviour
{
    [Header("발사 설정")]
    public Transform firePoint;

    [Header("스탯 설정")]
    public float fireRate = 2f;
    public float damage = 10f;

    [Header("사운드 설정")]
    public AudioClip shootSound;
    public float soundRange = 10f; 

    private Transform player;

    private void Awake()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    private void Start()
    {
        StartCoroutine(ShootRoutine());
    }

    private IEnumerator ShootRoutine()
    {
        while (true)
        {
            Shoot();
            yield return new WaitForSeconds(fireRate);
        }
    }

    private void Shoot()
    {
        if (firePoint == null) return;

        if (shootSound != null && SoundManager.Instance != null && player != null)
        {
            if (Vector2.Distance(transform.position, player.position) <= soundRange)
            {
                SoundManager.Instance.PlaySFX(shootSound);
            }
        }

        GameObject bulletObj = EnemyProjectilePool.Instance.feBulletPool.Get();
        bulletObj.transform.position = firePoint.position;
        bulletObj.transform.rotation = firePoint.rotation;
        Vector2 shootDir = firePoint.right;

        IProjectile projectile = bulletObj.GetComponent<IProjectile>();
        if (projectile != null)
        {
            projectile.Setup(shootDir, damage);
        }
    }
}
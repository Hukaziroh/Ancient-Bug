using UnityEngine;
using System.Collections;

public class TurretEnemy : MonoBehaviour
{
    [Header("발사 설정")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("스탯 설정")]
    public float fireRate = 2f;
    public float damage = 10f;

    private void Start()
    {
        StartCoroutine(ShootRoutine());
    }

    private IEnumerator ShootRoutine()
    {
        while(true)
        {
            Shoot();
            yield return new WaitForSeconds(fireRate);
        }
    }

    private void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject bulletObj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Vector2 shootDir = firePoint.right;
        
        IProjectile projectile = bulletObj.GetComponent<IProjectile>();
        if (projectile != null)
        {
            projectile.Setup(shootDir, damage);
        }
    }
}

using UnityEngine;
using UnityEngine.Pool;

public class EnemyProjectilePool : MonoBehaviour
{
    public static EnemyProjectilePool Instance;

    [Header("투사체 프리팹")]
    public GameObject spearPrefab;
    public GameObject feBulletPrefab;
    public GameObject frBulletPrefab;
    public GameObject boss2BulletPrefab; 

    public ObjectPool<GameObject> spearPool;
    public ObjectPool<GameObject> feBulletPool;
    public ObjectPool<GameObject> frBulletPool;
    public ObjectPool<GameObject> boss2BulletPool; 

    private void Awake()
    {
        Instance = this;

        spearPool = new ObjectPool<GameObject>(() => CreateObj(spearPrefab, spearPool), OnGet, OnRelease, OnDestroyObj);
        feBulletPool = new ObjectPool<GameObject>(() => CreateObj(feBulletPrefab, feBulletPool), OnGet, OnRelease, OnDestroyObj);
        frBulletPool = new ObjectPool<GameObject>(() => CreateObj(frBulletPrefab, frBulletPool), OnGet, OnRelease, OnDestroyObj);
        boss2BulletPool = new ObjectPool<GameObject>(() => CreateObj(boss2BulletPrefab, boss2BulletPool), OnGet, OnRelease, OnDestroyObj);
    }

    private GameObject CreateObj(GameObject prefab, ObjectPool<GameObject> pool)
    {
        GameObject obj = Instantiate(prefab, transform);
        IProjectile proj = obj.GetComponent<IProjectile>();
        if (proj != null) proj.SetManagedPool(pool);
        return obj;
    }

    private void OnGet(GameObject obj) => obj.SetActive(true);
    private void OnRelease(GameObject obj) => obj.SetActive(false);
    private void OnDestroyObj(GameObject obj) => Destroy(obj);

    public GameObject GetProjectile(GameObject requestedPrefab)
    {
        if (requestedPrefab == spearPrefab) return spearPool.Get();
        if (requestedPrefab == feBulletPrefab) return feBulletPool.Get();
        if (requestedPrefab == frBulletPrefab) return frBulletPool.Get();
        if (requestedPrefab == boss2BulletPrefab) return boss2BulletPool.Get();

        return null;
    }
}
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class EnemyProjectilePool : MonoBehaviour
{
    public static EnemyProjectilePool Instance;

    [Header("투사체 프리팹")]
    public GameObject spearPrefab;
    public GameObject feBulletPrefab;
    public GameObject frBulletPrefab;

    public ObjectPool<GameObject> spearPool;
    public ObjectPool<GameObject> feBulletPool;
    public ObjectPool<GameObject> frBulletPool;

    private void Awake()
    {
        Instance = this;

        spearPool = new ObjectPool<GameObject>(CreateSpear, OnGet, OnRelease, OnDestroyObj);
        feBulletPool = new ObjectPool<GameObject>(CreateFEBullet, OnGet, OnRelease, OnDestroyObj);
        frBulletPool = new ObjectPool<GameObject>(CreateFRBullet, OnGet, OnRelease, OnDestroyObj);
    }

    private GameObject CreateSpear()
    {
        GameObject obj = Instantiate(spearPrefab, transform);
        obj.GetComponent<Spear>().SetManagedPool(spearPool);
        return obj;
    }

    private GameObject CreateFEBullet()
    {
        GameObject obj = Instantiate(feBulletPrefab, transform);
        obj.GetComponent<FEBullet>().SetManagedPool(feBulletPool);
        return obj;
    }

    private GameObject CreateFRBullet()
    {
        GameObject obj = Instantiate(frBulletPrefab, transform);
        obj.GetComponent<FRBullet>().SetManagedPool(frBulletPool);
        return obj;
    }

    private void OnGet(GameObject obj) => obj.SetActive(true);
    private void OnRelease(GameObject obj) => obj.SetActive(false);
    private void OnDestroyObj(GameObject obj) => Destroy(obj);

    
}

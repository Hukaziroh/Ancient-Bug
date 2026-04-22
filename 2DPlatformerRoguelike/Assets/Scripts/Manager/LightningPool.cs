using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class LightningPool : MonoBehaviour
{
    public static LightningPool Instance;

    [Header("번개 설정")]
    public GameObject lightningPrefab;
    public int maxSize = 10;

    private ObjectPool<GameObject> pool;

    private void Awake()
    {
        Instance = this;

        pool = new ObjectPool<GameObject>(
            CreateLightning,
            OnGetLightning,
            OnReleaseLightning,
            OnDestroyLightning,
            maxSize: maxSize
        );
    }

    private GameObject CreateLightning()
    {
        return Instantiate(lightningPrefab, transform);
    }

    private void OnGetLightning(GameObject obj)
    {
        obj.SetActive(true);
    }

    private void OnReleaseLightning(GameObject obj)
    {
        obj.SetActive(false);
    }

    private void OnDestroyLightning(GameObject obj)
    {
        Destroy(obj);
    }


    public GameObject Get()
    {
        return pool.Get();
    }

    public void Release(GameObject obj)
    {
        pool.Release(obj);
    }
}
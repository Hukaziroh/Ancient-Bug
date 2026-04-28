using UnityEngine;

public class Lightning : MonoBehaviour
{
    private void OnEnable()
    {
        Invoke("ReturnToPool", 1.6f);
    }

    private void ReturnToPool()
    {
        if (LightningPool.Instance != null)
        {
            LightningPool.Instance.Release(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDisable()
    {
        CancelInvoke("ReturnToPool");
    }
}
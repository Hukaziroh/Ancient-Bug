using UnityEngine;
using UnityEngine.Pool;

public class Spear : MonoBehaviour, IProjectile
{
    [Header("≈ı√¥ º≥¡§")]
    public float throwPowerX = 8f;
    public float throwPowerY = 5f;
    public float lifetime = 3f;

    float damage;
    Rigidbody2D rb;

    private IObjectPool<GameObject> managedPool;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Setup(Vector2 moveDirection, float attackDamage)
    {
        damage = attackDamage;

        float dirX = moveDirection.x > 0 ? 1f : -1f;

        rb.linearVelocity = new Vector2(dirX * throwPowerX, throwPowerY);

        CancelInvoke("ReturnToPool");
        Invoke("ReturnToPool", lifetime);
    }

    private void Update()
    {
        if (rb.linearVelocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Player.Instance != null && collision.gameObject == Player.Instance.gameObject)
        {
            if (!Player.Instance.isInvincible)
            {
                Player.Instance.TakeDamage(damage);

                if (Player.Instance.TryGetComponent(out PlayerMovement movement))
                {
                    movement.ApplyKnockback(transform);
                }
            }
            ReturnToPool();
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            ReturnToPool();
        }
    }

    public void SetManagedPool(IObjectPool<GameObject> pool)
    {
        managedPool = pool;
    }

    private void ReturnToPool()
    {
        if (gameObject.activeSelf && managedPool != null)
        {
            managedPool.Release(gameObject);
        }
        else if (managedPool == null)
        {
            Destroy(gameObject);
        }
    }
}
using UnityEngine;
using UnityEngine.Pool;

public class FEBullet : MonoBehaviour, IProjectile
{
    [Header("È­¿°±¸ ¼³Á¤")]
    public float speed = 10f;
    public float lifetime = 7f;

    private float damage;
    private Rigidbody2D rb;
    private IObjectPool<GameObject> managedPool;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetManagedPool(IObjectPool<GameObject> pool)
    {
        managedPool = pool;
    }

    public void Setup(Vector2 moveDirection, float attackDamage)
    {
        damage = attackDamage;

        if (moveDirection.x < 0) transform.localScale = new Vector3(-1, 1, 1);
        else transform.localScale = new Vector3(1, 1, 1);

        rb.linearVelocity = moveDirection.normalized * speed;

        CancelInvoke("ReturnToPool");
        Invoke("ReturnToPool", lifetime);
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
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            ReturnToPool();
        }
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
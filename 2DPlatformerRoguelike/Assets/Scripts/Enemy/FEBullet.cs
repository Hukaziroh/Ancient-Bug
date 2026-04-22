using UnityEngine;
using UnityEngine.Pool;

public class FEBullet : MonoBehaviour, IProjectile
{
    [Header("È­¿°±¸ ¼³Á¤")]
    public float speed = 10f;      
    public float lifetime = 7f;

    float damage;
    Vector2 direction;
    Rigidbody2D rb;

    private IObjectPool<GameObject> managedPool;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Setup(Vector2 moveDirection, float attackDamage)
    {
        direction = moveDirection.normalized;
        damage = attackDamage;

        if (direction.x < 0) transform.localScale = new Vector3(-1, 1, 1);

        CancelInvoke("ReturnToPool");
        Invoke("ReturnToPool", lifetime);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null && player.isInvincible)
            {
                ReturnToPool();
                return;
            }

            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null) damageable.TakeDamage(damage);

            PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
            if (playerMovement != null) playerMovement.ApplyKnockback(transform);

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
    }
}

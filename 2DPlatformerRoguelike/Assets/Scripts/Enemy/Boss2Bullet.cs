using UnityEngine;
using UnityEngine.Pool;

public class Boss2Bullet : MonoBehaviour, IProjectile
{
    [Header("À¯µµÅº ¼³Á¤")]
    public float speed = 8f;
    public float rotationSpeed = 5f;
    public float lifetime = 3f;

    private float damage;
    private Transform playerTarget;
    private Rigidbody2D rb;
    private IObjectPool<GameObject> managedPool;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetManagedPool(IObjectPool<GameObject> pool) => managedPool = pool;

    public void Setup(Vector2 moveDirection, float attackDamage)
    {
        damage = attackDamage;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTarget = playerObj.transform;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        CancelInvoke("ReturnToPool");
        Invoke("ReturnToPool", lifetime);
    }

    private void ReturnToPool()
    {
        if (gameObject.activeSelf && managedPool != null)
            managedPool.Release(gameObject);
        else if (managedPool == null)
            Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if (playerTarget != null)
        {
            Vector2 direction = (Vector2)playerTarget.position - rb.position;
            direction.Normalize();
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
        rb.linearVelocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null && !player.isInvincible)
            {
                IDamageable damageable = collision.GetComponent<IDamageable>();
                if (damageable != null) damageable.TakeDamage(damage);
            }
            ReturnToPool();
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            ReturnToPool();
        }
    }
}
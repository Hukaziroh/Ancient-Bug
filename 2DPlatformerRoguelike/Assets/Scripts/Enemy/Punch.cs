using UnityEngine;

public class Punch : MonoBehaviour, IProjectile
{
    [Header("ÆÝÄ¡ ¼³Á¤")]  
    public float lifetime = 1f;

    float damage;
    Vector2 direction;
    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Setup(Vector2 moveDirection, float attackDamage)
    {
        direction = moveDirection.normalized;
        damage = attackDamage;

        if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
        Destroy(gameObject, lifetime);
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null && player.isInvincible)
            {
                Destroy(gameObject);
                return;
            }

            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null) damageable.TakeDamage(damage);

            PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
            if (playerMovement != null) playerMovement.ApplyKnockback(transform);

            Destroy(gameObject);
        }      
    }
}

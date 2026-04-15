using UnityEngine;

public class SpearProjectile : MonoBehaviour
{
    [Header("투척 설정")]
    public float throwPowerX = 8f;   
    public float throwPowerY = 5f;   
    public float lifetime = 3f;

    float damage;
    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Setup(Vector2 moveDirection, float attackDamage)
    {
        damage = attackDamage;
 
        float dirX = moveDirection.x > 0 ? 1f : -1f;

        rb.linearVelocity = new Vector2(dirX * throwPowerX, throwPowerY);

        Destroy(gameObject, lifetime);
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
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);

          
            // Destroy(gameObject) 대신 아래 3줄을 쓰면 됩니다.
            // rb.linearVelocity = Vector2.zero;
            // rb.gravityScale = 0f;
            // GetComponent<Collider2D>().enabled = false;
        }
    }
}
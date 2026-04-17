using UnityEngine;

public class FRBullet : MonoBehaviour, IProjectile
{
    [Header("브레스 설정")]
    public float lifetime = 2f; 
    float damage;

  
    public void Setup(Vector2 moveDirection, float attackDamage)
    {
        damage = attackDamage;
        Destroy(gameObject, lifetime);
    }

  
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null && player.isInvincible)
            {            
                return;
            }

            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null) damageable.TakeDamage(damage);

            PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
            if (playerMovement != null) playerMovement.ApplyKnockback(transform);

        }     
    }
}

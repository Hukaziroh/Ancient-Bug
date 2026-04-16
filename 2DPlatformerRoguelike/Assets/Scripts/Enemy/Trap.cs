using UnityEngine;

public class Trap : MonoBehaviour
{
    [Header("함정 데미지")]
    public float trapDamage = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ApplyTrapEffect(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        ApplyTrapEffect(collision);
    }

    private void ApplyTrapEffect(Collider2D collision)
    {
       
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null && player.isInvincible) return;

            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null) damageable.TakeDamage(trapDamage);

            PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
            if (playerMovement != null) playerMovement.ApplyKnockback(transform);
        }

       
    }
}
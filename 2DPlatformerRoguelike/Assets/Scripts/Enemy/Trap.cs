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
            if (collision.TryGetComponent(out Player player))
            {
                if (player.isInvincible) return; 

                player.TakeDamage(trapDamage); 

                if (collision.TryGetComponent(out PlayerMovement movement))
                {
                    movement.ApplyKnockback(transform);
                }
            }
        }
    }
}
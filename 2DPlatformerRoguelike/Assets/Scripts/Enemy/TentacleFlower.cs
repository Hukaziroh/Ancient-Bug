using UnityEngine;

public class TentacleFlower : MonoBehaviour
{
    [Header("ÃË¼ö ¼³Á¤")]
    public float damage = 10f;
    public Collider2D tentacleCollider;

    void Start()
    {
        if(tentacleCollider != null)
        {
            tentacleCollider.enabled = false;
        }
    }

    public void EnableTentacle()
    {
        if (tentacleCollider != null) tentacleCollider.enabled = true;
    }

    public void DisableTentacle()
    {
        if (tentacleCollider != null) tentacleCollider.enabled = false;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null && player.isInvincible) return;

            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null) damageable.TakeDamage(damage);

            PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
            if (playerMovement != null) playerMovement.ApplyKnockback(transform);
        }
    }
}

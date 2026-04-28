using UnityEngine;

public class TentacleFlower : MonoBehaviour
{
    [Header("촉수 설정")]
    public float damage = 10f;
    public Collider2D tentacleCollider;

    [Header("사운드 설정")]
    public AudioClip attackSound;
    public float soundRange = 10f;

    private Transform player;

    void Start()
    {
        if (Player.Instance != null) player = Player.Instance.transform;

        if (tentacleCollider != null)
        {
            tentacleCollider.enabled = false;
        }
    }

    public void EnableTentacle()
    {
        if (tentacleCollider != null) tentacleCollider.enabled = true;

        if (attackSound != null && SoundManager.Instance != null && player != null)
        {
            if (Vector2.Distance(transform.position, player.position) <= soundRange)
            {
                SoundManager.Instance.PlaySFX(attackSound);
            }
        }
    }

    public void DisableTentacle()
    {
        if (tentacleCollider != null) tentacleCollider.enabled = false;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player playerComponent = collision.GetComponent<Player>();
            if (playerComponent != null && playerComponent.isInvincible) return;

            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null) damageable.TakeDamage(damage);

            PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
            if (playerMovement != null) playerMovement.ApplyKnockback(transform);
        }
    }
}
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    [Header("스킬 설정")]
    public float speed = 10f;
    public float lifetime = 3f;

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

        if (direction.x < 0) transform.localScale = new Vector3(-2, 2, 1);

        Destroy(gameObject, lifetime);

    }
    private void FixedUpdate()
    {
        rb.linearVelocity = direction * speed;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
        }
    }
}

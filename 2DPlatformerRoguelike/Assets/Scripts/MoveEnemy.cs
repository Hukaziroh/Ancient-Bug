using UnityEngine;
using System.Collections;

public class MoveEnemy : MonoBehaviour, IDamageable
{
    [Header("스탯 및 설정")]
    public EnemyStatData statData;

    [Header("패트롤(순찰) 설정")]
    public Transform groundDetection; 
    public float rayDistance = 1f;   
    public LayerMask groundLayer;    

    private float currentHP;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private bool isDead = false;
    private bool movingRight = false;  

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (statData != null) currentHP = statData.maxHP;
    }

    private void Update()
    {
        if (isDead) return;     
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, rayDistance, groundLayer);  
        Vector2 forwardDirection = movingRight ? Vector2.right : Vector2.left;
        RaycastHit2D wallInfo = Physics2D.Raycast(groundDetection.position, forwardDirection, 0.1f, groundLayer);

        if (groundInfo.collider == null || wallInfo.collider != null)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        if (isDead) return;
    
        float direction = movingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * statData.moveSpeed, rb.linearVelocity.y);
    }

    private void Flip()
    {     
        movingRight = !movingRight;
       
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHP -= damage;
        StartCoroutine(HitFlashRoutine());

        if (currentHP <= 0) Die();
    }

    private IEnumerator HitFlashRoutine()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
        }
    }

    private void Die()
    {
        isDead = true;
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); 

        Collider2D coll = GetComponent<Collider2D>();
        if (coll != null) coll.enabled = false;

        rb.gravityScale = 0;
        Destroy(gameObject, 0.5f);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundDetection != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundDetection.position, groundDetection.position + Vector3.down * rayDistance);
        }
    }
}
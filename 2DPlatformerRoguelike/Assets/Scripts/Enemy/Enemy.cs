using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("스탯 및 설정")]
    public EnemyStatData statData;

    [Header("패트롤 설정")]
    public Transform groundDetection; 
    public float rayDistance = 1f;   
    public LayerMask groundLayer;

    [Header("공격 설정")]
    public float attackRange = 5f;      
    public Transform attackPoint;
    public GameObject projectilePrefab;

    [Header("보상 설정")]
    public int dropGold = 50;

    private float currentHP;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private Transform player;

    private bool isDead = false;
    private bool movingRight = false;
    private bool isAttacking = false;

   

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        if (statData != null) currentHP = statData.maxHP;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if(playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    private void Update()
    {
        if (isDead || isAttacking) return;   
        
        if(player != null && Vector2.Distance(transform.position,player.position) <= attackRange)
        {
            AttackPlayer();
            return;
        }
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, rayDistance, groundLayer);  
        Vector2 forwardDirection = movingRight ? Vector2.right : Vector2.left;
        RaycastHit2D wallInfo = Physics2D.Raycast(groundDetection.position, forwardDirection, 0.1f, groundLayer);

        if (groundInfo.collider == null || wallInfo.collider != null)
        {
            Flip();
        }
        if (anim != null) anim.SetBool("IsMoving", true);
    }

    private void FixedUpdate()
    {
        if (isDead) return;
    
        if(isAttacking)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

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

    void AttackPlayer()
    {
        isAttacking = true;

        if (anim != null)
        {
            anim.SetBool("IsMoving", false);
            anim.SetTrigger("Attack");
        }

        bool isPlayerOnRight = player.position.x > transform.position.x;
        if(isPlayerOnRight != movingRight)
        {
            Flip();
        }
        StartCoroutine(AttackCooldownRoutine());
    }

    public void PerformEnemyAttack()
    {
        if (attackPoint == null) return;

        GameObject projObj = EnemyProjectilePool.Instance.GetProjectile(projectilePrefab);

        if (projObj == null) return;

        projObj.transform.position = attackPoint.position;
        projObj.transform.rotation = Quaternion.identity;

        Vector2 throwDirection = movingRight ? Vector2.right : Vector2.left;
        IProjectile projectile = projObj.GetComponent<IProjectile>();

        if (projectile != null) projectile.Setup(throwDirection, statData.damage);

    }

    private IEnumerator AttackCooldownRoutine()
    {
        yield return new WaitForSeconds(1.5f); 
        isAttacking = false;

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
        if (isDead) return;
        isDead = true;

        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.OnEnemyKilled();
        }

        if (anim != null) anim.SetTrigger("Dead");
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        Collider2D[] allColliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D coll in allColliders)
        {
            coll.enabled = false;
        }

        rb.gravityScale = 0;

        GameObject activePlayer = GameObject.FindGameObjectWithTag("Player");

        if (activePlayer != null)
        {
            Player playerScript = activePlayer.GetComponent<Player>();
            if (playerScript != null)
            {             
                playerScript.AddGold(dropGold);
            }
        }
        Destroy(gameObject, 2f);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundDetection != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundDetection.position, groundDetection.position + Vector3.down * rayDistance);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (attackPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }

  
}
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("공격 설정")]
    public float AttackDamage = 5f;

    [Header("타격 판정 설정")]
    public Transform attackPoint;
    public Vector2 attackBoxSize = new Vector2(2f, 1f);
    public LayerMask enemyLayer;   

    Animator anim;
    Vector2 moveInput;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnAttack(InputValue value)
    {
        if (value.isPressed)
        {
            if (moveInput.y > 0.5f)
            {
                anim.SetTrigger("UpAttack");
            }
            else
            {
                anim.SetTrigger("Attack");
            }
        }
    }

    public void OnSkill(InputValue value)
    {
        if(value.isPressed)
        {
            anim.SetTrigger("IsSkill");
        }
    }

   
    public void PerformAttack()
    {
        if (attackPoint == null) return;

       
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPoint.position, attackBoxSize,0f, enemyLayer);

     
        foreach (Collider2D enemy in hitEnemies)
        {
           
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(AttackDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPoint.position, attackBoxSize);
    }
}

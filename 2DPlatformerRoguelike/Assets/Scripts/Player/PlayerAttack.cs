using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("스킬 UI")]
    public SkillCooldownUI skillUI;

    [Header("공격 설정")]
    public float AttackDamage = 5f;
    public GameObject FireBall;

    [Header("타격 판정 설정")]
    public Transform attackPoint;
    public Vector2 attackBoxSize = new Vector2(2f, 1f);
    public LayerMask enemyLayer;


    private bool movingRight = false;
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
        if (Time.timeScale == 0f) return;
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
        if (Time.timeScale == 0f) return;
        if (UIManager.Instance.skillUI.isCooldown) return;
          
        if (value.isPressed)
        {
            anim.SetTrigger("IsSkill");
          
            UIManager.Instance.skillUI.UseSkill(5f);
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

    
}

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("공격 설정")]
    public float AttackDamage = 5f;

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
}

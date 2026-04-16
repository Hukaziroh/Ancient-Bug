using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class Player : MonoBehaviour, IDamageable
{
    [Header("체력 설정")]
    public float maxHP = 100f;
    private float currentHP;

    [Header("피격 및 무적 설정")]
    public float invincibilityDuration = 2f; 
    public bool isInvincible { get; private set; }

    private Animator anim;
    private SpriteRenderer spriteRenderer;
    public bool IsDead { get; private set; }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float damage)
    {
        if (isInvincible || IsDead) return;

        PlayerMovement pm = GetComponent<PlayerMovement>();
        bool isDashing = (pm != null && pm.isDashing);

        if (isInvincible || IsDead || isDashing) return;
        currentHP -= damage;
        Debug.Log($"플레이어 피격! 남은 체력: {currentHP} (-{damage})");

        if (currentHP <= 0)
        {
            Die();
        }
        else
        {  
            StartCoroutine(InvincibilityRoutine());
        }
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;

        float timer = 0f;
        while (timer < invincibilityDuration)
        {   
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            yield return new WaitForSeconds(0.1f);

            spriteRenderer.color = new Color(1, 1, 1, 1f);
            yield return new WaitForSeconds(0.1f);

            timer += 0.2f; 
        }

        spriteRenderer.color = Color.white;
        isInvincible = false;
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;

        if (anim != null) anim.SetTrigger("Dead");
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<PlayerAttack>().enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        //SceneManager.LoadScene("Lobby");

        // TODO: 기획서에 명시된 대로 해당 회차 골드 소멸 및 로비(마을) 귀환 로직 추가 예정
    }
}

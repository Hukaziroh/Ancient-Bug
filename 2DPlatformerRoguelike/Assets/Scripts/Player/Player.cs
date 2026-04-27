using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IDamageable
{
    [Header("체력 설정")]
    public float maxHP = 200f;
    private float currentHP;

    [Header("보유 재화")]
    public int currentGold = 0;
    public int currentStone = 0;

    [Header("피격 및 무적 설정")]
    public float invincibilityDuration = 2f;
    public bool isInvincible { get; private set; }

    [Header("사운드 설정")]
    public AudioClip deathSound;

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
        UIManager.Instance.UpdateHp(currentHP, maxHP);
        UIManager.Instance.UpdateGold(currentGold);
        UIManager.Instance.UpdateStone(currentStone);
    }

    public void TakeDamage(float damage)
    {
        PlayerMovement pm = GetComponent<PlayerMovement>();
        bool isDashing = (pm != null && pm.isDashing);

        if (isInvincible || IsDead || isDashing) return;
        currentHP -= damage;
        UIManager.Instance.UpdateHp(currentHP, maxHP);

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

        if (deathSound != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(deathSound);
        }

        if (anim != null)
        {
            anim.SetTrigger("Dead");
            anim.SetBool("IsDead", true);
        }
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<PlayerAttack>().enabled = false;

        PlayerInput playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.enabled = false;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
        }
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        StartCoroutine(GoToLobbyRoutine());
    }

    private System.Collections.IEnumerator GoToLobbyRoutine()
    {
        yield return new WaitForSeconds(3f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        UIManager.Instance.UpdateGold(currentGold);
    }

    public void AddStone(int amount)
    {
        currentStone += amount;
        UIManager.Instance.UpdateStone(currentStone);
    }

    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            UIManager.Instance.UpdateGold(currentGold);
            return true;
        }
        return false;
    }

    public void Heal(float amount)
    {
        currentHP += amount;
        if (currentHP > maxHP) currentHP = maxHP;
        UIManager.Instance.UpdateHp(currentHP, maxHP);
    }

    public void IncreaseMaxHp(float amount)
    {
        maxHP += amount;
        currentHP += amount;
        UIManager.Instance.UpdateHp(currentHP, maxHP);
    }

    public void OnPause(InputValue value)
    {
        if (value.isPressed)
        {
            if (PauseManager.Instance != null)
            {
                PauseManager.Instance.TogglePause();
            }
        }
    }
}
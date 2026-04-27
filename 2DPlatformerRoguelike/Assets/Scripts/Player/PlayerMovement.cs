using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float fastFallSpeed = 20f;

    [Header("점프 설정")]
    public int maxJumpCount = 2;
    public float castDistance = 0.1f;
    private int currentJumpCount;
    public LayerMask groundLayer;

    [Header("대시 설정")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.05f;
    public int maxDashCount = 2;
    public float dashCooldown = 1f;
    int currentDashCount;
    Coroutine cooldownCoroutine;

    float defaultGravity;
    Coroutine dashCoroutine;

    [Header("활강 설정")]
    public float glideFallSpeed = 2f;
    private bool isJumpHolding;

    [Header("넉백 설정")]
    public float knockbackForce = 10f;
    public float knockbackUpForce = 5f;
    public float knockbackDuration = 0.2f;
    public bool isKnockbacked { get; private set; }
    public bool isDashing { get; private set; }

    [Header("단발성 사운드 설정")]
    public AudioClip jumpSound;
    public AudioClip doubleJumpSound;
    public AudioClip dashSound;

    [Header("지속성(루프) 사운드 설정")]
    public AudioClip walkSound;
    public AudioClip glideSound;
    public AudioSource loopAudioSource; 

    Rigidbody2D rb;
    CapsuleCollider2D capsuleCollider;
    Animator anim;

    Vector2 moveInput;
    bool isGrounded;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        defaultGravity = rb.gravityScale;
        currentDashCount = maxDashCount;
    }

    private void Update()
    {
        if (isDashing || isKnockbacked)
        {
            StopLoopSound();
            return;
        }

        isGrounded = CheckGrounded();

        if (isGrounded && rb.linearVelocity.y <= 0.1f)
        {
            currentJumpCount = maxJumpCount;
        }

        if (!isGrounded && moveInput.y < -0.5f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -fastFallSpeed);
        }
        else if (!isGrounded && rb.linearVelocity.y < 0f && isJumpHolding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -glideFallSpeed));
        }

        bool isMoving = Mathf.Abs(moveInput.x) > 0f;
        if (anim != null)
        {
            anim.SetBool("IsMoving", isMoving);
        }

        anim.SetBool("IsGrounded", isGrounded);
        anim.SetFloat("VelocityY", rb.linearVelocity.y);

        bool isGliding = !isGrounded && rb.linearVelocity.y < 0f && isJumpHolding;
        bool isWalking = isGrounded && isMoving;

        if (isGliding)
        {
            PlayLoopSound(glideSound);
        }
        else if (isWalking)
        {
            PlayLoopSound(walkSound);
        }
        else
        {
            StopLoopSound();
        }
    }

    private void PlayLoopSound(AudioClip clip)
    {
        if (loopAudioSource == null || clip == null) return;

        if (loopAudioSource.clip == clip && loopAudioSource.isPlaying) return;

        loopAudioSource.clip = clip;
        loopAudioSource.loop = true;
        loopAudioSource.Play();
    }

    private void StopLoopSound()
    {
        if (loopAudioSource != null && loopAudioSource.isPlaying)
        {
            loopAudioSource.Stop();
        }
    }

    private bool CheckGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(capsuleCollider.bounds.center, capsuleCollider.bounds.size, 0f, Vector2.down, castDistance, groundLayer);
        return raycastHit.collider != null;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (Time.timeScale == 0f) return;
        if (isDashing) return;

        isJumpHolding = value.isPressed;

        if (value.isPressed && currentJumpCount > 0 && moveInput.y > -0.5f)
        {
            if (currentJumpCount == maxJumpCount)
            {
                if (jumpSound != null && SoundManager.Instance != null) SoundManager.Instance.PlaySFX(jumpSound);
            }
            else
            {
                if (doubleJumpSound != null && SoundManager.Instance != null) SoundManager.Instance.PlaySFX(doubleJumpSound);
            }

            currentJumpCount--;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    public void OnDash(InputValue value)
    {
        if (Time.timeScale == 0f) return;
        if (value.isPressed && currentDashCount > 0)
        {
            currentDashCount--;
            UIManager.Instance.dashSkillUI.UseSkill(dashCooldown);

            if (dashSound != null && SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySFX(dashSound);
            }

            if (dashCoroutine != null)
            {
                StopCoroutine(dashCoroutine);
                rb.gravityScale = defaultGravity;
                rb.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
            }
            dashCoroutine = StartCoroutine(DashRoutine());

            if (cooldownCoroutine != null)
            {
                StopCoroutine(cooldownCoroutine);
            }
            cooldownCoroutine = StartCoroutine(DashCooldownRoutine());
        }
    }

    public void ApplyKnockback(Transform attacker)
    {
        if (isKnockbacked || isDashing) return;
        Player playerInfo = GetComponent<Player>();
        if (playerInfo != null && playerInfo.IsDead) return;
        StartCoroutine(KnockbackRoutine(attacker));
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;
        if (anim != null) anim.SetBool("IsDashing", true);
        rb.gravityScale = 0f;
        rb.constraints |= RigidbodyConstraints2D.FreezePositionY;
        rb.linearVelocity = new Vector2(transform.localScale.x * dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = defaultGravity;
        rb.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
        isDashing = false;
        if (anim != null) anim.SetBool("IsDashing", false);
        dashCoroutine = null;
    }

    private IEnumerator DashCooldownRoutine()
    {
        yield return new WaitForSeconds(dashCooldown);

        currentDashCount = maxDashCount;
        cooldownCoroutine = null;
    }

    private IEnumerator KnockbackRoutine(Transform attacker)
    {
        isKnockbacked = true;

        float direction = transform.position.x < attacker.position.x ? -1f : 1f;
        rb.linearVelocity = Vector2.zero;
        rb.linearVelocity = new Vector2(direction * knockbackForce, knockbackUpForce);
        yield return new WaitForSeconds(knockbackDuration);

        isKnockbacked = false;
    }

    private void FixedUpdate()
    {
        if (isDashing || isKnockbacked) return;

        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        if (moveInput.x > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput.x < 0) transform.localScale = new Vector3(-1, 1, 1);
    }
}
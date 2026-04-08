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
    bool isDashing;
    float defaultGravity; 
    Coroutine dashCoroutine;

    Rigidbody2D rb;
    BoxCollider2D boxCollider;
    Animator anim;

    Vector2 moveInput;
    bool isGrounded;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        defaultGravity = rb.gravityScale;
        currentDashCount = maxDashCount;
    }

    private void Update()
    {
        if (isDashing) return;

        isGrounded = CheckGrounded();

        if (isGrounded && rb.linearVelocity.y <= 0.1f)
        {
            currentJumpCount = maxJumpCount;
        }

        if (!isGrounded && moveInput.y < -0.5f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -fastFallSpeed);
        }

        if (anim != null)
        {         
            bool isMoving = Mathf.Abs(moveInput.x) > 0f;
            anim.SetBool("IsMoving", isMoving);
        }

        anim.SetBool("IsGrounded", isGrounded);
        anim.SetFloat("VelocityY", rb.linearVelocity.y);
    }

    private bool CheckGrounded()
    {     
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, castDistance, groundLayer);    
        Color rayColor = raycastHit.collider != null ? Color.green : Color.red;
        Debug.DrawRay(boxCollider.bounds.center + new Vector3(boxCollider.bounds.extents.x, 0), Vector2.down * (boxCollider.bounds.extents.y + castDistance), rayColor);

        return raycastHit.collider != null;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (isDashing) return;

        if (value.isPressed && currentJumpCount > 0 && moveInput.y > -0.5f)
        {
            currentJumpCount--;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    public void OnDash(InputValue value)
    {
        if (value.isPressed && currentDashCount > 0)
        {
            currentDashCount--; 
         
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

    private void FixedUpdate()
    {
        if (isDashing) return;

        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        if (moveInput.x > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput.x < 0) transform.localScale = new Vector3(-1, 1, 1);
    }
}
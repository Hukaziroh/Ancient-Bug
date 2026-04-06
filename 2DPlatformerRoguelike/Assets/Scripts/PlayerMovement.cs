using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("입력 설정")]
    [SerializeField] InputAction moveAction;
    [SerializeField] InputAction jumpAction;

    [Header("이동 및 점프 설정")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f; 

    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
    }

    private void Update()
    {     
        if (jumpAction.triggered)
        {          
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
          
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void FixedUpdate()
    {      
        float moveInput = moveAction.ReadValue<float>();
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);
    }
}
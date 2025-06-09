using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("이동")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float rotationSpeed = 10f;

    [Header("점프/중력")]
    public float jumpHeight = 1.2f;
    public float gravity = -9.81f;

    [Header("참조")]
    public Transform cameraTransform;
    public Animator animator;
    
    private CharacterController controller;
    private Vector3 velocity;
    private Vector2 moveInput;

    private bool isRunning;
    private bool isJumping;
    private bool isWalking;
    private bool isHoeing;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleInput();
        ApplyGravity();
        
        var state = animator.GetCurrentAnimatorStateInfo(0);
        if (state.IsName("Hoe")) return;
        
        UpdateAnimation();
        
        if (isWalking)
        {
            float speed = GetCurrentSpeed();
            Move(speed);
        }
        
        if (controller.isGrounded && isJumping)
        {
            Jump();
        }
    }
    
    void HandleInput()
    {
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        isWalking = moveInput.sqrMagnitude >= 0.01f;
        isRunning = Input.GetKey(KeyCode.LeftShift);
        isJumping = Input.GetKeyDown(KeyCode.Space);
        isHoeing = Input.GetMouseButtonDown(0);
    }
    void UpdateAnimation()
    {
        if (moveInput.sqrMagnitude >= 0.01f)
            animator.SetBool("Walk", true);
        else
            animator.SetBool("Walk", false);
        if (isHoeing)
            animator.SetTrigger("Hoe");
    }
    

    float GetCurrentSpeed()
    {
        if (isRunning) return runSpeed;
        else return walkSpeed;
    }

    void Move(float speed)
    {

        // 카메라 기준 방향 구하기
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // 입력을 카메라 기준 방향으로 변환
        Vector3 moveDir = forward * moveInput.y + right * moveInput.x;
        moveDir.Normalize();

        // 이동
        controller.Move(moveDir * speed * Time.deltaTime);

        // 회전: 이동 방향 기준으로 카메라 방향을 따름
        if (moveDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }




    void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        controller.Move(Vector3.up * velocity.y * Time.deltaTime);
    }
}
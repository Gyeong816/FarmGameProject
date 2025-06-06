using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("이동 관련")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float rotationSpeed = 10f;

    [Header("점프/중력")]
    public float jumpHeight = 1.2f;
    public float gravity = -9.81f;

    [Header("참조")]
    public Transform cameraTransform;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector2 moveInput;

    private bool isRunning;
    private bool isJumping;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleInput();
        ApplyGravity();

        float speed = GetCurrentSpeed();
        Move(speed);

        if (controller.isGrounded && isJumping)
        {
            Jump();
        }
    }

    void LateUpdate()
    {
        RotateToMoveDirection();
    }

    void HandleInput()
    {
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        isRunning = Input.GetKey(KeyCode.LeftShift);
        isJumping = Input.GetKeyDown(KeyCode.Space);


    }

    float GetCurrentSpeed()
    {
        if (isRunning) return runSpeed;
        return walkSpeed;
    }

    void Move(float speed)
    {
        if (moveInput.sqrMagnitude <= 0.01f) return;

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 move = forward * moveInput.y + right * moveInput.x;
        controller.Move(move.normalized * speed * Time.deltaTime);
    }

    void RotateToMoveDirection()
    {
        if (moveInput.sqrMagnitude <= 0.01f) return;

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 direction = forward * moveInput.y + right * moveInput.x;
        if (direction.sqrMagnitude < 0.01f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
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

using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("이동")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runSpeed = 7f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("점프/중력")]
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -9.81f;

    [Header("도구 사용 시간")]
    [SerializeField] private float hoeingTime = 0.7f;
    [SerializeField] private float wateringTime = 0.6f;
    [SerializeField] private float plantingTime = 0.6f;
    [SerializeField] private float toolDistance = 1.5f;
    
    
    [Header("참조")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private MapManager mapManager;
    [SerializeField] private float tileSize = 2f;
    
    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private Vector2 moveInput;

    private bool isRunning;
    private bool isJumping;
    private bool isWalking;
    private bool isHoeing;
    private bool isWatering;
    private bool isPlanting;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        HandleInput();
        ApplyGravity();
        UpdateAnimation();
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        if (state.IsName("Hoe") && state.normalizedTime >= hoeingTime && state.normalizedTime < 1f)
        {
            HoeInFront();
        }
        if (state.IsName("Water") && state.normalizedTime >= wateringTime && state.normalizedTime < 1f)
        {
            WaterInFront();
        }
        if (state.IsName("Plant") && state.normalizedTime >= plantingTime && state.normalizedTime < 1f)
        {
            PlantInFront();
        }
        if (state.IsName("Hoe") || state.IsName("Water") || state.IsName("Plant")) return;
        
        
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
        isWatering = Input.GetKeyDown(KeyCode.E);
        isPlanting = Input.GetMouseButtonDown(1);
         
    }
    void UpdateAnimation()
    {
        if (moveInput.sqrMagnitude >= 0.01f)
            animator.SetBool("Walk", true);
        else
            animator.SetBool("Walk", false);
        if(isHoeing) 
            animator.SetTrigger("Hoe");
        if(isWatering)
            animator.SetTrigger("Water");
        if (isPlanting)
            animator.SetTrigger("Plant");
        
            
    }
    
    private void HoeInFront()
    {
        Debug.Log("Hoe");
        var checkPos = transform.position + transform.forward * toolDistance;
        
        var tile = mapManager.GetTileAtWorldPos(checkPos);
        if (tile != null)
            tile.Hoe();
    }
    private void WaterInFront()
    {
        Debug.Log("Water");
        var checkPos = transform.position + transform.forward * toolDistance;
        
        var tile = mapManager.GetTileAtWorldPos(checkPos);
        if (tile != null) 
            tile.Water();
    }
    private void PlantInFront()
    {
        Debug.Log("Plant");
        var checkPos = transform.position + transform.forward * toolDistance;
        
        var tile = mapManager.GetTileAtWorldPos(checkPos);
        if (tile != null)
            tile.Plant();
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
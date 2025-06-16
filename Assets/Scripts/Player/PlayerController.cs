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
    [SerializeField] private float usingTime = 0.7f;
    [SerializeField] private float toolDistance = 1.5f;
    
    
    [Header("참조")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private MapManager mapManager;
    
    [Header("도구 오브젝트")]
    [SerializeField] private GameObject hoeObj;
    [SerializeField] private GameObject wateringPotObj;
    [SerializeField] private GameObject sickleObj;
    [SerializeField] private GameObject hammerObj;
    [SerializeField] private GameObject hammerCol;
    
    private LandTile lastHighlighted; 
    public ItemType currentItem = ItemType.None;
    public int itemId;
    public int seedId;
    
    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private Vector2 moveInput;

    private bool isRunning;
    private bool isJumping;
    private bool isHoldingTool;

    private bool isPerformingAction;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        
        HandleInput();
        ApplyGravity();
        UpdateAnimation();
        ShowPointerInFront();
        ShowFenceInFront();
        if (!isPerformingAction)
        { 
            if (state.IsName("Hoe") && state.normalizedTime >= usingTime && state.normalizedTime < 1f)
            {
                HoeInFront();
                isPerformingAction =  true;
            }
            if (state.IsName("Water") && state.normalizedTime >= usingTime && state.normalizedTime < 1f)
            {
                WaterInFront();
                isPerformingAction =  true;
            }
            if (state.IsName("Plant") && state.normalizedTime >= usingTime && state.normalizedTime < 1f)
            {
                PlantInFront();
                isPerformingAction =  true;
            }
            if (state.IsName("Hammer") && state.normalizedTime >= usingTime && state.normalizedTime < 1f)
            {
                hammerCol.SetActive(true);
                isPerformingAction =  true;
            }
            if (state.IsName("Harvest") && state.normalizedTime >= usingTime && state.normalizedTime < 1f)
            {
                HarvestInFront();
                isPerformingAction =  true;
            }
            if (state.IsName("Build") && state.normalizedTime >= usingTime && state.normalizedTime < 1f)
            {
                BuildInFront();
                isPerformingAction =  true;
            }
            
        }
        if (state.IsName("Hoe") || state.IsName("Water") || state.IsName("Hammer") ||
            state.IsName("Plant") || state.IsName("Harvest") || state.IsName("Build"))
        {
            return;
        }
        else
        {
            hammerCol.SetActive(false);
            isPerformingAction =  false;
        }
        
        
        float speed = GetCurrentSpeed();
        Move(speed);
        if (controller.isGrounded && isJumping)
        {
            Jump();
        }
    }
    
    private void HandleInput()
    {
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        isRunning = Input.GetKey(KeyCode.LeftShift);
        isJumping = Input.GetKeyDown(KeyCode.Space);
        if (Input.GetMouseButtonDown(0))
        { 
            UseItem();
        }
         

    }
    private void UpdateAnimation()
    {
        if (moveInput.sqrMagnitude >= 0.01f)
            animator.SetBool("Walk", true);
        else
            animator.SetBool("Walk", false);
    }

    public void UseItem()
    {
        switch (currentItem)
        {
            case ItemType.Hoe:
                animator.SetTrigger("Hoe"); 
                break;
            case ItemType.WateringPot:
                animator.SetTrigger("Water"); 
                break;
            case ItemType.Sickle: 
                animator.SetTrigger("Harvest"); 
                break;
            case ItemType.Hammer: 
                animator.SetTrigger("Hammer"); 
                break;
            case ItemType.Seed: 
                 animator.SetTrigger("Plant"); 
                break;
            case ItemType.Crop: 
               // animator.SetTrigger("Eat"); 
                break;
            case ItemType.Fence: 
                animator.SetTrigger("Build"); 
                break;
            case ItemType.None:
                break;
            default:
                break;
        }
    }
    
    public void SetItem()
    {
        hoeObj.SetActive(false);
        wateringPotObj.SetActive(false);
        sickleObj.SetActive(false);
        hammerObj.SetActive(false);
        
        switch (currentItem)
        {
            case ItemType.Hoe:
                hoeObj.SetActive(true);
                isHoldingTool = true;
                break;
            case ItemType.WateringPot:
                isHoldingTool = true;
                wateringPotObj.SetActive(true);
                break;
            case ItemType.Sickle:
                isHoldingTool = true;
                sickleObj.SetActive(true);
                break;
            case ItemType.Hammer:
                isHoldingTool = false;
                hammerObj.SetActive(true);
                break;
            case ItemType.Seed:
                isHoldingTool = true;
                break;
            case ItemType.Crop:
                isHoldingTool = false;
                break;
            case ItemType.Fence:
                isHoldingTool = false;
                break;
            case ItemType.None:
                isHoldingTool = false;
                break;
            default:
                break;
        }
    }
    
    
    private void HoeInFront()
    {
        var checkPos = transform.position + transform.forward * toolDistance;
        
        var tile = mapManager.GetTileAtWorldPos(checkPos);
        if (tile != null)
            tile.Hoe();
    }
    private void WaterInFront()
    {
        var checkPos = transform.position + transform.forward * toolDistance;
        
        var tile = mapManager.GetTileAtWorldPos(checkPos);
        if (tile != null) 
            tile.Water();
    }
    private void PlantInFront()
    {
        var checkPos = transform.position + transform.forward * toolDistance;
        var tile = mapManager.GetTileAtWorldPos(checkPos);
        mapManager.PlantCropAt(tile, seedId, itemId);
    } 
    private void BuildInFront()
    {
        var checkPos = transform.position + transform.forward * toolDistance;
        var tile = mapManager.GetTileAtWorldPos(checkPos);
        mapManager.BuildFenceAt(tile, mapManager.PreviewRotationY);
    }
    private void HarvestInFront()
    {
        var checkPos = transform.position + transform.forward * toolDistance;
        
        var tile = mapManager.GetTileAtWorldPos(checkPos);
        mapManager.HarvestCropAt(tile);
    }
    private void ShowPointerInFront()
    {
        if (!isHoldingTool)
        {
            if(lastHighlighted !=null)
             lastHighlighted.HideSelection();
            return;
        }
        
        var checkPos = transform.position + transform.forward * toolDistance;
        var tile = mapManager.GetTileAtWorldPos(checkPos);
        if (tile != null && tile != lastHighlighted)
        {
            if(lastHighlighted ==null)
                lastHighlighted = tile;
            lastHighlighted.HideSelection();
            tile.ShowSelection();
            lastHighlighted = tile; 
        }
        
    }

    private void ShowFenceInFront()
    {
        if (currentItem == ItemType.Fence)
        {
            var checkPos = transform.position + transform.forward * toolDistance;
            var tile = mapManager.GetTileAtWorldPos(checkPos);

            mapManager.ShowFencePreview(tile);

            if (Input.GetKeyDown(KeyCode.R))
                mapManager.RotateFencePreview();
        }
        else
        {
            mapManager.HideFencePreview();
        }
    }
    float GetCurrentSpeed()
    {
        if (isRunning) return runSpeed;
        else return walkSpeed;
    }

    void Move(float speed)
    {
        Vector3 forward = cameraTransform.forward; forward.y = 0; forward.Normalize();
        Vector3 right   = cameraTransform.right;   right.y   = 0; right.Normalize();
        Vector3 moveDir = forward * moveInput.y + right * moveInput.x;
        moveDir.Normalize();
        
        Vector3 horizontal = moveDir * speed * Time.deltaTime;
        Vector3 vertical   = Vector3.up * velocity.y * Time.deltaTime;
        controller.Move(horizontal + vertical);
        
        if (moveDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation,
                targetRotation,
                Time.deltaTime * rotationSpeed);
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
    }
}
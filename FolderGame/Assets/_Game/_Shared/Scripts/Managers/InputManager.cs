using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    
    [Header("Input Actions")]
    
    public bool PressInput { get; private set; }
    public bool HoldInput { get; private set; }
    public bool ReleaseInput { get; private set; }
    public bool SelectInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public Vector2 MoveInput { get; private set; }
    public bool JumpInput { get; private set; }
    public bool SprintInput { get; private set; }
    public bool CrouchInput { get; private set; }
    public bool InteractInput { get; private set; }
    public bool AttackInput { get; private set; }
    public bool PauseInput { get; private set; }
    
    private PlayerInput playerInput;
    
    private InputAction pressAction;
    private InputAction releaseAction;
    private InputAction selectAction;
    
    private InputAction lookAction;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction crouchAction;
    private InputAction interactAction;
    private InputAction attackAction;
    private InputAction pauseAction;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        
        playerInput = GetComponent<PlayerInput>();
        
        SetupInputActions();
    }

    private void SetupInputActions()
    {
        pressAction = playerInput.actions["Press"];
        selectAction = playerInput.actions["Select"];
        
        pauseAction = playerInput.actions["Pause"];
        
        lookAction = playerInput.actions["Look"];
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        sprintAction = playerInput.actions["Sprint"];
        crouchAction = playerInput.actions["Crouch"];
        interactAction = playerInput.actions["Interact"];
        attackAction = playerInput.actions["Attack"];
    }

    private void Update()
    {
        UpdateInputs();
    }

    private void UpdateInputs()
    {
        PressInput = pressAction.WasPressedThisFrame();
        HoldInput = pressAction.IsPressed();
        ReleaseInput = pressAction.WasReleasedThisFrame();
        
        SelectInput = selectAction.WasPressedThisFrame();
        
        PauseInput = pauseAction.WasPressedThisFrame();

        if (Mathf.Approximately(Time.timeScale, 0)) return;
        
        LookInput = lookAction.ReadValue<Vector2>();
        MoveInput = moveAction.ReadValue<Vector2>();
        
        JumpInput = jumpAction.WasPressedThisFrame();
        
        SprintInput = sprintAction.IsPressed();
        CrouchInput = crouchAction.IsPressed();
        
        InteractInput = interactAction.WasPressedThisFrame();
        AttackInput = attackAction.WasPressedThisFrame();
    }
}

using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float groundAcceleration = 35f;
    [SerializeField] private float groundDeceleration = 45f;
    [SerializeField] private float airAcceleration = 10f;
    
    [Header("Jump")]
    [SerializeField] private float coyoteTime = 0.2f;
    private float coyoteTimer;
    
    // [Header("Landing")]

    [Header("Ground Check")]
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform ceilingCheck;
    [SerializeField] private LayerMask groundLayer;
    
    [Header("Soft Lock")]
    [SerializeField] private float maxLockTime = 10f;

    private PlayerController playerController;
    private PlayerStats playerStats;
    
    private Rigidbody2D rb;
    private Vector2 moveInput;
    
    private bool isSprinting;
    private bool isCroucing;
    private bool isJumping;
    
    private bool isGrounded;
    private bool wasGrounded;
    private bool isCeiling;
    
    private bool isKnockbacked;

    private float timeSinceLastJump;
    private float timeSinceLastMove;

    private Coroutine knockbackRoutine;
    private Coroutine invulnerabilityRoutine;

    public bool IsCrouching => isCroucing;
    public bool IsSprinting => isSprinting;
    public bool IsGrounded => isGrounded;
    public bool IsJumping => isJumping;
    
    public float CurrentSpeed => new Vector2(rb.linearVelocity.x, 0f).magnitude;
    
    public event Action OnJumpStart;
    public event Action OnJumpEnd;
    
    public void Init(PlayerController player)
    {
        playerController = player;
        playerStats = player.Stats;
        
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    public void TickUpdate()
    {
        if (isKnockbacked) return;

        ReadInput();
        
        HandleMovement();
        HandleJump();
    }
    
    public void TickFixedUpdate()
    {
        UpdateGroundCheck();
        CheckAirTime();
    }
    
    private void UpdateGroundCheck()
    {
        if (groundCheck == null) return;

        wasGrounded = isGrounded;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer) != null;
        isCeiling = Physics2D.OverlapCircle(ceilingCheck.position, checkRadius, groundLayer) != null;
      
        if (wasGrounded && !isGrounded)
        {
            coyoteTimer = coyoteTime;
        }
        else if (!isGrounded)
        {
            coyoteTimer = Mathf.Max(0f, coyoteTimer - Time.fixedDeltaTime);
        }
        
        if (!wasGrounded && isGrounded)
        {
            coyoteTimer = 0f;
            HandleLanding();
        }
    }
    
    private void CheckAirTime()
    {
        if (isGrounded)
        {
            timeSinceLastJump = 0f;
        }
        else if (!playerController.isDead)
        {
            timeSinceLastJump += Time.fixedDeltaTime;
            if (timeSinceLastJump >= maxLockTime)
            {
                playerController.TakeDamage(Vector3.up, 999, 20);
            }
        }
    }
    
    private void ReadInput()
    {
        moveInput = InputManager.Instance.MoveInput;
        isSprinting = InputManager.Instance.SprintInput && !isCroucing;
    }
    
    private void HandleMovement()
    {
        float targetSpeed = isCroucing ? playerStats.CrouchSpeed : isSprinting ? playerStats.SprintSpeed : playerStats.MoveSpeed;

        Vector3 inputDirection = new Vector3(moveInput.x, 0f, 0f);
        inputDirection = Vector3.ClampMagnitude(inputDirection, 1f);

        Vector3 targetVelocity = transform.rotation * (inputDirection * targetSpeed);
        Vector3 velocity = rb.linearVelocity;

        Vector3 horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z );
        float acceleration = isGrounded ? inputDirection.sqrMagnitude > 0.01f ? groundAcceleration : groundDeceleration : airAcceleration;

        horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetVelocity, acceleration * Time.deltaTime);
        rb.linearVelocity = new Vector3(horizontalVelocity.x, velocity.y, horizontalVelocity.z);
    }
    
   
    private void HandleJump()
    {
        if (!InputManager.Instance.JumpInput) return;
        if (InputManager.Instance.CrouchInput) return;
        
        if (isJumping && !isGrounded) return;
        if (!isGrounded && coyoteTimer <= 0f) return;

        Jump();
    }

    private void Jump()
    {
        isJumping = true;
        coyoteTimer = 0f;

        rb.AddForce(Vector3.up * playerStats.JumpForce, ForceMode2D.Impulse);
        OnJumpStart?.Invoke();
    }

    private void HandleLanding()
    {
        isJumping = false;
        OnJumpEnd?.Invoke();
    }
    
    public void ApplyKnockback(Vector3 hitDirection, float knockbackForce = 4f)
    {
        if (playerController.isInvulnerable) return;

        if (invulnerabilityRoutine != null) StopCoroutine(invulnerabilityRoutine);
        invulnerabilityRoutine = StartCoroutine(InvulnerabilityRoutine(playerStats.InvulnerabilityTime));

        if (knockbackRoutine != null) StopCoroutine(knockbackRoutine);
        knockbackRoutine = StartCoroutine(ApplyKnockbackRoutine(hitDirection, knockbackForce));
    }
    
    private IEnumerator ApplyKnockbackRoutine(Vector2 hitDirection, float knockbackForce)
    {
        Stop();
        isKnockbacked = true;
        
        rb.AddForce(hitDirection.normalized * (playerStats.KnockbackMultiplier * knockbackForce), ForceMode2D.Impulse);
        
        yield return new WaitForSeconds(0.2f);
        yield return new WaitUntil(() => isGrounded);
        
        Stop();
        
        isKnockbacked = false;
        knockbackRoutine = null;
    }

    private IEnumerator InvulnerabilityRoutine(float duration)
    {
        playerController.isInvulnerable = true;

        yield return new WaitForSeconds(duration);

        playerController.isInvulnerable = false;
        invulnerabilityRoutine = null;
    }

    public void ResetKnockback()
    {
        isKnockbacked = false;
        playerController.isInvulnerable = false;

        if (knockbackRoutine != null)
        {
            StopCoroutine(knockbackRoutine);
            knockbackRoutine = null;
        }

        if (invulnerabilityRoutine != null)
        {
            StopCoroutine(invulnerabilityRoutine);
            invulnerabilityRoutine = null;
        }
    }
    
    public void Stop()
    {
        if (rb == null || rb.isKinematic) return;
        rb.linearVelocity = Vector3.zero;
    }
    
    public void SetKinematic(bool kinematic)
    {
        rb.isKinematic = kinematic;
        rb.gravityScale = kinematic ? 0f : 1f;
    }
    
}

using UnityEngine;

public class PlayerStats 
{
    public float MaxHealth { get; private set; }
    public float CurrentHealth  { get; private set; }
    
    public float MoveSpeed { get; private set; }
    public float SprintSpeed { get; private set; }
    public float CrouchSpeed { get; private set; }
    
    public float JumpForce { get; private set; }
    public float MaxJumpCount { get; private set; }
    public float CurrentJumpCount { get; private set; }
    
    public float InvulnerabilityTime { get; private set; }
    public float KnockbackMultiplier { get; private set; }

    public PlayerStats(PlayerData data)
    {
        UpdateData(data);
    }

    public void UpdateData(PlayerData data)
    {
        MaxHealth  = data.health;
        
        MoveSpeed = data.speed;
        SprintSpeed = data.sprintSpeed;
        CrouchSpeed = data.crouchSpeed;
        
        JumpForce = data.jumpForce;
        MaxJumpCount = data.jumpCount;
        
        InvulnerabilityTime = data.invulnerabilityTime;
        KnockbackMultiplier = data.knockbackMultiplier;
        
        ResetData();
    }

    public void ResetData()
    {
        CurrentHealth = MaxHealth;
        CurrentJumpCount = MaxJumpCount;
    }

    public void ConsumeHealth(float amount = 1)
    {
        CurrentHealth -= amount;
        if (CurrentHealth < 0) CurrentHealth = 0;
    }

    public void AddHealth(float amount = 1)
    {
        CurrentHealth += amount;
    }

    public bool HasHealth => CurrentHealth > 0;
    
    public void ConsumeJump(int amount = 1)
    {
        CurrentJumpCount -= amount;
        if (CurrentJumpCount < 0) CurrentJumpCount = 0;
    }
    
    public bool HasJump => CurrentJumpCount > 0;
}

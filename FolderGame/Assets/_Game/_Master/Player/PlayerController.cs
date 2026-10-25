using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] private PlayerData playerData;
    private PlayerData runtimeData;
    
    [Header("Player States")]
    public bool isMoving = false;
    public bool isActive = false;
    public bool isDead = false;
    public bool isInvulnerable = false;
    
    public PlayerMovement Movement { get; private set; }
    public PlayerActions Action { get; private set; }
    public PlayerStats Stats { get; private set; }
    
    [Header("Death Actions")]
    [SerializeField] private UnityEvent onDeath;
    
    private Coroutine playerCoroutine;
    private Coroutine hitCoroutine;

    private void Awake()
    {
        Movement = GetComponent<PlayerMovement>();
        Action = GetComponent<PlayerActions>();
        
        if (playerData != null)
            runtimeData = Instantiate(playerData); 
        
        Stats = new PlayerStats(runtimeData);
        Movement.Init(this);
        Action.Init(this);
    }

    private void Update()
    {
        if (isDead) return;
        
        if (isMoving)
        {
            Movement.TickUpdate();
        }

        if (isActive)
        {
            Action.TickUpdate();
        }
    }

    private void FixedUpdate()
    {
        if (isDead) return;
        
        if (isMoving)
        {
            Movement.TickFixedUpdate();
        }
    }
    
    public void ResetPlayerStats()
    {
        Movement.Stop();
        
        isActive = false;
        isMoving = false;
        isInvulnerable = false;
    }

    public void ActivatePlayer()
    {
        if (playerCoroutine != null) StopCoroutine(playerCoroutine);
        playerCoroutine = StartCoroutine(ActivatePlayerRoutine());
    }
    
    private IEnumerator ActivatePlayerRoutine()
    {
        isInvulnerable = false;
        
        Movement.Stop();
        Movement.SetKinematic(false);
        
        yield return null;
        
        isMoving = true;
    }
    
    public void DeactivatePlayer()
    {
        isMoving = false;
        isInvulnerable = true;
        
        Movement.Stop();
        Movement.SetKinematic(true);
    }
    
    public void TakeDamage(Vector3 hitDirection, float damage = 1, float knockbackForce = 4)
    {
        if (isInvulnerable || isDead) return;
        Stats.ConsumeHealth(damage);

        if (!Stats.HasHealth)
        {
            KillPlayer(hitDirection);
        }
        
        Movement.ApplyKnockback(hitDirection, knockbackForce);
    }
    
    private void KillPlayer(Vector3 hitDirection)
    {
        isDead = true;
        
        isActive = false;
        isMoving = false;
        
        Movement.Stop();
        Movement.SetKinematic(true);
        
        onDeath.Invoke();
        
        // GameOverManager.Instance.GameOver("LOST TO GRAVITY");
    }

    public PlayerData GetPlayerData()
    {
        return runtimeData;
    }
}

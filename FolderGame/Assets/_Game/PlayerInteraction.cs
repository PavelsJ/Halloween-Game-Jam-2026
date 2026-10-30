using UnityEngine;
using UnityEngine.Events;

public class PlayerInteraction : MonoBehaviour, IInteractable
{
    public bool IsUsed { get; set; }

    [Header("Click Actions")] 
    [SerializeField] private GameObject inventory;

    [Header("Click Effect")] 
    [SerializeField] private bool showOutline = true;
    
    private PlayerController playerController;
    private PlayerStats playerStats;

    public void Init(PlayerController player)
    {
        playerController = player;
        playerStats = player.Stats;
        
        inventory.SetActive(false);
    }

    public void TickUpdate()
    {
        if (InputManager.Instance.PauseInput)
        {
            Release();
        }
    }
    
    public bool CanInteract()
    {
        return !IsUsed;
    }

    public virtual void Interact(Vector3 worldPos)
    {
    }

    public virtual void Release()
    {
        inventory.SetActive(!inventory.activeSelf);
        // playerController.isMoving = !inventory.activeSelf;
    }

    public virtual void Cancel()
    {
       
    }

    public virtual void Move(Vector3 worldPos)
    {
        
    }

    public virtual void ShowInfoUI()
    {
        if (showOutline) 
            playerController.Visual.SetOutline(true);
    }

    public virtual void HideInfoUI()
    {
        if (showOutline) 
            playerController.Visual.SetOutline(false);
    }
}

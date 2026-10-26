using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerActions : MonoBehaviour
{
    [SerializeField] PlayerPreview playerPreview;
    [SerializeField] private LayerMask interactableLayer;
    
    private PlayerController playerController;
    private PlayerStats playerStats;
    private Camera playerCamera;
    
    private IInteractable currentInteractable;

    private void Start()
    {
        playerCamera = Camera.main;
    }

    public void Init(PlayerController player)
    {
        playerController = player;
        playerStats = player.Stats;
    }

    public void TickUpdate()
    {
        if (playerCamera == null) return;
        
        Vector3 screenPos = Input.mousePosition;
        Vector3 worldPos = playerCamera.ScreenToWorldPoint(screenPos);
        worldPos.z = 0f;

        playerPreview.UpdatePreview(worldPos);
        
        if (InputManager.Instance.PressInput)
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            CheckInteractable(worldPos);
        }

        if (currentInteractable == null) return;
        
        if (InputManager.Instance.HoldInput)
        {
            if (currentInteractable.CanInteract())
            {
                currentInteractable.Interact(worldPos);
            }
            else
            {
                playerPreview.ShakePreview(0.2f);
            }
        }
        else if (InputManager.Instance.ReleaseInput)
        {
            IInteractable interactable = GetInteractable(worldPos);
            
            if (interactable == null)
            {
                currentInteractable.Cancel();
            }
            else
            {
                currentInteractable.Release();
            }
          
            currentInteractable = null;
        }
    }

    private void CheckInteractable(Vector3 worldPos)
    {
        IInteractable interactable = GetInteractable(worldPos);
        if (interactable == null) return;

        if (interactable.CanInteract())
        {
            currentInteractable = interactable;
            interactable.Interact(worldPos);
        }
        else
        {
            playerPreview.ShakePreview(0.2f);
        }
    }

  
    private IInteractable GetInteractable(Vector3 worldPos)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(worldPos, 0.2f, interactableLayer);

        if (hits.Length == 0) return null;

        System.Array.Sort(hits, (a, b) =>
        {
            SpriteRenderer spriteA = a.GetComponent<SpriteRenderer>();
            SpriteRenderer spriteB = b.GetComponent<SpriteRenderer>();

            int orderA = spriteA != null ? spriteA.sortingOrder : 0;
            int orderB = spriteB != null ? spriteB.sortingOrder : 0;

            return orderB.CompareTo(orderA);
        });

        return hits[0].GetComponent<IInteractable>();
    }
}

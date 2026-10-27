using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerActions : MonoBehaviour
{
    [SerializeField] PlayerPreview playerPreview;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private LayerMask blockLayer;
    
    private Vector3 lastValidMousePosition;
    private bool mousePositionInitialized;
    
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

        if (!mousePositionInitialized)
        {
            lastValidMousePosition = screenPos;
            mousePositionInitialized = true;
        }
        
        Vector3 worldPos = playerCamera.ScreenToWorldPoint(screenPos);
        worldPos.z = 0f;
        
        if (Physics2D.OverlapCircle(worldPos, 0.1f, blockLayer))
        {
            Cursor.lockState = CursorLockMode.None;
            UnityEngine.InputSystem.Mouse.current.WarpCursorPosition(lastValidMousePosition);
            return;
        }
        
        lastValidMousePosition = screenPos;
        playerPreview.UpdatePreview(worldPos);
        
        if (InputManager.Instance.PressInput)
        {
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
            IInteractable interactable = GetInteractable();
            
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
        IInteractable interactable = GetInteractable();
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
  
    private IInteractable GetInteractable()
    {
        if (EventSystem.current != null)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            foreach (RaycastResult result in results)
            {
                IInteractable interactable =
                    result.gameObject.GetComponentInParent<IInteractable>();

                if (interactable != null)
                    return interactable;
            }
        }
        
        Vector3 worldPos = playerCamera.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;

        Collider2D[] hits = Physics2D.OverlapCircleAll(worldPos, 0.2f, interactableLayer);

        if (hits.Length == 0) return null;

        System.Array.Sort(hits, (a, b) =>
        {
            SpriteRenderer spriteA = a.GetComponentInParent<SpriteRenderer>();
            SpriteRenderer spriteB = b.GetComponentInParent<SpriteRenderer>();

            int orderA = spriteA != null ? spriteA.sortingOrder : 0;
            int orderB = spriteB != null ? spriteB.sortingOrder : 0;

            return orderB.CompareTo(orderA);
        });

        foreach (Collider2D hit in hits)
        {
            IInteractable interactable = hit.GetComponentInParent<IInteractable>();
            if (interactable != null) return interactable;
        }

        return null;
    }
}

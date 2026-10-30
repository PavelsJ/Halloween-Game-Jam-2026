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
    private bool isRepeling = false;

    private void Start()
    {
        playerCamera = Camera.main;
    }

    public void Init(PlayerController player)
    {
        playerController = player;
        playerStats = player.Stats;
    }
    
    private void Update()
    {
        if (playerController == null || !playerController.isDead) return;
        if (playerCamera == null) return;
        
        Vector3 screenPos = Input.mousePosition;
        Vector3 worldPos = playerCamera.ScreenToWorldPoint(screenPos);
        worldPos.z = 0f;
        
        playerPreview.UpdatePreview(worldPos);
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
            playerPreview.ShakePreview(0.2f);
            return;
        }
        
        lastValidMousePosition = screenPos;
        playerPreview.UpdatePreview(worldPos);

        if (isRepeling)
        {
            RepelInteractables(worldPos);
            return;
        }
        
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
                MusicManager.instance.PlayInstaSound(MusicManager.SoundType.WrongButtonPress, Random.Range(0.8f, 1.2f));
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
            MusicManager.instance.PlayInstaSound(MusicManager.SoundType.WrongButtonPress, Random.Range(0.8f, 1.2f));
        }
    }
    
    private void RepelInteractables(Vector3 worldPos)
    {
        if (EventSystem.current == null) return;

        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        HashSet<Transform> movedObjects = new HashSet<Transform>();

        foreach (RaycastResult result in results)
        {
            IInteractable interactable = result.gameObject.GetComponent<IInteractable>();
            if (interactable == null) continue;
            
            Interactable interactableScript = interactable as Interactable;
            if (interactableScript == null) continue;

            Transform target = interactableScript.parentBlock.transform;
            if (target == null || !movedObjects.Add(target)) continue;

            Vector3 direction = target.position - worldPos;
            direction.z = 0f;

            if (direction.sqrMagnitude < 0.001f) continue;
            interactable.Move(target.position += direction.normalized * (Time.deltaTime * 10f));
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
                IInteractable interactable = result.gameObject.GetComponent<IInteractable>();
                if (interactable != null) return interactable;
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

    public void SetRepel(bool state)
    {
        isRepeling = state;
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerActions : MonoBehaviour
{
    [SerializeField] PlayerPreview playerPreview;
    
    private PlayerController playerController;
    private PlayerStats playerStats;
    private Camera playerCamera;

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

            Apply(worldPos);
        }
    }

    private void Apply(Vector3 worldPos)
    {
        
    }

    public void TickFixedUpdate()
    {
        if (playerCamera == null) return;
        
        Vector3 mouseWorld = playerCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;
            
        playerPreview.UpdatePreview(mouseWorld);
    }
}

using UnityEngine;

public class PoweEnd : PowerInteraction
{
    [SerializeField] private GameFinshManager finshManager;
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (isUsed) return;
        
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController controller = other.gameObject.GetComponent<PlayerController>();
            if (controller != null) controller.DeactivatePlayer();
            finshManager.FinishGame();
            InventoryManager.Instance.SaveInventory();
            isUsed = true;
        }
    }
}

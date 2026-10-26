using UnityEngine;

public class FlowerInteraction : Interactable
{
    [SerializeField] private ItemData flowerData;
    
    public override bool CanInteract()
    {
        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsUsed) return;
        if (other.gameObject.CompareTag("Player"))
        {
            InventoryManager.Instance.AddItem(flowerData);
            IsUsed = true;
            
            parentBlock.gameObject.SetActive(false);
        }
    }
}

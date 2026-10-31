using UnityEngine;

public class ItemReceiver : MonoBehaviour, IItemReceiver
{
    [SerializeField] private MilfController milfController;
    
    public void ReceiveItem(ItemSlot sourceSlot)
    {
        InventoryManager.Instance.RemoveItem(sourceSlot);
        
        if (milfController != null)
            milfController.ReceiveFlover();
    }
}

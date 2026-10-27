using System.Collections;
using UnityEngine;

public class FlowerInteraction : MonoBehaviour
{
    [SerializeField] private ItemData flowerData;
    private bool isUsed;
    
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        
        if (flowerData == null || InventoryManager.Instance == null) yield break;

        foreach (ItemData item in InventoryManager.Instance.Items)
        {
            if (item != null && item.ItemID == flowerData.ItemID)
            {
                isUsed = true;
                gameObject.SetActive(false);
                yield break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isUsed) return;
        if (other.gameObject.CompareTag("Player"))
        {
            InventoryManager.Instance.AddItem(flowerData);
            isUsed = true;
            
            gameObject.SetActive(false);
        }
    }
}

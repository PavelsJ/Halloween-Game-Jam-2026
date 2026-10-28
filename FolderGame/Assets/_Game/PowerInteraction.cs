using UnityEngine;

public class PowerInteraction : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    private bool isUsed;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isUsed) return;
        
        if (other.gameObject.CompareTag("Player"))
        {
            InventoryManager.Instance.SaveInventory();
            CameraManager.Instance.ShakeCameraIncreasing(1, 0.5f);
            LoadManager.Instance.OpenScene(nextSceneName);
            isUsed = true;
        }
    }
}

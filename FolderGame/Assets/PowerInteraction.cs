using UnityEngine;

public class PowerInteraction : Interactable
{
    [SerializeField] private string nextSceneName;
    
    public override bool CanInteract()
    {
        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsUsed) return;
        
        if (other.gameObject.CompareTag("Player"))
        {
            LoadManager.Instance.OpenScene(nextSceneName);
            CameraManager.Instance.ShakeCameraIncreasing(1, 0.5f);
            IsUsed = true;
        }
    }
}

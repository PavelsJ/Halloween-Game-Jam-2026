using UnityEngine;

public interface IInteractable
{
    bool IsUsed { get; set; }

    bool CanInteract();
    
    void Interact(Vector3 worldPos);
    
    void Release();
    
    void Cancel();
    
    void Move(Vector3 worldPos);

    void ShowInfoUI();
    
    void HideInfoUI();
}

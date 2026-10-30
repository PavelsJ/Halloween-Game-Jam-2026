using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Interactable : MonoBehaviour, IInteractable
{
    public bool IsUsed { get; set; }
    
    internal BlockController parentBlock;
    internal Button button;
    
    [Header("Click Actions")]
    [SerializeField] protected UnityEvent onInteract;
    [SerializeField] protected UnityEvent onRelease;

    [Header("Click Effect")] 
    [SerializeField] internal Image image;
    [SerializeField] private bool showOutline = true;
    
    public virtual void Init(BlockController blockController, Material currentMaterial)
    {
        parentBlock =  blockController;

        if (image != null)
        {
            image.material = currentMaterial;
        }
        
        parentBlock.SetOutline(false);
    }
    
    public virtual bool CanInteract()
    {
        if (IsUsed)
        {
            return true;
        }
        return !IsUsed;
    }

    public virtual void Interact(Vector3 worldPos)
    {
        parentBlock.BringToFront();
        
        if (onInteract != null)
            onInteract.Invoke();
    }

    public virtual void Release()
    {
        parentBlock.BringToFront();
        
        if (onRelease != null)
            onRelease.Invoke();
    }

    public virtual void Cancel()
    {
        parentBlock.BringToFront();
    }

    public virtual void Move(Vector3 worldPos)
    {
        parentBlock.GetMotion().StopMotion();
        parentBlock.transform.position = worldPos;
    }

    public virtual void ShowInfoUI()
    {
        if (showOutline) parentBlock.SetOutline(true);
    }

    public virtual void HideInfoUI()
    {
        if (showOutline) parentBlock.SetOutline(false);
    }
}

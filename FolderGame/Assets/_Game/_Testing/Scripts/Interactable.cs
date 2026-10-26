using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour, IInteractable
{
    public bool IsUsed { get; set; }
    
    internal BlockController parentBlock;
    
    [Header("Click Actions")]
    [SerializeField] protected UnityEvent onInteract;
    [SerializeField] protected UnityEvent onRelease;

    [Header("Click Effect")] 
    [SerializeField] private bool showInfo = true;
    [SerializeField] internal SpriteRenderer spriteRenderer;
    [SerializeField] internal float darkenMultiplier = 0.2f;
    internal Color baseColor;

    public virtual void Init(BlockController blockController, Material currentMaterial)
    {
        parentBlock =  blockController;

        if (spriteRenderer != null)
        {
            spriteRenderer.material = currentMaterial;
            baseColor = spriteRenderer.color;
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
        
        if (spriteRenderer != null)
            spriteRenderer.color = new Color(baseColor.r * darkenMultiplier, baseColor.g * darkenMultiplier, baseColor.b * darkenMultiplier, baseColor.a);
        
        if (onInteract != null)
            onInteract.Invoke();
    }

    public virtual void Release()
    {
        parentBlock.BringToFront();
        
        if (spriteRenderer != null)
            spriteRenderer.color = baseColor;
        
        if (onRelease != null)
            onRelease.Invoke();
    }

    public virtual void Cancel()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = baseColor;
    }

    public virtual void SetLayer(int layer)
    {
        if (spriteRenderer != null)
            spriteRenderer.sortingOrder = layer;
    }

    public virtual void ShowInfoUI()
    {
        if (showInfo)
            parentBlock.SetOutline(true);
    }

    public virtual void HideInfoUI()
    {
        if (showInfo) 
            parentBlock.SetOutline(false);
    }
}

using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    public bool IsUsed { get; set; }
    
    internal BlockController parentBlock;
    
    [Header("Click Effect")]
    [SerializeField] internal SpriteRenderer spriteRenderer;
    [SerializeField] internal float darkenMultiplier = 0.2f;
    internal Color baseColor;

    public virtual void Init(BlockController blockController)
    {
        parentBlock =  blockController;
            
        if (spriteRenderer != null)
            baseColor = spriteRenderer.color;
    }
    
    public virtual bool CanInteract()
    {
        return !IsUsed;
    }

    public virtual void Interact(Vector3 worldPos)
    {
        parentBlock.BringToFront();
        
        if (spriteRenderer != null)
            spriteRenderer.color = new Color(baseColor.r * darkenMultiplier, baseColor.g * darkenMultiplier, baseColor.b * darkenMultiplier, baseColor.a);
    }

    public virtual void Release()
    {
        parentBlock.BringToFront();
        
        if (spriteRenderer != null)
            spriteRenderer.color = baseColor;
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
       
    }

    public virtual void HideInfoUI()
    {
        
    }
}

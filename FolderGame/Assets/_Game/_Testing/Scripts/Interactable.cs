using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    public bool IsUsed { get; set; }
    
    [SerializeField] internal GameObject parentBlock;
    
    [Header("Click Effect")]
    [SerializeField] internal SpriteRenderer spriteRenderer;
    [SerializeField] internal float darkenMultiplier = 0.2f;
    internal Color baseColor;

    private void Start()
    {
        if(spriteRenderer != null)
            baseColor = spriteRenderer.color;
    }
    
    public virtual bool CanInteract()
    {
        return !IsUsed;
    }

    public virtual void Interact(Vector3 worldPos)
    {
        if (spriteRenderer != null)
            spriteRenderer.color = new Color(baseColor.r * darkenMultiplier, baseColor.g * darkenMultiplier, baseColor.b * darkenMultiplier, baseColor.a);
    }

    public virtual void Release()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = baseColor;
    }

    public virtual void Cancel()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = baseColor;
    }

    public virtual void ShowInfoUI()
    {
       
    }

    public virtual void HideInfoUI()
    {
        
    }
}

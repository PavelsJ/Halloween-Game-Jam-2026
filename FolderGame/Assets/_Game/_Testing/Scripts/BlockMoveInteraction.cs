using UnityEngine;

public class BlockMoveInteraction : Interactable
{
    [SerializeField] Collider2D boxCollider;
    
    private bool isDragging;
    private Vector3 dragOffset;

    public override void Interact(Vector3 worldPos)
    {
        if (!isDragging)
        {
            isDragging = true;
            dragOffset = parentBlock.transform.position - worldPos;
            
            if (parentBlock != null)
                parentBlock.BringToFront();
            
            if (spriteRenderer != null)
                spriteRenderer.color = new Color(baseColor.r * darkenMultiplier, baseColor.g * darkenMultiplier, baseColor.b * darkenMultiplier, baseColor.a);
            
            if (boxCollider != null)
                boxCollider.enabled = false;
        }

        parentBlock.transform.position = worldPos + dragOffset;
    }
    
    public override void Release()
    {
        isDragging = false;
        
        if (boxCollider != null)
            boxCollider.enabled = true;
        
        base.Release();
    }
}

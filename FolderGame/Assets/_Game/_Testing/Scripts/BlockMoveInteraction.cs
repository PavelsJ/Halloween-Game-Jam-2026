using UnityEngine;

public class BlockMoveInteraction : Interactable
{
    private bool isDragging;
    private Vector3 dragOffset;

    public override void Interact(Vector3 worldPos)
    {
        if (!isDragging)
        {
            isDragging = true;
            dragOffset = parentBlock.transform.position - worldPos;
            
            if (spriteRenderer != null)
                spriteRenderer.color = new Color(baseColor.r * darkenMultiplier, baseColor.g * darkenMultiplier, baseColor.b * darkenMultiplier, baseColor.a);
        }

        parentBlock.transform.position = worldPos + dragOffset;
    }
    
    public override void Release()
    {
        isDragging = false;
        base.Release();
    }
}

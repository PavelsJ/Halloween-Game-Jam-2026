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

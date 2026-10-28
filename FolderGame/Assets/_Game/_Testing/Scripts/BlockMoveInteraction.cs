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
            
            if (parentBlock != null)
                parentBlock.BringToFront();

            Collider2D col = parentBlock.GetGround();
            if (col != null) col.enabled = false;

            MotionEffect motionEffect = parentBlock.GetMotion();
            if (motionEffect != null) motionEffect.StopMotion();
        }

        parentBlock.transform.position = worldPos + dragOffset;
    }
    
    public override void Release()
    {
        isDragging = false;
        
        Collider2D col = parentBlock.GetGround();
        if (col != null) col.enabled = true;
        
        MotionEffect motionEffect = parentBlock.GetMotion();
        if (motionEffect != null) motionEffect.StartMotion();
        
        base.Release();
    }
}

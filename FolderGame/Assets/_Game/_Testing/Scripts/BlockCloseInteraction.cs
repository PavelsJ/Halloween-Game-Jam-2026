using UnityEngine;

public class BlockCloseInteraction : Interactable
{
    public override void Interact(Vector3 worldPos)
    {
       base.Interact(worldPos);
    }

    public override void Release()
    {
        IsUsed = true;
        
        base.Release();
    }
}

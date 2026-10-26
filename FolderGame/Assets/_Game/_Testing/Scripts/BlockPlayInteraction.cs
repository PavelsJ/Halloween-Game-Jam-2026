using UnityEngine;

public class BlockPlayInteraction : Interactable
{
    public override bool CanInteract()
    {
        return false;
    }

    public override void Interact(Vector3 worldPos)
    {
        base.Interact(worldPos);
    }
}

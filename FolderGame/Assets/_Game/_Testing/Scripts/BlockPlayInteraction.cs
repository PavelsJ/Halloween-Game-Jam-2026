using UnityEngine;

public class BlockPlayInteraction : Interactable
{
    public override bool CanInteract()
    {
        return false;
    }

    public override void Interact(Vector3 worldPos)
    {
        parentBlock.BringToFront();
        
        if (spriteRenderer != null)
            spriteRenderer.color = new Color(baseColor.r * darkenMultiplier, baseColor.g * darkenMultiplier, baseColor.b * darkenMultiplier, baseColor.a);
    }
}

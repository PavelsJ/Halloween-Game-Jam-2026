using UnityEngine;

public class BlockCloseInteraction : Interactable
{
    public override void Interact(Vector3 worldPos)
    {
        parentBlock.BringToFront();
        
        if (spriteRenderer != null)
            spriteRenderer.color = new Color(baseColor.r * darkenMultiplier, baseColor.g * darkenMultiplier, baseColor.b * darkenMultiplier, baseColor.a);
    }

    public override void Release()
    {
        IsUsed = true;
        parentBlock.gameObject.SetActive(false);
        
        base.Release();
    }
}

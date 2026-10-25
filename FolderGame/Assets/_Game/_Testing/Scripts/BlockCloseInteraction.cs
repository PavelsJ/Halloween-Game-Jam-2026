using UnityEngine;

public class BlockCloseInteraction : Interactable
{
    public override void Interact(Vector3 worldPos)
    {
        if (spriteRenderer != null)
            spriteRenderer.color = new Color(baseColor.r * darkenMultiplier, baseColor.g * darkenMultiplier, baseColor.b * darkenMultiplier, baseColor.a);
    }

    public override void Release()
    {
        IsUsed = true;
        parentBlock.SetActive(false);
        
        base.Release();
    }
}

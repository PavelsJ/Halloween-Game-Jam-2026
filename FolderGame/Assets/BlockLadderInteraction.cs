using UnityEngine;

public class BlockLadderInteraction : BlockCloseInteraction
{
    [SerializeField] private GameObject newBlock;
    
    public override void Interact(Vector3 worldPos)
    {
        if (spriteRenderer != null)
            spriteRenderer.color = new Color(baseColor.r * darkenMultiplier, baseColor.g * darkenMultiplier, baseColor.b * darkenMultiplier, baseColor.a);
    }

    public override void Release()
    {
        IsUsed = true;
        Instantiate(newBlock, parentBlock.transform.localPosition + new Vector3(0.25f,0.25f), Quaternion.identity);
        
        if (spriteRenderer != null)
            spriteRenderer.color = baseColor;
    }
}

using UnityEngine;

public class BlockLadderInteraction : BlockCloseInteraction
{
    [Header("Ladder Interaction")]
    [SerializeField] private int ladderAmount;
    [SerializeField] private GameObject newBlock;
    private int currentLadderAmount;

    private void Start()
    {
        currentLadderAmount = ladderAmount;
    }
    
    public override void Interact(Vector3 worldPos)
    {
        parentBlock.BringToFront();
        
        if (spriteRenderer != null)
            spriteRenderer.color = new Color(baseColor.r * darkenMultiplier, baseColor.g * darkenMultiplier, baseColor.b * darkenMultiplier, baseColor.a);
    }

    public override void Release()
    {
        IsUsed = true;

        if (currentLadderAmount <= 0) return;
        
        GameObject block = Instantiate(newBlock, parentBlock.transform.localPosition + new Vector3(0.25f, 0.25f), Quaternion.identity);
        SubBlockRelay relay = block.GetComponentInChildren<SubBlockRelay>();
            
        if (relay == null) return;
        relay.SetLadder(this);
        relay.parentBlock.BringToFront();
            
        if (spriteRenderer != null)
            spriteRenderer.color = baseColor;
            
        currentLadderAmount--;
    }

    public void UpdateLadder(BlockController controller)
    {
        Debug.Log($"UpdateLadder called. Remaining: {currentLadderAmount}");
        
        if (currentLadderAmount <= 0) return;
        
        GameObject block = Instantiate(newBlock, controller.transform.localPosition + new Vector3(0.25f, 0.25f), Quaternion.identity);
        SubBlockRelay relay = block.GetComponentInChildren<SubBlockRelay>();
                
        if (relay == null) return;
        relay.SetLadder(this);
        relay.parentBlock.BringToFront();
            
        currentLadderAmount--;
    }
}

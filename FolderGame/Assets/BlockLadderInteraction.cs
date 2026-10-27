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
       base.Interact(worldPos);
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
        
        if (onRelease != null)
            onRelease.Invoke();
            
        currentLadderAmount--;
    }

    public void UpdateLadder(BlockController controller)
    {
        if (currentLadderAmount <= 0)
        {
            return;
        }
        
        GameObject block = Instantiate(newBlock, controller.transform.localPosition + new Vector3(0.25f, 0.25f), Quaternion.identity);
        SubBlockRelay relay = block.GetComponentInChildren<SubBlockRelay>();
                
        if (relay == null) return;
        relay.SetLadder(this);
        relay.parentBlock.BringToFront();
            
        currentLadderAmount--;
    }
}

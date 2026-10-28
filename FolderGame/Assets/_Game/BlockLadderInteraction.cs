using UnityEngine;

public class BlockLadderInteraction : BlockCloseInteraction
{
    [Header("Ladder Interaction")]
    [SerializeField] private Vector3 direction = new Vector3(0.25f, 0.25f);
    [SerializeField] private int ladderAmount;
    [SerializeField] private GameObject newBlock;
    private int currentLadderAmount;
    private Vector3 originPos;

    private void Start()
    {
        currentLadderAmount = ladderAmount;
        originPos = parentBlock.transform.localPosition;
    }
    
    public override void Interact(Vector3 worldPos)
    {
       base.Interact(worldPos);
    }

    public override void Release()
    {
        IsUsed = true;

        if (currentLadderAmount <= 0) return;
        
        GameObject block = Instantiate(newBlock, originPos + direction, Quaternion.identity);
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
        
        GameObject block = Instantiate(newBlock, controller.transform.localPosition + direction, Quaternion.identity);
        SubBlockRelay relay = block.GetComponentInChildren<SubBlockRelay>();
                
        if (relay == null) return;
        relay.SetLadder(this);
        relay.parentBlock.BringToFront();
            
        currentLadderAmount--;
    }
}

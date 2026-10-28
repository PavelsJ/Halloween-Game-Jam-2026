using UnityEngine;

public class SubBlockRelay : BlockCloseInteraction
{
    private BlockLadderInteraction ladderParent;

    public void SetLadder(BlockLadderInteraction ladderInteraction)
    {
        ladderParent = ladderInteraction;
    }
    
    public override void Release()
    {
        IsUsed = true;

        if (ladderParent ==null) return;
        ladderParent.UpdateLadder(parentBlock);
        
        if (onRelease != null)
            onRelease.Invoke();
    }
}

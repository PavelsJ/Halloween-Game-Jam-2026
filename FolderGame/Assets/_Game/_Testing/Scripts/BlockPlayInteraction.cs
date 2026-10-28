using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlockPlayInteraction : Interactable
{
    [SerializeField] protected bool isRandom = true;
    [SerializeField] protected Sprite[] adSprites;

    public virtual IEnumerator Start()
    {
        yield return null;
        
        if (isRandom)
        {
            if (adSprites.Length <= 0) yield break;
            image.sprite = adSprites[Random.Range(0, adSprites.Length)];
        }
        
        image.SetAllDirty();
        image.rectTransform.ForceUpdateRectTransforms();

        Canvas.ForceUpdateCanvases();
    }
    
    public override void Interact(Vector3 worldPos)
    {
        base.Interact(worldPos);
    }
}

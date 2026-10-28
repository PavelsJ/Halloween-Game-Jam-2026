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

        Reset();
    }

    public void Reset()
    {
        Sprite randomSprite = image.sprite;
        
        if (isRandom)
        {
            if (adSprites.Length > 0) 
                randomSprite = adSprites[Random.Range(0, adSprites.Length)];
        }
      
        image.sprite = randomSprite;
        
        AutoImageAspectRatio aspectRatio = image.GetComponent<AutoImageAspectRatio>();
        if (aspectRatio != null) aspectRatio.Init();
    }
    
    public override void Interact(Vector3 worldPos)
    {
        base.Interact(worldPos);
    }
}

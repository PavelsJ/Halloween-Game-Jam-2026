using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Image), typeof(AspectRatioFitter))]
public class AutoImageAspectRatio : MonoBehaviour
{
    public void Init()
    {
        Image image = GetComponent<Image>();
        AspectRatioFitter fitter = GetComponent<AspectRatioFitter>();

        if (image.sprite != null)
        {
            fitter.aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;
            fitter.aspectRatio = image.sprite.rect.width / image.sprite.rect.height;
        }
    }
}
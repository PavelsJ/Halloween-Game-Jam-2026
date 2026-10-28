using System.Collections;
using UnityEngine;

public class InteractableController : MonoBehaviour
{
    [Header("Block Visual")]
    [SerializeField] private Material hitMaterial;
    [SerializeField] private float flashTime = 0.5f;
    [SerializeField] private AnimationCurve flashCurve;
    [SerializeField] private SpriteRenderer[] renderers;
    
    private Material currentMaterial;

    private void Start()
    {
        currentMaterial = new Material(hitMaterial);

        if (renderers.Length > 0)
        {
            foreach (SpriteRenderer render in renderers)
            {
                render.material =  currentMaterial;
            }
            
            StartCoroutine(FlashCoroutine());
        }
    }
    
    private void OnEnable()
    {
        StartCoroutine(FlashCoroutine());
    }
    
    private IEnumerator FlashCoroutine()
    {
        float currentFlashAmount = 1;
        float elapsedTime = 0;
        
        yield return null;
        
        while (elapsedTime < flashTime)
        {
            elapsedTime += Time.deltaTime;
            currentFlashAmount = Mathf.Lerp(0, flashCurve.Evaluate(elapsedTime), elapsedTime / flashTime);
            PlayFlash(currentFlashAmount);
            yield return null;
        }
    }

    private void PlayFlash(float amount)
    {
        currentMaterial.SetFloat("_HitEffectBlend", amount);
    }
}

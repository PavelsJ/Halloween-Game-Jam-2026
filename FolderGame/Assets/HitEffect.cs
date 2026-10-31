using System.Collections;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    [SerializeField] private GameObject hitParticle;
    private bool isUsed;
    
    [Header("Block Visual")]
    [SerializeField] private Material hitMaterial;
    [SerializeField] private float flashTime = 0.5f;
    [SerializeField] private AnimationCurve flashCurve;
    
    [SerializeField] private SpriteRenderer renderer;
    private Material currentMaterial;
    
    private void Start()
    {
        currentMaterial = new Material(hitMaterial);
        
        if (renderer != null) 
            renderer.material = currentMaterial;
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

    public void PlayFlash(float amount)
    {
        currentMaterial.SetFloat("_HitEffectBlend", amount);
    }
    
    public void PlayAlpha(float amount)
    {
        currentMaterial.SetFloat("_AlphaEffectBlend", amount);
    }

    public void PlayParticles()
    {
        ParticleSystem particleSystem = Instantiate(hitParticle, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        if (particleSystem != null) particleSystem.Play();
        Destroy(particleSystem, 2);
    }
}

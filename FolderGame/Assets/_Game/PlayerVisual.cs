using System.Collections;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float flashTime = 0.5f;
    [SerializeField] private AnimationCurve flashCurve;
    
    [Header("Particles")]
    [SerializeField] private ParticleSystem jumpParticles;
    [SerializeField] private ParticleSystem landingParticles;
    [SerializeField] private ParticleSystem hitParticles;
    
    private Material currentMaterial;
    
    private PlayerController playerController;
    private PlayerStats playerStats;

    public void Init(PlayerController player)
    {
        playerController = player;
        playerStats = player.Stats;
        
        currentMaterial = new Material(spriteRenderer.material);
        spriteRenderer.material = currentMaterial;
    }

    private void OnEnable()
    {
        if (currentMaterial != null) 
            StartCoroutine(FlashCoroutine());
    }
    
    private IEnumerator FlashCoroutine()
    {
        float currentFlashAmount = 0;
        float elapsedTime = 0;
        
        yield return null;
        
        while (elapsedTime < flashTime)
        {
            elapsedTime += Time.deltaTime;
            currentFlashAmount = Mathf.Lerp(1, flashCurve.Evaluate(elapsedTime), elapsedTime / flashTime);
            PlayFlash(currentFlashAmount);
            yield return null;
        }
    }

    private void PlayFlash(float amount)
    {
        currentMaterial.SetFloat("_HitEffectBlend", amount);
    }

    public void SetOutline(bool enabled)
    {
        
    }
    
    public void PlayJumpParticles()
    {
        jumpParticles.Play();
    }
    
    public void PlayLandingParticles()
    {
        landingParticles.Play();
    }
    
    public void PlayHitParticles()
    {
        hitParticles.Play();
    }

    public void StopAllParticles()
    {
        jumpParticles.Stop();
        landingParticles.Stop();
        hitParticles.Stop();
    }
}

using System.Collections;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [Header("Particles")]
    [SerializeField] private ParticleSystem jumpParticles;
    [SerializeField] private ParticleSystem landingParticles;
    [SerializeField] private ParticleSystem hitParticles;
    
    [Header("Flash")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Material flashMaterial;
    [SerializeField] private float flashTime = 0.5f;
    [SerializeField] private AnimationCurve flashCurve;

    private Coroutine flashTransition;
    
    private Material currentMaterial;
    
    private PlayerController playerController;
    private PlayerStats playerStats;
    
    private Animator animator;

    public void Init(PlayerController player)
    {
        playerController = player;
        playerStats = player.Stats;
        
        currentMaterial = new Material(flashMaterial);
        spriteRenderer.material = currentMaterial;
        
        animator = GetComponent<Animator>();
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
    
    public void SetCutsceneFlash(bool locked)
    {
        if (currentMaterial == null) return;

        if (flashTransition != null)
            StopCoroutine(flashTransition);

        float target = locked ? 1 : 0f;

        flashTransition = StartCoroutine(AnimateCutsceneFlash(target));
    }

    private IEnumerator AnimateCutsceneFlash(float target)
    {
        float start = currentMaterial.GetFloat("_HitEffectBlend");
        float elapsed = 0f;

        while (elapsed < flashTime)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsed / flashTime);
            float amount = Mathf.Lerp(start, target, progress);

            PlayFlash(amount);

            yield return null;
        }

        PlayFlash(target);
        flashTransition = null;
    }

    private void PlayFlash(float amount)
    {
        currentMaterial.SetFloat("_HitEffectBlend", amount);
    }

    public void SetOutline(bool enabled)
    {
        
    }

    public void Flip(float direction)
    {
        if (direction > 0.1f) spriteRenderer.flipX = false;
        else if (direction < -0.1f) spriteRenderer.flipX = true;
    }

    public void SetJump(bool state)
    {
        animator.SetBool("Jump", state);
    }

    public void SetMove(int direction)
    {
        animator.SetFloat("Move", direction);
    }

    public void SetDeath(bool state)
    {
        if (currentMaterial != null) 
            StartCoroutine(FlashCoroutine());
        
        animator.SetBool("IsDead", state);
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

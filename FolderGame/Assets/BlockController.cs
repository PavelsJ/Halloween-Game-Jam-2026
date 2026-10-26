using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    [Header("Block Settings")]
    [SerializeField] private List<Interactable> interaction;
    public int LayerCount => interaction.Count;

    [Header("Block Visual")]
    [SerializeField] private SpriteRenderer outline;
    [SerializeField] private Material hitMaterial;
    [SerializeField] private float flashTime = 0.5f;
    [SerializeField] private AnimationCurve flashCurve;
    
    private Material currentMaterial;

    private void Awake()
    {
        if (outline != null)
        {
            Material newMaterial = outline.material;
            outline.material = new Material(newMaterial);
        }
        
        currentMaterial = new Material(hitMaterial);
        
        foreach (Interactable i in interaction)
        {
            if (i != null)
                i.Init(this, currentMaterial);
        }
    }

    private void OnEnable()
    {
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator Start()
    {
        yield return null;
        
        LayerManager.Instance.Register(this);
    }

    private void OnDestroy()
    {
        if (LayerManager.Instance != null)
            LayerManager.Instance.Unregister(this);
        
        if (currentMaterial != null)
            Destroy(currentMaterial);
    }

    public void SortLayers(int index)
    {
        foreach (Interactable i in interaction)
        {
            if (i == null) continue;

            i.SetLayer(index);
            index++;
        }
    }

    public void BringToFront()
    {
        LayerManager.Instance.BringToFront(this);
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
        outline.enabled = enabled;
    }
}
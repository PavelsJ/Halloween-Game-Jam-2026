using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class VirusInteraction : Interactable
{
    [SerializeField] private float damage;

    [Header("Virus Shake")] 
    [SerializeField] private bool canShake = true;
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeMagnitude = 0.1f;

    private Vector3 originalPosition;
    private Coroutine shakeCoroutine;
    
    private void Awake()
    {
        originalPosition = transform.localPosition;
    }
    
    public override bool CanInteract()
    {
        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.TryGetComponent(out PlayerController player))
            {
                Vector2 hitDirection = new Vector2(other.transform.position.x - transform.position.x, 1).normalized;

                player.TakeDamage(hitDirection, damage, 200);
                
                CameraManager.Instance.ShakeCamera(shakeDuration, 0.05f);
                
                if (!canShake) return;
                if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
                shakeCoroutine = StartCoroutine(Shake());
            }
        }
    }
    private IEnumerator Shake()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            transform.localPosition = originalPosition + (Vector3)Random.insideUnitCircle * shakeMagnitude;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
        shakeCoroutine = null;
    }
}

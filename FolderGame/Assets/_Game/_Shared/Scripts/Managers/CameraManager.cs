
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    
    private const float shakeCooldown = 0.2f;
    private float lastShakeTime = -Mathf.Infinity;
    
    private Vector3 originalPosition;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        
        originalPosition = transform.position;
    }

    public void ShakeCamera(float duration, float magnitude)
    {
        if (Time.time - lastShakeTime < shakeCooldown)
            return;

        lastShakeTime = Time.time;
        StartCoroutine(ShakeEffect(duration, magnitude));
    }
    
    private IEnumerator ShakeEffect(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            transform.position = originalPosition + new Vector3(offsetX, offsetY, -10);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
    }
    
    public void ShakeCameraIncreasing(float duration, float magnitude)
    {
        if (Time.time - lastShakeTime < shakeCooldown)
            return;

        lastShakeTime = Time.time;
        StartCoroutine(ShakeEffectIncreasing(duration, magnitude));
    }

    private IEnumerator ShakeEffectIncreasing(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float progress = elapsed / duration;
            float currentMagnitude = magnitude * progress;

            float offsetX = Random.Range(-1f, 1f) * currentMagnitude;
            float offsetY = Random.Range(-1f, 1f) * currentMagnitude;

            transform.position = originalPosition + new Vector3(offsetX, offsetY, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
    }
}


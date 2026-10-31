using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionEffect : MonoBehaviour
{
    public bool playOnAwake = true;
    
    [Header("Movement")]
    [SerializeField] private float amplitude = 0.1f;
    [SerializeField] private float cycleDuration = 4f;
    [SerializeField] private float rotateSpeed = 4f;
    
    [Header("Target Movement")]
    [SerializeField] private float targetMoveSpeed = 3f;
    [SerializeField] private float knockbackDistance = 0.3f;
    [SerializeField] private float knockbackDuration = 0.2f;
    [SerializeField] private float accelerationDuration = 0.8f;
  
    private Vector3 targetPosition;
    private Coroutine targetMoveCoroutine;
    private bool isMovingToTarget;
    
    private Vector3 startPosition;
    private Vector3 shakeOffset;
    
    private float timer;
    private Coroutine shakeCoroutine;

    public bool isMoving = true;
    public bool isRotating = false;
    
    private IEnumerator Start()
    {
        if (!playOnAwake) yield break;
        
        yield return null;

        StartMotion();
    }

    private void OnEnable()
    {
        if (!playOnAwake) return;
        
        StartMotion();
    }

    public void StartMotion()
    {
        startPosition = transform.position;
        timer = Random.Range(0f, cycleDuration);

        StopAllCoroutines();
        shakeCoroutine = null;
        shakeOffset = Vector3.zero;

        StartCoroutine(MoveRoutine());
        StartCoroutine(RotateRoutine());
    }
    
    public void StopMotion()
    {
        StopAllCoroutines();

        shakeCoroutine = null;
        shakeOffset = Vector3.zero;

        transform.position = startPosition;
    }

    private IEnumerator MoveRoutine()
    {
        while (true)
        {
            timer += Time.deltaTime;

            float yOffset = isMoving ? Mathf.Sin((timer / cycleDuration) * 2f * Mathf.PI) * amplitude : 0f;
            transform.position = startPosition + Vector3.up * yOffset + shakeOffset;

            yield return null;
        }
    }

    private IEnumerator RotateRoutine()
    {
        while (true)
        {
            if (isRotating)
            {
                transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
            }

            yield return null;
        }
    }
    
    public void Shake(float shakeDuration, float shakeIntensity)
    {
        if (!isActiveAndEnabled) return;

        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeRoutine(shakeDuration, shakeIntensity));
    }

    private IEnumerator ShakeRoutine(float shakeDuration, float shakeIntensity)
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float strength = 1f - Mathf.Clamp01(elapsed / shakeDuration);
            shakeOffset = Random.insideUnitSphere * (shakeIntensity * strength);
            shakeOffset.z = 0f;

            yield return null;
        }
        shakeOffset = Vector3.zero;
        shakeCoroutine = null;
    }

    public void MoveTowards(Vector3 target)
    {
        targetPosition = target;
        isMovingToTarget = true;

        if (targetMoveCoroutine != null) StopCoroutine(targetMoveCoroutine);
        targetMoveCoroutine = StartCoroutine(MoveTowardsRoutine(target));
    }
    
    private IEnumerator MoveTowardsRoutine(Vector3 destination)
    {
        Vector3 initialPosition = startPosition;
        Vector3 direction = (destination - initialPosition).normalized;
        
        Vector3 knockbackPosition = initialPosition - direction * knockbackDistance;
        float elapsed = 0f;

        while (elapsed < knockbackDuration)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsed / knockbackDuration);
            progress = Mathf.SmoothStep(0f, 1f, progress);

            startPosition = Vector3.Lerp(initialPosition, knockbackPosition, progress);
            yield return null;
        }

        startPosition = knockbackPosition;
        elapsed = 0f;

        while (elapsed < accelerationDuration)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsed / accelerationDuration);
            
            float easedProgress = progress * progress;
            startPosition = Vector3.Lerp(knockbackPosition, destination, easedProgress);

            yield return null;
        }

        startPosition = destination;
        targetMoveCoroutine = null;
    }
}

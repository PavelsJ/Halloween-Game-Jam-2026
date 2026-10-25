using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPreview : MonoBehaviour
{
    [Header("Preview Settings")]
    [SerializeField] SpriteRenderer previewRenderer;
    [SerializeField] private LayerMask interactableLayer;
    private Vector3 targetPosition;
    
    [Header("Preview Shake")]
    public float shakeMagnitude = 0.2f;
    
    private Vector3 shakeOffset;
    private Coroutine shakeCoroutine;
        
    [Header("Preview Visual")]
    [SerializeField] private Sprite defaultPreviewSprite;
    [SerializeField] private Sprite pointPreviewSprite;
    private bool isHoveringInteractable;
    
    [Header("Click Effect")]
    [SerializeField] private float darkenMultiplier = 0.7f;
    private bool isClicking;
    
    private void Awake()
    {
        Cursor.visible = false;
        
        if (previewRenderer != null)
            previewRenderer.sprite = defaultPreviewSprite;
    }

    private void OnDisable()
    {
        Cursor.visible = true;
    }

    private void Start()
    {
        ShowPreview(1);
    }
    
    public virtual void UpdatePreview(Vector3 worldPos)
    {
        if (previewRenderer == null || !previewRenderer.enabled) return;
        
        UpdatePosition(worldPos);
        CheckInteractable(worldPos);
        UpdateCursorVisual();
    }

    private void UpdatePosition(Vector3 worldPos)
    {
        targetPosition = worldPos;
        transform.position = targetPosition + shakeOffset;
    }

    private void UpdateCursorVisual()
    {
        previewRenderer.sprite = isHoveringInteractable ? pointPreviewSprite : defaultPreviewSprite;
        
        if (InputManager.Instance.PressInput)
        {
            previewRenderer.color = new Color(darkenMultiplier, darkenMultiplier, darkenMultiplier, 1f);
        }
        else if (InputManager.Instance.ReleaseInput)
        {
            previewRenderer.color = Color.white;
        }
    }
    
    private void CheckInteractable(Vector3 worldPos)
    {
        if (previewRenderer == null) return;

        Collider2D hit = Physics2D.OverlapPoint(worldPos, interactableLayer);

        IInteractable interactable = hit != null ? hit.GetComponent<IInteractable>() : null;
        isHoveringInteractable = interactable != null && interactable.CanInteract();
    }

    public void ShowPreview(float newAlpha)
    {
        if (previewRenderer == null) return;
        previewRenderer.enabled = true;
        SetAlpha(newAlpha);
    }

    public void ClearPreview()
    {
        if (previewRenderer == null) return;
        previewRenderer.enabled = false;
    }

    public void ChangePreview(Sprite previewSprite, float newAlpha)
    {
        if (previewRenderer == null) return;
        
        previewRenderer.enabled = true;
        previewRenderer.sprite = previewSprite;
        
        SetAlpha(newAlpha);
    }

    public void HidePreview(float newAlpha)
    {
        if (previewRenderer == null) return;
        SetAlpha(newAlpha);
    }
    
    private void SetAlpha(float alpha)
    {
        if (previewRenderer == null) return;

        Color c = previewRenderer.color;
        c.a = alpha;
        previewRenderer.color = c;
    }

    public void ShakePreview(float duration)
    {
        if (previewRenderer == null || !previewRenderer.enabled)
            return;

        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ShakeRoutine(duration));
    }

    private IEnumerator ShakeRoutine(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            shakeOffset = new Vector3(Random.Range(-1f, 1f) * shakeMagnitude, Random.Range(-1f, 1f) * shakeMagnitude, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero;
        shakeCoroutine = null;
    }
}

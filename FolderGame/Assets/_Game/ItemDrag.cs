using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class ItemDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    
    private CanvasGroup canvasGroup;
    private Canvas rootCanvas;
    
    private Transform originalParent;
    private Vector2 originalPosition;
    private Vector2 pointerOffset;
    
    private bool isDragging;
    
    private ItemSlot slotController;
    private Coroutine returnRoutine;
    
    
    public void Init(ItemSlot controller)
    {
        slotController = controller;
        
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        RefreshReferences();
    }
    
    
    private void RefreshReferences()
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();

        Canvas parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null) rootCanvas = parentCanvas.rootCanvas;

        originalParent = transform.parent;

        if (rectTransform != null)
            originalPosition = rectTransform.anchoredPosition;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        RefreshReferences();
        
        if (originalParent == null) return;
        if (rootCanvas == null) return;
            
        if (!slotController.IsFilled)
        {
            eventData.pointerDrag = null;
            return;
        }
        
        isDragging = true;
        slotController.SelectSlot();
        
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.7f;

        if (returnRoutine != null)
        {
            StopCoroutine(returnRoutine);
            rectTransform.anchoredPosition = slotController.originalPosition;
        }
        
        rectTransform.SetParent(rootCanvas.transform, false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (originalParent == null) return;
        if (rootCanvas == null) return;
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            slotController.slotImage.canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out var currentPointer);
        
        Vector2 delta = currentPointer - pointerOffset;
        slotController.dragOffset = delta;
        slotController.UpdateCombinedOffset();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (originalParent == null) return;
        if (rootCanvas == null) return;
        
        isDragging = false;
        
        rectTransform.SetParent(originalParent, true);
        slotController.dragOffset = rectTransform.anchoredPosition;
        
        slotController.UpdateCombinedOffset();

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
        
        if (!slotController.IsFilled) return;
        CheckItem(eventData);
    }

    private void CheckItem(PointerEventData eventData)
    {
        if (eventData.pointerEnter != null)
        {
            IItemReceiver receiver = eventData.pointerEnter.GetComponent<IItemReceiver>();

            if (receiver != null)
            {
                receiver.ReceiveItem(slotController);
                slotController.Selection.PlayConsumableParticles();
                return;
            }
        }

        if (returnRoutine != null) StopCoroutine(returnRoutine);
        returnRoutine = StartCoroutine(ItemReturn(slotController.originalPosition, 0.15f));
    }
    
    public bool IsDragging()
    {
        return isDragging;
    }
    
    private IEnumerator ItemReturn(Vector2 targetOffset, float duration)
    {
        Vector2 startOffset = slotController.dragOffset;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            
            slotController.dragOffset = Vector2.Lerp(startOffset, targetOffset, elapsed / duration);
            slotController.UpdateCombinedOffset();
            yield return null;
        }

        slotController.dragOffset = targetOffset;
        slotController.UpdateCombinedOffset();
        slotController.Selection.PlayConsumableParticles();
        returnRoutine = null;
    }
}
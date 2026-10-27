using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSelection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float moveDuration = 0.2f;
    
    [Header("UI Particles")]
    public ParticleSystem consumableParticles;
    
    private Button itemButton;
    private bool isSelected;
    
    private Vector3 selectedOffset = new Vector3(0, 0.2f, 0);
    private Coroutine moveCoroutine;
    
    private ItemSlot slotController;
    
    public void Init(ItemSlot controller)
    {
        slotController = controller;
        
        itemButton = GetComponent<Button>();
        itemButton.onClick.AddListener(slotController.SelectSlot);
        
        PlayConsumableParticles();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (slotController == null) return;
        
        if (!isSelected)
        {
            UpdateHighlightedImage(true, 0.5f);
            MusicManager.instance.PlayRandomSound(MusicManager.SoundType.ButtonSelection);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (slotController == null) return;
        
        if (!isSelected)
        {
            UpdateHighlightedImage(false);
        }
    }
    
    public void PlayConsumableParticles()
    {
        consumableParticles.Play();
    }
    
    internal void SetSelected(bool state)
    {
        isSelected = state;
        UpdateHighlightedImage(state);
    }

    private void UpdateHighlightedImage(bool selection, float alpha = 1)
    {
        Vector3 target = selection ? selectedOffset : Vector3.zero;
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveIconSmoothly(target));
    }
    
    private IEnumerator MoveIconSmoothly(Vector3 targetOffset)
    {
        Vector3 start = slotController.selectionOffset;
        
        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / moveDuration);
            slotController.selectionOffset = Vector3.Lerp(start, targetOffset, t);
            slotController.UpdateCombinedOffset();
            yield return null;
        }

        slotController.selectionOffset = targetOffset;
        slotController.UpdateCombinedOffset();
    }
    
    public bool IsSelected()
    {
        return isSelected;
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ItemSlot : MonoBehaviour
{
    public Image slotImage;
    public TMP_Text slotText;
    
    public ItemSelection Selection { get; private set; }
    public ItemDrag Drag { get; private set; }
    
    private ItemData currentItem;
    private int currentIndex = 0;
    
    public bool IsFilled => currentItem != null;
    
    [HideInInspector] public Vector3 selectionOffset;
    [HideInInspector] public Vector3 dragOffset;
    [HideInInspector] public Vector3 originalPosition;

    public void Init(ItemData slot, int index)
    {
        currentItem = slot;
        currentIndex = index;

        if (currentItem == null) return;
        slotImage.sprite = currentItem.ItemIcon;
        slotText.text = currentItem.ItemName;
        
        slotImage.gameObject.SetActive(true);
        
        Selection = GetComponent<ItemSelection>();
        Drag = slotImage.GetComponent<ItemDrag>();
        
        Selection.Init(this);
        Drag.Init(this);
        
        originalPosition = slotImage.rectTransform.anchoredPosition;
    }
    
    public void UpdateCombinedOffset()
    {
        slotImage.rectTransform.anchoredPosition = originalPosition + selectionOffset + dragOffset;
    }
    
    public virtual void SelectSlot()
    {
        if (GetItemData() == null) return;
        if (Selection.IsSelected() && !Drag.IsDragging())
        {
            DeselectSlot();
            return;
        }
            
        InventoryManager.Instance.SelectSlot(this);
        Selection.SetSelected(true);
    }
    
    public virtual void DeselectSlot()
    {
        if (GetItemData() == null) return;
        InventoryManager.Instance.DeselectSlot();
        Selection.SetSelected(false);
    }
    
    public virtual void DeleteSlot()
    {
        if (GetItemData() == null) return;

        if (Selection.IsSelected())
        {
            InventoryManager.Instance.DeselectSlot();
            Selection.SetSelected(false);
        }

        ClearSlot();
    }
    
    public void ClearSlot()
    {
        currentItem = null;
        slotImage.sprite = null;
        slotText.text = "";
        
        slotImage.gameObject.SetActive(false);
    }

    public ItemData GetItemData()
    {
        if (currentItem == null) return null;
        return currentItem;
    }
}

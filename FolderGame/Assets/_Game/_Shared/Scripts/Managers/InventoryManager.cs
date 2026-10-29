using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("Essential Elements")]
    public static InventoryManager Instance { get; private set; }
    
    private List<ItemData> items = new List<ItemData>();
    [SerializeField] private List<ItemData> allItemData = new List<ItemData>();
    [SerializeField] private List<ItemSlot> itemSlots = new List<ItemSlot>();
    
    public IReadOnlyList<ItemData> Items => items;
    private ItemSlot selectedSlot;
    
    [Header("Instruction Settings")]
    [SerializeField] private float instructionTime = 5f;
    [SerializeField] private GameObject instructionPanel;
    [SerializeField] private TMP_Text instructionText;
    
    private Coroutine instructionCoroutine;
    
    private const string SaveKey = "InventorySave";
    
    [Serializable] private class SavedSlot
    {
        public int slotId;
        public string itemId;
    }
    
    [Serializable] private class InventorySaveData
    {
        public List<SavedSlot> slots = new List<SavedSlot>();
    }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    
    private IEnumerator Start()
    {
        yield return null;
        
        if (PlayerPrefs.HasKey(SaveKey))
        {
            LoadInventory();
        }
        else
        {
            FillInventory();
        }
    }
    
    public void SaveInventory()
    {
        InventorySaveData saveData = new InventorySaveData();

        for (int i = 0; i < itemSlots.Count; i++)
        {
            ItemSlot slot = itemSlots[i];

            if (slot == null || !slot.IsFilled) continue;

            ItemData item = slot.GetItemData();
            if (item == null) continue;

            saveData.slots.Add(new SavedSlot
            {
                slotId = i,
                itemId = item.ItemID.ToString()
            });
        }

        PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(saveData));
        PlayerPrefs.Save();
    }

    public void LoadInventory()
    {
        string json = PlayerPrefs.GetString(SaveKey);
        InventorySaveData saveData = JsonUtility.FromJson<InventorySaveData>(json);

        if (saveData == null || saveData.slots == null)
        {
            FillInventory();
            return;
        }

        StopAllCoroutines();
        StartCoroutine(RestoreInventoryCoroutine(saveData));
    }

    private IEnumerator RestoreInventoryCoroutine(InventorySaveData saveData)
    {
        foreach (ItemSlot slot in itemSlots)
        {
            if (slot != null) 
                slot.ClearSlot();
        }

        foreach (SavedSlot savedSlot in saveData.slots)
        {
            if (savedSlot.slotId < 0 || savedSlot.slotId >= itemSlots.Count) continue;

            ItemSlot slot = itemSlots[savedSlot.slotId];
            if (slot == null) continue;
            
            ItemData item = FindItemById(savedSlot.itemId);

            if (item == null)
            {
                Debug.LogWarning($"Inventory: item with ID '{savedSlot.itemId}' not found.");
                continue;
            }

            int itemIndex = items.IndexOf(item);

            if (itemIndex == -1)
            {
                items.Add(item);
                itemIndex = items.Count - 1;
            }

            slot.Init(item, itemIndex);
        }

        yield return null;
    }

    private ItemData FindItemById(string id)
    {
        foreach (ItemData item in allItemData)
        {
            if (item != null && item.ItemID.ToString() == id)
                return item;
        }

        return null;
    }

    public void DeleteInventorySave()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
    }

    private void FillInventory()
    {
        StopAllCoroutines();
        StartCoroutine(SpawnInventorySlotsCoroutine());
    }
    
    private IEnumerator SpawnInventorySlotsCoroutine()
    {
        foreach (var slot in itemSlots)
        {
            slot.ClearSlot();
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    public void AddItem(ItemData newItem)
    {
        if (newItem == null) return;

        int itemIndex = items.IndexOf(newItem);

        if (itemIndex == -1)
        {
            items.Add(newItem);
            itemIndex = items.Count - 1;
        }

        foreach (var slot in itemSlots)
        {
            if (slot.IsFilled) continue;

            slot.Init(newItem, itemIndex);
            
            if (instructionCoroutine != null) StopCoroutine(instructionCoroutine);
            instructionCoroutine = StartCoroutine(ShowInstructionCoroutine(newItem));

            return;
        }
    }

    public void RemoveItem(ItemSlot slot)
    {
        if (slot == null) return;

        slot.DeleteSlot();

        if (selectedSlot == slot)
            selectedSlot = null;
    }

    public void SelectSlot(ItemSlot item)
    {
        selectedSlot = item;
    }

    public void DeselectSlot()
    {
        selectedSlot = null;
    }
    
    public bool HasItem(ItemData item)
    {
        return items.Contains(item);
    }
    
    public ItemData GetItem(int index)
    {
        if (index < 0 || index >= items.Count) return null;

        return items[index];
    }
    
    private IEnumerator ShowInstructionCoroutine(ItemData item)
    {
        instructionPanel.SetActive(true);
        instructionText.text = $"New Item Added: {item.ItemName} [Esc]";

        yield return new WaitForSeconds(instructionTime);

        instructionPanel.SetActive(false);
        instructionCoroutine = null;
    }
}

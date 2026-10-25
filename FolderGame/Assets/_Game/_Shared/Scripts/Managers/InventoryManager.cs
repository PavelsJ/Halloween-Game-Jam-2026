using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("Essential Elements")]
    public static InventoryManager Instance { get; private set; }
    
    [SerializeField] private List<ItemData> items = new List<ItemData>();
    public IReadOnlyList<ItemData> Items => items;
    
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
        if (items.Count == 0)
            yield break;
        
        yield return null;
    }
    
    public void AddItem(ItemData item)
    {
        if (item == null) return;
        items.Add(item);
    }

    public void RemoveItem(ItemData item)
    {
        if (item == null) return;
        items.Remove(item);
    }

    public void RemoveAllItems()
    {
        items.Clear();
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
}

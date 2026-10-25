using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Item", fileName = "New Item")]
public class ItemData : ScriptableObject
{
    [Header("Item Data")]
    [SerializeField] private int itemID;
    [SerializeField] private Sprite itemIcon;
    [TextArea(1, 2)] [SerializeField] private string itemName;
    [TextArea(2, 5)] [SerializeField] private string itemDescription;

    public int ItemID => itemID;
    public Sprite ItemIcon => itemIcon;
    public string ItemName => itemName;
    public string ItemDescription => itemDescription;
}

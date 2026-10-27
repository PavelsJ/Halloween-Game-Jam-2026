using System.Collections;
using UnityEngine;

public interface IItemReceiver
{
    void ReceiveItem(ItemSlot sourceSlot);
}

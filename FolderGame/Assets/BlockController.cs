using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    [SerializeField] private List<Interactable> interaction;

    public int LayerCount => interaction.Count;

    private void Awake()
    {
        foreach (Interactable i in interaction)
        {
            if (i != null)
                i.Init(this);
        }
    }

    private IEnumerator Start()
    {
        yield return null;
        
        LayerManager.Instance.Register(this);
    }

    private void OnDestroy()
    {
        if (LayerManager.Instance != null)
            LayerManager.Instance.Unregister(this);
    }

    public void SortLayers(int index)
    {
        foreach (Interactable i in interaction)
        {
            if (i == null) continue;

            i.SetLayer(index);
            index++;
        }
    }

    public void BringToFront()
    {
        LayerManager.Instance.BringToFront(this);
    }
}
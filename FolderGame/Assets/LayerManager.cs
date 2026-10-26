using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour
{
    public static LayerManager Instance { get; private set; }

    private readonly List<BlockController> blocks = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Register(BlockController block)
    {
        if (block == null || blocks.Contains(block)) return;

        blocks.Add(block);
        UpdateLayers();
    }

    public void Unregister(BlockController block)
    {
        if (!blocks.Remove(block)) return;

        UpdateLayers();
    }

    public void BringToFront(BlockController block)
    {
        if (block == null) return;

        blocks.Remove(block);
        blocks.Add(block);

        UpdateLayers();
    }

    private void UpdateLayers()
    {
        blocks.RemoveAll(block => block == null);

        int layerIndex = 0;

        foreach (BlockController block in blocks)
        {
            block.SortLayers(layerIndex);
            layerIndex += block.LayerCount;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextEffect : MonoBehaviour
{
    TMP_Text textMesh;
    Mesh mesh;
    Vector3[] vertices;
    
    public bool isActive = false;
    
    public float amplitude = 1f;   // высота волны
    public float frequency = 5f;   // скорость колебаний
    public float waveLength = 0.5f; // расстояние между вершинами волны

    void Start()
    {
        textMesh = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (isActive)
        {
           Wobble();
        }
    }

    private void Wobble()
    {
        textMesh.ForceMeshUpdate();
        mesh = textMesh.mesh;
        vertices = mesh.vertices;

        for (int i = 0; i < textMesh.textInfo.characterCount; i++)
        {
            TMP_CharacterInfo c = textMesh.textInfo.characterInfo[i];
            if (!c.isVisible) continue;

            int index = c.vertexIndex;
            
            float wave = Mathf.Sin(Time.time * frequency + i * waveLength) * amplitude;

            Vector3 offset = new Vector3(0, wave, 0);

            for (int j = 0; j < 4; j++)
                vertices[index + j] += offset;
        }

        mesh.vertices = vertices;
        textMesh.canvasRenderer.SetMesh(mesh);
    }
}
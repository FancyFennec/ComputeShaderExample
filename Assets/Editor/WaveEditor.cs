using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Wave))]
public class WaveEditor : Editor
{
    ComputeShader perlinNoise;
    int perlinNoiseHandle;
    int resolution = 256;
    RenderTexture texture;
    private void OnEnable()
    {
        texture = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGB32)
        {
            enableRandomWrite = true,
        };
        texture.Create();
        perlinNoise = (ComputeShader)Resources.Load("PerlinNoise");
        perlinNoiseHandle = perlinNoise.FindKernel("CSMain");
        perlinNoise.SetTexture(perlinNoiseHandle, "Result", texture);
        perlinNoise.Dispatch(perlinNoiseHandle, resolution / 8, resolution / 8, 1);
    }

    private void OnSceneGUI()
    {
        
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.ObjectField("Texture", texture, typeof(Texture2D), false);
        EditorGUILayout.EndHorizontal();
    }
}

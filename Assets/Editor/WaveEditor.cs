using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Wave))]
public class WaveEditor : Editor
{
    Wave wave;
    ComputeShader perlinNoise;
    int perlinNoiseHandle;
    int resolution = 256;
    public RenderTexture texture;
    private void OnEnable()
    {
        wave = (Wave)target;

        texture = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.RFloat)
        {
            enableRandomWrite = true
        };
        texture.Create();

        perlinNoise = (ComputeShader)Resources.Load("PerlinNoise");
        perlinNoiseHandle = perlinNoise.FindKernel("CSMain");
        perlinNoise.SetTexture(perlinNoiseHandle, "Result", texture);
        perlinNoise.SetFloat("res", (float) resolution);
        perlinNoise.Dispatch(perlinNoiseHandle, resolution / 8, resolution / 8, 1);

		Renderer rend = wave.GetComponent<Renderer>();
        rend.enabled = true;
        rend.sharedMaterial.SetTexture("_Texture2D", texture);
    }

    private void OnSceneGUI()
    {
        
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.ObjectField("Texture", texture, typeof(Texture), false);
        EditorGUILayout.EndHorizontal();
    }
}

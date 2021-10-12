using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

		ComputeBuffer gradients = new ComputeBuffer(256, sizeof(float) * 2);
		gradients.SetData(Enumerable.Range(0, 256).Select((i) => GetRandomDirection()).ToArray());

		texture = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.RFloat)
		{
			enableRandomWrite = true
		};
		texture.Create();

		perlinNoise = (ComputeShader)Resources.Load("PerlinNoise");
		perlinNoiseHandle = perlinNoise.FindKernel("CSMain");
		perlinNoise.SetTexture(perlinNoiseHandle, "Result", texture);
		perlinNoise.SetFloat("res", (float)resolution);
		perlinNoise.SetFloat("t", (float) EditorApplication.timeSinceStartup);
		perlinNoise.SetBuffer(perlinNoiseHandle, "gradients", gradients);
		perlinNoise.Dispatch(perlinNoiseHandle, resolution / 8, resolution / 8, 1);

		Renderer rend = wave.GetComponent<Renderer>();
		rend.enabled = true;
		rend.sharedMaterial.SetTexture("_Texture2D", texture);
	}

	private static Vector2 GetRandomDirection()
	{
		return new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
	}

	private void OnSceneGUI()
    {
		perlinNoise.SetFloat("t", (float)EditorApplication.timeSinceStartup);
		perlinNoise.Dispatch(perlinNoiseHandle, resolution / 8, resolution / 8, 1);
	}

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.ObjectField("Texture", texture, typeof(Texture), false);
        EditorGUILayout.EndHorizontal();
    }
}

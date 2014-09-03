using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class BoostRadius : MonoBehaviour {

	public Shader BlurRadiusShader;
	public float BlurStrength = 1;
	public float SampleStrength = 1;
	private Material curMaterial;


	Material material
	{
		get
		{
			if (curMaterial == null)
			{
				curMaterial = new Material(BlurRadiusShader);
				curMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return curMaterial;
		}
	}
	
	void Start () 
	{
		if (!SystemInfo.supportsImageEffects)
		{
			enabled = true;
			return;
		}
	}

	void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
	{
		if(BlurRadiusShader != null)
		{
			material.SetFloat("_BlurStrength", BlurStrength);
			material.SetFloat("_SampleStrength", SampleStrength);
			Graphics.Blit(sourceTexture, destTexture, material);
		} else 
		{
			Graphics.Blit(sourceTexture, destTexture);
		}
	}

	void OnDisable()
	{
		if (curMaterial)
		{
			DestroyImmediate(curMaterial);
		}
	}
}

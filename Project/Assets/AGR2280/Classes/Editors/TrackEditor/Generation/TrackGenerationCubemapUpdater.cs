using UnityEngine;
using System.Collections;

public class TrackGenerationCubemapUpdater : MonoBehaviour {

	int cubemapSize = 512;
	private GameObject CMCam;
	private Cubemap cubeMap;
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.U))
		{
			UpdateCubemap();
		}
	}

	public void UpdateCubemap()
	{
		/* Set Cubemap */
		if (cubeMap == null)
		{
			cubeMap = new Cubemap(cubemapSize, TextureFormat.RGB24,false);
		}


		/* Setup Camera */
		CMCam = new GameObject();
		CMCam.name = "CubemapCamera";
		CMCam.gameObject.AddComponent<Camera>();
		CMCam.hideFlags = HideFlags.HideAndDontSave;
		CMCam.transform.position = transform.position;
		CMCam.transform.rotation = Quaternion.identity;
		CMCam.GetComponent<Camera>().nearClipPlane = 0.001f;
		CMCam.GetComponent<Camera>().farClipPlane = 80000;
		CMCam.GetComponent<Camera>().aspect = 1.0f;
		CMCam.GetComponent<Camera>().hdr = true;

		CMCam.GetComponent<Camera>().RenderToCubemap(cubeMap);

		/* Pass Camera to RenderTexture */
		for (int i = 0; i < renderer.materials.Length; i++)
		{
			renderer.materials[i].SetTexture("_Cube", cubeMap);
		}
		
		DestroyImmediate(CMCam);
	
	}
}

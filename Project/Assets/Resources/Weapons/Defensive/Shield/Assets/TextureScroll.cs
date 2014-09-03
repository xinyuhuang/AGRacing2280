using UnityEngine;
using System.Collections;

public class TextureScroll : MonoBehaviour {

	float Offset;
	void Update () 
	{
		Offset += 10;
		renderer.material.mainTextureOffset = new Vector3(Offset, 0);
	}
}

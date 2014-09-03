using UnityEngine;
using System.Collections;

public class NoRender : MonoBehaviour {

	/* Attach this script to an object if you do not want it to be rendered ingame */

	void Start()
	{
		renderer.enabled = false;
	}
}

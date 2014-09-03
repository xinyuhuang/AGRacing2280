using UnityEngine;
using UnityEditor;
using System.Collections;

public class NodeInformation : MonoBehaviour {

	public float trackWidth;

	void OnDrawGizmosSelected()
	{
		transform.localScale = new Vector3(trackWidth, transform.localScale.y, transform.localScale.z);
	}

	public void SetFirstSize()
	{
		transform.localScale = new Vector3(trackWidth, transform.localScale.y, transform.localScale.z);
	}
}

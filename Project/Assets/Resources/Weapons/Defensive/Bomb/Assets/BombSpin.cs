using UnityEngine;
using System.Collections;

public class BombSpin : MonoBehaviour {

	void Update () 
	{
		transform.Rotate(Vector3.up * 100);
	}
}

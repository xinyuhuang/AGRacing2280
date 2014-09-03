using UnityEngine;
using System.Collections;

public class AGRLaserTool : MonoBehaviour {

	float lastWidth;
	public float width;
	bool updated;

	public GameObject LeftModel;
	public GameObject RightModel;

	public Material LineMaterial;
	public bool isFront;

	void Start()
	{
		if (isFront)
		{
			this.gameObject.AddComponent<LineRenderer>();
			GetComponent<LineRenderer>().material = LineMaterial;
			GetComponent<LineRenderer>().SetColors(Color.cyan, Color.cyan);
		}
	}

	void Update () 
	{
		if (lastWidth != width)
		{
			updated = false;
			lastWidth = width;
		}
		
		if (!updated)
		{
			// Zero out the model local positions
			LeftModel.transform.localPosition = Vector3.zero;
			RightModel.transform.localPosition = Vector3.zero;
			
			// Offset the models
			LeftModel.transform.localPosition = new Vector3(-width, 0, 0);
			RightModel.transform.localPosition = new Vector3(width, 0, 0);

			// Rotate the models
			LeftModel.transform.localRotation = Quaternion.Euler(0, -90, 0);
			RightModel.transform.localRotation = Quaternion.Euler(0, 90, 0);

			// Update Line Render
			if (isFront)
			{			
				GetComponent<LineRenderer>().SetPosition(0, new Vector3(LeftModel.transform.position.x, LeftModel.transform.position.y + 6.2f, LeftModel.transform.position.z));
				GetComponent<LineRenderer>().SetPosition(1, new Vector3(RightModel.transform.position.x, RightModel.transform.position.y + 6.2f, RightModel.transform.position.z));
			}
			
			
			updated = true;
		}
	}
}

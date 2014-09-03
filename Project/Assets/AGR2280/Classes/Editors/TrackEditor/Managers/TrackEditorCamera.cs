using UnityEngine;
using System.Collections;

public class TrackEditorCamera : MonoBehaviour {

	/* Maya Controls */
	private bool holdingAlt;
	private bool mayaControlisRotating;
	private bool mayaControlisPanning;
	private bool mayaControlisZooming;

	void Start()
	{
		/* Load Objects for Catalogue */
		TrackEditorObjectCatalogueLists.LoadObjects();
	}

	private void PlayerInput()
	{
		/* Maya Controls - Reset */
		holdingAlt = false;

		if (Input.GetKey(KeyCode.LeftAlt)) { holdingAlt = true; }
		if (holdingAlt && Input.GetMouseButton(0))
		{
			mayaControlisRotating = true;
		} else
		{
			mayaControlisRotating = false;
		}

		if (holdingAlt && Input.GetMouseButton(1))
		{
			mayaControlisZooming = true;
		} else
		{
			mayaControlisZooming = false;
		}

		if (holdingAlt && Input.GetMouseButton(2))
		{
			mayaControlisPanning = true;
		} else
		{
			mayaControlisPanning = false;
		}
	}

	void Update()
	{
		/* Get Input */
		PlayerInput();

		/* Apply Maya Controls */
		if (mayaControlisRotating)
		{
			/* Up Vector Rotation */
			transform.Translate(Vector3.right * -Input.GetAxis("Mouse X") * 7);
			transform.Rotate (Vector3.up * Input.GetAxis("Mouse X"));

			/* Right Vector Rotation */
			transform.Translate(Vector3.up * -Input.GetAxis("Mouse Y") * 7);
			transform.Rotate (Vector3.right * -Input.GetAxis("Mouse Y"));

			/* Zero Out Z Axis */
			transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);
		}

		if (mayaControlisPanning)
		{
			/* Right Axis Movement */
			transform.Translate(Vector3.right * -Input.GetAxis("Mouse X") * 4);

			/* Up Axis Movement */
			transform.Translate(Vector3.up * -Input.GetAxis("Mouse Y") * 4);
		}

		if (mayaControlisZooming)
		{
			/* Forward Axis Movement */
			transform.Translate(Vector3.forward * -Input.GetAxis("Mouse Y"));
		}
	}
}

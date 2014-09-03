using UnityEngine;
using System.Collections;

public class AGRShipVisualsManager : AGRShipEngineSoundModule {

	/* This class is part of the manager module set
	 * and manages the visuals for the ship! */

	/* Wind Line Containers */
	private GameObject LWLC;
	private GameObject RWLC;
	private float WLRotation;
	private Color WLOpacity;

	/* Wind Line Mesh */
	private Mesh WindLineMesh;

	/* CS Forces */
	private float CSX;
	private float CSZ;

	/* Slipstream*/
	private float slipstreamLifetime;
	private float lensflareIntensity;
	private float lightIntensity;

	/* Airbrakes */
	private float leftABAmount;
	private float leftABUpVelocity;
	private float leftABDownVelocity;
	private float rightABAmount;
	private float rightABUpVelocity;
	private float rightABDownVelocity;

	public void CreateVisualObjects()
	{
		/* Create WindLineMesh */
		GameObject tempMesh = GameObject.CreatePrimitive(PrimitiveType.Plane);
		WindLineMesh = tempMesh.GetComponent<MeshFilter>().mesh;
		Destroy(tempMesh);

		/* Create and setup wind line container */
		LWLC = new GameObject("LWLC");
		LWLC.transform.parent = ShipGroupContainer.transform;

		/* Create and parent left wind line */
		shipVisualManagerWindLineLeft = new GameObject("WLL");
		shipVisualManagerWindLineLeft.transform.parent = ShipGroupContainer.transform.Find("LWLC");

		/* Add mesh filter with plane gameobject */
		shipVisualManagerWindLineLeft.AddComponent<MeshFilter>();
		shipVisualManagerWindLineLeft.GetComponent<MeshFilter>().mesh = WindLineMesh;

		shipVisualManagerWindLineLeft.AddComponent<MeshRenderer>();
		shipVisualManagerWindLineLeft.GetComponent<MeshRenderer>().castShadows = false;
		shipVisualManagerWindLineLeft.GetComponent<MeshRenderer>().receiveShadows = false;

		/* Apply the wind line material */
		shipVisualManagerWindLineLeft.GetComponent<MeshRenderer>().material = Resources.Load("Ships/Materials/ShipWindLine") as Material;

		/* Scale and offset the wind line */
		shipVisualManagerWindLineLeft.transform.localScale = new Vector3(0.03f, 1, 2.14f);
		shipVisualManagerWindLineLeft.transform.localPosition = new Vector3(0,0, 8.12f);

		/* Create and setup wind line container */
		RWLC = new GameObject("RWLC");
		RWLC.transform.parent= ShipGroupContainer.transform;
		
		/* Create and parent left wind line */
		shipVisualManagerWindLineRight = new GameObject("WLR");
		shipVisualManagerWindLineRight.transform.parent = ShipGroupContainer.transform.Find("RWLC");
		
		/* Add mesh filter with plane gameobject */
		shipVisualManagerWindLineRight.AddComponent<MeshFilter>();
		shipVisualManagerWindLineRight.GetComponent<MeshFilter>().mesh = WindLineMesh;
		
		shipVisualManagerWindLineRight.AddComponent<MeshRenderer>();
		shipVisualManagerWindLineRight.GetComponent<MeshRenderer>().castShadows = false;
		shipVisualManagerWindLineRight.GetComponent<MeshRenderer>().receiveShadows = false;
		
		/* Apply the wind line material */
		shipVisualManagerWindLineRight.GetComponent<MeshRenderer>().material = Resources.Load("Ships/Materials/ShipWindLine") as Material;
		
		/* Scale and offset the wind line */
		shipVisualManagerWindLineRight.transform.localScale = new Vector3(0.03f, 1, 2.14f);
		shipVisualManagerWindLineRight.transform.localPosition = new Vector3(0,0, 8.12f);
	}

	public void UpdateVisualObjects()
	{
		/* Update Wind Line Positions (being updated constantly so they can be tweaked in realtime */
		LWLC.transform.localPosition = new Vector3(-shipVisualManagerWindLineOffset.x, shipVisualManagerWindLineOffset.y, shipVisualManagerWindLineOffset.z);
		RWLC.transform.localPosition = shipVisualManagerWindLineOffset;

		WLRotation = Mathf.Lerp(WLRotation, shipCameraModuleShipRotation * 5, Time.deltaTime * 6);
		LWLC.transform.localRotation = Quaternion.Euler(0, 180 - WLRotation, 0);
		RWLC.transform.localRotation = Quaternion.Euler(0, 180 - WLRotation, 0);

		/* Update Wind Line Opacity */
		if (shipVelocityModuleBoostTimer > 0)
		{
			WLOpacity = Color.Lerp(WLOpacity, new Color(1,1,1,1), Time.deltaTime * 8);
		} else
		{
			WLOpacity = Color.Lerp(WLOpacity, new Color(1,1,1,0), Time.deltaTime * 2);
		}

		/* Apply Wind Line Opacity */
		shipVisualManagerWindLineLeft.renderer.material.SetColor("_TintColor", WLOpacity);
		shipVisualManagerWindLineRight.renderer.material.SetColor("_TintColor", WLOpacity);

		/* Rotate control stick */
		CSX = Mathf.Lerp(CSX, GetComponent<PlayerInputManager>().playerInputAxisPitching * 15, Time.deltaTime * 7);
		CSZ = Mathf.Lerp(CSZ, -GetComponent<PlayerInputManager>().playerInputAxisSteering * 15, Time.deltaTime * 7);
		if (shipVisualManagerCockPitControlStick != null)
		{
			shipVisualManagerCockPitControlStick.transform.localRotation = Quaternion.Euler(CSX, 0, CSZ);
		}

		/* Slipstream */
		if (GetComponent<PlayerInputManager>().playerInputButtonThruster)
		{
			slipstreamLifetime = Mathf.Lerp(slipstreamLifetime, 0.31f, Time.deltaTime * 5);
			lensflareIntensity = Mathf.Lerp(lensflareIntensity, 1, Time.deltaTime * 5);
			lightIntensity = Mathf.Lerp(lightIntensity, 1, Time.deltaTime * 5);
		} else
		{
			slipstreamLifetime = Mathf.Lerp(slipstreamLifetime, 0f, Time.deltaTime);
			lensflareIntensity = Mathf.Lerp(lensflareIntensity, 0.5f, Time.deltaTime);
			lightIntensity = Mathf.Lerp(lightIntensity, 0.8f, Time.deltaTime);
		}

		if (shipVelocityModuleBoostTimer > 0 || ((Input.GetButtonDown("[KB] Thruster") || Input.GetButtonDown("[PAD] Thruster")) && shipVelocityModuleVelocity < shipSettingsEngineMaxThrust * 0.1f))
		{
			slipstreamLifetime = Mathf.Lerp(slipstreamLifetime, 0.55f, Time.deltaTime * 50);
			lensflareIntensity = Mathf.Lerp(lensflareIntensity, 2f, Time.deltaTime * 50);
			lightIntensity = Mathf.Lerp(lightIntensity, 3, Time.deltaTime * 50);
		}

		shipVisualsManagerSlipstream.GetComponent<ParticleSystem>().startLifetime = slipstreamLifetime;
		shipVisualsManagerLensflare.GetComponent<LensFlare>().brightness = lensflareIntensity;
		shipVisualsManagerEngineLight.GetComponent<Light>().intensity = lightIntensity;

		/* Airbrakes */
		if (GetComponent<PlayerInputManager>().playerInputActionBraking)
		{
			leftABDownVelocity = 0;
			leftABUpVelocity = Mathf.Lerp(leftABUpVelocity, shipSettingsAirbrakeAnimationUpSpeed, Time.deltaTime * (shipSettingsAirbrakeAnimationUpSpeed / 100));
			leftABAmount = Mathf.MoveTowards(leftABAmount, shipSettingsAirbrakeAnimationMaxAngle, Time.deltaTime * leftABUpVelocity);

			rightABDownVelocity = 0;
			rightABUpVelocity = Mathf.Lerp(rightABUpVelocity, shipSettingsAirbrakeAnimationUpSpeed, Time.deltaTime * (shipSettingsAirbrakeAnimationUpSpeed / 100));
			rightABAmount = Mathf.MoveTowards(rightABAmount, shipSettingsAirbrakeAnimationMaxAngle, Time.deltaTime * rightABUpVelocity);

		} else 
		{
			if (GetComponent<PlayerInputManager>().playerInputAxisAirbrakes > 0)
			{
				leftABDownVelocity = 0;
				leftABUpVelocity = Mathf.Lerp(leftABUpVelocity, shipSettingsAirbrakeAnimationUpSpeed, Time.deltaTime * (shipSettingsAirbrakeAnimationUpSpeed / 100));
				leftABAmount = Mathf.MoveTowards(leftABAmount, GetComponent<PlayerInputManager>().playerInputAxisAirbrakes * shipSettingsAirbrakeAnimationMaxAngle, Time.deltaTime * leftABUpVelocity);
			} else 
			{
				leftABUpVelocity = 0;
				leftABDownVelocity = Mathf.Lerp(leftABUpVelocity, shipSettingsAirbrakeAnimationDownSpeed, Time.deltaTime * (shipSettingsAirbrakeAnimationDownSpeed / 100));
				leftABAmount = Mathf.MoveTowards(leftABAmount, 0, Time.deltaTime * leftABDownVelocity);
			}

			if (GetComponent<PlayerInputManager>().playerInputAxisAirbrakes < 0)
			{
				rightABDownVelocity = 0;
				rightABUpVelocity = Mathf.Lerp(rightABUpVelocity, shipSettingsAirbrakeAnimationUpSpeed, Time.deltaTime * (shipSettingsAirbrakeAnimationUpSpeed / 100));
				rightABAmount = Mathf.MoveTowards(rightABAmount, -GetComponent<PlayerInputManager>().playerInputAxisAirbrakes * shipSettingsAirbrakeAnimationMaxAngle, Time.deltaTime * rightABUpVelocity);
			} else 
			{
				rightABUpVelocity = 0;
				rightABDownVelocity = Mathf.Lerp(rightABUpVelocity, shipSettingsAirbrakeAnimationDownSpeed, Time.deltaTime * (shipSettingsAirbrakeAnimationDownSpeed / 100));
				rightABAmount = Mathf.MoveTowards(rightABAmount, 0, Time.deltaTime * rightABDownVelocity);
			}
		}
		shipVisualsManagerAirbrakeLeft.transform.localRotation = Quaternion.Euler(leftABAmount, shipVisualsManagerAirbrakeLeft.transform.localEulerAngles.y,shipVisualsManagerAirbrakeLeft.transform.localEulerAngles.z);
		shipVisualsManagerAirbrakeRight.transform.localRotation = Quaternion.Euler(rightABAmount, shipVisualsManagerAirbrakeRight.transform.localEulerAngles.y,shipVisualsManagerAirbrakeRight.transform.localEulerAngles.z);

	}
}

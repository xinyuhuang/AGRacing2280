using UnityEngine;
using System.Collections;

public class AGRShipCameraModule : AGRShipTurnModule {
	
	/* 
	 * This class handles the spring arm for the camera
	 */

	/* Camera Calculations*/
	private float offsetZ;
	private float offsetY;
	private float spring = 12;
	private float springConst;
	private float interpolationSpeed;
	private float wantedTightInterpolationSpeed;
	private float wantedFreeInterpolationSpeed;

	private float shipInterpolatedTurn;

	private float springFreeVelocity;
	private float springTightVelocity;
	private float springVelocity;

	private float zDistance;

	private float lookAtHeightMove;

	/* Camera Effects and movement */
	private float boostTimer;
	private bool gotBoostTimer;

	private float cameraFOV;
	private float cameraBoostFOV = 20;

	private float boostDistanceMax = 1f;
	private float boostDistance;
	private float boostHeight;

	private float accelerationLength;
	private bool restLengthLastAccel;
	private bool restLengthLastBrake;
	private float cameraBank;
	private float springStablizerTight;
	private float springStablizerFree;

	private float rotationHelper;

	/* Cockpit view Manipulation */
	private float FPBackForce;

	private float FPBoostFOV;
	private float FPAccelerationFOV;

	/* Camera Mode Int */
	private int cameraMode;

	private float cameraGroundedHeightSpeed;

	/* Camera Effectss */
	private float cameraFOVInSpring;
	private float cameraFOVOutSpring;

	private float heightDiff;
	private float heightHelper;

	/* Orbit Camera */
	private float orbitRotation;

	public void CameraModeChange()
	{
		/* Cycle camera modes */
		if (GetComponent<PlayerInputManager>().playerInputButtonCamera)
		{
			cameraMode++;
			if (cameraMode > 2)
			{
				cameraMode = 0;
			}
		}

		// Overrides
		if (GetComponent<PlayerInputManager>().playerInputButtonReverseCam)
		{
			cameraMode = 5;
		}

		/* Change Enum accordingly */
		switch (cameraMode)
		{
		case 0:
			shipCameraModuleCurrentCameraState = shipCameraModuleCameraStates.Close;
			offsetZ = shipSettingsCameraCloseOffsetZ;
			offsetY = shipSettingsCameraCloseOffsetY;
			springConst = shipSetingsCameraCloseSpring;
			shipCameraModuleCameraFOV = shipSettingsCameraCloseFOV;
			cameraGroundedHeightSpeed = 25;
			break;

		case 1:
			shipCameraModuleCurrentCameraState = shipCameraModuleCameraStates.Far;
			offsetZ = shipSettingsCameraFarOffsetZ;
			offsetY = shipSettingsCameraFarOffsetY;
			springConst = shipSetingsCameraFarSpring;
			shipCameraModuleCameraFOV = shipSettingsCameraFarFOV;
			cameraGroundedHeightSpeed = 15;
			break;

		case 2:
			shipCameraModuleCurrentCameraState = shipCameraModuleCameraStates.Internal;
			shipCameraModuleCameraFOV = shipSettingsCameraInternalFOV;
			break;

		case 3:
			shipCameraModuleCurrentCameraState = shipCameraModuleCameraStates.Orbit;
			break;

		case 4:
			shipCameraModuleCurrentCameraState = shipCameraModuleCameraStates.Destroyed;
			break;

		case 5:
			shipCameraModuleCurrentCameraState = shipCameraModuleCameraStates.Backward;
			break;
		}
	}

	public void CameraFramePass()
	{

		FPAccelerationFOV = Mathf.Lerp(FPAccelerationFOV, 70 + (transform.InverseTransformDirection(rigidbody.velocity).z * Time.deltaTime), Time.deltaTime * 15);

		if (shipCameraModuleCurrentCameraState == shipCameraModuleCameraStates.Internal)
		{
			shipCameraModudleCameraObject.transform.position = shipCameraModuleCockpitLocator.transform.TransformPoint(0,0,0);
			shipCameraModudleCameraObject.transform.rotation = Quaternion.Euler
				(
					shipCameraModuleCockpitLocator.transform.eulerAngles.x,
					shipCameraModuleCockpitLocator.transform.eulerAngles.y,
					shipCameraModuleCockpitLocator.transform.eulerAngles.z
				);

			/* Boost */
			if (shipVelocityModuleBoostTimer > boostTimer * 0.1f)
			{
				FPBoostFOV = Mathf.MoveTowards(FPBoostFOV, 5, Time.deltaTime * 20);
			} else 
			{
				FPBoostFOV = Mathf.MoveTowards(FPBoostFOV, 0, Time.deltaTime * 2);
			}

			/* Effects */
			shipCameraModudleCameraObject.GetComponent<Camera>().fieldOfView = FPAccelerationFOV + FPBoostFOV;


			shipCameraModuleCockpitObject.renderer.enabled = true;
		} else 
		{
			shipCameraModuleCockpitObject.renderer.enabled = false;
		}

	}

	public void CameraPass()
	{
		/* Get what the boost timer was*/
		if (shipVelocityModuleBoostTimer > 0 && !gotBoostTimer)
		{
			boostTimer = shipVelocityModuleBoostTimer;
			gotBoostTimer = true;
		}
		
		if (shipVelocityModuleBoostTimer < 0.05f)
		{
			gotBoostTimer = false;
		}
		
		/* Camera Effects */
		if (shipVelocityModuleBoostTimer > boostTimer * 0.1f)
		{
			cameraFOVOutSpring = 0;
			cameraFOVInSpring = Mathf.Lerp(cameraFOVInSpring, 25, Time.deltaTime * 4);
			cameraFOV = Mathf.Lerp(cameraFOV, shipCameraModuleCameraFOV + 20, Time.deltaTime * cameraFOVInSpring);
			boostDistance = Mathf.Lerp(boostDistance, 6, Time.deltaTime * cameraFOVInSpring * 1.2f);
			boostHeight = Mathf.Lerp(boostHeight, 0, Time.deltaTime * cameraFOVInSpring * 1.2f);
			shipCameraModuleBoostStrength = Mathf.Lerp(shipCameraModuleBoostStrength, 2, Time.deltaTime * 8);
			FPBackForce = Mathf.Lerp(FPBackForce, 0.5f, Time.deltaTime * 2);
		} else 
		{
			cameraFOVInSpring = 0;
			cameraFOVOutSpring = Mathf.Lerp(cameraFOVOutSpring, 10, Time.deltaTime * 6);
			cameraFOV = Mathf.Lerp(cameraFOV, shipCameraModuleCameraFOV, Time.deltaTime * cameraFOVOutSpring);
			boostDistance = Mathf.Lerp(boostDistance, 0, Time.deltaTime * 4);
			boostHeight = Mathf.Lerp(boostHeight, 0, Time.deltaTime * 8);
			shipCameraModuleBoostStrength = Mathf.Lerp(shipCameraModuleBoostStrength, 0, Time.deltaTime);
			FPBackForce = Mathf.Lerp(FPBackForce, 0, Time.deltaTime * 2);
		}
		
		/* Acceleration/Brake Length */
		if (GetComponent<PlayerInputManager>().playerInputButtonThruster && !GetComponent<PlayerInputManager>().playerInputActionBraking)
		{
			restLengthLastAccel = true;
			restLengthLastBrake = false;
			accelerationLength = Mathf.Lerp(accelerationLength, 1.5f, Time.deltaTime * 3);
		} else 
		{
			if (GetComponent<PlayerInputManager>().playerInputActionBraking && shipVelocityModuleVelocity > 1)
			{
				restLengthLastAccel = false;
				restLengthLastBrake = true;
				accelerationLength = Mathf.Lerp(accelerationLength, -5f, Time.deltaTime * 3);
			} else 
			{
				if (restLengthLastAccel || (!restLengthLastAccel && !restLengthLastBrake))
				{
					accelerationLength = Mathf.Lerp(accelerationLength, 0, Time.deltaTime * 3);
				} else if (restLengthLastBrake)
				{
					accelerationLength = Mathf.Lerp(accelerationLength, 0, Time.deltaTime * 1.2f);
				}
			}
		}
		
		/* Acceleration Lag */
		shipCameraModuleSpringArmDistanceLag = Mathf.Lerp
			(
				shipCameraModuleSpringArmDistanceLag, 
				((transform.InverseTransformDirection(rigidbody.velocity).z) / 4) * Time.deltaTime,
				Time.deltaTime * 5
				);
		
		/* Apply Camera Effects */
		shipCameraModudleCameraObject.GetComponent<Camera>().fieldOfView = cameraFOV;
		shipCameraModudleCameraObject.GetComponent<BoostRadius>().SampleStrength = shipCameraModuleBoostStrength;
		
		/* Calculate need variables */
		
		Vector3 LocalDistance = transform.InverseTransformPoint(shipCameraModudleCameraObject.transform.position);
		LocalDistance.y = 0;
		LocalDistance.z = 0;
		float xLocalDistance = LocalDistance.magnitude;
		float shipRotation = shipCameraModuleShipRotation;
		if (shipRotation < 0)
		{
			shipRotation *= -1;
		}
		
		/* Rotation Helper (keeps nose of ship in the centre of the screen */
		if (currentEngineSound == EngineType.F1Agility || currentEngineSound == EngineType.F1Fighter || currentEngineSound == EngineType.F1Speed)
		{
			rotationHelper = LocalDistance.x * 0.8f;
		} else 
		{
			rotationHelper = LocalDistance.x * 0.6f;
		}
		
		/* Set needed variables for spring calculation */
		float interpSpeed = springConst;
		if (shipCameraModuleCurrentCameraState == shipCameraModuleCameraStates.Far)
		{
			xLocalDistance *= 0.5f;
			shipInterpolatedTurn = Mathf.Lerp(shipInterpolatedTurn, shipRotation, Time.deltaTime);
			interpSpeed = 1;
		} else 
		{
			xLocalDistance *= 0.6f;
			shipInterpolatedTurn = shipRotation;
		}
		
		if (shipRotation > shipSettingsTurnMaxAngle && GetComponent<PlayerInputManager>().playerInputActionAirbraking)
		{
			/* Spring Stabilizer */
			springStablizerTight = 0;
			springStablizerFree = Mathf.Lerp(springStablizerFree, xLocalDistance, Time.deltaTime * springConst);
			
			/* Zero the tight velocity */
			springTightVelocity = 0;
			
			/* Lerp the camera spring velocity */
			springFreeVelocity = Mathf.Lerp(springFreeVelocity, 12, Time.deltaTime * interpSpeed);
			springVelocity = Mathf.Lerp(springVelocity, springFreeVelocity, Time.deltaTime * interpSpeed);
			
			/* Set the camera interpolation speed */
			wantedFreeInterpolationSpeed = Mathf.Lerp(wantedFreeInterpolationSpeed, springConst + (xLocalDistance), Time.deltaTime * springVelocity);
			spring = Mathf.Lerp(spring, wantedFreeInterpolationSpeed + springStablizerFree, Time.deltaTime);
			
			/* Lerp the Z distance to bring the ship closer to the screen */
			zDistance = Mathf.Lerp(zDistance, xLocalDistance / 5 , Time.deltaTime * 5);
			
		} else 
		{
			/* Spring Stabilizer */
			springStablizerFree = 0;
			springStablizerTight = Mathf.Lerp(springStablizerTight, xLocalDistance * 8, Time.deltaTime * springConst);
			
			/* Zero the free velocity */
			springFreeVelocity = 0;
			
			/* Lerp the camera spring velocity */
			springTightVelocity = Mathf.Lerp(springTightVelocity, 2, Time.deltaTime * interpSpeed);
			springVelocity = Mathf.Lerp(springVelocity, springTightVelocity, Time.deltaTime * interpSpeed);
			
			/* Set the camera interpolation speed */
			wantedFreeInterpolationSpeed = Mathf.Lerp(wantedFreeInterpolationSpeed, springConst + (xLocalDistance *  shipInterpolatedTurn), Time.deltaTime * springTightVelocity);
			spring = Mathf.Lerp(spring, wantedFreeInterpolationSpeed + springStablizerTight, Time.deltaTime);
			
			/* Lerp the Z distance to bring the ship closer to the screen */
			zDistance = Mathf.Lerp(zDistance, xLocalDistance / 100 , Time.deltaTime * 2);
		}
		
		
		/* Parent the camera to the ship to help with the distance*/
		shipCameraModudleCameraObject.transform.parent = transform;
		shipCameraModudleCameraObject.transform.localPosition = new Vector3
			(
				shipCameraModudleCameraObject.transform.localPosition.x, 
				shipCameraModudleCameraObject.transform.localPosition.y,
				-shipCameraModuleSpringArmInterpolatedLength - shipCameraModuleSpringArmDistanceLag
				);
		shipCameraModudleCameraObject.transform.parent = null;
		
		shipCameraModuleSpringArmInterpolatedLength = offsetZ - transform.InverseTransformDirection(rigidbody.velocity).z / (spring / (Time.deltaTime * spring)) - zDistance;
		
		Vector3 wantedPosition = transform.TransformPoint(0, offsetY + (boostHeight / 12), -shipCameraModuleSpringArmInterpolatedLength - shipCameraModuleSpringArmDistanceLag + (boostDistance / 2));
		
		if (shipCameraModuleCameraFollowType == 1)
		{
			shipCameraModudleCameraObject.transform.position = Vector3.Slerp
				(
					shipCameraModudleCameraObject.transform.position, 
					new Vector3(wantedPosition.x, shipCameraModudleCameraObject.transform.position.y, shipCameraModudleCameraObject.transform.position.z), 
					Time.deltaTime * spring * 2.8f
					);
		} else 
		{
			shipCameraModudleCameraObject.transform.position = Vector3.Slerp
				(
					shipCameraModudleCameraObject.transform.position, 
					new Vector3(wantedPosition.x, shipCameraModudleCameraObject.transform.position.y, shipCameraModudleCameraObject.transform.position.z), 
					Time.deltaTime * spring
					);
		}


		if (shipHoverModuleCurrentState == shipHoverModuleShipStates.InFlight)
		{
			heightHelper = Mathf.Lerp(heightHelper, 0, Time.deltaTime * springConst);
		} else 
		{
			heightHelper = Mathf.Lerp(heightHelper, rigidbody.velocity.y, Time.deltaTime * (springConst * Time.deltaTime));
			if (shipCameraModudleCameraObject.transform.position.y < wantedPosition.y)
			{
				heightHelper *= wantedPosition.y - shipCameraModudleCameraObject.transform.position.y;
			}
		}

		if (heightHelper < 0)
		{
			heightHelper *= -1;
		}

		shipCameraModudleCameraObject.transform.position = Vector3.Slerp
			(
				shipCameraModudleCameraObject.transform.position, 
				new Vector3(shipCameraModudleCameraObject.transform.position.x, wantedPosition.y, shipCameraModudleCameraObject.transform.position.z), 
				Time.deltaTime * (spring * Mathf.Clamp(shipGravityModuleGravityDistance, 1, shipSettingsAntiGravRideHeight))
				);
		
		if (shipCameraModuleCameraFollowType == 1)
		{
			shipCameraModudleCameraObject.transform.position = Vector3.Slerp
				(
					shipCameraModudleCameraObject.transform.position, 
					new Vector3(shipCameraModudleCameraObject.transform.position.x, shipCameraModudleCameraObject.transform.position.y, wantedPosition.z), 
					Time.deltaTime * spring * 2.8f
					);
		} else
		{
			shipCameraModudleCameraObject.transform.position = Vector3.Slerp
				(
					shipCameraModudleCameraObject.transform.position, 
					new Vector3(shipCameraModudleCameraObject.transform.position.x, shipCameraModudleCameraObject.transform.position.y, wantedPosition.z), 
					Time.deltaTime * spring
					);
		}
		
		/* Look at Rotation */
		Vector3 lookAtPosition = transform.TransformPoint(0, -shipCameraModuleSpringArmLookAtHeight + boostHeight, shipCameraModuleSpringArmLookAtLength + accelerationLength + boostDistance);
		if (shipCameraModuleCurrentCameraState == shipCameraModuleCameraStates.Far)
		{
			lookAtPosition = transform.TransformPoint(0, 0, shipCameraModuleSpringArmLookAtLength + accelerationLength);
		}
		float lookAtHeightMoveVel = rigidbody.velocity.y;
		if (lookAtHeightMoveVel != 0)
		{
			lookAtHeightMoveVel /= spring;
		}
		if (lookAtHeightMoveVel < 0)
		{
			lookAtHeightMoveVel = -lookAtHeightMoveVel;
		}
		
		if (shipCameraModuleCurrentCameraState == shipCameraModuleCameraStates.Far)
		{
			lookAtHeightMove = Mathf.Lerp (lookAtHeightMove, lookAtPosition.y, Time.deltaTime * (spring * 2f));
		} else
		{
			lookAtHeightMove = Mathf.Lerp (lookAtHeightMove, lookAtPosition.y, Time.deltaTime * (spring * 3f));
		}

		Quaternion targetRot = Quaternion.LookRotation(new Vector3(lookAtPosition.x, lookAtHeightMove - ((transform.eulerAngles.z * Time.deltaTime) * Time.deltaTime), lookAtPosition.z) - shipCameraModudleCameraObject.transform.position);
		if (shipHoverModuleCurrentState == shipHoverModuleShipStates.InFlight)
		{
			heightDiff = Mathf.Lerp(heightDiff, 0, Time.deltaTime * springConst);
			targetRot = Quaternion.LookRotation(new Vector3(lookAtPosition.x, lookAtHeightMove, lookAtPosition.z) - shipCameraModudleCameraObject.transform.position);
		} else 
		{
			heightDiff = Mathf.Lerp(heightDiff, shipCameraModudleCameraObject.transform.position.y - wantedPosition.y, Time.deltaTime * springConst);
		}

		shipCameraModudleCameraObject.transform.rotation = Quaternion.Euler(targetRot.eulerAngles.x, targetRot.eulerAngles.y - rotationHelper, transform.eulerAngles.z + shipHoverModuleCurrentWobble);


	}

	public void OrbitPass()
	{
		orbitRotation += Time.deltaTime * 30;
		Quaternion rotation = Quaternion.Euler(0, transform.eulerAngles.y + orbitRotation, 0);

		shipCameraModudleCameraObject.transform.rotation = rotation;
		shipCameraModudleCameraObject.transform.position = rotation * new Vector3(0,2, -15) + transform.TransformPoint(0,0, 8);
	}
}

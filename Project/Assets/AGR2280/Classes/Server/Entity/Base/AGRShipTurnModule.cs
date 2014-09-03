using UnityEngine;
using System.Collections;

public class AGRShipTurnModule : AGRShipVelocityModule {
	
	/* 
	 * This class handles the turning, banking and airbraking for the ship
	 */

	/* Springs */
	private float ABFalloffSpring;
	private float ABFalloffSpringDamper;

	private float ABGainSpring;
	private float ABGainSpringDamper;
	/* Airbrake Speed */
	public void TurningPass() 
	{
		/* Apply Constraints */
		rigidbody.constraints = RigidbodyConstraints.FreezeRotationY;

		if (GetComponent<PlayerInputManager>().playerInputAxisSteering != 0)
		{
			/* Set the falloff velocity to 0*/
			shipTurnModuleSteerFalloffVelocity = 0;
			/* Lerp the gain velocity - this gives the turning weight*/
			shipTurnModuleSteerGainVelocity = Mathf.Lerp(shipTurnModuleSteerGainVelocity, shipSettingsTurnGain * Time.deltaTime, Time.deltaTime * 2);
			/* Lerp the turning velocity */
			shipTurnModuleTurningVelocity = Mathf.Lerp
				(
					shipTurnModuleTurningVelocity, 
					GetComponent<PlayerInputManager>().playerInputAxisSteering * shipSettingsTurnMaxAngle, 
					Time.deltaTime * shipTurnModuleSteerGainVelocity
				);
		} else 
		{
			/* Set the gain velocity to 0 */
			shipTurnModuleSteerGainVelocity = 0;
			/* Lerp the falloff velocity - gives the ship a feel of momentum */
			shipTurnModuleSteerFalloffVelocity = Mathf.Lerp(shipTurnModuleSteerFalloffVelocity, shipSettingsTurnFalloff * Time.deltaTime, Time.deltaTime * 2);
			/* Lerp the turning velocity */
			shipTurnModuleTurningVelocity = Mathf.Lerp(shipTurnModuleTurningVelocity, 0, Time.deltaTime * shipTurnModuleSteerFalloffVelocity);
		}

		/* Set Airbrake force */
		float airBrakeSpeed = shipVelocityModuleVelocity / ((shipSettingsEngineMaxThrust + shipVelocityModuleBoostAmount) * (shipSettingsAirbarkeTurn / (shipSettingsAirbarkeTurn + ((shipSettingsAirbrakeAmount * Time.deltaTime) * 3))));
		if (GetComponent<PlayerInputManager>().playerInputAxisAirbrakes != 0)
		{
			/* Set the falloff velocity to 0 */
			shipTurnModuleAirbrakeFalloffVelocity = 0;
			ABFalloffSpring = 0;
			ABFalloffSpringDamper = 0;
			/* Lerp the gain velocity */
			if ((GetComponent<PlayerInputManager>().playerInputAxisAirbrakes > 0 && shipTurnModuleAirbrakeVelocity > 0) ||
			    (GetComponent<PlayerInputManager>().playerInputAxisAirbrakes < 0 && shipTurnModuleAirbrakeVelocity < 0))
			{
				ABGainSpring = 3f;
			} else 
			{
				ABGainSpring = 1f;
			}
			shipTurnModuleAirbrakeGainVelocity = Mathf.Lerp(shipTurnModuleAirbrakeGainVelocity, (shipSettingsAirbrakeGain) * Time.deltaTime, Time.deltaTime * ABGainSpring);
			/* Lerp the airbrake velocity */
			shipTurnModuleAirbrakeVelocity = Mathf.Lerp
				(
					shipTurnModuleAirbrakeVelocity, 
					-GetComponent<PlayerInputManager>().playerInputAxisAirbrakes * airBrakeSpeed, 
					Time.deltaTime * shipTurnModuleAirbrakeGainVelocity
				);

			/* Lerp banking */
			shipTurnModuleBankAirbrakeAngle = Mathf.Lerp(shipTurnModuleBankAirbrakeAngle, -GetComponent<PlayerInputManager>().playerInputAxisAirbrakes * 10, Time.deltaTime * 5);

		} else 
		{
			/* Set the gain velocity to 0 */
			shipTurnModuleAirbrakeGainVelocity = 0;
			ABGainSpring = 0;
			ABGainSpringDamper = 0;
			/* Lerp the falloff velocity */
			ABFalloffSpringDamper = Mathf.Lerp(ABFalloffSpringDamper, 4, Time.deltaTime / 2);
			ABFalloffSpring = Mathf.Lerp(ABFalloffSpring, shipSettingsAirbrakeFalloff * (ABFalloffSpringDamper * 5), Time.deltaTime * ABFalloffSpringDamper);
			shipTurnModuleAirbrakeFalloffVelocity = Mathf.Lerp(shipTurnModuleAirbrakeFalloffVelocity, shipSettingsAirbrakeFalloff * Time.deltaTime, Time.deltaTime * ABFalloffSpring);
			/* Lerp the airbrake velocity */
			shipTurnModuleAirbrakeVelocity = Mathf.Lerp(shipTurnModuleAirbrakeVelocity, 0, Time.deltaTime * shipTurnModuleAirbrakeFalloffVelocity);
			/* Lerp banking */
			shipTurnModuleBankAirbrakeAngle = Mathf.Lerp(shipTurnModuleBankAirbrakeAngle, 0, Time.deltaTime * 5);
		}

		/* Apply Turning Velocity */
		shipCameraModuleShipRotation = shipTurnModuleTurningVelocity + shipTurnModuleAirbrakeVelocity;
		transform.Rotate(Vector3.up * shipCameraModuleShipRotation);


		/* Apply Swing Force */
		if (shipHoverModuleCurrentState == shipHoverModuleShipStates.InFlight)
		{
			shipTurnModuleAirbrakeSwingForce = Mathf.Lerp
				(
					shipTurnModuleAirbrakeSwingForce, 
					airBrakeSpeed * (shipCameraModuleShipRotation * (shipSettingsAirbrakeAmount / shipSettingsAntiGravAirGrip) * shipTurnModuleAirbrakeSwingAir),
					Time.deltaTime * ((shipSettingsAirbrakeGain * 1) * Time.deltaTime)
					);
		} else 
		{
			shipTurnModuleAirbrakeSwingForce = Mathf.Lerp
				(
					shipTurnModuleAirbrakeSwingForce, 
					airBrakeSpeed * ((shipCameraModuleShipRotation * (shipSettingsAirbrakeAmount / shipSettingsAntiGravGroundGrip) * shipTurnModuleAirbrakeSwingGround) * (shipSettingsAirbrakeGrip * Time.deltaTime)),
					Time.deltaTime * ((shipSettingsAirbrakeGain * 0.5f) * Time.deltaTime)
				);
		}
		Vector3 airbrakeSwingAxis = transform.TransformDirection(Vector3.left);
		rigidbody.AddForceAtPosition(airbrakeSwingAxis * shipTurnModuleAirbrakeSwingForce, transform.TransformPoint(0,0, -(shipSettingsPhysicalLength)));

		;
		ShipBanking();
	}

	void ShipBanking()
	{
		shipTurnModuleBankFullAngle = shipTurnModuleBankMaxAngle + (GetComponent<PlayerInputManager>().playerInputAxisSteering * shipTurnModuleBankAirbrakeAngle);

		/* Banking Right */
		if (GetComponent<PlayerInputManager>().playerInputAxisSteering > 0)
		{
			/* Reset falloff */
			shipTurnModuleBankFalloffVelocity = 0;

			/* Set Bank Angle */
			if (shipTurnModuleBankAngle >= 0)
			{
				if (shipTurnModuleBankAngle > shipTurnModuleBankMaxAngle * 0.5f)
				{
					shipTurnModuleBankGainVelocity = Mathf.Lerp(shipTurnModuleBankGainVelocity, shipTurnModuleBankOutExternalSpeed, Time.deltaTime * 5);
				} else
				{
					shipTurnModuleBankGainVelocity = Mathf.Lerp(shipTurnModuleBankGainVelocity, shipTurnModuleBankOutInternalSpeed, Time.deltaTime * 5);
				}
			} else 
			{
				shipTurnModuleBankGainVelocity = Mathf.Lerp(shipTurnModuleBankGainVelocity, 2, Time.deltaTime * 5);
			}

			/* Lerp Velocity */
			shipTurnModuleBankVelocity = Mathf.Lerp(shipTurnModuleBankVelocity, shipTurnModuleBankGainVelocity, Time.deltaTime * 50);
		}

		/* Banking Left */
		if (GetComponent<PlayerInputManager>().playerInputAxisSteering < 0)
		{
			/* Reset falloff */
			shipTurnModuleBankFalloffVelocity = 0;
			
			/* Set Bank Angle */
			if (shipTurnModuleBankAngle <= 0)
			{
				if (shipTurnModuleBankAngle < -shipTurnModuleBankMaxAngle * 0.5f)
				{
					shipTurnModuleBankGainVelocity = Mathf.Lerp(shipTurnModuleBankGainVelocity, shipTurnModuleBankOutExternalSpeed, Time.deltaTime * 5);
				} else
				{
					shipTurnModuleBankGainVelocity = Mathf.Lerp(shipTurnModuleBankGainVelocity, shipTurnModuleBankOutInternalSpeed, Time.deltaTime * 5);
				}
			} else 
			{
				shipTurnModuleBankGainVelocity = Mathf.Lerp(shipTurnModuleBankGainVelocity, 2, Time.deltaTime * 5);
			}
			
			/* Lerp Velocity */
			shipTurnModuleBankVelocity = Mathf.Lerp(shipTurnModuleBankVelocity, shipTurnModuleBankGainVelocity, Time.deltaTime * 50);
		}

		if (GetComponent<PlayerInputManager>().playerInputAxisSteering == 0)
		{
			/* Reset Gain */
			shipTurnModuleBankGainVelocity = 0;

			/* Set Bank Angle */
			if (shipTurnModuleBankAngle < 0)
			{
				if (shipTurnModuleBankAngle < -shipTurnModuleBankMaxAngle * 0.5f)
				{
					shipTurnModuleBankFalloffVelocity = Mathf.Lerp(shipTurnModuleBankFalloffVelocity, shipTurnModuleBankInExternalSpeed, Time.deltaTime * 5);
				} else 
				{
					shipTurnModuleBankFalloffVelocity = Mathf.Lerp(shipTurnModuleBankFalloffVelocity, shipTurnModuleBankInInternalSpeed, Time.deltaTime * 5);
				}
			} else 
			{
				if (shipTurnModuleBankAngle > shipTurnModuleBankMaxAngle * 0.5f)
				{
					shipTurnModuleBankFalloffVelocity = Mathf.Lerp(shipTurnModuleBankFalloffVelocity, shipTurnModuleBankInExternalSpeed, Time.deltaTime * 5);
				} else 
				{
					shipTurnModuleBankFalloffVelocity = Mathf.Lerp(shipTurnModuleBankFalloffVelocity, shipTurnModuleBankInInternalSpeed, Time.deltaTime * 5);
				}
			}
			/* Lerp Velocity */
			shipTurnModuleBankVelocity = Mathf.Lerp(shipTurnModuleBankVelocity, shipTurnModuleBankFalloffVelocity, Time.deltaTime * 50);

			/* Lerp Bank */
			shipTurnModuleBankAngle = Mathf.Lerp(shipTurnModuleBankAngle, 0 + shipTurnModuleBankAirbrakeAngle, Time.deltaTime * shipTurnModuleBankVelocity);
		} else 
		{
			/* Lerp Bank */
			shipTurnModuleBankAngle = Mathf.Lerp
				(
					shipTurnModuleBankAngle, 
					GetComponent<PlayerInputManager>().playerInputAxisSteering * shipTurnModuleBankFullAngle,
					Time.deltaTime * shipTurnModuleBankVelocity
				);
		}

		/* Apply bank to ship container */
		ShipGroupContainer.transform.localRotation = Quaternion.Euler(0,0, -shipTurnModuleBankAngle + shipHoverModuleBarrelRollActual + shipHoverModuleCurrentWobble);
	}
}

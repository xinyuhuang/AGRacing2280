using UnityEngine;
using System.Collections;

public class AGRShipVelocityModule : AGRShipGravityModule {

	/* 
	 * This class handles the velocity and external forces for the craft
	 */

	private Vector3 sideShiftDirection;
	public void AccelerationPass() 
	{
		if (GetComponent<PlayerInputManager>().playerInputButtonThruster)
		{
			/* Increase acceleration and velocity */
			shipVelocityModuleAcceleration = Mathf.MoveTowards(shipVelocityModuleAcceleration, shipSettingsEngineMaxAccel, Time.deltaTime * (shipSettingsEngineAccelSpeed * Time.deltaTime) * 5);
			shipVelocityModuleVelocity = Mathf.MoveTowards(shipVelocityModuleVelocity, shipSettingsEngineMaxThrust + shipVelocityModuleBoostAmount, shipVelocityModuleAcceleration);

			/* Set the velocity loss to 0 - we are not loosing velocity*/
			shipVelocityModuleVelocityLoss = 0;
		} else 
		{
			/* Reduce velocity loss */
			if (shipVelocityModuleVelocity > 100)
			{
				shipVelocityModuleVelocityLoss = (shipVelocityModuleVelocity * 0.5f) * Time.deltaTime;
			} else 
			{
				
				if (shipVelocityModuleVelocity < 4)
				{
					shipVelocityModuleVelocityLoss = Time.deltaTime;
				} else 
				{
					shipVelocityModuleVelocityLoss = (shipVelocityModuleVelocity * 0.2f) * Time.deltaTime;
				}
			}

			shipVelocityModuleAcceleration = 0;
		}

		/* Braking */
		if (GetComponent<PlayerInputManager>().playerInputActionBraking)
		{
			shipVelocityModuleBrakeAmount = Mathf.MoveTowards(shipVelocityModuleBrakeAmount ,shipSettingsBrakesSlowdown * ((shipVelocityModuleVelocity * 0.08f) * Time.deltaTime), Time.deltaTime * (shipSettingsBrakesSlowdownGain / 2));

			// Apply Drag
			Vector3 tempVel = transform.InverseTransformDirection(rigidbody.velocity);
			tempVel.z *= 1 - 0.1f;
			rigidbody.velocity = transform.TransformDirection(tempVel);

		} else
		{
			shipVelocityModuleBrakeAmount = Mathf.MoveTowards(shipVelocityModuleBrakeAmount , 0, Time.deltaTime * shipSettingsBrakesSlowdownFalloff);
		}

		/* Air Resistance */
		if (shipVelocityModuleVelocity > (shipSettingsEngineMaxThrust * 0.4f))
		{
			if (GetComponent<PlayerInputManager>().playerInputAxisAirbrakes != 0)
			{
				if (shipHoverModuleCurrentState == shipHoverModuleShipStates.FullyGrounded)
				{
					shipVelocityModuleAirbrakeResistanceDamper = Mathf.Lerp(shipVelocityModuleAirbrakeResistanceDamper, shipSettingsAntiGravGroundGrip * 1.6f, Time.deltaTime * 3);
				} else
				{
					shipVelocityModuleAirbrakeResistanceDamper = Mathf.Lerp(shipVelocityModuleAirbrakeResistanceDamper , shipSettingsAntiGravGroundGrip * rigidbody.drag, Time.deltaTime * shipSettingsAirbrakeGrip);
				}
				shipVelocityModuleAirbrakeResistance = Mathf.Lerp(shipVelocityModuleAirbrakeResistance, 1, Time.deltaTime * shipVelocityModuleAirbrakeResistanceDamper);
				if (shipHoverModuleCurrentState == shipHoverModuleShipStates.InFlight)
				{
					shipVelocityModuleResistanceAmount = shipVelocityModuleResistanceAir;
				} else 
				{
					shipVelocityModuleResistanceAmount = shipVelocityModuleResistanceGround;
				}
				shipVelocityModuleAirbrakeDeacceleration = (shipCameraModuleShipRotation / shipVelocityModuleAirbrakeResistance) * (Mathf.Clamp(shipVelocityModuleVelocity, 0, shipVelocityModuleResistanceAmount));
			} else 
			{
				shipVelocityModuleAirbrakeResistanceDamper = 0;
				shipVelocityModuleAirbrakeResistance = 4;
				shipVelocityModuleAirbrakeDeacceleration = 0;
			}
			
			if (shipVelocityModuleAirbrakeDeacceleration < 0)
			{
				shipVelocityModuleAirbrakeDeacceleration *= -1;
			}

			shipVelocityModuleVelocity -= shipVelocityModuleAirbrakeDeacceleration;
		}

		/* Reduce the velocity */
		shipVelocityModuleVelocity -= (shipVelocityModuleVelocityLoss + shipVelocityModuleBrakeAmount);

		/* Prevent velocity from going under 0 */
		if (shipVelocityModuleVelocity < 0 + shipVelocityModuleBoostAmount)
		{
			shipVelocityModuleVelocity = 0 + shipVelocityModuleBoostAmount;
		}

		/* Apply Thrust */
		rigidbody.AddRelativeForce(Vector3.forward * shipVelocityModuleVelocity);

		/* Execute Speed Pad Boost*/
		ShipSpeedPadHit();

		/* Side Shift Forces */
		SideShiftPass();
	}

	public void ShipSpeedPadHit()
	{
		if (shipHoverModuleHitSpeed)
		{
			shipVelocityModuleBoostTimer = 0.27f;
		}

		/* Reduce and clamp boost timer*/
		shipVelocityModuleBoostTimer -= Time.deltaTime;
		shipVelocityModuleBoostTimer = Mathf.Clamp(shipVelocityModuleBoostTimer, 0 ,Mathf.Infinity);

		if (shipVelocityModuleBoostTimer > 0)
		{
			shipVelocityModuleBoostAmount = ServerSettings.ServerVarSpeedPadForce * rigidbody.drag;
			shipVelocityModuleAcceleration = shipSettingsEngineMaxAccel;
		} else 
		{
			shipVelocityModuleBoostAmount = 0;
		}


	}

	public void SideShiftInputPass()
	{
		shipVelocityModuleSideShiftSuccessCooler -= 1 * Time.deltaTime;
		if (shipVelocityModuleSideShiftSuccessCooler < 1)
		{
			/* Oposite reset */
			if ((lastSideShiftInput == -1 && GetComponent<PlayerInputManager>().playerInputActionSideShiftRight) || lastSideShiftInput == 1 && GetComponent<PlayerInputManager>().playerInputActionSideShiftLeft)
			{
				shipVelocityModuleSideShiftTapAmount = 0;
				lastSideShiftInput = 0;
			}

			if (GetComponent<PlayerInputManager>().playerInputActionSideShiftLeft)
			{
				lastSideShiftInput = -1;
				if (shipVelocityModuleSideShiftCooler > 0 && shipVelocityModuleSideShiftTapAmount  == 1)
				{
					shipVelocityModuleSideShiftTimer = 0.18f;
					shipVelocityModuleSideShiftSuccessCooler = 2.5f;
					sideShiftDirection = transform.right;
				} else 
				{
					shipVelocityModuleSideShiftCooler = 0.2f;
					shipVelocityModuleSideShiftTapAmount++;
				}
			}

			if (GetComponent<PlayerInputManager>().playerInputActionSideShiftRight)
			{
				lastSideShiftInput = 1;
				if (shipVelocityModuleSideShiftCooler > 0 && shipVelocityModuleSideShiftTapAmount  == 1)
				{
					shipVelocityModuleSideShiftTimer = 0.18f;
					shipVelocityModuleSideShiftSuccessCooler = 2.5f;
					sideShiftDirection = transform.right;
				} else 
				{
					shipVelocityModuleSideShiftCooler = 0.2f;
					shipVelocityModuleSideShiftTapAmount++;
				}
			}
			if (shipVelocityModuleSideShiftCooler > 0)
			{
				shipVelocityModuleSideShiftCooler -= 1 * Time.deltaTime;
			} else 
			{
				shipVelocityModuleSideShiftTapAmount = 0;
			}
		}
	}

	public void SideShiftPass()
	{
		if (shipVelocityModuleSideShiftTimer > 0)
		{
			shipVelocityModuleSideShiftTimer -= 1 * Time.deltaTime;
			rigidbody.AddForce(sideShiftDirection * (-lastSideShiftInput * shipSettingsAirbrakeSideShiftForce));
		} else 
		{
			shipVelocityModuleSideShiftTimer = 0;
		}
	}
}

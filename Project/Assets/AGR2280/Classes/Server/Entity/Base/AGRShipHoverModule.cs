using UnityEngine;
using System.Collections;

public class AGRShipHoverModule : AGRShipSettingsContainer {

	/* 
	 * This class handles the hovering logic for a craft
	 */

	float UnstableHoverTimer;
	float UnstableHoverSpeed = 1f;
	float UnstableHoverAmount = 0.2f;
	float UnstableHoverDamp = 12;
	private float UnstableHoverOffsetX;
	private float UnstableHoverOffsetY;
	private float UnstableHoverOffsetZ;
	private Vector3 lerpedVector;
	private Quaternion trackLookAt;

	private float hoverStablize;

	public void HoverPass()
	{
		UnstableAntiGravity();
		BarrelRolls();

		/* Pad hit resets*/
		shipHoverModuleHitSpeed = false;
		shipHoverModuleHitWeapon = false;

		/* Hovering resets*/

		shipHoverModuleFrontHovering = false;
		shipHoverModuleBackHovering = false;

		/* Create Rays for Casts */
		Vector3 vFrontLeft = transform.TransformPoint(-(shipSettingsPhysicalWidth / 2), 0, shipSettingsPhysicalLength / 2);
		Vector3 vFrontRight = transform.TransformPoint(shipSettingsPhysicalWidth / 2, 0, shipSettingsPhysicalLength / 2);
		Vector3 vBackLeft = transform.TransformPoint(-(shipSettingsPhysicalWidth / 2), 0, -(shipSettingsPhysicalLength / 2));
		Vector3 vBackRight = transform.TransformPoint(shipSettingsPhysicalWidth / 2, 0, -(shipSettingsPhysicalLength / 2));

		Ray rFrontLeft = new Ray(vFrontLeft, -transform.up);
		Ray rFrontRight = new Ray(vFrontRight, -transform.up);
		Ray rBackLeft = new Ray(vBackLeft, -transform.up);
		Ray rBackRight = new Ray(vBackRight, -transform.up);

		/* Create the Casts */
		RaycastHit[] rhFrontLeft = Physics.RaycastAll(rFrontLeft);
		RaycastHit[] rhFrontRight = Physics.RaycastAll(rFrontRight);
		RaycastHit[] rhBackLeft = Physics.RaycastAll(rBackLeft);
		RaycastHit[] rhBackRight = Physics.RaycastAll(rBackRight);

		for (int FL = 0; FL < rhFrontLeft.Length; FL++)
		{
			bool exitRebound = true;
			bool landRebound = false;
			RaycastHit hitArray = rhFrontLeft[FL];
			if (hitArray.collider.gameObject.layer == LayerMask.NameToLayer("Track_Floor") && hitArray.distance <= shipSettingsAntiGravRideHeight)
			{
				shipHoverModuleFrontHovering = true;
				ShipHover(vFrontLeft,  hitArray.point,  hitArray.distance, shipHoverModuleHoverForce, shiphoverModuleHoverDamping);
			}
		}

		for (int FR = 0; FR < rhFrontRight.Length; FR++)
		{
			bool exitRebound = true;
			bool landRebound = false;
			RaycastHit hitArray = rhFrontRight[FR];
			if (hitArray.collider.gameObject.layer == LayerMask.NameToLayer("Track_Floor") && hitArray.distance <= shipSettingsAntiGravRideHeight)
			{
				shipHoverModuleFrontHovering = true;
				ShipHover(vFrontRight,  hitArray.point,  hitArray.distance, shipHoverModuleHoverForce, shiphoverModuleHoverDamping);
			}
		}

		for (int BL = 0; BL < rhBackLeft.Length; BL++)
		{
			bool exitRebound = true;
			bool landRebound = false;
			RaycastHit hitArray = rhBackLeft[BL];
			if (hitArray.collider.gameObject.layer == LayerMask.NameToLayer("Track_Floor") && hitArray.distance <= shipSettingsAntiGravRideHeight)
			{
				shipHoverModuleBackHovering = true;
				ShipHover(vBackLeft,  hitArray.point,  hitArray.distance, shipHoverModuleHoverForce, (shiphoverModuleHoverDamping));
			}
		}
		
		for (int BR = 0; BR < rhBackRight.Length; BR++)
		{
			bool exitRebound = true;
			bool landRebound = false;
			RaycastHit hitArray = rhBackRight[BR];
			if (hitArray.collider.gameObject.layer == LayerMask.NameToLayer("Track_Floor") && hitArray.distance <= shipSettingsAntiGravRideHeight)
			{
				shipHoverModuleBackHovering = true;
				ShipHover(vBackRight,  hitArray.point,  hitArray.distance, shipHoverModuleHoverForce, (shiphoverModuleHoverDamping));
			}
		}

		/* Raycast for Speed/Weapon Pads */
		if (Physics.Raycast(transform.position, -transform.up, out shipHoverModulePadInfo))
		{
			
			/* Speed Pad Hit */
			if (shipHoverModulePadInfo.collider.gameObject.layer == LayerMask.NameToLayer("Speed") && shipHoverModulePadInfo.distance <= shipSettingsAntiGravRideHeight + 4)
			{
				if (!shipHoverModuleHitSpeedRegistered)
				{
					shipHoverModuleHitSpeed = true;
					shipHoverModuleHitSpeedRegistered = true;
				}
				shipHoverModuleSpeedPadDirection = shipHoverModulePadInfo.collider.gameObject.transform.right;
			} else 
			{
				shipHoverModuleHitSpeedRegistered = false;
			}
		}

		/* Ship States */
		if (shipHoverModuleFrontHovering && shipHoverModuleBackHovering)
		{
			shipHoverModuleCurrentState = shipHoverModuleShipStates.FullyGrounded;
		} else if ((shipHoverModuleFrontHovering && !shipHoverModuleBackHovering) || (!shipHoverModuleFrontHovering && shipHoverModuleBackHovering))
		{
			shipHoverModuleCurrentState = shipHoverModuleShipStates.InFlight;
		} else if (!shipHoverModuleFrontHovering && !shipHoverModuleBackHovering)
		{
			shipHoverModuleCurrentState = shipHoverModuleShipStates.InFlight;
		}

		/* Wobbling */
		shipHoverModuleWobbleTime += Time.deltaTime;
		shipHoverModuleWobbleSpeed -= Time.deltaTime / 2;
		shipHoverModuleWobbleSpeed = Mathf.Clamp (shipHoverModuleWobbleSpeed, 0, 10);
		shipHoverModuleWobbleAmount -= Time.deltaTime / 2;
		shipHoverModuleWobbleAmount = Mathf.Clamp (shipHoverModuleWobbleAmount, 0, 10);
		
		shipHoverModuleCurrentWobble = Mathf.Sin (shipHoverModuleWobbleTime * shipHoverModuleWobbleSpeed) * shipHoverModuleWobbleAmount;
	}

	void ShipHover(Vector3 hoverPoint, Vector3 hitPoint, float distance, float forceConst, float damping)
	{
		if (shipHoverModuleCurrentState != shipHoverModuleShipStates.InFlight)
		{
			if (shipSettingsRenderDebugging)
			{
				Debug.DrawLine(hoverPoint, hitPoint);
			}
			float hoverForce = ((distance - shipSettingsAntiGravRideHeight) / Time.deltaTime) * (-0.2f * ((shipSettingsAntiGravRideHeight - distance)));
			hoverForce -= damping * rigidbody.velocity.y;

			if (shipHoverModuleFallCapped)
			{
				hoverForce = 0;
				rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
				shipHoverModuleWobbleAmount = 2.3f;
				shipHoverModuleWobbleSpeed = 6;
				shipHoverModuleWobbleTime = 0;
				shipHoverModuleFallCapped = false;
			}
			rigidbody.AddForceAtPosition(transform.up * hoverForce, hoverPoint);

			/* Extra Force */
			if (distance < shipSettingsAntiGravRideHeight / 2)
			{
				rigidbody.AddForceAtPosition(transform.up * (hoverForce / 2), hoverPoint);
			}

			/* Unstable Wobble */
			if (distance < shipSettingsAntiGravRideHeight / 3)
			{
				shipHoverModuleWobbleAmount = 2.3f;
				shipHoverModuleWobbleSpeed = 6;
				shipHoverModuleWobbleTime = 0;
			}
		}
	}

	void UnstableAntiGravity()
	{

		if (shipVelocityModuleVelocity > 100)
		{
			UnstableHoverTimer = 0;
			UnstableHoverOffsetX = Mathf.Lerp(UnstableHoverOffsetX, 0, Time.deltaTime);
			UnstableHoverOffsetY = Mathf.Lerp(UnstableHoverOffsetY, 0, Time.deltaTime);
			UnstableHoverOffsetZ = Mathf.Lerp(UnstableHoverOffsetZ, 0, Time.deltaTime);

		} else 
		{
			UnstableHoverTimer += Time.deltaTime * UnstableHoverSpeed;
			UnstableHoverOffsetX = Mathf.Sin((UnstableHoverTimer * UnstableHoverSpeed) * 1.2f);
			UnstableHoverOffsetY = Mathf.Sin((UnstableHoverTimer * UnstableHoverSpeed) * 2.4f);
			UnstableHoverOffsetZ = Mathf.Sin((UnstableHoverTimer * UnstableHoverSpeed) / 10);

		}

		ShipGroupContainer.transform.localPosition = new Vector3
			(
				UnstableHoverOffsetX / UnstableHoverDamp, 
				UnstableHoverOffsetY / (UnstableHoverDamp * 2), 
				UnstableHoverOffsetZ / UnstableHoverDamp
			);

		ShipGroupContainer.transform.localRotation = Quaternion.Euler
			(
				(UnstableHoverOffsetY * 10),
				(UnstableHoverOffsetX * 100),
				ShipGroupContainer.transform.localEulerAngles.z
			);
	}

	void BarrelRolls()
	{
		/* Check if flying */
		if (shipHoverModuleCurrentState == shipHoverModuleShipStates.InFlight)
		{
			shipHoverModuleCanBarrelRoll = true;
		} else 
		{
			shipHoverModuleBarrelRollProgress = Mathf.LerpAngle(shipHoverModuleBarrelRollProgress, 0, Time.deltaTime * 10);
			shipHoverModuleBarrelRollActual = shipHoverModuleBarrelRollProgress;
			if (shipHoverModuleBarrelRollSuccess)
			{
				shipAudioModuleTurboObject.GetComponent<AudioSource>().Play();
				shipVelocityModuleBoostTimer = 0.20f;
				shipHoverModuleBarrelRollSuccess = false;
			}
			shipHoverModuleBarrelRollLastValue = 0;
			shiphoverModuleBarrelRollState = 0;
			shipHoverModuleCanBarrelRoll = false;
			shipHoverModuleBarrelRollHasRolled = false;
		}

		if (shipHoverModuleCanBarrelRoll)
		{
			shipHoverModuleBarrelRollTimer -= 1 * Time.deltaTime;
			if (shipHoverModuleBarrelRollTimer < 0)
			{
				shipHoverModuleBarrelRollTimer = 0;
				if (!shipHoverModuleBarrelRollHasRolled)
				{
					shipHoverModuleBarrelRollSuccess = false;
				}
			}

			if (!shipHoverModuleBarrelRollHasRolled)
			{
				shipHoverModuleBarrelRollProgress = 0;
				/* Left Right Left*/
				if (GetComponent<PlayerInputManager>().playerInputAxisSteering < 0 && shiphoverModuleBarrelRollState == 0)
				{
					shipHoverModuleBarrelRollLastValue = -1;
					shipHoverModuleBarrelRollTimer = 0.5f;
					shiphoverModuleBarrelRollState ++;
				}

				if (shipHoverModuleBarrelRollLastValue == -1 && GetComponent<PlayerInputManager>().playerInputAxisSteering > 0 && shiphoverModuleBarrelRollState == 1 && shipHoverModuleBarrelRollTimer > 0)
				{
					shipHoverModuleBarrelRollLastValue = 1;
					shipHoverModuleBarrelRollTimer = 0.5f;
					shiphoverModuleBarrelRollState ++;
				}

				if (shipHoverModuleBarrelRollLastValue == 1 && GetComponent<PlayerInputManager>().playerInputAxisSteering < 0 && shiphoverModuleBarrelRollState == 2 && shipHoverModuleBarrelRollTimer > 0)
				{
					shipAudioModuleBRObject.GetComponent<AudioSource>().Play();
					shipHoverModuleBarrelRollHasRolled = true;
				}

				/* Right Left Right */
				if (GetComponent<PlayerInputManager>().playerInputAxisSteering > 0 && shiphoverModuleBarrelRollState == 0)
				{
					shipHoverModuleBarrelRollLastValue = 1;
					shipHoverModuleBarrelRollTimer = 0.5f;
					shiphoverModuleBarrelRollState ++;
				}
				
				if (shipHoverModuleBarrelRollLastValue == 1 && GetComponent<PlayerInputManager>().playerInputAxisSteering < 0 && shiphoverModuleBarrelRollState == 1 && shipHoverModuleBarrelRollTimer > 0)
				{
					shipHoverModuleBarrelRollLastValue = -1;
					shipHoverModuleBarrelRollTimer = 0.5f;
					shiphoverModuleBarrelRollState ++;
				}
				
				if (shipHoverModuleBarrelRollLastValue == -1 && GetComponent<PlayerInputManager>().playerInputAxisSteering > 0 && shiphoverModuleBarrelRollState == 2 && shipHoverModuleBarrelRollTimer > 0)
				{
					shipAudioModuleBRObject.GetComponent<AudioSource>().Play();
					shipHoverModuleBarrelRollHasRolled = true;
				}
			} else 
			{
				float rollSpeed = 1;
				if (shipHoverModuleBarrelRollProgress < 350)
				{
					rollSpeed = 8;
				}
				if (shipHoverModuleBarrelRollProgress < 320)
				{
					rollSpeed = 12;
				}
				if (shipHoverModuleBarrelRollProgress < 300)
				{
					rollSpeed = 19;
				}
				if (shipHoverModuleBarrelRollProgress < 90)
				{
					rollSpeed = 14;
				}
				if (shipHoverModuleBarrelRollProgress < 25)
				{
					rollSpeed = 7;
				}
				if (shipHoverModuleBarrelRollProgress < 8)
				{
					rollSpeed = 4;
				}
				if (shipHoverModuleBarrelRollProgress > 310)
				{
					rollSpeed = 7;
					if (shipHoverModuleBarrelRollProgress > 350)
					{
						rollSpeed = 2;
					}

				}

				shipHoverModuleBarrelRollProgress += rollSpeed;
				if (shipHoverModuleBarrelRollProgress > 285)
				{
					shipHoverModuleBarrelRollSuccess = true;
				}

				if (shipHoverModuleBarrelRollProgress > 360)
				{
					shipHoverModuleBarrelRollProgress = 360;
				}

				if (shipHoverModuleBarrelRollLastValue == 1)
				{
					shipHoverModuleBarrelRollActual = shipHoverModuleBarrelRollProgress;
				} else 
				{
					shipHoverModuleBarrelRollActual = -shipHoverModuleBarrelRollProgress;
				}
			}


		} else 
		{
			shipHoverModuleBarrelRollLastValue = 0;
			shipHoverModuleBarrelRollTimer = 0;
			shipHoverModuleBarrelRollSuccess = false;
			shipHoverModuleBarrelRollHasRolled = false;
		}


	}
}

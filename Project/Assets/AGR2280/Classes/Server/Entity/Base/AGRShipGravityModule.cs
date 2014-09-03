using UnityEngine;
using System.Collections;

public class AGRShipGravityModule : AGRShipHoverModule {

	/* 
	 * This class handles the gravity for the craft
	 */

	/* Pitching */
	private float flightPitching;

	private float groundPitching;

	public void SetupPhysics()
	{
		/* Create and setup Rigidbody */
		this.gameObject.AddComponent<Rigidbody>();
		rigidbody.drag = 5;
		rigidbody.angularDrag = 20;
		rigidbody.useGravity = false;
		rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

		shipGravityModuleDirection = -Vector3.up;

	}

	public void GravityPass () 
	{
		/* Execute hovering */
		HoverPass();

		/* Reset Centre of Mass*/
		rigidbody.centerOfMass = new Vector3(0,0,0);

		/* Work out what grounded state the ship is in and adjust the gravity acordingly*/
		if (shipHoverModuleCurrentState == shipHoverModuleShipStates.FullyGrounded || shipHoverModuleCurrentState == shipHoverModuleShipStates.PartiallyGrounded)
		{
			shipSettingsAntiGravJumpTime = 0;
			shipGravityModuleAirTime = 0;

			RaycastHit distanceCheck;
			if (Physics.Raycast(transform.position, -transform.up, out distanceCheck))
			{
				shipGravityModuleGravityDistance = shipSettingsAntiGravRideHeight - distanceCheck.distance;
				if (distanceCheck.collider.gameObject.tag == "TrackSegment")
				{
					shipRespawnManagerTimer = 0;
				} else
				{
					shipRespawnManagerTimer += Time.deltaTime;
				}
			}
			float distance = Mathf.Clamp((shipSettingsAntiGravRideHeight - distanceCheck.distance) * shipSettingsAntiGravRideHeight, 1, Mathf.Infinity);
			
			shipGravityModuleGravity = (shipSettingsTrackGravity - (distance)) * shipGravityModuleGravityMultiplier;

			/* Lerp damping back to 0.1 */
			//shiphoverModuleHoverDamping = Mathf.Lerp(shiphoverModuleHoverDamping, shipSettingsAntiGravRebound, Time.deltaTime);
			shiphoverModuleHoverDamping = Mathf.Lerp(shiphoverModuleHoverDamping, 0, Time.deltaTime * 5);
			shipGravityModuleDirection = -transform.up;

			/* Pitching */
			groundPitching = Mathf.Lerp(groundPitching, GetComponent<PlayerInputManager>().playerInputAxisPitching * 1f, Time.deltaTime * 4);
		//	transform.Rotate(Vector3.right * groundPitching);
			rigidbody.AddRelativeTorque(groundPitching * 300,0,0);

			/* Set Angular Drag */
			float dragHelper = groundPitching * 5;
			rigidbody.angularDrag = 20 - dragHelper;

		} else if (shipHoverModuleCurrentState == shipHoverModuleShipStates.InFlight)
		{
			shipGravityModuleDirection = Vector3.down;
			shipGravityModuleGravity = Mathf.MoveTowards(shipGravityModuleGravity, (((shipSettingsFlightGravity * shipSettingsNormalGravity) * shipGravityModuleGravityMultiplier)), Time.deltaTime * (shipGravityModuleGravityMultiplier * 550));
			shipGravityModuleAirTime += Time.deltaTime;

			RaycastHit distanceCheck;
			if (Physics.Raycast(transform.position, -transform.up, out distanceCheck))
			{
				if (distanceCheck.collider.gameObject.tag == "TrackSegment")
				{
					shipRespawnManagerTimer = 0;
				} else
				{
					shipRespawnManagerTimer += Time.deltaTime;
				}
			} else 
			{
				shipRespawnManagerTimer += Time.deltaTime;
			}

			/* Pitch Correction Speed */
			if (GetComponent<PlayerInputManager>().playerInputAxisPitching != 0)
			{
				flightPitching = Mathf.Lerp(flightPitching, GetComponent<PlayerInputManager>().playerInputAxisPitching * 15, Time.deltaTime * shipSettingsPitchDamping);
				flightPitching = Mathf.Clamp(flightPitching, 1, 15);
			} else
			{
				flightPitching = Mathf.Lerp(flightPitching, 5, Time.deltaTime * shipSettingsPitchDamping);
			}

			/* Pitch Correction */
			float shipAngle = 360 - transform.localEulerAngles.x;
			if ((shipAngle > 90 && shipAngle < 360) || (shipAngle < 65 && shipAngle > 0))
			{
				transform.Rotate(Vector3.right * (Time.deltaTime * flightPitching));
			}
			
			/* Set hover damping to air time (stablise the landing)*/
			shiphoverModuleHoverDamping = Mathf.Lerp (shiphoverModuleHoverDamping, 0.3f, Time.deltaTime * 5);

		}

		/* Apply Force */
		rigidbody.AddForce(shipGravityModuleDirection * shipGravityModuleGravity);

		/* Set Fall Cap */
		if (shipGravityModuleAirTime > 0.5f)
		{
			shipHoverModuleFallCapped = true;
		}
	}
}

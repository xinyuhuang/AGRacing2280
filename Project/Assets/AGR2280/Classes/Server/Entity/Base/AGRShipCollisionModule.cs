using UnityEngine;
using System.Collections;

public class AGRShipCollisionModule : AGRShipSoundModule {

	public void CollisionPass()
	{
		if (shipCollisionModuleColliding)
		{
			//print (shipCollisionModuleSide);
			for (int i = 0; i < shipCollisionModuleContactCount; i++)
			{
				/* Constrain Angular Movement */
				rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

				/* Set Collision Vars */
				shipCollisionModuleSpeed[i] = rigidbody.GetPointVelocity(shipCollisionModuleContact[i].point);
				Vector3 hitVector = transform.InverseTransformPoint(shipCollisionModuleContact[i].point);
				float hitDot = Vector3.Dot(shipCollisionModuleContact[i].normal, transform.forward);

				float hitForce = Mathf.Abs(Mathf.Abs(hitDot) * (rigidbody.velocity.x + rigidbody.velocity.z + rigidbody.velocity.y) / Time.deltaTime * rigidbody.drag * rigidbody.mass);
				float hitFriction = Mathf.Abs(hitDot) * Mathf.Abs(shipCollisionModuleSpeed[i].x * shipCollisionModuleSpeed[i].z / Time.deltaTime) * rigidbody.drag * rigidbody.mass;

				Vector3 impact = new Vector3(hitVector.z * hitVector.y, hitVector.x * hitVector.z, 0) * hitFriction * rigidbody.drag * rigidbody.mass;

				/* Zero out y velocity and angular velocity */
				rigidbody.velocity = new Vector3(rigidbody.velocity.x, -2, rigidbody.velocity.z);
				rigidbody.angularVelocity = Vector3.zero;

				/* Slow ship down */
				if (shipVelocityModuleVelocity > shipSettingsEngineMaxThrust * 0.2f)
				{
					float slowDownForce = shipVelocityModuleAcceleration * 1.1f;
					shipVelocityModuleVelocity -= slowDownForce;
				}

				/* Rotate Ship */
				if (impact != Vector3.zero)
				{
					impact = Vector3.ClampMagnitude(impact, 160);

					float pitchForce = impact.x;
					if (pitchForce < 0)
					{
						pitchForce *= -1;
					}
					transform.Rotate(pitchForce * Time.deltaTime * Time.deltaTime, 0, 0);
					transform.Rotate(Vector3.up * ((impact.y * Time.deltaTime) * Time.deltaTime));

				}

				Vector3 wallMove = hitVector;
				wallMove.y = 0;
				rigidbody.AddForce(wallMove * Time.deltaTime);

				/* Throw ship back */
				if (hitDot < -0.8f)
				{
					float tempVel = transform.InverseTransformDirection(rigidbody.velocity).z * Time.deltaTime;
					tempVel *= 1000;
					print (tempVel);
					if (tempVel < 0)
					{
						tempVel = 0;
					}

					tempVel = Mathf.Clamp(tempVel, 0, 300);
					shipVelocityModuleBoostTimer = 0;
					shipVelocityModuleVelocity = 0;
					rigidbody.velocity = Vector3.zero;

					if (tempVel > 15)
					{
						rigidbody.AddRelativeForce(new Vector3(0,(tempVel * Time.deltaTime), -tempVel), ForceMode.Impulse);
					}


				}
			}
		}
	}
}

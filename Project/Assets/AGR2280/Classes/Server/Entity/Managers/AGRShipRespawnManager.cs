using UnityEngine;
using System.Collections;

public class AGRShipRespawnManager : AGRShipRaceManager {
	
	public void RespawnPass()
	{
		if (shipRespawnManagerTimer > 0.8f)
		{
			shipVelocityModuleVelocity = 0;
			shipVelocityModuleAcceleration = 0;
			shipVelocityModuleBoostTimer = 0;
			shipHoverModuleBarrelRollSuccess = false;

			rigidbody.velocity = Vector3.zero;
			rigidbody.angularVelocity = Vector3.zero;
			transform.position = shipRespawnManagerPosition;
			transform.rotation = Quaternion.Euler(shipRespawnManagerRotation);
			shipRespawnManagerTimer = 0;
		}
	}
}

using UnityEngine;
using System.Collections;

public class AGRShipDebugVisuals : AGRShipCameraModule {

	private Color cameraColor;
	public void DebugPass()
	{
		/* Camera Drawing */
		DrawCameraDebugInfo();
	}

	void DrawCameraDebugInfo()
	{
		/* Set Camera Color */
		cameraColor = Color.red;
		Gizmos.color = cameraColor;

		/* LookAt Position */
		Vector3 lookAtPosition = transform.TransformPoint(0, -shipCameraModuleSpringArmLookAtHeight, shipCameraModuleSpringArmLookAtLength);
		Gizmos.DrawWireSphere(lookAtPosition, 1);

		/* Close Wanted Position */
		Gizmos.color = Color.magenta;
		Vector3 wantedPosition = transform.TransformPoint(0, shipSettingsCameraCloseOffsetY, -shipSettingsCameraCloseOffsetZ);
		Gizmos.DrawWireSphere(wantedPosition, 0.2f);

		/* Far Wanted Position */
		Gizmos.color = Color.green;
		wantedPosition = transform.TransformPoint(0, shipSettingsCameraFarOffsetY, -shipSettingsCameraFarOffsetZ);
		Gizmos.DrawWireSphere(wantedPosition, 0.2f);
	}
}

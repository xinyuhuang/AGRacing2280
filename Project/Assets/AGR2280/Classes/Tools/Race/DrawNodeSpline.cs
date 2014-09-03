using UnityEngine;
using UnityEditor;
using System.Collections;

public class DrawNodeSpline : MonoBehaviour {

	public bool DrawingDisable;
	void OnDrawGizmos()
	{
		if (!DrawingDisable)
		{
			/* Draw Spheres */
			if (GetComponent<TrackInformation>().TrackNodeCount > 0)
			{
				for (int i = 0; i < GetComponent<TrackInformation>().TrackNodeCount; i++)
				{
					if (i == 0)
					{
						Gizmos.DrawWireSphere(GetComponent<TrackInformation>().TrackNodePositions[i], 10);
					} else 
					{
						Gizmos.DrawWireSphere(GetComponent<TrackInformation>().TrackNodePositions[i], 5);
					}
				}
			}

			/* Draw Lines */
			if (GetComponent<TrackInformation>().TrackNodeCount > 1)
			{
				for (int i = 1; i < GetComponent<TrackInformation>().TrackNodeCount; i++)
				{
					Debug.DrawLine(GetComponent<TrackInformation>().TrackNodePositions[i - 1], GetComponent<TrackInformation>().TrackNodePositions[i]);
				}
			}
		}
	}
}

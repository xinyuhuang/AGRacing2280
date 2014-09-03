using UnityEngine;
using System.Collections;

public class TrackInformation : MonoBehaviour {

	/* This class contains all of the information
	 * for each node on the track */

	/* Track Name */
	public string TrackName;

	/* Global Node Information */
	public int TrackNodeCount;

	/* Node instance information */
	public GameObject[] TrackNodeObject;
	public Vector3[] TrackNodePositions;
	public float[] TrackNodeTrackWidth;
	public bool[] TrackNodeIsRespawn;
	
}

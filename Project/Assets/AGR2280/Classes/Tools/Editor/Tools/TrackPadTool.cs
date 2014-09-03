using UnityEngine;
using UnityEditor;
using System.Collections;

public class TrackPadTool : EditorWindow {
	
	public enum PadTypes { Weapon, Speed, Start}
	public PadTypes PadToPlace;

	public static GameObject SpeedPad;
	public GameObject WeaponPad;
	public GameObject StartPad;

	public static GameObject InputObject;

	public static bool isPlacing;

	// Raycast
	Ray ray;
	RaycastHit hit;
	Vector3 hitLoc;
	Quaternion hitRot;

	[MenuItem("AGR2280/PadTool")]

	public static void ShowWindow()
	{
		EditorWindow thisWindow = EditorWindow.GetWindow(typeof(TrackPadTool));
		thisWindow.position = new Rect(Screen.width / 2, Screen.height / 2, 400, 600);
		thisWindow.minSize = new Vector2(400,600);
		thisWindow.maxSize = new Vector2(400,600);

		EditorInputManager.CreateInputManager();
		
		/* Set Input Objects */
		InputObject = GameObject.Find("EditorInput");
	}

	void Update()
	{
		if (isPlacing)
		{
			Selection.activeObject = InputObject;
			
			if (Camera.current != null)
			{
				/* Place Node */
				if (EditorInputManager.AGREditorKeyCode == KeyCode.P)
				{
					RaycastHit Hit;
					if (Physics.Raycast(EditorInputManager.AGREditorRay, out Hit, Mathf.Infinity, ~LayerMask.NameToLayer("Track_Floor")))
					{
						hitLoc = Hit.point;
						CreatePad();
					}
				}
			}
		}
	}

	void OnGUI()
	{
		GUILayout.Label("Pad Tool", EditorStyles.boldLabel);

		isPlacing = EditorGUILayout.BeginToggleGroup("Enabled", isPlacing);

		PadToPlace = (PadTypes)EditorGUILayout.EnumPopup("Place:", PadToPlace);
		SpeedPad = (GameObject)EditorGUILayout.ObjectField("Speed Prefab:",SpeedPad, typeof(GameObject), true);

		EditorGUILayout.EndToggleGroup();
	}

	void CreatePad()
	{
		GameObject newPad;
		if (PadToPlace == PadTypes.Speed)
		{
			/* Create and use raycast to get rotations */
			newPad = Instantiate(SpeedPad) as GameObject;
			newPad.transform.position = new Vector3(hitLoc.x, hitLoc.y + 4, hitLoc.z);
			RaycastHit padHit;
			if (Physics.Raycast(newPad.transform.position, -Vector3.up, out padHit))
			{
				Debug.Log("Place success!");
				newPad.transform.up = padHit.normal;
			}
			newPad.transform.position = new Vector3(hitLoc.x, hitLoc.y - 1.6f, hitLoc.z);
			newPad.name = "Speed Pad";

			/* Get the closest gate to rotate to */
			float distance = 10000;
			GameObject ChosenGate = null;
			if (GameObject.Find("NodeInformation"))
			{
				GameObject nodeInfo = GameObject.Find("NodeInformation");
				for (int i = 0; i < nodeInfo.GetComponent<TrackInformation>().TrackNodeCount; i++)
				{
					if (Vector3.Distance(newPad.transform.position, nodeInfo.GetComponent<TrackInformation>().TrackNodePositions[i]) < distance)
					{
						distance = Vector3.Distance(newPad.transform.position, nodeInfo.GetComponent<TrackInformation>().TrackNodePositions[i]);
						ChosenGate = nodeInfo.GetComponent<TrackInformation>().TrackNodeObject[i];
					}
				}
				newPad.transform.Rotate(Vector3.up * -newPad.transform.eulerAngles.y);
				newPad.transform.Rotate(Vector3.up * ChosenGate.transform.eulerAngles.y);
				newPad.transform.Rotate(Vector3.up * -90);

			} else 
			{
				Debug.Log("Could not find any gates - no auto rotate");
			}

		}
	}

}

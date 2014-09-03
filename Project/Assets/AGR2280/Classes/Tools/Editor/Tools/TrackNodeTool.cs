using UnityEngine;
using UnityEditor;
using System.Collections;

public class TrackNodeTool : EditorWindow {

	/* Editing */
	public bool NodeToolIsEditing;

	/* Raycast */
	Ray ray;
	RaycastHit hit;
	Vector3 hitLocation;
	Quaternion hitRotation;

	/* Game Objects */
	public static GameObject InputObject;

	public static int respawnJumps = 10;

	public static float racingLineSensitivity;
	public static float racingLineMultiplier;

	[MenuItem("AGR2280/Node Tool")]

	public static void ShowWindow()
	{
		/* Create Window and lock size */
		EditorWindow thisWindow = EditorWindow.GetWindow(typeof(TrackNodeTool));
		thisWindow.position = new Rect(Screen.width / 2, Screen.height / 2, 400, 400);
		thisWindow.minSize = new Vector2(400, 600);
		thisWindow.maxSize = new Vector2(400, 600);

		EditorInputManager.CreateInputManager();

		/* Set Input Objects */
		InputObject = GameObject.Find("EditorInput");

		/* Create Node Information Objects */
		if (!GameObject.Find("NodeInformation"))
		{
			GameObject nodeInfo = new GameObject("NodeInformation");
			nodeInfo.AddComponent<TrackInformation>();
			nodeInfo.AddComponent<DrawNodeSpline>();
		}

	}

	void Update () 
	{
		/* Select input object */
		if (NodeToolIsEditing)
		{
			Selection.activeObject = InputObject;

			if (Camera.current != null)
			{
				/* Place Node */
				if (EditorInputManager.AGREditorKeyCode == KeyCode.P)
				{
					Debug.Log("AGR2280: Pressed Place Key");
					RaycastHit nodeHit;
					if (Physics.Raycast(EditorInputManager.AGREditorRay, out nodeHit, Mathf.Infinity, ~LayerMask.NameToLayer("Track_Floor")))
					{
						/* Print a sucess message*/
						Debug.Log("AGR2280: Placed new node");

						/* Set GameObject */
						GameObject NodeInfo = GameObject.Find("NodeInformation");

						/* Hit Position */
						Vector3 hit = nodeHit.point;
						hit.y += 6;

						/* Increase position array to add new position */
						System.Array.Resize(ref NodeInfo.GetComponent<TrackInformation>().TrackNodePositions, NodeInfo.GetComponent<TrackInformation>().TrackNodePositions.Length + 1);

						/* Add the position */
						NodeInfo.GetComponent<TrackInformation>().TrackNodePositions[NodeInfo.GetComponent<TrackInformation>().TrackNodeCount] = hit;

						/* Increase the node Count */
						NodeInfo.GetComponent<TrackInformation>().TrackNodeCount++;
					}
				}
			}
		}
	}

	void OnGUI()
	{

		GUILayout.Label("Node Tool", EditorStyles.boldLabel);

		/* Create toggle group */
		NodeToolIsEditing = EditorGUILayout.BeginToggleGroup("Enabled", NodeToolIsEditing);

		/* Generate gates button */
		if (GUILayout.Button("Generate Gates"))
		{
			GenerateRaceGates();
		}

		/* Rotate gates button */
		if (GUILayout.Button("Rotate Gates"))
		{
			RotateRaceGates();
		}

		respawnJumps = EditorGUILayout.IntSlider("Respawn Jumps", respawnJumps, 5, 25);

		/* Rotate gates button */
		if (GUILayout.Button("Update Respawns"))
		{
			SetGateRespawns();
		}

		racingLineSensitivity = EditorGUILayout.Slider("Race line sensitivity", racingLineSensitivity, 0.01f, 0.05f);
		racingLineMultiplier = EditorGUILayout.Slider("Race line multiplier", racingLineMultiplier, 1, 2);

		/* Rotate gates button */
		if (GUILayout.Button("Generate Race Kine"))
		{
			GenerateRaceLine();
		}

		/* Rotate gates button */
		if (GUILayout.Button("Reset Race Line"))
		{
			ResetRaceLine();
		}
		
		
		/* End toggle group */
		EditorGUILayout.EndToggleGroup();
	}

	public void GenerateRaceGates()
	{
		/* Set GameObject */
		GameObject NodeInfo = GameObject.Find("NodeInformation");

		/* Resize array for storing objects in */
		System.Array.Resize(ref NodeInfo.GetComponent<TrackInformation>().TrackNodeObject, NodeInfo.GetComponent<TrackInformation>().TrackNodeCount);

		if (NodeInfo.GetComponent<TrackInformation>().TrackNodeCount > 0)
		{
			for (int i = 0; i < NodeInfo.GetComponent<TrackInformation>().TrackNodeCount; i++)
			{
				if (!GameObject.Find("RaceGate_" + i))
				{
					/* Setup needed stuff */
					Vector3 position = NodeInfo.GetComponent<TrackInformation>().TrackNodePositions[i];

					Material NodeMaterial = Resources.Load("AGREditing/Materials/AGRGate") as Material;
					Object NodeArrow = Resources.Load("AGREditing/NodeArrow") as Object;

					/* Create node parent */
					GameObject nodeParent;
					if (!GameObject.Find("NodeParent"))
					{
						nodeParent = new GameObject("NodeParent");
					} else 
					{
						nodeParent = GameObject.Find("NodeParent");
					}

					/* Create Node */
					GameObject newNode = GameObject.CreatePrimitive(PrimitiveType.Cube);
					newNode.transform.parent = nodeParent.transform;
					newNode.transform.position = position;
					newNode.name = "RaceGate_" + i;

					newNode.AddComponent<NoRender>();
					newNode.renderer.material = NodeMaterial;

					NodeInfo.GetComponent<TrackInformation>().TrackNodeObject[i] = newNode;

					newNode.AddComponent<NodeID>();
					newNode.GetComponent<NodeID>().NodeNumber = i;

					newNode.transform.localScale = new Vector3(85, 50, 3);

					newNode.GetComponent<BoxCollider>().isTrigger = true;
					newNode.layer = LayerMask.NameToLayer("Ignore Raycast");
					newNode.tag = "Gate";

					/* Create Gate Direction Arrow */
					GameObject newArrow = Instantiate(NodeArrow) as GameObject;
					newArrow.transform.parent = newNode.transform;
					newArrow.transform.localPosition = Vector3.zero;
					newArrow.transform.localRotation = Quaternion.Euler(0, 90, 0);
					newArrow.transform.localScale = new Vector3(0.6f, 0.039f, 0.024f);
					newArrow.AddComponent<NoRender>();
					newArrow.name = "NodeArrow_" + i;

					/* Create AI Helper */
					Material AIHelperMat = Resources.Load("AGREditing/Materials/GateNode") as Material;
					GameObject AIHelper = GameObject.CreatePrimitive(PrimitiveType.Cube);
					AIHelper.AddComponent<NoRender>();
					AIHelper.transform.position = newNode.transform.position;
					AIHelper.transform.parent = newNode.transform;
					AIHelper.name = "AIHelper_" + i;
					AIHelper.renderer.material = AIHelperMat;
					AIHelper.GetComponent<BoxCollider>().enabled = false;

					/* Create Auto Pilot Helper */
					Material APHelperMat = Resources.Load("AGREditing/Materials/AGRArrow") as Material;
					GameObject APHelper = GameObject.CreatePrimitive(PrimitiveType.Cube);
					APHelper.AddComponent<NoRender>();
					APHelper.transform.position = newNode.transform.position;
					APHelper.transform.parent = newNode.transform;
					APHelper.name = "APHelper_" + i;
					APHelper.renderer.material = APHelperMat;
					APHelper.GetComponent<BoxCollider>().enabled = false;
				}
			}
		}
	}

	public void RotateRaceGates()
	{
		/* Set GameObject */
		GameObject NodeInfo = GameObject.Find("NodeInformation");

		if (NodeInfo.GetComponent<TrackInformation>().TrackNodeCount > 0)
		{
			for (int i = 1; i < NodeInfo.GetComponent<TrackInformation>().TrackNodeCount; i++)
			{
				/* Look At Previous Gate */
				Vector3 thisGatePosition = NodeInfo.GetComponent<TrackInformation>().TrackNodePositions[i];
				Vector3 previousGatePosition = NodeInfo.GetComponent<TrackInformation>().TrackNodePositions[i - 1];

				Quaternion lookAt = Quaternion.LookRotation(previousGatePosition - thisGatePosition);
				NodeInfo.GetComponent<TrackInformation>().TrackNodeObject[i].transform.rotation = lookAt;

				/* Store Y Axis */
				float yRot = NodeInfo.GetComponent<TrackInformation>().TrackNodeObject[i].transform.eulerAngles.y + 180;

				/* Raycast to get track normals */
				RaycastHit hit;
				if (Physics.Raycast(NodeInfo.GetComponent<TrackInformation>().TrackNodePositions[i], -Vector3.down, out hit))
				{
					if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Track_Floor"))
					{
						NodeInfo.GetComponent<TrackInformation>().TrackNodeObject[i].transform.up = hit.normal;
					}
				}

				/* Restore Y Axis */
				Quaternion rotation = Quaternion.Euler
					(
						-NodeInfo.GetComponent<TrackInformation>().TrackNodeObject[i].transform.eulerAngles.x,
						yRot,
						-NodeInfo.GetComponent<TrackInformation>().TrackNodeObject[i].transform.eulerAngles.z
					);

				NodeInfo.GetComponent<TrackInformation>().TrackNodeObject[i].transform.rotation = rotation;

			}
		}
	}

	public void SetGateRespawns()
	{
		/* Set GameObject */
		GameObject NodeInfo = GameObject.Find("NodeInformation");

		/* Resize array*/
		System.Array.Resize(ref NodeInfo.GetComponent<TrackInformation>().TrackNodeIsRespawn, NodeInfo.GetComponent<TrackInformation>().TrackNodeCount);

		if (NodeInfo.GetComponent<TrackInformation>().TrackNodeCount > respawnJumps)
		{
			/* Reset Gates */
			for (int i = 0; i < NodeInfo.GetComponent<TrackInformation>().TrackNodeCount; i++)
			{
				NodeInfo.GetComponent<TrackInformation>().TrackNodeIsRespawn[i] = false;
			}

			/* Set Gates */
			for (int i = 0; i < NodeInfo.GetComponent<TrackInformation>().TrackNodeCount; i += respawnJumps)
			{
				if (i < NodeInfo.GetComponent<TrackInformation>().TrackNodeCount)
				{
					NodeInfo.GetComponent<TrackInformation>().TrackNodeIsRespawn[i] = true;
				}
			}
		}
	}

	public void GenerateRaceLine()
	{
		/* Set GameObject */
		GameObject NodeInfo = GameObject.Find("NodeInformation");

		if (NodeInfo.GetComponent<TrackInformation>().TrackNodeCount > 1)
		{
			/* Zero out positions */
			for (int i = 0; i < NodeInfo.GetComponent<TrackInformation>().TrackNodeCount; i++)
			{
				NodeInfo.GetComponent<TrackInformation>().TrackNodeObject[i].transform.Find("AIHelper_" + i).transform.localPosition = Vector3.zero;
			}

			/* Generate race line */
			for (int i = 1; i < NodeInfo.GetComponent<TrackInformation>().TrackNodeCount; i++)
			{
				Vector3 localDistance = NodeInfo.GetComponent<TrackInformation>().TrackNodeObject[i - 1].transform.InverseTransformPoint
					(
						NodeInfo.GetComponent<TrackInformation>().TrackNodeObject[i].transform.position
					);
				localDistance.y = 0;
				localDistance.z = 0;
				float distance = -localDistance.magnitude * racingLineMultiplier;
				if (distance < racingLineSensitivity || distance > racingLineSensitivity)
				{
					NodeInfo.GetComponent<TrackInformation>().TrackNodeObject[i].transform.Find("AIHelper_" + i).transform.localPosition = new Vector3(distance, 0, 0);
				}
			}
		}
	}

	public void ResetRaceLine()
	{
		/* Set GameObject */
		GameObject NodeInfo = GameObject.Find("NodeInformation");
		
		if (NodeInfo.GetComponent<TrackInformation>().TrackNodeCount > 1)
		{
			/* Zero out positions */
			for (int i = 0; i < NodeInfo.GetComponent<TrackInformation>().TrackNodeCount; i++)
			{
				NodeInfo.GetComponent<TrackInformation>().TrackNodeObject[i].transform.Find("AIHelper_" + i).transform.localPosition = Vector3.zero;
			}
			
		}
	}
}

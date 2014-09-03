using UnityEngine;
using System.Collections;

public class TrackEditorGUI : MonoBehaviour {

	public GUISkin skin;

	private bool fileMenuOpen;

	private Rect ObjectWindow = new Rect(20, 20, 400, 400);

	private bool ObjecrWindowOpen;

	private enum CatalogueFolders {Adverts, Audio, Buildings, Crowds, Decorations, Scenery, Pads, Roads};
	private CatalogueFolders currentFolder;

	private Vector2 DecoSliderPos;
	private int DecoCurrentSelected = 9000;

	private GameObject currentObjectToPlace;
	public bool hasObject;

	private int placeHeight;
	private int placeRotation;

	/* Audio Clips */
	public AudioClip ObjectMove;
	public AudioClip ObjectPlaced;

	void Start()
	{
		/* Add audio source to camera */
		gameObject.AddComponent<AudioSource>();
	}

	void Update()
	{

		/* Update Place Vars */
		if (Input.GetKeyDown(KeyCode.KeypadPlus))
		{
			int increaseAmount = 5;
			if (Input.GetKey(KeyCode.LeftShift))
			{
				increaseAmount = 10;
			}
			placeHeight += increaseAmount;
			GetComponent<TrackEditorGrid>().placeHeight = placeHeight;
			GameObject.Find("GroundPlane").transform.Translate(Vector3.up * increaseAmount);

			/* Set and play sound */
			GetComponent<AudioSource>().clip = ObjectMove;
			GetComponent<AudioSource>().Play();
		}

		if (Input.GetKeyDown(KeyCode.KeypadMinus))
		{
			int increaseAmount = 5;
			if (Input.GetKey(KeyCode.LeftShift))
			{
				increaseAmount = 10;
			}
			placeHeight -= increaseAmount;
			GetComponent<TrackEditorGrid>().placeHeight = placeHeight;
			GameObject.Find("GroundPlane").transform.Translate(Vector3.down * increaseAmount);

			/* Set and play sound */
			GetComponent<AudioSource>().clip = ObjectMove;
			GetComponent<AudioSource>().Play();
		}

		if (Input.GetKeyDown(KeyCode.Period))
		{
			int increaseAmount = 5;
			if (Input.GetKey(KeyCode.LeftShift))
			{
				increaseAmount = 10;
			}
			placeRotation += increaseAmount;

			/* Set and play sound */
			GetComponent<AudioSource>().clip = ObjectMove;
			GetComponent<AudioSource>().Play();
		}

		if (Input.GetKeyDown(KeyCode.Comma))
		{
			int increaseAmount = 5;
			if (Input.GetKey(KeyCode.LeftShift))
			{
				increaseAmount = 10;
			}
			placeRotation -= increaseAmount;

			/* Set and play sound */
			GetComponent<AudioSource>().clip = ObjectMove;
			GetComponent<AudioSource>().Play();
		}

		if (Input.GetKeyDown(KeyCode.Backspace))
		{
			placeHeight = 0;
			GetComponent<TrackEditorGrid>().placeHeight = placeHeight;
			GameObject.Find("GroundPlane").transform.position = Vector3.zero;
		}

		/* Placing */
		Ray PlaceRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit PlaceHit;
		if (Physics.Raycast(PlaceRay, out PlaceHit) && hasObject)
		{
			currentObjectToPlace.transform.position = new Vector3
				(
					Mathf.Round(PlaceHit.point.x / 100) * 100,
					placeHeight,
					Mathf.Round(PlaceHit.point.z / 100) * 100
				);

			currentObjectToPlace.transform.rotation = Quaternion.Euler(0, placeRotation, 0);
			if (Input.GetMouseButtonDown(1) && !Input.GetKey(KeyCode.LeftAlt))
			{
				currentObjectToPlace.transform.position = new Vector3
					(
						Mathf.Round(PlaceHit.point.x / 100) * 100,
						placeHeight,
						Mathf.Round(PlaceHit.point.z / 100) * 100
					);

				if (currentObjectToPlace.GetComponent<MeshCollider>())
				{
					currentObjectToPlace.GetComponent<MeshCollider>().enabled = true;
				}

				/* Add Cubemap Updater */
				currentObjectToPlace.AddComponent<TrackGenerationCubemapUpdater>();

				/* Set and play sound */
				GetComponent<AudioSource>().clip = ObjectPlaced;
				GetComponent<AudioSource>().Play();

				hasObject = false;
			}
		}
	}

	void OnGUI()
	{
		GUIStyle style = skin.GetStyle("ShipHUD");
		style.alignment = TextAnchor.UpperLeft;


		/* Toolbar Background */
		GUI.Box(new Rect(0,0, Screen.width, Screen.height * 0.03f),"", style);

		style.normal.textColor = new Color(0.3f, 0.3f, 0.3f);
		style.fontSize = Mathf.RoundToInt((Screen.width + Screen.height) * 0.01f);

		if (GUI.Button(new Rect(Screen.width * 0.002f, 0, Screen.width * 0.03f, Screen.height * 0.02f), "File", style))
		{
			if (fileMenuOpen)
			{
				fileMenuOpen = false;
			} else if (!fileMenuOpen)
			{
				fileMenuOpen = true;
			}
		}

		/* File Menu */
		if (fileMenuOpen)
		{
			GUI.Box(new Rect(0,Screen.height * 0.03f, Screen.width * 0.05f, Screen.height * 0.4f),"", style);
		}

		/* Watermark */
		style.fontSize = Mathf.RoundToInt((Screen.width + Screen.height) * 0.01f);
		GUI.Label(new Rect(0, Screen.height * 0.96f, 100, 0), "AGR2280 Track Editor - ALPHA", style);

		/* Object Window */
		GUI.color = new Color(1,1,1, 0.6f);
		if (ObjecrWindowOpen)
		{
			ObjectWindow.height = Mathf.MoveTowards(ObjectWindow.height, 30, 50);
		} else 
		{
			ObjectWindow.height = Mathf.MoveTowards(ObjectWindow.height, 400, 50);
		}
		ObjectWindow = GUI.Window(0, ObjectWindow, ObjectCatalogue, "", style);
	}

	void ObjectCatalogue(int windowID)
	{
		GUIStyle style = skin.GetStyle("ShipHUD");
		GUI.color = new Color(1,1,1, 1);

		/* Title Bar */
		GUI.Box(new Rect(0,0, 400, 30),"", style);


		/* Minimise Button */
		style.fontSize = 28;
		if (GUI.Button(new Rect(370,0, 30, 30), "+", style))
		{
			if (ObjecrWindowOpen)
			{
				ObjecrWindowOpen = false;
			} else if (!ObjecrWindowOpen)
			{
				ObjecrWindowOpen = true;
			}
		}

		/* Allow the window to be dragged */
		GUI.DragWindow(new Rect(0,0, 10000, 30));

		/* Title */
		style.fontSize = 28;
		style.alignment = TextAnchor.MiddleCenter;
		GUI.Label(new Rect(0, 15, 400, 0),"Object Catalogue", style);


		/* Side Bar Seperator */
		GUI.Box(new Rect(105,40, 5, 330),"", style);

		/* Side Bar Buttons */
		style.fontSize = 12;
		int buttonHeight = 48;
		if (GUI.Button(new Rect(0, buttonHeight, 100, 35), "Advertisments", style))
		{
			currentFolder = CatalogueFolders.Adverts;
		}

		buttonHeight += 40;
		if (GUI.Button(new Rect(0, buttonHeight, 100, 35), "Ambient Sounds", style))
		{
			currentFolder = CatalogueFolders.Audio;	
		}

		buttonHeight += 40;
		if (GUI.Button(new Rect(0, buttonHeight, 100, 35), "City Buildings", style))
		{
			currentFolder = CatalogueFolders.Buildings;
		}

		buttonHeight += 40;
		if (GUI.Button(new Rect(0, buttonHeight, 100, 35), "Crowd Stands", style))
		{
			currentFolder = CatalogueFolders.Crowds;
		}

		buttonHeight += 40;
		if (GUI.Button(new Rect(0, buttonHeight, 100, 35), "Decorations", style))
		{
			currentFolder = CatalogueFolders.Decorations;
		}

		buttonHeight += 40;
		if (GUI.Button(new Rect(0, buttonHeight, 100, 35), "Foilage", style))
		{
			currentFolder = CatalogueFolders.Scenery;
		}

		buttonHeight += 40;
		if (GUI.Button(new Rect(0, buttonHeight, 100, 35), "Track Pads", style))
		{
			currentFolder = CatalogueFolders.Pads;
		}

		buttonHeight += 40;
		if (GUI.Button(new Rect(0, buttonHeight, 100, 35), "Track Pieces", style))
		{
			currentFolder = CatalogueFolders.Roads;
		}

		/* Catalogue Background*/
		GUI.Box(new Rect(115,40, 278, 330),"", style);


		GUI.BeginGroup(new Rect(115,40, 278, 330));


		/* Decorations */
		if (currentFolder == CatalogueFolders.Decorations)
		{
			DecoSliderPos = GUI.BeginScrollView(new Rect(0,0, 278, 330), DecoSliderPos,new Rect(0,0, 256, 5000));
			
			int drawHeight = 0;
			int drawRight = 0;
			for (int i = 0; i < TrackEditorObjectCatalogueLists.TrackEditorObjectDecorationPrefab.Length; i++)
			{

				/* Interactive Button */
				GUI.color = new Color(1,1,1,1);
				if (GUI.Button(new Rect(drawRight, drawHeight, 128,128), new GUIContent("Men", TrackEditorObjectCatalogueLists.TrackEditorObjectDecorationName[i]), style))
				{
					if (!hasObject)
					{
						DecoCurrentSelected = i;
						
						/* Instantiate the object far away from the placable grid */
						currentObjectToPlace = Instantiate(TrackEditorObjectCatalogueLists.TrackEditorObjectDecorationPrefab[i], new Vector3(90000,0,0), Quaternion.identity) as GameObject;
						if (currentObjectToPlace.GetComponent<MeshCollider>())
						{
							currentObjectToPlace.GetComponent<MeshCollider>().enabled = false;
						}

						hasObject = true;
					}

				}

				/* Draw the Texture */
				GUI.DrawTexture(new Rect(drawRight, drawHeight, 128,128), TrackEditorObjectCatalogueLists.TrackEditorObjectDecorationTexture[i]);

				/* Draw the Tooltip */
				GUI.Label(new Rect(drawRight, drawHeight, 128,128), GUI.tooltip);
				if (GUI.tooltip != "")
				{
					GUI.Label(new Rect(drawRight, drawHeight + 32, 128,122), TrackEditorObjectCatalogueLists.TrackEditorObjectDecorationDesc[i]);
				}

				/* Draw the selected overlay */
				if (i == DecoCurrentSelected)
				{
					GUI.color = new Color(1,1,1,0.4f);
					GUI.Box(new Rect(drawRight, drawHeight, 128,128),"", style);
				}

				/* Grid Position Logic */
				drawRight += 128;
				if (drawRight > 128)
				{
					drawHeight += 128;
					drawRight = 0;
				}
			}

			                 
			GUI.EndScrollView();
		} else 
		{
			DecoCurrentSelected = 0;
		}

		GUI.EndGroup();

	}
}

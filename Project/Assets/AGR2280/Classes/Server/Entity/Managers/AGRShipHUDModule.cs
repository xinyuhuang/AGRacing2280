using UnityEngine;
using System.Collections;

public class AGRShipHUDModule : AGRShipRespawnManager {

	/* HUD movement */
	private float hudOffsetX;
	private float hudOffsetY;
	private float hudSensitivity = 3;

	private bool loadedGUI;
	private Texture2D GUITexture;
	private Texture2D GUIGrid;

	private int completeScreenSelection;

	private int screenshotNumber;


	void OnGUI()
	{
		if (!loadedGUI)
		{
			GUITexture = Resources.Load("GUI/GUIBackground") as Texture2D;
			loadedGUI = true;
		}

		if (!finishedRace)
		{
			hudOffsetX = shipCameraModuleShipRotation * hudSensitivity;
			hudOffsetY = rigidbody.velocity.y / (hudSensitivity * 2);
			if (hudOffsetY > 0)
			{
				hudOffsetY = 0;
			}

			GUI.BeginGroup(new Rect(hudOffsetX,hudOffsetY, Screen.width, Screen.height));

			GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height), GUITexture);

			shipHUDModuleStyle.customStyles[0].alignment = TextAnchor.UpperLeft;

			/* Get the ship HUD and assign it to a local variable */
			GUIStyle shipStyle = shipHUDModuleStyle.GetStyle("ShipHUD");

			/* Setup drawing variables - this will make it easier to make adjustments */
			Vector2 screenDim = new Vector2(Screen.width, Screen.height);
			Rect shipHUDRect = new Rect(0,0,0,0);

			Vector2 startPos = new Vector2(0,0);
			Vector2 drawDim = new Vector2(0,0);

			/* Draw Thrust Bar */
			float thrustAmount = (shipVelocityModuleVelocity / shipSettingsEngineMaxThrust) * 0.13f;

			startPos.x = screenDim.x * 0.965f;
			startPos.y = screenDim.y * 0.84f;
			drawDim.x = -screenDim.x * thrustAmount;
			drawDim.y = -screenDim.y * 0.049f;
			GUI.color = new Color(1, 0, 0, 0.5f);
			GUI.Box(new Rect(startPos.x,startPos.y, drawDim.x, drawDim.y), "", shipStyle);

			/* Draw KHM */
			startPos.x = screenDim.x * 0.845f;
			startPos.y = screenDim.y * 0.81f;
			drawDim.x = screenDim.x * 0.1f;
			drawDim.y = -screenDim.y * 0f;
			GUI.color = new Color(1, 1, 1);
			shipHUDModuleStyle.customStyles[0].fontSize = Mathf.RoundToInt((screenDim.x + screenDim.y) * 0.006f);
			GUI.Label(new Rect(startPos.x,startPos.y, drawDim.x, drawDim.y), "KM/H", shipStyle);

			/* Draw Speed Value */
			float speed = Mathf.RoundToInt(transform.InverseTransformDirection(rigidbody.velocity).z * (rigidbody.drag / 2));
			startPos.x = screenDim.x * 0.81f;
			startPos.y = screenDim.y * 0.8f;
			drawDim.x = screenDim.x * 0.1f;
			drawDim.y = -screenDim.y * 0f;
			GUI.color = new Color(1, 1, 1);
			shipHUDModuleStyle.customStyles[0].fontSize = Mathf.RoundToInt((screenDim.x + screenDim.y) * 0.01f);
			GUI.Label(new Rect(startPos.x,startPos.y, drawDim.x, drawDim.y), speed.ToString(), shipStyle);

			/* Update top speed */
			if (currentLap > 0  && currentLap <= 5)
			{
				if (speed > topSpeed[currentLap - 1])
				{
					topSpeed[currentLap - 1] = Mathf.RoundToInt(speed);
				}
			}

			/* Draw Speed Text*/
			startPos.x = screenDim.x * 0.92f;
			startPos.y = screenDim.y * 0.793f;
			drawDim.x = screenDim.x * 0.1f;
			drawDim.y = -screenDim.y * 0f;
			GUI.color = new Color(1, 1, 1);
			shipHUDModuleStyle.customStyles[0].fontSize = Mathf.RoundToInt((screenDim.x + screenDim.y) * 0.008f);
			GUI.Label(new Rect(startPos.x,startPos.y, drawDim.x, drawDim.y), "SPEED", shipStyle);

			/* Draw Thrust Bar */
			bool thrust = GetComponent<PlayerInputManager>().playerInputButtonThruster;
			startPos.x = screenDim.x * 0.965f;
			startPos.y = screenDim.y * 0.902f;
			if (thrust)
			{
				drawDim.x = -screenDim.x * 0.1658f;
			} else 
			{
				drawDim.x = -screenDim.x * 0;
			}
			drawDim.y = -screenDim.y * 0.049f;
			GUI.color = new Color(1, 0, 0, 0.5f);
			GUI.Box(new Rect(startPos.x,startPos.y, drawDim.x, drawDim.y), "", shipStyle);

			/* Draw Thrust Text*/
			startPos.x = screenDim.x * 0.912f;
			startPos.y = screenDim.y * 0.86f;
			drawDim.x = screenDim.x * 0.1f;
			drawDim.y = -screenDim.y * 0f;
			GUI.color = new Color(1, 1, 1);
			shipHUDModuleStyle.customStyles[0].fontSize = Mathf.RoundToInt((screenDim.x + screenDim.y) * 0.008f);
			GUI.Label(new Rect(startPos.x,startPos.y, drawDim.x, drawDim.y), "THRUST", shipStyle);

			/* Draw Current Seperator */
			startPos.x = screenDim.x * 0.085f;
			startPos.y = screenDim.y * 0.14f;
			drawDim.x = screenDim.x * 0.1f;
			drawDim.y = -screenDim.y * 0;
			GUI.color = new Color(1, 0, 0, 0.5f);
			shipHUDModuleStyle.customStyles[0].fontSize = Mathf.RoundToInt((screenDim.x + screenDim.y) * 0.03f);
			shipHUDModuleStyle.customStyles[0].alignment = TextAnchor.UpperRight;

			string thisLap = currentLap.ToString();
			GUI.Label(new Rect(startPos.x,startPos.y, drawDim.x, drawDim.y), thisLap, shipStyle);

			/* Draw Max Seperator */
			startPos.x = screenDim.x * 0.195f;
			startPos.y = screenDim.y * 0.035f;
			drawDim.x = screenDim.x * 0.1f;
			drawDim.y = -screenDim.y * 0;
			GUI.color = new Color(1, 1, 1);
			shipHUDModuleStyle.customStyles[0].fontSize = Mathf.RoundToInt((screenDim.x + screenDim.y) * 0.04f);
			shipHUDModuleStyle.customStyles[0].alignment = TextAnchor.UpperLeft;
			GUI.Label(new Rect(startPos.x,startPos.y, drawDim.x, drawDim.y), "5", shipStyle);

			/* Draw Current MS */
			startPos.x = screenDim.x * 0.04f;
			startPos.y = screenDim.y * 0.79f;
			drawDim.x = screenDim.x * 0.1f;
			drawDim.y = -screenDim.y * 0;
			GUI.color = new Color(1, 1, 1);
			shipHUDModuleStyle.customStyles[0].fontSize = Mathf.RoundToInt((screenDim.x + screenDim.y) * 0.014f);
			shipHUDModuleStyle.customStyles[0].alignment = TextAnchor.UpperRight;
			string currentMS = currentLap_Miliseconds + ":";
			if (currentLap_Miliseconds < 10)
			{
				currentMS = "0" + currentLap_Miliseconds + ":";
			}
			GUI.Label(new Rect(startPos.x,startPos.y, drawDim.x, drawDim.y), currentMS, shipStyle);

			/* Draw Current S */
			startPos.x = screenDim.x * 0.068f;
			startPos.y = screenDim.y * 0.79f;
			drawDim.x = screenDim.x * 0.1f;
			drawDim.y = -screenDim.y * 0;
			GUI.color = new Color(1, 1, 1);
			shipHUDModuleStyle.customStyles[0].fontSize = Mathf.RoundToInt((screenDim.x + screenDim.y) * 0.014f);
			shipHUDModuleStyle.customStyles[0].alignment = TextAnchor.UpperRight;
			string currentS = currentLap_Seconds + ":";
			if (currentLap_Seconds < 10)
			{
				currentS = "0" + currentLap_Seconds + ":";
			}
		    GUI.Label(new Rect(startPos.x,startPos.y, drawDim.x, drawDim.y), currentS, shipStyle);

			/* Draw Current S */
			startPos.x = screenDim.x * 0.09f;
			startPos.y = screenDim.y * 0.79f;
			drawDim.x = screenDim.x * 0.1f;
			drawDim.y = -screenDim.y * 0;
			GUI.color = new Color(1, 1, 1);
			shipHUDModuleStyle.customStyles[0].fontSize = Mathf.RoundToInt((screenDim.x + screenDim.y) * 0.014f);
			shipHUDModuleStyle.customStyles[0].alignment = TextAnchor.UpperRight;
			string currentM = currentLap_Minutes.ToString();
			if (currentLap_Minutes < 10)
			{
				currentM = "0" + currentLap_Minutes.ToString();
			}
			GUI.Label(new Rect(startPos.x,startPos.y, drawDim.x, drawDim.y), currentM, shipStyle);

			/* Draw Best MS */
			startPos.x = screenDim.x * 0.04f;
			startPos.y = screenDim.y * 0.852f;
			drawDim.x = screenDim.x * 0.1f;
			drawDim.y = -screenDim.y * 0;
			GUI.color = new Color(1, 1, 1);
			shipHUDModuleStyle.customStyles[0].fontSize = Mathf.RoundToInt((screenDim.x + screenDim.y) * 0.014f);
			shipHUDModuleStyle.customStyles[0].alignment = TextAnchor.UpperRight;
			string bestMS = bestLap_Miliseconds + ":";
			if (bestLap_Miliseconds < 10)
			{
				bestMS = "0" + bestLap_Miliseconds + ":";
			}
			GUI.Label(new Rect(startPos.x,startPos.y, drawDim.x, drawDim.y), bestMS, shipStyle);
			
			/* Draw Best S */
			startPos.x = screenDim.x * 0.068f;
			startPos.y = screenDim.y * 0.852f;
			drawDim.x = screenDim.x * 0.1f;
			drawDim.y = -screenDim.y * 0;
			GUI.color = new Color(1, 1, 1);
			shipHUDModuleStyle.customStyles[0].fontSize = Mathf.RoundToInt((screenDim.x + screenDim.y) * 0.014f);
			shipHUDModuleStyle.customStyles[0].alignment = TextAnchor.UpperRight;
			string bestS = bestLap_Seconds + ":";
			if (bestLap_Seconds < 10)
			{
				bestS = "0" + bestLap_Seconds + ":";
			}
			GUI.Label(new Rect(startPos.x,startPos.y, drawDim.x, drawDim.y), bestS, shipStyle);
			
			/* Draw Best S */
			startPos.x = screenDim.x * 0.09f;
			startPos.y = screenDim.y * 0.852f;
			drawDim.x = screenDim.x * 0.1f;
			drawDim.y = -screenDim.y * 0;
			GUI.color = new Color(1, 1, 1);
			shipHUDModuleStyle.customStyles[0].fontSize = Mathf.RoundToInt((screenDim.x + screenDim.y) * 0.014f);
			shipHUDModuleStyle.customStyles[0].alignment = TextAnchor.UpperRight;
			string bestM = bestLap_Minutes.ToString();
			if (bestLap_Minutes < 10)
			{
				bestM = "0" + bestLap_Minutes.ToString();
			}
			GUI.Label(new Rect(startPos.x,startPos.y, drawDim.x, drawDim.y), bestM, shipStyle);



			GUI.EndGroup();
		} else
		{
			GUI.BeginGroup(new Rect(Screen.width * 0.1f,Screen.height * 0.1f, Screen.width * 0.8f, Screen.height * 0.8f));

			/* Get the ship HUD and assign it to a local variable */
			GUIStyle shipStyle = shipHUDModuleStyle.GetStyle("ShipHUD");

			/* Draw background box */
			GUI.color = new Color(0, 0, 0, 0.6f);
			GUI.Box(new Rect(0,0, Screen.width * 0.8f, Screen.height * 0.8f), "", shipStyle);

			/* Draw options list background */
			GUI.color = new Color(0, 0, 0, 0.8f);
			GUI.Box(new Rect(0,0, Screen.width * 0.1f, Screen.height * 0.8f), "", shipStyle);

			/* Draw options text*/
			GUI.color = new Color(1, 0 ,0 , 0.5f);
			shipHUDModuleStyle.customStyles[0].fontSize = Mathf.RoundToInt((Screen.width * 0.8f + Screen.height * 0.8f) * 0.014f);
			shipHUDModuleStyle.customStyles[0].alignment = TextAnchor.MiddleCenter;
			GUI.Label(new Rect(0,Screen.height * 0.1f, Screen.width * 0.1f, Screen.height * 0), "MENU", shipStyle);

			GUI.color = new Color(1, 0 ,0 , 0.5f);
			shipHUDModuleStyle.customStyles[0].fontSize = Mathf.RoundToInt((Screen.width * 0.8f + Screen.height * 0.8f) * 0.014f);
			shipHUDModuleStyle.customStyles[0].alignment = TextAnchor.MiddleCenter;
			GUI.Label(new Rect(0,Screen.height * 0.15f, Screen.width * 0.1f, Screen.height * 0), "RESTART", shipStyle);

			GUI.color = new Color(1, 0 ,0 , 0.5f);
			shipHUDModuleStyle.customStyles[0].fontSize = Mathf.RoundToInt((Screen.width * 0.8f + Screen.height * 0.8f) * 0.014f);
			shipHUDModuleStyle.customStyles[0].alignment = TextAnchor.MiddleCenter;
			GUI.Label(new Rect(0,Screen.height * 0.2f, Screen.width * 0.1f, Screen.height * 0), "PHOTO", shipStyle);

			GUI.color = new Color(1, 0 ,0 , 0.5f);
			shipHUDModuleStyle.customStyles[0].fontSize = Mathf.RoundToInt((Screen.width * 0.8f + Screen.height * 0.8f) * 0.014f);
			shipHUDModuleStyle.customStyles[0].alignment = TextAnchor.MiddleCenter;
			GUI.Label(new Rect(0,Screen.height * 0.25f, Screen.width * 0.1f, Screen.height * 0), "OPTIONS", shipStyle);

			GUI.color = new Color(1, 0 ,0 , 0.5f);
			shipHUDModuleStyle.customStyles[0].fontSize = Mathf.RoundToInt((Screen.width * 0.8f + Screen.height * 0.8f) * 0.014f);
			shipHUDModuleStyle.customStyles[0].alignment = TextAnchor.MiddleCenter;
			GUI.Label(new Rect(0,Screen.height * 0.3f, Screen.width * 0.1f, Screen.height * 0), "QUIT", shipStyle);


			/* Draw lap results background */
			GUI.color = new Color(0, 0, 0, 0.8f);
			GUI.Box(new Rect(Screen.width * 0.13f,Screen.height * 0.1f, Screen.width * 0.3f, Screen.height * 0.65f), "", shipStyle);

			/* Draw title bar */
			GUI.color = new Color(0, 0, 0, 1);
			GUI.Box(new Rect(Screen.width * 0.13f,Screen.height * 0.1f, Screen.width * 0.3f, Screen.height * 0.05f), "", shipStyle);

			/* Draw lap results title text*/
			GUI.color = new Color(1,1,1,1);
			shipHUDModuleStyle.customStyles[0].fontSize = Mathf.RoundToInt((Screen.width * 0.8f + Screen.height * 0.8f) * 0.014f);
			shipHUDModuleStyle.customStyles[0].alignment = TextAnchor.MiddleCenter;
			GUI.Label(new Rect(Screen.width * 0.13f,Screen.height * 0.13f, Screen.width * 0.3f, Screen.height * 0), "BEST LAPS", shipStyle);

			/* Draw laps */
			float height = 0.2f;

			for (int i = 0; i < 5; i++)
			{
				GUI.color = new Color(1, 0 ,0 , 0.5f);
				shipHUDModuleStyle.customStyles[0].alignment = TextAnchor.MiddleCenter;
				shipHUDModuleStyle.customStyles[0].fontSize = Mathf.RoundToInt((Screen.width * 0.8f + Screen.height * 0.8f) * 0.013f);
				shipHUDModuleStyle.customStyles[0].fontStyle = FontStyle.Bold;
				GUI.Label(new Rect(Screen.width * 0.13f, Screen.height * height, Screen.width * 0.3f, Screen.height * 0), "LAP " + (i + 1), shipStyle);

				shipHUDModuleStyle.customStyles[0].fontSize = Mathf.RoundToInt((Screen.width * 0.8f + Screen.height * 0.8f) * 0.011f);
				shipHUDModuleStyle.customStyles[0].fontStyle = FontStyle.Normal;
				string MS = Lap_Miliseconds[i].ToString();
				if (Lap_Miliseconds[i] < 10)
				{
					MS = "0" + Lap_Miliseconds[i].ToString();
				}

				string S = Lap_Seconds[i].ToString();
				if (Lap_Seconds[i] < 10)
				{
					S = "0" + Lap_Seconds[i].ToString();
				}

				string M = Lap_Minutes[i].ToString();
				if (Lap_Minutes[i] < 10)
				{
					M = "0" + Lap_Minutes[i].ToString();
				}

				string lapTimeToDisplay = MS + ":" + S + ":" + M;

				GUI.color = new Color(1, 0.3f ,0.3f , 0.5f);
				GUI.Label(new Rect(Screen.width * 0.13f, Screen.height * (height + 0.03f), Screen.width * 0.3f, Screen.height * 0), lapTimeToDisplay , shipStyle);

				shipHUDModuleStyle.customStyles[0].fontSize = Mathf.RoundToInt((Screen.width * 0.8f + Screen.height * 0.8f) * 0.010f);

				int padsHit = 0;

				GUI.color = new Color(1, 1 ,1 , 0.5f);
				GUI.Label(new Rect(Screen.width * 0.13f, Screen.height * (height + 0.06f), Screen.width * 0.3f, Screen.height * 0),
				          "Pads Hit: " + padsHit + " | Top Speed: " + topSpeed[i] + " KM/H", shipStyle);
				height += 0.1f;
			}

			/* Draw title bar */
			GUI.color = new Color(0, 0, 0, 1);
			GUI.Box(new Rect(0,0, Screen.width * 0.8f, Screen.height * 0.05f), "", shipStyle);
			
			/* Draw title */
			GUI.color = new Color(1,1,1,1);
			shipHUDModuleStyle.customStyles[0].fontSize = Mathf.RoundToInt((Screen.width * 0.8f + Screen.height * 0.8f) * 0.014f);
			shipHUDModuleStyle.customStyles[0].alignment = TextAnchor.MiddleCenter;
			GUI.Label(new Rect(0,Screen.height * 0.03f, Screen.width * 0.8f, Screen.height * 0), "EVENT COMPLETE", shipStyle);

			/* Save screenshot */
			if (Input.GetKeyDown(KeyCode.S))
			{
				string documentsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments).ToString();
				if (!System.IO.Directory.Exists(documentsFolder + "/AGR2280/Screenshots/"))
				{
					System.IO.Directory.CreateDirectory(documentsFolder + "/AGR2280/Screenshots/");
				}
				string path = documentsFolder + "/AGR2280/Screenshots/";
				Application.CaptureScreenshot(path + "Screenshot" + screenshotNumber + "_" + System.DateTime.Today.Day.ToString() + System.DateTime.Today.Month.ToString() + System.DateTime.Today.Year.ToString() + ".png");
				screenshotNumber ++;
			}

			GUI.EndGroup();
		}
	}
}

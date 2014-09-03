using UnityEngine;
using System.Collections;

public class AGRShipController : AGRShipHUDModule {

	/* Error Reporting */
	private bool errorFound;
	private string[] errorMessage;
	private int currentErrorMessage;

	/* Triggers */
	private bool inWallJump;

	/* AI */
	private float lookRot;
	private float lookForce;

	/* Ghost */
	private int ghostCurrentPoint;
	private GameObject ghostObjects;
	private bool setNextPosition;
	private float ghostPlaybackTimer;

	/* Save time */
	string trackName;
	string timeFormat = ".AGRTime";
	string ghostFormat = ".AGRGhost";

	void Start () 
	{
		/* Attach Input Module*/
		gameObject.AddComponent<PlayerInputManager>();

		/* Run through all start functions */
		SetupPhysics();
		SetRequiredReferences();
		AudioSetArrayLimits();
		CreateEngine();
		CreateVisualObjects();

		/* Create Camera */
		GameObject clientCamera = GameObject.Find("_SCENECAM");
		clientCamera.name = "Client Camera";
		clientCamera.GetComponent<Camera>().farClipPlane =  40000;
		clientCamera.GetComponent<Camera>().hdr = true;

		/* Setup Effects and Graphics */
		clientCamera.AddComponent<BoostRadius>();
		clientCamera.GetComponent<BoostRadius>().BlurRadiusShader = shipCameraModuleBoostShader;
		clientCamera.GetComponent<BoostRadius>().BlurStrength = 0.3f;

		/* Set Camera for Camera Module */
		shipCameraModudleCameraObject = clientCamera;

		/* Set spawn respawn */
		shipRespawnManagerPosition = transform.position;
		shipRespawnManagerRotation = transform.eulerAngles;

		/* Set lap vars */
		passedMidGate = true;
		passedFinalGate = true;

		/* Get track name */
		if (GameObject.Find("NodeInformation"))
		{
			trackName = GameObject.Find("NodeInformation").GetComponent<TrackInformation>().TrackName;
		} else 
		{
			trackName = "NULLTRACK";
		}

		/* Search for saved time */
		string documentsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
		if (!System.IO.Directory.Exists(documentsFolder + "/AGR2280/Times/" + trackName))
		{
			System.IO.Directory.CreateDirectory(documentsFolder + "/AGR2280/Times/" + trackName);
		}
		
		string filePath = documentsFolder + "/AGR2280/Times/" + trackName + "/" + trackName + "_TimeTrial" + timeFormat;

		if (System.IO.File.Exists(filePath))
		{
			/* Load best times */
			System.IO.StreamReader newTime = new System.IO.StreamReader(filePath);
			bestLap_Miliseconds = int.Parse(newTime.ReadLine());
			bestLap_Seconds = int.Parse(newTime.ReadLine());
			bestLap_Minutes = int.Parse(newTime.ReadLine());
			newTime.Close();

			print("BEST TIME FOUND - LOADED");
		} else 
		{
			/* Set best times to 999 */
			bestLap_Miliseconds = 999;
			bestLap_Minutes = 999;
			bestLap_Seconds = 999;
		}

		/* Search for ghost file */
		filePath = documentsFolder + "/AGR2280/Times/" + trackName + "/" + trackName + "_BestLap" + ghostFormat;
		if (System.IO.File.Exists(filePath) && !shipAIControlling)
		{
			System.IO.StreamReader loadGhost = new System.IO.StreamReader(filePath);
			int amountOfPoints = int.Parse(loadGhost.ReadLine());

			/* Resize ghost arrays */
			System.Array.Resize(ref savedGhostPositions, amountOfPoints);
			System.Array.Resize(ref savedGhostRotations, amountOfPoints);

			ghostShipPrefabLocation = loadGhost.ReadLine();
			for (int i = 0; i < amountOfPoints; i++)
			{
				savedGhostPositions[i].x = float.Parse(loadGhost.ReadLine());
				savedGhostPositions[i].y = float.Parse(loadGhost.ReadLine());
				savedGhostPositions[i].z = float.Parse(loadGhost.ReadLine());
				savedGhostRotations[i].x = float.Parse(loadGhost.ReadLine());
				savedGhostRotations[i].y = float.Parse(loadGhost.ReadLine());
				savedGhostRotations[i].z = float.Parse(loadGhost.ReadLine());
				savedGhostRotations[i].w = float.Parse(loadGhost.ReadLine());
			}
			ghostWasLoaded = true;
			print("GHOST FOUND - LOADED");
		}
		
		/* Rest ghost array lengths */
		System.Array.Resize(ref ghostPosition, 0);
		System.Array.Resize(ref ghostRotation, 0);

		/* Resize lap array to fit current event */
		System.Array.Resize(ref Lap_Miliseconds, 5);
		System.Array.Resize(ref Lap_Minutes, 5);
		System.Array.Resize(ref Lap_Seconds, 5);
		System.Array.Resize(ref topSpeed, 5);
	}

	void FixedUpdate () 
	{
		if (!errorFound)
		{
			/* Run through all fixedupdate functions */
			GravityPass();
			AccelerationPass();
			TurningPass();
			if (shipAIControlling)
			{
				OrbitPass();
			} else 
			{
				CameraPass();
			}
			AudioManagerPass();
			CollisionPass();
		} else 
		{
			for (int i = 0; i < currentErrorMessage; i++)
			{
				Debug.LogError("ERROR: " + errorMessage[i]);
			}
		}

	}

	void Update()
	{
		if (!errorFound)
		{
			/* Run through all update functions */
			CameraModeChange();
			SideShiftInputPass();
			CameraFramePass();
			EnginePass();
			UpdateVisualObjects();
			RespawnPass();
			RacePass();
			GhostController();

			/* Update AI Controlling */
			GetComponent<PlayerInputManager>().AIControlling = shipAIControlling;
			if (shipAIControlling)
			{
				AIController();
			}
		}
	}

	void SetRequiredReferences()
	{
		/* Set ship container */
		if (!transform.Find("ShipContainer").gameObject)
		{
			System.Array.Resize(ref errorMessage, errorMessage.Length + 1);
			errorMessage[currentErrorMessage] = "Ship Container does not exist!";
			currentErrorMessage++;
			errorFound = true;
		} else
		{
			ShipGroupContainer = transform.Find("ShipContainer").gameObject;
		}
	}

	void OnCollisionEnter(Collision other)
	{
		if (other.collider.gameObject.layer != LayerMask.NameToLayer("Track_Floor"))
		{
			shipCollisionModuleColliding = true;
		} else 
		{
			shipCollisionModuleColliding = false;
		}

	}

	void OnCollisionExit(Collision other)
	{
		shipCollisionModuleColliding = false;
	}

	void OnCollisionStay(Collision other)
	{
		/* Disable Angular Velocity */
		rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
		rigidbody.angularVelocity = Vector3.zero;
		if (other.gameObject.layer == LayerMask.NameToLayer("Track_Wall"))
		{
			rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
		}

		/* Custom Collision */
		shipCollisionModuleContactCount = other.contacts.Length;
		System.Array.Resize(ref shipCollisionModuleCollisionVector, other.contacts.Length);
		System.Array.Resize(ref shipCollisionModuleContact, other.contacts.Length);
		System.Array.Resize(ref shipCollisionModuleSpeed, other.contacts.Length);
		for (int i = 0; i < shipCollisionModuleContactCount; i++)
		{
			shipCollisionModuleContact[i] = other.contacts[i];
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.collider.gameObject.tag == "Gate")
		{
			/* Get Node Number */
			int NodeNumber = other.collider.gameObject.GetComponent<NodeID>().NodeNumber;

			/* Set current gate */
			currentGate = NodeNumber;

			/* Get Track Information Objects */
			GameObject nodeInfo = GameObject.Find("NodeInformation");

			/* Update respawn positions */
			if (nodeInfo.GetComponent<TrackInformation>().TrackNodeIsRespawn[NodeNumber])
			{
				shipRespawnManagerPosition = other.collider.gameObject.transform.position;
				shipRespawnManagerRotation = new Vector3(0, other.collider.gameObject.transform.eulerAngles.y, 0);
			}

			/* Update AI targets */
			if (GameObject.Find(nodeInfo.GetComponent<TrackInformation>().TrackNodeObject[NodeNumber + 4].name))
			{
				nextAIPoint = nodeInfo.GetComponent<TrackInformation>().TrackNodeObject[NodeNumber + 4].transform.Find("AIHelper_" + (NodeNumber + 4)).transform.position;
				nextAPPoint = nodeInfo.GetComponent<TrackInformation>().TrackNodeObject[NodeNumber + 4].transform.Find("APHelper_" + (NodeNumber + 4)).transform.position;
			} else 
			{
				nextAIPoint = nodeInfo.GetComponent<TrackInformation>().TrackNodeObject[0].transform.Find("AIHelper_" + 0).transform.position;
				nextAPPoint = nodeInfo.GetComponent<TrackInformation>().TrackNodeObject[0].transform.Find("APHelper_" + 0).transform.position;
			}

			if (NodeNumber == Mathf.RoundToInt(nodeInfo.GetComponent<TrackInformation>().TrackNodeCount / 2))
			{
				passedMidGate = true;
			}

			if (NodeNumber == nodeInfo.GetComponent<TrackInformation>().TrackNodeCount - 5)
			{
				passedFinalGate = true;
			}

			/* Increase current lap */
			if (currentGate == 0 && passedMidGate && passedFinalGate && !updatedLap)
			{
				/* Lap time math */
				if (currentLap > 0)
				{
					if (currentLap <= 5)
					{
						Lap_Miliseconds[currentLap - 1] = currentLap_Miliseconds;
						Lap_Seconds[currentLap - 1] = currentLap_Seconds;
						Lap_Minutes[currentLap - 1] = currentLap_Minutes;
					}

					bool updateBest = false;
					bool minuteBeaten = false;
					bool secondBeaten = false;
					bool milisecondBeaten = false;

					/* Set whether the times have been beaten */
					if (currentLap_Minutes < bestLap_Minutes)
					{
						minuteBeaten = true;
					}

					if (currentLap_Seconds < bestLap_Seconds)
					{
						secondBeaten = true;
					}

					if (currentLap_Miliseconds < bestLap_Miliseconds)
					{
						milisecondBeaten = true;
					}

					if (minuteBeaten)
					{
						updateBest = true;
					} else
					{
						if (secondBeaten)
						{
							updateBest = true;
						} else 
						{
							if (milisecondBeaten)
							{
								updateBest = true;
							}
						}
					}

					if (updateBest)
					{
						bestLap_Miliseconds = currentLap_Miliseconds;
						bestLap_Seconds = currentLap_Seconds;
						bestLap_Minutes = currentLap_Minutes;

						/* Save ghost positions */
						System.Array.Resize(ref savedGhostPositions, ghostPosition.Length);
						System.Array.Resize(ref savedGhostRotations, ghostRotation.Length);

						for (int i = 0; i < savedGhostPositions.Length; i++)
						{
							savedGhostPositions[i] = ghostPosition[i];
							savedGhostRotations[i] = ghostRotation[i];
						}

						/* Create ghost */
						if (!GameObject.Find ("GhostShip"))
						{
							Object shipObject = Resources.Load(ghostShipPrefabLocation) as Object;
							GameObject newGhost = Instantiate(shipObject) as GameObject;
							newGhost.name = "GhostShip";
							newGhost.transform.position = savedGhostPositions[0];
							newGhost.transform.rotation = savedGhostRotations[0];
							ghostObjects = newGhost;
						}

						/* Save time to file */
						string documentsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
						if (!System.IO.Directory.Exists(documentsFolder + "/AGR2280/Times/" + trackName))
						{
							System.IO.Directory.CreateDirectory(documentsFolder + "/AGR2280/Times/" + trackName);
						}

						string filePath = documentsFolder + "/AGR2280/Times/" + trackName + "/" + trackName + "_TimeTrial" + timeFormat;

						System.IO.StreamWriter newTime = new System.IO.StreamWriter(filePath);
						newTime.WriteLine(bestLap_Miliseconds);
						newTime.WriteLine(bestLap_Seconds);
						newTime.WriteLine(bestLap_Minutes);
						newTime.Close();

						/* Write Ghost File */
						filePath = documentsFolder + "/AGR2280/Times/" + trackName + "/" + trackName + "_BestLap" + ghostFormat;

						System.IO.StreamWriter loadGhost = new System.IO.StreamWriter(filePath);
						loadGhost.WriteLine(savedGhostPositions.Length);
						loadGhost.WriteLine(ghostShipPrefabLocation);
						for (int i = 0; i < savedGhostPositions.Length; i++)
						{
							loadGhost.WriteLine(savedGhostPositions[i].x);
							loadGhost.WriteLine(savedGhostPositions[i].y);
							loadGhost.WriteLine(savedGhostPositions[i].z);
							loadGhost.WriteLine(savedGhostRotations[i].x);
							loadGhost.WriteLine(savedGhostRotations[i].y);
							loadGhost.WriteLine(savedGhostRotations[i].z);
							loadGhost.WriteLine(savedGhostRotations[i].w);
						}

						/* Rest ghost array lengths */
						System.Array.Resize(ref ghostPosition, 0);
						System.Array.Resize(ref ghostRotation, 0);
					}
				}

				/* Reset current lap */
				currentLap_Miliseconds = 0;
				currentLap_Minutes = 0;
				currentLap_Seconds = 0;
				
				/* Invoke new timer */
				CancelInvoke("RaceTimer");
				InvokeRepeating("RaceTimer", 0, 0.01f);

				/* If ghost was loaded */
				if (ghostWasLoaded)
				{
					/* Create ghost */
					if (!GameObject.Find ("GhostShip"))
					{
						Object shipObject = Resources.Load(ghostShipPrefabLocation) as Object;
						GameObject newGhost = Instantiate(shipObject) as GameObject;
						newGhost.name = "GhostShip";
						newGhost.transform.position = savedGhostPositions[0];
						newGhost.transform.rotation = savedGhostRotations[0];
						ghostObjects = newGhost;
					}
					ghostWasLoaded = false;
				}

				/* Play Ghost */
				currentGhostState = ghostState.InProgress;
				ghostCurrentPoint = 1;
				ghostPlaybackTimer = 1;

				currentLap++;
				passedMidGate = false;
				passedFinalGate = false;
				updatedLap = true;
			}

		}

		if (other.collider.gameObject.tag == "Respawn")
		{
			shipRespawnManagerTimer = 100;
		}
	}

	void OnTriggerStay(Collider other)
	{

	}

	void OnTriggerExit(Collider other)
	{
		inWallJump = false;
		updatedLap = false;
	}

	void OnDrawGizmos()
	{
		if (shipSettingsRenderDebugging)
		{
			DebugPass();
		}
	}
	
	void OnApplicationQuit()
	{
		DestroyEngine();
	}

	void AIController()
	{
		GetComponent<PlayerInputManager>().playerInputButtonThruster = true;

		Vector3 velocity = transform.InverseTransformDirection(rigidbody.velocity);
		rigidbody.AddRelativeForce(new Vector3(velocity.x * -rigidbody.mass * rigidbody.drag * 0.2f, 0, 0) * Time.deltaTime, ForceMode.Impulse);

		lookRot = Vector3.Angle(transform.forward, nextAIPoint - transform.position);
		Vector3 nextNodeLocal = transform.InverseTransformPoint(nextAIPoint);

		lookRot *= 0.01f;
		if (nextNodeLocal.x < 0)
		{
			lookRot *= -1;
		}
		lookRot += nextNodeLocal.x / 50;

		GetComponent<PlayerInputManager>().playerInputAxisSteering = Mathf.Clamp(lookRot, -1, 1);
		if (lookRot < -0.5f || lookRot > 0.5f)
		{
			GetComponent<PlayerInputManager>().playerInputAxisAirbrakes = -Mathf.Clamp(lookRot, -1, 1);
		} else 
		{
			GetComponent<PlayerInputManager>().playerInputAxisAirbrakes = 0;
		}

		Debug.DrawLine(transform.position, nextAIPoint, Color.red);

		float steeringForce = GetComponent<PlayerInputManager>().playerInputAxisSteering;
		if (steeringForce < 0)
		{
			steeringForce *= -1;
		}
		lookForce = Mathf.Lerp(lookForce, steeringForce * lookRot, Time.deltaTime * 3);
		transform.Rotate(Vector3.up * lookForce);

	}

	void RaceTimer()
	{
		currentLap_Miliseconds++;

		/* Ghost Positions */
		if (!shipAIControlling)
		{
			System.Array.Resize(ref ghostPosition, ghostPosition.Length + 1);
			System.Array.Resize(ref ghostRotation, ghostRotation.Length + 1);

			ghostPosition[ghostPosition.Length - 1] = transform.position;
			ghostRotation[ghostRotation.Length - 1] = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, ShipGroupContainer.transform.eulerAngles.z);
		}

		if (currentLap_Miliseconds > 99)
		{
			currentLap_Seconds++;
			currentLap_Miliseconds = 0;
		}

		if (currentLap_Seconds > 59)
		{
			currentLap_Minutes ++;
			currentLap_Seconds =0;
		}
	}

	void GhostController()
	{
		if (currentGhostState == ghostState.InProgress)
		{
			ghostPlaybackTimer += Time.deltaTime * 100;
			ghostCurrentPoint = Mathf.RoundToInt(ghostPlaybackTimer);
			if (ghostCurrentPoint < savedGhostPositions.Length)
			{
				ghostObjects.transform.position = Vector3.Lerp(ghostObjects.transform.position, savedGhostPositions[ghostCurrentPoint], Time.deltaTime * 7);
				ghostObjects.transform.rotation = Quaternion.Lerp(ghostObjects.transform.rotation, savedGhostRotations[ghostCurrentPoint], Time.deltaTime * 7);
			} else 
			{
				currentGhostState = ghostState.Ready;
			}

		}
	}
}

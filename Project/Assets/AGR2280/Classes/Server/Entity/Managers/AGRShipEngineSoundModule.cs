using UnityEngine;
using FMOD;
using System.Collections;

public class AGRShipEngineSoundModule : AGRShipCollisionModule {
	

	/* Engine Properties */
	private float engineFreq;
	private GameObject engineObject;
	private int gear;
	private float gearTimer;

	/* Volume Control */
	private float engineFinalAudio;
	private float engineTurnReduction;
	private float engineRingMod;
	private float engineRingModTime;
	private float engineRingModAmount;
	private float engineTurnOff;

	public void CreateEngine() 
	{
		engineObject = new GameObject("Ship Engine");
		engineObject.AddComponent<AudioSource>();
		engineObject.GetComponent<AudioSource>().clip = shipAudioModuleEngineAudio;
		engineObject.GetComponent<AudioSource>().loop = true;
		engineObject.GetComponent<AudioSource>().Play();
		engineObject.GetComponent<AudioSource>().dopplerLevel = 0;
		engineObject.GetComponent<AudioSource>().maxDistance = 200;
		engineObject.GetComponent<AudioSource>().minDistance = 100;
		engineObject.AddComponent<AudioChorusFilter>();
		engineObject.AddComponent<AudioReverbFilter>();
		engineObject.GetComponent<AudioReverbFilter>().reverbPreset = AudioReverbPreset.Drugged;
		engineObject.transform.parent = transform;
		engineObject.transform.localPosition = Vector3.zero;
	}

	public void DestroyEngine()
	{

	}

	public void EnginePass()
	{
		if (currentEngineSound == EngineType.F1Agility || currentEngineSound == EngineType.F1Fighter || currentEngineSound == EngineType.F1Speed)
		{
			float shipTurn = shipCameraModuleShipRotation;
			if (shipTurn < 0)
			{
				shipTurn *= -1;
			}
			if (shipTurn < shipSettingsTurnMaxAngle)
			{
				engineFreq = Mathf.Lerp(engineFreq, shipVelocityModuleVelocity / (shipSettingsEngineMaxThrust * 0.8f - ((gear / 10) * 2)), Time.deltaTime);
				shipTurn = 0;
			} else 
			{
				gear = 0;
				engineFreq = Mathf.Lerp(engineFreq, 0, Time.deltaTime);
			}


			if (GetComponent<PlayerInputManager>().playerInputButtonThruster)
			{
				gearTimer += Time.deltaTime;
				if (gearTimer > 3 && gear < 4)
				{
					shipAudioModuleGearSwitchObject.GetComponent<AudioSource>().Play();
					engineFreq -= 25;
					engineTurnReduction = 0.6f;
					gear++;
					gearTimer = 0;
				}
			} else
			{
				gear = 0;
			}

			if (gear == 1 || gear == 3)
			{
				engineRingModAmount = 10;
				engineRingModTime += Time.deltaTime * 1200;
				engineRingMod = Mathf.Sin(engineRingModTime);
				engineRingMod = Mathf.Clamp(engineRingMod, -1, 0);
			} else
			{
				if (gear == 0 || gear == 2)
				{
					engineRingModAmount = 10;
					engineRingModTime += Time.deltaTime * 1200;
					engineRingMod = Mathf.Sin(engineRingModTime);
					engineRingMod = Mathf.Clamp(engineRingMod, -0.3f, 0);
				} else 
				{
					engineRingMod = 0;
				}
			}
			if (shipHoverModuleCurrentState == shipHoverModuleShipStates.InFlight)
			{
				engineTurnReduction = Mathf.Lerp(engineTurnReduction, 0, Time.deltaTime);
			} else 
			{
				engineTurnReduction = Mathf.Lerp(engineTurnReduction, 1 - Mathf.Clamp(shipTurn, 0, 0.2f), Time.deltaTime * 10);
			}

			if (shipHoverModuleHitSpeedRegistered)
			{
				gear = 0;
				engineTurnReduction = 0;
			}

			/* Engine turnoff */
			if (engineFreq < 0.505f)
			{
				engineTurnOff = Mathf.Lerp(engineTurnOff, 1, Time.deltaTime);
			} else
			{
				engineTurnOff = 0;
			}

			engineFinalAudio = Mathf.Clamp((engineTurnReduction + engineRingMod) - engineTurnOff, 0, 1);
			engineObject.GetComponent<AudioSource>().volume = engineFinalAudio;


			engineFreq = Mathf.Clamp(engineFreq, 0.5f, 3);
			engineObject.GetComponent<AudioSource>().pitch = engineFreq;

			/* Engine Release Sound */
			if (Input.GetButtonUp("[KB] Thruster") || Input.GetButtonUp("[PAD] Thruster") && engineFreq > 0.7f)
			{
				shipAudioModuleEngineReleaseObject.GetComponent<AudioSource>().Play();
			} else if(Input.GetButtonDown("[KB] Thruster") || Input.GetButtonDown("[PAD] Thruster") && engineFreq < 0.6f)
			{
				shipAudioModuleEngineStartObject.GetComponent<AudioSource>().Play();
			}
		} else 
		{
			/* Set Engine Frequency */
			engineFreq = Mathf.Lerp(engineFreq, shipVelocityModuleVelocity / (shipSettingsEngineMaxThrust * 0.8f), Time.deltaTime);

			/* Engine turnoff */
			if (engineFreq < 0.505f)
			{
				engineTurnOff = Mathf.Lerp(engineTurnOff, 1, Time.deltaTime);
			} else
			{
				engineTurnOff = 0;
			}

			engineFreq = Mathf.Clamp(engineFreq, 0.5f, 1);
			engineObject.GetComponent<AudioSource>().pitch = engineFreq;
			engineObject.GetComponent<AudioSource>().volume = 0.5f;
		}
	}



}

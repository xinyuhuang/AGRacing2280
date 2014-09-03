using UnityEngine;
using System.Collections;

public class AGRShipSoundModule : AGRShipDebugVisuals {

	/* Boost Audio */
	private int boostAudioLimit = 2;
	private int boostAudioCurrentIndex;
	
	private float flameLength;
	public void AudioSetArrayLimits()
	{
		/* Boost Audio */
		if (shipAudioModuleSingleBoost)
		{
			System.Array.Resize(ref shipAudioModuleBoostObjects, 1);
			boostAudioLimit = 0;
		} else 
		{
			System.Array.Resize(ref shipAudioModuleBoostObjects, 3);
		}

		/* Turbo Audio */
		GameObject turboAudio = new GameObject("TurboAudio");
		turboAudio.AddComponent<AudioSource>();
		turboAudio.GetComponent<AudioSource>().clip = shipAudioModuleTurboAudio;
#if UNITY_PRO_LICENSE
		turboAudio.AddComponent<AudioReverbFilter>();
		turboAudio.GetComponent<AudioReverbFilter>().reverbPreset = AudioReverbPreset.Drugged;
#endif
		shipAudioModuleTurboObject = turboAudio;

		turboAudio.transform.parent = transform;
		turboAudio.transform.localPosition = Vector3.zero;

		/* BR Audio */
		GameObject BRAudio = new GameObject("BRAudio");
		BRAudio.AddComponent<AudioSource>();
		BRAudio.GetComponent<AudioSource>().clip = shipAudioModuleBRAudio;
		shipAudioModuleBRObject = BRAudio;
		
		BRAudio.transform.parent = transform;
		BRAudio.transform.localPosition = Vector3.zero;

		/* Gear Switch */
		GameObject gearSwitch = new GameObject("GearSwitch");
		gearSwitch.AddComponent<AudioSource>();
		gearSwitch.GetComponent<AudioSource>().clip = shipAudioModuleGearSwitch;
		gearSwitch.transform.parent = transform;
		gearSwitch.transform.localPosition = Vector3.zero;
		shipAudioModuleGearSwitchObject = gearSwitch;

		/* Engine Backing */
		GameObject engineBacking = new GameObject("Engine Backing");
		engineBacking.AddComponent<AudioSource>();
		engineBacking.GetComponent<AudioSource>().clip = shipAudioModuleEngineBacking;
		engineBacking.transform.parent = transform;
		engineBacking.transform.localPosition = Vector3.zero;
		engineBacking.GetComponent<AudioSource>().loop = true;
		engineBacking.GetComponent<AudioSource>().Play();
		engineBacking.GetComponent<AudioSource>().volume = 0.3f;

		/* Engine Release */
		GameObject engineRelease = new GameObject("Engine Release");
		engineRelease.AddComponent<AudioSource>();
		engineRelease.GetComponent<AudioSource>().clip = shipAudioModuleEngineRelease;
		engineRelease.transform.parent = transform;
		engineRelease.transform.localPosition = Vector3.zero;
		shipAudioModuleEngineReleaseObject = engineRelease;

		/* Engine Startup */
		GameObject engineStart = new GameObject("Engine Start");
		engineStart.AddComponent<AudioSource>();
		engineStart.GetComponent<AudioSource>().clip = shipAudioModuleEngineStart;
		engineStart.transform.parent = transform;
		engineStart.transform.localPosition = Vector3.zero;
		shipAudioModuleEngineStartObject = engineStart;

		/* Left Airbrake */
		GameObject leftAB = new GameObject("Left Airbrake");
		leftAB.AddComponent<AudioSource>();
		leftAB.GetComponent<AudioSource>().clip = shipAudioModuleAirBrake;
		leftAB.GetComponent<AudioSource>().pan = -1;
		leftAB.GetComponent<AudioSource>().volume = 0.4f;
		leftAB.transform.parent = transform;
		leftAB.transform.localPosition = Vector3.zero;
		shipAudioModuleABleft = leftAB;

		/* Right Airbrake */
		GameObject rightAB = new GameObject("Right Airbrake");
		rightAB.AddComponent<AudioSource>();
		rightAB.GetComponent<AudioSource>().clip = shipAudioModuleAirBrake;
		rightAB.GetComponent<AudioSource>().pan = 1;
		rightAB.GetComponent<AudioSource>().volume = 0.4f;
		rightAB.transform.parent = transform;
		rightAB.transform.localPosition = Vector3.zero;
		shipAudioModuleABRight = rightAB;


	}

	public void AudioManagerPass()
	{
		/* Boost Audio */
		if (shipHoverModuleHitSpeed)
		{
			if (shipAudioModuleBoostObjects[boostAudioCurrentIndex] != null)
			{
				Destroy(shipAudioModuleBoostObjects[boostAudioCurrentIndex]);
			}

			GameObject newBoostAudio = new GameObject("Boost Audio");
			newBoostAudio.transform.position = transform.position;
			newBoostAudio.AddComponent<AudioSource>();
			newBoostAudio.GetComponent<AudioSource>().clip = shipAudioModuleBoostAudio;
			newBoostAudio.GetComponent<AudioSource>().Play();
			newBoostAudio.transform.parent = transform;
#if UNITY_PRO_LICENSE
			newBoostAudio.AddComponent<AudioReverbFilter>();
			newBoostAudio.GetComponent<AudioReverbFilter>().reverbPreset = AudioReverbPreset.Dizzy;
#endif
			shipAudioModuleBoostObjects[boostAudioCurrentIndex] = newBoostAudio;

			boostAudioCurrentIndex++;
			if (boostAudioCurrentIndex > boostAudioLimit)
			{
				boostAudioCurrentIndex = 0;
			}
		}

		/* Airbrakes */
		if (GetComponent<PlayerInputManager>().playerInputAxisAirbrakes < 0)
		{
			if (!shipAudioModuleABRight.GetComponent<AudioSource>().isPlaying)
			{
				shipAudioModuleABRight.GetComponent<AudioSource>().Play();
			}
		} else 
		{
			shipAudioModuleABRight.GetComponent<AudioSource>().Stop();
		}

		if (GetComponent<PlayerInputManager>().playerInputAxisAirbrakes > 0)
		{
			if (!shipAudioModuleABleft.GetComponent<AudioSource>().isPlaying)
			{
				shipAudioModuleABleft.GetComponent<AudioSource>().Play();
			}
		} else 
		{
			shipAudioModuleABleft.GetComponent<AudioSource>().Stop();
		}
	}
}

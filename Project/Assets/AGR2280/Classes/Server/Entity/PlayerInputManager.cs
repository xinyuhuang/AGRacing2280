using UnityEngine;
using System.Collections;

public class PlayerInputManager : MonoBehaviour {

	/*
	 * The player input manager is where all of the input for a player comes from.
	 */

	/* Stores if the player is holding down the thruster */
	public bool playerInputButtonThruster;

	/* Stores if the player has changed the camer view */
	public bool playerInputButtonCamera;

	/* Stores if the player is looking behind */
	public bool playerInputButtonReverseCam;

	/* Stores the input for turning */
	public float playerInputAxisSteering;
	
	/* Stores the input for pitching */
	public float playerInputAxisPitching;

	/* Stores the input for airbrakes */
	public float playerInputAxisAirbrakes;

	/* Stores if the player is braking */
	public bool playerInputActionBraking;

	/* Stores if the player is airbraking */
	public bool playerInputActionAirbraking;

	/* Stores if the player is sideshifting */
	public bool playerInputActionSideShiftLeft;
	public bool playerInputActionSideShiftRight;
	public bool playerInputButtonSSLeftPressed;
	public bool playerInputButtonSSRightPressed;

	/* AI */
	public bool AIControlling;
	
	void Update ()
	{
		if (!AIControlling)
		{
			/* Thruster Input */
			playerInputButtonThruster  = false;
			if (Input.GetButton("[KB] Thruster") || Input.GetButton("[PAD] Thruster")) { playerInputButtonThruster = true; }

			/* Set steering input */
			playerInputAxisSteering = Input.GetAxis("[KB] Steer");
			if (playerInputAxisSteering == 0) { playerInputAxisSteering = Input.GetAxis("[PAD] Steer"); }

			/* Set pitching input */
			playerInputAxisPitching = Input.GetAxis("[KB] Pitch");
			if (playerInputAxisPitching == 0) { playerInputAxisPitching = Input.GetAxis("[PAD] Pitch"); }

			/* Set airbrake input */
			playerInputAxisAirbrakes = Input.GetAxis("[KB] Airbrake");
			if (playerInputAxisAirbrakes == 0) { playerInputAxisAirbrakes = Input.GetAxis("[PAD] Airbrake"); }
			if (playerInputAxisAirbrakes == 0) { playerInputAxisAirbrakes = Input.GetAxis("[PAD] Analog Airbrake"); }

			/* Airbraking Logic */
			if (playerInputAxisAirbrakes != 0)
			{
				playerInputActionAirbraking = true;
			} else
			{
				playerInputActionAirbraking = false;
			}

			/* Detect if the player is braking */
			playerInputActionBraking = false;
			if (Input.GetButton("[KB] Brake Left") && Input.GetButton("[KB] Brake Right"))
			{
				playerInputActionBraking = true;
				playerInputAxisAirbrakes = 0;
			}
			
			if (Input.GetButton("[PAD] Brake Left") && Input.GetButton("[PAD] Brake Right"))
			{
				playerInputActionBraking = true;
				playerInputAxisAirbrakes = 0;
			}

			/* Camera Change */
			playerInputButtonCamera = false;
			if (Input.GetButtonDown("[KB] Camera") || Input.GetButtonDown("[PAD] Camera")) { playerInputButtonCamera = true; }

			/* Reverse Cam */
			playerInputButtonReverseCam = false;
			if (Input.GetButton("[KB] LookBehind") || Input.GetButton("[PAD] LookBehind")) { playerInputButtonReverseCam = true; }

			// SideShift Input
			playerInputActionSideShiftLeft = false;
			playerInputActionSideShiftRight = false;
			if ((playerInputAxisAirbrakes > 0 ) && !playerInputButtonSSRightPressed ) { playerInputActionSideShiftRight = true; playerInputButtonSSRightPressed = true; playerInputActionSideShiftLeft = false; }
			if ((playerInputAxisAirbrakes < 0) && !playerInputButtonSSLeftPressed ) { playerInputActionSideShiftLeft = true; playerInputButtonSSLeftPressed = true; playerInputActionSideShiftRight = false; }
			
			if (playerInputAxisAirbrakes == 0) { playerInputButtonSSLeftPressed  = false; playerInputButtonSSRightPressed  = false; }
		}
	}
}

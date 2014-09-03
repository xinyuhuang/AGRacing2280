using UnityEngine;
using System.Collections;

public class AGRShipSettingsContainer : MonoBehaviour {

	/* 
	 * This class contains all of the settings for a ship 
	 */

	/* Debugging */
	public bool shipSettingsRenderDebugging;

	/* Menu Stats */
	public int shipSettingsMenuStatSpeed;
	public int shipSettingsMenuStatThrust;
	public int shipSettingsMenuStatHandling;
	public int shipSettingsMenuStatShield;

	/* Camera Settings */
	public float shipSettingsCameraInternalFOV;
	public float shipSettingsCameraInternalOffsetY;
	public float shipSettingsCameraInternalOffsetZ;

	public float shipSettingsCameraBackwardFOV;
	public float shipSettingsCameraBackwardOffsetY;
	public float shipSettingsCameraBackwardOffsetZ;

	public float shipSettingsCameraFarFOV;
	public float shipSettingsCameraFarOffsetY;
	public float shipSettingsCameraFarOffsetZ;
	public float shipSetingsCameraFarSpring;

	public float shipSettingsCameraCloseFOV;
	public float shipSettingsCameraCloseOffsetY;
	public float shipSettingsCameraCloseOffsetZ;
	public float shipSetingsCameraCloseSpring;

	/* Animated Airbrake Models */
	public float shipSettingsAirbrakeAnimationMaxAngle;
	public float shipSettingsAirbrakeAnimationDownSpeed;
	public float shipSettingsAirbrakeAnimationUpSpeed;

	/* Physical Properties */
	public float shipSettingsPhysicalHeight;
	public float shipSettingsPhysicalLength;
	public float shipSettingsPhysicalWidth;

	/* Engine Variables */
	public float shipSettingsEngineMaxAccel_D;
	public float shipSettingsEngineMaxAccel_C;
	public float shipSettingsEngineMaxAccel_B;
	public float shipSettingsEngineMaxAccel_A;
	public float shipSettingsEngineMaxAccel_AP;
	public float shipSettingsEngineMaxAccel;
	public float shipSettingsEngineAccelSpeed;
	public float shipSettingsEngineAccelFalloff;

	public float shipSettingsEngineMaxThrust_D;
	public float shipSettingsEngineMaxThrust_C;
	public float shipSettingsEngineMaxThrust_B;
	public float shipSettingsEngineMaxThrust_A;
	public float shipSettingsEngineMaxThrust_AP;
	public float shipSettingsEngineMaxThrust;
	public float shipSettingsEngineTurboAmount;

	/* Brake Variables */
	public float shipSettingsBrakesSlowdown;
	public float shipSettingsBrakesSlowdownGain;
	public float shipSettingsBrakesSlowdownFalloff;

	/* Turn Variables */
	public float shipSettingsTurnMaxAngle;
	public float shipSettingsTurnGain;
	public float shipSettingsTurnFalloff;

	/* Airbrake Variables */
	public float shipSettingsAirbrakeAmount;
	public float shipSettingsAirbrakeDrag;
	public float shipSettingsAirbrakeFalloff;
	public float shipSettingsAirbrakeGain;
	public float shipSettingsAirbarkeTurn;
	public float shipSettingsAirbrakeGrip;
	public float shipSettingsAirbrakeSideShiftForce;

	/* Anti-Gravity Variables */

	public float shipSettingsAntiGravGroundGrip_D;
	public float shipSettingsAntiGravGroundGrip_C;
	public float shipSettingsAntiGravGroundGrip_B;
	public float shipSettingsAntiGravGroundGrip_A;
	public float shipSettingsAntiGravGroundGrip_AP;
	public float shipSettingsAntiGravGroundGrip;

	public float shipSettingsAntiGravAirGrip_D;
	public float shipSettingsAntiGravAirGrip_C;
	public float shipSettingsAntiGravAirGrip_B;
	public float shipSettingsAntiGravAirGrip_A;
	public float shipSettingsAntiGravAirGrip_AP;
	public float shipSettingsAntiGravAirGrip;


	public float shipSettingsAntiGravLandingRebound;
	public float shipSettingsAntiGravRebound;
	public float shipSettingsAntiGravJumpTime;
	public float shipSettingsAntiGravRideHeight;

	/* Anti-Gravity Variables */
	public float shipSettingsFlightGravity_LowerClass;
	public float shipSettingsFlightGravity_HigherClass;
	public float shipSettingsFlightGravity;
	public float shipSettingsTrackGravity_LowerClass;
	public float shipSettingsTrackGravity_HigherClass;
	public float shipSettingsTrackGravity;
	public float shipSettingsNormalGravity;

	/* Pitch Variables */
	public float shipSettingsPitchAir;
	public float shipSettingsPitchGround;
	public float shipSettingsPitchDamping;
	public float shipSettingsPitchAntiGravHeightAdjust;
	
	/* Needed Gameobjes */
	public GameObject ShipGroupContainer;

	#region TurnModule
		/* Torque */
		public float shipTurnModuleTurningVelocity;
		public float shipTurnModuleSteerGainVelocity;
		public float shipTurnModuleSteerFalloffVelocity;

		/* Airbakes */
		public float shipTurnModuleAirbrakeVelocity;
		public float shipTurnModuleAirbrakeGainVelocity;
		public float shipTurnModuleAirbrakeFalloffVelocity;
		public float shipTurnModuleAirbrakeSwingForce;
		public float shipTurnModuleAirbrakeSwingGround;
		public float shipTurnModuleAirbrakeSwingAir;
		
		/* Banking */
		public float shipTurnModuleBankAngle;
		public float shipTurnModuleBankMaxAngle = 40;
		public float shipTurnModuleBankAirbrakeAngle;
		public  float shipTurnModuleBankFullAngle;
		
		public float shipTurnModuleBankVelocity;
		public float shipTurnModuleBankGainVelocity;
		public float shipTurnModuleBankFalloffVelocity;
		
		public float shipTurnModuleBankOutInternalSpeed = 3;
		public float shipTurnModuleBankOutExternalSpeed = 5;
		
		public float shipTurnModuleBankInInternalSpeed = 4.5f;
		public float shipTurnModuleBankInExternalSpeed = 2.5f;

	#endregion

	#region VelocityModule
		/* Acceleration Variables */
		public float shipVelocityModuleVelocity;
		public float shipVelocityModuleAcceleration;
		
		/* Brake Variables */
		public float shipVelocityModuleBrakeAmount;
		
		public float shipVelocityModuleVelocityLoss;
		
		/* Boost Timing */
		public float shipVelocityModuleBoostTimer;
		public float shipVelocityModuleBoostAmount;

		/* Side Shift */
		public float shipVelocityModuleSideShiftCooler;
		public float shipVelocityModuleSideShiftLeft;
		public int lastSideShiftInput;
		public float shipVelocityModuleSideShiftRight;
		public int shipVelocityModuleSideShiftTapAmount;
		public float shipVelocityModuleSideShiftSuccessCooler;

		public float shipVelocityModuleSideShiftTimer;

		/* Air Resistance */
		public float shipVelocityModuleAirbrakeResistanceDamper;
		public float shipVelocityModuleAirbrakeResistance;
		public float shipVelocityModuleAirbrakeDeacceleration;
		public float shipVelocityModuleResistanceAmount;
		public float shipVelocityModuleResistanceGround;
		public float shipVelocityModuleResistanceAir;
		
	#endregion

	#region HoverModule
		/* Hover States */
		public bool shipHoverModuleFrontHovering;
		public bool shipHoverModuleBackHovering;
		
		public enum shipHoverModuleShipStates { FullyGrounded, PartiallyGrounded, InFlight }
		public shipHoverModuleShipStates shipHoverModuleCurrentState;
		
		/* Hover Settings */
		public float shipHoverModuleRebound;
		public float shipHoverModuleHoverForce;
		public float shiphoverModuleHoverDamping;
		public float shipHoverModuleHoverCompressionForce;
		public bool shipHoverModuleFallCapped;

		public float shipHoverModuleWobbleAmount;
		public float shipHoverModuleWobbleSpeed;
		public float shipHoverModuleWobbleTime;
		public float shipHoverModuleCurrentWobble;
		
		/* Ship Rotation */
		public Vector3 shipHoverModuleTargetTrackNormal;
		public float shipHoverModuleRotationVelocity;
		public float shipHoverModulePitchRotation;
		
		/* Raycast Settings */
		public RaycastHit shipHoverModulePadInfo;
		public float shipHoverModuleBackDistance;
		public float shipHoverModuleFrontDistance;
		
		/* Pad Hits */
		public bool shipHoverModuleHitSpeed;
		public bool shipHoverModuleHitSpeedRegistered;
		public bool shipHoverModuleHitWeapon;
		
		public Vector3 shipHoverModuleSpeedPadDirection;

		/* Barrel Roll */
		public float shipHoverModuleBarrelRollProgress;
		public float shipHoverModuleBarrelRollActual;
		public float shipHoverModuleBarrelRollTimer;
		public int shiphoverModuleBarrelRollState;
		public float shipHoverModuleBarrelRollLastValue;
		public bool shipHoverModuleBarrelRollSuccess;
		public bool shipHoverModuleBarrelRollHasRolled;
		public bool shipHoverModuleCanBarrelRoll;

	#endregion

	#region GravityModule
		/* Gravity Settings */
		public float shipGravityModuleGravity;
		public Vector3 shipGravityModuleDirection = Vector3.down;
		public float shipGravityModuleAirTime;

		public float shipGravityModuleGravityMultiplier_D;
		public float shipGravityModuleGravityMultiplier_C;
		public float shipGravityModuleGravityMultiplier_B;
		public float shipGravityModuleGravityMultiplier_A;
		public float shipGravityModuleGravityMultiplier_AP;
		public float shipGravityModuleGravityMultiplier;

		public float shipGravityModuleGravityDistance;
	#endregion

	#region CameraModule
		/* Camera States */
		public enum shipCameraModuleCameraStates {Internal, Backward, Close, Far, Orbit, Destroyed}
		public shipCameraModuleCameraStates shipCameraModuleCurrentCameraState;

		/* Spring Arm Settings */
		public float shipCameraModuleSpringArmLookAtLength = 25;
		public float shipCameraModuleSpringArmLookAtHeight = 4;
		public float shipCameraModuleSpringArmDistanceLag = 3;
		public float shipCameraModuleSpringArmHeightLag;
		public float shipCameraModuleSpringArmInterpolatedLength;
		public float shipCameraModuleSpringCollisionOffset;

		/* Other settings */
		public float shipCameraModuleAcclerateLength;
		public float shipCameraModuleBoostDistance;

		public float shipCameraModuleCameraFOV;

		public GameObject shipCameraModudleCameraObject;
		public GameObject shipCameraModuleCockpitLocator;
		public GameObject shipCameraModuleCockpitObject;

		public Vector3 shipCameraModuleCameraLocation;

		public float shipCameraModuleShipRotation;
		public int shipCameraModuleCameraFollowType;

		/* Post Processing */
		public Shader shipCameraModuleBoostShader;
		public float shipCameraModuleBoostStrength;


	#endregion

	#region AudioModule

		/* GameObject Manager */
		public GameObject[] shipAudioModuleBoostObjects;
		public GameObject shipAudioModuleEngineObject;
		public GameObject shipAudioModuleTurboObject;
		public GameObject shipAudioModuleBRObject;
		public GameObject shipAudioModuleABleft;
		public GameObject shipAudioModuleABRight;

		/* Audio Clips */
		public AudioClip shipAudioModuleBoostAudio;
		public AudioClip shipAudioModuleEngineAudio;
		public AudioClip shipAudioModuleTurboAudio;
		public AudioClip shipAudioModuleBRAudio;
		public AudioClip shipAudioModuleAirBrake;
		public bool shipAudioModuleSingleBoost;

	 	/* Engine Flames */
		public GameObject[] shipAudioModuleEngineFlame;

		/* Engine Type */
		public enum EngineType { F1Fighter, F1Speed, F1Agility, SlipstreamFigther, SlipstreamSpeed, SlipstreamAgility};
		public EngineType currentEngineSound;

		/* Engine Sounds */
		public AudioClip shipAudioModuleGearSwitch;
		public GameObject shipAudioModuleGearSwitchObject;

		public AudioClip shipAudioModuleEngineBacking;

		public AudioClip shipAudioModuleEngineRelease;
		public GameObject shipAudioModuleEngineReleaseObject;

		public AudioClip shipAudioModuleEngineStart;
		public GameObject shipAudioModuleEngineStartObject;
	#endregion

	#region CollisionModule

		/* Basic Checks */
		public bool shipCollisionModuleColliding;
		public enum shipCollisionModuleSide { Left, Right, Front, Back, Up, Down };
		public shipCollisionModuleSide shipCollisionModuleCollisionSide;
		public Vector3[] shipCollisionModuleCollisionVector;

		public float shipCollisionModuleContactCount;
		public ContactPoint[] shipCollisionModuleContact;
		public Vector3[] shipCollisionModuleSpeed;


	#endregion

	#region VisualsManager
		/* Wind Lines */
		public GameObject shipVisualManagerWindLineLeft;
		public GameObject shipVisualManagerWindLineRight;
		public Vector3 shipVisualManagerWindLineOffset;
		public float shipVisualManagerWindLineOpacity;
		public Material shipVisualManagerWindLineMaterial;

		/* Control Stick */
		public GameObject shipVisualManagerCockPitControlStick;

		/* Slip stream */
		public GameObject shipVisualsManagerLensflare;
		public GameObject shipVisualsManagerSlipstream;
		public GameObject shipVisualsManagerEngineLight;

		/* Air brakes */
		public GameObject shipVisualsManagerAirbrakeLeft;
		public GameObject shipVisualsManagerAirbrakeRight;
		public float shipVisualsManagerAirbrakeUpSpeed;
		public float shipVisualsManagerAirbrakeDownSpeed;
		public float shipVisualsManagerAirbrakeAmount;

	#endregion

	#region RespawnManager
		/* Transform */
		public Vector3 shipRespawnManagerPosition;
		public Vector3 shipRespawnManagerRotation;

		public float shipRespawnManagerTimer;


	#endregion

	#region AI
		/* Objects */
		public Vector3 nextAIPoint;
		public Vector3 nextAPPoint;

		public bool shipAIControlling;

	#endregion

	#region RaceManager
		/* Ghost Time */
		public Vector3[] ghostPosition;
		public Quaternion[] ghostRotation;

		public Vector3[] savedGhostPositions;
		public Quaternion[] savedGhostRotations;

		public string ghostShipPrefabLocation;

		public bool ghostWasLoaded;

		/* Ghost Play Settings */
		public enum ghostState { Ready, InProgress}
		public ghostState currentGhostState;

		/* Race Position */
		public int currentLap_Miliseconds;
		public int currentLap_Seconds;
		public int currentLap_Minutes;
		public int bestLap_Miliseconds;
		public int bestLap_Seconds;
		public int bestLap_Minutes;

		public int[] Lap_Miliseconds;
		public int[] Lap_Seconds;
		public int[] Lap_Minutes;

		public int currentPosition;
		public int currentLap;
		public int currentGate;

		public bool updatedLap;
		public bool passedMidGate;
		public bool passedFinalGate;

		public bool finishedRace;

		public int[] topSpeed;

	#endregion

	#region HUDModule

		/* Style */
		public GUISkin shipHUDModuleStyle;
		public string shipHUDShipName;
		public Color shipHUDShipColor;
	#endregion
}

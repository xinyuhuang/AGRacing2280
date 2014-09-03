using UnityEngine;
using System.Collections;

public class ServerSettings : MonoBehaviour {

	/* Enums */
	public enum ServerEnumSpeedClasses 
	{ 
		D, 
		C, 
		B, 
		A, 
		AP
	};

	public enum ServerEnumGameMode 
	{ 
		SingleRace, 
		Combat, 
		ScoreRace, 
		ZoneRace,
		BoostRace, 
		VelocityRace, 
		Zone, 
		BoostZone, 
		ProceduralZone, 
		TimeTrial, 
		BoostTrial, 
		SpeedLap, 
		Endurance
	};

	/* Race Settings */
	public static float ServerVarSpeedPadForce = 120;
	public static float ServerVarWeaponPadRefresh = 0.75f;

	/* Server Settings */
	public static bool ServerVarIsSingle = true;
	public static bool ServerVarLANOnly = true;

	public static int ServerVarMaxPlayers = 8;
	public static bool ServerVarAIEnabled = true;

	public static int ServerVarNumberOfLaps = 99;

	public static bool ServerVarWeaponsEnabled = true;
	public static bool ServerVarBREnabled = true;
	public static bool ServerVarSpeedPadsEnabled = true;

	public static ServerEnumSpeedClasses ServerVarSpeedClass;

	public static string ServerVarTypeName = "Single Race";
	public static string ServerVarServerName = "AGR2280 Listen Server";
}

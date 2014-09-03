using UnityEngine;
using System.Collections;

public class AGRShipRaceManager : AGRShipVisualsManager {


	public void RacePass()
	{
		/* Pass ship to AI if player has finished */
		if (currentLap > 5)
		{
			finishedRace = true;
			shipAIControlling = true;
		}
	}
}

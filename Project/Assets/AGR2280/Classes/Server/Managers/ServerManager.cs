using UnityEngine;
using System.Collections;

public class ServerManager : MonoBehaviour {
	
	/* Call this from somewhere to start the server! */
	public void StartServer()
	{
		Network.InitializeServer(ServerSettings.ServerVarMaxPlayers, 25565, !Network.HavePublicAddress());
		MasterServer.RegisterHost(ServerSettings.ServerVarTypeName, ServerSettings.ServerVarServerName);

		/* Make the Master Server the host machine */
		MasterServer.ipAddress = "127.0.0.1";
	}

	void OnServerInitialized()
	{
		Debug.Log("Created Server " + ServerSettings.ServerVarServerName);
	}
}

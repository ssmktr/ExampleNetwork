using UnityEngine;
using System.Collections;

public class ExampleUnityNetworkingConnectToMasterServer : MonoBehaviour
{
	// Assuming Master Server and Facilitator are on the same machine
	public string MasterServerIP = "127.0.0.1";

	void Awake()
	{
		// set the IP and port of the Master Server to connect to
		MasterServer.ipAddress = MasterServerIP;
		MasterServer.port = 23466;

		// set the IP and port of the Facilitator to connect to
		Network.natFacilitatorIP = MasterServerIP;
		Network.natFacilitatorPort = 50005;
	}
}
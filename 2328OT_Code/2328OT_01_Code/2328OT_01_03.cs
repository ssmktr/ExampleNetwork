using UnityEngine;
using System.Collections;

public class ExampleUnityNetworkInitializeServer : MonoBehaviour
{
	void OnGUI()
	{
		if( GUILayout.Button( "Launch Server" ) )
		{
			LaunchServer();
		}
	}
	
	// launch the server
	void LaunchServer()
	{
		// Start a server that enables NAT punchthrough,
		// listens on port 25000,
		// and allows 8 clients to connect
		Network.InitializeServer( 8, 25005, true );
	}

	// called when the server has been initialized
	void OnServerInitialized()
	{
		Debug.Log( "Server initialized" );
	}
}
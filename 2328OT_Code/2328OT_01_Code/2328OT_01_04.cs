using UnityEngine;
using System.Collections;

public class ExampleUnityNetworkingConnectToServer : MonoBehaviour
{
	private string ip = "";
	private string port = "";
	private string password = "";

	void OnGUI()
	{
		GUILayout.Label( "IP Address" );
		ip = GUILayout.TextField( ip, GUILayout.Width( 200f ) );

		GUILayout.Label( "Port" );
		port = GUILayout.TextField( port, GUILayout.Width( 50f ) );

		GUILayout.Label( "Password (optional)" );
		password = GUILayout.PasswordField( password, '*', GUILayout.Width( 200f ) );

		if( GUILayout.Button( "Connect" ) )
		{
			int portNum = 25005;

			// failed to parse port number – a more ideal solution is to limit input to numbers only, a number of examples can be found on the Unity forums
			if( !int.TryParse( port, out portNum ) )
			{
				Debug.LogWarning( "Given port is not a number" );
			}
			// try to initiate a direct connection to the server
			else
			{
				Network.Connect( ip, portNum, password );
			}
		}
	}

	void OnConnectedToServer()
	{
		Debug.Log( "Connected to server!" );
	}

	void OnFailedToConnect( NetworkConnectionError error )
	{
		Debug.Log( "Failed to connect to server: " + error.ToString() );
	}

}
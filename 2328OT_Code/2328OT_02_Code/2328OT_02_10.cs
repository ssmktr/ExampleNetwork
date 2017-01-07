using UnityEngine;
using System.Collections;

public class ConnectToPhoton : MonoBehaviour
{
	public GameObject LobbyScreen;

	private string username = "";

	private bool connecting = false;
	private string error = null;

	void Start()
	{
		// load the last username the player entered
		username = PlayerPrefs.GetString( "Username", "" );
	}

	void OnGUI()
	{
		// in the process of connecting...
		if( connecting )
		{
			GUILayout.Label( "Connecting..." );
			return;
		}

		// an error occurred, display it
		if( error != null )
		{
			GUILayout.Label( "Failed to connect: " + error );
			return;
		}

		// let the user enter their username
		GUILayout.Label( "Username" );
		username = GUILayout.TextField( username, GUILayout.Width( 200f ) );

		if( GUILayout.Button( "Connect" ) )
		{
			// remember username for next time
			PlayerPrefs.SetString( "Username", username );

			// in the process of connecting
			connecting = true;

			// set username, connect to photon
			PhotonNetwork.playerName = username;
			PhotonNetwork.ConnectUsingSettings( "v1.0" );
		}
	}

	void OnJoinedLobby()
	{
		// joined the lobby, show lobby screen

		connecting = false;
		gameObject.SetActive( false );
		LobbyScreen.SetActive( true );
	}

	void OnFailedToConnectToPhoton( DisconnectCause cause )
	{
		// failed to connect, store error for display

		connecting = false;
		error = cause.ToString();
	}
}
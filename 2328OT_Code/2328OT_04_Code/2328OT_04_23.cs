using UnityEngine;
using System.Collections;

public class MessageHandler : MonoBehaviour
{
	void OnEnable()
	{
		NetworkUtils.connection.OnMessage += connection_OnMessage;
		NetworkUtils.connection.OnDisconnect += connection_OnDisconnect;
	}

	void OnDisable()
	{
		NetworkUtils.connection.OnMessage -= connection_OnMessage;
		NetworkUtils.connection.OnDisconnect -= connection_OnDisconnect;
	}

	void connection_OnDisconnect( object sender, string message )
	{
		Debug.Log( "Disconnected from server" );
		NetworkUtils.connection = null;

		// save player object
		NetworkUtils.playerObject.Save();

		// go back to main menu
		Application.LoadLevel( "MainMenu" );
	}

	void connection_OnMessage( object sender, PlayerIOClient.Message e )
	{
		// handle incoming messages
	}
}
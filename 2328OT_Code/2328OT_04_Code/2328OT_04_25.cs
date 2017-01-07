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
		switch( e.Type )
		{
			// server sent us our ID
			case "SetID":
				NetworkUtils.localPlayerID = e.GetInt( 0 );
				break;
			// add a player to list of players in the room
			case "UserJoined":
				NetworkUtils.PlayersInRoom.Add( e.GetInt( 0 ), e.GetString( 1 ) );
				break;
			// remove player from list of players
			case "UserLeft":
				NetworkUtils.PlayersInRoom.Remove( e.GetInt( 0 ) );
				//clean up this player's bots
				foreach( int botID in BotInfo.botMap.Keys )
				{
					Destroy( BotInfo.botMap[ botID ].gameObject );
				}
				break;
			// spawn a bot
			case "OnBotSpawned":
				break;
			// update a bot
			case "UpdateBot":
				break;
			// destroy a bot
			case "BotDied":
				break;
			// local player got a kill
			case "GotKill":
				break;
			// one of local player's bots took damage
			case "TookDamage":
				break;
		}
	}
}
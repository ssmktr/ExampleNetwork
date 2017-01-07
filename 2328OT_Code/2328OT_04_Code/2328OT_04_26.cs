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

	public GameObject BotPrefab;
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
				foreach( ulong botID in BotInfo.botMap.Keys )
				{
					Destroy( BotInfo.botMap[ botID ].gameObject );
				}
				break;
			// spawn a bot
			case "OnBotSpawned":
				int spawnedBotOwnerID = e.GetInt( 0 );
				ulong spawnedBotID = e.GetULong( 1 );
				float spawnedBotPosX = e.GetFloat( 2 );
				float spawnedBotPosY = e.GetFloat( 3 );

				GameObject bot = (GameObject)Instantiate( BotPrefab, new Vector3( spawnedBotPosX, 0f, spawnedBotPosY ), Quaternion.identity );
				bot.GetComponent<BotInfo>().OwnerID = spawnedBotOwnerID;
				bot.GetComponent<BotInfo>().BotID = spawnedBotID;
				break;
			// update a bot
			case "UpdateBot":
				ulong updateBotID = e.GetULong( 0 );
				float updatePosX = e.GetFloat( 1 );
				float updatePosY = e.GetFloat( 2 );
				int updateBotHealth = e.GetInt( 3 );

				BotInfo updateBot = BotInfo.botMap[ updateBotID ];
				updateBot.transform.position = new Vector3( updatePosX, 0f, updatePosY );
				updateBot.SendMessage( "SetHealth", updateBotHealth, SendMessageOptions.DontRequireReceiver );
				break;
			// destroy a bot
			case "BotDied":
				// kill bot
				ulong killedBotID = e.GetULong( 0 );

				BotInfo killedBot = BotInfo.botMap[ killedBotID ];
				if( killedBot.IsMine )
				{
					// increment lost bots
					NetworkUtils.playerObject.Set( "Deaths", NetworkUtils.playerObject.GetInt( "Deaths" ) + 1 );
				}

				// destroy bot obj
				GameObject.Destroy( killedBot );
				break;
			// local player got a kill
			case "GotKill":
				// increment kills
				NetworkUtils.playerObject.Set( "Kills", NetworkUtils.playerObject.GetInt( "Kills" ) + 1 );
				break;
			// one of local player's bots took damage
			case "TookDamage":
				Debug.Log( "Taking damage!" );
				break;
		}
	}
}
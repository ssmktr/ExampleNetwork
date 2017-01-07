using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using PlayerIOClient;

public class ConnectScreen : MonoBehaviour
{
	string playerName = "Player" + Random.Range( 0, 10000 );

	bool connecting = true;

	int botsKilled = 0;
	int botsLost = 0;

	void Connect()
	{
		// we need some monobehavior in the scene for player.io to work, but it can be any monobehavior so we’ll just put our NetworkUtils component on it.
		if( GameObject.Find( “_playerIO” ) == null )
		{
			GameObject go = new GameObject( “_playerIO” );
			go.AddComponent<NetworkUtils>();
			DontDestroyOnLoad( go );
			PlayerIO.UnityInit( go.GetComponent<NetworkUtils>() );
		}
		
		PlayerIO.Connect( "YourGameIDHere", "public", playerName, null, null,
			delegate( Client client )
			{
				Debug.Log( "Connected" );
				
				// store client for later retrieval
				NetworkUtils.client = client;

				// load player object
				client.BigDB.LoadMyPlayerObject(
					delegate( DatabaseObject playerObj )
					{
						// store player object for later retrieval
						NetworkUtils.playerObj = playerObj;

						// read stats from player object
						botsKilled = playerObj.GetInt( "Kills", 0 );
						botsLost = playerObj.GetInt( "Deaths", 0 );
					},
					delegate( PlayerIOError error )
					{
						Debug.Log( "Failed loading player object: " + error.Message );
					} );
			},
			delegate( PlayerIOError error )
			{
				Debug.Log( "Failed to connect: " + error.Message );
			} );
	}

	void JoinRoom()
	{
		NetworkUtils.client.Multiplayer.CreateJoinRoom( "$service-room$", "GameRoom", true,
			null,
			new Dictionary<string, string> { },
			delegate( Connection connection )
			{
				Debug.Log( "Connected to room" );
				NetworkUtils.connection = connection;

				// load gameplay scene
				Application.LoadLevel( "GameplayScene" );
			},
			delegate( PlayerIOError error )
			{
				Debug.Log( "Failed to join room: " + error.Message );
			} );
	}

	void OnGUI()
	{
		if( !connecting )
		{
			if( NetworkUtils.playerObject != null )
			{
				GUILayout.Label( "Enemy Bots Destroyed: " + botsKilled );
				GUILayout.Label( "Bots Lost: " + botsLost );
				if( GUILayout.Button( "Play", GUILayout.Width( 100f ) ) )
				{
					// join random room
					JoinRoom();
				}
			}
			else
			{
				playerName = GUILayout.TextField( playerName, GUILayout.Width( 200f ) );
				if( GUILayout.Button( "Connect", GUILayout.Width( 100f ) ) )
				{
					Connect();
				}
			}
		}
		else
		{
			GUILayout.Label( "Connecting..." );
		}
	}
}
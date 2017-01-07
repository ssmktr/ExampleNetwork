using UnityEngine;
using System.Collections;

public class Example_ConnectToPhoton : MonoBehaviour
{
	bool joined = false;
	string error = null;

	Vector2 scroll = Vector2.zero;

	RoomInfo[] rooms = new RoomInfo[ 0 ];

	bool includeFull = true;
	Hashtable filterProperties = null;

	void Start()
	{
		// connect to Photon
		PhotonNetwork.ConnectUsingSettings( "v1.0" );

		// make sure all players are always in the same scene
		// If the master client loads a new scene, all other players will also load the scene
		PhotonNetwork.automaticallySyncScene = true;
	}

	void OnJoinedLobby()
	{
		// we joined Photon and are ready to get a list of rooms
		joined = true;
	}

	void OnFailedToConnectToPhoton( DisconnectCause cause )
	{
		// some error occurred, store it to be displayed
		error = cause.ToString();
	}

	void OnGUI()
	{
		if( PhotonNetwork.room != null )
			return;

		if( !joined && string.IsNullOrEmpty( error ) )
		{
			// we're still connecting to photon...
			GUI.Label( new Rect( 50, 50, 200, 200 ), "Connecting..." );
		}
		else if( joined )
		{
			GUI.Label( new Rect( 50, 50, 200, 200 ), "Connected to photon" );

			// draw the list of games
			GUILayout.BeginArea( new Rect( 50, 80, 200, 500 ), "" );

			if( GUILayout.Button( "Join Random", GUILayout.Width( 200f ) ) )
			{
				PhotonNetwork.JoinRandomRoom( filterProperties, 0 );
			}

			drawLobby();

			GUILayout.EndArea();
		}
		else if( !string.IsNullOrEmpty( error ) )
		{
			// some error occurred, display it
			GUI.Label( new Rect( 50, 50, 200, 200 ), "ERROR: " + error );
		}
	}

	void drawLobby()
	{
		if( rooms.Length == 0 )
		{
			GUILayout.Label( "No rooms available" );
		}
		else
		{
			scroll = GUILayout.BeginScrollView( scroll, GUILayout.ExpandWidth( true ), GUILayout.ExpandHeight( true ) );

			foreach( RoomInfo room in rooms )
			{
				if( GUILayout.Button( room.name + " - " + room.playerCount + "/" + room.maxPlayers ) )
				{
					PhotonNetwork.JoinRoom( room );
				}
			}

			GUILayout.EndScrollView();
		}
	}

	void OnPhotonRandomJoinFailed()
	{
		// create a new room if no room is available to randomly join
		// passing null as the room name will cause the server to generate a name.
		PhotonNetwork.CreateRoom( null, true, true, 8 );
	}

	// if a room is created, load a scene
	void OnCreatedRoom()
	{
		// this will automatically pause the network queue, load the level, and unpause it
		// if other players connect, they will also load this level due to PhotonNetwork.automaticallySyncScene
		PhotonNetwork.LoadLevel( "LevelNameHere" );
	}

	void OnReceivedRoomListUpdate()
	{
		rooms = Example_FilterRooms.FilterRooms( PhotonNetwork.GetRoomList(), includeFull, filterProperties );
	}
}
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

	void OnReceivedRoomListUpdate()
	{
		rooms = Example_FilterRooms.FilterRooms( PhotonNetwork.GetRoomList(), includeFull, filterProperties );
	}
}
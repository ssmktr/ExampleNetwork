using UnityEngine;
using System.Collections;

public class Example_ConnectToPhoton : MonoBehaviour
{
	bool joined = false;
	string error = null;

	Vector2 scroll = Vector2.zero;

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
		// no rooms available
		if( PhotonNetwork.GetRoomList().Length == 0 )
		{
			GUILayout.Label( "No rooms available" );
		}
		// draw a button for each room in a scroll view
		else
		{
			scroll = GUILayout.BeginScrollView( scroll, GUILayout.ExpandWidth( true ), GUILayout.ExpandHeight( true ) );

			foreach( RoomInfo room in PhotonNetwork.GetRoomList() )
			{
				GUILayout.Button( room.name + " - " + room.playerCount + "/" + room.maxPlayers );
			}

			GUILayout.EndScrollView();
		}
	}
}

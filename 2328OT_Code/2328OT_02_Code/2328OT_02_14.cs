using UnityEngine;
using System.Collections;

public class LobbyScreen : MonoBehaviour
{
	public GameObject FriendsListScreen;

	Vector2 lobbyScroll = Vector2.zero;

	void Awake()
	{
		// make sure every player is always on the same scene
		PhotonNetwork.automaticallySyncScene = true;
	}

	void OnGUI()
	{
		// join a random room
		if( GUILayout.Button( "Join Random", GUILayout.Width( 200f ) ) )
		{
			PhotonNetwork.JoinRandomRoom();
		}

		// create a new room
		if( GUILayout.Button( "Create Room", GUILayout.Width( 200f ) ) )
		{
			PhotonNetwork.CreateRoom( PlayerPrefs.GetString( "Username" ) + "'s Room", true, true, 32 );
		}
		
		// show the friends list management page
		if( GUILayout.Button( "Friends", GUILayout.Width( 200f ) ) )
		{
			gameObject.SetActive( false );
			FriendsListScreen.SetActive( true );
		}

		RoomInfo[] rooms = PhotonNetwork.GetRoomList();

		// no rooms available, inform the user
		if( rooms.Length == 0 )
		{
			GUILayout.Label( "No Rooms Available" );
		}
		else
		{
			// show a scrollable list of rooms

			lobbyScroll = GUILayout.BeginScrollView( lobbyScroll, GUILayout.Width( 220f ), GUILayout.ExpandHeight( true ) );

			foreach( RoomInfo room in PhotonNetwork.GetRoomList() )
			{
				GUILayout.BeginHorizontal( GUILayout.Width( 200f ) );

				// display room name and capacity
				GUILayout.Label( room.name + " - " + room.playerCount + "/" + room.maxPlayers );

				// connect to the room
				if( GUILayout.Button( "Enter" ) )
				{
					PhotonNetwork.JoinRoom( room );
				}

				GUILayout.EndHorizontal();
			}

			GUILayout.EndScrollView();
		}
	}

	// if no room could be randomly joined, create a new room
	void OnPhotonRandomJoinFailed()
	{
		PhotonNetwork.CreateRoom( PlayerPrefs.GetString( "Username" ) + "'s Room", true, true, 32 );
	}

	// after creating the room, load the chat room scene
	void OnCreatedRoom()
	{
		PhotonNetwork.LoadLevel( "ChatRoom" );
	}
}
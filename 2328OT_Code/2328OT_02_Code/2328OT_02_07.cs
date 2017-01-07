using UnityEngine;
using System.Collections;

public class Example_FindFriends : MonoBehaviour
{
	public string[] Friends = new string[ 0 ];

	void OnJoinedLobby()
	{
		// Set our player name?
		PhotonNetwork.playerName = "TestPlayerName";

		// fetch the friends list from Photon
		PhotonNetwork.FindFriends( Friends );
	}

	void OnGUI()
	{
		if( !PhotonNetwork.connected )
			return;

		if( PhotonNetwork.Friends == null )
			return;

		GUILayout.BeginArea( new Rect( 300, 0, 200, 500 ) );

		// display a list of friends
		foreach( FriendInfo friend in PhotonNetwork.Friends )
		{
			GUILayout.BeginHorizontal( GUILayout.Width( 200f ) );

			// display friend's name and online status
			GUILayout.Label( friend.Name );
			GUILayout.Label( friend.IsOnline ? "Online" : "Offline" );

			// if the friend is playing in a room, allow the player to join them
			if( friend.IsInRoom )
			{
				if( GUILayout.Button( "Join" ) )
				{
					PhotonNetwork.JoinRoom( friend.Room );
				}
			}

			GUILayout.EndHorizontal();
		}

		GUILayout.EndArea();
	}
}
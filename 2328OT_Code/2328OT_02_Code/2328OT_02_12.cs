using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chatbox : Photon.MonoBehaviour
{
	// keep up to this many messages, after which older messages start to be deleted
	public int MaxMessages = 100;

	private Vector2 chatScroll = Vector2.zero;
	private List<string> chatMessages = new List<string>();

	private string message = "";

	void OnGUI()
	{
		if( GUILayout.Button( "Leave Room" ) )
		{
			// leave the room we're in
			PhotonNetwork.LeaveRoom();
		}

		// display a scrolling list of chat messages
		chatScroll = GUILayout.BeginScrollView( chatScroll, GUILayout.Width( Screen.width ), GUILayout.ExpandHeight( true ) );

		foreach( string msg in chatMessages )
		{
			GUILayout.Label( msg );
		}

		GUILayout.EndScrollView();

		GUILayout.BeginHorizontal();

		// let the user type a message
		message = GUILayout.TextField( message, GUILayout.ExpandWidth( true ) );

		if( GUILayout.Button( "Send", GUILayout.Width( 100f ) ) )
		{
			// tell everybody to add this message
			photonView.RPC( "AddChat", PhotonTargets.All, message );
			message = "";
		}

		GUILayout.EndHorizontal();
	}

	[RPC]
	void AddChat( string message, PhotonMessageInfo info )
	{
		// store the received message
		chatMessages.Add( info.sender.name + ": " + message );

		// enforce maximum stored messages
		if( chatMessages.Count > MaxMessages )
		{
			chatMessages.RemoveAt( 0 );
		}

		// set scroll Y to a really big value
		// Unity will automatically clamp this, having the effect of scrolling to the bottom
		chatScroll.y = 10000;
	}

	// if the user leaves, go back to the main scene
	void OnLeftRoom()
	{
		Application.LoadLevel( "Main" );
	}
}
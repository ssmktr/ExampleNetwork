using UnityEngine;
using System.Collections;

public class Chatbox : MonoBehaviour
{
	private string PlayerName;
	private string _playerName;

	private string chatText = "";
	
	private List<string> messages = new List<string>();
	private Vector2 scrollPosition = Vector2.zero;
	
	void Start()
	{
		// we�ll assign a random guest name to the player, or load up their last name from player prefs
		PlayerName = PlayerPrefs.GetString( "PlayerName", "Guest" + Random.Range( 0, 9999 ) );
		
		// we use a temporary _playerName to pass to GUILayout.TextField so that we avoid changing our actual name until we�ve hit the �Change� button.
		_playerName = PlayerName;

		// subscribe to the chatroom.
		// if you wanted, you could easily create separate chatrooms, allow the user to pick a chatroom and simply insert the name of the chatroom here.
		PubNubWrapper.instance.Subscribe( "Chatbox", HandleMessage );
	}

	void HandleMessage( string message )
	{
		Debug.Log( message );
		messages.Add( message );

		// too many messages? Remove the oldest one
		if( messages.Count > 100 )
			messages.RemoveAt( 0 );

		// Unity clamps the scroll value. Setting it sufficiently high will cause it to scroll to bottom.
		scrolPosition.y = messages.Count * 100f;
	}

	void OnGUI()
	{
		_playerName = GUILayout.TextField( _playerName, GUILayout.Width( 200f ) );
		if( GUILayout.Button( "Change Name", GUILayout.Width( 200f ) ) )
		{
			// inform everyone else in the room that the player has changed their name.
			PubNubWrapper.instance.Publish( PlayerName + " changed their name to " + _playerName, "Chatbox" );
			
			// assign the new name
			PlayerName = _playerName;
		}
		
		scrollPosition = GUILayout.BeginScrollView( scrollPosition, GUILayout.Width( Screen.width ), GUILayout.Height( Screen.height - 75f ) );
		{
			// display each message
			for( int i = 0; i < messages.Count; i++ )
			{
				GUILayout.Label( messages[ i ] );
			}
		}
		GUILayout.EndScrollView();
		
		GUILayout.BeginHorizontal( GUILayout.Width( Screen.width ) );
		{
			chatText = GUILayout.TextField( chatText, GUILayout.ExpandWidth( true ) );
			if( GUILayout.Button( "Send", GUILayout.Width( 100f ) ) )
			{
				// did the player type a message like this:
				// /me [message]?
				// If so, publish as:
				// [playername] [message] without the colon.	
				if( chatText.StartsWith( "/me " ) )
				{
					chatText = chatText.Replace( "/me", "" );
					PubNubWrapper.instance.Publish( PlayerName + chatText, "Chatbox" );
				}
				else
				{
					// publish the message the player typed as:
					// [playername]: [message]
					PubNubWrapper.instance.Publish( PlayerName + ": " + chatText, "Chatbox" );
				}
				chatText = "";
			}
		}
		GUILayout.EndHorizontal();
	}

	void OnApplicationQuit()
	{
		// when the player quits, save their name to player prefs so when they come back later it�s saved for them.
		PlayerPrefs.SetString( "PlayerName", PlayerName );
	}
}
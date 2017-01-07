using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// contains data about the logged message
struct LogMessage
{
	public string message;
	public LogType type;
}

public class CustomLog : MonoBehaviour
{
	// how many past log messages to store
	public int MaxHistory = 50;

	// a list of stored log messages
	private List<LogMessage> messages = new List<LogMessage>();

	// the position within the scroll view
	private Vector2 scrollPos = Vector2.zero;

	void OnEnable()
	{
		// register a custom log handler
		Application.RegisterLogCallback( HandleLog );
	}
	
	void OnDisable()
	{
		// unregister the log handler
		Application.RegisterLogCallback( null );
	}

	void OnGUI()
	{
		scrollPos = GUILayout.BeginScrollView( scrollPos, GUILayout.ExpandWidth( true ), GUILayout.ExpandHeight( true ) );

		//draw each debug log – switch colors based on log type
		for( int i = 0; i < messages.Count; i++ )
		{
			Color color = Color.white;
			if( messages[i].type == LogType.Warning )
			{
				color = Color.yellow;
			}
			else if( messages[i].type != LogType.Log )
			{
				color = Color.red;
			}

			GUI.color = color;
			GUILayout.Label( messages[i].message );
		}

		GUILayout.EndScrollView();
	}

	void HandleLog( string message, string stackTrace, LogType type )
	{
		// add the message, remove entries if there’s too many
		LogMessage msg = new LogMessage();
		msg.message = message;
		msg.type = type;

		messages.Add( msg );
		
		if( messages.Count >= MaxHistory )
		{
			messages.RemoveAt( 0 );
		}

		// scroll to the newest message by setting to a huge amount
		// will automatically be clamped
		scrollPos.y = 1000f;
	}
}
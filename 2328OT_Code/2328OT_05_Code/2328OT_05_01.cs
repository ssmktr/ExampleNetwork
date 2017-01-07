using UnityEngine;
using System.Collections;

using SimpleJSON;

public class PubNubUtils
{
	public static string[] ParseSubscribeResponse( string response, out string timeToken )
	{
		// parse the JSON as a JSON array
		var json = JSON.Parse( response ).AsArray;

		// first entry is another JSON array
		var messages = json[ 0 ].AsArray;

		// second entry is new time token
		timeToken = json[ 1 ].Value;

		// parse message JSON array into string array
		string[] ret = new string[ messages.Count ];
		for( int i = 0; i < ret.Length; i++ )
		{
			ret[ i ] = messages[ i ].Value;
		}

		// return messages
		return ret;
	}
}
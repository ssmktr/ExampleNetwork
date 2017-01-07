using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PubNubWrapper : MonoBehaviour
{
	public static PubNubWrapper instance;

	public string PublishKey = "";
	public string SubscribeKey = "";

	private Dictionary<string, System.Action<string>> channelMessageHandlers = new Dictionary<string, System.Action<string>>();

	private string timeToken = "0";

	void Awake()
	{
		instance = this;
	}

	// publish a message to the given channel
	public void Publish( string message, string channel )
	{
		// escape the message so we can put it in a URL
		string escapedMessage = WWW.EscapeURL( message ).Replace( "+", "%20" ); // Unity's URL escaping function replaces space with '+'. It's better on some platforms to use %20

		// form the URL
		// http://pubsub.pubnub.com
		// /publish
		// /[publish key]
		// /[subscribe key]
		// /0
		// /[channel name]
		// /0
		// /[JSON message data]
		string url =
			"http://pubsub.pubnub.com" +
			"/publish" +
			"/" + PublishKey +
			"/" + SubscribeKey +
			"/0" +
			"/" + channel +
			"/0" +
			"/\”" + escapedMessage + “\””;

		// make the request
		WWW www = new WWW( url );
	}

	// subscribe to receive messages from the given channel
	public void Subscribe( string channel, System.Action<string> messageHandler )
	{
		channelMessageHandlers.Add( channel, messageHandler );
		StartCoroutine( doSubscribe( channel ) );
	}

	IEnumerator doSubscribe( string channel )
	{
		// as long as we have a message handler for the given channel (we're subscribed), keep making requests
		while( channelMessageHandlers.ContainsKey( channel ) )
		{
			// form the URL
			// http://pubsub.pubnub.com
			// /subscribe
			// /[subscribe key here]
			// /[channel name here]
			// /0
			// /[time token here]
			string url =
				"http://pubsub.pubnub.com" +
				"/subscribe" +
				"/" + SubscribeKey +
				"/" + channel +
				"/0" +
				"/" + timeToken;

			// make the request
			WWW www = new WWW( url );
			
			// in Unity, we can yield a WWW object,
			// which makes Unity “pause” this coroutine
			// until the request has either encountered an error
			// or finished.
			yield return www;

			// www.error is a string
			// it will either be null/empty if there is no error, or it
			// will contain the error message if there was one.
			if( !string.IsNullOrEmpty( www.error ) )
			{
				// log the error to the console
				Debug.LogWarning( "Subscribe failed: " + www.error );
				
				// unsubscribe from the channel,
				// we don’t want error messages spamming the console.
				Unsubscribe( channel );
				
				// yield break causes Unity to stop exiting this
				// coroutine. It is equivalent to “return;” in
				// a regular method.
				yield break;
			}

			// parse the response
			string newToken;
			// parse the response from the server
			// returned is an array of new messages posted since we
			// last made a request
			string[] newMessages = PubNubUtils.ParseSubscribeResponse( www.text, out newToken );

			// store the returned time token
			// this is important to ensure we only get new messages
			timeToken = newToken;

			// make sure we’re still subscribed to this channel
			if( channelMessageHandlers.ContainsKey( channel ) )
			{
				// handle each message separately
				for( int i = 0; i < newMessages.Length; i++ )
				{
					channelMessageHandlers[ channel ]( newMessages[ i ] );
				}
			}
		}
	}

	// unsubscribe to stop receiving messages from the given channel
	public void Unsubscribe( string channel )
	{
	}
}
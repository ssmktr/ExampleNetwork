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
	}

	// unsubscribe to stop receiving messages from the given channel
	public void Unsubscribe( string channel )
	{
	}
}
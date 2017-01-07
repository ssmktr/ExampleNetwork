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
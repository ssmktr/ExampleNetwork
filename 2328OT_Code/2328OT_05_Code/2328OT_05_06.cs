using UnityEngine;
using System.Collections;

public class PubNubTest : MonoBehaviour
{
	void Start()
	{
		PubNubWrapper.instance.Subscribe( "HelloWorld",
			delegate( string message )
			{
				Debug.Log( "Received message: " + message );
			} );

		PubNubWrapper.instance.Publish( "Hello, world!", "HelloWorld" );
	}
}
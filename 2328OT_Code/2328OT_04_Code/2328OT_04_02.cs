using UnityEngine;
using System.Collections;

using PlayerIOClient;

public class ConnectToPlayerIO : MonoBehaviour
{
	public bool UseDevServer = true;

	Client client;

	void Start()
	{
		PlayerIO.UnityInit( this );

		PlayerIO.Connect( "YourGameIDHere", "public", "YourUserIDHere", null, null,
		delegate( Client c )
		{
			// connected successfully
			client = c;
			Debug.Log( "Connected" );

			// if we're using the dev server, connect to the local IP
			if( UseDevServer )
			{
				client.Multiplayer.DevelopmentServer = new ServerEndpoint( "127.0.0.1", 8184 );
			}
		},
		delegate( PlayerIOError error )
		{
			// did not connect successfully
			Debug.Log( error.Message );
		} );
	}
}
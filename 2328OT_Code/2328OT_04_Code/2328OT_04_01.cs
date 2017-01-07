using UnityEngine;
using System.Collections;

using PlayerIOClient;

public class ConnectToPlayerIO : MonoBehaviour
{
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
		},
		delegate( PlayerIOError error )
		{
			// did not connect successfully
			Debug.Log( error.Message );
		} );
	}
}
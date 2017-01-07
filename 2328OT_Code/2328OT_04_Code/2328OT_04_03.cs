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

				GetRoomList();
			}
		},
		delegate( PlayerIOError error )
		{
			// did not connect successfully
			Debug.Log( error.Message );
		} );
	}

	void GetRoomList()
	{
		// get a list of all rooms with the given room type and search criteria (null = all rooms)
		client.Multiplayer.ListRooms( "SomeRoomType", null, 0, 0,
			delegate( RoomInfo[] rooms )
			{
				Debug.Log( "Found rooms: " + rooms.Length );
			},
			delegate( PlayerIOError error )
			{
				Debug.Log( error.Message );
			} );
	}
}
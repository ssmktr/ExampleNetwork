using UnityEngine;
using System.Collections;

using PlayerIOClient;

public class ConnectToPlayerIO : MonoBehaviour
{
	public bool UseDevServer = true;

	Client client;
	Connection roomConnection;

	private RoomInfo[] rooms = null;

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
		client.Multiplayer.ListRooms( "MyCode", null, 0, 0,
			delegate( RoomInfo[] rooms )
			{
				Debug.Log( "Found rooms: " + rooms.Length );
				this.rooms = rooms;
			},
			delegate( PlayerIOError error )
			{
				Debug.Log( error.Message );
			} );
	}

	void OnGUI()
	{
		if( roomConnection != null )
			return;

		if( GUILayout.Button( "Create Room", GUILayout.Width( 200f ) ) )
		{
			// “MyCode” is the default room type provided with the Serverside Code solution
			client.Multiplayer.CreateRoom( null, "MyCode", true, null,
				delegate( string roomID )
				{
					Debug.Log( "Room created" );
					client.Multiplayer.JoinRoom( roomID, null,
					delegate( Connection connection )
					{
							
Debug.Log( "Connected to room!" );
roomConnection = connection;
					},
					delegate( PlayerIOError error )
					{
						Debug.Log( error.Message );
					} );
				},
				delegate( PlayerIOError error )
				{
					Debug.Log( error.Message );
				} );
		}

		if( rooms == null )
			return;

		foreach( RoomInfo room in rooms )
		{
			if( GUILayout.Button( room.Id, GUILayout.Width( 200f ) ) )
			{
				client.Multiplayer.JoinRoom( room.Id, null,
					delegate( Connection connection )
					{
						Debug.Log( "Connected to room!" );
						roomConnection = connection;
					},
					delegate( PlayerIOError error )
					{
						Debug.Log( error.Message );
					} );
			}
		}
	}

	void OnApplicationQuit()
	{
		if( roomConnection != null )
			roomConnection.Disconnect();
	}
}
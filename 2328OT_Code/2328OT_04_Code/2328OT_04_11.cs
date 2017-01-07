using UnityEngine;
using System.Collections;

using PlayerIOClient;

public class ConnectToPlayerIO : MonoBehaviour
{
	public bool UseDevServer = true;

	Client client;
	Connection roomConnection;

	private RoomInfo[] rooms = null;

	DatabaseObject playerObj;

	void Start()
	{
		PlayerIO.UnityInit( this );

		PlayerIO.Connect( "YourGameIDHere", "public", "YourUserIDHere", null, null,
		delegate( Client c )
		{
			// connected successfully
			client = c;
			Debug.Log( "Connected" );

			// load the player object
			client.BigDB.LoadMyPlayerObject(
			delegate( DatabaseObject obj )
			{
				playerObj = obj;

				Debug.Log( "Player object loaded" );
			},
			delegate( PlayerIOError error )
			{
				Debug.Log( error.Message );
			} );

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
		{
			if( GUILayout.Button( "Send Message", GUILayout.Width( 200f ) ) )
			{
				roomConnection.Send( "TestMessage", "Hello, world!" );
			}
			return;
		}
		
		if( GUILayout.Button( "Save Test Player Object", GUILayout.Width( 200f ) ) )
		{
			playerObj.Set( "TestProperty", "someValue" );
			playerObj.Save(
			delegate()
			{
				Debug.Log( "Player object saved" );
			},
			delegate( PlayerIOError error )
			{
				Debug.Log( error.Message );
			} );
		}

		if( GUILayout.Button( "Join Random", GUILayout.Width( 200f ) ) )
		{
			client.Multiplayer.CreateJoinRoom( "$service-room$", "MyCode", true, null, null,
				delegate( Connection connection )
				{
					Debug.Log( "Joining room" );
					roomConnection = connection;
					roomConnection.OnMessage += new MessageReceivedEventHandler( OnMessage );
					roomConnection.OnDisconnect += new DisconnectEventHandler( OnDisconnect );
				},
				delegate( PlayerIOError error )
				{
					Debug.Log( error.Message );
				} );
		}

		if( GUILayout.Button( "Create Room", GUILayout.Width( 200f ) ) )
		{
			client.Multiplayer.CreateRoom( null, "MyCode", true, null,
				delegate( string roomID )
				{
					Debug.Log( "Room created" );
					client.Multiplayer.JoinRoom( roomID, null,
					delegate( Connection connection )
					{
					Debug.Log( "Connected to room!" );
					roomConnection = connection;
					roomConnection.OnMessage += new MessageReceivedEventHandler( OnMessage );
					roomConnection.OnDisconnect += new DisconnectEventHandler( OnDisconnect );
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
					roomConnection.OnMessage += new MessageReceivedEventHandler( OnMessage );
					roomConnection.OnDisconnect += new DisconnectEventHandler( OnDisconnect );
					},
					delegate( PlayerIOError error )
					{
						Debug.Log( error.Message );
					} );
			}
		}
	}

	// called when we've disconnected from the room
	void OnDisconnect( object sender, string message )
	{
		Debug.Log( "Disconnected from room" );
	}

	// called when a message is received
	void OnMessage( object sender, Message e )
	{
	}

	void OnApplicationQuit()
	{
		if( roomConnection != null )
			roomConnection.Disconnect();
	}
}
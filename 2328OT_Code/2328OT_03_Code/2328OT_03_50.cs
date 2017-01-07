using UnityEngine;
using System.Collections;

using ExitGames.Client.Photon;

public class StarCollectorClient : MonoBehaviour, IPhotonPeerListener
{
	public static PhotonPeer Connection;
	public static bool Connected = false;
	public static long PlayerID;

	public string ServerIP = "127.0.0.1:5055";
	public string AppName = "StarCollectorDemo";

	public GameObject PlayerPrefab;
	public GameObject StarPrefab;

	void Start()
	{
		Debug.Log( "Connecting..." );
		Connection = new PhotonPeer( this, ConnectionProtocol.Udp );
		Connection.Connect( ServerIP, AppName );

		StartCoroutine( doService() );
	}

	void OnDestroy()
	{
		// explicitly disconnect if the client game object is destroyed
			if( Connected )
				Connection.Disconnect();
	}

	// update peer 20 times per second
	IEnumerator doService()
	{
		while( true )
		{
			Connection.Service();
			yield return new WaitForSeconds( 0.05f );
		}
	}

	#region IPhotonPeerListener Members

	public void DebugReturn( DebugLevel level, string message )
	{
		// log message to console
		Debug.Log( message );
	}

	public void OnEvent( EventData eventData )
	{
		//server raised an event
		switch( (StarCollectorEventTypes)eventData.Code )
		{
			// store player ID
			case StarCollectorEventTypes.ReceivePlayerID:
				long playerId = (long)eventData.Parameters[ 0 ];
				PlayerID = playerId;
				Debug.Log( "Received player ID, awaiting game state..." );
				break;
			// spawn actor
			case StarCollectorEventTypes.CreateActor:
				byte actorType = (byte)eventData.Parameters[ 0 ];
				long actorID = (long)eventData.Parameters[ 1 ];
				float posX = (float)eventData.Parameters[ 2 ];
				float posY = (float)eventData.Parameters[ 3 ];
				GameObject actor = null;
				switch( actorType )
				{
					// Star
					case 0:
						actor = (GameObject)Instantiate( StarPrefab, new Vector3( posX, 0f, posY ), Quaternion.identity );
						break;
					// Player
					case 1:
						long ownerID = (long)eventData.Parameters[ 4 ];
						actor = (GameObject)Instantiate( PlayerPrefab, new Vector3( posX, 0f, posY ), Quaternion.identity );
						actor.SendMessage( "SetOwnerID", ownerID );
						break;
				}
				actor.SendMessage( "SetActorID", actorID );
				break;
			// destroy actor
			case StarCollectorEventTypes.DestroyActor:
				GameActor destroyActor = GameActor.Actors[ (long)eventData.Parameters[ 0 ] ];
				if( destroyActor != null )
					destroyActor.Destruct();
				break;
			// update actor
			case StarCollectorEventTypes.UpdateActor:
				GameActor updateActor = GameActor.Actors[ (long)eventData.Parameters[ 0 ] ];
				float newPosX = (float)eventData.Parameters[ 1 ];
				float newPosY = (float)eventData.Parameters[ 2 ];
				updateActor.SendMessage( "UpdatePosition", new Vector3( newPosX, 0f, newPosY ), SendMessageOptions.DontRequireReceiver );
				break;
		}
	}

	public void OnOperationResponse( OperationResponse operationResponse )
	{
		//server sent operation response
	}

	public void OnStatusChanged( StatusCode statusCode )
	{
		// log status change
		Debug.Log( "Status change: " + statusCode.ToString() );

		switch( statusCode )
		{
			case StatusCode.Connect:
				Debug.Log( "Connected, awaiting player ID..." );
				break;
			case StatusCode.Disconnect:
			case StatusCode.DisconnectByServer:
			case StatusCode.DisconnectByServerLogic:
			case StatusCode.DisconnectByServerUserLimit:
			case StatusCode.Exception:
			case StatusCode.ExceptionOnConnect:
			case StatusCode.SecurityExceptionOnConnect:
			case StatusCode.TimeoutDisconnect:
				StopAllCoroutines();
				Connected = false;
				break;
		}
	}

	#endregion
}
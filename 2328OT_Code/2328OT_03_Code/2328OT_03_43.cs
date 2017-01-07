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
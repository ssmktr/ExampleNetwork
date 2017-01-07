using UnityEngine;
using System.Collections;

using ExitGames.Client.Photon;

public class PhotonAckClient : MonoBehaviour, IPhotonPeerListener
{
	public PhotonPeer peer;

	private bool connected = false;

	public void Start()
	{
		// connect to Photon server
		peer = new PhotonPeer( this, ConnectionProtocol.Udp );
		peer.Connect( "127.0.0.1:5055", "PhotonAckServer" );

		StartCoroutine( doService() );
	}

	// update peer 10 times per second
	IEnumerator doService()
	{
		while( true )
		{
			peer.Service();
			yield return new WaitForSeconds( 0.1f );
		}
	}

	void OnGUI()
	{
		GUILayout.Label( "Connected: " + connected.ToString() );

		if( connected )
		{
			if( GUILayout.Button( "Send Operation Request" ) )
			{
				// send a message to the server
				peer.OpCustom( 0, new System.Collections.Generic.Dictionary<byte, object>(), true );
			}
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
		Debug.Log( "Received event - type: " + eventData.Code.ToString() );
	}

	public void OnOperationResponse( OperationResponse operationResponse )
	{
		//server sent operation response
		Debug.Log( "Received op response - type: " + operationResponse.OperationCode.ToString() );
	}

	public void OnStatusChanged( StatusCode statusCode )
	{
		// connected to Photon server
		if( statusCode == StatusCode.Connect )
		{
			connected = true;
		}

		// log status change
		Debug.Log( "Status change: " + statusCode.ToString() );
	}

	#endregion
}
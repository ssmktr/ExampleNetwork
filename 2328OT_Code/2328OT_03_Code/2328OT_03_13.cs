using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using System.Collections.Generic;

class PhotonAckPeer : PeerBase
{
	private static long lastAssignedPlayerID = long.MinValue;
	private static object lockPlayerID = new object();

	public ulong PlayerID;

	public PhotonAckPeer( IRpcProtocol protocol, IPhotonPeer unmanagedPeer )
		: base( protocol, unmanagedPeer )
	{
		lock( lockPlayerID )
		{
			this.PlayerID = lastAssignedPlayerID;
			lastAssignedPlayerID++;
		}

		PhotonAckGame.Instance.PeerConnected( this );

		EventData evt = new EventData( (byte)PhotonAckEventType.AssignPlayerID );
		evt.Parameters = new Dictionary<byte, object>() { { 0, this.PlayerID } };
		this.SendEvent( evt, new SendParameters() );
	}

	protected override void OnDisconnect( DisconnectReason reasonCode, string reasonDetail )
	{
		PhotonAckGame.Instance.PeerDisconnected( this );
	}

	protected override void OnOperationRequest( OperationRequest operationRequest, SendParameters sendParameters )
	{
		PhotonAckGame.Instance.OnOperationRequest( this, operationRequest, sendParameters );
	}
}
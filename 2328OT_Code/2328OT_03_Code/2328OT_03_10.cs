using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using System.Collections.Generic;

class PhotonAckPeer : PeerBase
{
	public PhotonAckPeer( IRpcProtocol protocol, IPhotonPeer unmanagedPeer )
		: base( protocol, unmanagedPeer )
	{
		PhotonAckGame.Instance.PeerConnected( this );
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
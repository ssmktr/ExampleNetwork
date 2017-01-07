using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using System.Collections.Generic;

class PhotonAckPeer : PeerBase
{
	public PhotonAckPeer( IRpcProtocol protocol, IPhotonPeer unmanagedPeer )
		: base( protocol, unmanagedPeer )
	{
	}

	protected override void OnDisconnect( DisconnectReason reasonCode, string reasonDetail )
	{
		
	}

	protected override void OnOperationRequest( OperationRequest operationRequest, SendParameters sendParameters )
	{
		// send an "ack" back to the client
		OperationResponse response = new OperationResponse( (byte)PhotonAckResponseTypes.Ack );
		this.SendOperationResponse( response, sendParameters );
	}
}
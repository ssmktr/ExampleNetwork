using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;

namespace StarCollectorDemo
{
	public class StarCollectorDemoPeer : PeerBase
	{
		public StarCollectorDemoPeer( IRpcProtocol protocol, IPhotonPeer unmanagedPeer )
			: base( protocol, unmanagedPeer )
		{
			lock( StarCollectorDemoGame.Instance )
			{
				StarCollectorDemoGame.Instance.PeerJoined( this );
			}
		}

		protected override void OnDisconnect( DisconnectReason reasonCode, string reasonDetail )
		{
			lock( StarCollectorDemoGame.Instance )
			{
				StarCollectorDemoGame.Instance.PeerLeft( this );
			}
		}

		protected override void OnOperationRequest( OperationRequest operationRequest, SendParameters sendParameters )
		{
			StarCollectorDemoGame.Instance.OnOperationRequest( this, operationRequest, sendParameters );
		}
	}
}
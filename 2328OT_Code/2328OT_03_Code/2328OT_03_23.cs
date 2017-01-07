using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;

namespace StarCollectorDemo
{
	public class StarCollectorDemoPeer : PeerBase
	{
		private static long lastAssignedID = long.MinValue;
		private static object allocateIDLock = new object();

		public long PlayerID;

		public StarCollectorDemoPeer( IRpcProtocol protocol, IPhotonPeer unmanagedPeer )
			: base( protocol, unmanagedPeer )
		{
			lock( StarCollectorDemoGame.Instance )
			{
				StarCollectorDemoGame.Instance.PeerJoined( this );
			}

			lock( allocateIDLock )
			{
				PlayerID = lastAssignedID;
				lastAssignedID++;
			}

			//notify player of their ID
			EventData evt = new EventData();
			evt.Code = (byte)StarCollectorEventTypes.ReceivePlayerID;
			evt.Parameters = new System.Collections.Generic.Dictionary<byte, object>();
			evt.Parameters[ 0 ] = PlayerID;

			this.SendEvent( evt, new SendParameters() );
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
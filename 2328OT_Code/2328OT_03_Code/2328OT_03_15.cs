using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;

using System.Collections.Generic;

namespace StarCollectorDemo
{
	public class StarCollectorDemoGame
	{
		public static StarCollectorDemoGame Instance;

		public void Startup()
		{
		}

		public void Shutdown()
		{
		}

		public void PeerJoined( StarCollectorDemoPeer peer )
		{
		}

		public void PeerLeft( StarCollectorDemoPeer peer )
		{
		}

		public void OnOperationRequest( StarCollectorDemoPeer sender, OperationRequest operationRequest, SendParameters sendParameters )
		{
		}
	}
}
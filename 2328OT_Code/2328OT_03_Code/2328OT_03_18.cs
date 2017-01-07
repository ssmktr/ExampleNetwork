using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using ExitGames.Concurrency.Fibers;

using System.Collections.Generic;

namespace StarCollectorDemo
{
	public class StarCollectorDemoGame
	{
		public static StarCollectorDemoGame Instance;

		private IFiber executionFiber;

		public void Startup()
		{
			// create a new execution fiber and start it
			executionFiber = new PoolFiber();
			executionFiber.Start();
		}

		public void Shutdown()
		{
			// dispose the execution fiber
			executionFiber.Dispose();
		}

		public void PeerJoined( StarCollectorDemoPeer peer )
		{
		}

		public void PeerLeft( StarCollectorDemoPeer peer )
		{
		}

		public void OnOperationRequest( StarCollectorDemoPeer sender, OperationRequest operationRequest, SendParameters sendParameters )
		{
			// schedule a message to be processed on the main thread
			executionFiber.Enqueue( () => { this.ProcessMessage( sender, operationRequest, sendParameters ); } );
		}

		public void ProcessMessage( StarCollectorDemoPeer sender, OperationRequest operationRequest, SendParameters sendParameters )
		{
			// process messages here
		}
	}
}
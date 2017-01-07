using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using ExitGames.Concurrency.Fibers;

using System.Collections.Generic;

namespace StarCollectorDemo
{
	public class StarCollectorDemoGame
	{
		public static StarCollectorDemoGame Instance;

		public List<StarCollectorDemoPeer> PeerList;

		private IFiber executionFiber;

		public void Startup()
		{
			// create a new execution fiber and start it
			executionFiber = new PoolFiber();
			executionFiber.Start();

			PeerList = new List<StarCollectorDemoPeer>();
		}

		public void Shutdown()
		{
			// dispose the execution fiber
			executionFiber.Dispose();
		}

		public void PeerJoined( StarCollectorDemoPeer peer )
		{
			// schedule peer to be added to PeerList on the main thread
			executionFiber.Enqueue( () =>
				{
					PeerList.Add( peer );
				} );
		}

		public void PeerLeft( StarCollectorDemoPeer peer )
		{
			// schedule peer to be removed from PeerList on the main thread
			executionFiber.Enqueue( () =>
			{
				PeerList.Remove( peer );
			} );
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
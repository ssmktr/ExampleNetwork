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

		public List<Star> Stars = new List<Star>();
		public List<Player> Players = new List<Player>();

		private IFiber executionFiber;

		private long lastAssignedActorID = long.MinValue;

		private System.Random rand = new System.Random();

		public long AllocateActorID()
		{
			return lastAssignedActorID++;
		}

		public void SpawnStar()
		{
			// find a random position
			double x = rand.NextDouble();
			double y = rand.NextDouble();

			// map to the range -50, +50
			x -= 0.5f;
			x *= 100f; // 0.5 * 100 = 50
			y -= 0.5f;
			y *= 100f;

			Star star = new Star();
			star.PosX = (float)x;
			star.PosY = (float)y;

			star.ActorID = AllocateActorID();

			Stars.Add( star );
		}

		public void SpawnPlayer( StarCollectorDemoPeer peer )
		{
			Player player = new Player();
			player.Owner = peer;
			player.ActorID = AllocateActorID();

			Players.Add( player );
		}

		public void Simulate( float timeStep )
		{
			// copy star collection so we can modify collection while iterating
			Star[] stars = Stars.ToArray();

			foreach( Player player in Players )
			{
				// simulate "physics"
				player.Simulate( timeStep );

				// compare player with each star
				foreach( Star star in stars )
				{
					if( star.DetectCollision( player ) )
					{
						// collision detected with star
					}
				}
			}
		}

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

					SpawnPlayer( peer );
				} );
		}

		public void PeerLeft( StarCollectorDemoPeer peer )
		{
			// schedule peer to be removed from PeerList on the main thread
			executionFiber.Enqueue( () =>
			{
				PeerList.Remove( peer );

				// remove player object belonging to this peer
				Players.RemoveAll( player => { return player.Owner == peer; } );
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
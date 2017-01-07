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
		
		public void InitRound()
		{
			// reset players
			foreach( Player player in Players )
			{
				player.PosX = 0f;
				player.PosY = 0f;
				player.VelocityX = 0f;
				player.VelocityY = 0f;
				player.Score = 0;
			}

			// spawn new stars
			for( int i = 0; i < 100; i++ )
			{
				SpawnStar();
			}
		}
		
		public void StarPickedUp( Star star, Player taker )
		{
			Stars.Remove( star );

			taker.Score++;

			if( Stars.Count == 0 )
			{
				// the round is over!

				// restart round
				InitRound();
			}
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

			EventData evt = new EventData( (byte)StarCollectorEventTypes.CreateActor );
			evt.Parameters = new Dictionary<byte, object>();
			evt.Parameters[ 0 ] = star.ActorType;
			evt.Parameters[ 1 ] = star.ActorID;
			evt.Parameters[ 2 ] = star.PosX;
			evt.Parameters[ 3 ] = star.PosY;

			BroadcastEvent( evt, new SendParameters() );
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
						StarPickedUp( star, player );
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

			// start a new round
			InitRound();
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
		
		public void BroadcastEvent( IEventData evt, SendParameters param )
		{
			foreach( StarCollectorDemoPeer peer in PeerList )
			{
				peer.SendEvent( evt, param );
			}
		}
	}
}
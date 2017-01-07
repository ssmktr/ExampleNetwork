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

			// broadcast DestroyActor event
			EventData evt = new EventData( (byte)StarCollectorEventTypes.DestroyActor );
			evt.Parameters = new Dictionary<byte, object>();
			evt.Parameters[ 0 ] = star.ActorID;

			BroadcastEvent( evt, new SendParameters() );

			taker.Score++;

			if( Stars.Count == 0 )
			{
				// the round is over!

				// order players by score, pick the player with the highest score
				Player winner = ( from p in Players orderby taker.Score descending select p ).First();

				// broadcast a chat message
				EventData chatEvt = new EventData( (byte)StarCollectorEventTypes.ChatMessage );
				chatEvt.Parameters = new Dictionary<byte, object>();
				chatEvt.Parameters[0] = "Player " + ( winner.Owner as StarCollectorDemoPeer).PlayerID.ToString() + " wins the round with " + winner.Score + " stars!";

				BroadcastEvent( chatEvt, new SendParameters() );

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

			EventData evt = new EventData( (byte)StarCollectorEventTypes.CreateActor );
			evt.Parameters = new Dictionary<byte, object>();
			evt.Parameters[ 0 ] = player.ActorType;
			evt.Parameters[ 1 ] = player.ActorID;
			evt.Parameters[ 2 ] = player.PosX;
			evt.Parameters[ 3 ] = player.PosY;
			evt.Parameters[ 4 ] = peer.PlayerID;

			BroadcastEvent( evt, new SendParameters() );
		}

		public void Simulate( float timeStep )
		{
			// copy star collection so we can modify collection while iterating
			Star[] stars = Stars.ToArray();

			foreach( Player player in Players )
			{
				// simulate "physics"
				player.Simulate( timeStep );

				// broadcast move event
				EventData moveEvt = new EventData( (byte)StarCollectorEventTypes.UpdateActor );
				moveEvt.Parameters = new Dictionary<byte, object>();
				moveEvt.Parameters[ 0 ] = player.ActorID;
				moveEvt.Parameters[ 1 ] = player.PosX;
				moveEvt.Parameters[ 2 ] = player.PosY;

				BroadcastEvent( moveEvt, new SendParameters() );

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

			// schedule Simulate 10 times per second, or once every 100 milliseconds
			executionFiber.ScheduleOnInterval(
				delegate()
				{
					Simulate( 0.1f );
				}, 0, 100 );
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

					// send player CreateActor events for all players and stars

					foreach( Player p in Players )
					{
						EventData evt = new EventData( (byte)StarCollectorEventTypes.CreateActor );
						evt.Parameters = new Dictionary<byte, object>();
						evt.Parameters[ 0 ] = p.ActorType;
						evt.Parameters[ 1 ] = p.ActorID;
						evt.Parameters[ 2 ] = p.PosX;
						evt.Parameters[ 3 ] = p.PosY;
						evt.Parameters[ 4 ] = ( p.Owner as StarCollectorDemoPeer ).PlayerID;

						peer.SendEvent( evt, new SendParameters() );
					}

					foreach( Star s in Stars )
					{
						EventData evt = new EventData( (byte)StarCollectorEventTypes.CreateActor );
						evt.Parameters = new Dictionary<byte, object>();
						evt.Parameters[ 0 ] = s.ActorType;
						evt.Parameters[ 1 ] = s.ActorID;
						evt.Parameters[ 2 ] = s.PosX;
						evt.Parameters[ 3 ] = s.PosY;

						peer.SendEvent( evt, new SendParameters() );
					}

					SpawnPlayer( peer );
				} );
		}

		public void PeerLeft( StarCollectorDemoPeer peer )
		{
			// schedule peer to be removed from PeerList on the main thread
			executionFiber.Enqueue( () =>
			{
				PeerList.Remove( peer );

				// find the player object belonging to the peer
				Player player = Players.Find( actor => { return actor.Owner == peer; } );

				// broadcast DestroyActor event with player's actor ID
				EventData evt = new EventData( (byte)StarCollectorEventTypes.DestroyActor );
				evt.Parameters = new Dictionary<byte, object>();
				evt.Parameters[ 0 ] = player.ActorID;

				BroadcastEvent( evt, new SendParameters() );

				// remove from Players list
				Players.Remove( player );
			} );
		}

		public void OnOperationRequest( StarCollectorDemoPeer sender, OperationRequest operationRequest, SendParameters sendParameters )
		{
			// schedule a message to be processed on the main thread
			executionFiber.Enqueue( () => { this.ProcessMessage( sender, operationRequest, sendParameters ); } );
		}

		public void ProcessMessage( StarCollectorDemoPeer sender, OperationRequest operationRequest, SendParameters sendParameters )
		{
			if( operationRequest.OperationCode == (byte)StarCollectorRequestTypes.MoveCommand )
			{
				// move command from player
				long actorID = (long)operationRequest.Parameters[ 0 ];
				float velX = (float)operationRequest.Parameters[ 1 ];
				float velY = (float)operationRequest.Parameters[ 2 ];

				// find actor
				Player player = ( Players.Find( pl => { return pl.ActorID == actorID; } ) );

				// apply velocity
				player.VelocityX = velX;
				player.VelocityY = velY;
			}
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
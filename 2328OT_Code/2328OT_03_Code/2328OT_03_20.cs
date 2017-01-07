using Photon.SocketServer;

namespace StarCollectorDemo
{
	public class Actor
	{
		public PeerBase Owner; // the Peer that owns this actor, or NULL if actor is owned by server
		public long ActorID; // the ID of this actor instance
		public byte ActorType; // the type of this actor (player, star, etc)
		public float PosX; // the world X position of this actor
		public float PosY; // the world Y position of this actor
		public float Radius; // the collision radius of this actor
	}
}
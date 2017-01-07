using Photon.SocketServer;

namespace StarCollectorDemo
{
	public class Player : Actor
	{
		public float VelocityX = 0f; // the X velocity of the player
		public float VelocityY = 0f; // the Y velocity of the player

		public int Score = 0; // the number of stars this player has collected

		public Player()
		{
			this.ActorType = 1; // player
			this.Radius = 0.5f; // radius, used to detect pickup
		}

		public void Simulate( float timestep )
		{
			this.PosX += this.VelocityX * timestep;
			this.PosY += this.VelocityY * timestep;
		}
	}
}
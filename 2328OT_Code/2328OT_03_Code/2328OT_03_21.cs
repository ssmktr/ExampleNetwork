using Photon.SocketServer;

namespace StarCollectorDemo
{
	public class Star : Actor
	{
		public Star()
		{
			this.ActorType = 0; // star
			this.Radius = 0.25f; // radius, used to detect pickup
		}

		public bool DetectCollision( Actor other )
		{
			// calculate square distance between actors
			float sqrDist = ( ( this.PosX - other.PosX ) * ( this.PosX - other.PosX ) + ( this.PosY - other.PosY ) * ( this.PosY - other.PosY ) );

			// if the distance is less than the sum of the radii, collision occurs
			if( sqrDist <= ( this.Radius + other.Radius ) )
			{
				return true;
			}

			return false;
		}
	}
}
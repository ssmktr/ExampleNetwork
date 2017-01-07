using UnityEngine;
using System.Collections;

public class Damage : MonoBehaviour
{
	public float Health = 100f;

	void TakeDamage( float damage )
	{
		// this code should only ever execute on the server
		if( !Network.isServer )
			return;

		// if you plan on calling TakeDamage multiple times per frame (for example, Shotgun-type weapons can easily result in this), this part is important – it checks if the entity already died earlier in the frame, to ensure the death code isn’t triggered more than once.
		if( Health <= 0 ) return;

		// subtract damage from health
		Health -= damage;

		// check if player died
		if( Health <= 0 )
		{
			// clamp to zero just in case – it looks weird when a player has negative health.
			Health = 0;

			// kick player out of the game when they die
			Network.CloseConnection( networkView.owner, true );
		}

		// notify clients of new health value
		networkView.RPC( “setHealth”, RPCMode.Others, Health ); 
	}

	[RPC]
	void setHealth( float health )
	{
		Health = health;
	}
}
using UnityEngine;
using System.Collections;

public class Damage : MonoBehaviour
{
	public float Health = 100f;

	void TakeDamage( float damage )
	{
		if( !Network.isServer )
		{
			// send an RPC to the server
			networkView.RPC( "serverTakeDamage", RPCMode.Server, damage );
		}
		else
		{
			// note: RPCs with RPCMode.Server, sent from the server "to itself" will not work, the RPC simply fails silently.
			// so we'll manually call the function instead
			serverTakeDamage( damage );
		}
	}

	[RPC]
	void serverTakeDamage( float damage )
	{
		// make sure the player isn’t already dead
		if( Health <= 0 ) return;
			
		// subtract damage from health
		Health -= damage;

		// check if player is dead
		if( Health <= 0f )
		{
			// kick player out of the game when they die
			Network.CloseConnection( networkView.owner, true );
		}
	}
}
using UnityEngine;
using System.Collections;

public class WeaponScript : MonoBehaviour
{
	public float Damage = 10f;

	public LayerMask HitLayers = ~0;
	// note: ~ [tilde] is the unary complement operator - basically it flips all bits of input
	// LayerMask, internally, is actually a bitmask where each bit represents a layer – every bit enabled includes the given layer in the mask (this is why there are 32 layers - there are 32 bits in an integer). No bits on (zero) means an empty layermask,
	// in the number 0, all bits are zero. ~0 is the opposite - all bits are on (and is equal to int.MaxValue). This equivalent to an "Everything" layermask.

	void Update()
	{
		if( networkView.isMine && Input.GetMouseButtonDown( 0 ) )
		{
			// if we’re the server, just directly call the function
			// remember, server cannot use RPCMode.Server, the RPC is simply dropped. So we have to directly call method instead
			if( Network.isServer ) Fire();
			else networkView.RPC( “Fire”, RPCMode.Server );
		}
	}

	[RPC]
	void Fire()
	{
		// this code should never execute on any machine other than the server/host
		if( !Network.isServer )
			return;

		RaycastHit hit;
		if( Physics.Raycast( transform.position, transform.forward, out hit, 100f, HitLayers ) )
		{
			// let a script on the object handle taking damage
			hit.collider.SendMessage( "TakeDamage", Damage, SendMessageOptions.DontRequireReceiver );
		}
	}
}
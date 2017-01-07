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
		// if this is the local network view, and the user presses the left mouse button, call the Fire function
		if( networkView.isMine && Input.GetMouseButtonDown( 0 ) )
		{
			Fire();
		}
	}

	void Fire()
	{
		RaycastHit hit;
		if( Physics.Raycast( transform.position, transform.forward, out hit, 100f, HitLayers ) )
		{
			// let a script on the object handle taking damage
			hit.collider.SendMessage( "TakeDamage", Damage, SendMessageOptions.DontRequireReceiver );
		}
	}
}
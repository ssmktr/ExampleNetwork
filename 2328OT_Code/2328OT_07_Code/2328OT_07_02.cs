using UnityEngine;
using System.Collections;

public class WeaponScript : MonoBehaviour
{
	public float Damage = 10f;
	public LayerMask HitLayers;
	
	public void Fire()
	{
		// tell the server to perform hit detection
		networkView.RPC( "serverDoFire", RPCMode.Server );
	}

	[RPC]
	public void serverDoFire()
	{
		RaycastHit hit;
		if( Physics.Raycast( transform.position, transform.forward, out hit, 100f, HitLayers ) )
		{
			// a script on the object could handle this by generating an RPC, for example
			hit.collider.SendMessage( "TakeDamage", Damage, SendMessageOptions.DontRequireReceiver );
		}
	}
}
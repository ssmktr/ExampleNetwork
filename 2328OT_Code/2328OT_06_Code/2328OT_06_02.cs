using UnityEngine;
using System.Collections;

public class NetworkedPlayer : MonoBehaviour
{
	void OnSerializeNetworkView( BitStream stream, NetworkMessageInfo info )
	{
		Vector3 position = Vector3.zero;
		if( stream.isWriting )
		{
			position = transform.position;
			stream.Serialize( ref position );
		}
		else
		{
			stream.Serialize( ref position );
			transform.position = position;
		}
	}
}
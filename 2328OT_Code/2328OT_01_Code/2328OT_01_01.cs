using UnityEngine;
using System.Collections;

public class ExampleUnityNetworkSerializePosition : MonoBehaviour
{
	public void OnSerializeNetworkView( BitStream stream, NetworkMessageInfo info )
	{
		// we are currently writing information to the network
		if( stream.isWriting )
		{
			// send the object's position
			Vector3 position = transform.position;
			stream.Serialize( ref position );
		}
		// we are currently reading information from the network
		else
		{
			// read the vector3 and store it in position
			Vector3 position = Vector3.zero;
			stream.Serialize( ref position );
			
			// set object's position to the value we just received
			transform.position = position;
		}
	}
}
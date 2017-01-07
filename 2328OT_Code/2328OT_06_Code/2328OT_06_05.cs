using UnityEngine;
using System.Collections;

public class NetworkedPlayer : MonoBehaviour
{
	private Vector3 lastReceivedPosition;
	
	void Start()
	{
		lastReceivedPosition = transform.position;
	}
	
	void Update()
	{
		if( !networkView.isMine )
		{
			transform.position = Vector3.Lerp( transform.position, lastReceivedPosition, Time.deltaTime * 10f );
		}
	}

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
			lastReceivedPosition = position;
		}
	}
}
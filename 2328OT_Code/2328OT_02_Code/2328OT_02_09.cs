using UnityEngine;
using System.Collections;

public class Example_PhotonView : Photon.MonoBehaviour
{
	void Update()
	{
		if( photonView.isMine )
		{
			// if the space key is pressed, and this photon view belongs to the local player, call the RPC
			if( Input.GetKeyDown( KeyCode.Space ) )
			{
				photonView.RPC( "TestRPC", PhotonTargets.All );
			}
		}
	}

	[RPC]
	void TestRPC()
	{
		Debug.Log( "An RPC was called!" );
	}

	void OnPhotonSerializeView( PhotonStream stream, PhotonMessageInfo info )
	{
		if( stream.isWriting )
		{
			// writing to the stream, send position
			stream.SendNext( transform.position );
		}
		else
		{
			// reading from the stream, get position
			transform.position = (Vector3)stream.ReceiveNext();
		}
	}
}
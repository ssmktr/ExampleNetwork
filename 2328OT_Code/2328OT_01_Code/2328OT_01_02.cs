using UnityEngine;
using System.Collections;

public class ExampleUnityNetworkCallRPC : MonoBehaviour
{
	void Update()
	{
		// important – make sure not to run if this networkView is not ours
		if( !networkView.isMine )
			return;
		
		// if space key is pressed, call RPC for everybody
		if( Input.GetKeyDown( KeyCode.Space ) )
			networkView.RPC( "testRPC", RPCMode.All );
	}

	[RPC]
	void testRPC( NetworkMessageInfo info )
	{
		// log the IP address of the machine that called this RPC
		Debug.Log( "Test RPC called from " + info.sender.ipAddress );
	}
}
using UnityEngine;
using System.Collections;

public class NetworkPlayer : MonoBehaviour
{
	private Transform camTransform;

	private Vector3 lastReceivedPos;
	private Quaternion lastReceivedRot;
	private Quaternion lastReceivedCamRot;

	void Start()
	{
		// get camera transform
		camTransform = GetComponentInChildren<Camera>().transform;

		// if this doesn’t belong to the local player, disable input and camera
		if( !networkView.isMine )
		{
			GetComponent<FPSInputController>().enabled = false;
			camTransform.camera.enabled = false;
		}
	}

	void Update()
	{
		// interpolate towards last received network state
		if( !networkView.isMine )
		{
			transform.position = Vector3.Lerp( transform.position, lastReceivedPos, Time.deltaTime * 10f );
			transform.rotation = Quaternion.Slerp( transform.rotation, lastReceivedRot, Time.deltaTime * 10f );
			camTransform.localRotation = Quaternion.Slerp( camTransform.localRotation, lastReceivedCamRot, Time.deltaTime * 10f );
		}
	}

	void OnSerializeNetworkView( BitStream stream, NetworkMessageInfo info )
	{
		if( stream.isWriting )
		{
			// serialize position and rotation
			Vector3 position = transform.position;
			Quaternion rotation = transform.rotation;
			Quaternion camRotation = camTransform.localRotation;
			
			stream.Serialize( ref position );
			stream.Serialize( ref rotation );
			stream.Serialize( ref camRotation );
		}
		else
		{
			// deserialize position and rotation
			Vector3 position = Vector3.zero;
			Quaternion rotation = Quaternion.identity;
			Quaternion camRotation = Quaternion.identity;

			stream.Serialize( ref position );
			stream.Serialize( ref rotation );
			stream.Serialize( ref camRotation );

			// store values to be interpolated towards
			lastReceivedPos = position;
			lastReceivedRot = rotation;
			lastReceivedCamRot = camRotation;
		}
	}
}
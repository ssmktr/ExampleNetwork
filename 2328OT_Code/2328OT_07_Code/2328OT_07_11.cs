using UnityEngine;
using System.Collections;

public class NetworkPlayer : MonoBehaviour
{
	private struct networkState
	{
		public Vector3 Position;
		public Vector3 Rotation;
		public Vector3 CamRotation;
		public double Timestamp;
	}

	private Transform camTransform;

	private Vector3 lastReceivedPos;
	private Quaternion lastReceivedRot;
	private Quaternion lastReceivedCamRot;
	
	private networkState[] stateBuffer = new networkState[20];
	private int stateCount = 0;

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
	
	private Vector3 lastPosition;
	private Quaternion lastRotation;
	private Quaternion lastCamRotation;
	public void Rewind( double timestamp )
	{
		// we’re going to rewind to a given position in time, in general this would be the timestamp value of a network message.

		// first, temporarily store the current state. We need this for when we “undo” the rewind.
		lastPosition = transform.position;
		lastRotation = transform.rotation;
		lastCamRotation = camTransform.localRotation;	

		// check if we have enough states to perform a proper rewind
		// if not, return
		if( stateCount <= 1 ) return;

		// check if we have any packets older than the timestamp
		// if not, clamp the timestamp to whatever range we have.
		if( stateBuffer[stateCount-1].Timestamp > timestamp ) timestamp = stateBuffer[stateCount-1].Timestamp;

		// network states on either side of the target timestamp
		networkState lhs, rhs;

		// find the first state with timestamp <= target rewind time, as well as the very next state
		for( int i = 0; i < stateCount; i++ )
		{
			// is this state older than the given timestamp?
			if( stateBuffer[i].Timestamp <= timestamp )
			{
				// if the newest state we have is already older than the target timestamp, we just don’t have enough information to provide a proper rewind, we’ll just return instead.
				if( i == 0 )
					return;


				// store the state, and the next, which we will interpolate between
				lhs = stateBuffer[i];
				rhs = stateBuffer[i-1];
				break;
			}
		}

		// calculate the total time difference between each packet
		// we’ll use this to calculate a normalized time value between 0 and 1 for interpolation.
		double interpLen = rhs.Timestamp – lhs.Timestamp;

		// calculate the time difference between the first packet and the target timestamp;
		double diff = timestamp – lhs.Timestamp;

		// diff will range between 0 (if it is equal to the first packet’s timestamp) and ‘interpLen’ (if it is equal to the second packet’s timestamp). Dividing it by ‘interpLen’ will produce a value between 0 and 1
		double t = diff / interpLen;

		// interpolate between both packets
		transform.position = Vector3.Lerp( lhs.Position, rhs.Position, t );
		transform.rotation = Quaternion.Slerp( lhs.Rotation, rhs.Rotation, t );
		camTransform.localRotation = Quaternion.Slerp( lhs.CamRotation, rhs.CamRotation, t );
	}

	void Restore()
	{
		// restore the player to the state which was stored in Rewind.
		transform.position = lastPosition;
		transform.rotation = lastRotation;
		camTransform.localRotation = lastCamRotation;
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
			
			// buffer network state
			networkState state = new networkState();
			state.Position = position;
			state.Rotation = rotation;
			state.CamRotation = camRotation;
			state.Timestamp = info.timestamp;

			bufferState( state );
		}
	}
	
	void bufferState( networkState state )
	{
		// shift states
		// the state at index 0 moves to index 1, 1 moves to 2, etc
		// the state at index 20 is deleted
		for( int i = stateBuffer.Length – 1; i > 0; i-- )
		{
			stateBuffer[i] = stateBuffer[i-1];
		}

		// insert newest state at 0
		stateBuffer[0] = state;

		// increment state count, up to a maximum of 20
		stateCount = Mathf.Max( stateBuffer.Length, stateCount + 1 );
	}
}
using UnityEngine;
using System.Collections;

public class Paddle : MonoBehaviour
{
	// how fast the paddle can move
	public float MoveSpeed = 10f;

	// how far up and down the paddle can move
	public float MoveRange = 10f;

	// whether this paddle can accept player input
	public bool AcceptsInput = true;

	// the position read from the network
	// used for interpolation
	private Vector3 readNetworkPos;

	void Start()
	{
		// if this is our paddle, it accepts input
		// otherwise, if it is someone else�s paddle, it does not
		AcceptsInput = NetworkView.isMine;
	}

	void Update()
	{
		// does not accept input, interpolate network pos
		if( !AcceptsInput )
		{
			transform.position = Vector3.Lerp( transform.position, readNetworkPos, 10f * Time.deltaTime );

			// don�t use player input
			return;
		}

		//get user input
		float input = Input.GetAxis( "Vertical" );
		
		// move paddle
		Vector3 pos = transform.position;
		pos.z += input * MoveSpeed * Time.deltaTime;

		// clamp paddle position
		pos.z = Mathf.Clamp( pos.z, -MoveRange, MoveRange );

		// set position
		transform.position = pos;
	}

	void OnSerializeNetworkView( BitStream stream )
	{
		// writing information, push current paddle position
		if( stream.isWriting )
		{
			Vector3 pos = transform.position;
			stream.Serialize( ref pos );
		}
		// reading information, read paddle position
		else
		{
			Vector3 pos = Vector3.zero;
			stream.Serialize( ref pos );
			readNetworkPos = pos;
		}
	}
}
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class NetworkedPlayer : MonoBehaviour
{
	// how far back to rewind interpolation?
	public float InterpolationBackTime = 0.1f;

	// a snapshot of values received over the network
	private struct networkState
	{
		public Vector3 Position;
		public double Timestamp;

		public networkState( Vector3 pos, double time )
		{
			this.Position = pos;
			this.Timestamp = time;
		}
	}

	// represents a move command sent to the server
	private struct move
	{
		public float HorizontalAxis;
		public float VerticalAxis;
		public double Timestamp;

		public move( float horiz, float vert, double timestamp )
		{
			this.HorizontalAxis = horiz;
			this.VerticalAxis = vert;
			this.Timestamp = timestamp;
		}
	}

	// we'll keep a buffer of 20 states
	networkState[] stateBuffer = new networkState[ 20 ];
	int stateCount = 0; // how many states have been recorded

	// a history of move commands sent from the client to the server
	List<move> moveHistory = new List<move>();

	Player playerScript;

	void Start()
	{
		playerScript = GetComponent<Player>();
	}

	void FixedUpdate()
	{
		if( networkView.isMine )
		{
			// get current move state
			move moveState = new move( playerScript.horizAxis, playerScript.vertAxis, Network.time );

			// buffer move state
			moveHistory.Insert( 0, moveState );

			// cap history at 200
			if( moveHistory.Count > 200 )
			{
				moveHistory.RemoveAt( moveHistory.Count - 1 );
			}

			// simulate
			playerScript.Simulate();

			// send state to server
			networkView.RPC( "ProcessInput", RPCMode.Server, moveState.HorizontalAxis, moveState.VerticalAxis, transform.position );
		}
	}

	[RPC]
	void ProcessInput( float horizAxis, float vertAxis, Vector3 position, NetworkMessageInfo info )
	{
		if( networkView.isMine )
			return;

		if( !Network.isServer )
			return;

		// execute input
		playerScript.horizAxis = horizAxis;
		playerScript.vertAxis = vertAxis;
		playerScript.Simulate();

		// compare results
		if( Vector3.Distance( transform.position, position ) > 0.1f )
		{
			// error is too big, tell client to rewind and replay
			networkView.RPC( "CorrectState", info.sender, transform.position );
		}
	}

	[RPC]
	void CorrectState( Vector3 correctPosition, NetworkMessageInfo info )
	{
		// find past state based on timestamp
		int pastState = 0;
		for( int i = 0; i < moveHistory.Count; i++ )
		{
			if( moveHistory[ i ].Timestamp <= info.timestamp )
			{
				pastState = i;
				break;
			}
		}

		// rewind
		transform.position = correctPosition;

		// replay
		for( int i = 0; i <= pastState; i++ )
		{
			playerScript.horizAxis = moveHistory[ i ].HorizontalAxis;
			playerScript.vertAxis = moveHistory[ i ].VerticalAxis;
			playerScript.Simulate();
		}

		// clear
		moveHistory.Clear();
	}

	private float updateTimer = 0f;
	void Update()
	{
		// is this the server? send out position updates every 1/10 second
		if( Network.isServer )
		{
			updateTimer += Time.deltaTime;
			if( updateTimer >= 0.1f )
			{
				updateTimer = 0f;
				networkView.RPC( "netUpdate", RPCMode.Others, transform.position );
			}
		}

		if( networkView.isMine ) return; // don't run interpolation on the local object
		if( stateCount == 0 ) return; // no states to interpolate

		double currentTime = Network.time;
		double interpolationTime = currentTime - InterpolationBackTime;

		// the latest packet is newer than interpolation time - we have enough packets to interpolate
		if( stateBuffer[ 0 ].Timestamp > interpolationTime )
		{
			for( int i = 0; i < stateCount; i++ )
			{
				// find the closest state that matches network time, or use oldest state
				if( stateBuffer[ i ].Timestamp <= interpolationTime || i == stateCount - 1 )
				{
					// the state closest to network time
					networkState lhs = stateBuffer[ i ];

					// the state one slot newer
					networkState rhs = stateBuffer[ Mathf.Max( i - 1, 0 ) ];

					// use time between lhs and rhs to interpolate
					double length = rhs.Timestamp - lhs.Timestamp;
					float t = 0f;
					if( length > 0.0001 )
					{
						t = (float)( ( interpolationTime - lhs.Timestamp ) / length );
					}

					transform.position = Vector3.Lerp( lhs.Position, rhs.Position, t );
					break;
				}
			}
		}
	}

	[RPC]
	void netUpdate( Vector3 position, NetworkMessageInfo info )
	{
		if( !networkView.isMine )
		{
			bufferState( new networkState( position, info.timestamp ) );
		}
	}

	// save new state to buffer
	void bufferState( networkState state )
	{
		// shift buffer contents to accomodate new state
		for( int i = stateBuffer.Length - 1; i > 0; i-- )
		{
			stateBuffer[ i ] = stateBuffer[ i - 1 ];
		}

		// save state to slot 0
		stateBuffer[ 0 ] = state;

		// increment state count (up to buffer size)
		stateCount = Mathf.Min( stateCount + 1, stateBuffer.Length );
	}
}
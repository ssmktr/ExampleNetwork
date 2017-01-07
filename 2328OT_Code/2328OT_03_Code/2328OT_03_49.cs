using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
	public float MoveSpeed = 5f;

	private OwnerInfo ownerInfo;
	private GameActor actorInfo;
	private bool isMine = false;

	private Vector3 lastReceivedMove;

	private float timeOfLastMoveCmd = 0f;

	void Start()
	{
		timeOfLastMoveCmd = Time.time;

		lastReceivedMove = transform.position;

		ownerInfo = GetComponent<OwnerInfo>();
		actorInfo = GetComponent<GameActor>();
		isMine = ( ownerInfo.OwnerID == StarCollectorClient.PlayerID );
	}

	void Update()
	{
		if( isMine )
		{
			// get movement direction
			float mX = Input.GetAxis( "Horizontal" ) * MoveSpeed;
			float mY = Input.GetAxis( "Vertical" ) * MoveSpeed;

			if( Time.time >= timeOfLastMoveCmd + 0.1f )
			{
				timeOfLastMoveCmd = Time.time;

				// send move command to server every 0.1 seconds
				Dictionary<byte, object> moveParams = new Dictionary<byte, object>();
				moveParams[ 0 ] = actorInfo.ActorID;
				moveParams[ 1 ] = mX;
				moveParams[ 2 ] = mY;
				StarCollectorClient.Connection.OpCustom( (byte)StarCollectorRequestTypes.MoveCommand, moveParams, false );
			}
		}

		// lerp toward last received position
		transform.position = Vector3.Lerp( transform.position, lastReceivedMove, Time.deltaTime * 20f );
	}

	void UpdatePosition( Vector3 newPos )
	{
		lastReceivedMove = newPos;
	}
}
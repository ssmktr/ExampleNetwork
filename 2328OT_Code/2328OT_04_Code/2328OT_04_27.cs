using UnityEngine;
using System.Collections;

public class BotScript : MonoBehaviour
{
	public static BotScript SelectedBot;

	private Vector3 targetMovePos;
	private BotInfo targetEnemy;

	private float attackTimer = 0f;

	void Awake()
	{
		targetMovePos = Vector3.zero;
	}

	void Update()
	{
		// left mouse button pressed?
		if( Input.GetMouseButtonDown( 0 ) )
		{
			// raycast
			RaycastHit hit;
			if( Physics.Raycast( Camera.main.ScreenPointToRay( Input.mousePosition ), out hit ) )
			{
				BotInfo hitBot = hit.collider.GetComponent<BotInfo>();
				if( hitBot != null && hitBot.IsMine )
				{
					// select bot
					SelectedBot = hitBot.GetComponent<BotScript>();
				}
			}
		}

		if( !GetComponent<BotInfo>().IsMine )
			return;

// current target not null? move towards and attack
		if( targetEnemy != null )
		{
			moveTowards( targetEnemy.transform.position, 2f );
			targetMovePos = transform.position;

			// close enough to target? attack
			if( Vector3.Distance( transform.position, targetEnemy.transform.position ) <= 2f )
			{
				attackTimer += Time.deltaTime;
				if( attackTimer >= 1f )
				{
					attackTimer = 0f;
					// send damage message
					NetworkUtils.connection.Send( "TakeDamage", targetEnemy.BotID );
				}
			}
		}
		else
		{
			moveTowards( targetMovePos, 0.5f );
		}

		if( SelectedBot != this )
			return;

		// right mouse button pressed?
		if( Input.GetMouseButtonDown( 1 ) )
		{
			// raycast
			RaycastHit hit;
			if( Physics.Raycast( Camera.main.ScreenPointToRay( Input.mousePosition ), out hit ) )
			{
				BotInfo hitBot = hit.collider.GetComponent<BotInfo>();
				if( hitBot != null && !hitBot.IsMine )
				{
					// target selected bot
					targetEnemy = hitBot;
				}
				else if( hitBot == null )
				{
					// move to position
					targetMovePos = hit.point;
				}
			}
		}
	}

	// move to within a certain distance of the target
	void moveTowards( Vector3 pos, float range )
	{
		if( Vector3.Distance( transform.position, pos ) > range )
		{
			// move toward at 5 meters per second
			transform.position = Vector3.MoveTowards( transform.position, pos, Time.deltaTime * 5f );
		}
	}
}
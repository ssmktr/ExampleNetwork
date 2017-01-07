using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour
{
	// the speed the ball starts with
	public float StartSpeed = 5f;

	// the maximum speed of the ball
	public float MaxSpeed = 20f;

	// how much faster the ball gets with each bounce
	public float SpeedIncrease = 0.25f;

	// the current speed of the ball
	private float currentSpeed;

	// the current direction of travel
	private Vector2 currentDir

	// whether or not the ball is resetting
	private bool resetting = false;

	void Start()
	{
		// initialize starting speed
		currentSpeed = StartSpeed;

		// initialize direction
		currentDir = Random.insideUnitCircle.normalized;
	}

	void Update()
	{
		// don’t move the ball if it’s resetting
		if( resetting )
			return;

		// move the ball in the current direction
		Vector2 moveDir = currentDir * currentSpeed * Time.deltaTime;
		transform.Translate( new Vector3( moveDir.x, 0f, moveDir.y ) );
	}

	void OnTriggerEnter( Collider other )
	{
		if( other.tag == "Boundary" )
		{
			// vertical boundary, reverse Y direction
			currentDir.y *= -1;
		}
		else if( other.tag == "Player" )
		{
			// player paddle, reverse X direction
			currentDir.x *= -1;
		}
		else if( other.tag == "Goal" )
		{
			// reset the ball
			StartCoroutine( resetBall() );
			// inform goal of the score
			other.SendMessage( "GetPoint", SendMessageOptions.DontRequireReceiver );
		}

		// increase speed
		currentSpeed += SpeedIncrease;
		
		// clamp speed to maximum
		currentSpeed = Mathf.Clamp( currentSpeed, StartSpeed, MaxSpeed );
	}

	IEnumerator resetBall()
	{
		// reset position, speed, and direction
		resetting = true;
		transform.position = Vector3.zero;
		
		currentDir = Vector3.zero;
		currentSpeed = 0f;

		// wait for 3 seconds before starting the round
		yield return new WaitForSeconds( 3f );

		Start();

		resetting = false;
	}
}
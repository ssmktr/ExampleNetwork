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

	void Update()
	{
		// does not accept input, abort
		if( !AcceptsInput )
			return;

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
}
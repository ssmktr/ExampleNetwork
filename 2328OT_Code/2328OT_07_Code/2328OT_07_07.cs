using UnityEngine;
using System.Collections;

public class SpawnScript : MonoBehaviour
{
	public GameObject PlayerObject;

	void Start()
	{
		// spawn the player object
		Network.Instantiate( PlayerObject, transform.position, transform.rotation, 0 );
	}
}
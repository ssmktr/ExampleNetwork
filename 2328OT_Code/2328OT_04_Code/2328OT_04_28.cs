using UnityEngine;
using System.Collections;

public class SpawnBots : MonoBehaviour
{
	public GameObject BotPrefab;

	public Transform[] SpawnPoints;

	private int lastGeneratedBotID = 0;

	IEnumerator Start()
	{
		// 3 second prepare time
		yield return new WaitForSeconds( 3f );

		foreach( Transform spawn in SpawnPoints )
		{
			ulong botID = AllocateBotID();

			GameObject bot = (GameObject)Instantiate( BotPrefab, new Vector3( spawn.position.x, 0f, spawn.position.z ), Quaternion.identity );
			bot.GetComponent<BotInfo>().OwnerID = NetworkUtils.localPlayerID;
			bot.GetComponent<BotInfo>().BotID = botID;

			// send spawn message to server
			NetworkUtils.connection.Send( "SpawnBot", botID, spawn.position.x, spawn.position.z );
		}
	}

	ulong AllocateBotID()
	{
		// here, we will generate a unique network ID for one of our bots
		// to do this without server intervention, the ID will be based on the Player ID (which is guaranteed to be unique per player)
		// four of the bytes will be player ID, and the other four bytes will be bot instance ID
		ulong id = (ulong)lastGeneratedBotID++;				// int fills four bytes, leaving a remaining four
		id |= ( (ulong)NetworkUtils.localPlayerID << 4 );	// shift player ID to fill the remaining four bytes

		return id;
	}
}
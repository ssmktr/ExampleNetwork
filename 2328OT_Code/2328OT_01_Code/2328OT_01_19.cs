using UnityEngine;
using System.Collections;

public class Scorekeeper : MonoBehaviour
{
	// the maximum score a player can reach
	public int ScoreLimit = 10;

	// the start points for each player paddle
	public Transform SpawnP1;
	public Transform SpawnP2;

	// the paddle prefab
	public GameObject paddlePrefab;

	// the display test for player 1�s score
	public TextMesh Player1ScoreDisplay;

	// the display text for player 2�s score
	public TextMesh Player2ScoreDisplay;

	// Player 1�s score
	private int p1score = 0;

	// Player 2�s score
	private int p2score = 0;

	void Start()
	{
		if( Network.isServer )
		{
			// server doesn�t trigger OnPlayerConnected, manually spawn
			Network.Instantiate( paddlePrefab, SpawnP1.position, Quaternion.identity, 0 );

			// nobody has joined yet, display "Waiting..." for player 2
			Player2ScoreDisplay.text = "Waiting...";
		}
	}

void OnPlayerConnected( NetworkPlayer player )
	{
		// when a player joins, tell them to spawn
		networkView.RPC( "net_DoSpawn", player, SpawnP2.position );

		// change player 2�s score display from "waiting..." to "0"
		Player2ScoreDisplay.text = "0";
	}

	void OnPlayerDisconnected( NetworkPlayer player )
	{
		// player 2 left, reset scores
		p1Score = 0;
		p2Score = 0;

		// display each player�s scores
		// display "Waiting..." for player 2
		Player1ScoreDisplay.text = p1Score.ToString();
		Player2ScoreDisplay.text = "Waiting...";
	}

	void OnDisconnectedFromServer( NetworkDisconnection cause )
	{
		// go back to the main menu
		Application.LoadLevel( "Menu" );
	}

	[RPC]
	void net_DoSpawn( Vector3 position )
	{
		// spawn the player paddle
		Network.Instantiate( paddlePrefab, position, Quaternion.identity, 0 );
	}

	// call an RPC to give the player a point
	public void AddScore( int player )
	{
		NetworkView.RPC( "net_AddScore", RPCMode.All, player );
	}

	// give the appropriate player a point
	[RPC]
	public void net_AddScore( int player )
	{
		// player 1
		if( player == 1 )
		{
			p1score++;
		}
		// player 2
		else if( player == 2 )
		{
			p2score++;
		}

		// check if either player reached the score limit
		if( p1score >= ScoreLimit || p2score >= ScoreLimit )
		{
			// player 1 has a better score than player 2
			if( p1score > p2score )
				Debug.Log( "Player 1 wins" );
			// player 2 has a better score than player 1
			if( p2score > p1score )
				Debug.Log( "Player 2 wins" );
			// both players have the same score - tie
			else
				Debug.Log( "Players are tied" );

			// reset scores and start over
			p1score = 0;
			p2score = 0;
		}

		// display each player�s score
		Player1ScoreDisplay.text = p1score.ToString();
		Player2ScoreDisplay.text = p2score.ToString();
	}
}
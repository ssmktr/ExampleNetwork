using UnityEngine;
using System.Collections;

public class Scorekeeper : MonoBehaviour
{
	// the maximum score a player can reach
	public int ScoreLimit = 10;

	// the display test for player 1’s score
	public TextMesh Player1ScoreDisplay;

	// the display text for player 2’s score
	public TextMesh Player2ScoreDisplay;

	// Player 1’s score
	private int p1score = 0;

	// Player 2’s score
	private int p2score = 0;

	// give the appropriate player a point
	public void AddScore( int player )
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

		// display each player’s score
		Player1ScoreDisplay.text = p1score.ToString();
		Player2ScoreDisplay.text = p2score.ToString();
	}
}
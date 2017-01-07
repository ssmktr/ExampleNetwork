using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using PlayerIO.GameLibrary;
using System.Drawing;

namespace BotWarsGame
{
	public class Player : BasePlayer
	{
		public string Name;

		// the bots this player owns
		public List<Bot> OwnedBots = new List<Bot>();
	}

	[RoomType( "GameRoom" )]
	public class GameCode : Game<Player>
	{
		// This method is called when an instance of your the game is created
		public override void GameStarted()
		{
		}

		// This method is called when the last player leaves the room, and it's closed down.
		public override void GameClosed()
		{
		}

		// This method is called whenever a player joins the game
		public override void UserJoined( Player player )
		{
			player.Name = player.JoinData[ "Name" ];

			// send the player their own ID
			player.Send( "SetID", player.Id );

			// inform everyone that this user has joined
			Broadcast( "UserJoined", player.Id, player.Name );

			// inform the user of everyone else in the room,
			foreach( Player p in Players )
			{
				if( p == player )
					continue;

				player.Send( "UserJoined", player.Id, player.Name );
			}
		}

		// This method is called when a player leaves the game
		public override void UserLeft( Player player )
		{
			Broadcast( "UserLeft", player.Id );
		}

		// This method is called when a player sends a message into the server code
		public override void GotMessage( Player player, Message message )
		{
		}
	}
}
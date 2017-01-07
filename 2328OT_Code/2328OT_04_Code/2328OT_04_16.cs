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
			// plus their bots
			foreach( Player p in Players )
			{
				if( p == player )
					continue;

				player.Send( "UserJoined", player.Id, player.Name );

				// notify new player of existing bots
				foreach( Bot bot in p.OwnedBots )
				{
					player.Send( "OnBotSpawned", p.Id, bot.BotID, bot.PositionX, bot.PositionY );
				}
			}
		}

		// This method is called when a player leaves the game
		public override void UserLeft( Player player )
		{
			Broadcast( "UserLeft", player.Id );
		}
		
		// This method is called when a player sends a message into the server code
		Dictionary<ulong,Bot> bots = new Dictionary<ulong,Bot>();
		public override void GotMessage( Player player, Message message )
		{
			switch( message.Type )
			{
				case "SpawnBot":
				{
					// player spawned a bot
					ulong botID = message.GetULong( 0 );
					float botPosX = message.GetFloat( 1 );
					float botPosY = message.GetFloat( 2 );
					Bot bot = new Bot( player, botID );
					bot.PositionX = botPosX;
					bot.PositionY = botPosY;
					player.OwnedBots.Add( bot );

					// add bot to dictionary so we can later look up bots by ID
					bots.Add( botID, bot );
					
					// broadcast spawn message to other players
					foreach( Player pl in Players )
					{
						if( pl == player )
							continue;

						pl.Send( "OnBotSpawned", pl.Id, botID, botPosX, botPosY );
					}
				}
					break;
			}
		}
	}
}
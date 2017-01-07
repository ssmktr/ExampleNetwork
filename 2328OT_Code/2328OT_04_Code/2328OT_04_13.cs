using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BotWarsGame
{
	public class Bot
	{
		public ulong BotID;
		public int OwnerID;

		public float PositionX = 0f;
		public float PositionY = 0f;

		public int Health = 100;

		public Bot( Player owner, ulong botID )
		{
			this.OwnerID = owner.Id;
			this.BotID = botID;
		}
	}
}
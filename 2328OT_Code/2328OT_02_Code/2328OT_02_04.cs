using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class Example_FilterRooms
{
	public static RoomInfo[] FilterRooms( RoomInfo[] src, bool includeFull, Hashtable properties )
	{
		// use a Where expression to filter out rooms that do not match the given criteria
		// then convert that to an array
		return src.Where( room =>
			(
				filterRoom( room, includeFull, properties )
			) ).ToArray();
	}

	private static bool filterRoom( RoomInfo src, bool includeFull, Hashtable properties )
	{
		// if includeFull is false, filter out the room if it's full
		bool include_full = ( src.playerCount >= src.maxPlayers || includeFull );

		// compare each custom property in the room for a match
		bool include_props = true;

		if( properties != null )
		{
			foreach( object key in properties )
			{
				// does not contain the key, therefore doesn't match our criteria
				if( !src.customProperties.ContainsKey( key ) )
				{
					include_props = false;
					break;
				}

				// value of key does not match, therefore doesn't match our criteria
				if( src.customProperties[ key ] != properties[ key ] )
				{
					include_props = false;
					break;
				}
			}
		}

		return include_full && include_props;
	}
}
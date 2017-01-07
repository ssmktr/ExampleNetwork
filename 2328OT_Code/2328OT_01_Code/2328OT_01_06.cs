using UnityEngine;
using System.Collections;

public class ExampleUnityNetworkingBrowseServers : MonoBehaviour
{
	// are we currently trying to download a host list?
	private bool loading = false;

	// the current position within the scrollview
	private Vector2 scrollPos = Vector2.zero;

	void Start()
	{
		// immediately request a list of hosts
		refreshHostList();
	}

	void OnGUI()
	{
		if( GUILayout.Button( "Refresh" ) )
		{
			refreshHostList();
		}

		if( loading )
		{
			GUILayout.Label( "Loading..." );
		}
		else
		{
			scrollPos = GUILayout.BeginScrollView( scrollPos, GUILayout.Width( 200f ), GUILayout.Height( 200f ) );

			HostData[] hosts = MasterServer.PollHostList();
			for( int i = 0; i < hosts.Length; i++ )
			{
				if( GUILayout.Button( hosts[i].gameName, GUILayout.ExpandWidth( true ) ) )
				{
					Network.Connect( hosts[i] );
				}
			}

			if( hosts.Length == 0 )
			{
				GUILayout.Label( "No servers running" );
			}

			GUILayout.EndScrollView();
		}
	}

	void refreshHostList()
	{
		// let the user know we are awaiting results from the master server
		loading = true;
		MasterServer.ClearHostList();
		MasterServer.RequestHostList( "GameTypeNameHere" );
	}

	// this is called when the Master Server reports an event to the client – for example, server registered successfully, host list received, etc
	void OnMasterServerEvent( MasterServerEvent msevent )
	{
		if( msevent == MasterServerEvent.HostListReceived )
		{
			// received the host list, no longer awaiting results
			loading = false;
		}
	}
}
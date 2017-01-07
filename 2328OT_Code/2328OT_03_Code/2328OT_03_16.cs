using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;

namespace StarCollectorDemo
{
	public class StarCollectorDemoApplication : ApplicationBase
	{
		protected override PeerBase CreatePeer( InitRequest initRequest )
		{
			return new StarCollectorDemoPeer( initRequest.Protocol, initRequest.PhotonPeer );
		}

		protected override void Setup()
		{
			StarCollectorDemoGame.Instance = new StarCollectorDemoGame();
			StarCollectorDemoGame.Instance.Startup();
		}

		protected override void TearDown()
		{
			StarCollectorDemoGame.Instance.Shutdown();
		}
	}
}
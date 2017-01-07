using Photon.SocketServer;

public class PhotonAckServer : ApplicationBase
{
	protected override PeerBase CreatePeer( InitRequest initRequest )
	{
		return new PhotonAckPeer( initRequest.Protocol, initRequest.PhotonPeer );
	}

	protected override void Setup()
	{
	}

	protected override void TearDown()
	{
	}
}
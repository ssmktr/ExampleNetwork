using UnityEngine;
using System.Collections;

public class ExampleUnityNetworkInitializeServer : MonoBehaviour {

    public UILabel NetworkStateLbl;

    string ip = "";
    string port = "";
    string password = "";

    void Start()
    {
        NetworkStateLbl.text = "Ready";
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 110, 150, 50), "Launch Server"))
        {
            LaunchServer();
        }

        GUI.Label(new Rect(110, 170, 150, 50), "IP Address");
        ip = GUI.TextField(new Rect(260, 170, 150, 50), ip);

        GUI.Label(new Rect(110, 230, 150, 30), "Port");
        port = GUI.TextField(new Rect(260, 230, 150, 50), port);

        GUI.Label(new Rect(110, 290, 150, 30), "Password");
        password = GUI.PasswordField(new Rect(260, 290, 150, 50), password, '*');

        if (GUI.Button(new Rect(110, 350, 150, 50), "Connect"))
        {
            int portNum = 2305;
            if (!int.TryParse(port, out portNum))
            {
                NetworkStateLbl.text = "Given port is not a number";
            }
            else
            {
                Network.Connect(ip, portNum, password);
            }
        }
    }

    private void OnConnectedToServer()
    {
        NetworkStateLbl.text = "Client On";
    }

    private void OnFailedToConnect(NetworkConnectionError error)
    {
        NetworkStateLbl.text = "Failed to connect to server : " + error.ToString();
    }

    void LaunchServer()
    {
        Network.InitializeServer(8, 2305, true);
    }

    private void OnServerInitialized()
    {
        NetworkStateLbl.text = "Server On";
    }
}
 
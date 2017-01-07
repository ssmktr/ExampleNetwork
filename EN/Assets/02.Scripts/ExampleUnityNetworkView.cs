using UnityEngine;
using System.Collections;

public class ExampleUnityNetworkView : MonoBehaviour {

    public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        if (stream.isWriting)
        {
            Vector3 position = transform.position;
            stream.Serialize(ref position);
        }
        else
        {
            Vector3 position = Vector3.zero;
            stream.Serialize(ref position);

            transform.position = position;
        }
    }
}

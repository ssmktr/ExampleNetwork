using UnityEngine;
using System.Collections;

public class DisableServerCamera : MonoBehaviour
{
#if SERVER
	void Update()
	{
		// culling mask is a bitmask – setting all bits to zero means render nothing
		camera.cullingMask = 0;
	}
#endif
}
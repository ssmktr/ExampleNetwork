using UnityEngine;
using System.Collections;

public class Paddle : MonoBehaviour {

    public float MoveSpeed = 10f;
    public float MoveRange = 4f;
    public bool AcceptInput = true;

	void Update () {
        if (!AcceptInput)
            return;

        float input = Input.GetAxis("Vertical");

        Vector3 pos = transform.position;
        pos.y += input * MoveSpeed * Time.deltaTime;
        pos.y = Mathf.Clamp(pos.y, -MoveRange, MoveRange);

        transform.position = pos;
	}
}

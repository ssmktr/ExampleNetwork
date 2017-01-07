using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {

    public float StartSpeed = 5f;
    public float MaxSpeed = 20f;
    public float SpeedIncrease = 0.25f;

    float CurSpeed = 0f;
    Vector2 CurDir = Vector2.zero;
    bool Resetting = false;

	void Start () {
        CurSpeed = StartSpeed;
        CurDir = Random.insideUnitCircle.normalized;
	}
	
	void Update () {
        if (Resetting)
            return;

        Vector2 MoveDir = CurDir * CurSpeed * Time.deltaTime;
        transform.Translate(new Vector3(MoveDir.x, MoveDir.y, 0));
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Boundary")
        {
            CurDir.y *= -1;
        }
        else if (other.tag == "Player")
        {
            CurDir.x *= -1;
        }
        else if (other.tag == "Goal")
        {
            StartCoroutine(ResetBall());
            other.SendMessage("GetPoint", SendMessageOptions.DontRequireReceiver);
        }

        CurSpeed += SpeedIncrease;
        CurSpeed = Mathf.Clamp(CurSpeed, StartSpeed, MaxSpeed);
    }

    IEnumerator ResetBall()
    {
        Resetting = true;
        transform.position = Vector3.zero;

        CurDir = Vector3.zero;
        CurSpeed = 0f;

        yield return new WaitForSeconds(3f);

        Start();
        Resetting = false;
    }
}

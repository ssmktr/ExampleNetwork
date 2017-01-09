using UnityEngine;
using UnityEngine.Networking;

public class PlayerMove : NetworkBehaviour {

    public GameObject BulletPrefab;

    float MoveSpeed = 3f;

    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.red;
    }

    void Update () {

        if (!isLocalPlayer)
            return;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        transform.Translate(x * MoveSpeed * Time.deltaTime, 0, z * MoveSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdFire();
        }
	}

    [Command]
    void CmdFire()
    {
        GameObject Bullet = (GameObject)Instantiate(BulletPrefab);
        Bullet.transform.position = new Vector3((transform.position - transform.forward).x, (transform.position - transform.forward).y, (transform.position - transform.forward).z - 1f);
        Bullet.transform.rotation = Quaternion.identity;
        Bullet.GetComponent<Rigidbody>().velocity = -transform.forward * 4;

        NetworkServer.Spawn(Bullet);

        Destroy(Bullet, 2f);
    }
}

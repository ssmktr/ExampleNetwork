using UnityEngine;
using UnityEngine.Networking;

public class EnemySpawner : NetworkBehaviour {

    public GameObject EnemyPrefab;
    public int NumEnemies;

    public override void OnStartServer()
    {
        for (int i = 0; i < NumEnemies; ++i)
        {
            GameObject Enemy = (GameObject)Instantiate(EnemyPrefab);
            Enemy.transform.position = new Vector3(Random.Range(-8f, 8f), 0.2f, Random.Range(-8f, 8f));
            Enemy.transform.rotation = Quaternion.identity;

            NetworkServer.Spawn(Enemy);
        }
    }
}

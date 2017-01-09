using UnityEngine;
using UnityEngine.Networking;

public class Combat : NetworkBehaviour {

    public const int MaxHp = 100;
    public bool DestroyOnDeath = false;

    [SyncVar]
    public int Hp = MaxHp;

    public void TakeDamage(int amount)
    {
        if (!isServer)
            return;

        Hp -= amount;
        if (Hp <= 0)
        {
            if (DestroyOnDeath)
            {
                Destroy(gameObject);
            }
            else
            {
                Hp = MaxHp;
                RpcRespawn();
                Debug.Log("Dead!");
            }
        }
    }

    [ClientRpc]
    void RpcRespawn()
    {
        if (isLocalPlayer)
        {
            transform.position = Vector3.zero;
        }
    }
}

using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    private void OnCollisionEnter(Collision collision)
    {
        GameObject hit = collision.gameObject;
        Combat hitCombat = hit.GetComponent<Combat>();
        if (hitCombat != null)
        {
            hitCombat.TakeDamage(10);
            Destroy(gameObject);
        }
    }
}

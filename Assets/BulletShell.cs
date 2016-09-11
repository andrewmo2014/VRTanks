using UnityEngine;
using System.Collections;

public class BulletShell : MonoBehaviour {
    public GameObject ExplosionPrefab;
    void OnTriggerEnter(Collider c)
    {
        c.gameObject.SendMessage("BulletHit", SendMessageOptions.DontRequireReceiver);
        Destroy((GameObject)Instantiate(ExplosionPrefab, this.transform.position, Quaternion.identity), 7f);
        Destroy(this.gameObject);
    }
}

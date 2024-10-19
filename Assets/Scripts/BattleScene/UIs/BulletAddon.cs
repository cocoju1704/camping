using UnityEngine;

public class BulletAddon : MonoBehaviour {
    public Rigidbody2D rigid;
     void Awake() {
        rigid = GetComponent<Rigidbody2D>();
    }
}
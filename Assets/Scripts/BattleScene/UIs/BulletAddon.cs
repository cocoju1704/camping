using UnityEngine;

public class BulletAddon : MonoBehaviour { // Decorator Pattern으로 총알에 다양한 속성 부여
    public Rigidbody2D rigid;
     void Awake() {
        rigid = GetComponent<Rigidbody2D>();
    }
}
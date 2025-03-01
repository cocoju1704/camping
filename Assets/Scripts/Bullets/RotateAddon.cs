using UnityEngine;

public class RotateAddon : BulletAddon { // 투사체 회전 속성 부여
    public float rotateSpeed = 500f;
    void Update() {
        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
    }
}
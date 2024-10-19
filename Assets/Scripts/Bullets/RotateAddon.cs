using UnityEngine;

public class RotateAddon : BulletAddon {
    public float rotateSpeed = 500f;
    void Update() {
        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
    }
}
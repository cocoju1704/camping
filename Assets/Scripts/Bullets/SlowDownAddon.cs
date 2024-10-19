using UnityEngine;

public class SlowDownAddon : BulletAddon {
    public float slowDownRate  = 1f;
    public float disappearTime = 3f;
    public float timer = 0f;
    void OnEnable() {
        timer = 0f;
    }
    void FixedUpdate() {
        timer += Time.fixedDeltaTime;
        if (rigid.velocity.magnitude > 0) {
            rigid.velocity = rigid.velocity * (1 - slowDownRate * Time.fixedDeltaTime);
        }
        if (timer > disappearTime) {
            gameObject.SetActive(false);
        }
    }
}
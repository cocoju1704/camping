using UnityEngine;

public class SlowDownAddon : BulletAddon { // 투사체에 서서히 속도가 감소되는 속성 부여
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
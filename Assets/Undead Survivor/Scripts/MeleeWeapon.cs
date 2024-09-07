using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    public Bullet bullet;
    Collider2D bulletCollider;
    int cooldown;
    float swingTime = 0.05f;
    bool canSwing = true;
    void Awake() {
        bullet = GetComponentInChildren<Bullet>();
        bulletCollider = bullet.GetComponent<Collider2D>();
        cooldown = bullet.per;
    }
    void Update() {
        if (Input.GetMouseButtonDown(0) && canSwing) {
            StartCoroutine(Swing());
        }
    }
    IEnumerator Swing() {
        canSwing = false;
        bulletCollider.enabled = true;
        yield return new WaitForSeconds(swingTime);
        bulletCollider.enabled = false;
        yield return new WaitForSeconds(cooldown);
        canSwing = true;
    }
    IEnumerator SwingAnimation() {
        // 구현 예정
        yield return null;
    }
}

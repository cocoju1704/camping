using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DamageShower : MonoBehaviour {
    public enum Subject { Player, Enemy, Loot }
    public Subject subject;

    void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.CompareTag("Bullet") && !collision.CompareTag("Enemy") && !collision.CompareTag("EnemyBullet")) return;

        // 기본적으로 충돌한 총알의 damage 가져오기
        float damage = 0;
        Color color = Color.white;
        switch (subject) {
            case Subject.Player:
                if (collision.CompareTag("Enemy") || collision.CompareTag("EnemyBullet")) {
                    Bullet enemyBullet = collision.GetComponent<Bullet>();
                    if (enemyBullet != null) {
                        damage = enemyBullet.damage;
                        color = Color.red;
                    }
                } else {
                    return;
                }
                break;

            case Subject.Enemy:
                if (collision.CompareTag("Bullet")) {
                    Bullet bullet = collision.GetComponent<Bullet>();
                    if (bullet != null) {
                        damage = bullet.damage;
                        color = Color.white;
                    }
                } else {
                    return;
                }
                break;

            case Subject.Loot:
                if (collision.CompareTag("Bullet")) {
                    Bullet bullet = collision.GetComponent<Bullet>();
                    if (bullet != null && bullet.isMelee) {
                        damage = bullet.damage;
                        color = Color.yellow;
                    } else {
                        // Loot는 원거리 총알로 인해 데미지를 받지 않음
                        return;
                    }
                } else {
                    // Loot는 EnemyBullet이나 Enemy로부터 데미지를 받지 않음
                    return;
                }
                break;
        }

        // 피격 시 ShowDamage 코루틴 실행
        if (gameObject.activeInHierarchy && damage > 0) {
            StartCoroutine(ShowDamage(damage, color));
        }
    }

    IEnumerator ShowDamage(float bulletDamage, Color color) {
        GameObject damageText = PoolManager.instance.Get("DamageText");
        damageText.transform.position = transform.position;
        damageText.GetComponent<TempText>().SetTempText(bulletDamage.ToString(), color, 8f);
        yield return new WaitForFixedUpdate();
    }
}
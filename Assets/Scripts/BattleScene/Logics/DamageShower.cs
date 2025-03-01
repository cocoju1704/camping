using System.Collections;
using UnityEngine;

public class DamageShower : MonoBehaviour {
    public enum Subject { Player, Enemy, Loot }
    public Subject subject;

    private void OnTriggerEnter2D(Collider2D collision) {
        // 컨디션 체크해서 연산 줄이기
        if (!IsValidCollision(collision)) return;
        float damage = 0;
        Color color = Color.white;
        Bullet bullet = collision.GetComponent<Bullet>();

        if (bullet == null) return;
        // 태그에 따라 조건 및 대미지 색상 변경
        switch (subject) {
            case Subject.Player:
                if (collision.CompareTag("Enemy") || collision.CompareTag("EnemyBullet")) {
                    damage = bullet.damage;
                    color = Color.red;
                }
                break;

            case Subject.Enemy:
                if (collision.CompareTag("Bullet")) {
                    damage = bullet.damage;
                    color = Color.white;
                }
                break;

            case Subject.Loot:
                if (collision.CompareTag("Bullet") && bullet.isMelee) {
                    damage = bullet.damage;
                    color = Color.yellow;
                }
                break;
        }

        if (damage > 0 && gameObject.activeInHierarchy) {
            StartCoroutine(ShowDamage(damage, color));
        }
    }

    private bool IsValidCollision(Collider2D collision) {
        return collision.CompareTag("Bullet") || collision.CompareTag("Enemy") || collision.CompareTag("EnemyBullet");
    }
    // 대미지 텍스트 표기 + 위치에 살짝 랜덤성 부여
    private IEnumerator ShowDamage(float bulletDamage, Color color) {
        GameObject damageText = PoolManager.instance.Get("DamageText");
        damageText.transform.position = transform.position + (Vector3)Random.insideUnitCircle * 0.5f;
        damageText.GetComponent<TempText>().SetTempText(bulletDamage.ToString(), color, 8f);
        yield return new WaitForFixedUpdate();
    }
}

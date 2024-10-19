using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum BulletType {
        Player,
        Enemy,
    }
    public BulletType bulletType;
    public float damage; // 대미지
    public int pierce; // 관통
    public bool isMelee;
    public float knockback;
    public float speed;
    public Rigidbody2D rigid;
    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
    }

    public virtual void Init(float damage, int pierce, float knockback, Vector3 dir, float speed) {
        SetSpec(damage, pierce, knockback);
        // 원거리 무기
        if (!isMelee) {
            this.speed = speed;
            rigid.velocity = dir * speed;
        }
    }
    public void SetSpec(float damage, int pierce, float knockback) {
        this.damage = damage;
        this.pierce = pierce;
        this.knockback = knockback;
    }
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (isMelee) return;
        if (pierce < 0) return;
        switch (bulletType) {
            case BulletType.Player:
                // 태그가 enemy도 block도 아니면 리턴
                if (!collision.CompareTag("Enemy") && !collision.CompareTag("Block")) return;

                // Enemy와 충돌 시 피어스 감소
                Enemy enemy = collision.GetComponent<Enemy>();
                if (enemy != null && enemy.isLive) {
                    pierce--;
                }

                // Block과 충돌 시 피어스 감소
                Block block = collision.GetComponent<Block>();
                if (block != null) {
                    pierce--;
                }

                // pierce가 0 이하일 때 속도 중지 및 비활성화
                if (pierce < 0) {
                    rigid.velocity = Vector2.zero;
                    gameObject.SetActive(false);
                }
                break;
            case BulletType.Enemy:
                if (collision.CompareTag("Player")) {
                    if (!collision.GetComponent<Player>().isLive) return;
                        rigid.velocity = Vector2.zero;
                        gameObject.SetActive(false);
                }
                else if (collision.CompareTag("Block")) {
                    rigid.velocity = Vector2.zero;
                    gameObject.SetActive(false);
                }
                else if (collision.CompareTag("Bullet")) {
                    if (!collision.GetComponent<Bullet>().isMelee) return;
                    gameObject.SetActive(false);
                }
                break;
        }

    }
    void OnTriggerExit2D(Collider2D collision) {
        if (isMelee) return;
        if (!collision.CompareTag("Area") || pierce < 0) return;
        gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Bullet : MonoBehaviour // 투사체 기본 컴포넌트
{
    public enum BulletType {
        Player,
        Enemy,
    }
    public BulletType bulletType;
    public float damage; // 대미지
    public int pierceCount; // 관통
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
    // 투사체 데이터 설정
    public void SetSpec(float damage, int pierce, float knockback) {
        this.damage = damage;
        this.pierceCount = pierce;
        this.knockback = knockback;
    }
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (isMelee) return;
        if (pierceCount < 0) return;
        switch (bulletType) {
            case BulletType.Player:
                // 태그가 enemy도 block도 아니면 리턴
                if (!collision.CompareTag("Enemy") && !collision.CompareTag("Block")) return;
                Enemy enemy = collision.GetComponent<Enemy>();
                if (enemy != null && enemy.isLive) {
                    pierceCount--;
                }
                Block block = collision.GetComponent<Block>();
                if (block != null) {
                    pierceCount--;
                }
                if (pierceCount < 0) {
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
        if (!collision.CompareTag("Area") || pierceCount < 0) return;
        gameObject.SetActive(false);
    }
}

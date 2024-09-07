using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public int per; // 몇 마리까지 관통가능? 음수면 근접무기 스윙 쿨타임
    Rigidbody2D rigid;
    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
    }
    public void Init(float damage, int per, Vector3 dir) {
        this.damage = damage;
        this.per = per;
        // 원거리 무기
        if (per >= 0) {
            rigid.velocity = dir * 15f;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy") || per < 0) return;
        per--;
        if (per < 0) {
            rigid.velocity = Vector2.zero;
            gameObject.SetActive(false);
        }
    }
    void OntriggerExit2D(Collider2D collision) {
        if (!collision.CompareTag("Area") || per < 0) return;
        gameObject.SetActive(false);
    }
}

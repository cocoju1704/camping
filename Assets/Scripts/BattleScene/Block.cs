using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Block : MonoBehaviour // 플레이어/적 공격에 부서지는 장애물
{
    bool isActive = true;

    void OnEnable() {
        isActive = true;
    }

    public void Init(LootData data) {
        //GetComponent<SpriteRenderer>().sprite = data.sprite;
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Bullet") && isActive) {
            Break();
        }
        if (collision.CompareTag("EnemyBullet") && isActive) {
            Break();
        }
    }

    void Break() {
        isActive = false;
        // 애니메이션 추가
        gameObject.SetActive(false);
    }
}
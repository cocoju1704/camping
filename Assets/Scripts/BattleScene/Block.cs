using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Block : MonoBehaviour
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
        // 블록 파괴에 대한 추가적인 애니메이션이나 로직이 있다면 이곳에 추가 가능
        gameObject.SetActive(false);
    }
}
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Loot : Unit // 파괴 시 재료를 드랍하는 오브젝트
{
    public LootData data;

    void OnEnable() {
        health = maxHealth;
    }
    public void Init(LootData data) {
        this.data = data;
        health = maxHealth = data.maxHealth;
        GetComponent<SpriteRenderer>().sprite = data.icon;
    }

    void OnTriggerEnter2D(Collider2D collision) { // 근접 공격에 맞으면 체력 감소
        if (health <= 0) {
            return;
        }
        if (collision.CompareTag("Bullet")) {
            Bullet bullet = collision.GetComponent<Bullet>();
            if (!bullet.isMelee) return;
            health -= bullet.damage;
            StartCoroutine(DropLoot());
        }
    }
    void OnTriggerExit2D(Collider2D collision) { // 화면 밖으로 나가면 비활성화
        if (!collision.CompareTag("Area")) return;
        gameObject.SetActive(false);
    }
    IEnumerator DropLoot() { // 파괴시 재료 드랍
        if (health <= 0) {
            for (int i = 0; i < data.materialPair.Count; i++) {
                for (int j = 0; j < data.materialPair[i].y; j++) {
                    Ingredient material = PoolManager.instance.Get("Material").GetComponent<Ingredient>();
                    material.transform.position = transform.position;
                    Vector3 destination = material.transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
                    material.Init(data.materialPair[i].x, transform.position);
                    material.transform.DOMove(destination, 1f).SetEase(Ease.OutQuad);
                }
            }
            gameObject.SetActive(false);
        }
        yield return null;
    }
}

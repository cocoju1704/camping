using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Loot : Unit
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

    void OnTriggerEnter2D(Collider2D collision) {
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
    void OnTriggerExit2D(Collider2D collision) {
        if (!collision.CompareTag("Area")) return;
        gameObject.SetActive(false);
    }
    IEnumerator DropLoot() {
        if (health <= 0) {
            for (int i = 0; i < data.materialPair.Count; i++) {
                for (int j = 0; j < data.materialPair[i].y; j++) {
                    Material material = PoolManager.instance.Get("Material").GetComponent<Material>();
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

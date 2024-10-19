using UnityEngine;
using DG.Tweening;
using System.Collections;

public class ExplodingBullet : Bullet {
    public float explosionRadius = 2f;
    public float explosionDamage = 10f;
    public float explosionKnockback = 10f;
    public float explosionDuration = 0.5f;
    public GameObject explosionEffect;
    void OnTriggerEnter2D(Collider2D collision) {
        base.OnTriggerEnter2D(collision);
        if (pierce < 0) {
            StartCoroutine(Explode());
        }
    }
    IEnumerator Explode() {
        // Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        // foreach (Collider2D collider in colliders) {
        //     if (collider.CompareTag("Enemy")) {
        //         Enemy enemy = collider.GetComponent<Enemy>();
        //         if (enemy != null && enemy.isLive) {
        //             enemy.GetDamage(explosionDamage, explosionKnockback, transform.position);
        //         }
        //     }
        //     if (collider.CompareTag("Block")) {
        //         Block block = collider.GetComponent<Block>();
        //         if (block != null) {
        //             block.Break();
        //         }
        //     }
        // }
        // GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        // Destroy(effect, explosionDuration);
        // gameObject.SetActive(false);
        yield return null;
    }
}
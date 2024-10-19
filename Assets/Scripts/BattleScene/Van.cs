using UnityEngine;

public class Van : Unit {
    Bullet selfBullet;
    Canvas canvas;
    void Start() {
        maxHealth = GameManager.instance.carSpec.carHealth;
        health = maxHealth;
        selfBullet = GetComponent<Bullet>();
        canvas = GetComponentInChildren<Canvas>();
    }
    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("EnemyBullet")) {
            Bullet bullet = collision.GetComponent<Bullet>();
            health -= bullet.damage;
            if (health <= 0) {
                health = 0;
                StageManager.instance.onGameOver.Invoke();
            }
        }
    }
    public void ToggleUI() {
        canvas.enabled = !canvas.enabled;
    }
}
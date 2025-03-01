using UnityEngine;

public class Van : Unit { // 캠핑카 유닛
    Bullet selfBullet; // 컷신에서 장애물 부수면서 등장하기 위해 캠핑카에 투사체 속성 부여
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
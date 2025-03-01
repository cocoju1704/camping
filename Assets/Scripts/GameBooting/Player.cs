using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Player : Singleton<Player>, ISavable
{
    [Header("스탯")]
    public float health;
    public int maxHealth;
    public float speed;
    public float _reloadMultiplier = 0f;
    public float reloadMultiplier {
        get {
            return _reloadMultiplier;
        }
        set {
            Debug.Log("ReloadMultiplier Changed on" + gameObject.name);
            _reloadMultiplier = value;
        }
    }
    public float bonusHealth = 0f;
    public float meleeBonusDmg = 0f;
    public float meleeBonusKnockback = 0f;

    [Header("스태미나")]
    public float stamina = 100f;
    public float maxStamina = 100f;
    float staminaCost = 100f;
    public bool isSprinting = false;

    [Header("UI")]
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator animator;
    Canvas UICanvas;

    [Header("기타")]
    public Vector2 inputVec;
    public Scanner scanner;
    public bool isLive = true;
    public bool isMoving = true;
    public UnityEvent OnPlayerSpecChanged;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
        UICanvas = GetComponentInChildren<Canvas>();
        Load(null);
    }

    void FixedUpdate() {
        if (!isMoving) return;

        // 스태미나가 있을 때만 달리기 가능
        if (isSprinting && stamina > 0) {
            Debug.Log(staminaCost);
            stamina -= staminaCost * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
        } else {
            // 스태미나 회복
            isSprinting = false;
            stamina += maxStamina * Time.deltaTime / 4;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
        }

        // 속도 증가
        float currentSpeed = isSprinting ? speed * 1.5f : speed;
        Vector2 nextVec = inputVec.normalized * currentSpeed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    void OnMove(InputValue value) {
        inputVec = value.Get<Vector2>();
    }

    void OnSprint(InputValue value) {
        if (value.isPressed && stamina > 0) {
            isSprinting = true;
        } else {
            isSprinting = false;
        }
    }
    void LateUpdate() {
        animator.SetFloat("Speed", inputVec.magnitude);
        //마우스 방향에 따라 캐릭터 회전
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 dir = mousePos - transform.position;
        if (dir.x > 0) {
            spriteRenderer.flipX = false;
        } else if (dir.x < 0) {
            spriteRenderer.flipX = true;
        }
        else {
            if (inputVec.x > 0) {
                spriteRenderer.flipX = false;
            } else if (inputVec.x < 0) {
                spriteRenderer.flipX = true;
            }
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        bool tookDamage = false;
        float damage = 0f;
        // Enemy와 충돌했을 경우
        if (collision.CompareTag("Enemy")) {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null) {
                damage = enemy.damage;
                health -= damage;
                tookDamage = true;
            }
        }
        // EnemyBullet과 충돌했을 경우
        else if (collision.CompareTag("EnemyBullet")) {
            Bullet enemyBullet = collision.GetComponent<Bullet>();
            if (enemyBullet != null) {
                damage = enemyBullet.damage;
                health -= damage;
                tookDamage = true;
            }
        }
        else {
            return;
        }
        if (tookDamage) {
            Vector2 velocity = collision.GetComponent<Rigidbody2D>()?.velocity ?? Vector2.zero;
            if (health > 0) {
                animator.SetTrigger("Hit");
                StartCoroutine(Hit(5, velocity));
            }
            else {
                isLive = false;
            }
        }
    }
    IEnumerator Hit(float knockback, Vector2 velocity) {
        Debug.Log("Hit");
        isMoving = false;
        rigid.velocity = Vector2.zero;
        rigid.AddForce(velocity * knockback, ForceMode2D.Impulse);
        yield return new WaitForFixedUpdate();
        isMoving = true;
    }
    // 활성화/비활성화
    public void Deactivate() {
        enabled = false;
        GetComponent<PlayerInput>().enabled = false;
        GetComponent<WeaponSystem>().DisableWeapon();
        GetComponent<Rigidbody2D>().mass = 1000000;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Scanner>().enabled = false;
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>()) {
            sr.enabled = false;
        }
        UICanvas.enabled = false;
    }
    public void DeactivateWeapon() {
        GetComponent<WeaponSystem>().DisableWeapon();
    }
    public void Activate() {
        enabled = true;
        GetComponent<PlayerInput>().enabled = true;
        GetComponent<Rigidbody2D>().mass = 1;
        GetComponent<Collider2D>().enabled = true;
        GetComponent<Scanner>().enabled = true;
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>()) {
            if (!sr.gameObject.CompareTag("Bullet"))
            sr.enabled = true;
        }
        UICanvas.enabled = true;
        health = maxHealth;
    }
    public void ActivateWeapon() {
        GetComponent<WeaponSystem>().EnableWeapon();
    }
    // 저장/불러오기
    public void Save(GameData data) {
        data.playerSpec = new PlayerSpec(health, maxHealth, speed);

    }
    public void Load(GameData data) {
        if (data == null) {
            health = 100;
            maxHealth = 100;
            speed = 5;
            return;
        }
        health = data.playerSpec.health;
        maxHealth = data.playerSpec.maxHealth;
        speed = data.playerSpec.speed;
    }
}

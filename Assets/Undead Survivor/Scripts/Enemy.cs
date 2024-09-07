using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public int exp;
    public int damage;
    public int materialType;
    public int dropRate;
    public RuntimeAnimatorController[] animControllers;
    Rigidbody2D target; // 몬스터가 향하는 대상
    bool isLive = true; //몬스터 사망여부
    Animator anim;
    
    Rigidbody2D rigid; // 몬스터의 rigidbody
    Collider2D coll; // 몬스터의 콜라이더

    SpriteRenderer spriteRenderer;
    WaitForFixedUpdate waitForFixedUpdate;

    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        waitForFixedUpdate = new WaitForFixedUpdate();
        coll = GetComponent<Collider2D>();
    }

    void FixedUpdate()
    {
        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit")) return;
        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec); //플레이어의 키입력 + 몬스터 방향
        rigid.velocity = Vector2.zero; //플레이어 충돌 시 밀려남 무시
    }
    void LateUpdate() {
        spriteRenderer.flipX = target.position.x < rigid.position.x;
    }
    void OnEnable() {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        health = maxHealth;
        coll.enabled = true;
        rigid.simulated = true;
        spriteRenderer.sortingOrder = 2;
        anim.SetBool("Dead", false);

    }
    public void Init(SpawnData data) {
        anim.runtimeAnimatorController = animControllers[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
        exp = data.exp;
        materialType = data.materialType;
        dropRate = data.dropRate;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet") || !isLive) return;
        Bullet bullet = collision.GetComponent<Bullet>();
        health -= bullet.damage;
        StartCoroutine(KnockBack(3));
        if (health > 0) {
            anim.SetTrigger("Hit");
        }
        else {
            isLive = false;
            coll.enabled = false;
            rigid.simulated = false;
            spriteRenderer.sortingOrder = 1;
            anim.SetBool("Dead", true);
            GameManager.instance.kills++;
            GameManager.instance.GetExp(exp);
            DropLoot();
        }
    }
    void Dead() {
        gameObject.SetActive(false);
    }
    IEnumerator KnockBack(int knockBackForce) {
        yield return waitForFixedUpdate;
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos;
        rigid.AddForce(dirVec.normalized * knockBackForce, ForceMode2D.Impulse);
    }
    IEnumerator Hit() {
        yield return waitForFixedUpdate; // 다음 물리프레임까지 딜레이
    }
    void DropLoot() {
        bool isDrop = Random.Range(0, 100) < dropRate;
        if (!isDrop) return;
        Material material = GameManager.instance.poolManager.Get(3).GetComponent<Material>();
        material.transform.position = transform.position;
        material.Init(materialType, transform.position);
    }
}

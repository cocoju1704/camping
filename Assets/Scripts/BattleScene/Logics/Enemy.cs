using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
public class Enemy : Unit
{
    public float speed;
    public int exp;
    public int damage;
    public int materialID;
    public int dropRate;

    public EnemyData enemyData;
    public RuntimeAnimatorController[] animControllers;
    Rigidbody2D target; // 몬스터가 향하는 대상
    public bool isLive = true; //몬스터 사망여부
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
        GetComponent<EnemyAction>().enabled = true;
        transform.localScale = new Vector3(1, 1, 1);
    }
    public void Init(EnemyData data) {
        enemyData = data;
        anim.runtimeAnimatorController = animControllers[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
        exp = data.exp;
        materialID = data.materialType;
        dropRate = data.dropRate;
        damage = data.damage;
        GetComponent<EnemyMovement>().Init();
        GetComponent<EnemyAction>().Init();

    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.CompareTag("Bullet") || !isLive) return;

        Bullet bullet = collision.GetComponent<Bullet>();
        health -= bullet.damage;
        StartCoroutine(KnockBack(bullet.knockback));
        StartCoroutine(ShowDamage(bullet.damage));

        if (health > 0) {
            anim.SetTrigger("Hit");
        } else {
            isLive = false;
            coll.enabled = false;
            spriteRenderer.sortingOrder = 1;
            anim.SetBool("Dead", true);
            StageManager.instance.kills++;
            StageManager.instance.GetExp(exp);
            if (enemyData.isBoss) StageManager.instance.onBossClear.Invoke();
            GetComponent<EnemyAction>().enabled = false;
            StartCoroutine(DropLoot());
        }
    }

    void Dead() {
        rigid.simulated = false;
        gameObject.SetActive(false);
    }

    IEnumerator KnockBack(float knockBackForce) {
        yield return waitForFixedUpdate;
        rigid.velocity = Vector2.zero;
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos;
        rigid.AddForce(dirVec.normalized * knockBackForce, ForceMode2D.Impulse);
    }

    IEnumerator DropLoot() {
        bool isDrop = Random.Range(0, 100) < dropRate;
        if (!isDrop) yield break;

        Material material = PoolManager.instance.Get("Material").GetComponent<Material>();
        material.transform.position = transform.position;
        Vector3 destination = material.transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
        material.Init(materialID, transform.position);
        yield return material.transform.DOMove(destination, 1f).SetEase(Ease.OutQuad).WaitForCompletion();
        yield return new WaitForSeconds(1f);
        Dead();
    }

    IEnumerator ShowDamage(float bulletDamage) {
        GameObject damageText = PoolManager.instance.Get("DamageText");
        damageText.transform.position = transform.position;
        damageText.GetComponent<TempText>().SetTempText(bulletDamage.ToString(), Color.white, 8);
        yield return new WaitForFixedUpdate();
    }
}
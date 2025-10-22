using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
public class Enemy : Unit // 몬스터의 기본 정보
{
    [Header("Monster Params")]
    public float speed;
    public int exp;
    public int damage;
    public int materialID;
    public int dropRate;
    public bool isBoss = false;

    public EnemyData enemyData;
    Rigidbody2D target; // 몬스터가 향하는 대상
    public bool isLive = true; //몬스터 사망여부
    Animator anim;
    Rigidbody2D rigid; // 몬스터의 rigidbody
    Collider2D coll; // 몬스터의 콜라이더
    SpriteRenderer spriteRenderer;
    WaitForFixedUpdate waitForFixedUpdate;

    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponent<Animator>();
        waitForFixedUpdate = new WaitForFixedUpdate();
        coll = GetComponent<Collider2D>();
    }
    void LateUpdate() { // 방향에 따라 스프라이트 뒤집기
        spriteRenderer.flipX = target.position.x < rigid.position.x;
    }

    public void Init(EnemyData data) { // 몬스터 프리팹이 어떤 종류의 몬스터일지 결정
        enemyData = data;
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
        exp = data.exp;
        materialID = data.materialType;
        dropRate = data.dropRate;
        damage = data.damage;
        GetComponent<EnemyPathfinding>().Init();
        GetComponent<EnemyAction>().Init();

    }
    void OnEnable() { // 결정된 프리팹에 따라 몬스터 초기화
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

    void OnTriggerEnter2D(Collider2D collision) { // 플레이어 투사체와의 충돌 처리
        if (!collision.CompareTag("Bullet") || !isLive) return;

        Bullet bullet = collision.GetComponent<Bullet>();
        health -= bullet.damage;
        StartCoroutine(KnockBack(bullet.knockback));
        //StartCoroutine(ShowDamage(bullet.damage));

        if (health > 0) {
            anim.SetTrigger("Hit");
        } else {
            isLive = false;
            coll.enabled = false;
            spriteRenderer.sortingOrder = 1;
            anim.SetBool("Dead", true);
            StageManager.instance.currentKills++;
            StageManager.instance.GetExp(exp);
            GetComponent<EnemyAction>().enabled = false;
            StartCoroutine(DropLoot());
        }
    }

    void AfterDeathCleanup()
    { //몬스터 사망 처리(비활성화)
        rigid.simulated = false;
        gameObject.SetActive(false);
        if (isBoss) {
            StageManager.instance.onStageClear.Invoke();
        }
    }

    IEnumerator KnockBack(float knockBackForce) { // 투사체 넉백 변수를 인자로 받아 그만큼 넉백
        yield return waitForFixedUpdate;
        rigid.velocity = Vector2.zero;
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos;
        rigid.AddForce(dirVec.normalized * knockBackForce, ForceMode2D.Impulse);
    }

    IEnumerator DropLoot() { 
        bool isDrop = Random.Range(0, 100) < dropRate;
        if (!isDrop) yield break;

        Ingredient material = PoolManager.instance.Get("Material").GetComponent<Ingredient>();
        material.transform.position = transform.position;
        Vector3 destination = material.transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
        material.Init(materialID, transform.position);
        yield return material.transform.DOMove(destination, 1f).SetEase(Ease.OutQuad).WaitForCompletion();
        yield return new WaitForSeconds(1f);
        AfterDeathCleanup();
    }
}
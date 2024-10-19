using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class EnemyAction : MonoBehaviour {
    float timer = 0;
    Rigidbody2D target; // 공격 대상
    Rigidbody2D rigid; // 자신
    public GameObject selfBullet; // 돌진시 활성화되는 콜라이더
    public EnemyActionData[] enemyActionData;
    public int prefabId;
    public EnemyActionData.AttackType attackType;
    public int damage;
    public float cooldown;
    public int count;
    public float speed;
    public float spread;
    public float range;
    public int repeat;
    public GameObject projectile;
    WaitForFixedUpdate waitForFixedUpdate;
    EnemyMovement enemyMovement;

    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        waitForFixedUpdate = new WaitForFixedUpdate();
    }

    void OnEnable() {
        selfBullet.SetActive(false);
    }

    public void Init() {
        target = Player.instance.GetComponent<Rigidbody2D>();
        enemyActionData = GetComponent<Enemy>().enemyData.enemyActionData;
        enemyMovement = GetComponent<EnemyMovement>();
        SetNextAttack();
    }

    void Update() {
        timer += Time.deltaTime;

        if (timer > cooldown) {
            timer = 0;
            SetNextAttack();
            if (Vector3.Distance(target.position, transform.position) < range) {
                UseAttack();
            }
        }
    }

    void SetNextAttack() {
        int random = Random.Range(0, enemyActionData.Length);
        attackType = enemyActionData[random].attackType;
        damage = enemyActionData[random].damage;
        // 쿨다운 시간은 랜덤으로 설정
        cooldown = enemyActionData[random].cooldown + Random.Range(-1, 1);
        count = enemyActionData[random].count;
        speed = enemyActionData[random].speed;
        spread = enemyActionData[random].spread;
        range = enemyActionData[random].range;
        repeat =  enemyActionData[random].repeat;
        projectile = enemyActionData[random].projectile;

        for (int i = 0; i < PoolManager.instance.prefabs.Length; i++) {
            if (projectile == PoolManager.instance.prefabs[i]) {
                prefabId = i;
                break;
            }
        }
    }

    void UseAttack() {
        Debug.Log(attackType);
        switch (attackType) {
            case EnemyActionData.AttackType.Melee_Charge:
                StartCoroutine(Melee_Charge());
                break;
            case EnemyActionData.AttackType.Ranged_Normal:
                StartCoroutine(Range_Normal());
                break;
            case EnemyActionData.AttackType.Ranged_Spread:
                StartCoroutine(Range_Spread());
                break;
            case EnemyActionData.AttackType.Ranged_Circle:
                StartCoroutine(CircleAttack());
                break;
            default:
                break;
        }           
    }

    IEnumerator Range_Normal() {
        for (int i = 0; i < repeat; i++) {
            for (int j = 0; j < count; j++) {
                Vector3 targetPos = target.position;
                Vector3 dir = targetPos - transform.position;
                dir.Normalize();
                // spread 적용
                dir = Quaternion.Euler(0, 0, Random.Range(-spread, spread)) * dir;
                Transform newBullet = PoolManager.instance.Get(prefabId).transform;
                newBullet.position = transform.position;
                newBullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
                newBullet.GetComponent<Bullet>().Init(damage, 0, 3, dir, speed);
                // 0.1초 간격으로 발사
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator Melee_Charge() {
        // Stop movement during charge
        enemyMovement.isMoving = false;
        // 선딜
        selfBullet.SetActive(true);
        Vector3 targetPos = target.position;
        Vector3 dir = (targetPos - transform.position).normalized;
        selfBullet.transform.rotation = Quaternion.FromToRotation(Vector3.right, dir);
        yield return new WaitForSeconds(0.3f);
        targetPos = target.position;
        dir = (targetPos - transform.position).normalized;
        selfBullet.transform.rotation = Quaternion.FromToRotation(Vector3.right, dir);
        if (!enemyMovement.isLive) yield break;
        yield return waitForFixedUpdate; // 물리 업데이트 후 확인
        rigid.velocity = Vector2.zero; // 먼저 속도를 초기화

        // 목표 방향으로 힘을 가합니다.
        rigid.AddForce(dir * speed, ForceMode2D.Impulse);
        // 후딜
        yield return new WaitForSeconds(0.5f);
        selfBullet.SetActive(false);
        // Allow movement again after charge ends
        enemyMovement.isMoving = true;  
    }

    IEnumerator Range_Spread() {
        float spreadAngle = 10f; // Adjust this to control the spread width

        for (int i = 0; i < repeat; i++) {
            for (int j = 0; j < count; j++) {
                // Calculate the base direction
                Vector3 targetPos = target.position;
                Vector3 baseDir = targetPos - transform.position;
                baseDir.Normalize();

                // Calculate spread angle for each bullet
                float angleOffset = spreadAngle * (j - (count - 1) / 2f); // Center the spread
                Vector3 dir = Quaternion.Euler(0, 0, angleOffset) * baseDir;

                // Fire the bullet
                Transform newBullet = PoolManager.instance.Get(prefabId).transform;
                newBullet.position = transform.position;
                newBullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
                newBullet.GetComponent<Bullet>().Init(damage, 0, 3, dir, speed);

                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator CircleAttack() {
        //자신을 중심으로 원형으로 전방위 발사
        for (int i = 0; i < repeat; i++) {
            for (int j = 0; j < count; j++) {
                // 각도 살짝 변화
                Vector3 dir = Quaternion.Euler(0, 0, 360 / count * j) * Vector3.up;
                // 홀수 i에서만 살짝 방향을 바꿔줌
                if (i % 2 == 1) dir = Quaternion.Euler(0, 0, 10) * dir;
                Transform newBullet = PoolManager.instance.Get(prefabId).transform;
                newBullet.position = transform.position;
                newBullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
                newBullet.GetComponent<Bullet>().Init(damage, 0, 3, dir, speed);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    // IEnumerator SpawnEnemy() {
    //     GameObject enemy = PoolManager.instance.Get("Enemy");
    //     enemy.transform.position = transform.position;
    //     int rand = UnityEngine.Random.Range(0, level+1);
    //     enemy.GetComponent<Enemy>().Init(enemyData[rand]);
    //     yield return null;
    // }
}
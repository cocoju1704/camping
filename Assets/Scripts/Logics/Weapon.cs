using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Weapon : MonoBehaviour
{
    [Header("무기 데이터")]
    public WeaponData weaponData;
    [Header("근접 무기 투사체")]
    Bullet meleeBullet;
    Collider2D bulletCollider;
    SpriteRenderer bulletRenderer;
    Player player;
    WeaponSystem weaponSystem;

    [Header("무기 스펙")]
    // 공통
    public int level = 0;
    public float damage;
    public float cooldown;
    public float knockback;
    public float reloadTime;
    // 근접
    float swingTime = 0.05f;
    // 원거리
    public int mag = 0;
    public int maxMag = 0;
    public float speed = 0;
    public float spread = 0;
    public int pierce = 0;

    [Header("기본 변수")]
    int prefabId;
    public bool isMelee;
    public bool canClick = true;
    public bool isReloading = false; // New flag to track reloading state
    
    void Awake() {
        player = GameManager.instance.player;
        weaponSystem = player.GetComponent<WeaponSystem>();
        Player.instance.OnPlayerSpecChanged.AddListener(SetSpec);
    }

    void Update() {
        // UI에서 클릭된 경우 발사하지 않음
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (!canClick) return; // Prevent firing when reloading

        if (isMelee) {
            // 항상 무기 방향 따라 위치 재조정
            if (GetComponent<SpriteRenderer>().flipX) {
                meleeBullet.transform.localPosition = new Vector3(-0.6f, 0, 0);
                meleeBullet.GetComponent<SpriteRenderer>().flipX = true;
            } else {
                meleeBullet.transform.localPosition = new Vector3(0.6f, 0, 0);
                meleeBullet.GetComponent<SpriteRenderer>().flipX = false;
            }
            // 스윙
            if (Input.GetMouseButton(0) && canClick) {
                StartCoroutine(Swing());
            }
        } else {
            if (Input.GetMouseButton(0) && canClick) {
                if (mag > 0) {
                    mag--;
                    StartCoroutine(Fire(weaponSystem.targetPos));
                } else {
                    StartCoroutine(Reload());
                }
            }
        }
    }

    public void Init(WeaponData data = null) {
        // 무기 데이터 초기화
        if (data != null) {
            weaponData = data;
        }
        if (GameManager.instance.obtainedWeapons.Exists(x => x.Item1.id == weaponData.id)) {
            level = GameManager.instance.obtainedWeapons.Find(x => x.Item1.id == weaponData.id).Item2;
        } else {
            level = 0;
        }
        canClick = true;
        mag = weaponData.magazine[level];
        isMelee = weaponData.type == WeaponData.WeaponType.Melee;
        if (isMelee) {
            InitMeleeWeapon();
        } else {
            InitRangeWeapon();
        }
    }

    void InitMeleeWeapon() {
        // 근접 무기용 투사체 생성
        // 이미 있으면 생성x
        if (meleeBullet == null) {
            GameObject bulletPrefab = Instantiate(weaponData.projectile, transform);
            bulletPrefab.transform.localPosition = new Vector3(0.6f, 0, 0);
            meleeBullet = bulletPrefab.GetComponent<Bullet>();
            bulletCollider = bulletPrefab.GetComponent<Collider2D>();
            bulletRenderer = meleeBullet.GetComponent<SpriteRenderer>();
        }
        SetSpec();
    }

    void InitRangeWeapon() {
        // 원거리 무기 설정
        for (int i = 0; i < PoolManager.instance.prefabs.Length; i++) {
            if (weaponData.projectile == PoolManager.instance.prefabs[i]) {
                prefabId = i;
                break;
            }
        }
        SetSpec();
    }
    
    public void SetSpec() {
        if (weaponSystem == null) level = 0;
        else level = GameManager.instance.obtainedWeapons.Find(x => x.Item1.id == weaponData.id).Item2;
        damage = weaponData.damage[level];
        cooldown = weaponData.rate[level];
        knockback = weaponData.knockback[level];
        spread = weaponData.spread[level];
        pierce = weaponData.pierce[level];
        mag = weaponData.magazine[level];
        maxMag = weaponData.magazine[level];
        reloadTime = weaponData.reload * (1 - Player.instance.reloadMultiplier);
        if (isMelee) {
            meleeBullet.SetSpec(damage, pierce, knockback);
        }
    }

    IEnumerator Swing() {
        // 근접 공격 로직
        canClick = false;
        bulletCollider.enabled = true;
        bulletRenderer.enabled = true;
        yield return new WaitForSeconds(swingTime);
        bulletCollider.enabled = false;
        bulletRenderer.enabled = false;
        yield return new WaitForSeconds(cooldown);
        canClick = true;
    }

    IEnumerator Fire(Vector3 targetPos) {
        canClick = false;
        // 적의 방향 계산
        if (targetPos == Vector3.zero) yield break; // targetPos가 없으면 종료
        Vector3 dir = targetPos - transform.position;
        dir.Normalize();
        float speed = weaponData.speed;
        
        switch (weaponData.type) {
            case WeaponData.WeaponType.Pistol:
            case WeaponData.WeaponType.Rifle:
            case WeaponData.WeaponType.Sniper:
                dir = Quaternion.Euler(0, 0, Random.Range(-spread, spread)) * dir;
                Transform newBullet = PoolManager.instance.Get(prefabId).transform;
                newBullet.position = transform.position;
                newBullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
                newBullet.GetComponent<Bullet>().Init(damage, pierce, knockback, dir, speed);
                break;
            // 샷건은 spread = 탄알 개수
            case WeaponData.WeaponType.Shotgun:
                for (int i = 0; i < spread; i++) {
                    float angle = Random.Range(-20, 20);
                    Vector3 newDir = Quaternion.Euler(0, 0, angle) * dir;
                    Transform newBullet2 = PoolManager.instance.Get(prefabId).transform;
                    newBullet2.position = transform.position;
                    newBullet2.rotation = Quaternion.FromToRotation(Vector3.up, newDir);
                    newBullet2.GetComponent<Bullet>().Init(damage, pierce, knockback, newDir, speed);
                }
                break;
            case WeaponData.WeaponType.Launcher:
                // 로켓 발사 처리
                break;
        }
        
        yield return new WaitForSeconds(cooldown);  // 발사 후 쿨다운 대기
        canClick = true;
    }

    public IEnumerator Reload() {
        // Set canFire to false and track reloading state
        canClick = false;
        isReloading = true;
        //hud.ChangeTypeTo(SliderHUD.InfoType.Reload);
        
        yield return new WaitForSeconds(reloadTime);

        // Reload finished, reset mag and canFire, and update HUD
        mag = maxMag;
        isReloading = false;
        canClick = true;
        //hud.ChangeTypeTo(SliderHUD.InfoType.Mag);
    }
}
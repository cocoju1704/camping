using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id; //  무기 자체 id
    public int prefabId; //PoolManager에 있는 프리팹 id
    public float damage;
    public int count; // 근접무기는 개수, 원거리는 관통개수
    public float speed; // 근접무기는 회전속도, 원거리는 연사속도

    Player player;
    float timer = 0;
    void Awake() {
        player = GameManager.instance.player;
    }
    void Update() {
        switch(id) {
            case 0:
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;
            default:
                timer += Time.deltaTime;
                if (timer > speed) {
                    timer = 0f;
                    AutoFire();
                }
                break;
        }
    }
    public void LevelUp(float damage, int count) {
        this.damage = damage;
        this.count = count;
        if (id == 0) {
            //PlaceShield();
        }
        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);

    }

    public void Init(ItemData data) {
        // Basic Set
        name = "Weapon" + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;
        // Property Set
        id = data.itemId;
        damage = data.baseDmg;
        count = data.baseCount;
        for (int i = 0; i  < GameManager.instance.poolManager.prefabs.Length; i++) {
            if (data.projectile == GameManager.instance.poolManager.prefabs[i]) {
                prefabId = i;
                break;
            }
        }

        switch(id) {
            case 0:
                speed = 150;
                SpawnMelee();
                //PlaceShield();
                break;
            default:
                speed = 0.4f;
                break;
        }
        // Hand Set
        Hand hand = player.GetComponent<HandController>().hands[(int)data.itemType]; // Range = 0 = 왼손, Melee = 1 = 오른손
        hand.spriteRenderer.sprite = data.hand;
        hand.gameObject.SetActive(true);

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }
    void PlaceShield() {
        for (int i = 0; i < count; i++ ) {
            Transform bullet;
            if (i < transform.childCount) { // 이미 있는거는 레벨업시 위치만 이동
                bullet = transform.GetChild(i);
            } 
            else { // 새로 생긴거는 새로 풀링하고 위치 이동
                bullet = GameManager.instance.poolManager.Get(prefabId).transform;
                bullet.parent = transform;
            }
            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            Vector3 rotVec = Vector3.forward * (360 / count) * i;
            bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * 1.5f, Space.World);
            bullet.GetComponent<Bullet>().Init(damage, -1, Vector3.zero);

        }
    }
    void AutoFire() {
        if (!player.scanner.nearestTarget) return;

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;
        Transform bullet = GameManager.instance.poolManager.Get(prefabId).transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<Bullet>().Init(damage, count, dir);
    }
    // 근접 무기 소환하고 장착
    void SpawnMelee() {
        Transform bullet = GameManager.instance.poolManager.Get(prefabId).transform;
        bullet.parent = GameManager.instance.player.GetComponent<HandController>().hands[0].transform;
        bullet.localPosition = bullet.parent.localPosition;
        bullet.localRotation = bullet.parent.localRotation * Quaternion.Euler(0, 0, -90);
        bullet.GetComponent<Bullet>().Init(damage, -1, Vector3.zero);
        bullet.GetComponent<Collider2D>().enabled = false;

    }
}

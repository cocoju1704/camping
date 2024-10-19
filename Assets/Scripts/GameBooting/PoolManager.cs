using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 자주 사용하는 오브젝트를 매번 Instantiate하면 메모리 낭비가 심하다.
// 하여 자주 사용하는 오브젝트 집단(풀)을 Instantiate해놓고 필요할때마다 활성화/비활성화 하는걸 풀링이라함

public class PoolManager : Singleton<PoolManager>
{
    // 프리팹들을 보관할 변수
    public GameObject[] prefabs;
    public static Dictionary<string, int> prefabIndex = new Dictionary<string, int>() {
        {"Enemy", 0},
        {"NormalBullet", 1},
        {"Material", 2},
        {"DamageText", 3},
        {"Loot", 4},
        {"EnemyBullet", 5},
        {"Block", 6},
        {"PlayerBullet_Rotate", 7},
        {"PlayerBullet_Tracking", 8},
        {"EnemyBullet_Rotate", 9},
        {"PlayerBullet_Slowdown", 10}
    };
    // 풀 담당을 하는 리스트들
    public List<GameObject>[] pools;
    void Awake() {
        pools = new List<GameObject>[prefabs.Length];
        for (int i = 0; i < pools.Length; i++) {
            pools[i] = new List<GameObject>();
        }
    }
    public GameObject Get(string name) {
        return Get(prefabIndex[name]);
    }
    public GameObject Get(int index) {
        GameObject select = null;
        // ... 선택한 풀의 놀고 있는(=비활성화 된) 게임 오브젝트 접근
            // ... 발견 시 select 변수에 할당
        foreach (GameObject obj in pools[index]) {
            if (obj == null) {
                Debug.LogError(obj);
            }
            if (!obj.activeSelf) {
                select = obj;
                select.SetActive(true);
                break;
            }
        }
        // ... 비활성화 오브젝트가 없을 시
            // ... 새롭게 생성하고 select 변수에 할당
        if (!select) {
            select = Instantiate(prefabs[index], new Vector3(999, 999, 0), Quaternion.identity);
            // 부모 오브젝트를 PoolManager로 설정
            select.transform.SetParent(transform);
            select.name = prefabs[index].name + pools[index].Count;
            pools[index].Add(select);
        }
        return select;
    }
    // 풀 안의 모든 오브젝트 비활성화
    public void Clear(int index) {
        foreach (GameObject obj in pools[index]) {
            obj.SetActive(false);
        }
    }
    public void ClearAll(){
        for (int i = 0; i < pools.Length; i++) {
            Clear(i);
        }
    }
    public void ClearExceptBlock(){
        for (int i = 0; i < pools.Length; i++) {
            if (i ==6) continue;
            Clear(i);
        }
    }
    public void Destroy(int index) {
        foreach (GameObject obj in pools[index]) {
            Destroy(obj);
        }
        pools[index].Clear();
    }
    public void DestroyAll() {
        ClearAll();
        for (int i = 0; i < pools.Length; i++) {
            Destroy(i);
        }
    }
}

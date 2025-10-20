using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 자주 사용하는 오브젝트를 매번 Instantiate하면 메모리 낭비가 심하다.
// 하여 자주 사용하는 오브젝트 집단(풀)을 Instantiate해놓고 필요할때마다 활성화/비활성화 하는걸 풀링이라함

public class PoolManager : Singleton<PoolManager>
{
    // 프리팹들을 보관할 변수 (Project 뷰의 Prefab Asset 참조)
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

    // 풀 담당 리스트들
    public List<GameObject>[] pools;

    // ▶ 추가: 프리팹별 그룹 부모 Transform
    private Transform[] parents;

    void Awake() {
        pools = new List<GameObject>[prefabs.Length];
        parents = new Transform[prefabs.Length];

        for (int i = 0; i < pools.Length; i++) {
            pools[i] = new List<GameObject>();
            EnsureParent(i); // ▶ 인덱스별 그룹 폴더 생성
        }
    }

    /// <summary>
    /// 문자열 키로 Get
    /// </summary>
    public GameObject Get(string name) {
        return Get(prefabIndex[name]);
    }

    /// <summary>
    /// 인덱스로 Get (부모 지정 없음 → 그룹 하위에 둠)
    /// </summary>
    public GameObject Get(int index) {
        return Get(index, parents[index]); // ▶ 기본은 그룹 하위
    }

    /// <summary>
    /// 인덱스로 Get + 부모 지정 (원하면 호출자가 별도 부모로 붙일 수 있음)
    /// </summary>
    public GameObject Get(int index, Transform parentOverride) {
        GameObject select = null;

        // 놀고 있는(비활성) 오브젝트 재사용
        foreach (GameObject obj in pools[index]) {
            if (obj == null) {
                Debug.LogError("Pool contains null object");
                continue;
            }
            if (!obj.activeSelf) {
                select = obj;

                // ▶ 부모를 보장: 로컬값 유지(false)로 스케일/회전 꼬임 방지
                if (parentOverride != null && select.transform.parent != parentOverride) {
                    select.transform.SetParent(parentOverride, false);
                }

                select.SetActive(true);
                break;
            }
        }

        // 없으면 새로 생성
        if (!select) {
            // 생성은 월드 구석에서
            select = Instantiate(prefabs[index], new Vector3(999, 999, 0), Quaternion.identity);

            // ▶ 부모를 전용 그룹으로 설정(로컬값 유지)
            var parent = parentOverride != null ? parentOverride : parents[index];
            select.transform.SetParent(parent, false);

            // ▶ 스케일 초기화: 프리팹 원본 localScale을 그대로 복원 (0.6 등 유지)
            //    (혹시 외부 코드가 Vector3.one을 강제했다면 여기서 다시 보정)
            select.transform.localScale = prefabs[index].transform.localScale;

            // 이름 지정 및 풀 등록
            select.name = prefabs[index].name + "_" + pools[index].Count;
            pools[index].Add(select);
        }

        return select;
    }

    /// <summary>
    /// 프리팹별 그룹 폴더 생성/보장
    /// </summary>
    private void EnsureParent(int index) {
        if (parents[index] != null) return;

        string groupName = $"{prefabs[index].name}_Group";
        var groupGO = new GameObject(groupName);
        // ▶ PoolManager 밑에 붙임 (로컬 유지)
        groupGO.transform.SetParent(this.transform, false);

        // 그룹의 트랜스폼은 기본값 유지 (특히 localScale=1)
        groupGO.transform.localPosition = Vector3.zero;
        groupGO.transform.localRotation = Quaternion.identity;
        groupGO.transform.localScale = Vector3.one; // 부모 스케일 1 유지가 중요 (자식 스케일 보존)

        parents[index] = groupGO.transform;
    }

    // 풀 안의 모든 오브젝트 비활성화
    public void Clear(int index) {
        foreach (GameObject obj in pools[index]) {
            if (obj) obj.SetActive(false);
        }
    }

    public void ClearAll(){
        for (int i = 0; i < pools.Length; i++) {
            Clear(i);
        }
    }

    public void ClearExceptBlock(){
        for (int i = 0; i < pools.Length; i++) {
            if (i == 6) continue;
            Clear(i);
        }
    }

    public void Destroy(int index) {
        foreach (GameObject obj in pools[index]) {
            if (obj) Destroy(obj);
        }
        pools[index].Clear();

        // 그룹 폴더도 비우고 싶다면 주석 해제
        // if (parents[index]) Destroy(parents[index].gameObject);
        // parents[index] = null;
    }

    public void DestroyAll() {
        ClearAll();
        for (int i = 0; i < pools.Length; i++) {
            Destroy(i);
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 키(문자열) 기반의 Variant 풀 매니저.
/// - Register(key, prefab[, initialSize, groupParent]) 로 등록
/// - Prewarm(key, count) / Prewarm(plan) 로 프리워밍
/// - GetByKey(key[, parentOverride]) 로 가져오기
/// - SetCap(key, cap) 로 풀 상한 지정(TrimExcess 시 적용)
/// - Clear/Destroy/DestroyAll 로 정리
/// </summary>
public class NewPoolManager : Singleton<NewPoolManager>
{
    // ─────────────────────────────────────────────────────────────────────────────
    // 내부 상태
    // ─────────────────────────────────────────────────────────────────────────────
    // key → index
    private readonly Dictionary<string, int> _keyToIndex = new Dictionary<string, int>();
    // index → prefab
    private readonly List<GameObject> _prefabs = new List<GameObject>();
    // index → pool
    private readonly List<List<GameObject>> _pools = new List<List<GameObject>>();
    // index → parent group
    private readonly List<Transform> _parents = new List<Transform>();
    // index → cap(상한). 기본 -1(무제한)
    private readonly List<int> _caps = new List<int>();

    // 풀 그룹 이름 prefix
    private const string GroupPrefix = "Pool_";

    // ─────────────────────────────────────────────────────────────────────────────
    // 등록/설정
    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// 프리팹을 키로 등록. 이미 등록된 키는 기존 index 를 반환한다.
    /// </summary>
    /// <param name="key">Variant 논리명(예: "Enemy_Goblin")</param>
    /// <param name="prefab">프리팹</param>
    /// <param name="initialSize">선행 생성할 개수(프리워밍)</param>
    /// <param name="groupParent">Hierarchy에서 그룹을 묶을 상위 Transform(없으면 NewPoolManager 하위에 생성)</param>
    /// <returns>등록된 내부 index</returns>
    public int Register(string key, GameObject prefab, int initialSize = 0, Transform groupParent = null)
    {
        if (string.IsNullOrEmpty(key) || prefab == null)
        {
            Debug.LogError("[NewPoolManager] Register 실패: key 또는 prefab 이 null");
            return -1;
        }

        if (_keyToIndex.TryGetValue(key, out int exist))
        {
            // 이미 등록되어 있으면 프리워밍만 적용
            if (initialSize > 0)
                Prewarm(key, initialSize);
            return exist;
        }

        int idx = _prefabs.Count;
        _keyToIndex[key] = idx;
        _prefabs.Add(prefab);
        _pools.Add(new List<GameObject>());
        _caps.Add(-1); // cap 미설정 = 무제한

        // 그룹 폴더 생성
        var parent = new GameObject($"{GroupPrefix}{key}").transform;
        parent.SetParent(groupParent ? groupParent : transform, false);
        _parents.Add(parent);

        if (initialSize > 0)
            Prewarm(key, initialSize);

        return idx;
    }

    /// <summary>특정 키의 풀 상한(cap)을 설정. -1 이면 무제한.</summary>
    public void SetCap(string key, int cap)
    {
        if (!TryGetIndex(key, out int idx)) return;
        _caps[idx] = cap < -1 ? -1 : cap;
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // 프리워밍
    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>해당 키로 count 개수를 미리 생성(비활성)해 둔다.</summary>
    public void Prewarm(string key, int count)
    {
        if (count <= 0) return;
        if (!TryGetIndex(key, out int idx)) return;

        var list = _pools[idx];
        int need = count - list.Count;
        for (int i = 0; i < need; i++)
            CreateNew(idx).SetActive(false);
    }

    /// <summary>스테이지 스폰 테이블 등을 그대로 넣어 일괄 프리워밍.</summary>
    public void Prewarm(Dictionary<string, int> plan)
    {
        if (plan == null) return;
        foreach (var kv in plan)
            Prewarm(kv.Key, kv.Value);
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // 가져오기 / 반납 헬퍼
    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// 키로 객체 가져오기. 비활성 오브젝트가 있으면 재사용, 없으면 새로 생성.
    /// parentOverride 가 있으면 그 부모로 붙임(로컬값 유지), 없으면 풀 그룹 부모 유지.
    /// </summary>
    public GameObject GetByKey(string key, Transform parentOverride = null)
    {
        if (!TryGetIndex(key, out int idx))
        {
            Debug.LogError($"[NewPoolManager] Get 실패: 등록되지 않은 key = {key}");
            return null;
        }

        var pool = _pools[idx];
        GameObject select = null;

        // 비활성 재사용
        for (int i = 0; i < pool.Count; i++)
        {
            var obj = pool[i];
            if (obj == null) continue;
            if (!obj.activeSelf)
            {
                select = obj;
                break;
            }
        }

        if (select == null)
            select = CreateNew(idx);

        // 부모 지정
        if (parentOverride != null)
            select.transform.SetParent(parentOverride, false);
        else
            select.transform.SetParent(_parents[idx], false);

        select.SetActive(true);
        return select;
    }

    /// <summary>
    /// 외부에서 수동으로 반납하고 싶을 때 호출(선택 사항).
    /// cap이 설정되어 있고 비활성이 cap 초과면 초과분 정리(TrimExcess)도 가능.
    /// </summary>
    public void Return(GameObject obj, bool trimIfOverCap = false)
    {
        if (obj == null) return;
        obj.SetActive(false);
        if (trimIfOverCap)
            TrimExcessAll();
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // 정리/파괴
    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>특정 키 풀에서 '비활성' 객체만 모두 삭제(메모리 해제)</summary>
    public void Clear(string key)
    {
        if (!TryGetIndex(key, out int idx)) return;
        Clear(idx);
    }

    /// <summary>모든 풀에서 '비활성' 객체만 삭제</summary>
    public void ClearAll()
    {
        for (int i = 0; i < _pools.Count; i++)
            Clear(i);
    }

    /// <summary>특정 키 풀을 완전히 파괴(활성 포함), 그룹 폴더는 유지</summary>
    public void DestroyPool(string key)
    {
        if (!TryGetIndex(key, out int idx)) return;
        DestroyPool(idx);
    }

    /// <summary>모든 풀을 완전히 파괴(활성 포함). Scene 전환 시 호출 적합.</summary>
    public void DestroyAll()
    {
        ClearAll();
        for (int i = 0; i < _pools.Count; i++)
            DestroyPool(i);
    }

    /// <summary>cap 이 설정된 풀들에 대해 비활성 초과분을 제거</summary>
    public void TrimExcessAll()
    {
        for (int i = 0; i < _pools.Count; i++)
            TrimExcess(i);
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // 내부 유틸
    // ─────────────────────────────────────────────────────────────────────────────

    private bool TryGetIndex(string key, out int idx) => _keyToIndex.TryGetValue(key, out idx);

    private GameObject CreateNew(int idx)
    {
        var prefab = _prefabs[idx];
        if (prefab == null)
        {
            Debug.LogError($"[NewPoolManager] CreateNew 실패: prefab 이 null (idx={idx})");
            return null;
        }

        var go = Instantiate(prefab, _parents[idx], false);
        _pools[idx].Add(go); // 풀에 등록
        go.SetActive(false);
        return go;
    }

    private void Clear(int index)
    {
        var pool = _pools[index];
        // 비활성만 제거
        for (int i = pool.Count - 1; i >= 0; i--)
        {
            var obj = pool[i];
            if (obj == null)
            {
                pool.RemoveAt(i);
                continue;
            }
            if (!obj.activeSelf)
            {
                Destroy(obj);
                pool.RemoveAt(i);
            }
        }
    }

    private void DestroyPool(int index)
    {
        var pool = _pools[index];
        for (int i = pool.Count - 1; i >= 0; i--)
        {
            if (pool[i] != null)
                Destroy(pool[i]);
        }
        pool.Clear();
        // 그룹 폴더는 유지(원하면 아래 주석 해제)
        // if (_parents[index]) Destroy(_parents[index].gameObject);
        // _parents[index] = null;
    }

    private void TrimExcess(int index)
    {
        int cap = _caps[index];
        if (cap < 0) return; // 무제한

        var pool = _pools[index];

        // 현재 비활성 개수 계산
        int inactiveCount = 0;
        for (int i = 0; i < pool.Count; i++)
            if (pool[i] != null && !pool[i].activeSelf)
                inactiveCount++;

        // 초과분 파괴
        int toDestroy = Mathf.Max(0, inactiveCount - cap);
        if (toDestroy == 0) return;

        for (int i = pool.Count - 1; i >= 0 && toDestroy > 0; i--)
        {
            var obj = pool[i];
            if (obj != null && !obj.activeSelf)
            {
                Destroy(obj);
                pool.RemoveAt(i);
                toDestroy--;
            }
        }
    }
}

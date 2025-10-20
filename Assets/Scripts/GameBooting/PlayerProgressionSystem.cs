// Assets/Scripts/GameBooting/Systems/PlayerProgressionSystem.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 플레이어 성장(가구/진행도 기반) 전담 시스템.
/// - GameManager에 보관된 가구 상태(List<FurnitureIdLvPos>)를 받아 능력치를 재계산/적용한다.
/// - 최초 1회 플레이어의 '기본 스탯'을 캐시하고, 이후에는 가구 보정치만 누적해서 적용한다.
/// - 내부 이벤트(OnPlayerSpecApplied)로 적용 완료를 브로드캐스트한다.
/// 
/// 권장 연동:
/// 1) 로비에서 가구 배치/레벨 변경 시 InteriorManager가 PlayerProgressionSystem.ApplyAll() 호출
/// 2) SaveSystem 로드 완료 후 GameManager 데이터 세팅이 끝나면 ApplyAll() 호출
/// 3) 전투씬 입장 인트로가 끝나고 Stage 시작 직전에 ApplyAll() 한 번 더 안전 호출(옵션)
/// 
/// 주의:
/// - 아래 'ID 매핑' 스위치문은 프로젝트의 실제 가구 ID/효과로 맞춰 수정해줘.
/// - Player 필드명(예: maxStamina, moveSpeed, attackPower)은 네 프로젝트에 맞게 매핑 수정 필요.
/// </summary>
[DefaultExecutionOrder(+50)]
public class PlayerProgressionSystem : MonoBehaviour
{
    // -------- Singleton --------
    public static PlayerProgressionSystem Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // -------- Events --------
    [System.Serializable]
    public class PlayerSpecAppliedEvent : UnityEvent { }
    /// <summary>스펙 재계산/적용이 끝났을 때 발생</summary>
    public PlayerSpecAppliedEvent OnPlayerSpecApplied = new PlayerSpecAppliedEvent();

    // -------- Base cache (captured from Player on first apply) --------
    private bool _baseCaptured;
    private float _baseMaxHP;
    private float _baseMaxStamina;
    private float _baseMoveSpeed;
    private float _baseAttackPower;

    // 필요하다면 여기에 더 캐시(방어력, 투사체속도 등)를 추가
    // private float _baseDefense, _basePickupRange, ...;

    // -------- Accumulator struct --------
    private struct Accum
    {
        // add = 절대량 가산, mul = 배수 가산(예: +20% → 0.2f)
        public float addMaxHP;
        public float addMaxStamina;
        public float mulMoveSpeed;
        public float mulAttackPower;
        // public float addDefense, mulPickupRange, ...

        public void Clear()
        {
            addMaxHP = 0f;
            addMaxStamina = 0f;
            mulMoveSpeed = 0f;
            mulAttackPower = 0f;
        }
    }

    private Accum _accum;

    // -------- Public API --------

    /// <summary>
    /// GameManager가 들고 있는 현재 가구 상태로부터 전량 재계산하여 적용.
    /// </summary>
    public void ApplyAll()
    {
        var gm = GameManager.instance;
        if (gm == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning("[PlayerProgressionSystem] GameManager.instance 가 없음");
#endif
            return;
        }
        ApplyFrom(gm.furnitureIdLvPos);
    }

    /// <summary>
    /// 전달된 가구 상태 리스트로부터 전량 재계산하여 적용.
    /// </summary>
    public void ApplyFrom(List<FurnitureIdLvPos> furnitureList)
    {
        var player = GetPlayerSafe();
        if (player == null) return;

        // 1) 플레이어 기본 스탯 캡처(최초 1회)
        CaptureBaseIfNeeded(player);

        // 2) 가구 보정 누적치 초기화
        _accum.Clear();

        // 3) 가구 → 누적치 계산
        if (furnitureList != null)
        {
            foreach (var item in furnitureList)
            {
                ApplyFurnitureToAccum(item);
            }
        }

        // 4) 누적치 → 실제 스탯 적용
        ApplyAccumToPlayer(player);

        // 5) 통지
        OnPlayerSpecApplied?.Invoke();
    }

    /// <summary>
    /// 가구 1건 변경 시 호출(간단 구현: 전체 재계산)
    /// </summary>
    public void ApplyDelta(FurnitureIdLvPos changed)
    {
        ApplyAll(); // 정확성과 단순성을 위해 전체 재계산. 필요 시 최적화 가능.
    }

    // -------- Internals --------

    private Player GetPlayerSafe()
    {
        var p = Player.instance;
        if (p == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning("[PlayerProgressionSystem] Player.instance 가 없음");
#endif
        }
        return p;
    }

    private void CaptureBaseIfNeeded(Player player)
    {
        if (_baseCaptured) return;

        // ⚠️ 아래 필드명은 프로젝트 Player 스크립트에 맞게 조정하세요.
        //    없으면 간단히 TryGet 패턴이나 별도 BaseSpec 보관으로 대체.
        _baseMaxHP       = TryGet(player, nameof(player.health),        100f);
        _baseMaxStamina  = TryGet(player, nameof(player.maxHealth),   100f);
        _baseMoveSpeed   = TryGet(player, nameof(player.speed),      5f);

        _baseCaptured = true;
    }

    /// <summary>
    /// 리플렉션으로 float 필드/프로퍼티를 안전하게 읽어온다(없으면 기본값 반환).
    /// </summary>
    private float TryGet(object obj, string name, float @default)
    {
        var t = obj.GetType();
        var f = t.GetField(name);
        if (f != null && f.FieldType == typeof(float))
            return (float)f.GetValue(obj);

        var p = t.GetProperty(name);
        if (p != null && p.PropertyType == typeof(float) && p.CanRead)
            return (float)p.GetValue(obj, null);

        return @default;
    }

    /// <summary>
    /// 리플렉션으로 float 필드/프로퍼티를 안전하게 세팅(없으면 무시).
    /// </summary>
    private void TrySet(object obj, string name, float value)
    {
        var t = obj.GetType();
        var f = t.GetField(name);
        if (f != null && f.FieldType == typeof(float))
        {
            f.SetValue(obj, value);
            return;
        }
        var p = t.GetProperty(name);
        if (p != null && p.PropertyType == typeof(float) && p.CanWrite)
        {
            p.SetValue(obj, value, null);
        }
    }

    /// <summary>
    /// 실제로 플레이어 객체에 누적치를 적용한다.
    /// 기본값 + add, 기본값 * (1 + mul) 규칙.
    /// </summary>
    private void ApplyAccumToPlayer(Player player)
    {
        // ⚠️ 프로젝트 Player 필드/프로퍼티명에 맞게 수정할 것
        float newMaxHP       = Mathf.Max(1f, _baseMaxHP      + _accum.addMaxHP);
        float newMaxStamina  = Mathf.Max(0f, _baseMaxStamina + _accum.addMaxStamina);
        float newMoveSpeed   = Mathf.Max(0.1f, _baseMoveSpeed  * (1f + _accum.mulMoveSpeed));
        float newAttackPower = Mathf.Max(0f, _baseAttackPower * (1f + _accum.mulAttackPower));

        TrySet(player, nameof(player.maxHealth),       newMaxHP);
        TrySet(player, nameof(player.maxStamina),  newMaxStamina);
        TrySet(player, nameof(player.speed),   newMoveSpeed);

#if UNITY_EDITOR
        // Debug.Log($"[PlayerProgressionSystem] Applied -> HP:{newMaxHP} STA:{newMaxStamina} SPD:{newMoveSpeed} ATK:{newAttackPower}");
#endif
    }

    /// <summary>
    /// 단일 가구 항목을 누적치에 반영.
    /// 실제 프로젝트의 가구 ID/효과 매핑에 맞게 스위치문을 수정하세요.
    /// </summary>
    private void ApplyFurnitureToAccum(FurnitureIdLvPos idLvPos)
    {
        var dm = DataManager.instance;
        if (dm == null) return;

        // ⚠️ DataManager에서 가구 데이터 접근 방식에 맞게 교체하세요.
        // 예시: dm.furnitureDataList[idLvPos.id] 가 존재한다고 가정.
        var data = SafeGetFurnitureData(dm, idLvPos.id);
        if (data == null) return;

        int levelIndex = Mathf.Max(0, idLvPos.level - 1);
        float val = GetLevelValueSafe(data, levelIndex);

        // -------- 가구 ID → 효과 매핑 --------
        // 아래 상수는 '예시'다. 실제 프로젝트의 ID/효과에 맞게 바꿔줘.
        const int ID_TREADMILL = 3;   // 예: 러닝머신 → 최대 스태미나 +X
        const int ID_SOFA      = 7;   // 예: 소파 → 최대 HP +X
        const int ID_SHOERACK  = 11;  // 예: 신발장 → 이동속도 +Y%
        const int ID_WEAPON_BENCH = 21; // 예: 무기 작업대 → 공격력 +Y%

        switch (idLvPos.id)
        {
            case ID_TREADMILL:
                _accum.addMaxStamina += val;            // 절대량 가산
                break;
            case ID_SOFA:
                _accum.addMaxHP += val;                 // 절대량 가산
                break;
            case ID_SHOERACK:
                _accum.mulMoveSpeed += val * 0.01f;     // % 값으로 들어온다고 가정 → 배수 전환
                break;
            case ID_WEAPON_BENCH:
                _accum.mulAttackPower += val * 0.01f;   // % 값으로 들어온다고 가정
                break;
            default:
                // 기타 가구는 필요 시 매핑 추가
                break;
        }
    }

    // -------- Helpers for DataManager / FurnitureData --------

    private object SafeGetFurnitureData(DataManager dm, int id)
    {
        // 프로젝트 구조에 맞춰 수정하세요.
        // 예: ScriptableObject 배열/리스트, 딕셔너리 등
        // 여기서는 dm.furnitureDataList 가 있고 인덱스 접근이 가능하다고 가정.
        try
        {
            if (dm.furnitureDataList == null) return null;
            if (id < 0 || id >= dm.furnitureDataList.Length) return null;
            return dm.furnitureDataList[id];
        }
        catch { return null; }
    }

    private float GetLevelValueSafe(object furnitureData, int levelIndex)
    {
        // 프로젝트의 FurnitureData 구조체/클래스에 맞춰 반영.
        // 예: furnitureData.levelNums[levelIndex] 형태를 가정.
        var t = furnitureData.GetType();
        var f = t.GetField("levelNums");
        if (f != null && f.FieldType.IsArray)
        {
            var arr = f.GetValue(furnitureData) as System.Array;
            if (arr != null && arr.Length > 0)
            {
                int idx = Mathf.Clamp(levelIndex, 0, arr.Length - 1);
                object v = arr.GetValue(idx);
                if (v is float fv) return fv;
                if (v is int iv)   return iv;
            }
        }
        return 0f;
    }
}

using System.Collections;
using UnityEngine;

/// <summary>
/// 요구사항:
/// 1) 발사 후 일정 시간(startDelay) 동안 타겟 스캔 지연
/// 2) 타겟 사망 시: 회전각속도 0(조향 중지), 현재 진행 방향 유지
/// 3) 타겟 없을 시: 회전각속도 0(조향 중지), 현재 진행 방향 유지
/// </summary>
public class TrackingAddon : BulletAddon
{
    [Header("Scan / Delay")]
    [Tooltip("발사 후 스캔 시작까지 지연(초)")]
    public float startDelay = 0f;

    [Header("Targeting")]
    [Tooltip("비워두면 BulletType에 따라 자동으로 Player/Enemy 선택")]
    public string targetTagOverride = "";

    [Header("Homing")]
    [Tooltip("유효 타겟일 때만 적용되는 초당 최대 회전각속도(도/초)")]
    public float turnRateDegPerSec = 360f;
    [Tooltip("속도 보정 최소값")]
    public float minSpeed = 2f;
    [Range(0.5f, 1.5f)]
    [Tooltip("속도 유지 가중치(1=유지, <1 감속, >1 가속)")]
    public float speedPreserveFactor = 1f;
    [Tooltip("탄의 회전을 속도 벡터에 맞출지")]
    public bool alignRotationToVelocity = true;

    // 내부 상태
    public Transform _target;
    private Bullet _bullet;
    private Coroutine _loopCo;
    private Vector2 _lastNonZeroVel;
    public float _currentTurnRate; // 유효 타겟: turnRateDegPerSec, 무효: 0

    void OnEnable()
    {
        if (_bullet == null) _bullet = GetComponent<Bullet>();

        // 초기 진행방향 캐시
        if (rigid != null)
        {
            if (rigid.velocity.sqrMagnitude > 1e-6f)
                _lastNonZeroVel = rigid.velocity;
            else
                _lastNonZeroVel = (Vector2)transform.right * Mathf.Max(minSpeed, 0.01f);
        }

        _currentTurnRate = 0f; // 딜레이/무타겟 상태에서 회전각속도 0
        _loopCo = StartCoroutine(MainLoop());
    }

    void OnDisable()
    {
        if (_loopCo != null) StopCoroutine(_loopCo);
        _loopCo = null;
        _target = null;
        _currentTurnRate = 0f;
    }

    IEnumerator MainLoop()
    {
        // 1) 스캔 지연: 헤딩만 유지
        float t = 0f;
        while (t < startDelay)
        {
            MaintainHeading();
            t += Time.deltaTime;
            yield return null;
        }

        // 2) 본 루프
        while (true)
        {
            if (IsValidTarget(_target) == 1)
            {
                // 타겟이 죽음 → 타겟 해제
                _target = null;
                break;
            }
            // 타겟 유효성 평가
            if (IsValidTarget(_target) == 0)
            {
                // 타겟이 없거나 죽음 → 회전각속도 0, 방향 유지
                _currentTurnRate = 0f;
                _target = AcquireTarget(); // 즉시 한 번은 스캔 (원하면 주기스캔으로 확장)
                if (IsValidTarget(_target) == 0)
                {
                    MaintainHeading();
                    yield return null;
                    continue;
                }
            }

            // 타겟 유효 → 회전각속도 정상값으로 조향
            _currentTurnRate = turnRateDegPerSec;
            SteerTowards(_target.position, Time.deltaTime);
            yield return null;
        }
    }

    // 가장 가까운 대상 취득(없으면 null)
    Transform AcquireTarget()
    {
        string tagToFind = targetTagOverride;
        if (string.IsNullOrEmpty(tagToFind))
        {
            var type = (_bullet != null) ? _bullet.bulletType : Bullet.BulletType.Player;
            tagToFind = (type == Bullet.BulletType.Player) ? "Enemy" : "Player";
        }

        if (tagToFind == "Player")
        {
            var player = GameObject.FindWithTag("Player");
            return player != null ? player.transform : null;
        }

        GameObject[] candidates = GameObject.FindGameObjectsWithTag(tagToFind);
        Transform best = null;
        float bestD2 = float.PositiveInfinity;
        Vector3 pos = transform.position;

        for (int i = 0; i < candidates.Length; i++)
        {
            var go = candidates[i];
            if (go == null || !go.activeInHierarchy) continue;
            float d2 = (go.transform.position - pos).sqrMagnitude;
            if (d2 < bestD2)
            {
                bestD2 = d2;
                best = go.transform;
            }
        }
        return best;
    }

    int IsValidTarget(Transform t)
    {
        // 추가 조건: 체력 컴포넌트가 있으면 살아있는지 검사 등
        if (t == null) return 0;
        if (t.gameObject == null) return 0;
        if (!t.gameObject.activeInHierarchy) return 0;
        // Enemy 타겟이면 체력 검사
        if (t.CompareTag("Enemy") && t.GetComponent<Enemy>() != null)
            if (t.GetComponent<Enemy>().health <= 0) 
                return 1;
        return 2;
    }

    // 타겟이 없거나 사망했을 때: 현재/마지막 헤딩으로 직진 유지
    void MaintainHeading()
    {
        if (rigid == null) return;

        Vector2 baseVel = rigid.velocity.sqrMagnitude > 1e-6f ? rigid.velocity : _lastNonZeroVel;
        if (baseVel.sqrMagnitude < 1e-6f)
            baseVel = (Vector2)transform.right * Mathf.Max(minSpeed, 0.01f);

        float speed = Mathf.Max(minSpeed, baseVel.magnitude * speedPreserveFactor);
        Vector2 newVel = baseVel.normalized * speed;
        rigid.velocity = newVel;

        if (newVel.sqrMagnitude > 1e-6f)
            _lastNonZeroVel = newVel;

        if (alignRotationToVelocity && newVel.sqrMagnitude > 1e-6f)
        {
            float z = Mathf.Atan2(newVel.y, newVel.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, z);
        }
    }

    // 타겟 유효할 때만 호출: 현재 회전각속도(_currentTurnRate)로 조향
    void SteerTowards(Vector3 targetPos, float dt)
    {
        if (rigid == null) return;

        Vector2 v = rigid.velocity;
        float speed = Mathf.Max(minSpeed, v.magnitude);
        Vector2 toTarget = ((Vector2)(targetPos - transform.position));
        if (toTarget.sqrMagnitude < 1e-10f)
        {
            MaintainHeading();
            return;
        }

        Vector2 dirToTarget = toTarget.normalized;
        float angNow = Mathf.Atan2(v.y, v.x);
        float angTarget = Mathf.Atan2(dirToTarget.y, dirToTarget.x);

        float deltaRad = Mathf.DeltaAngle(angNow * Mathf.Rad2Deg, angTarget * Mathf.Rad2Deg) * Mathf.Deg2Rad;
        float maxDeltaRad = Mathf.Max(0f, _currentTurnRate) * Mathf.Deg2Rad * dt; // 타겟 없으면 0 → 회전 없음
        float clamped = Mathf.Clamp(deltaRad, -maxDeltaRad, maxDeltaRad);

        float newAng = angNow + clamped;
        float newSpeed = Mathf.Max(minSpeed, speed * speedPreserveFactor);
        Vector2 newVel = new Vector2(Mathf.Cos(newAng), Mathf.Sin(newAng)) * newSpeed;

        rigid.velocity = newVel;

        if (newVel.sqrMagnitude > 1e-6f)
            _lastNonZeroVel = newVel;

        if (alignRotationToVelocity && newVel.sqrMagnitude > 1e-6f)
        {
            float z = Mathf.Atan2(newVel.y, newVel.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, z);
        }
    }
}

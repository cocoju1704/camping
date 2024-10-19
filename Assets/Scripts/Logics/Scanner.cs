using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    public float scanRange; // 스캔 범위
    public LayerMask targetLayer; // 타겟으로 삼을 레이어
    public RaycastHit2D[] targets; // 타겟 후보들 저장
    public Transform nearestTarget;
    public Rigidbody2D thisRigidbody; // 플레이어의 이동 방향을 계산하기 위한 참조

    void FixedUpdate() {
        targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, targetLayer);
        nearestTarget = GetNearest();
        thisRigidbody = GetComponent<Rigidbody2D>();
    }

    public Transform GetNearest() {
        Transform result = null;
        float bestScore = float.MaxValue;

        Vector2 playerPosition = transform.position;
        Vector2 playerDirection = thisRigidbody.velocity.normalized; // 플레이어의 이동 방향 벡터

        foreach (RaycastHit2D target in targets) {
            // enemy도 loot도 아닌 경우는 제외
            if (!target.transform.CompareTag("Enemy") && !target.transform.CompareTag("Loot")) continue;
            // 원거리 무기인 경우 loot에 조준 x
            if (GetComponent<WeaponSystem>().currentWeaponIndex == 0) {
                if (target.transform.CompareTag("Loot")) continue;
            }
            Vector2 targetPosition = target.transform.position;
            float distance = Vector2.Distance(playerPosition, targetPosition);

            // 플레이어 이동 방향과 적 위치 사이의 각도 계산
            Vector2 directionToTarget = (targetPosition - playerPosition).normalized;
            float angle = Vector2.Angle(playerDirection, directionToTarget);

            // 가중치 계산: 각도가 작을수록 가중치를 높게 설정
            float weight = 1 + (angle / 180f); // 각도가 0이면 weight는 1, 각도가 180이면 weight는 2

            // 가중치와 거리를 조합한 점수 계산
            float score = distance * weight;

            // 가장 점수가 낮은 타겟 선택 (가까우면서도 플레이어 이동 방향에 가까운 적)
            if (score < bestScore) {
                bestScore = score;
                result = target.transform;
            }
        }

        return result;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    public float scanRange; // 스캔 범위
    public LayerMask targetLayer; // 타겟으로 삼을 레이어
    public RaycastHit2D[] targets; // 타겟 후보들 저장
    public Transform nearestTarget;

    void FixedUpdate() {
        targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, targetLayer);
        nearestTarget = GetNearest();
    }
    Transform GetNearest() {
        Transform result = null;
        float diff = 100;
        foreach (RaycastHit2D target in targets) {
            if (!target.transform.CompareTag("Enemy")) continue;
            Vector3 myPos = transform.position;
            Vector3 targetPos = target.transform.position;
            float curDiff = Vector2.Distance(myPos, targetPos);
            if (curDiff < diff) {
                diff = curDiff;
                result = target.transform;
            }
        }
        return result;
    }
}

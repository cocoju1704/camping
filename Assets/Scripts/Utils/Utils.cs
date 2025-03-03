using System.Collections.Generic;
using UnityEngine;

public static class Utils {
    // InteriorSystem에서 World 좌표를 Grid로 변환할때 쓰는 함수들
    public static List<Vector2Int> GetFurniturePos(Vector2Int leftDown, Vector2Int size) {
        List<Vector2Int> pos = new List<Vector2Int>();
        for (int i = 0; i < size.x; i++) {
            for (int j = 0; j < size.y; j++) {
                pos.Add(new Vector2Int(leftDown.x + i, leftDown.y + j));
            }
        }
        return pos;
    }
    public static Vector2Int WorldToGrid(Vector3 worldPos) {
        return new Vector2Int(Mathf.RoundToInt(worldPos.x + 5.5f), Mathf.RoundToInt(worldPos.y + 2f));
    }
    public static Vector2 GetCenterPos(Vector2Int pos, Vector2Int size) { // 가구의 중심 좌표를 계산할 때 사용
        float offsetX = -5.5f + (size.x - 1) / 2f;
        float offsetY = -2f + (size.y - 1) / 2f;
        Vector2 realPos = new Vector2(pos.x + offsetX, pos.y + offsetY);
        return realPos;
    }
    public static List<Vector2Int[]> _precomputedPatterns;
    public static List<Vector2Int[]> precomputedPatterns {
        get {
            if (_precomputedPatterns == null) {
                _precomputedPatterns = new List<Vector2Int[]>
                {
                    new Vector2Int[] {
                    new Vector2Int(-4, -4), new Vector2Int(-3, -4), new Vector2Int(-2, -4), new Vector2Int(-1, -4), new Vector2Int(0, -4),
                    new Vector2Int(-4, -4), new Vector2Int(-3, -4), new Vector2Int(-2, -4), new Vector2Int(-1, -4), new Vector2Int(0, -4),
                    new Vector2Int(-4, -3), new Vector2Int(-3, -3), new Vector2Int(-2, -3), new Vector2Int(-1, -3), new Vector2Int(0, -3),
                    new Vector2Int(-4, -2), new Vector2Int(-3, -2), new Vector2Int(-2, -2), new Vector2Int(-1, -2), new Vector2Int(0, -2),
                    new Vector2Int(-4, -1), new Vector2Int(-3, -1), new Vector2Int(-2, -1), new Vector2Int(-1, -1), new Vector2Int(0, -1),
                    new Vector2Int(-4, 0), new Vector2Int(-3, 0), new Vector2Int(-2, 0), new Vector2Int(-1, 0), new Vector2Int(0, 0),
                    new Vector2Int(-4, 0), new Vector2Int(-3, 0), new Vector2Int(-2, 0), new Vector2Int(-1, 0),
                    new Vector2Int(-1, 1), new Vector2Int(-1, 2), new Vector2Int(-1, 3), new Vector2Int(-2, 3), new Vector2Int(-3, 3)
                }
                };
            }
            return _precomputedPatterns;
        }
    }

    public static Vector2Int[] GetRandomBlockPattern() {
        return precomputedPatterns[UnityEngine.Random.Range(0, precomputedPatterns.Count)];
    }
}
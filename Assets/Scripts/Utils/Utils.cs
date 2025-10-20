using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    // ===== Grid Config =====
    // 그리드 원점(월드 좌표에서 (0,0) 셀의 "왼쪽 아래 코너" 위치)
    private static readonly Vector2 GRID_ORIGIN = new Vector2(-5.5f, -2f);
    // 셀 한 칸 크기
    private const float CELL_SIZE = 1f;

    // ===== Helpers =====
    public static Vector2Int WorldToGrid(Vector3 worldPos)
    {
        // 왼쪽-아래(LD) 셀 인덱스를 구할 때는 Floor가 일반적
        float gx = (worldPos.x - GRID_ORIGIN.x) / CELL_SIZE;
        float gy = (worldPos.y - GRID_ORIGIN.y) / CELL_SIZE;
        return new Vector2Int(Mathf.FloorToInt(gx), Mathf.FloorToInt(gy));
    }

    // 그리드의 "왼쪽 아래" pos와 크기 size를 가진 가구의 월드 중심 좌표
    public static Vector2 GetCenterPos(Vector2Int pos, Vector2Int size)
    {
        // 왼쪽 아래 코너 → 중심: (size - 1)/2 + 0.5 = size/2
        Vector2 centerInCells = new Vector2(pos.x + size.x * 0.5f, pos.y + size.y * 0.5f);
        return GRID_ORIGIN + centerInCells * CELL_SIZE;
    }

    // 왼쪽 아래 기준 범위를 채우는 좌표들
    public static List<Vector2Int> GetFurniturePos(Vector2Int leftDown, Vector2Int size)
    {
        var list = new List<Vector2Int>(size.x * size.y);
        for (int i = 0; i < size.x; i++)
            for (int j = 0; j < size.y; j++)
                list.Add(new Vector2Int(leftDown.x + i, leftDown.y + j));
        return list;
        // 필요 시 HashSet으로 중복 제거 가능
    }

    // ===== Precomputed Patterns =====
    private static List<Vector2Int[]> _precomputedPatterns;
    public static List<Vector2Int[]> precomputedPatterns
    {
        get
        {
            if (_precomputedPatterns == null)
            {
                _precomputedPatterns = new List<Vector2Int[]>();

                // 패턴 A: 5x5 정사각형(중복 없음)
                _precomputedPatterns.Add(MakeRect(5, 5, new Vector2Int(-4, -4)));

                // 패턴 B: L자 블록
                _precomputedPatterns.Add(Dedup(new[]
                {
                    new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(2,0),
                    new Vector2Int(0,1),
                    new Vector2Int(0,2)
                }));

                // 패턴 C: 계단형
                _precomputedPatterns.Add(Dedup(new[]
                {
                    new Vector2Int(-3,0), new Vector2Int(-2,0), new Vector2Int(-1,0),
                    new Vector2Int(-2,1), new Vector2Int(-1,1),
                    new Vector2Int(-1,2)
                }));
            }
            return _precomputedPatterns;
        }
    }

    public static Vector2Int[] GetRandomBlockPattern()
    {
        var list = precomputedPatterns;
        int idx = UnityEngine.Random.Range(0, list.Count);
        return list[idx];
    }

    // ===== Internals =====
    private static Vector2Int[] MakeRect(int w, int h, Vector2Int leftDown)
    {
        var res = new List<Vector2Int>(w * h);
        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                res.Add(new Vector2Int(leftDown.x + x, leftDown.y + y));
        return res.ToArray();
    }

    private static Vector2Int[] Dedup(IEnumerable<Vector2Int> coords)
    {
        var set = new HashSet<Vector2Int>();
        foreach (var c in coords) set.Add(c);
        var arr = new Vector2Int[set.Count];
        set.CopyTo(arr);
        return arr;
    }
}

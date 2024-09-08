using System.Collections.Generic;
using UnityEngine;

public static class Utils {
    public static Vector3 GetNearestGround(Vector3 position) {
        RaycastHit hit;
        if (Physics.Raycast(position, Vector3.down, out hit, 100f)) {
            return hit.point;
        }
        return position;
    }
    public static List<Vector2Int> GetFurniturePos(Vector2Int leftDown, Vector2Int size) {
        List<Vector2Int> pos = new List<Vector2Int>();
        for (int i = 0; i < size.x; i++) {
            for (int j = 0; j < size.y; j++) {
                pos.Add(new Vector2Int(leftDown.x + i, leftDown.y + j));
            }
        }
        return pos;
    }
}
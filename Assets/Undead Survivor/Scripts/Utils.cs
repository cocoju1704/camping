using UnityEngine;

public static class Utils {
    public static Vector3 GetNearestGround(Vector3 position) {
        RaycastHit hit;
        if (Physics.Raycast(position, Vector3.down, out hit, 100f)) {
            return hit.point;
        }
        return position;
    }
}
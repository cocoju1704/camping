using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteriorSystem : MonoBehaviour
{
    // 가구 좌표계
    Dictionary<Vector2Int, InteriorTile> grid = new Dictionary<Vector2Int, InteriorTile>();
    public GameObject[] furniturePrefabs;
    public GameObject interiorTilePrefab;

    void Start()
    {
        Init();
    }

    void Init() {
        for (int i = 0; i < 12; i++) {
            for (int j = 0; j < 5; j++) {
                GameObject stove = Instantiate(interiorTilePrefab, GetPos(new Vector2Int(i, j)), Quaternion.identity);
                stove.transform.parent = transform;
                InteriorTile tile = stove.GetComponent<InteriorTile>();
                tile.Init();
                grid.Add(new Vector2Int(i, j), tile);
            }
        }
    }
    Vector2 GetPos(Vector2Int pos) {
        float offsetX = -5.5f;
        float offsetY = -2f;
        Vector2 realPos = new Vector2(pos.x + offsetX, pos.y + offsetY);
        return realPos;
    }
}

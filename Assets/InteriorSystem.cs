using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteriorSystem : MonoBehaviour
{
    // 가구 좌표계
    Dictionary<Vector2Int, InteriorTile> grid = new Dictionary<Vector2Int, InteriorTile>();
    public GameObject interiorTilePrefab;

    void Start()
    {
        Init();
    }

    void Init() {
        PlaceTile();
        PlaceFurniture(new Vector2Int(0, 0), DataManager.instance.furnitureData[0]);
        PlaceFurniture(new Vector2Int(3, 0), DataManager.instance.furnitureData[3]);
    }

    // 타일 배치
    void PlaceTile() {
        for (int i = 0; i < 12; i++) {
            for (int j = 0; j < 5; j++) {
                GameObject tileObj = Instantiate(interiorTilePrefab, GetCenterPos(new Vector2Int(i, j), new Vector2Int(1, 1)), Quaternion.identity);
                tileObj.transform.parent = transform.GetChild(0);
                InteriorTile tile = tileObj.GetComponent<InteriorTile>();
                tile.Init();
                grid.Add(new Vector2Int(i, j), tile);
            }
        }
    }
    // 가구 크기에 따라 가구 중심 위치 계산
    Vector2 GetCenterPos(Vector2Int pos, Vector2Int size) {
        float offsetX = -5.5f + (size.x - 1)  / 2f;
        float offsetY = -2f + (size.y - 1) / 2f;
        Vector2 realPos = new Vector2(pos.x + offsetX, pos.y + offsetY);
        return realPos;
    }
    // 가구 배치
    public void PlaceFurniture(Vector2Int placePos, FurnitureData furnitureData) {
        if (!TryPlace(placePos, furnitureData)) return;
        List<Vector2Int> pos = Utils.GetFurniturePos(placePos, furnitureData.size);
        int id = furnitureData.id;
        foreach (Vector2Int p in pos) {
            if (grid.ContainsKey(p)) {
                grid[p].isOccupied = true;
                grid[p].furnitureId = id;
            }
        }
        Furniture furniture = Instantiate(DataManager.instance.furniturePrefab).GetComponent<Furniture>();
        furniture.transform.position = GetCenterPos(placePos, furnitureData.size);
        furniture.Init(furnitureData);
    }
    // 가구 배치 가능한지 확인
    public bool TryPlace(Vector2Int placePos, FurnitureData furnitureData) {
        List<Vector2Int> pos = Utils.GetFurniturePos(placePos, furnitureData.size);
        foreach (Vector2Int p in pos) {
            if (!grid.ContainsKey(p) || grid[p].isOccupied) {
                return false;
            }
        }
        return true;
    }
}

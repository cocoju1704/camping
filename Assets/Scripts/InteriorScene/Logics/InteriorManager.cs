using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteriorManager : MonoBehaviour, ISavable // 로비 씬에서 캠핑카 내부 관리
{
    // 가구 좌표계
    Dictionary<Vector2Int, InteriorTile> grid = new Dictionary<Vector2Int, InteriorTile>();
    public List<FurnitureIdLvPos> placedFurnitureList;
    public GameObject interiorTilePrefab;
    public SpriteRenderer furniturePreview;
    public float playerLoadTimeout = 5f;
    const int TILE_WIDTH = 12;
    const int TILE_HEIGHT = 5;
    void Start()
    {
        StartCoroutine(InitAfterPlayer());
    }

    IEnumerator InitAfterPlayer() {
        float elapsedTime = 0f;
        while (Player.instance == null) {
            yield return null;
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= playerLoadTimeout) {
                Debug.LogError("Player initialization timed out.");
                yield break;
            }
        }
        placedFurnitureList = GameManager.instance.furnitureIdLvPos;
        PlaceTile();
    }

    // 타일 배치
    void PlaceTile() {
        for (int tileWidth = 0; tileWidth < TILE_WIDTH; tileWidth++) {
            for (int tileHeight = 0; tileHeight < TILE_HEIGHT; tileHeight++) {
                GameObject tileObj = Instantiate(interiorTilePrefab, Utils.GetCenterPos(new Vector2Int(tileWidth, tileHeight), new Vector2Int(1, 1)), Quaternion.identity);
                tileObj.transform.parent = transform.GetChild(0);
                InteriorTile tile = tileObj.GetComponent<InteriorTile>();
                tile.gameObject.name = $"Tile ({tileWidth}, {tileHeight})";
                tile.Init();
                grid.Add(new Vector2Int(tileWidth, tileHeight), tile);
            }
        }
    }
    // 가구 배치
    public void PlaceFurniture(Vector2Int placePos, FurnitureData data) {
        if (!TryPlace(placePos, data)) return;
        // 재료 부족하면 false, 충분하면 재료 소진하고 진행
        if (!GameManager.instance.storage.UseMaterials(data.levelIngredients[0].materials)) return;
        List<Vector2Int> furniturePos = Utils.GetFurniturePos(placePos, data.size);
        int id = data.id;
        foreach (Vector2Int pos in furniturePos) {
            if (grid.ContainsKey(pos)) {
                grid[pos].isOccupied = true;
                grid[pos].furnitureId = id;
            }
        }
        Furniture furniture = Instantiate(DataManager.instance.furniturePrefab).GetComponent<Furniture>();
        furniture.transform.position = Utils.GetCenterPos(placePos, data.size);
        placedFurnitureList.Add(new FurnitureIdLvPos(id, 1, placePos));
        furniture.Init(data);
    }
    // 가구 배치 가능한지 확인
    public bool TryPlace(Vector2Int placePos, FurnitureData data) {

        if (placePos.x < 0 || placePos.x + data.size.x > TILE_WIDTH || placePos.y < 0 || placePos.y + data.size.y > TILE_HEIGHT) return false;
        // 이미 설치한 가구이면 false
        if (placedFurnitureList.Exists(x => x.id == data.id)) return false;
        // 재료 부족하면 false
        if (!GameManager.instance.storage.CheckStorage(data.levelIngredients[0].materials)) return false;
        List<Vector2Int> furniturePos = Utils.GetFurniturePos(placePos, data.size);
        foreach (Vector2Int pos in furniturePos) {
            if (!grid.ContainsKey(pos) || grid[pos].isOccupied) {
                return false;
            }
        }
        return true;
    }

    public void ShowFurniturePreview(FurnitureData furnitureData) {
        if (furnitureData == null) {
            Debug.LogError("FurnitureData is null.");
            return;
        }
        // 플레이어 발이 기준이라 -0.5f 해줘야함
        Vector3 playerPos = Player.instance.transform.position - new Vector3(0, 0.5f, 0);
        Vector2Int placePos = Utils.WorldToGrid(playerPos);
        furniturePreview.transform.position = Utils.GetCenterPos(placePos, furnitureData.size);
        furniturePreview.sprite = furnitureData.icon;
        if (TryPlace(placePos, furnitureData)) {
            furniturePreview.color = Color.green;
        } else {
            furniturePreview.color = Color.red;
        }
    }
    public void HideFurniturePreview() {
        furniturePreview.sprite = null;
    }
    public int FindFurnitureLv(int id) {
        return placedFurnitureList.Find(x => x.id == id).level;
    }
    // ISavable
    public void Save(GameData gameData) {
        gameData.furnitureIdLvPos = placedFurnitureList;
    }
    public void Load(GameData gameData) {
        placedFurnitureList = gameData.furnitureIdLvPos;
        foreach (FurnitureIdLvPos data in placedFurnitureList) {
            PlaceFurniture(data.pos, DataManager.instance.furnitureDataList[data.id]);
        }
    }
}

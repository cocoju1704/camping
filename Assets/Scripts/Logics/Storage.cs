using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour, ISavable {
    // 캠핑카의 인벤토리
    public Dictionary<int, int> itemList;
    void Awake() {
        itemList = new Dictionary<int, int>();
        Debug();
    }
    public void AddToStorage(int id, int amount) {
        if (itemList.ContainsKey(id)) {
            itemList[id] += amount;
        } else {
            itemList.Add(id, amount);
        }
    }
    // 전투에서 플레이어의 인벤토리 아이템 캠핑카 인벤토리로 이동
    public void AddToStorage(Dictionary<int, int> inventory) {
        foreach (KeyValuePair<int, int> item in inventory) {
            AddToStorage(item.Key, item.Value);
        }
    }
    public bool CheckStorage(Vector2Int material) {
        if (!itemList.ContainsKey(material.x) || itemList[material.x] < material.y) return false;
        return true;
    }
    public bool CheckStorage(List<Vector2Int> materials) {
        foreach (Vector2Int material in materials) {
            if (!CheckStorage(material)) return false;
        }
        return true;
    }
    public void RemoveFromStorage(List<Vector2Int> materials) {
        foreach (Vector2Int material in materials) {
            itemList[material.x] -= material.y;
        }
    }
    public bool UseMaterials(List<Vector2Int> materials) {
        if (!CheckStorage(materials)) 
        {
            GameManager.instance.ShowTempMessage("재료가 부족합니다.");
            return false;
        }
        RemoveFromStorage(materials);
        return true;
    }
    public void Save(GameData data) {
        data.storage = itemList;
    }
    public void Load(GameData data) {
        itemList = data.storage;
    }
    void Debug() {
        AddToStorage(new Dictionary<int, int> {
        });
    }
}
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour, ISavable {
    public Dictionary<int, int> itemList;
    void Awake() {
        itemList = new Dictionary<int, int>();
        Debug();
    }
    // Storage 관련
    public void AddToStorage(int id, int amount) {
        if (itemList.ContainsKey(id)) {
            itemList[id] += amount;
        } else {
            itemList.Add(id, amount);
        }
    }
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
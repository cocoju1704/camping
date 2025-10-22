using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{ // 전투 스테이지에서 플레이어의 인벤토리
    [SerializeField] public Dictionary<int, int> itemList;
    public int maxInventory = 8;
    public UnityEvent onInventoryChange;

    void Start() {
        itemList = new Dictionary<int, int>();
        onInventoryChange = new UnityEvent();
        InitForDebug();
    }

    public void Obtain(Ingredient material) { // 재료가 충돌 시 호출
        if (itemList.ContainsKey(material.ingredientId)) {
            itemList[material.ingredientId] += 1;
        } else {
            itemList.Add(material.ingredientId, 1); 
        }
        onInventoryChange.Invoke();
    }

    public void InitForDebug() {
        itemList.Add(0, 60);
        itemList.Add(1, 60);
        itemList.Add(2, 60);
        itemList.Add(3, 60);
        itemList.Add(4, 60);
        itemList.Add(5, 60);
        itemList.Add(6, 60);
        onInventoryChange.Invoke();
    }
    public bool HasSpace() {
        return itemList.Count < maxInventory;
    }
    public void AddToStorage() {
        GameManager.instance.storage.AddToStorage(itemList);
    }
}
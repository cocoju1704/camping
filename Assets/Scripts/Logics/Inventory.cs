using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    [SerializeField] public Dictionary<int, int> itemList;
    public int maxInventory = 8;
    public UnityEvent onInventoryChange;

    void Start() {
        itemList = new Dictionary<int, int>();
        onInventoryChange = new UnityEvent();
        if (StageManager.instance != null) {
            StageManager.instance.onStageWrapUp.AddListener(AddToStorage);
        }
        InitForDebug();
    }

    public void Obtain(Material material) {
        if (itemList.ContainsKey(material.materialId)) {
            itemList[material.materialId] += 1;
        } else {
            itemList.Add(material.materialId, 1); 
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventorySystem : MonoBehaviour
{
    [SerializeField] public Dictionary<int, int> inventory;
    public int maxInventory = 8;
    public UnityEvent onInventoryChange;

    void Awake() {
        inventory = new Dictionary<int, int>();
        onInventoryChange = new UnityEvent();
        InitForDebug();
    }

    public void Obtain(Material material) {
        if (inventory.ContainsKey(material.materialId)) {
            Debug.Log("Obtained1 " + material.materialName);
            inventory[material.materialId] += 1;
        } else {
            Debug.Log("Obtained2 " + material.materialName);
            inventory.Add(material.materialId, 1); 
        }
        onInventoryChange.Invoke();
    }
    public void InitForDebug() {
        inventory.Add(0, 4);
        inventory.Add(1, 4);
        inventory.Add(2, 4);
        inventory.Add(3, 4);
        onInventoryChange.Invoke();
    }
    public bool HasSpace() {
        return inventory.Count < maxInventory;
    }
}
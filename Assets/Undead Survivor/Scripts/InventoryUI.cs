using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;
public class InventoryUI : MonoBehaviour
{
    InventorySystem inventorySystem;
    InventorySlot[] slots;

    void Start()
    {
        slots = GetComponentsInChildren<InventorySlot>();
        inventorySystem = DataManager.instance.inventorySystem;
        inventorySystem.onInventoryChange.AddListener(UpdateUI);
    }
    void UpdateUI() {
        foreach (InventorySlot slot in slots) {
            slot.Init();
        }
        int i = 0;
        foreach (KeyValuePair<int, int> item in inventorySystem.inventory) {
            slots[i].SetItem(item.Key, item.Value);
            i++;
        }
    }
}

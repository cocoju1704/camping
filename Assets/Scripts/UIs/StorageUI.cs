using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StorageUI : MonoBehaviour
{
    public enum InfoType {
        inventory,
        storage,
    }
    public InfoType type;
    public Dictionary<int, int> inventory;    
    public InventorySlot[] slots;
    public Button showHideButton;
    public GameObject panel;
    public bool isShown = false;
    void Start()
    {
        slots = GetComponentsInChildren<InventorySlot>();
        if (showHideButton != null) {
            showHideButton = GetComponentInChildren<Button>();
            showHideButton.onClick.AddListener(Toggle);
            panel.SetActive(isShown);
        }
        else {
           if (panel != null) panel.SetActive(true);
        }
        
    }
    void LateUpdate() {
        foreach (InventorySlot slot in slots) {
            slot.Init();
        }
        int i = 0;
        switch (type) {
            case InfoType.inventory:
            Debug.Log(StageManager.instance.inventory.itemList.Count + " : " + slots.Length);
                foreach (KeyValuePair<int, int> item in StageManager.instance.inventory.itemList) {
                    slots[i].SetItem(item.Key, item.Value);
                    Debug.Log(i);
                    i++;
                }
                break;
            case InfoType.storage:
                // maxStorage까지만 슬롯 비활성화
                int maxStorage = GameManager.instance.carSpec.maxStorage;
                Debug.Log(maxStorage);
                for (int j = 0; j < slots.Length; j++) {
                    if (j < maxStorage) {
                        slots[j].gameObject.SetActive(true);
                    } else {
                        slots[j].gameObject.SetActive(false);
                    }
                }
                foreach (KeyValuePair<int, int> item in GameManager.instance.storage.itemList) {
                    slots[i].SetItem(item.Key, item.Value);
                    i++;
                }
                break;
        }
    }
    void Toggle() {
        isShown = !isShown;
        panel.SetActive(isShown);
    }
}

using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
public class IngredientPopup : MonoBehaviour, IToggleable
{
    public TMP_Text desc;
    public int level;
    InventorySlot[] slots;
    public GameObject panel;
    void Awake() {
        slots = GetComponentsInChildren<InventorySlot>(true);
        desc = GetComponentInChildren<TMP_Text>();
        for (int i = 0; i < slots.Length; i++) {
            slots[i].gameObject.SetActive(false);
        }
    }
    public void Set(List<Vector2Int> materials) {
        Reset();
        for (int i = 0; i < materials.Count; i++) {
            slots[i].gameObject.SetActive(true);
            slots[i].SetItem(materials[i].x, materials[i].y, true);
        }
    }
    public void Set(List<Vector2Int> materials, string description) {
        Reset();
        desc.text = description;
        Set(materials);
    }
    public void Reset() {
        foreach (InventorySlot slot in slots) {
            slot.gameObject.SetActive(false);
        }
    }
    public void Toggle() {
        panel.SetActive(!panel.activeSelf);
    }
}

using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;
public class FurniturePopup : MonoBehaviour
{
    public Image icon;
    public TMP_Text name;
    public TMP_Text desc;
    public int level;
    InventorySlot[] slots;
    void Awake() {
        slots = GetComponentsInChildren<InventorySlot>(true);
    }

    public void Set(FurnitureData data, int level) {
        icon.sprite = data.icon;
        name.text = data.name;
        this.level = level;
        desc.text = data.desc + "Lv." + level + " -> Lv." + (level + 1);
        List<Vector2Int> materials = data.levelIngredients[level-1].materials;
        for (int i = 0; i < materials.Count; i++) {
            slots[i].gameObject.SetActive(true);
            slots[i].SetItem(materials[i].x, materials[i].y);
        }
    }
    public void Reset() {
        foreach (InventorySlot slot in slots) {
            slot.gameObject.SetActive(false);
        }
    }
}

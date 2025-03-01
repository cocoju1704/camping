using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
public class IngredientPopup : MonoBehaviour, IToggleable
{  // 업그레이드에 필요한 재료 미리보기 팝업
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
    public void Set(List<Vector2Int> materials) { // 요구 아이템 미리보기에 세팅
        Reset();
        for (int i = 0; i < materials.Count; i++) {
            slots[i].gameObject.SetActive(true);
            slots[i].SetItem(materials[i].x, materials[i].y, true);
        }
    }
    public void Reset() {
        foreach (InventorySlot slot in slots) {
            slot.gameObject.SetActive(false);
        }
    }
    public void Toggle() { // 팝업 열기/닫기
        panel.SetActive(!panel.activeSelf);
    }
}

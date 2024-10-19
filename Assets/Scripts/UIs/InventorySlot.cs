using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class InventorySlot : MonoBehaviour
{
    int amount;
    public Image icon;
    public Image backgroundImg;
    TMP_Text amountText;
    public bool checkStorage = false;
    void Awake()
    {
        amountText = GetComponentInChildren<TMP_Text>();
    }

    public void SetItem(int materialId, int amount, bool checkStorage = false) {
        this.amount = amount;
        amountText.text = amount.ToString();
        icon.sprite = DataManager.instance.materialDataDict[materialId].icon;
        if (checkStorage) {
            if (!GameManager.instance.storage.CheckStorage(new Vector2Int(materialId, amount))) {
                backgroundImg.color = Color.red;
            } else {
                backgroundImg.color = Color.green;
            }
        }
    }
    public void Init() {
        amount = 0;
    }
}

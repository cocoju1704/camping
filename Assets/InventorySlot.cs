using System.Collections;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class InventorySlot : MonoBehaviour
{
    int amount;
    Image icon;
    TMP_Text amountText;
    void Awake()
    {
        icon = GetComponentsInChildren<Image>()[2];
        amountText = GetComponentInChildren<TMP_Text>();
    }
    public void SetItem(int materialId, int amount) {
        Debug.Log("SetItem");
        this.amount = amount;
        icon.sprite = DataManager.instance.materialDatas[materialId].icon;
        amountText.text = amount.ToString();
    }
    public void Init() {
        amount = 0;
    }
}

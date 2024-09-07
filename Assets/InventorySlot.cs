using System.Collections;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class InventorySlot : MonoBehaviour
{
    public int amount;
    Image icon;
    TMP_Text amountText;
    void Awake()
    {
        icon = GetComponentsInChildren<Image>()[1];
        amountText = GetComponentInChildren<TMP_Text>();
    }
    public void SetItem(int materialId, int amount) {
        this.amount = amount;
        icon.sprite = GameManager.instance.materialDatas[materialId].materialIcon;
        amountText.text = amount.ToString();
    }
    public void Init() {
        
        amount = 0;
    }
}

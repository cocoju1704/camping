using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class InventorySlot : MonoBehaviour // 인벤토리에 쓰이는 아이템 한 개가 담기는 한 칸
{
    int amount;
    public Image icon;
    public Image backgroundImg;
    TMP_Text amountText;
    public bool checkStorage = false; // 제작에서 아이템 개수 체크 미리보기에 사용
    void Awake()
    {
        amountText = GetComponentInChildren<TMP_Text>();
    }

    public void SetItem(int materialId, int amount, bool checkStorage = false) { // 칸에 아이템 설정
        this.amount = amount;
        amountText.text = amount.ToString();
        icon.sprite = DataManager.instance.materialDataDict[materialId].icon;
        if (checkStorage) { // 아이템 개수 부족하면 빨강, 충분하면 초록 배경
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

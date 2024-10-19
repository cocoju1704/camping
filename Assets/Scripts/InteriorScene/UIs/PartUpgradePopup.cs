using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PartUpgradePopup : MonoBehaviour, IToggleable
{
    [Header("데이터")]
    public CarPartsData[] carPartsList;
    public int index;

    [Header("내부 오브젝트")]
    public GameObject panel;
    public Image partImage;
    public TMP_Text partName;
    public TMP_Text partDesc;
    public IngredientPopup ingredientPopup;
    public Button toLeft;
    public Button toRight;
    public Button upgrade;
    [Header("외부 오브젝트")]
    [Header("기타")]
    public Vector2Int playerPos;
    public void Start() {
        carPartsList = DataManager.instance.carPartsDataList;
        index = 0;
        toLeft.onClick.AddListener(ToLeft);
        toRight.onClick.AddListener(ToRight);
        upgrade.onClick.AddListener(Upgrade);
        Toggle();
    }
    public void UpdateUI() {
        partImage.sprite = carPartsList[index].icon;
        partName.text = carPartsList[index].itemName;
        partDesc.text = CreateText();
        ingredientPopup.Set(carPartsList[index].levelIngredients[0].materials);
    }
    public string CreateText() {
        string temp = carPartsList[index].desc;
        temp += "\n";
        temp += "Level " + GameManager.instance.vehicleLevels[index] + " / " + (carPartsList[index].levelNum.Length-1);
        temp += "\n";
        temp += carPartsList[index].effectDesc + carPartsList[index].levelNum[GameManager.instance.vehicleLevels[index]];
        return temp;
    }
    public void ToLeft() {
        index = (index - 1 + carPartsList.Length) % carPartsList.Length;
        UpdateUI();
    }
    public void ToRight() {
        index = (index + 1) % carPartsList.Length;
        UpdateUI();
    }
    public void Toggle() {
        // 팝업 여는 경우 UpdateUI까지 호출
        Debug.Log(panel.activeSelf);
        panel.SetActive(!panel.activeSelf);
        if (panel.activeSelf) {
            UpdateUI();
        }
    }
    void Upgrade() {
        int lv = GameManager.instance.vehicleLevels[index];
        if (lv == carPartsList[index].levelNum.Length - 1) {
            return;
        }
        if (GameManager.instance.storage.UseMaterials(carPartsList[index].levelIngredients[lv].materials)) {
            GameManager.instance.vehicleLevels[index]++;
        }
        GameManager.instance.SetCarSpec();
        UpdateUI();
    }
}

using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
public class UpgradePopup : MonoBehaviour, IToggleable
{ // 가구, 무기 등 업그레이드 효과와 요구 재료를 안내해주는 UI
    [Header("데이터")]
    public FurnitureData data;
    [Header("내부 오브젝트")]
    public GameObject panel;
    public Image icon;
    public TMP_Text furnitureName;
    public TMP_Text desc;
    public Button furnitureUpgrade;

    public IngredientPopup ingredientPopup;
    [Header("외부 오브젝트")]
    public InteriorSystem interiorSystem;
    public virtual void Start()
    {
        furnitureUpgrade.onClick.AddListener(Upgrade);
        interiorSystem = FindObjectOfType<InteriorSystem>();
        Toggle();
    }

    public virtual void Set(FurnitureData data)
    {
        this.data = data;
        UpdateUI();
    }
    public void UpdateUI()
    {
        Debug.Log("InteriorSystem: " + interiorSystem);
        Debug.Log("data: " + data);
        icon.sprite = data.icon;
        furnitureName.text = data.itemName;
        desc.text = CreateText();
        ingredientPopup.Set(data.levelIngredients[GetLevel()].materials);
    }
    public virtual string CreateText()
    {
        string temp = data.desc;
        temp += "\n";
        temp += "Level " + GetLevel() + " / " + (data.levelIngredients.Length - 1);
        temp += "\n";
        temp += data.effectDesc;
        return temp;
    }

    public void Toggle()
    {
        panel.SetActive(!panel.activeSelf);
        if (panel.activeSelf)
        {
            UpdateUI();
        }
    }
    public virtual void Upgrade()
    {
        interiorSystem.UpgradeFurniture(data.id);
    }
    public int GetLevel()
    {
        return interiorSystem.FindFurnitureLv(data.id);
    }
}

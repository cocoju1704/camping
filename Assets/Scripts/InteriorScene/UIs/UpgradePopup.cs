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
{
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
    public InteriorManager interiorSystem;

    [Header("내부 변수")]
    public int level;

    public virtual void Start() {
        furnitureUpgrade.onClick.AddListener(Upgrade);
        interiorSystem = FindObjectOfType<InteriorManager>();
        Toggle();
    }

    public virtual void Set(FurnitureData data) {
        this.data = data;
        UpdateUI();
    }
    public void UpdateUI() {
        Debug.Log("InteriorSystem: " + interiorSystem);
        Debug.Log("data: " + data);
        level = interiorSystem.FindFurnitureLv(data.id);
        icon.sprite = data.icon;
        furnitureName.text = data.itemName;
        desc.text = CreateText();
        ingredientPopup.Set(data.levelIngredients[level].materials);
    }
    public virtual string CreateText() {
        string temp = data.desc;
        temp += "\n";
        temp += "Level " + level + " / " + (data.levelIngredients.Length-1);
        temp += "\n";
        temp += data.effectDesc;
        return temp;
    }

    public void Toggle() {
        panel.SetActive(!panel.activeSelf);
        if (panel.activeSelf) {
            UpdateUI();
        }
    }
    public virtual void Upgrade() 
    {
        if (level >= data.levelIngredients.Length - 1) return;
        if (GameManager.instance.storage.UseMaterials(data.levelIngredients[level].materials)) {
            interiorSystem.placedFurnitureList.Find(x => x.id == data.id).level++;
            level = interiorSystem.FindFurnitureLv(data.id);
        }
        GameManager.instance.SetPlayerSpec();

        UpdateUI();
    }
}

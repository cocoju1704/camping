using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using Unity.Collections;

public class CraftingTablePopup : UpgradePopup { // 조합대 팝업창
    [Header("재료 오브젝트")]
    public Image materialImage;
    public Button create;
    public Button toLeft;
    public Button toRight;
    private int index = 0;
    private List<Vector2Int> upgradeMaterials;
    public IngredientPopup materialIngredientPopup;
    int maxIndex = MaterialRecipe.findMaxIndex(199);
    public override void Start() {
        base.Start();
        StartCoroutine(WaitForPlayerAndInitialize());
    }
    IEnumerator WaitForPlayerAndInitialize() {  // Player.instance가 준비될 때까지 대기
        while (Player.instance == null) {
            yield return null;  // 한 프레임 대기
        }
        Set();
        toLeft.onClick.AddListener(ToLeft);
        toRight.onClick.AddListener(ToRight);
        create.onClick.AddListener(CraftMaterial);
    }
    private void Set() {  // 레벨에 따라 재료 설정
        int key = MaterialRecipe.GetKeyByIndex(index);
        upgradeMaterials = MaterialRecipe.recipe[key];
        Debug.Log(upgradeMaterials.Count);
        Debug.Log(materialIngredientPopup);
        // materialID로 찿기
        materialImage.sprite = DataManager.instance.materialDataDict[key].icon;
        materialIngredientPopup.Set(upgradeMaterials);
    }
    public void ToRight() {  
        index = (index + 1) % maxIndex;
        int key = MaterialRecipe.GetKeyByIndex(index);
        Set();
    }
    public void ToLeft() {
        index = (index - 1 + maxIndex) % maxIndex;
        Set();
    }
    // 재료 생성
    public void CraftMaterial() {
        if (GameManager.instance.storage.UseMaterials(upgradeMaterials)) {
            GameManager.instance.storage.AddToStorage(index + 100, 1);
        }
    }
    public override void Upgrade() {
        base.Upgrade();
        maxIndex = MaterialRecipe.findMaxIndex((int)data.levelNums[level]);
        Debug.Log(maxIndex);
    }
}
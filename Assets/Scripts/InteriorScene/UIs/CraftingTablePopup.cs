using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using Unity.Collections;

public class CraftingTablePopup : UpgradePopup {
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
    IEnumerator WaitForPlayerAndInitialize() {
        // Player.instance가 준비될 때까지 대기
        while (Player.instance == null) {
            yield return null;  // 한 프레임 대기
        }
        Set();
        toLeft.onClick.AddListener(ToLeft);
        toRight.onClick.AddListener(ToRight);
        create.onClick.AddListener(CreateMaterial);
    }
    // 무기 초기화
    private void Set() {
        int key = MaterialRecipe.GetKeyByIndex(index);
        upgradeMaterials = MaterialRecipe.recipe[key];
        Debug.Log(upgradeMaterials.Count);
        Debug.Log(materialIngredientPopup);
        // materialID로 찿기
        materialImage.sprite = DataManager.instance.materialDataDict[key].icon;
        materialIngredientPopup.Set(upgradeMaterials);
    }
    // 다음 무기 선택
    public void ToRight() {
        index = (index + 1) % maxIndex;
        int key = MaterialRecipe.GetKeyByIndex(index);
        Set();
    }

    // 이전 무기 선택
    public void ToLeft() {
        index = (index - 1 + maxIndex) % maxIndex;
        Set();
    }
    // 재료 생성
    public void CreateMaterial() {
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
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;

public class WeaponUpgradePopup : UpgradePopup {
    [Header("무기 오브젝트")]
    public Image weaponImage;
    public Button weaponUpgrade;
    public Button selectWeapon;
    public Button toLeft;
    public Button toRight;
    private int currentWeaponIndex = 0;
    private int weaponLevel;
    private WeaponData currentWeapon;
    private List<Vector2Int> upgradeMaterials;
    public IngredientPopup weaponIngredientPopup;
    public override void Start() {
        base.Start();
        StartCoroutine(WaitForPlayerAndInitialize());
    }
    IEnumerator WaitForPlayerAndInitialize() {
        // Player.instance가 준비될 때까지 대기
        while (Player.instance == null) {
            yield return null;  // 한 프레임 대기
        }
        SetWeapon();
        toLeft.onClick.AddListener(ToLeft);
        toRight.onClick.AddListener(ToRight);
        selectWeapon.onClick.AddListener(SelectWeapon);
        weaponUpgrade.onClick.AddListener(UpgradeWeapon);
    }
    // 무기 초기화
    private void SetWeapon() {
        var weaponSystem = Player.instance.GetComponent<WeaponSystem>();
        currentWeapon = GameManager.instance.obtainedWeapons[currentWeaponIndex].Item1;
        weaponLevel = GameManager.instance.obtainedWeapons[currentWeaponIndex].Item2;
        upgradeMaterials = currentWeapon.levelIngredients[weaponLevel].materials;
        weaponImage.sprite = currentWeapon.icon;
        if (weaponIngredientPopup.gameObject.activeSelf) weaponIngredientPopup.Set(upgradeMaterials);
    
    }
    public override string CreateText() {
        string temp = "";
        temp += currentWeapon.itemName + " ";
        temp += "Level [" + weaponLevel + " / " + (currentWeapon.levelIngredients.Length - 1) + "]";
        return temp;
    }        
    // 다음 무기 선택
    public void ToRight() {
        currentWeaponIndex = (currentWeaponIndex + 1) % GameManager.instance.obtainedWeapons.Count;
        SetWeapon();
        UpdateUI();
    }

    // 이전 무기 선택
    public void ToLeft() {
        currentWeaponIndex = (currentWeaponIndex - 1 + GameManager.instance.obtainedWeapons.Count) % GameManager.instance.obtainedWeapons.Count;
        SetWeapon();
        UpdateUI();
    }
    
    // 주 무기 설정
    public void SelectWeapon() {
        if (currentWeapon.type == WeaponData.WeaponType.Melee) {
            Player.instance.GetComponent<WeaponSystem>().SetSecondaryWeapon(currentWeapon);
        }
        else {
            Player.instance.GetComponent<WeaponSystem>().SetPrimaryWeapon(currentWeapon);
        }
    }

    // 무기 업그레이드
    public void UpgradeWeapon() {
        //최대레벨이면 리턴
        if (weaponLevel >= currentWeapon.levelIngredients.Length - 1) return;
        var weaponSystem = Player.instance.GetComponent<WeaponSystem>();
        if (GameManager.instance.storage.UseMaterials(upgradeMaterials)) {
            weaponSystem.LevelUpWeapon(currentWeaponIndex);
            weaponLevel = GameManager.instance.obtainedWeapons[currentWeaponIndex].Item2;
            upgradeMaterials = currentWeapon.levelIngredients[weaponLevel].materials;
            weaponIngredientPopup.Set(upgradeMaterials);
        }
        UpdateUI();
        // 현재 장착된 무기라면 새로고침
        if (weaponSystem.currentWeapon.weaponData.id == currentWeapon.id) {
            weaponSystem.RefreshCurrentWeapon();
        }
    }
    public void ShowWeaponIngredientPopup() {
        weaponIngredientPopup.gameObject.SetActive(true);
        SetWeapon();
    }
    public void HideWeaponIngredientPopup() {
        weaponIngredientPopup.gameObject.SetActive(false);
    }

}
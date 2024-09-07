using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
public class VehiclePart : MonoBehaviour
{
    public VehiclePartData vehiclePartData;
    public int level;
    public int maxLevel;
    Image icon;
    TMP_Text textLevel;
    TMP_Text description;
    void Awake() {
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = vehiclePartData.partIcon;

        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>();
        textLevel = texts[0];
        description = texts[1];
        level = vehiclePartData.level;
        maxLevel = vehiclePartData.maxLevel;
        GetComponent<Button>().onClick.AddListener(OnClick);
    }
    void LateUpdate() {
        textLevel.text = "Lv." + (level);
        description.text = vehiclePartData.partDesc;
    }
    // 재료 확인
    public bool CheckInventory(){
        return true;
    }
    public void OnClick() {
        if (CheckInventory()) {
            PartLevelUp();
        }
    }
    void PartLevelUp() {
        level++;
        switch(vehiclePartData.partType) {
            case VehiclePartData.PartType.Bumper:
                DataManager.instance.vehicleData.carHealth = vehiclePartData.baseNum * vehiclePartData.levelMultipliers[level];
                break;
            case VehiclePartData.PartType.Engine:
                DataManager.instance.vehicleData.callTime = vehiclePartData.baseNum * vehiclePartData.levelMultipliers[level];
                break;
            case VehiclePartData.PartType.Wheel:
                DataManager.instance.vehicleData.boardTime = vehiclePartData.baseNum * vehiclePartData.levelMultipliers[level];
                break;
            case VehiclePartData.PartType.Battery:
                DataManager.instance.vehicleData.maxBattery = (int)Mathf.Floor(vehiclePartData.levelMultipliers[level]);
                break;
        }
        if (level == vehiclePartData.levelMultipliers.Length) {
            GetComponent<Button>().interactable = false;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>, ISavable {

    [Header("게임 오브젝트")]
    public Player player;
    public CarSpec carSpec;
    public GameObject playerPrefab;
    public Storage storage;
    [Header("저장 정보")]
    public List<int> vehicleLevels = new List<int> { 0, 0, 0, 0 };
    public List<FurnitureIdLvPos> furnitureIdLvPos = new List<FurnitureIdLvPos>();
    public List<int> stageNoList;
    public int stageNoIdx;
    public List<(WeaponData, int)> obtainedWeapons = new List<(WeaponData, int)>(); // 플레이어가 소유한 무기와 레벨

    // 이벤트들
    protected override void Awake() {
        storage = GetComponent<Storage>();
        InitPlayer();
        InitCarSpec();
    }
    public void InitCarSpec() {
        carSpec = new CarSpec(100, 5, 10, 3, 8);
    }
    public void InitPlayer() {
        player = Instantiate(playerPrefab).GetComponent<Player>();
        player.name = "Player";
        player.transform.parent = transform;
    }
    public void SetCarSpec() {
        for (int i = 0; i < vehicleLevels.Count; i++) {
            int level = vehicleLevels[i];
            switch (i) {
                case 0:
                    carSpec.carHealth = DataManager.instance.carPartsDataList[i].levelNum[level];
                    break;
                case 1:
                    carSpec.callTime = DataManager.instance.carPartsDataList[i].levelNum[level];
                    break;
                case 2:
                    carSpec.boardTime = DataManager.instance.carPartsDataList[i].levelNum[level];
                    break;
                case 3:
                    carSpec.maxBattery = (int)DataManager.instance.carPartsDataList[i].levelNum[level];
                    break;
            }
        }
    }
    public void SetPlayerSpec() {
        foreach (var idLvPos in furnitureIdLvPos) {
            FurnitureData data = DataManager.instance.furnitureDataList[idLvPos.id];
            int level = idLvPos.level -1;
            switch (idLvPos.id) {
                case 0:
                    // 재료 조합 해금
                    break;
                case 1:
                    // 무기 팝업에서 알아서
                    break;
                case 2: // Gym
                    break;
                case 3: // TreadMill
                    Player.instance.maxStamina = data.levelNums[level];
                    break;
                case 4: // Crate
                    carSpec.maxStorage = (int)data.levelNums[level];
                    break;
                case 5: // Shooting Range
                    Player.instance.reloadMultiplier = data.levelNums[level] / 100f;
                    break;
                case 6: //
                    Player.instance.maxHealth = (int)data.levelNums[level];
                    break;
                default:
                    break;
            }
            Player.instance.OnPlayerSpecChanged.Invoke();
        }                    
    }
    public void ShowTempMessage(string message) {
        GameObject tempMessage = PoolManager.instance.Get("DamageText");
        tempMessage.transform.position = player.transform.position + new Vector3(0, 1, 0);
        tempMessage.GetComponent<TempText>().SetTempText(message, Color.red, 3);
    }
    public void Save(GameData data) {
        data.vehicleLevels = vehicleLevels;
        data.furnitureIdLvPos = furnitureIdLvPos;
        data.stageNoList = stageNoList;
        data.stageNoIdx = stageNoIdx;
    }
    public void Load(GameData data) {
        vehicleLevels = data.vehicleLevels;
        furnitureIdLvPos = data.furnitureIdLvPos;
        stageNoList = data.stageNoList;
        stageNoIdx = data.stageNoIdx;
        SetCarSpec();
        SetPlayerSpec();
    }
}


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

public class GameManager : Singleton<GameManager>, ISavable, ILinkOnSceneLoad
{
    [Header("Prefabs")]
    public GameObject playerPrefab;

    [Header("게임매니저 연관 오브젝트")]
    public Player player;
    public CarSpec carSpec;
    public Storage storage;
    [Header("저장 정보")]
    public List<int> vehicleLevels = new List<int> { 0, 0, 0, 0 };
    public List<FurnitureIdLvPos> furnitureIdLvPos = new List<FurnitureIdLvPos>();
    public List<int> stageNoList;
    public List<(WeaponData, int)> obtainedWeapons = new List<(WeaponData, int)>(); // 플레이어가 소유한 무기와 레벨
    public int stageNoIdx = 0;
    public int stageData;
    public string SaveKey => "GameManager";
    // 이벤트들
    protected override void Awake()
    {
        storage = GetComponent<Storage>();
        InstantiatePlayer();
        InitCarSpec();
        SaveBus.Register(this);
    }
    void OnDestroy()
    {
        SaveBus.Unregister(this);
    }
    public void InitCarSpec()
    {
        carSpec = new CarSpec(100, 5, 10, 3, 8);
    }
    public void InstantiatePlayer()
    {
        player = Instantiate(playerPrefab).GetComponent<Player>();
        player.name = "Player";
        player.transform.parent = transform;
    }
    public void DestroyPlayer()
    {
        if (player != null)
            Destroy(player.gameObject);
    }
    public void SetCarSpec()
    {
        carSpec.SetCarSpec(vehicleLevels);
    }
    public void SetPlayerSpec()
    {
        player.SetPlayerSpec(furnitureIdLvPos);
    }
    public void ShowTempMessage(string message)
    {
        GameObject tempMessage = PoolManager.instance.Get("DamageText");
        tempMessage.transform.position = player.transform.position + new Vector3(0, 1, 0);
        tempMessage.GetComponent<TempText>().SetTempText(message, Color.red, 3);
    }
    public void ToNextStageIdx()
    {
        if (stageNoIdx < stageNoList.Count - 1)
            stageNoIdx++;
        else
            stageNoIdx = 0;
    }
    public void Save(GameData data)
    {
        data.vehicleLevels = vehicleLevels;
        data.furnitureIdLvPos = furnitureIdLvPos;
        data.stageNoList = stageNoList;
        data.stageNoIdx = stageNoIdx;
        storage.Save(data);
        // Player, carspec은 어차피 가구 기준으로 재계산하니까 저장 X
    }
    public void Load(GameData data)
    {
        vehicleLevels = data.vehicleLevels;
        furnitureIdLvPos = data.furnitureIdLvPos;
        stageNoList = data.stageNoList;
        stageNoIdx = data.stageNoIdx;
        storage.Load(data);
        SetCarSpec();
        SetPlayerSpec();
    }
    public void LinkObjectsOnSceneLoad()
    {
        player = GameManager.instance.player;
    }
}


using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
public class DataManager : Singleton<DataManager>
{
    [Header("# Prefabs")]
    [Tooltip("게임 내에서 배치될 가구 프리팹 (기본 가구 생성에 사용)")]
    public GameObject furniturePrefab;

    [Header("# Scriptable Data")]
    [Tooltip("캠핑카 부품 데이터 (업그레이드/교체용)")]
    public CarPartsData[] carPartsDataList;

    [Tooltip("가구 데이터 (설치, 배치, 상호작용에 사용)")]
    public FurnitureData[] furnitureDataList;

    [Tooltip("재료 데이터 (id 기반 접근을 위해 Dictionary로 관리)")]
    public Dictionary<int, MaterialData> materialDataDict;

    [Tooltip("무기 데이터 (id 기반 접근을 위해 Dictionary로 관리)")]
    public Dictionary<int, WeaponData> weaponDataDict;

    [Tooltip("드랍 아이템 데이터 (전리품/재료 드랍 정보)")]
    public LootData[] lootDataList;

    [Tooltip("일반 적 데이터")]
    public EnemyData[] enemyDataList;

    [Tooltip("보스 적 데이터")]
    public EnemyData[] bossDataList;

    [Tooltip("스테이지 데이터 (맵, 웨이브 정보 등)")]
    public StageData[] stageDataList;
    protected override void Awake()
    {
        LoadAllData();
        Init();
    }
    void Init()
    {
    }
    // 데이터 로딩 관련
    void LoadAllData()
    {
        // List 형태로 저장
        furnitureDataList = Resources.LoadAll<FurnitureData>("Data/Furnitures");
        carPartsDataList = Resources.LoadAll<CarPartsData>("Data/CarParts");
        lootDataList = Resources.LoadAll<LootData>("Data/Loots");
        enemyDataList = Resources.LoadAll<EnemyData>("Data/Enemies");
        bossDataList = Resources.LoadAll<EnemyData>("Data/Bosses");
        stageDataList = Resources.LoadAll<StageData>("Data/Stages");
        materialDataDict = new Dictionary<int, MaterialData>();
        weaponDataDict = new Dictionary<int, WeaponData>();
        //material, weapon 데이터는 인덱스 기반으로 불러올 수 있도록 딕셔너리로 변환
        foreach (MaterialData materialData in Resources.LoadAll<MaterialData>("Data/Materials"))
        {
            materialDataDict.Add(materialData.id, materialData);
        }
        foreach (WeaponData weaponData in Resources.LoadAll<WeaponData>("Data/Weapons"))
        {
            weaponDataDict.Add(weaponData.id, weaponData);
        }
    }
    // 새 게임 시작 시 초기화
}









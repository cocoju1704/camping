using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
public class DataManager : Singleton<DataManager>
{
    [Header("#Prefab")]
    public GameObject furniturePrefab;
    [Header("#Scriptable Data")]
    public CarPartsData[] carPartsDataList;
    public FurnitureData[] furnitureDataList;
    public Dictionary<int, MaterialData> materialDataDict;
    public Dictionary<int, WeaponData> weaponDataDict;
    public LootData[] lootDataList;
    public EnemyData[] enemyDataList;
    public EnemyData[] bossDataList;
    public StageData[] stageDataList;

    protected override void Awake() {
        LoadAllData();
        Init();
    }
    void Init() {
    }
    // 데이터 로딩 관련
    void LoadAllData() {
        furnitureDataList = Resources.LoadAll<FurnitureData>("Data/Furnitures");
        materialDataDict = new Dictionary<int, MaterialData>();
        foreach (MaterialData materialData in Resources.LoadAll<MaterialData>("Data/Materials")) {
            materialDataDict.Add(materialData.id, materialData);
        }
        carPartsDataList = Resources.LoadAll<CarPartsData>("Data/CarParts");
        weaponDataDict = new Dictionary<int, WeaponData>();
        foreach (WeaponData weaponData in Resources.LoadAll<WeaponData>("Data/Weapons")) {
            weaponDataDict.Add(weaponData.id, weaponData);
        }
        lootDataList = Resources.LoadAll<LootData>("Data/Loots");
        enemyDataList = Resources.LoadAll<EnemyData>("Data/Enemies");
        bossDataList = Resources.LoadAll<EnemyData>("Data/Bosses");
        stageDataList = Resources.LoadAll<StageData>("Data/Stages");
    }
    void LoadAllPrefab() {
    }

}





[Serializable]
public class CarSpec {
    public float carHealth; // 범퍼: 캠핑카 체력
    public float callTime; // 엔진: 캠핑카 도착까지 걸리는 시간
    public float boardTime; // 바퀴: 캠핑카 탑승까지 걸리는 시간
    public int maxBattery; // 배터리: 캠핑카 최대 배터리
    public int maxStorage;
    public CarSpec(float health, float callTime, float boardTime, int maxBattery, int maxStorage) {
        this.carHealth = health;
        this.callTime = callTime;
        this.boardTime = boardTime;
        this.maxBattery = maxBattery;
        this.maxStorage = maxStorage;
    }
}
[Serializable]
public class PlayerSpec {
    public float health;
    public int maxHealth;
    public float speed;
    public PlayerSpec() {
        health = 100;
        maxHealth = 100;
        speed = 5;
    }
    public PlayerSpec(float health, int maxHealth, float speed) {
        this.health = health;
        this.maxHealth = maxHealth;
        this.speed = speed;
    }
}

[Serializable]
public class GameData {
    public PlayerSpec playerSpec;
    public int stageNo;
    public List<FurnitureIdLvPos> furnitureIdLvPos;
    public Dictionary<int, int> storage;
    public List<int> vehicleLevels;
    public GameData() : this(new PlayerSpec(), 0, new Dictionary<Vector2Int, FurnitureData>()) { }
    public GameData(PlayerSpec playerData, int stageNo, Dictionary<Vector2Int, FurnitureData> furnitureData) {
        this.playerSpec = playerData;
        this.stageNo = stageNo;
        this.furnitureIdLvPos = new List<FurnitureIdLvPos>();
    }
    public List<KeyValuePair<int, int>> obtainedWeaponLvs = new List<KeyValuePair<int, int>>();
    public Vector2Int playerSelectedWeapon;
    public List<int> stageNoList = new List<int>();
    public int stageNoIdx;
}

[Serializable]
public class LevelUpMaterials {
    public List<Vector2Int> materials;
}
[Serializable]
public class FurnitureIdLvPos {
    public int id;
    public int level;
    public Vector2Int pos;
    public FurnitureIdLvPos(int id, int level, Vector2Int pos) {
        this.id = id;
        this.level = level;
        this.pos = pos;
    }
}
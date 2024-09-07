using System;
using System.Collections.Generic;
using UnityEngine;
public class DataManager : Singleton<DataManager>
{
    public VehicleData vehicleData;
    public PlayerData playerData;
    public SpawnData[] spawnData;
    public SpawnData[] spawnResourceData;
    public List<VehiclePartData> vehiclePartDataList = new List<VehiclePartData>();
    public List<ItemData> itemDataList = new List<ItemData>();
    public InventorySystem inventorySystem;
    void Awake() {
        base.Awake();
        inventorySystem = GetComponent<InventorySystem>();
    }
}
[Serializable]
public class VehicleData {
    public float carHealth; // 범퍼: 캠핑카 체력
    public float callTime; // 엔진: 캠핑카 도착까지 걸리는 시간
    public float boardTime; // 바퀴: 캠핑카 탑승까지 걸리는 시간
    public int maxBattery; // 배터리: 캠핑카 최대 배터리
}
[Serializable]
public class PlayerData {
    public int health;
    public int maxHealth;
}
[Serializable]
public class SpawnData {
    public int spriteType;
    public float spawnTime;
    public int health;
    public int exp;
    public float speed;
    public int damage;
    public int materialType;
    public int dropRate;
}

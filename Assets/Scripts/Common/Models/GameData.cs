using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData { // 저장해야될 데이터 묶음
    [Tooltip("현재 스테이지 번호")]
    public int stageNo;
    [Tooltip("설치된 가구 ID, 레벨, 위치")]
    public List<FurnitureIdLvPos> furnitureIdLvPos;
    [Tooltip("스토리지")]
    public Dictionary<int, int> storage;
    [Tooltip("캠핑카 스펙")]
    public List<int> vehicleLevels;
    public List<KeyValuePair<int, int>> obtainedWeaponLvs = new List<KeyValuePair<int, int>>();
    public Vector2Int playerSelectedWeapon;
    public List<int> stageNoList = new List<int>();
    public int stageNoIdx;
}
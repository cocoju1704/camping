using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CarSpec
{
    public float carHealth; // 범퍼: 캠핑카 체력
    public float callTime; // 엔진: 캠핑카 도착까지 걸리는 시간
    public float boardTime; // 바퀴: 캠핑카 탑승까지 걸리는 시간
    public int maxBattery; // 배터리: 캠핑카 최대 배터리
    public int maxStorage;
    public CarSpec(float health, float callTime, float boardTime, int maxBattery, int maxStorage)
    {
        this.carHealth = health;
        this.callTime = callTime;
        this.boardTime = boardTime;
        this.maxBattery = maxBattery;
        this.maxStorage = maxStorage;
    }
    public void SetCarSpec(List<int> vehicleLevels)
    {
        for (int i = 0; i < vehicleLevels.Count; i++) {
            int level = vehicleLevels[i];
            switch (i)
            {
                case 0:
                    carHealth = DataManager.instance.carPartsDataList[i].levelNum[level];
                    break;
                case 1:
                    callTime = DataManager.instance.carPartsDataList[i].levelNum[level];
                    break;
                case 2:
                    boardTime = DataManager.instance.carPartsDataList[i].levelNum[level];
                    break;
                case 3:
                    maxBattery = (int)DataManager.instance.carPartsDataList[i].levelNum[level];
                    break;
            }
        }
    }
}
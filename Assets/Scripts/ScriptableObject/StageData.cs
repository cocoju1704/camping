using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "ScriptableObjects/StageData")]
public class StageData : ScriptableObject
{
    [Tooltip("스테이지 ID")]
    public string key;
    [Tooltip("스테이지 클리어 조건이 되는 시간 (초 단위)")]
    public int stageMaxTime;

    [Tooltip("적 스폰 최소 대기 시간 (초)")]
    public int earlySpawnTime;

    [Tooltip("적 스폰 최대 대기 시간 (초)")]
    public int lateSpawnTime;

    [Tooltip("적 데이터 목록")]
    public List<EnemyData> enemyDataList;

    [Tooltip("스테이지 마지막 등장하는 보스 데이터 목록")]
    public List<EnemyData> bossDataList;

    [Tooltip("루트 데이터 목록")]
    public List<LootData> lootDataList;
    [Tooltip("등장 적 프리팹")]
    public List<GameObject> blockPrefabList;
    
}
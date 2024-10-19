using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "StageData", menuName = "ScriptableObjects/StageData")]
public class StageData : ScriptableObject {
    public int stageExp; // 보스 소환시 필요 경험치
    public int earlySpawnTime; // 적 소환 시간(후반)
    public int lateSpawnTime; // 적 소환 시간(초반)
    public List<EnemyData> enemyDataList;
    public List<EnemyData> bossDataList;
    public List<LootData> lootDataList;

}
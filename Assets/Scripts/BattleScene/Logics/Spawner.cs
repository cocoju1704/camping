using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Spawner : MonoBehaviour { // BattleScene서 적 및 자원의 생성을 담당하는 스크립트
    public StageManager stageManager;
    public Transform[] spawnPoints;
    List<EnemyData> enemyData;
    List<EnemyData> bossData;
    List<LootData> lootData;
    float earlySpawnTime;
    float lateSpawnTime;
    float stageTimer; // 스테이지 진행 시간
    float monsterTimer = 0; // 적 스폰 타이머. 적 스폰 간격에 도달하면 적을 스폰하고 초기화
    static float stageMaxTime = 30f;
    float enemySpawnTime;
    [Header("# Debug")]
    public bool isDebugMode = true;
    void Start() {
        spawnPoints = GetComponentsInChildren<Transform>();
        stageManager = GetComponent<StageManager>();
    }
    public void Init()
    { // 해당 스테이지의 데이터(SO)를 받아 초기화
        enemyData = stageManager.stageData.enemyDataList;
        bossData = stageManager.stageData.bossDataList;
        lootData = stageManager.stageData.lootDataList;
        earlySpawnTime = stageManager.stageData.earlySpawnTime;
        lateSpawnTime = stageManager.stageData.lateSpawnTime;
        stageManager.onStageMaxTime.AddListener(SpawnBoss);
        
    }
    void Update()
    {
        //Debug();
        if (isDebugMode) return;
        transform.position = GameManager.instance.player.transform.position;
        stageTimer = StageManager.instance.currentTime;
        if (stageTimer > stageMaxTime) stageTimer = stageMaxTime;
        enemySpawnTime = Mathf.Lerp(earlySpawnTime, lateSpawnTime, stageTimer / stageMaxTime);
        monsterTimer += Time.deltaTime;
        if (monsterTimer > enemySpawnTime)
        {
            monsterTimer = 0;
            //SpawnEnemy();
        }
    }
    void Debug() {
        if (Keyboard.current.f1Key.wasPressedThisFrame) {
            SpawnBoss();
        }
        if (Keyboard.current.f2Key.wasPressedThisFrame) {
            SpawnEnemy();
        }
    }
    void SpawnEnemy() { // 풀매니저를 통해 스테이지 데이터에 등록된 적 중 래덤하게 생성
        GameObject enemy = PoolManager.instance.Get("Enemy");
        enemy.transform.position = spawnPoints[UnityEngine.Random.Range(1, spawnPoints.Length)].position;
        int rand = UnityEngine.Random.Range(0, enemyData.Count);
        enemy.GetComponent<Enemy>().Init(enemyData[rand]);
    }
    public void SpawnBoss()
    {
        GameObject enemy = PoolManager.instance.Get("Enemy");
        enemy.transform.position = spawnPoints[UnityEngine.Random.Range(1, spawnPoints.Length)].position;
        enemy.GetComponent<Enemy>().Init(bossData[0]);
        // 크기 5배
        enemy.transform.localScale = new Vector3(5, 5, 5);
        //freeze xy position
        enemy.GetComponent<Rigidbody2D>().mass = 10;
        // Reposition 비활성화
        enemy.GetComponent<Reposition>().enabled = false;
        enemy.GetComponent<Enemy>().isBoss = true;
    }

}


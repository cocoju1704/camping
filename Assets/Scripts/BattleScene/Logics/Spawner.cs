using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Spawner : MonoBehaviour
{
    public StageManager stageManager;
    public Transform[] spawnPoints;
    List<EnemyData> enemyData;
    List<EnemyData> bossData;
    List<LootData> lootData;
    float earlySpawnTime;
    float lateSpawnTime;
    float stageTimer = 0f;
    float monsterTimer = 0;
    static float stageMaxTime = 30f;
    float enemySpawnTime;
    [Header("# Debug")]
    public bool isDebugMode = true;
    void Start() {
        spawnPoints = GetComponentsInChildren<Transform>();
        stageManager = GetComponent<StageManager>();
    }
    public void Init() {
        enemyData = stageManager.stageData.enemyDataList;
        bossData = stageManager.stageData.bossDataList;
        lootData = stageManager.stageData.lootDataList;
        earlySpawnTime = stageManager.stageData.earlySpawnTime;
        lateSpawnTime = stageManager.stageData.lateSpawnTime;
        enemySpawnTime = stageManager.stageData.earlySpawnTime;
    }
    void Update()
    {
        Debug();
        if (isDebugMode) return;
        transform.position = GameManager.instance.player.transform.position;
        stageTimer += Time.deltaTime;
        monsterTimer += Time.deltaTime;
        enemySpawnTime = Mathf.Lerp(earlySpawnTime, lateSpawnTime, stageTimer / stageMaxTime);
        if (monsterTimer > enemySpawnTime) {
            monsterTimer = 0;
            SpawnEnemy();
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
    void SpawnEnemy() {
        GameObject enemy = PoolManager.instance.Get("Enemy");
        enemy.transform.position = spawnPoints[UnityEngine.Random.Range(1, spawnPoints.Length)].position;
        int rand = UnityEngine.Random.Range(0, enemyData.Count);
        enemy.GetComponent<Enemy>().Init(enemyData[rand]);
    }
    public void SpawnBoss() {
        GameObject enemy = PoolManager.instance.Get("Enemy");
        enemy.transform.position = spawnPoints[UnityEngine.Random.Range(1, spawnPoints.Length)].position;
        enemy.GetComponent<Enemy>().Init(bossData[0]);
        // 크기 5배
        enemy.transform.localScale = new Vector3(5, 5, 5);
        //freeze xy position
        enemy.GetComponent<Rigidbody2D>().mass =10;
        // Reposition 비활성화
        enemy.GetComponent<Reposition>().enabled = false;
    }

}


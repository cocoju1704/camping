using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    SpawnData[] spawnData;
    SpawnData[] spawnResourceData;
    public float levelTime;
    int level;
    float timer = 0;
    void Awake() {
        spawnPoints = GetComponentsInChildren<Transform>();
        spawnData = DataManager.instance.spawnData;
        spawnResourceData = DataManager.instance.spawnResourceData;
        Debug.Log(GameManager.instance);
        levelTime = GameManager.instance.maxGameTime / spawnData.Length;
    }

    void Update()
    {
        timer += Time.deltaTime;
        level = Mathf.FloorToInt(Mathf.Min((GameManager.instance.gameTime / levelTime), spawnData.Length - 1));
        if (level < 0) return;
        if (timer > spawnData[level].spawnTime) {
            timer = 0;
            SpawnEnemy();
        }
    }
    void SpawnEnemy() {
        GameObject enemy = GameManager.instance.poolManager.Get(0);
        enemy.transform.position = spawnPoints[UnityEngine.Random.Range(1, spawnPoints.Length)].position;
        int rand = UnityEngine.Random.Range(0, level+1);
        enemy.GetComponent<Enemy>().Init(spawnData[rand]);
    }
    void SpawnResource() {
        GameObject resource = GameManager.instance.poolManager.Get(3);
        resource.transform.position = spawnPoints[UnityEngine.Random.Range(1, spawnPoints.Length)].position;
        resource.GetComponent<Enemy>().Init(spawnResourceData[level]);
    }
}


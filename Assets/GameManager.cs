using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager> {

    [Header("게임 오브젝트")]

    public Player player;
    public PoolManager poolManager;
    public MaterialData[] materialDatas;
    public GameObject playerPrefab;
    public GameObject poolManagerPrefab;
    public float gameTime;

    [Header("시작 컷씬 관련")]
    public Cinemachine.CinemachineVirtualCamera vCam;
    public float cutsceneTime = 4f;
    public float tempTime = 0f;
    public float maxGameTime = 20f;
    [Header("플레이어 정보")]
    public int health;
    public int maxHealth = 100;
    public int kills;
    [Header("탈출 이벤트 관련")]
    public int exp;
    public float ArrivalTime = 20f;

    public int stageExp = 10;

    public UnityEvent onExpFull;
    void Awake()
    {
        base.Awake();
        gameTime = -cutsceneTime;
    }
    void Start() {
        health = maxHealth;
    }
    void Update() {
        gameTime += Time.deltaTime;
        if (gameTime > maxGameTime) {
            gameTime = maxGameTime;
        }
        tempTime += Time.deltaTime;
    }
    public void GetExp(int exp) {
        this.exp += exp;
        if (this.exp >= stageExp) {
            onExpFull.Invoke();
        }
    }
}


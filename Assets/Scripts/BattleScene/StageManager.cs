using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
public class StageManager : Singleton<StageManager> {
    [Header("스테이지 데이터")]
    public StageData stageData;
    [Header("스테이지 연관 오브젝트")]
    public Spawner spawner;
    public GameObject ground;
    public Reposition[] groundRepositions;
    public CutsceneManager cutscene;
    public Inventory inventory;
    [Header("스테이지 수치")]
    public float tempTime = 0f;
    public float gameTime = 0f;
    public float maxGameTime = 20f;
    public bool timerActive = false;
    public int kills = 0;
    public int exp = 0;
    public int stageExp = 0;

    [Header("이벤트")]
    public UnityEvent onStageStart;
    public UnityEvent onExpFull;
    public UnityEvent onStageWrapUp;
    public UnityEvent onBossClear;
    public UnityEvent onGameOver;
    public void Awake() {
        // 오브젝트 연결
        spawner = GetComponentInChildren<Spawner>();
        cutscene = GetComponentInChildren<CutsceneManager>();
        groundRepositions = ground.GetComponentsInChildren<Reposition>();
        inventory = GetComponentInChildren<Inventory>();
        // 데이터 초기화
        stageData = DataManager.instance.stageDataList[0];
        // 이벤트 연결
        onStageStart.AddListener(SetStage);
        onStageStart.AddListener(StartTimer);
        onStageWrapUp.AddListener(WrapupStage);
        onGameOver.AddListener(GameOver);
    }


    void Update() {
        if (!timerActive) return;
        gameTime += Time.deltaTime;
        if (gameTime > maxGameTime) {
            gameTime = maxGameTime;
        }
        tempTime += Time.deltaTime;
    }
    void SetStage () {
        exp = 0;
        kills = 0;
        stageExp = stageData.stageExp;
        spawner.Init();
    }
    void StartTimer() {
        timerActive = true;
    }
    void WrapupStage() {
        timerActive = false;
    }
    public void GetExp(int exp) {
        this.exp += exp;
        if (this.exp >= stageExp) {
            onExpFull.Invoke();
            onExpFull.RemoveAllListeners();
        }
    }
    public void GameOver() {
        cutscene.GameOver();
    }
}
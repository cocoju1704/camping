using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.UI;
public class StageManager : Singleton<StageManager> { // 전투 스테이지 데이터 관리
    [Header("스테이지 데이터")]
    public StageData stageData;
    [Header("스테이지 연관 오브젝트")]
    public Spawner spawner;
    public GameObject ground;
    public Reposition[] groundRepositions;
    public CutsceneManager cutscene;
    public Inventory inventory;
    public SliderHUD mainSlider;
    [Header("스테이지 수치")]
    public float currentTime = 0f;
    public float stageMaxTime;
    public bool timerActive = false;
    public int currentKills = 0;
    public int currentExp = 0;
    public int maxExp = 0;

    [Header("이벤트")]
    public UnityEvent onStageStart;
    public UnityEvent onExpFull;
    public UnityEvent onStageMaxTime;
    public UnityEvent onGameOver;
    public UnityEvent onStageClear;
    public void Awake()
    {
        // 오브젝트 연결
        spawner = GetComponentInChildren<Spawner>();
        cutscene = GetComponentInChildren<CutsceneManager>();
        groundRepositions = ground.GetComponentsInChildren<Reposition>();
        inventory = GetComponentInChildren<Inventory>();
        // 데이터 초기화
        stageData = DataManager.instance.stageDataList[GameManager.instance.stageNoList[GameManager.instance.stageNoIdx]]; ;
        stageMaxTime = stageData.stageMaxTime;
        // 이벤트 연결
        onStageStart.AddListener(SetStageParams);
        onStageStart.AddListener(StartStage);
        onStageMaxTime.AddListener(ChangeSliderToCarHealth);
        onStageClear.AddListener(WrapupStage);
        onGameOver.AddListener(GameOver);
    }


    void Update()
    {
        if (!timerActive) return;
        currentTime += Time.deltaTime;
        if (currentTime >= stageMaxTime)
        {
            onStageMaxTime.Invoke();
            onStageMaxTime.RemoveAllListeners();
        }
    }
    void SetStageParams () {
        currentExp = 0;
        currentKills = 0;
        maxExp = stageData.stageMaxTime;
        spawner.Init();
        
    }
    void StartStage()
    {
        timerActive = true;
        Player.instance.ActivateWeapon();
    }
    void SetUpPoolsForStage()
    {
        // enemyPrefab 및 enemy 총알 풀에 등록
        foreach (EnemyData enemyData in stageData.enemyDataList)
        {
            NewPoolManager.instance.Register(enemyData.name, enemyData.enemyPrefab, 20, null);
            foreach (EnemyActionData enemyActionData in enemyData.enemyActionDataList)
            {
                NewPoolManager.instance.Register(enemyActionData.projectile.name, enemyActionData.projectile, 100, null);
            }
        }

        // bullet
    }
    // 탈출 누르면 스테이지 모든 param 고정
    void WrapupStage() {
        timerActive = false;
    }
    public void GetExp(int exp) {
        currentExp += exp;
        if (currentExp >= maxExp) {
            onExpFull.Invoke();
            onExpFull.RemoveAllListeners();
        }
    }
    public void ChangeSliderToCarHealth()
    {
        mainSlider.ChangeTypeTo(SliderHUD.InfoType.CarHealth);
    }
    public void GameOver()
    {
        onGameOver.Invoke();
    }
    public void CreateStage()
    {
        currentTime = 0f;
        timerActive = true;
    }
}
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextHUD : MonoBehaviour // HUD에 표시되는 텍스트
{
    public enum InfoType
    {
        Level,
        Kill,
        Time,
        CanCall,
        CallTime,
        BoardTime,
        WeaponStats,
    }

    public InfoType type;
    public Image image;
    public TMP_Text myText;
    public float time = 0f;

    // --- Lazy binding 캐시 ---
    private GameManager gm;
    private StageManager stage;
    private Player player;
    private WeaponSystem ws;
    // carSpec 타입이 무엇이든 GameManager에서 들고있는 그대로 참조
    private object carSpecRef; // 필요시 형식 지정(예: CarSpec)으로 바꿔도 됨

    void Awake()
    {
        myText = GetComponent<TMP_Text>() ?? GetComponentInChildren<TMP_Text>();
    }

    // 필요할 때만 참조를 바인딩 (없으면 다음 프레임에 재시도)
    private bool TryBind()
    {
        gm ??= GameManager.instance;
        stage ??= StageManager.instance;

        if (gm != null)
        {
            player ??= gm.player;
            carSpecRef ??= gm.carSpec;
        }

        if (player != null && ws == null)
            ws = player.GetComponent<WeaponSystem>();

        // 최소한 GameManager까지는 있어야 의미 있는 HUD 갱신이 가능
        return gm != null;
    }

    public void ChangeTypeTo(InfoType newType)
    {
        type = newType;
        time = 0f;
    }

    void LateUpdate()
    {
        time += Time.deltaTime;

        if (!TryBind()) return; // 아직 핵심 참조가 준비 안됨 → 조용히 스킵

        switch (type)
        {
            case InfoType.Kill:       // 킬 수 텍스트
                if (stage != null) UpdateKill();
                break;

            case InfoType.Time:       // 생존 시간 텍스트
                if (stage != null) UpdateTime();
                break;

            case InfoType.CanCall:    // 차량 호출 가능 텍스트
                UpdateCanCall();
                break;

            case InfoType.CallTime:   // 차량 도착까지 남은 시간 텍스트
                if (stage != null && gm?.carSpec != null)
                    UpdateCallTime(gm.carSpec.callTime - stage.currentTime, "도착까지 {0:D2}:{1:D2}");
                break;

            case InfoType.BoardTime:  // 탈출가능까지 남은 시간 텍스트
                if (stage != null && gm?.carSpec != null)
                    UpdateBoardTime(gm.carSpec.boardTime - stage.currentTime);
                break;

            case InfoType.WeaponStats: // 무기 정보 텍스트
                if (ws != null) UpdateWeaponStats();
                break;
        }
    }

    void UpdateKill()
    {
        myText.text = "Kills: " + stage.currentKills;
    }

    void UpdateTime()
    {
        float remainTime = Mathf.Max(stage.stageMaxTime - stage.currentTime, 0f);
        int min = Mathf.FloorToInt(remainTime / 60);
        int sec = Mathf.FloorToInt(remainTime % 60);
        myText.text = $"Survival Time: {min:00}:{sec:00}";
    }

    void UpdateCanCall()
    {
        myText.text = "차량 호출 가능";
    }

    void UpdateCallTime(float leftTime, string format)
    {
        int min = Mathf.FloorToInt(leftTime / 60);
        int sec = Mathf.FloorToInt(leftTime % 60);
        myText.text = string.Format(format, min, sec);
    }

    void UpdateBoardTime(float leftTime)
    {
        if (leftTime < 0)
        {
            myText.text = "탈출 가능";
        }
        else
        {
            UpdateCallTime(leftTime, "탈출 가능까지 {0:D2}:{1:D2}");
        }
    }

    void UpdateWeaponStats()
    {
        if (ws.currentWeapon == null) return;

        var weapon = ws.currentWeapon;
        if (image != null && weapon.weaponData != null)
            image.sprite = weapon.weaponData.icon;

        // 예: "Uzi+3" 같은 표기
        var name = weapon.weaponData != null ? weapon.weaponData.itemName : weapon.name;
        myText.text = $"{name}+{weapon.level}";
    }
}

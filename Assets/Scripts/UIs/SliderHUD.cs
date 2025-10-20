using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderHUD : MonoBehaviour // HUD에 표시되는 슬라이더
{
    public enum InfoType
    {
        Exp,
        Health,
        EnemyHealth,
        LootHealth,
        Mag,
        Reload,
        CarHealth,
        Stamina,
        StageTime,
    }

    public InfoType type;
    public TMP_Text myText;
    public Slider mySlider;
    public Image mySliderFill;
    public float time = 0f;

    // Lazy binding용 캐시
    private GameManager gm;
    private Player player;
    private WeaponSystem ws;
    private StageManager stage;
    private Van van;

    void Awake()
    {
        myText = GetComponent<TMP_Text>() ?? GetComponentInChildren<TMP_Text>();
        mySlider = GetComponent<Slider>();
        mySliderFill = mySlider.fillRect.GetComponent<Image>();
    }

    // 필요할 때만 참조 바인딩
    private bool TryBind()
    {
        gm ??= GameManager.instance;
        if (gm == null) return false;

        player ??= gm.player;
        stage ??= StageManager.instance;
        van ??= FindObjectOfType<Van>();
        if (player != null && ws == null)
            ws = player.GetComponent<WeaponSystem>();

        return true;
    }

    public void ChangeTypeTo(InfoType newType)
    {
        type = newType;
        time = 0f;
    }

    void LateUpdate()
    {
        if (!TryBind()) return; // 아직 GameManager나 Player가 준비되지 않은 경우 스킵

        time += Time.deltaTime;

        switch (type)
        {
            case InfoType.Exp:
                if (stage != null) UpdateExp();
                break;
            case InfoType.Health:
                if (player != null) UpdatePlayerHealth();
                break;
            case InfoType.EnemyHealth:
                var enemy = GetComponentInParent<Enemy>();
                if (enemy != null) UpdateHealth(enemy.health, enemy.maxHealth);
                break;
            case InfoType.LootHealth:
                var loot = GetComponentInParent<Loot>();
                if (loot != null) UpdateHealth(loot.health, loot.maxHealth);
                break;
            case InfoType.Mag:
                if (ws != null) UpdateMag();
                break;
            case InfoType.Reload:
                if (ws != null) UpdateReload();
                break;
            case InfoType.CarHealth:
                if (van != null) UpdateCarHealth();
                break;
            case InfoType.Stamina:
                if (player != null) UpdateStamina();
                break;
            case InfoType.StageTime:
                if (stage != null) UpdateTime();
                break;
        }
    }

    void UpdateExp()
    {
        float curExp = stage.currentExp;
        float maxExp = stage.maxExp;
        mySlider.value = maxExp > 0 ? curExp / maxExp : 0f;
    }

    void UpdateHealth(float curHealth, float maxHealth)
    {
        if (curHealth < 0) curHealth = 0;
        myText.text = ((int)curHealth).ToString();
        mySlider.value = maxHealth > 0 ? curHealth / maxHealth : 0f;
    }

    void UpdatePlayerHealth()
    {
        float curHealth = player.health;
        float maxHealth = player.maxHealth;
        myText.text = ((int)curHealth).ToString();
        mySlider.value = maxHealth > 0 ? curHealth / maxHealth : 0f;
    }

    void UpdateMag()
    {
        if (ws.currentWeapon == null) return;
        var weapon = ws.currentWeapon;

        if (weapon.isMelee)
        {
            mySlider.value = 1f;
            myText.text = "";
        }
        else
        {
            myText.text = $"{weapon.mag} / {weapon.maxMag}";
            mySlider.value = weapon.maxMag > 0 ? (float)weapon.mag / weapon.maxMag : 0f;
            if (weapon.isReloading)
                ChangeTypeTo(InfoType.Reload);
        }
    }

    void UpdateReload()
    {
        if (ws.currentWeapon == null) return;
        var weapon = ws.currentWeapon;

        mySlider.value = weapon.reloadTime > 0 ? time / weapon.reloadTime : 0f;
        myText.text = weapon.isMelee ? "" : "RELOADING";
        if (!weapon.isReloading)
            ChangeTypeTo(InfoType.Mag);
    }

    void UpdateCarHealth()
    {
        if (van == null) return;
        float curHealth = van.health;
        float maxHealth = van.maxHealth;
        myText.text = ((int)curHealth).ToString();
        mySlider.value = maxHealth > 0 ? curHealth / maxHealth : 0f;
    }

    void UpdateStamina()
    {
        float curStamina = player.stamina;
        float maxStamina = player.maxStamina;
        myText.text = $"{(int)curStamina} / {maxStamina}";
        mySlider.value = maxStamina > 0 ? curStamina / maxStamina : 0f;
    }

    void UpdateTime()
    {
        float remainTime = Mathf.Max(stage.stageMaxTime - stage.currentTime, 0f);
        mySlider.value = stage.stageMaxTime > 0 ? remainTime / stage.stageMaxTime : 0f;
        int min = Mathf.FloorToInt(remainTime / 60);
        int sec = Mathf.FloorToInt(remainTime % 60);
        myText.text = $"{min:00}:{sec:00}";
    }
}

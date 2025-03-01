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
    }

    public InfoType type;
    public TMP_Text myText;
    public Slider mySlider;
    public Image mySliderFill;
    public float time = 0f;

    void Awake()
    {
        myText = GetComponent<TMP_Text>() ?? GetComponentInChildren<TMP_Text>();
        mySlider = GetComponent<Slider>();
        mySliderFill = mySlider.fillRect.GetComponent<Image>();
    }

    public void ChangeTypeTo(InfoType newType)
    {
        type = newType;
        time = 0f;
    }

    void LateUpdate()
    {
        time += Time.deltaTime;
        switch (type)
        {
            case InfoType.Exp:
                UpdateExp();
                break;
            case InfoType.Health:
                if (GameManager.instance.player == null) return;
                UpdateHealth(GameManager.instance.player.health, GameManager.instance.player.maxHealth);
                break;
            case InfoType.EnemyHealth:
                if (GetComponentInParent<Enemy>() == null) return;
                UpdateHealth(GetComponentInParent<Enemy>().health, GetComponentInParent<Enemy>().maxHealth);
                break;
            case InfoType.LootHealth:
                if (GetComponentInParent<Loot>() == null) return;
                UpdateHealth(GetComponentInParent<Loot>().health, GetComponentInParent<Loot>().maxHealth);
                break;
            case InfoType.Mag:
                UpdateMag();
                break;
            case InfoType.Reload:
                UpdateReload();
                break;
            case InfoType.CarHealth:
                if (GameManager.instance.carSpec == null) return;
                UpdateCarHealth();
                break;
            case InfoType.Stamina:
                UpdateStamina();
                break;
        }
    }

    void UpdateExp()
    {
        float curExp = StageManager.instance.exp;
        float maxExp = StageManager.instance.stageExp;
        Debug.Log("curExp: " + curExp + ", maxExp: " + maxExp);
        mySlider.value = curExp / maxExp;
    }
    void UpdateHealth(float curHealth, float maxHealth)
    {
        if (curHealth < 0) curHealth = 0;
        myText.text = "" + curHealth;
        mySlider.value = curHealth / maxHealth;
    }

    void UpdateMag()
    {
        Weapon weapon = Player.instance.GetComponentInParent<WeaponSystem>().currentWeapon;
        if (weapon.isMelee)
        {
            mySlider.value = 1;
            myText.text = "";
        }
        else
        {
            myText.text = weapon.mag + " / " + weapon.maxMag;
            mySlider.value = (float)weapon.mag / weapon.maxMag;
            if (weapon.isReloading)
            {
                ChangeTypeTo(InfoType.Reload);
            }
        }
    }

    void UpdateReload()
    {
        Weapon weapon = Player.instance.GetComponentInParent<WeaponSystem>().currentWeapon;
        mySlider.value = time / weapon.reloadTime;
        myText.text = weapon.isMelee ? "" : "RELOADING";
        if (!weapon.isReloading) {
            ChangeTypeTo(InfoType.Mag);
        }
    }
    void UpdateCarHealth() {
        float curHealth = FindObjectOfType<Van>().health;
        float maxHealth = FindObjectOfType<Van>().maxHealth;
        myText.text = "" + curHealth;
        mySlider.value = curHealth / maxHealth;
    }
    void UpdateStamina() {
        float curStamina = Player.instance.stamina;
        float maxStamina = Player.instance.maxStamina;
        myText.text = (int)curStamina + " / " + maxStamina;
        mySlider.value = curStamina / maxStamina;
    }
}
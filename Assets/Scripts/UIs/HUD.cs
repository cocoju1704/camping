using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
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

    void Awake()
    {
        myText = GetComponent<TMP_Text>() ?? GetComponentInChildren<TMP_Text>();
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
            case InfoType.Kill:
                UpdateKill();
                break;
            case InfoType.Time:
                UpdateTime();
                break;
            case InfoType.CanCall:
                UpdateCanCall();
                break;
            case InfoType.CallTime:
                UpdateCallTime(GameManager.instance.carSpec.callTime - StageManager.instance.tempTime, "도착까지 {0:D2}:{1:D2}");
                break;
            case InfoType.BoardTime:
                UpdateBoardTime(GameManager.instance.carSpec.boardTime - StageManager.instance.tempTime);
                break;
            case InfoType.WeaponStats:
                UpdateWeaponStats();
                break;
        }
    }

    void UpdateKill()
    {
        myText.text = "Kills: " + StageManager.instance.kills;
    }

    void UpdateTime()
    {
        float remainTime = StageManager.instance.gameTime;
        int min = Mathf.FloorToInt(remainTime / 60);
        int sec = Mathf.FloorToInt(remainTime % 60);
        myText.text = string.Format("Survival Time: {0:D2}:{1:D2}", min, sec);
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
    void UpdateWeaponStats() {
        Weapon weapon = GameManager.instance.player.GetComponent<WeaponSystem>().currentWeapon;
        image.sprite = weapon.weaponData.icon;
        myText.text = weapon.weaponData.itemName + "+" + weapon.level;

    }
}
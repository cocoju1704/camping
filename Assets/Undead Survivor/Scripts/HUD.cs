using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
     public enum InfoType {
        Exp,
        Level,
        Kill,
        Time,
        Health,
        CanCall,
        CallTime,
        BoardTime,
     }
     public InfoType type;
     TMP_Text myText;
     Slider mySlider;
     void Awake() {
        myText = GetComponent<TMP_Text>();
        mySlider = GetComponent<Slider>();
     }
     public void ChangeTypeTo(InfoType type) {
        GameManager.instance.tempTime = 0;
        this.type = type;
     }
     void LateUpdate() {
        switch (type) {
            case InfoType.Exp:
                float curExp = GameManager.instance.exp;
                float maxExp = GameManager.instance.stageExp;
                mySlider.value = curExp / maxExp;
                break;
            case InfoType.Time:
                float remainTime = GameManager.instance.maxGameTime - GameManager.instance.gameTime;
                int min = Mathf.FloorToInt(remainTime / 60);
                int sec = Mathf.FloorToInt(remainTime % 60);
                myText.text = string.Format("{0:D2}:{1:D2}", min, sec);
                break;
            case InfoType.Health:
                float curHealth = DataManager.instance.playerData.health;
                float maxHealth = DataManager.instance.playerData.maxHealth;
                mySlider.value = curHealth / maxHealth;
                break;
            case InfoType.CanCall:
                myText.text = "차량 호출 가능";
                break;
            case InfoType.CallTime:
                float leftTime = DataManager.instance.vehicleData.callTime - GameManager.instance.tempTime;
                min = Mathf.FloorToInt(leftTime / 60);
                sec = Mathf.FloorToInt(leftTime % 60);
                myText.text = string.Format("도착까지 {0:D2}:{1:D2}", min, sec);
                break;
            case InfoType.BoardTime:
                leftTime = DataManager.instance.vehicleData.boardTime - GameManager.instance.tempTime;
                min = Mathf.FloorToInt(leftTime / 60);
                sec = Mathf.FloorToInt(leftTime % 60);
                if (leftTime < 0) {
                    myText.text = "탈출 가능";
                }
                else {
                    myText.text = string.Format("탈출 가능까지 {0:D2}:{1:D2}", min, sec);
                }
                break;
        }
     }
}

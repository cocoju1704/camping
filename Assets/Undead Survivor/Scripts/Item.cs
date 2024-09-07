using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;
public class Item : MonoBehaviour
{
    public ItemData data;
    public int level;
    public Weapon weapon;
    public Gear gear;
    Image icon;
    TMP_Text textLevel;
    void Awake() {
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.itemIcon;

        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>();
        textLevel = texts[0];

    }
    void LateUpdate() {
        textLevel.text = "Lv." + (level);
    }
    public void OnClick() {
        ItemLevelUp();
    }
    void ItemLevelUp() {
        switch(data.itemType) {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                if (level == 0){
                    GameObject newWeapon = new GameObject();
                    weapon = newWeapon.AddComponent<Weapon>();
                    weapon.Init(data);
                }
                else {
                    float nextDmg = data.baseDmg;
                    nextDmg += data.baseDmg * data.damages[level];
                    int nextCount = 0;
                    nextCount += data.counts[level];
                    weapon.LevelUp(nextDmg, nextCount);
                }
                level++;

                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoes:
                if (level == 0) {
                    GameObject newGear = new GameObject();
                    gear = newGear.AddComponent<Gear>();
                    gear.Init(data);
                }
                else {
                    float nextRate = data.damages[level];
                    gear.LevelUp(nextRate);
                }
                level++;
                break;
            case ItemData.ItemType.Heal:
                DataManager.instance.playerData.health = DataManager.instance.playerData.maxHealth;
                break;
        }
        if (level == data.damages.Length) {
            GetComponent<Button>().interactable = false;
        }
    }
}

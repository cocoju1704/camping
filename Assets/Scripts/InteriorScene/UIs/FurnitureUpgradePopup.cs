using TMPro;
using UnityEngine;

public class FurnitureUpgradePopup : UpgradePopup {
    public TMP_Text details;
    public override string CreateText() {
        string temp = data.effectDesc;
        temp += "\n";

        if (level >= data.levelIngredients.Length - 1) {
            temp += "최대 레벨입니다.";
            return temp;
        }
        temp += $"[Lv.{level,2}] -> [Lv.{level + 1,2}]";
        temp += "\n";
        temp += $"[{data.levelNums[level-1],-5}] -> [{data.levelNums[level],-5}]";

        return temp;
    }
}
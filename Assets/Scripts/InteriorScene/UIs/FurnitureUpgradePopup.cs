using TMPro;
using UnityEngine;

public class FurnitureUpgradePopup : UpgradePopup {
    // 가구 레벨 업그레이드 창
    public TMP_Text details;
    public override string CreateText() {
        string temp = data.effectDesc;
        temp += "\n";

        if (GetLevel() >= data.levelIngredients.Length - 1) {
            temp += "최대 레벨입니다.";
            return temp;
        }
        temp += $"[Lv.{GetLevel(),2}] -> [Lv.{GetLevel() + 1,2}]";
        temp += "\n";
        temp += $"[{data.levelNums[GetLevel()-1],-5}] -> [{data.levelNums[GetLevel()],-5}]";

        return temp;
    }
}
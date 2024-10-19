using UnityEngine;
using System;
using System.Collections.Generic;
public class UpgradableItemData : ItemData {
    [Serializable]
    public class LevelUpMaterials {
        public List<Vector2Int> materials;
    }

    public LevelUpMaterials[] levelIngredients;
    // maxLevel은 각 클래스에서 별도로 설정
}
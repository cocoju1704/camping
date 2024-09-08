using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FurnitureData", menuName = "ScriptableObjects/FurnitureData")]
public class FurnitureData : ScriptableObject
{
    public enum FurnitureType { Crafting, Upgrade, Utility, Vehicle, Active };
    [Header("# Main Info")]
    public int id;
    public string name;
    public string desc;
    public Sprite icon;
    public FurnitureType furnitureType;

    [Header("# Level Data")]
    public int level;
    public int maxLevel;
    public float baseNum;
    public LevelUpMaterials[] levelIngredients;

    [Header("# Position Data")]
    public bool isPlaced;
    public Vector2Int pos; // 가구가 차지하는 좌표 중 왼쪽 아래 좌표
    public Vector2Int size; //1x1, 1x2, 2x2, 2x3, 3x3
}
[Serializable]
public class LevelUpMaterials {
    public float multiplier;
    public List<Vector2Int> materials;
}

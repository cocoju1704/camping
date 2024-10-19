using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FurnitureData", menuName = "ScriptableObjects/FurnitureData")]
public class FurnitureData : UpgradableItemData
{
    public enum FurnitureType { Crafting, Upgrade, Utility, Vehicle, Active };
    public FurnitureType furnitureType;

    [Header("# Level Data")]
    public float[] levelNums; // 가구의 레벨에 따른 Magic Number
    public string effectDesc;

    [Header("# Position Data")]
    public Vector2Int size; //1x1, 1x2, 2x2, 2x3, 3x3
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/VehiclePartData")]
public class CarPartsData : UpgradableItemData {
    public enum PartType { Bumper, Engine, Wheel, Battery }
    public PartType type;


    [Header("# Level Data")]
    public static int maxLevel = 5;
    public float[] levelNum;
    public string effectDesc;

}
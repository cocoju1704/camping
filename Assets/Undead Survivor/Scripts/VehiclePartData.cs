using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object/VehiclePartData")]
public class VehiclePartData : ScriptableObject {
    public enum PartType { Bumper, Engine, Wheel, Battery }
    [Header("# Main Info")]
    public int partId;
    public string partDesc;
    public Sprite partIcon;
    public PartType partType;


    [Header("# Level Data")]
    public int level;
    public int maxLevel;
    public float baseNum;
    public float[] levelMultipliers;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Item", menuName = "ScriptablObjects/VehiclePartData")]
public class VehiclePartData : ScriptableObject {
    public enum PartType { Bumper, Engine, Wheel, Battery }
    [Header("# Main Info")]
    public int id;
    public string name;
    public string desc;
    public Sprite icon;
    public PartType type;


    [Header("# Level Data")]
    public int level;
    public int maxLevel;
    public float baseNum;
    public float[] levelMultipliers;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MaterialData", menuName = "ScriptableObjects/MaterialData")]
public class MaterialData : ScriptableObject
{
    public enum MaterialType { Wood, Fabric, Stone, Scrap}
    [Header("# Main Info")]
    public int id;
    public string name;
    public string desc;
    public Sprite icon;
    public int tier;

}

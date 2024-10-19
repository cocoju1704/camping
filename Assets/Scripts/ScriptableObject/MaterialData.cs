using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MaterialData", menuName = "ScriptableObjects/MaterialData")]
public class MaterialData : ItemData
{
    public enum MaterialType { Wood, Fabric, Stone, Scrap}
    [Header("# Main Info")]
    public int tier;

}

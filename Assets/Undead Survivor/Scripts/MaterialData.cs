using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MaterialData", menuName = "Scriptable Object/MaterialData")]
public class MaterialData : ScriptableObject
{
    public enum MaterialType { Wood, Fabric, Stone, Scrap}
    [Header("# Main Info")]
    public int materialId;
    public string materialName;
    public string materialDesc;
    public Sprite materialIcon;
    public int MaterialTier;

}

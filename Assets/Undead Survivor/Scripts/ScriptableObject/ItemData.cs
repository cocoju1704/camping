using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/ItemData")]
public class ItemData : ScriptableObject {
    public enum ItemType { Melee, Range, Glove, Shoes, Heal }
    [Header("# Main Info")]
    public int id;
    public string name;
    public string desc;
    public Sprite icon;
    public ItemType itemType;


    [Header("# Level Data")]
    public float baseDmg;
    public int baseCount;
    public float[] damages;
    public int[] counts;

    [Header("# Weapon")]
    public GameObject projectile;
    public Sprite hand;
}
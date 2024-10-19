using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/WeaponData")]
public class WeaponData : UpgradableItemData {
    public enum WeaponType { Melee, Pistol, Rifle, Shotgun, Sniper, Launcher, Miscellaneous }

    public int price;
    public WeaponType type;
    [Header("# 레벨 데이터")]
    public static int maxLevel = 5;
    public float[] damage = new float[maxLevel]; //대미지
    public float[] rate = new float[maxLevel]; // 연사력
    public int[] pierce = new int[maxLevel]; // 관통
    public float[] spread = new float[maxLevel]; // 탄퍼짐. 샷건의 경우 탄피 개수
    public float[] knockback = new float[maxLevel]; // 넉백
    public int[] magazine = new int[maxLevel]; // 탄창
    public float reload;

    [Header("# 투사체")]
    public GameObject projectile;
    public float speed;
}
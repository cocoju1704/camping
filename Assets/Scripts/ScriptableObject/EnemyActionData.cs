using UnityEngine;
[CreateAssetMenu(fileName = "EnemyActionData", menuName = "ScriptableObjects/EnemyActionData")]

public class EnemyActionData : ScriptableObject {
    public enum AttackType {
        Melee_Charge,
        Ranged_Normal,
        Ranged_Spread,
        Ranged_Tracking,
        Ranged_Circle,
        Ranged_Explosive,
    }
    [Header("# Projectile")]
    public AttackType attackType;
    public GameObject projectile;
    [Header("# Damage")]
    public int damage; // damage
    [Header("# Cooldown")]
    public int cooldown; // cooldown
    [Header("# NO. of Projectiles or Duration")]
    public int count; // No. of projectiles or duration
    [Header("# Speed")]
    public int speed; // speed
    [Header("# Spread")]
    public int spread; // spread
    [Header("# Range")]
    public int range; // range
    [Header("Repeat")]
    public int repeat;
}
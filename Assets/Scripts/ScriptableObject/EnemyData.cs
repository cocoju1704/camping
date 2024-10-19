using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData")]
public class EnemyData : ScriptableObject
{

    [Header("# Meta")]
    public int id;
    public string name;
    public int spawnTime;
    public bool isBoss;
    [Header("# Spec")]
    public int health;
    public int damage;
    public float speed;
    [Header("# Drop")]
    public int exp;
    public int materialType;
    public int dropRate;
    [Header("# Visual")]
    public int spriteType;
    [Header("# Actions")]
    public EnemyActionData[] enemyActionData;


}
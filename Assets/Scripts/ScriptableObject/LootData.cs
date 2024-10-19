using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LootData", menuName = "ScriptableObjects/LootData")]
public class LootData : ItemData {
    public List<Vector2Int> materialPair; // 드랍할 재료의 종류와 개수
    public float maxHealth; // 최대 타격 회수
}
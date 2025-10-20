using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FurnitureIdLvPos { //배치된 가구의 위치와 레벨 묶음
    public int id;
    public int level;
    public Vector2Int pos;
    public FurnitureIdLvPos(int id, int level, Vector2Int pos) {
        this.id = id;
        this.level = level;
        this.pos = pos;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteriorTile : MonoBehaviour // 가구를 배치할 수 있는 타일
{
    public bool isOccupied;
    public int furnitureId;
    public void Init() {
        isOccupied = false;
        furnitureId = -1;
    }
}

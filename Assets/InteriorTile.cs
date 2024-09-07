using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteriorTile : MonoBehaviour
{
    public bool isOccupied;
    public int furnitureType;
    public void Init() {
        isOccupied = false;
        furnitureType = -1;
    }
}

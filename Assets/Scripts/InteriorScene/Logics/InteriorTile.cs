using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteriorTile : MonoBehaviour
{
    public bool isOccupied;
    public int furnitureId;
    public void Init() {
        isOccupied = false;
        furnitureId = -1;
    }
}

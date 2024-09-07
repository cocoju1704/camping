using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
    public Canvas OutdoorCanvas;
    public Canvas UICanvas;
    ExitVehicle exitVehicle;
    void Awake() {
        exitVehicle = GetComponentInChildren<ExitVehicle>();
        exitVehicle.OnPlayerExitVehicle.AddListener(OpenOutdoorCanvas);
    }
    public void CloseOutdoorCanvas() {
        OutdoorCanvas.gameObject.SetActive(false);
        UICanvas.gameObject.SetActive(true);
    }
    public void OpenOutdoorCanvas() {
        OutdoorCanvas.gameObject.SetActive(true);
        UICanvas.gameObject.SetActive(false);
    }
}

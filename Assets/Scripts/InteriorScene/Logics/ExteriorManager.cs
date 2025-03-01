using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ExteriorManager : MonoBehaviour, ITriggerObserver
{ // 로비 씬 캠핑카 외부 담당
    public GameObject playerOutsidePos;
    public GameObject playerInsidePos;
    public Canvas outdoorCanvas;
    public Canvas indoorCanvas;
    public CinemachineVirtualCamera vcam;
    public PartUpgradePopup workBenckPopup;
    Player player;
    // Start is called before the first frame update
    void Start()
    {
        var pressurePlates = FindObjectsOfType<PressurePlate>();
        foreach (var plate in pressurePlates) {
            plate.Subscribe(this);
        }
        player = GameManager.instance.player;
        if (!player.gameObject.activeSelf) {
            player.gameObject.SetActive(true);
        }
        else {
            player.Activate();
            player.ActivateWeapon();
        }
        player.transform.position = playerOutsidePos.transform.position;
        vcam.Follow = transform;
    }
    public void HandleTriggerEntrance(string objName)
    {
        switch (objName)
        {
            case "VanEntrance":
                EnterVan();
                break;
            case "CampEntrance":
                //save game
                break;
            case "Workbench":
                Debug.Log("workbench");
                workBenckPopup.Toggle();
                break;
            case "NextStage":
                SceneLoadSystem.instance.LoadScene("BattleScene");
                break;
            case "VanExit":
                ExitVan();
                break;
        }
    }
    public void HandleTriggerExit(string objName)
    {
        switch (objName)
        {
            case "VanEntrance":
                break;
            case "CampEntrance":
                SaveSystem.instance.SaveFile();
                break;
            case "Workbench":
                workBenckPopup.Toggle();
                break;
            case "NextStage":
                break;
            case "VanExit":
                break;
        }
    }
    void EnterVan() {
        player.transform.position = playerInsidePos.transform.position;
        outdoorCanvas.enabled = false;
        indoorCanvas.enabled = true;
        vcam.Follow = player.transform;
    }
    void ExitVan() {
        player.transform.position = playerOutsidePos.transform.position;
        indoorCanvas.enabled = false;
        outdoorCanvas.enabled = true;
        vcam.Follow = transform;
    }

    public void OnTriggerEntered(string objName) {
        HandleTriggerEntrance(objName);
    }

    public void OnTriggerExited(string objName) {
        HandleTriggerExit(objName);
    }
}

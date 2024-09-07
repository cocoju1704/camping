using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
public class Cutscene : MonoBehaviour
{
    public GameObject van;
    public GameObject player;
    public GameObject Canvas;
    Vector3 vanDir;
    Vector3 arrivalPos;
    Vector3 endPos;
    float callTime;
    float callTimer;
    float boardTime;
    float boardTimer;
    public GameObject ground;
    public HUD timer;
    public Button escapeButton;
    public Canvas resultCanvas;
    public CinemachineVirtualCamera vcam;
    void Awake() {
        vanDir = new Vector3(1.7f, 1f, 0) * 15;
        arrivalPos = GetComponentsInChildren<Transform>()[1].position;
        endPos = arrivalPos - vanDir;
        StartCoroutine(StartCutscene());
        callTime = DataManager.instance.vehicleData.callTime;
        boardTime = DataManager.instance.vehicleData.boardTime;
        callTimer = 0;
        boardTimer = 0;
        escapeButton.onClick.AddListener(EscapeEvent);
        escapeButton.interactable = false;
        GameManager.instance.onExpFull.AddListener(EnableEscapeButton);
        player = GameManager.instance.player.gameObject;
    }
    void EnableEscapeButton() {
        escapeButton.interactable = true;
    }
    IEnumerator StartCutscene() {
        yield return StartCoroutine(VanArrive());
        yield return StartCoroutine(DropPlayer());
        yield return StartCoroutine(VanLeave());
        yield return StartCoroutine(StartStage());
    }
    IEnumerator EndCutscene() {
        yield return StartCoroutine(PickupPlayer());
        yield return StartCoroutine(VanLeave());
        yield return StartCoroutine(ShowResult());
    }
    IEnumerator VanArrive() {
        van.SetActive(true);
        van.transform.position = arrivalPos + vanDir;
        yield return van.transform.DOMove(arrivalPos, 2f).SetEase(Ease.OutQuad).WaitForCompletion();
    }
    void CallVan() {
        van.SetActive(true);
        van.transform.position = arrivalPos + vanDir;
        van.transform.DOMove(arrivalPos, 2f).SetEase(Ease.OutQuad).WaitForCompletion();
        // 플레이어랑 기타 오브젝트를 0, 0, 0으로 재배치 및 고정
        player.transform.position = Vector3.zero;
        Reposition[] repositions = ground.GetComponentsInChildren<Reposition>();
        foreach (Reposition reposition in repositions) {
            reposition.Reset();
            reposition.enabled = false;
        }
        //카메라 고정 및 플레이어 화면 밖으로 못나가게 고정
        vcam.Follow = ground.transform;
        // 모든 투사체 및 적 비활성화
        GameManager.instance.poolManager.ClearAll();
    }
    IEnumerator DropPlayer() {
        player.SetActive(true);
        player.GetComponent<PlayerInput>().enabled = false;
        player.GetComponentInChildren<Spawner>().enabled = false;
        yield return new WaitForSeconds(1f);
    }
    IEnumerator PickupPlayer() {
        //플레이어의 area 빼고 모두 비활성화
        Canvas.SetActive(false);
        player.GetComponent<SpriteRenderer>().enabled = false;
        player.GetComponent<Player>().enabled = false;
        player.GetComponent<PlayerInput>().enabled = false;
        player.GetComponentInChildren<Spawner>().enabled = false;

        foreach (Transform child in player.transform)
        {
            // 오브젝트 이름이 "Area"가 아니면 비활성화
            if (child.name != "Area")
            {
                child.gameObject.SetActive(false);
            }
        }
        yield return new WaitForSeconds(1f);
    }
    IEnumerator VanLeave() {
        van.transform.position = arrivalPos;
        yield return van.transform.DOMove(endPos, 2f).SetEase(Ease.InQuad).WaitForCompletion();
    }
    IEnumerator StartStage() {
        van.SetActive(false);
        Canvas.SetActive(true);
        player.GetComponent<PlayerInput>().enabled = true;
        player.GetComponentInChildren<Spawner>().enabled = true;
        yield return null;
    }
    IEnumerator CallVanCoroutine() {
        // Call Timer 진행
        callTimer = 0;
        while (callTimer < callTime) {
            callTimer += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }
        CallVan();
        // Board Timer UI 변경
        timer.ChangeTypeTo(HUD.InfoType.BoardTime);
        boardTimer = 0;
        // Board Timer 진행
        while (boardTimer < boardTime) {
            boardTimer += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 버튼을 다시 활성화
        escapeButton.interactable = true;
    }
    IEnumerator ShowResult() {
        resultCanvas.gameObject.SetActive(true);
        yield return null;
    }
    public void EscapeEvent() {
        Debug.Log("Escape Called");

        // 버튼의 이전 리스너를 제거하고 새로운 리스너 추가
        escapeButton.onClick.RemoveAllListeners();
        escapeButton.onClick.AddListener(EndCutScene);
        escapeButton.interactable = false;

        // 타이머 UI 변경
        timer.ChangeTypeTo(HUD.InfoType.CallTime);

        // 코루틴으로 타이머를 관리
        StartCoroutine(CallVanCoroutine());
    }
    void EndCutScene() {
        //만약 플레이어와 밴이 겹쳐있는 상태이면 실행
        StartCoroutine(EndCutscene());
    }

}

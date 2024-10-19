using System.Collections;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Unity.VisualScripting;

public class CutsceneManager : MonoBehaviour
{
    [Header("연관 오브젝트")]
    public Spawner spawner;
    public StageManager stageManager;
    [Header("참여 오브젝트")]
    public GameObject van;
    public GameObject player;
    public GameObject ground;
    public Button escapeButton;
    public HUD timer;

    [Header("캔버스")]
    public GameObject Canvas;
    public Canvas resultCanvas;
    public CinemachineVirtualCamera vcam;

    private Vector3 vanDir;
    private Vector3 arrivalPos;
    private Vector3 endPos;
    private float callTime;
    private float boardTime;

    void Start()
    {
        SetupCutscene();
        StartCoroutine(StartCutscene());
    }

    // 초기 세팅
    void SetupCutscene()
    {
        vanDir = new Vector3(1.7f, 1f, 0) * 15;
        arrivalPos = GetComponentsInChildren<Transform>()[1].position;
        endPos = arrivalPos - vanDir;
        callTime = GameManager.instance.carSpec.callTime;
        boardTime = GameManager.instance.carSpec.boardTime;
        ConfigureEscapeButton(StartEscapeSequence, false);  
        StageManager.instance.onExpFull.AddListener(EnableEscapeButton);
        StageManager.instance.onStageWrapUp.AddListener(EndGameSequence);
        player = GameManager.instance.player.gameObject;
        DisablePlayerInput();
        ground.SetActive(true);

    }

    // Escape 버튼 활성화
    void EnableEscapeButton()
    {
        escapeButton.interactable = true;
    }

    // 메인 컷신 시작
    IEnumerator StartCutscene()
    {
        yield return VanArrive();
        yield return DropPlayer();
        yield return VanLeave();
        yield return StartStage();
    }

    // 밴 도착
    IEnumerator VanArrive()
    {
        van.SetActive(true);
        van.transform.position = arrivalPos + vanDir;
        yield return van.transform.DOMove(arrivalPos, 2f).SetEase(Ease.OutQuad).WaitForCompletion();
    }

    // 플레이어 내려놓기
    IEnumerator DropPlayer()
    {
        player.SetActive(true);
        player.transform.position = Vector3Int.zero;
        vcam.Follow = player.transform;
        DisablePlayerInput();
        Player.instance.DeactivateWeapon();
        yield return new WaitForSeconds(1f);
    }

    // 밴 떠남
    IEnumerator VanLeave()
    {
        van.transform.position = arrivalPos;
        yield return van.transform.DOMove(endPos, 2f).SetEase(Ease.InQuad).WaitForCompletion();
    }

    // 스테이지 시작
    IEnumerator StartStage()
    {
        van.SetActive(false);
        Canvas.SetActive(true);
        spawner.enabled = true;
        EnablePlayerInput();
        Player.instance.ActivateWeapon();
        StageManager.instance.onStageStart.Invoke();
        yield return null;
    }

    // 플레이어 입력 비활성화
    void DisablePlayerInput()
    {
        player.GetComponent<PlayerInput>().enabled = false;
    }

    // 플레이어 입력 활성화
    void EnablePlayerInput()
    {
        player.GetComponent<PlayerInput>().enabled = true;
    }

    // 탈출 시퀀스 시작
    void StartEscapeSequence()
    {
        StartCoroutine(EscapeSequence());
    }

    // 탈출 시퀀스
    IEnumerator EscapeSequence()
    {
        ConfigureEscapeButton(StageManager.instance.onStageWrapUp.Invoke, false);

        // Call Timer 시작
        yield return RunTimer(HUD.InfoType.CallTime, callTime);

        // 밴 호출
        CallVan();

        // Board Timer 시작
        yield return RunTimer(HUD.InfoType.BoardTime, boardTime);

        // 버튼 다시 활성화
        escapeButton.interactable = true;
    }

    // 밴 호출 및 환경 리셋
    void CallVan()
    {
        van.SetActive(true);
        van.transform.position = arrivalPos + vanDir;
        van.transform.DOMove(arrivalPos, 2f).SetEase(Ease.OutQuad).WaitForCompletion();

        ResetPlayerPosition();
        LockCameraAndClearProjectiles();
    }

    // 플레이어 위치 리셋
    void ResetPlayerPosition()
    {
        player.transform.position = Vector3.zero;
        foreach (Reposition reposition in ground.GetComponentsInChildren<Reposition>())
        {
            reposition.ResetGround();
            reposition.enabled = false;
        }
    }

    // 카메라 고정 및 투사체 제거
    void LockCameraAndClearProjectiles()
    {
        vcam.Follow = ground.transform;
        PoolManager.instance.ClearExceptBlock();
    }

    // Escape 버튼 설정
    void ConfigureEscapeButton(UnityAction action, bool interactable)
    {
        escapeButton.onClick.RemoveAllListeners();
        escapeButton.onClick.AddListener(action);
        escapeButton.interactable = interactable;
    }

    // 타이머 실행
    IEnumerator RunTimer(HUD.InfoType timerType, float duration)
    {
        timer.ChangeTypeTo(timerType);
        StageManager.instance.tempTime = 0;
        float timerValue = 0;

        while (timerValue < duration)
        {
            timerValue += Time.deltaTime;
            yield return null;
        }
    }
    // 게임 종료 처리
    void EndGameSequence()
    {
        StartCoroutine(EndSequence());
    }

    // 종료 시퀀스
    IEnumerator EndSequence()
    {
        yield return DeactivatePlayerAndCanvas();
        yield return VanLeave();
        yield return ShowResultCanvas();
    }

    // 플레이어와 캔버스 비활성화
    IEnumerator DeactivatePlayerAndCanvas()
    {
        Player.instance.Deactivate();
        Player.instance.DeactivateWeapon();
        Canvas.SetActive(false);
        spawner.enabled = false;
        Reposition[] repositions = GetComponentsInChildren<Reposition>();
        foreach (Reposition reposition in repositions)
        {
            reposition.enabled = false;
        }
        yield return new WaitForSeconds(1f);

    }
    public void GameOver() {
        StartCoroutine(GameOverSequence());
    }
    // 결과 창 표시
    IEnumerator ShowResultCanvas()
    {
        PoolManager.instance.ClearAll();
        resultCanvas.gameObject.SetActive(true);
        ground.SetActive(false);
        yield return null;
    }
    IEnumerator GameOverSequence() {
        yield return DeactivatePlayerAndCanvas();
        yield return ShowGameOverCanvas();
    }
    IEnumerator ShowGameOverCanvas() {
        PoolManager.instance.ClearAll();
        resultCanvas.gameObject.SetActive(true);
        ground.SetActive(false);
        yield return null;
    }
}

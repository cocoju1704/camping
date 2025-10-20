using System.Collections;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Unity.VisualScripting;

public class CutsceneManager : MonoBehaviour // 전투 씬에서 일어나는 컷씬 관련 함수들
{
    [Header("연관 오브젝트")]
    public Spawner spawner;
    public StageManager stageManager;
    [Header("참여 오브젝트")]
    public GameObject van;
    public GameObject player;
    public GameObject ground;
    public Button returnToLobbyButton;
    [Header("캔버스")]
    public GameObject canvas;
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
        returnToLobbyButton = resultCanvas.GetComponentInChildren<Button>();
        returnToLobbyButton.onClick.AddListener(() => StartCoroutine(ReturnToLobby()));
    }
    // 초기 세팅
    void SetupCutscene()
    {
        vanDir = new Vector3(1.7f, 1f, 0) * 15;
        arrivalPos = GetComponentsInChildren<Transform>()[1].position;
        endPos = arrivalPos - vanDir;
        callTime = GameManager.instance.carSpec.callTime;
        boardTime = GameManager.instance.carSpec.boardTime;
        StageManager.instance.onStageMaxTime.AddListener(CallVan);
        StageManager.instance.onStageClear.AddListener(EndGameSequence);
        player = GameManager.instance.player.gameObject;
        DisablePlayerInput();
        ground.SetActive(true);

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
        canvas.SetActive(true);
        spawner.enabled = true;
        EnablePlayerInput();
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
        // 카메라 추적 대상을 ground로 고정
        vcam.Follow = ground.transform;

        // 현재 사이즈에서 12까지 1.5초 동안 서서히 줌아웃
        DOTween.To(() => vcam.m_Lens.OrthographicSize,
                x => vcam.m_Lens.OrthographicSize = x,
                12f, // 목표 값
                1.5f // 지속 시간(초)
        ).SetEase(Ease.OutQuad); // 부드러운 가속-감속

        // 투사체 정리
        PoolManager.instance.ClearExceptBlock();
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
        Player.instance.DeactivateWeapon();
        Player.instance.Deactivate();
        canvas.SetActive(false);
        spawner.enabled = false;
        Reposition[] repositions = GetComponentsInChildren<Reposition>();
        foreach (Reposition reposition in repositions)
        {
            reposition.enabled = false;
        }
        yield return new WaitForSeconds(1f);
    }
    public void GameOver()
    {
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
    IEnumerator GameOverSequence()
    {
        yield return DeactivatePlayerAndCanvas();
        yield return ShowGameOverCanvas();
    }
    IEnumerator ShowGameOverCanvas()
    {
        PoolManager.instance.ClearAll();
        resultCanvas.gameObject.SetActive(true);
        ground.SetActive(false);
        yield return null;
    }
    IEnumerator ReturnToLobby()
    {
        SceneFlowManager.instance.ExitBattle(true);
        yield return null;
    }
}

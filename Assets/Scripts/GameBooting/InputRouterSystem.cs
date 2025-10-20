using UnityEngine;
using UnityEngine.InputSystem; // Keyboard, Mouse
// using UnityEditor.SearchService; // 에디터 전용이면 주석 유지

// Input 시스템은 상태 패턴을 통해 구현. 어떤 씬에 있냐에 따라 입력 처리 방식이 달라짐
public class InputRouterSystem : MonoBehaviour // 이름 충돌시 클래스명 변경 권장
{
    [Header("Cached refs (Lazy Binding)")]
    [SerializeField] private Player player;          // 가능하면 인스펙터 주입
    [SerializeField] private WeaponSystem ws;        // 가능하면 인스펙터 주입
    [SerializeField] private StorageUI storageUI;    // 가능하면 인스펙터 주입

    private Camera mainCam;                          // Camera.main 캐싱

    public Vector3 mousePos;

    // 필요할 때만 참조 시도 (끊기면 다음 프레임에 다시 묶임)
    private bool TryBind()
    {
        // 메인 카메라 캐싱
        if (mainCam == null)
            mainCam = Camera.main;

        // GameManager/Player 준비 전이면 다음 프레임에 재시도
        if (player == null)
            player = GameManager.instance != null ? GameManager.instance.player : null;

        if (ws == null && player != null)
            ws = player.GetComponent<WeaponSystem>();

        // StorageUI는 존재 씬이 제한적일 수 있어 느슨하게: 필요할 때만 탐색
        if (storageUI == null)
        {
            // 이름으로 직접 찾기 → 실패 시 FindObjectOfType 1회
            var go = GameObject.Find("StorageUI");
            if (go != null) storageUI = go.GetComponent<StorageUI>();
            if (storageUI == null) storageUI = FindObjectOfType<StorageUI>(includeInactive: true);
        }

        // 핵심 입력 대상인 ws만 있어도 입력 처리는 가능
        return ws != null;
    }

    private void Update()
    {
        // 바인딩 시도 (실패해도 예외 없이 조용히 스킵)
        bool ready = TryBind();

        // 마우스 좌표 갱신 (카메라가 아직 없을 수 있으므로 가드)
        if (mainCam != null && Mouse.current != null)
        {
            mousePos = mainCam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePos.z = 0f;
        }

        // 키 입력 처리 (키보드가 없을 수도 있으니 가드)
        if (Keyboard.current == null) return;

        // 무기 스왑/리로드는 ws 준비가 되어야만 처리
        if (ready)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
                ws.SwitchWeapon();

            if (Keyboard.current.rKey.wasPressedThisFrame)
                ws.CheckClickableAndReload();
        }

        // 탭: StorageUI 토글 (필요할 때만 탐색 → 없으면 조용히 무시)
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (storageUI == null)
            {
                // 탭 순간에 한 번 더 적극 탐색
                var go = GameObject.Find("StorageUI");
                if (go != null) storageUI = go.GetComponent<StorageUI>();
                if (storageUI == null) storageUI = FindObjectOfType<StorageUI>(includeInactive: true);
            }

            if (storageUI != null)
                storageUI.Toggle();
        }
    }

    // 씬 전환 후 레퍼런스가 끊길 때를 대비해서, 외부에서 호출 가능한 리셋 훅
    public void InvalidateCache()
    {
        mainCam = null;
        // player / ws / storageUI 중 파괴가 잦은 것만 비우고 재바인딩 유도
        // 여기서는 전부 비워도 안전
        player = null;
        ws = null;
        storageUI = null;
    }
}

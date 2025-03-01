using UnityEngine;

public class EnemyPathfinding : MonoBehaviour {
    public float speed;
    public bool isLive;
    Animator anim;
    Rigidbody2D rigid;
    public LayerMask blockLayerMask; // Block을 탐지할 레이어 마스크
    Rigidbody2D target;
    Enemy enemy;
    public bool isMoving;

    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        enemy = GetComponent<Enemy>();
        isMoving = true;
        Init();
    }

    void OnEnable() {
        Init();
    }

    public void Init() {
        isLive = enemy.isLive;
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        speed = enemy.speed;
    }

    void FixedUpdate() {
        isLive = enemy.isLive;
        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit")) return;
        if (!isMoving) return;

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;

        // 장애물 탐지
        bool isObstacle = IsObstacleInPath(dirVec);
        if (!isObstacle) {
            // 장애물이 없으면 플레이어 방향으로 이동
            rigid.MovePosition(rigid.position + nextVec);
        } else {
            // 장애물이 있으면 회피 경로를 계산하여 이동
            Vector2 avoidanceVec = GetAvoidanceDirection(dirVec);
            rigid.MovePosition(rigid.position + avoidanceVec * speed * Time.fixedDeltaTime);
        }

        rigid.velocity = Vector2.zero; // 플레이어 충돌 시 밀려남 무시

        // 디버그용 Ray를 그려서 장애물 탐지 확인
        Debug.DrawRay(transform.position, dirVec.normalized, Color.red);
    }

    // Raycast 기반 길찾기
    bool IsObstacleInPath(Vector2 direction) {
        float rayLength = 2f; // Ray 길이 조정
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, rayLength, blockLayerMask);

        // Raycast 시각화
        Debug.DrawRay(transform.position, direction.normalized * rayLength, Color.green);

        return hit.collider != null;
    }
    // 장애물 우회 방향 계산
    Vector2 GetAvoidanceDirection(Vector2 originalDir) {
        Vector2 avoidanceDir = originalDir;

        for (int i = 0; i < 8; i++) {
            float angle = i * 45f; // 8방향 탐색 (45도 간격)
            Vector2 tempDir = Quaternion.Euler(0, 0, angle + 90f) * originalDir;
            
            if (!IsObstacleInPath(tempDir)) {
                avoidanceDir = tempDir;
                break;
            }
        }

        return avoidanceDir.normalized;
    }
}
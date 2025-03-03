using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reposition : MonoBehaviour // 뱀서라이크의 무한맵 구현을 위한 재배치를 담당하는 함수
{
    Collider2D collider;
    Vector3 startPos;
    public bool[,] grid = new bool[32, 20];
    public List<GameObject> blocks = new List<GameObject>();
    const int maxLootCount = 5;

    void Awake() {
        collider = GetComponent<Collider2D>();
        startPos = transform.position;
    }
    void Start() {
        if (transform.CompareTag("Ground")) {
            StartCoroutine(DelayedSetLandScape());
        }
    }

    void OnTriggerExit2D(Collider2D collision) {
        if (!collision.CompareTag("Area")) {
            return;
        }
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 myPos = transform.position;
        if (transform.CompareTag("Bullet") || transform.CompareTag("Loot") || transform.CompareTag("EnemyBullet")) {
            HandleEntityReposition();
        } else if (transform.CompareTag("Ground")) {
            HandleGroundReposition(playerPos, myPos);
        } else if (transform.CompareTag("Enemy") && collider.enabled) {
            HandleEnemyReposition(playerPos, myPos);
        }
    }
    void HandleEntityReposition() {
        gameObject.SetActive(false);
    }
    void HandleGroundReposition(Vector3 playerPos, Vector3 myPos) {
        float diffX = playerPos.x - myPos.x;
        float diffY = playerPos.y - myPos.y;

        float dirX = diffX < 0 ? -1 : 1;
        float dirY = diffY < 0 ? -1 : 1;
        diffX = Mathf.Abs(diffX) / 32;
        diffY = Mathf.Abs(diffY) / 20;

        if (diffX > diffY) {
            transform.Translate(Vector3.right * dirX * 64);
        } else {
            transform.Translate(Vector3.up * dirY * 40);
        }

        SetLandScape();
    }

    void HandleEnemyReposition(Vector3 playerPos, Vector3 myPos) {
        Vector3 dist = playerPos - myPos;
        Vector3 ran = new Vector3(UnityEngine.Random.Range(-3, 3), UnityEngine.Random.Range(-3, 3));
        transform.Translate(ran + dist * 2);
    }

    IEnumerator DelayedSetLandScape() {
        yield return new WaitForSeconds(0.1f);
        SetLandScape();
    }
    
    public void SetLandScape() { // 타일맵 재배치 시 블록 및 자원 배치
        ResetGrid();
        SetBlock();
        SetLoot();
    }
    void SetBlock() {
        Vector3[] offsets = {
            transform.position + new Vector3(-16, -10, 0) + new Vector3(0.5f, 0.5f, 0),
            transform.position + new Vector3(-16, -10, 0) + new Vector3(0.5f, 1f, 0),
            transform.position + new Vector3(-16, -10, 0) + new Vector3(1f, 0.5f, 0),
            transform.position + new Vector3(-16, -10, 0) + new Vector3(1f, 1f, 0)
        };

        for (int i = 0; i < 5; i++) {
            Vector2Int centerPos = new Vector2Int(UnityEngine.Random.Range(0, 32), UnityEngine.Random.Range(0, 20));
            Vector2Int[] pattern = Utils.GetRandomBlockPattern();
            List<GameObject> newBlocks = new List<GameObject>();
            foreach (Vector2Int offsetPos in pattern) {
                Vector2Int pos = centerPos + offsetPos;
                if (IsOutOfBounds(pos.x, pos.y)) continue; // 먼저 범위 바깥을 확인
                if (grid[pos.x, pos.y]) continue; // 그리드에 이미 값이 있는지 확인

                // 그리드 업데이트 및 블록 배치
                grid[pos.x, pos.y] = true;

                foreach (Vector3 offset in offsets) {
                    GameObject block = PoolManager.instance.Get("Block");
                    block.name = gameObject.name + " Block";
                    block.transform.localPosition = new Vector3(pos.x, pos.y, 0) + offset;
                    newBlocks.Add(block);
                }
            }
            blocks.AddRange(newBlocks);
        }
    }
    void SetLoot() { // 랜덤하게 자원 배치
        Vector3 offset = transform.position + new Vector3(-16, -10, 0) + new Vector3(0.5f, 0.5f, 0);
        for (int lootCount = 0; lootCount < maxLootCount; lootCount++) {
            Vector2Int centerPos = new Vector2Int(UnityEngine.Random.Range(0, 32), UnityEngine.Random.Range(0, 20));
            if (grid[centerPos.x, centerPos.y]) continue;
            grid[centerPos.x, centerPos.y] = true;
            GameObject loot = PoolManager.instance.Get("Loot");
            loot.transform.localPosition = new Vector3(centerPos.x, centerPos.y, 0) + offset;
            int rand = UnityEngine.Random.Range(0, DataManager.instance.lootDataList.Length);
            loot.GetComponent<Loot>().Init(DataManager.instance.lootDataList[rand]);
            blocks.Add(loot);
        }
    }

    void ResetGrid() { // Reposition 시 타일맵 초기화
        for (int tileWidth = 0; tileWidth < 32; tileWidth++) {
            for (int tileHeight = 0; tileHeight < 20; tileHeight++) {
                grid[tileWidth, tileHeight] = false;
            }
        }
        foreach (GameObject block in blocks) {
            if (block != null) block.SetActive(false);

        }
    }
    // 타일맵 바깥 조건
    bool IsOutOfBounds(int tileWidth, int tileHeight) {
        return tileWidth < 0 || tileWidth >= 32 || tileHeight < 0 || tileHeight >= 20;
    }
    // 탈출 페이즈로 전환 시 타일맵 초기화
    public void ResetGround() {
        HandleGroundReposition(startPos, transform.position);
    }
}
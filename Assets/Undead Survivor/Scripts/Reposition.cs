using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Reposition : MonoBehaviour
{
    Collider2D collider;
    Vector3 startPos;
    void Awake() {
        collider = GetComponent<Collider2D>();
        startPos = transform.position;
    }
    void OnTriggerExit2D(Collider2D collision) {
        if (!collision.CompareTag("Area")) {
            return;
        }
        // 플레이어 위치랑 그라운드의 위치(myPos)의 차를 구함
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 myPos = transform.position;


        switch (transform.tag) {
            case "Ground" :
                float diffX = playerPos.x - myPos.x;
                float diffY = playerPos.y - myPos.y;

                // 대각선일 시 normalize되어서 벡터 크기가 1보다 작으니까 -1 또는 1로 크기 변경
                float dirX = diffX < 0 ? -1 : 1;
                float dirY = diffY < 0 ? -1 : 1;
                diffX = Mathf.Abs(diffX) / 32;
                diffY = Mathf.Abs(diffY) / 20;
                //플레이어가 X축으로 더 많이 이동한 경우 X축방향으로 재배치
                if (diffX > diffY) {
                    transform.Translate(Vector3.right * dirX * 64);
                }
                //플레이어가 Y축으로 더 많이 이동한 경우 Y축방향으로 재배치
                else {
                    transform.Translate(Vector3.up * dirY * 40);
                }
                break;
            case "Enemy" :
                if (collider.enabled) {
                    Vector3 dist = playerPos - myPos;
                    Vector3 ran = new Vector3(UnityEngine.Random.Range(-3, 3), UnityEngine.Random.Range(-3, 3));
                    transform.Translate(ran + dist * 2);
                }
                break;
        }
    }
    public void Reset(){
        transform.position = startPos;
    }
}

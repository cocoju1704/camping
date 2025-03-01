using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{ // 플레이어 손 스프라이트. 무기 변경할때 사용. 로직은 X
    public SpriteRenderer spriteRenderer;

    SpriteRenderer player;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GetComponentsInParent<SpriteRenderer>()[1];
    }
    public void UpdateVisual(Sprite sprite) {
        spriteRenderer.sprite = sprite;
    }
}

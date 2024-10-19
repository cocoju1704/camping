using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    SpriteRenderer player;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GetComponentsInParent<SpriteRenderer>()[1];
    }
    public void UpdateVisual(Sprite sprite) {
        spriteRenderer.sprite = sprite;
    }
    void LateUpdate() {
        // bool isReverse = player.flipX;
        // spriteRenderer.flipY = isReverse;
        // spriteRenderer.sortingOrder = isReverse ? 4 : 6;
    }

}

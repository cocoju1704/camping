using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;  // DOTween 사용

public class TempText : MonoBehaviour
{ // 대미지 표기 등 UI에서 사용하는 일시적인 텍스트
    TMP_Text text;
    public float fadeDuration = 2f; // 페이드 아웃 지속 시간
    public Vector3 moveOffset = new Vector3(0, 1, 0); // 이동할 거리

    public void Awake() {
        text = GetComponent<TMP_Text>();
    }

    public void OnEnable() {
        text.color = new Color(1, 1, 1, 1);
    }

    public void SetTempText(string text, Color color, float size) {
        this.text.text = text;
        this.text.color = color;
        this.text.fontSize = size;
        StartFadeOut();
    }

    public void SetTempText(float damage) {
        text.text = damage.ToString();
        text.color = new Color(1, 1, 1, 1);
        text.fontSize = 6;
        StartFadeOut();
    }

    void StartFadeOut() {
        // 텍스트를 위로 이동시키면서 서서히 사라지게 만듦
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + moveOffset;

        // 이동 + 페이드 아웃
        transform.DOMove(endPos, fadeDuration).SetEase(Ease.OutQuad); // 위로 이동하는 애니메이션
        text.DOFade(0, fadeDuration); // 투명하게 되는 애니메이션
    }
}
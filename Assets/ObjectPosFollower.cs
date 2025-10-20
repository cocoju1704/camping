using UnityEngine;

[ExecuteAlways]
public class ObjectPosFollowAndScaler : MonoBehaviour
{
    [Header("Reference")]
    public SpriteRenderer spriteRendererToFollow; // 따라갈 스프라이트
    public Transform thisObject;                  // 따라가는 대상 (UI or Sprite)
    public bool isUI = true;                      // true면 RectTransform(UI), false면 SpriteRenderer(월드 오브젝트)

    [Header("Offset Settings")]
    public float yOffset = 0.2f;                  // 위/아래 위치 오프셋
    public float xScaleFactor = 1.0f;             // 추가 배율 인자

    private RectTransform thisRect;
    private SpriteRenderer thisSprite;

    void OnEnable()
    {
        if (!thisObject) thisObject = transform;
        if (isUI)
        {
            thisRect = thisObject.GetComponent<RectTransform>();
            thisSprite = null;
        }
        else
        {
            thisSprite = thisObject.GetComponent<SpriteRenderer>();
            thisRect = null;
        }
    }

    void LateUpdate()
    {
        if (!spriteRendererToFollow || !thisObject) return;

        var b = spriteRendererToFollow.bounds;

        // -------- 위치 계산 --------
        Vector3 pos = thisObject.position;
        pos.x = b.center.x;
        pos.y = b.max.y + yOffset;
        thisObject.position = pos;

        // -------- 스케일/사이즈 맞추기 --------
        float spriteWidthWorld = b.size.x;
        if (isUI && thisRect != null)
        {
            // ✅ World Space Canvas 기반 RectTransform
            float parentScaleX = thisRect.lossyScale.x;
            if (parentScaleX < 1e-6f) parentScaleX = 1e-6f;

            float desiredSizeDeltaX = (spriteWidthWorld / parentScaleX) * xScaleFactor;
            thisRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, desiredSizeDeltaX);
        }
        else if (thisSprite != null)
        {
            // ✅ SpriteRenderer 기반 오브젝트
            float thisSpriteWidth = thisSprite.bounds.size.x;
            if (thisSpriteWidth > 0)
            {
                float scaleFactor = (spriteWidthWorld / thisSpriteWidth) * xScaleFactor;
                thisObject.localScale = new Vector3(
                    thisObject.localScale.x * scaleFactor,
                    thisObject.localScale.y * scaleFactor,
                    thisObject.localScale.z
                );
            }
        }
    }
}

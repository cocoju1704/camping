using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandController : MonoBehaviour
{
    public Hand[] hands;
    
    void Awake() {
        hands = GetComponentsInChildren<Hand>(true);
    }
    void Update()
    {
        RotateHand();
    }
    void RotateHand()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0;

        Vector3 axisPos = transform.position + new Vector3(0, -0.1f, 0);
        Hand hand = hands[0];
        Vector3 offset = hand.transform.position - axisPos;
        float distance = offset.magnitude;

        Vector2 direction = mousePos - axisPos;  
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;  // 벡터를 각도로 변환

        Vector3 newHandPosition = axisPos + (Quaternion.Euler(0, 0, angle) * Vector3.right * distance);
        hand.transform.position = newHandPosition;
        hand.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}

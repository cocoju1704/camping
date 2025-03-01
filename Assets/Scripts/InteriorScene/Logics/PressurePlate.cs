using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// PressurePlate 클래스
public class PressurePlate : MonoBehaviour // 플레이어 접촉 시 트리거 발동 스크립트
{
    private List<ITriggerObserver> observers = new List<ITriggerObserver>();

    public void Subscribe(ITriggerObserver observer) {
        if (!observers.Contains(observer)) {
            observers.Add(observer);
        }
    }

    public void Unsubscribe(ITriggerObserver observer) {
        if (observers.Contains(observer)) {
            observers.Remove(observer);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || !other.isTrigger)
        {
            foreach (var observer in observers) {
                observer.OnTriggerEntered(gameObject.name);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || !other.isTrigger)
        {
            foreach (var observer in observers) {
                observer.OnTriggerExited(gameObject.name);
            }
        }
    }
}

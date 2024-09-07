using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExitVehicle : MonoBehaviour
{
    public UnityEvent OnPlayerExitVehicle;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerExitVehicle.Invoke();
        }
    }
}

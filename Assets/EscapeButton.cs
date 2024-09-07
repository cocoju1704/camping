using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapeButton : MonoBehaviour
{
    [HideInInspector] public Button button;
    void Awake()
    {
        button = GetComponent<Button>();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour
{
    Button button;
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => ReturnToIndoor());
    }
    void ReturnToIndoor() {
        SceneLoadSystem.instance.LoadScene("LobbyScene");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;

public class TitleMenuUI : MonoBehaviour // 시작화면 UI 
{
    public Button[] buttons;
    void Awake()
    {

        buttons = GetComponentsInChildren<Button>(); 
        buttons[0].onClick.AddListener(() => StartNewGame());
        buttons[1].onClick.AddListener(() => ContinueGame());
        buttons[2].onClick.AddListener(() => ExitGame());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void StartNewGame() {
        SceneFlowManager.instance.EnterLobby();

    }
    void ContinueGame() {
        // 로딩
        SceneFlowManager.instance.EnterLobby();
    }
    
    void ExitGame() {
        Application.Quit();
    }
}

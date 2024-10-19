using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleMenu : MonoBehaviour
{
    public Button[] buttons;
    // Start is called before the first frame update
    void Awake()
    {

        buttons = GetComponentsInChildren<Button>(); //0: Start, 1: Continue, 2: Exit
        buttons[0].onClick.AddListener(() => StartNewGame());
        buttons[1].onClick.AddListener(() => ContinueGame());
        buttons[2].onClick.AddListener(() => ExitGame());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void StartNewGame() {
        SceneLoadSystem.instance.LoadScene("LobbyScene");

    }
    void ContinueGame() {
        // 로딩
        SceneLoadSystem.instance.LoadScene("LobbyScene");
    }
    
    void ExitGame() {
        Application.Quit();
    }
}

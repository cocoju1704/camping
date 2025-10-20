using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
public class SceneFlowManager : Singleton<SceneFlowManager>
{
    public event Action BattleEntered;
    public event Action<bool> BattleExited; // cleared
    public event Action LobbyEntered;
    public event Action LobbyExited;
    public async void EnterBattle(StageData stageData)
    {
        LobbyExited?.Invoke();
        SaveSystem.instance.SaveAll();
        GameManager.instance.ToNextStageIdx();

        await SceneLoadSystem.instance.LoadSceneAsync(SceneLoadSystem.Scene.BattleScene);

        // 씬 준비 완료 시점
        BattleEntered?.Invoke();
        LinkAllILink();

        // ex) FindObjectOfType<StageManager>()?.StartStage(stageData);
    }

    public async void ExitBattle(bool cleared)
    {
        if (cleared)
        {
            Debug.Log("Stage Cleared!");
            StageManager.instance.inventory.AddToStorage();
            SaveSystem.instance.SaveAll();

            await SceneLoadSystem.instance.LoadSceneAsync(SceneLoadSystem.Scene.LobbyScene);

            BattleExited?.Invoke(true);
            // 로비 UI 열기 등
        }
        else
        {
            Debug.Log("Stage Failed.");
            // 실패 처리/패널티 후 로비 복귀 여부 결정
            await SceneLoadSystem.instance.LoadSceneAsync(SceneLoadSystem.Scene.LobbyScene);
            BattleExited?.Invoke(false);
        }
        LinkAllILink();

    }

    public async void EnterLobby(bool isNewGame = false)
    {
        if (isNewGame) SaveSystem.instance.SaveAll();
        else SaveSystem.instance.LoadAll();

        await SceneLoadSystem.instance.LoadSceneAsync(SceneLoadSystem.Scene.LobbyScene);
        LobbyEntered?.Invoke();
        LinkAllILink();
    }
    public void LinkAllILink()
    {
        List<ILinkOnSceneLoad> linkers = FindObjectsOfType<MonoBehaviour>().OfType<ILinkOnSceneLoad>().ToList();
        foreach (ILinkOnSceneLoad linker in linkers)
        {
            linker.LinkObjectsOnSceneLoad();
        }
    }
}

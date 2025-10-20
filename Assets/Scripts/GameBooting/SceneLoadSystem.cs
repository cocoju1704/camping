using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadSystem : Singleton<SceneLoadSystem>
{
    public enum Scene { TitleScene, BattleScene, LobbyScene }
    [SerializeField] GameObject fadingBlackPrefab;
    [SerializeField, Range(0f, 1f)] float fadeOutTime = 0.5f;
    [SerializeField, Range(0f, 1f)] float fadeInTime = 0.5f;

    CanvasGroup cgFadeBlack;
    Image imageFadeBlack;

    public Scene NowScene { get; private set; } = Scene.TitleScene;

    protected override void Awake()
    {
        base.Awake();
        var fadingBlack = Instantiate(fadingBlackPrefab, transform);
        cgFadeBlack = fadingBlack.GetComponent<CanvasGroup>();
        imageFadeBlack = fadingBlack.GetComponent<Image>();
        cgFadeBlack.blocksRaycasts = false;
        imageFadeBlack.color = new Color(0,0,0,0);
        DontDestroyOnLoad(gameObject);
    }

    public Task LoadSceneAsync(Scene scene) => LoadSceneAsync(scene.ToString());

    public async Task LoadSceneAsync(string sceneName)
    {
        // 필요 시 풀 비우기 (도메인 의존이 크면 밖으로 이동도 고려)
        PoolManager.instance.DestroyAll();

        cgFadeBlack.blocksRaycasts = true;
        await imageFadeBlack.DOFade(1f, fadeOutTime).AsyncWaitForCompletion();

        var op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
            await Task.Yield();

        op.allowSceneActivation = true;

        await imageFadeBlack.DOFade(0f, fadeInTime).AsyncWaitForCompletion();
        cgFadeBlack.blocksRaycasts = false;

        NowScene = StringToScene(sceneName);
    }

    Scene StringToScene(string sceneName)
    {
        if (sceneName == Scene.TitleScene.ToString()) return Scene.TitleScene;
        if (sceneName == Scene.BattleScene.ToString()) return Scene.BattleScene;
        if (sceneName == Scene.LobbyScene.ToString()) return Scene.LobbyScene;
        Debug.LogError("Unknown scene: " + sceneName);
        return Scene.TitleScene;
    }
}

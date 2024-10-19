using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadSystem : Singleton<SceneLoadSystem> {
    public enum Scene {
        TitleScene,
        BattleScene,
        LobbyScene,
    }
    public UnityEvent OnFinishTransition;
    
    [SerializeField]GameObject fadingBlackPrefab;
    [SerializeField, Range(0, 1)] float fadeOutTime = 0.5f;
    [SerializeField, Range(0, 0.5f)] float fadeInTime = 0.5f;

    CanvasGroup cgFadeBlack;
    Image imageFadeBlack;

    protected override void Awake() {
        base.Awake();
        GameObject fadingBlack = Instantiate(fadingBlackPrefab, transform);
        OnFinishTransition = new UnityEvent();
        cgFadeBlack = fadingBlack.GetComponent<CanvasGroup>();
        imageFadeBlack = fadingBlack.GetComponent<Image>();
    }

    public void LoadScene(Scene scene) {
        LoadScene(scene.ToString());
    }

    public void LoadScene(string sceneName) {
        PoolManager.instance.DestroyAll();
        StartCoroutine(LoadSceneCo(sceneName));
    }

    IEnumerator LoadSceneCo(string sceneName) {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;
        cgFadeBlack.blocksRaycasts = true;

        yield return PlayFadeOutAnim().WaitForCompletion();
        
        while(!asyncOperation.isDone) {
            if(asyncOperation.progress >= 0.9f) {
                break;
            }
            yield return null;
        }

        asyncOperation.allowSceneActivation = true;
        yield return PlayFadeInAnim().WaitForCompletion();
        cgFadeBlack.blocksRaycasts = false;

        OnFinishTransition.Invoke();
        OnFinishTransition.RemoveAllListeners();
    }
    public void LoadStageScene() {
        LoadScene("StageScene");
    }
    Tweener PlayFadeOutAnim() {
        return imageFadeBlack.DOFade(1, fadeOutTime);
    }
    Tweener PlayFadeInAnim() {
        Tweener tweener = imageFadeBlack.DOFade(0, fadeInTime);
        return tweener;
    }
}

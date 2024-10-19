using System.Collections.Generic;
using UnityEngine;

public class GameBooting {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void InstantiateCoreObjects() {
        List<GameObject> objs = new List<GameObject>() {
            //GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/GameBooting/ResourceSystem")),
            GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/GameBooting/SaveSystem")),
            GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/GameBooting/SceneLoadSystem")),
            //GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/GameBooting/AudioSystem")),
            GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/GameBooting/DataManager")),
            GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/GameBooting/GameManager")),
            GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/GameBooting/PoolManager")),
            //GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/GameBooting/PoolManager")),
        };

        foreach(GameObject obj in objs) {
            int idx = obj.name.IndexOf("(Clone)");
            obj.name = idx > 0 ? obj.name = obj.name.Substring(0, idx) : obj.name;
            GameObject.DontDestroyOnLoad(obj);
        }
    }
}
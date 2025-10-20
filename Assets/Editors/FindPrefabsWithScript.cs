using UnityEditor;
using UnityEngine;

public class ScriptFinderWindow : EditorWindow
{
    string scriptName = "";

    [MenuItem("Tools/Script Finder")]
    public static void OpenWindow()
    {
        GetWindow<ScriptFinderWindow>("Script Finder");
    }

    void OnGUI()
    {
        GUILayout.Label("🔍 프리팹에서 스크립트 찾기", EditorStyles.boldLabel);
        scriptName = EditorGUILayout.TextField("스크립트 이름:", scriptName);

        if (GUILayout.Button("검색"))
        {
            FindPrefabs(scriptName);
        }
    }

    void FindPrefabs(string scriptName)
    {
        if (string.IsNullOrEmpty(scriptName))
        {
            Debug.LogWarning("⚠️ 스크립트 이름을 입력하세요.");
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:Prefab");
        int count = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (!prefab) continue;

            foreach (var comp in prefab.GetComponentsInChildren<MonoBehaviour>(true))
            {
                if (comp == null) continue;
                if (comp.GetType().Name == scriptName)
                {
                    Debug.Log($"✅ {scriptName} found in {path}", prefab);
                    count++;
                    break;
                }
            }
        }

        Debug.Log($"총 {count}개의 프리팹에서 {scriptName}을(를) 찾았습니다.");
    }
}
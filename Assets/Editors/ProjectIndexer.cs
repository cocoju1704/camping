// Assets/Editor/ProjectIndexer.cs
// Unity Editor에서 실행되는 사전 인덱서.
// - NDJSON(한 줄 = 하나의 JSON 오브젝트)로 결과 출력
// - 의존성/프리팹 계보/오버라이드/빌드 씬/Animator 얇은 메타/Animation Event/Shader Property/Addressables(옵션)
//
// 실행 방법:
//   - 배치모드: -executeMethod ProjectIndexer.Run indexOut=./Library/index-out.jsonl includeAnim=true includeShader=true includeSceneOverrides=false
//   - 에디터 메뉴: Tools/Project Indexer/Run
//
// 요구 폴더: Assets/, ProjectSettings/, Packages/ (추천)
// Addressables 사용 시: Scripting Define Symbols에 ADDRESSABLES 추가(또는 using 구문 조절)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditor.Animations;

#if ADDRESSABLES
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
#endif

using UMat = UnityEngine.Material;

public static class ProjectIndexer
{
    // ===== 옵션 파싱 =====
    class Options
    {
        public string indexOut = "Library/index-out.jsonl";
        public bool includeAnim = true;            // Animator/AnimationEvent 메타
        public bool includeShader = true;          // Shader/Material 프로퍼티 시그니처
        public bool includeSceneOverrides = false; // 씬 열어서 프리팹 인스턴스 오버라이드 수집
        public bool includeDependencies = true;    // AssetDatabase 의존성 그래프
        public bool includeBuildScenes = true;     // ProjectSettings: EditorBuildSettings.scenes
        public bool includeAddressables = true;    // ADDRESSABLES 정의 시만 유효

        public static Options FromArgs(string[] args)
        {
            var o = new Options();
            foreach (var a in args)
            {
                var kv = a.Split('=');
                if (kv.Length != 2) continue;
                var k = kv[0]; var v = kv[1];
                switch (k)
                {
                    case "indexOut": o.indexOut = v; break;
                    case "includeAnim": o.includeAnim = ParseBool(v, o.includeAnim); break;
                    case "includeShader": o.includeShader = ParseBool(v, o.includeShader); break;
                    case "includeSceneOverrides": o.includeSceneOverrides = ParseBool(v, o.includeSceneOverrides); break;
                    case "includeDependencies": o.includeDependencies = ParseBool(v, o.includeDependencies); break;
                    case "includeBuildScenes": o.includeBuildScenes = ParseBool(v, o.includeBuildScenes); break;
                    case "includeAddressables": o.includeAddressables = ParseBool(v, o.includeAddressables); break;
                }
            }
            return o;
        }
        static bool ParseBool(string s, bool defVal)
        {
            if (bool.TryParse(s, out var b)) return b;
            if (s == "1") return true;
            if (s == "0") return false;
            return defVal;
        }
    }

    // ===== NDJSON 라이터 =====
    class NdjsonWriter : IDisposable
    {
        StreamWriter sw;
        public NdjsonWriter(string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
            sw = new StreamWriter(path, false, new UTF8Encoding(false));
        }
        public void Write(object record)
        {
            // Unity JsonUtility는 익명/사전 직렬화가 약함 → 간단한 미니 직렬화기 사용
            sw.WriteLine(MiniJson.Serialize(record));
        }
        public void Dispose() { sw?.Dispose(); }
    }

    // ===== 최소 JSON 직렬화기(사전/배열/기본형/POCO 일부 지원) =====
    static class MiniJson
    {
        public static string Serialize(object obj)
        {
            var sb = new StringBuilder(256);
            SerializeValue(obj, sb);
            return sb.ToString();
        }
        static void SerializeValue(object v, StringBuilder sb)
        {
            if (v == null) { sb.Append("null"); return; }
            switch (v)
            {
                case string s: sb.Append('\"').Append(Escape(s)).Append('\"'); return;
                case bool b: sb.Append(b ? "true" : "false"); return;
                case Enum e: sb.Append('\"').Append(e.ToString()).Append('\"'); return;
                case int or long or float or double or decimal:
                    sb.Append(Convert.ToString(v, System.Globalization.CultureInfo.InvariantCulture)); return;
                case IDictionary<string, object> dict:
                    SerializeDict(dict, sb); return;
                case System.Collections.IEnumerable arr:
                    SerializeArray(arr, sb); return;
                default:
                    SerializeObject(v, sb); return;
            }
        }
        static void SerializeArray(System.Collections.IEnumerable arr, StringBuilder sb)
        {
            sb.Append('[');
            bool first = true;
            foreach (var it in arr)
            {
                if (!first) sb.Append(',');
                SerializeValue(it, sb);
                first = false;
            }
            sb.Append(']');
        }
        static void SerializeDict(IDictionary<string, object> dict, StringBuilder sb)
        {
            sb.Append('{');
            bool first = true;
            foreach (var kv in dict)
            {
                if (kv.Value == null) continue;
                if (!first) sb.Append(',');
                sb.Append('\"').Append(Escape(kv.Key)).Append("\":");
                SerializeValue(kv.Value, sb);
                first = false;
            }
            sb.Append('}');
        }
        static void SerializeObject(object o, StringBuilder sb)
        {
            var t = o.GetType();
            var props = t.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            sb.Append('{');
            bool first = true;
            foreach (var p in props)
            {
                if (!p.CanRead) continue;
                var val = p.GetValue(o, null);
                if (val == null) continue;
                if (!first) sb.Append(',');
                sb.Append('\"').Append(Escape(p.Name)).Append("\":");
                SerializeValue(val, sb);
                first = false;
            }
            sb.Append('}');
        }
        static string Escape(string s) =>
            s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
    }

    // ===== 공개 엔트리포인트 =====
    [MenuItem("Tools/Project Indexer/Run")]
    public static void RunFromMenu()
    {
        Run(); // 기본 옵션으로 실행
        EditorUtility.RevealInFinder("Library");
    }

    public static void Run()
    {
        var args = Environment.GetCommandLineArgs();
        var opt = Options.FromArgs(args);
        using var w = new NdjsonWriter(opt.indexOut);

        LogInfo($"[Indexer] start → {opt.indexOut}");
        var allGuids = AssetDatabase.FindAssets("").Distinct().ToArray();
        w.Write(new { type = "asset_list", count = allGuids.Length });

        // 1) 노드 메타(기본)
        foreach (var guid in allGuids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var mt = AssetDatabase.GetMainAssetTypeAtPath(path);
            w.Write(new
            {
                type = "node",
                kind = KindFromPathAndType(path, mt),
                guid,
                path,
                mainType = mt?.FullName ?? "Unknown"
            });
        }

        // 2) 의존성 그래프(AssetDatabase)
        if (opt.includeDependencies)
        {
            for (int i = 0; i < allGuids.Length; i++)
            {
                var guid = allGuids[i];
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var deps = AssetDatabase.GetDependencies(path, true);
                foreach (var depPath in deps)
                {
                    if (depPath == path) continue;
                    var depGuid = AssetDatabase.AssetPathToGUID(depPath);
                    if (string.IsNullOrEmpty(depGuid)) continue;
                    w.Write(new { type = "edge", rel = "uses", src_guid = guid, dst_guid = depGuid, provenance = "assetdb" });
                }
                if (i % 500 == 0) LogInfo($"[Indexer] deps {i}/{allGuids.Length}");
            }
        }

        // 3) 프리팹 계보/오버라이드(에셋 레벨)
        var prefabGuids = allGuids.Where(g => IsPrefab(AssetDatabase.GUIDToAssetPath(g))).ToArray();
        foreach (var guid in prefabGuids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (go == null) continue;

            var src = PrefabUtility.GetCorrespondingObjectFromOriginalSource(go);
            if (src != null)
            {
                var srcPath = AssetDatabase.GetAssetPath(src);
                var srcGuid = AssetDatabase.AssetPathToGUID(srcPath);
                if (!string.IsNullOrEmpty(srcGuid) && srcGuid != guid)
                {
                    w.Write(new { type = "edge", rel = "variant-of", src_guid = guid, dst_guid = srcGuid, provenance = "prefab" });
                }
            }
            var mods = PrefabUtility.GetPropertyModifications(go);
            if (mods != null && mods.Length > 0)
            {
                foreach (var m in mods)
                {
                    w.Write(new
                    {
                        type = "edge",
                        rel = "override-of",
                        src_guid = guid,
                        dst_guid = src != null ? AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(src)) : null,
                        provenance = "prefab",
                        detail = new
                        {
                            propertyPath = m.propertyPath,
                            value = m.value,
                            objectRef = m.objectReference ? AssetDatabase.GetAssetPath(m.objectReference) : null
                        }
                    });
                }
            }
        }

        // 4) 빌드 씬
        if (opt.includeBuildScenes)
        {
            var scenePaths = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToList();
            foreach (var sPath in scenePaths)
            {
                var sGuid = AssetDatabase.AssetPathToGUID(sPath);
                w.Write(new { type = "scene_entry", guid = sGuid, path = sPath });
                var deps = AssetDatabase.GetDependencies(sPath, true);
                foreach (var d in deps)
                {
                    var dGuid = AssetDatabase.AssetPathToGUID(d);
                    if (string.IsNullOrEmpty(dGuid) || dGuid == sGuid) continue;
                    w.Write(new { type = "edge", rel = "uses", src_guid = sGuid, dst_guid = dGuid, provenance = "build" });
                }
            }
        }

        // 5) 씬 인스턴스 오버라이드(옵션)
        if (opt.includeSceneOverrides)
        {
            CollectSceneOverridesForAllBuildScenes(w);
        }

        // 6) Animator 얇은 메타 / Animation Events
        if (opt.includeAnim)
        {
            var controllerGuids = allGuids.Where(g => AssetDatabase.GUIDToAssetPath(g).EndsWith(".controller", StringComparison.OrdinalIgnoreCase)).ToArray();
            foreach (var cg in controllerGuids)
            {
                var cPath = AssetDatabase.GUIDToAssetPath(cg);
                var ctrl = AssetDatabase.LoadAssetAtPath<AnimatorController>(cPath);
                if (ctrl == null) continue;

                foreach (var p in ctrl.parameters)
                {
                    w.Write(new { type = "anim_param", controller_guid = cg, name = p.name, param_type = p.type.ToString() });
                }

                foreach (var layer in ctrl.layers)
                {
                    var sm = layer.stateMachine;
                    foreach (var st in sm.states)
                    {
                        var state = st.state;
                        var from = state.name;
                        foreach (var t in state.transitions)
                        {
                            var to = t.destinationState ? t.destinationState.name : "Exit";
                            var conds = t.conditions.Select(c => new { param = c.parameter, op = c.mode.ToString(), value = c.threshold });
                            w.Write(new
                            {
                                type = "anim_transition",
                                controller_guid = cg,
                                from,
                                to,
                                conditions = conds
                            });
                        }
                        // Clip 연결
                        var motion = state.motion;
                        if (motion is AnimationClip clip)
                        {
                            var clipPath = AssetDatabase.GetAssetPath(clip);
                            var clipGuid = AssetDatabase.AssetPathToGUID(clipPath);
                            if (!string.IsNullOrEmpty(clipGuid))
                            {
                                w.Write(new { type = "edge", rel = "state-uses-clip", src_guid = cg, dst_guid = clipGuid, provenance = "animator" });
                            }
                        }
                    }
                }
            }

            // Animation Events
            var clipGuids = allGuids.Where(g => AssetDatabase.GUIDToAssetPath(g).EndsWith(".anim", StringComparison.OrdinalIgnoreCase)).ToArray();
            foreach (var ag in clipGuids)
            {
                var aPath = AssetDatabase.GUIDToAssetPath(ag);
                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(aPath);
                if (clip == null) continue;
                var events = AnimationUtility.GetAnimationEvents(clip);
                foreach (var ev in events)
                {
                    w.Write(new { type = "anim_event", clip_guid = ag, name = ev.functionName, time = ev.time });
                }
            }
        }

        // 7) Shader/Material 프로퍼티 시그니처
        if (opt.includeShader)
        {
            var matGuids = allGuids.Where(g => AssetDatabase.GUIDToAssetPath(g).EndsWith(".mat", StringComparison.OrdinalIgnoreCase)).ToArray();
            foreach (var mg in matGuids)
            {
                var mPath = AssetDatabase.GUIDToAssetPath(mg);
                var mat = AssetDatabase.LoadAssetAtPath<UMat>(mPath);
                if (mat == null || mat.shader == null) continue;

                var shader = mat.shader;
                int count = ShaderUtil.GetPropertyCount(shader);
                for (int i = 0; i < count; i++)
                {
                    var pname = ShaderUtil.GetPropertyName(shader, i);
                    var ptype = ShaderUtil.GetPropertyType(shader, i).ToString();
                    w.Write(new
                    {
                        type = "shader_prop",
                        material_guid = mg,
                        shader = shader.name,
                        prop = new { name = pname, type = ptype }
                    });
                }
            }
        }

        // 8) Addressables 매핑(옵션, 심볼 필요)
#if ADDRESSABLES
        if (opt.includeAddressables)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings != null)
            {
                foreach (var group in settings.groups.Where(g => g != null))
                {
                    foreach (var entry in group.entries)
                    {
                        var eGuid = entry.guid;
                        var ePath = AssetDatabase.GUIDToAssetPath(eGuid);
                        // 키→GUID
                        w.Write(new { type = "addr_resolve", key = entry.address, dst_guid = eGuid, provenance = "addressables", detail = new { group = group.Name, path = ePath } });
                        // 라벨→GUID
                        foreach (var label in entry.labels)
                        {
                            w.Write(new { type = "addr_label_of", label, dst_guid = eGuid, provenance = "addressables", detail = new { group = group.Name, path = ePath } });
                        }
                    }
                }
            }
        }
#endif

        LogInfo($"[Indexer] done → {opt.indexOut}");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    // ===== 보조 함수들 =====
    static string KindFromPathAndType(string path, Type t)
    {
        if (string.IsNullOrEmpty(path)) return "unknown";
        if (path.EndsWith(".unity", StringComparison.OrdinalIgnoreCase)) return "scene";
        if (path.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase)) return "prefab";
        if (path.EndsWith(".controller", StringComparison.OrdinalIgnoreCase)) return "animator_controller";
        if (path.EndsWith(".anim", StringComparison.OrdinalIgnoreCase)) return "animation_clip";
        if (path.EndsWith(".mat", StringComparison.OrdinalIgnoreCase)) return "material";
        if (path.EndsWith(".shader", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".shadergraph", StringComparison.OrdinalIgnoreCase)) return "shader";
        if (path.EndsWith(".asset", StringComparison.OrdinalIgnoreCase)) return "scriptable_object";
        if (path.StartsWith("Assets/Resources/", StringComparison.OrdinalIgnoreCase) || path.Contains("/Resources/")) return "resources_asset";
        return t != null ? t.Name.ToLowerInvariant() : "asset";
    }

    static bool IsPrefab(string path) => path.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase);

    static void CollectSceneOverridesForAllBuildScenes(NdjsonWriter w)
    {
        var scenePaths = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToList();
        foreach (var sPath in scenePaths)
        {
            try
            {
                var scene = EditorSceneManager.OpenScene(sPath, OpenSceneMode.Single);
                foreach (var root in scene.GetRootGameObjects())
                {
                    var gos = root.GetComponentsInChildren<Transform>(true).Select(t => t.gameObject);
                    foreach (var go in gos)
                    {
                        var status = PrefabUtility.GetPrefabInstanceStatus(go);
                        if (status == PrefabInstanceStatus.NotAPrefab) continue;

                        var src = PrefabUtility.GetCorrespondingObjectFromSource(go);
                        var srcPath = AssetDatabase.GetAssetPath(src);
                        var srcGuid = AssetDatabase.AssetPathToGUID(srcPath);

                        var propMods = PrefabUtility.GetPropertyModifications(go);
                        if (propMods != null)
                        {
                            foreach (var m in propMods)
                            {
                                w.Write(new
                                {
                                    type = "edge",
                                    rel = "override-of",
                                    src_guid = "SCENE:" + sPath, // 씬 노드 식별자
                                    dst_guid = srcGuid,
                                    provenance = "prefab_instance",
                                    detail = new { go = go.name, propertyPath = m.propertyPath, value = m.value }
                                });
                            }
                        }

                        var added = PrefabUtility.GetAddedComponents(go);
                        foreach (var ac in added)
                        {
                            w.Write(new
                            {
                                type = "edge",
                                rel = "added-component",
                                src_guid = "SCENE:" + sPath,
                                dst_guid = srcGuid,
                                provenance = "prefab_instance",
                                detail = new { go = go.name, component = ac.instanceComponent ? ac.instanceComponent.GetType().FullName : null }
                            });
                        }
                        var removed = PrefabUtility.GetRemovedComponents(go);
                        foreach (var rc in removed)
                        {
                            w.Write(new
                            {
                                type = "edge",
                                rel = "removed-component",
                                src_guid = "SCENE:" + sPath,
                                dst_guid = srcGuid,
                                provenance = "prefab_instance",
                                detail = new { go = go.name, component = rc.assetComponent ? rc.assetComponent.GetType().FullName : null }
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogWarn($"[Indexer] scene override fail: {sPath} - {ex.Message}");
            }
        }
    }

    static void LogInfo(string msg) => Debug.Log(msg);
    static void LogWarn(string msg) => Debug.LogWarning(msg);
}

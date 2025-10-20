using System.Collections.Generic;
using UnityEngine;
public static class SaveBus
{
    private static readonly Dictionary<string, GameData> lastPayload = new();
    private static readonly Dictionary<string, List<ISavable>> listeners = new();

    public static void Publish(string key, GameData data)
    {
        lastPayload[key] = data;
        if (listeners.TryGetValue(key, out var list))
        {
            // 현재 살아있는 모든 리스너에 즉시 전달
            foreach (var l in list) l.Load(data);
            Debug.Log($"[SaveBus] Published to {list.Count} listeners for key '{key}'");
        }
    }

    public static void Register(ISavable target)
    {
        if (!listeners.TryGetValue(target.SaveKey, out var list))
        {
            list = new List<ISavable>();
            listeners[target.SaveKey] = list;
        }
        if (!list.Contains(target)) list.Add(target);

        // 과거에 퍼블리시된 데이터가 있었다면 즉시 리플레이
        if (lastPayload.TryGetValue(target.SaveKey, out var cached))
        {
            target.Load(cached);
        }
    }

    public static void Unregister(ISavable target)
    {
        if (listeners.TryGetValue(target.SaveKey, out var list))
        {
            list.Remove(target);
        }
    }
}
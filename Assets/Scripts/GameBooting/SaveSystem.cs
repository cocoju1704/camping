using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;


public class SaveSystem : Singleton<SaveSystem>
{
    [SerializeField] string saveFileName;
    JsonSerializerSettings settings;
    string saveFilePath;

    protected override void Awake() {
        settings = new JsonSerializerSettings { 
            TypeNameHandling = TypeNameHandling.Auto, 
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore ,
        };
        saveFileName = "save.json";
        saveFilePath = Path.Combine(Application.dataPath, saveFileName);
    }
    void OnApplicationQuit() {
        SaveFile();
    }
    public bool SaveFileExists() {
        return File.Exists(saveFilePath);
    }
    public bool SaveFile() {
        GameData data = new GameData();
        List<ISavable> savables = FindObjectsOfType<MonoBehaviour>().OfType<ISavable>().ToList();
        foreach (ISavable savable in savables) {
            savable.Save(data);
        }
        string json = JsonConvert.SerializeObject(data, Formatting.Indented, settings);
        File.WriteAllText(saveFilePath, json);
        return true;
    }
    public bool LoadFile() {
        if(!File.Exists(saveFilePath)) {
            return false;
        }

        string jsonFormat = File.ReadAllText(saveFilePath);
        GameData gameData = JsonConvert.DeserializeObject<GameData>(jsonFormat, settings);

        List<ISavable> savables = FindObjectsOfType<MonoBehaviour>().OfType<ISavable>().ToList();
        foreach(ISavable savable in savables) {
            savable.Load(gameData);
        }

        return true;
    }

    public bool DeleteFile() {
        if(!File.Exists(saveFilePath)) {
            return false;
        }

        File.Delete(saveFilePath);
        return true;
    }
}

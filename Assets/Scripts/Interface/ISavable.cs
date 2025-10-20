public interface ISavable {
    string SaveKey { get; }   // 예: "Interior"
    void Save(GameData data);
    void Load(GameData data);
}
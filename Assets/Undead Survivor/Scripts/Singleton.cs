using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
    static T _instance;
    public static T instance {
        get {
            if (_instance == null) {
                _instance = (T)FindObjectOfType(typeof(T));
            }

            return _instance;
        }
    }

    protected virtual void Awake() {
        if(_instance == null) {
            _instance = this as T;
        }
        else if(_instance != null) {
            Destroy(gameObject);
            return;
        }
    }
}
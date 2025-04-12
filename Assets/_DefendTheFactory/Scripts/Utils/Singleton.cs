using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component {
    protected static T instance;
    public static bool HasInstance => instance != null;
    public static T TryGetInstance() => instance ? instance : null;
    public static T Current => instance;

    public static T Instance {
        get {
            if (instance != null) return instance;

            instance = FindFirstObjectByType<T>();

            if (instance != null) return instance;

            GameObject obj = new GameObject { name = typeof(T).Name + "( Singleton Auto Created)" };
            instance = obj.AddComponent<T>();
            return instance;
        }
    }

    protected virtual void Awake() => InitializeSingleton();

    protected virtual void InitializeSingleton() {
        if (!Application.isPlaying) {
            return;
        }

        instance = this as T;
    }
}
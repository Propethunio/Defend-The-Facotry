using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component {
    private static T instance;
    public static bool HasInstance => instance != null;
    public static T TryGetInstance() => instance ? instance : null;
    public static T Current => instance;

    public static T Instance {
        get {
            if (instance != null) return instance;

            instance = FindFirstObjectByType<T>();

            if (instance != null) return instance;

            Debug.LogError($"Singleton {typeof(T).Name} not found");
            return null;
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
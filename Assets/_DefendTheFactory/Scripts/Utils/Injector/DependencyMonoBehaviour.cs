using UnityEngine;

public abstract class DependencyMonoBehaviour<T> : MonoBehaviour where T : class {
    protected virtual void Awake() {
        Injector.Register<T>(this as T);
    }
}
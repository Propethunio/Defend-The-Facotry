using UnityEngine;

public abstract class DependencyMonoBehaviour<T> : MonoBehaviour, IDependencyProvider where T : class {
    [Provide]
    public virtual T ProvideDependencies() {
        return this as T;
    }
}
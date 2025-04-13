using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property)]
public sealed class InjectAttribute : PropertyAttribute { }

[AttributeUsage(AttributeTargets.Method)]
public sealed class ProvideAttribute : PropertyAttribute { }

[DefaultExecutionOrder(-1000)]
public class Injector : Singleton<Injector> {
    private const BindingFlags k_bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    private readonly Dictionary<Type, object> registry = new();

    protected override void Awake() {
        base.Awake();

        var monoBehaviours = FindMonoBehaviours();
        var providers = monoBehaviours.OfType<IDependencyProvider>().ToArray();
        var injectables = monoBehaviours.Where(IsInjectable).ToArray();

        foreach (var provider in providers) {
            Register(provider);
        }

        foreach (var injectable in injectables) {
            Inject(injectable);
        }
    }

    private void OnDestroy() {
        ClearDependencies();
        registry.Clear();
        Debug.Log("[Injector] Registry cleared and dependencies wiped.");
    }

    public static void Register<T>(T instance) {
        Instance.registry[typeof(T)] = instance;
    }

    public static T Resolve<T>() where T : class {
        return Instance.Resolve(typeof(T)) as T;
    }

    private object Resolve(Type type) {
        registry.TryGetValue(type, out var instance);
        return instance;
    }

    private void Inject(object instance) {
        var type = instance.GetType();

        foreach (var field in type.GetFields(k_bindingFlags)) {
            if (!Attribute.IsDefined(field, typeof(InjectAttribute))) continue;

            if (field.GetValue(instance) != null) {
                Debug.LogWarning($"[Injector] Field '{field.Name}' of class '{type.Name}' is already set.");
                continue;
            }

            var resolved = Resolve(field.FieldType);

            if (resolved == null) {
                throw new Exception($"Failed to inject dependency into field '{field.Name}' of class '{type.Name}'.");
            }

            field.SetValue(instance, resolved);
        }

        foreach (var method in type.GetMethods(k_bindingFlags)) {
            if (!Attribute.IsDefined(method, typeof(InjectAttribute))) continue;

            var parameters = method.GetParameters();
            var args = new object[parameters.Length];
            bool fail = false;

            for (int i = 0; i < parameters.Length; i++) {
                args[i] = Resolve(parameters[i].ParameterType);

                if (args[i] != null) continue;

                fail = true;
                break;
            }

            if (fail) {
                throw new Exception($"Failed to inject dependencies into method '{method.Name}' of class '{type.Name}'.");
            }

            method.Invoke(instance, args);
        }

        foreach (var property in type.GetProperties(k_bindingFlags)) {
            if (!Attribute.IsDefined(property, typeof(InjectAttribute))) continue;

            var resolved = Resolve(property.PropertyType);

            if (resolved == null) {
                throw new Exception($"Failed to inject dependency into property '{property.Name}' of class '{type.Name}'.");
            }

            property.SetValue(instance, resolved);
        }
    }

    private void Register(IDependencyProvider provider) {
        foreach (var method in provider.GetType().GetMethods(k_bindingFlags)) {
            if (!Attribute.IsDefined(method, typeof(ProvideAttribute))) continue;

            var instance = method.Invoke(provider, null);

            if (instance != null) {
                registry.Add(method.ReturnType, instance);
            }
            else {
                throw new Exception($"Provider method '{method.Name}' in class '{provider.GetType().Name}' returned null when providing type '{method.ReturnType.Name}'.");
            }
        }
    }

    public void ValidateDependencies() {
        var monoBehaviours = FindMonoBehaviours();
        var providers = monoBehaviours.OfType<IDependencyProvider>().ToArray();
        var providedDependencies = GetProvidedDependencies(providers);
        var invalidDependencies = new List<string>();

        foreach (var mb in monoBehaviours) {
            foreach (var field in mb.GetType().GetFields(k_bindingFlags)) {
                if (!Attribute.IsDefined(field, typeof(InjectAttribute))) continue;

                if (field.GetValue(mb) == null && !providedDependencies.Contains(field.FieldType)) {
                    invalidDependencies.Add($"[Validation] {mb.GetType().Name} is missing dependency {field.FieldType.Name} on GameObject {mb.gameObject.name}");
                }
            }
        }

        if (invalidDependencies.Count == 0) {
            Debug.Log("[Validation] All dependencies are valid.");
        }
        else {
            Debug.LogError($"[Validation] {invalidDependencies.Count} dependencies are invalid:");

            foreach (var message in invalidDependencies) {
                Debug.LogError(message);
            }
        }
    }

    private HashSet<Type> GetProvidedDependencies(IEnumerable<IDependencyProvider> providers) {
        var set = new HashSet<Type>();

        foreach (var provider in providers) {
            foreach (var method in provider.GetType().GetMethods(k_bindingFlags)) {
                if (Attribute.IsDefined(method, typeof(ProvideAttribute))) {
                    set.Add(method.ReturnType);
                }
            }
        }

        return set;
    }

    public void ClearDependencies() {
        foreach (var mb in FindMonoBehaviours()) {
            foreach (var field in mb.GetType().GetFields(k_bindingFlags)) {
                if (Attribute.IsDefined(field, typeof(InjectAttribute))) {
                    field.SetValue(mb, null);
                }
            }
        }
    }

    private static MonoBehaviour[] FindMonoBehaviours() {
        return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
    }

    private static bool IsInjectable(MonoBehaviour obj) {
        return obj.GetType().GetMembers(k_bindingFlags).Any(m => Attribute.IsDefined(m, typeof(InjectAttribute)));
    }
}
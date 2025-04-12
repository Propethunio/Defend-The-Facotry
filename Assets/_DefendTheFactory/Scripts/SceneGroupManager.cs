using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneGroupManager {
    public event Action OnSceneLoaded;
    public event Action OnSceneGroupLoaded;
    public event Action OnSceneUnloaded;
    public event Action OnSceneGroupUnloaded;

    private SceneGroup activeSceneGroup;

    public async Task LoadScenes(SceneGroup group, IProgress<float> progress, float minLoadingDuration = 0f, bool reloadScenes = false) {
        activeSceneGroup = group;
        var loadedScenes = new List<string>();

        await UnloadScenes();

        int sceneCount = SceneManager.sceneCount;

        for (int i = 0; i < sceneCount; i++) {
            loadedScenes.Add(SceneManager.GetSceneAt(i).name);
        }

        var totalScenesToLoad = activeSceneGroup.scenes.Count;

        var operationGroup = new AsyncOperationGroup(totalScenesToLoad);

        for (int i = 0; i < totalScenesToLoad; i++) {
            var sceneData = group.scenes[i];

            if (!reloadScenes && loadedScenes.Contains(sceneData.Name)) continue;

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneData.reference.Path, LoadSceneMode.Additive);
            operation.allowSceneActivation = false;
            operationGroup.operations.Add(operation);
            OnSceneLoaded?.Invoke();
        }

        float loadingTimer = 0f;

        while (loadingTimer < minLoadingDuration) {
            loadingTimer += 0.05f;
            float sceneProgress = operationGroup.progress;
            float timeProgress = Mathf.Clamp01(loadingTimer / minLoadingDuration);
            float smallerProgress = Mathf.Min(sceneProgress, timeProgress) + 0.1f;
            progress?.Report(smallerProgress);
            await Task.Delay(50);
        }

        foreach (var op in operationGroup.operations) {
            op.allowSceneActivation = true;
        }

        while (!operationGroup.isDone) {
            await Task.Delay(50);
        }

        await Task.Delay(200);

        foreach (var op in operationGroup.operations) {
            op.allowSceneActivation = true;
        }

        Scene activeScene = SceneManager.GetSceneByName(activeSceneGroup.FindSceneNameByType(SceneType.ActiveScene));

        if (activeScene.IsValid()) {
            SceneManager.SetActiveScene(activeScene);
        }

        OnSceneGroupLoaded?.Invoke();
    }

    private async Task UnloadScenes() {
        List<string> scenes = new List<string>();
        int sceneCount = SceneManager.sceneCount;

        for (int i = sceneCount - 1; i > 0; i--) {
            var sceneAt = SceneManager.GetSceneAt(i);
            if (!sceneAt.isLoaded) continue;

            var sceneName = sceneAt.name;
            if (sceneName == "Bootstrapper") continue;

            scenes.Add(sceneName);
        }

        var operationGroup = new AsyncOperationGroup(scenes.Count);

        foreach (var scene in scenes) {
            var operation = SceneManager.UnloadSceneAsync(scene);
            if (operation == null) continue;

            operationGroup.operations.Add(operation);
            OnSceneUnloaded?.Invoke();
        }

        while (!operationGroup.isDone) {
            await Task.Delay(50);
        }

        OnSceneGroupUnloaded?.Invoke();
    }
}

public readonly struct AsyncOperationGroup {
    public readonly List<AsyncOperation> operations;
    public float progress => operations.Count == 0 ? 0 : operations.Average(o => Mathf.Clamp01(o.progress / 0.9f));
    public bool isDone => operations.All(o => o.isDone);

    public AsyncOperationGroup(int initialCapacity) {
        operations = new List<AsyncOperation>(initialCapacity);
    }
}
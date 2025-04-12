using System;
using System.Threading.Tasks;
using UnityEngine;

public class SceneLoader : Singleton<SceneLoader> {
    public event Action OnSceneLoaded;
    public event Action OnSceneGroupLoaded;
    public event Action OnSceneUnloaded;
    public event Action OnSceneGroupUnloaded;

    [SerializeField] private float minLoadTime;
    [SerializeField] private LoadingScreen loadingScreenPrefab;
    [SerializeField] private SceneGroup[] sceneGroups;

    private readonly SceneGroupManager manager = new SceneGroupManager();

    protected override void Awake() {
        base.Awake();
        manager.OnSceneLoaded += () => OnSceneLoaded?.Invoke();
        manager.OnSceneGroupLoaded += () => OnSceneGroupLoaded?.Invoke();
        manager.OnSceneUnloaded += () => OnSceneUnloaded?.Invoke();
        manager.OnSceneGroupUnloaded += () => OnSceneGroupUnloaded?.Invoke();
    }

    private async void Start() {
        await LoadSceneGroup(0, true);
    }

    public async Task LoadSceneGroup(int index, bool loadImmediately = false) {
        LoadingScreen loadingScreen = Instantiate(loadingScreenPrefab);

        if (loadImmediately) {
            loadingScreen.UpdateTargetValue(1f);
        }

        LoadingProgress loadingProgress = new LoadingProgress();
        loadingProgress.Progressed += OnProgressed;
        await manager.LoadScenes(sceneGroups[index], loadingProgress, loadImmediately ? 0 : minLoadTime);
        loadingProgress.Progressed -= OnProgressed;
        Destroy(loadingScreen.gameObject);
        return;

        void OnProgressed(float target) => loadingScreen.UpdateTargetValue(target);
    }
}

public class LoadingProgress : IProgress<float> {
    public event Action<float> Progressed;

    private const float ratio = 1f;

    public void Report(float value) {
        Progressed?.Invoke(value / ratio);
    }
}
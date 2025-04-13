using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static async void Init() {
        await SceneManager.LoadSceneAsync("Bootstrapper", LoadSceneMode.Single);
    }
}
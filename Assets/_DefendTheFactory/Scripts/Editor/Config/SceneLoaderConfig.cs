using UnityEngine;
using Eflatun.SceneReference;

[System.Serializable]
public class SceneGroupEditor {
    public string groupName;
    public SceneReference[] scenes;
}

[CreateAssetMenu(fileName = "SceneLoaderConfig", menuName = "Tools/Scene Loader Config")]
public class SceneLoaderConfig : ScriptableObject {
    public SceneGroupEditor[] sceneGroups;
}
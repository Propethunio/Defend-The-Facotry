using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneLoaderEditor : EditorWindow {
    private SceneLoaderConfig config;
    private static SceneLoaderEditor window;
    private const string configPath = "Assets/_DefendTheFactory/Scripts/Editor/Config/SceneLoaderConfig.asset";

    [MenuItem("Tools/Scene Loader")]
    public static void ShowWindow() {
        if (window == null) {
            window = GetWindow<SceneLoaderEditor>("Scene Loader");
        }

        window.LoadConfig();
        window.Show();
    }

    private void OnEnable() {
        LoadConfig();
    }

    private void LoadConfig() {
        config = AssetDatabase.LoadAssetAtPath<SceneLoaderConfig>(configPath);

        if (config == null) {
            Debug.LogWarning($"SceneLoaderConfig not found at path: {configPath}");
        }
    }

    private void OnGUI() {
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Scene Config", EditorStyles.boldLabel);
        GUI.enabled = false;
        EditorGUILayout.ObjectField(config, typeof(SceneLoaderConfig), false);
        GUI.enabled = true;

        if (config != null) {
            var rect = GUILayoutUtility.GetLastRect();

            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition)) {
                EditorGUIUtility.PingObject(config);
                Selection.activeObject = config;
            }
        }

        EditorGUILayout.Space(10);
        GUILayout.Label("Scene Groups To Load:", EditorStyles.boldLabel);

        if (config == null || config.sceneGroups == null || config.sceneGroups.Length == 0) {
            EditorGUILayout.HelpBox("Assign a SceneLoaderConfig with at least one SceneGroup.", MessageType.Info);
            return;
        }

        foreach (var group in config.sceneGroups) {
            if (group?.scenes == null || group.scenes.Length == 0) continue;

            EditorGUILayout.Space(2);

            if (!GUILayout.Button("Load: " + group.groupName)) continue;

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
                LoadSceneGroup(group);
            }
        }
    }

    private void LoadSceneGroup(SceneGroupEditor groupEditor) {
        for (int i = 0; i < groupEditor.scenes.Length; i++) {
            var sceneRef = groupEditor.scenes[i];

            if (sceneRef == null || string.IsNullOrEmpty(sceneRef.Path)) continue;

            var mode = (i == 0) ? OpenSceneMode.Single : OpenSceneMode.Additive;
            EditorSceneManager.OpenScene(sceneRef.Path, mode);
        }
    }
}
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class DefendTheFactoryEditor : OdinMenuEditorWindow {

    private static readonly string[] mainTabs = { "Machines", "Items", "Recipes", "Towers", "Enemies", "Localization" };
    private int selectedMainTab = 0;
    private int selectedSubTab = 0;

    private static readonly Type[] machineTypes = { typeof(GatheringMachineSO), typeof(ConstructorSO) };
    private Type selectedType;
    private NewFactoryData newFactoryData;
    private BaseBuildableObjectSO selectedObject;

    [MenuItem("Tools/DTF Editor")]
    static void OpenEditor() => GetWindow<DefendTheFactoryEditor>().Show();

    protected override void OnImGUI() {
        selectedMainTab = GUILayout.Toolbar(selectedMainTab, mainTabs, GUILayout.Height(34));

        if(selectedMainTab == 0) {
            string[] machineTypeNames = machineTypes.Select(t => Regex.Replace(t.Name.Replace("SO", ""), "(?<!^)([A-Z])", " $1")).ToArray();
            selectedSubTab = GUILayout.Toolbar(selectedSubTab, machineTypeNames, GUILayout.Height(28));
            selectedType = machineTypes[selectedSubTab];
        } else if(selectedMainTab == 1) {

        } else if(selectedMainTab == 2) {

        } else if(selectedMainTab == 3) {

        } else if(selectedMainTab == 4) {

        } else {

        }

        base.OnImGUI();
    }

    protected override OdinMenuTree BuildMenuTree() {
        OdinMenuTree menuTree = new OdinMenuTree();

        if(selectedMainTab == 0) {
            string directory = GetDirectoryForType(selectedType);
            menuTree.Add("Create New", new NewFactoryData(selectedType));
            var assets = menuTree.AddAllAssetsAtPath(string.Empty, directory, selectedType);
            assets.AddIcons<BaseBuildableObjectSO>(x => x.icon);
        } else if(selectedMainTab == 1) {
            // Handle the second tab (if necessary)
            /*string towerDirectory = "Assets/_DefendTheFactory/Data/TowerDefense/Towers";
            var towerSection = new MachineTypeMenu(typeof(TowerSO));
            menuTree.Add("Create New", towerSection);
            var towerAssets = menuTree.AddAllAssetsAtPath(string.Empty, towerDirectory, typeof(TowerSO), true);
            towerAssets.AddIcons<BaseBuildableObjectSO>(x => x.icon);*/
        }

        return menuTree;
    }


    protected override void OnBeginDrawEditors() {
        OdinMenuTreeSelection selected = MenuTree.Selection;
        SirenixEditorGUI.BeginHorizontalToolbar();
        GUILayout.FlexibleSpace();
        selectedObject = selected.SelectedValue as BaseBuildableObjectSO;

        if(selectedObject) {
            if(SirenixEditorGUI.ToolbarButton("Delete Object")) {
                DeleteCurrentObject();
            }
        } else if(selected.SelectedValue is NewFactoryData newMachine) {
            if(SirenixEditorGUI.ToolbarButton("Create Object")) {
                newMachine.CreateNewData();
            }
        }

        SirenixEditorGUI.EndHorizontalToolbar();
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        if(newFactoryData != null) {
            DestroyImmediate(newFactoryData.factoryData);
        }
    }

    void DeleteCurrentObject() {
        string path = AssetDatabase.GetAssetPath(selectedObject);
        AssetDatabase.DeleteAsset(path);
        AssetDatabase.SaveAssets();
    }

    private static string GetDirectoryForType(Type type) {
        if(type == typeof(GatheringMachineSO)) return "Assets/_DefendTheFactory/Data/FactorySide/GatheringMachines";
        if(type == typeof(ConstructorSO)) return "Assets/_DefendTheFactory/Data/FactorySide/Constructors";
        return "Assets/_DefendTheFactory/Data/FactorySide";
    }

    public class NewFactoryData {
        private Type selectedType;

        public NewFactoryData(Type selectedType) {
            this.selectedType = selectedType;
            factoryData = CreateInstance(selectedType) as BaseBuildableObjectSO;
        }

        [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
        public BaseBuildableObjectSO factoryData;

        public void CreateNewData() {
            if(string.IsNullOrWhiteSpace(factoryData.nameString)) {
                Debug.LogWarning("Cannot create asset: NameString is empty.");
                return;
            }

            string directory = GetDirectoryForType(selectedType) + "/";
            Directory.CreateDirectory(directory);
            string assetPath = directory + factoryData.nameString.Trim() + ".asset";

            AssetDatabase.CreateAsset(factoryData, assetPath);
            AssetDatabase.SaveAssets();

            // Ensure Unity recognizes the change
            EditorUtility.SetDirty(factoryData);
            AssetDatabase.Refresh();

            // Create a new instance so the user can immediately edit a fresh one
            factoryData = ScriptableObject.CreateInstance(selectedType) as BaseBuildableObjectSO;
            EditorUtility.SetDirty(factoryData);
        }
    }
}
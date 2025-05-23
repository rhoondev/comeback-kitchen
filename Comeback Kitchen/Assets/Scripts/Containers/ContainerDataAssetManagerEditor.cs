using UnityEditor;
using UnityEngine;
using System.IO;

[CustomEditor(typeof(ContainerDataAssetManager))]
public class ContainerDataAssetManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        ContainerDataAssetManager containerDataHandler = (ContainerDataAssetManager)target;

        if (target == null)
        {
            EditorGUILayout.HelpBox("Target is missing or destroyed.", MessageType.Error);
            return;
        }

        SerializedProperty containerProp = serializedObject.FindProperty("container");
        SerializedProperty prefabProp = serializedObject.FindProperty("objectPrefab");
        SerializedProperty assetProp = serializedObject.FindProperty("containerDataAsset");

        // Draw other fields first
        EditorGUILayout.PropertyField(containerProp);
        EditorGUILayout.PropertyField(prefabProp);

        // Draw the ContainerDataAsset field with inline "New" button LAST
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(assetProp, new GUIContent("Container Data Asset"));
        if (assetProp.objectReferenceValue == null)
        {
            if (GUILayout.Button("New", GUILayout.Width(40)))
            {
                CreateAndAssignAsset(containerDataHandler);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // Show Save/Load buttons only if asset exists
        if (containerDataHandler.ContainerDataAsset != null)
        {
            if (GUILayout.Button("Save to Asset"))
            {
                SaveToAsset(containerDataHandler);
            }

            if (GUILayout.Button("Load from Asset"))
            {
                containerDataHandler.LoadFromAsset();
            }

            if (GUILayout.Button("Clear Objects"))
            {
                containerDataHandler.ClearObjects();
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void CreateAndAssignAsset(ContainerDataAssetManager containerDataHandler)
    {
        // Ensure directory exists
        string directory = "Assets/Data";

        if (!AssetDatabase.IsValidFolder(directory))
        {
            Directory.CreateDirectory(directory);
            AssetDatabase.Refresh();
        }

        // Auto-generate unique name
        string baseName = containerDataHandler.name + "_DataAsset";
        string path = AssetDatabase.GenerateUniqueAssetPath($"{directory}/{baseName}.asset");

        // Create and save asset
        ContainerDataAsset newAsset = CreateInstance<ContainerDataAsset>();
        AssetDatabase.CreateAsset(newAsset, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Assign asset to the ObjectSaver instance
        Undo.RecordObject(containerDataHandler, "Assign Data Asset");
        containerDataHandler.ContainerDataAsset = newAsset;
        EditorUtility.SetDirty(containerDataHandler);

        Debug.Log("Created and assigned new asset at: " + path);
    }

    private void SaveToAsset(ContainerDataAssetManager containerDataHandler)
    {
        containerDataHandler.ContainerDataAsset.objectData.Clear();

        foreach (Transform obj in containerDataHandler.Container.ObjectHolder)
        {
            containerDataHandler.ContainerDataAsset.objectData.Add(new ObjectData(obj.localPosition, obj.localRotation));
        }

        EditorUtility.SetDirty(containerDataHandler.ContainerDataAsset);
        AssetDatabase.SaveAssets();

        Debug.Log("Object data saved to asset.");
    }
}

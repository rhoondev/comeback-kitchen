using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObjectSaver))]
public class ObjectSaverEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ObjectSaver objectSaver = (ObjectSaver)target;

        if (GUILayout.Button("Save"))
        {
            objectSaver.Save();
        }

        if (GUILayout.Button("Load"))
        {
            objectSaver.Load();
        }
    }
}

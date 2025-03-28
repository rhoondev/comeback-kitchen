using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RiceLoader))]
public class RiceLoaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RiceLoader loader = (RiceLoader)target;

        if (GUILayout.Button("Save Rice"))
        {
            loader.SaveRice();
        }

        if (GUILayout.Button("Load Rice"))
        {
            loader.LoadRice();
        }
    }
}

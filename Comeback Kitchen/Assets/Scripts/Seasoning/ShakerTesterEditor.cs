using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShakerTester))]
public class ShakerTesterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ShakerTester shakerTester = (ShakerTester)target;

        if (GUILayout.Button("Shake"))
        {
            shakerTester.Shake();
        }

        if (GUILayout.Button("Sprinkle"))
        {
            shakerTester.Sprinkle();
        }
    }
}

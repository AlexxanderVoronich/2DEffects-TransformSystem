using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(effectsManager))]
public class scriptEffectsManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        effectsManager my_script = (effectsManager)target;

        if (GUILayout.Button("Clear effects (test)"))
        {
            my_script.runClearTest();
        }


        if (GUILayout.Button("Run effect (test)"))
        {
            my_script.runTest();
        }

    }
}
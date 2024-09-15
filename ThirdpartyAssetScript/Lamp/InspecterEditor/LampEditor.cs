#if UNITY_EDITOR && Light2D
using UnityEngine;
using UnityEditor;
using lLCroweTool.LampSystem;

[CustomEditor(typeof(Lamp))]
public class LampEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Lamp lamp = (Lamp)target;

        if (GUILayout.Button("LampONOFF"))
        {
            lamp.ActiveLamp();
        }
        if (GUILayout.Button("ResetLamp"))
        {
            lamp.ResetLamp();
        }
    }
}
#endif

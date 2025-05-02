using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(AssetGridSetter))]
public class AssetGridSetterInspectorEditor : Editor
{
    private AssetGridSetter targetComponent;
    private void OnEnable()
    {
        targetComponent = target as AssetGridSetter;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Á¤·Ä"))
        {
            targetComponent.BatchObject();
        }
    }
}
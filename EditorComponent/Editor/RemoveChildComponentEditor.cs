using lLCroweTool.QC.EditorOnly;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RemoveChildComponent))]
public class RemoveChildComponentEditor : Editor
{
    private RemoveChildComponent targetComponent;
    private void OnEnable()
    {
        targetComponent = target as RemoveChildComponent;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("RemoveTargetComponentForAllChild"))
        {
            targetComponent.RemoveTargetComponentForAllChild();
        }

        if (GUILayout.Button("DeleteThisComponent"))
        {
            targetComponent.DeleteThisComponent();
        }
    }
}
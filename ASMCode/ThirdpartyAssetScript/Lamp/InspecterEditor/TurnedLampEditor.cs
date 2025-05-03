#if UNITY_EDITOR && Light2D
using UnityEngine;
using UnityEditor;
using lLCroweTool.LampSystem;

namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(TurnedLamp))]
    public class TurnedLampEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            TurnedLamp lamp = (TurnedLamp)target;

            if (GUILayout.Button("TurnedLampONOFF"))
            {
                lamp.ActiveTurnedLamp();
            }
        }
    }
}
#endif


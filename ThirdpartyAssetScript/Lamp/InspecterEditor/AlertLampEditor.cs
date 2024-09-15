
#if UNITY_EDITOR && Light2D
using UnityEngine;
using UnityEditor;
using lLCroweTool.LampSystem;

namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(AlertLamp))]
    public class AlertLampEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            AlertLamp lamp = (AlertLamp)target;

            //나중에 에디터버튼 수정해야함
            //if (GUILayout.Button("LampONOFF"))
            //{
            //    lamp.ActiveLamp();
            //    lamp.ActiveAlertLamp(false);
            //}
            //if (GUILayout.Button("ResetLamp"))
            //{
            //    lamp.ResetLamp();
            //    lamp.ActiveAlertLamp(false);
            //}
            if (GUILayout.Button("LampAlertONOFF"))
            {
                lamp.ActiveAlertLamp();
            }
            if (GUILayout.Button("ResetLampAlert"))
            {
                lamp.ResetAlertLamp();
            }
        }
    }
}
#endif
#if UNITY_EDITOR && Shape
using UnityEngine;
using Shapes;
using lLCroweTool.Visual.ShapeAsset;
using static lLCroweTool.Visual.ShapeAsset.ShapeOffSetAnim;

using UnityEditor;
#pragma warning disable 0618
namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(ShapeOffSetAnim))]
    public class ShapeOffSetAnimInspectorEditor : Editor
    {
        private ShapeOffSetAnim targetShapeOffSetAnim;
        
        private void OnEnable()
        {
            targetShapeOffSetAnim = (ShapeOffSetAnim)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("대쉬오프셋 초기세팅하기"))
            {
                targetShapeOffSetAnim.dashableArray = targetShapeOffSetAnim.GetComponentsInChildren<IDashable>();
                targetShapeOffSetAnim.offSetSettingArray = new OffSetSetting[targetShapeOffSetAnim.dashableArray.Length];
            }            
        }
    }
}
#endif
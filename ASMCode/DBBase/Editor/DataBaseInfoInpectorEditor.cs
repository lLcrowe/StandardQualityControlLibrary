//using lLCroweTool.DataBase;
//using UnityEditor;
//using UnityEngine;

//namespace lLCroweTool.QC.EditorOnly
//{
//    [CustomEditor(typeof(DataBaseInfo_Base))]
//    public class DataBaseInfoInpectorEditor : Editor
//    {
//        private DataBaseInfo_Base targetDataBaseInfo;

//        private void OnEnable()
//        {
//            targetDataBaseInfo = (DataBaseInfo_Base)target;
//        }

//        public override void OnInspectorGUI()
//        {
//            base.OnInspectorGUI();

//            if (GUILayout.Button("게임데이터베이스윈도우열기"))
//            {
//                DataBaseInfoBaseWindowEditor.targetDataBaseInfo = targetDataBaseInfo;
//                DataBaseInfoBaseWindowEditor.ShowWindow();
//            }
//        }
//    }
//}
#if UNITY_EDITOR && Doozy
using UnityEngine;
using lLCroweTool.QuestSystem;
using UnityEditor;
#pragma warning disable 0618
namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(QuestDBObjectScript))]
    public class QuestDBInspectorEditor : Editor
    {
        //인스팩터창 에디터

        //현재 등록된 기초정보들과 들어간 데이터량들을 표시
        //윈도우에디터를 켜서 해당 DB 데이터를 볼수 있게 세팅
        private QuestDBObjectScript dBObjectScript;
        private bool checkOverlap = false;//중복체크

        private void OnEnable()
        {
            dBObjectScript = (QuestDBObjectScript)target;

            string path = "Assets/Resources";
            QuestDBObjectScript[] questDBObjectScriptArray = lLcroweUtilEditor.GetCustomFile<QuestDBObjectScript>.GetScriptableObjectFile(path, "*.asset");
            if (questDBObjectScriptArray.Length == 0)
            {
                checkOverlap = questDBObjectScriptArray.Length > 1 ? true : false;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (checkOverlap)
            {
                EditorGUILayout.HelpBox("데이터가 중복되지않아야됩니다.", MessageType.Warning);
            }

            if (GUILayout.Button("퀘스트윈도우 에디터열기"))
            {
                QuestDBWindowEditor.SetQuestDBData(dBObjectScript);
                QuestDBWindowEditor.ShowWindow();
            }
        }
    }
}
#endif
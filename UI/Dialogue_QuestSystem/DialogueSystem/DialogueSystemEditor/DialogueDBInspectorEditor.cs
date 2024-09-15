using UnityEngine;
#if UNITY_EDITOR && Doozy
using lLCroweTool.DialogueSystem;
using UnityEditor;
#pragma warning disable 0618
namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(DialogueDBObjectScript))]
    public class DialogueDBInspectorEditor : Editor
    {
        //인스팩터창 에디터

        //현재 등록된 기초정보들과 들어간 데이터량들을 표시
        //윈도우에디터를 켜서 해당 DB 데이터를 볼수 있게 세팅        
        private DialogueDBObjectScript dBObjectScript;
        private bool checkOverlap = false;//중복체크

        private void OnEnable()
        {
            dBObjectScript = (DialogueDBObjectScript)target;

            string path = "Assets/Resources";
            DialogueDBObjectScript[] dialogueDBObjectScriptArray = lLcroweUtilEditor.GetCustomFile<DialogueDBObjectScript>.GetScriptableObjectFile(path, "*.asset");
            if (dialogueDBObjectScriptArray.Length == 0)
            {
                checkOverlap = dialogueDBObjectScriptArray.Length > 1 ? true : false;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (checkOverlap)
            {
                EditorGUILayout.HelpBox("데이터가 중복되지않아야됩니다.", MessageType.Warning);
            }

            if (GUILayout.Button("대화윈도우 에디터열기"))
            {
                DialogueDBWindowEditor.SetDialogueDBData(dBObjectScript);
                DialogueDBWindowEditor.ShowWindow();                
            }
        }
    }
}
#endif
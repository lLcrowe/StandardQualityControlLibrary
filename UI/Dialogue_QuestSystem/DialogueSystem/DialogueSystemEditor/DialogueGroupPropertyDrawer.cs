#if Doozy

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using lLCroweTool.DialogueSystem;

namespace lLCroweTool.QC.EditorOnly
{
    [CustomPropertyDrawer(typeof(DialogueGroupAttribute))]
    public class DialogueGroupPropertyDrawer : PropertyDrawer
    {  
        private int index;
        private List<string> groupNames = new List<string>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {   
            //DialogueDBObjectScript dialogueDBData = Resources.Load<DialogueDBObjectScript>("ScrapNomadDialogueDB");

            string path = "Assets/Resources";
            DialogueDBObjectScript[] dialogueDBObjectScriptArray = lLcroweUtilEditor.GetCustomFile<DialogueDBObjectScript>.GetScriptableObjectFile(path, "*.asset");

            DialogueDBObjectScript dialogueDBData = null;
            if (dialogueDBObjectScriptArray.Length != 0)
            {
                dialogueDBData = dialogueDBObjectScriptArray[0];
            }

            groupNames.Clear();

            if (dialogueDBData != null)
            {
                //Debug.Log("선택된 대화DB데이터" + dialogueDBData.name);
                List<string> tempList = new List<string>();
                for (int i = 0; i < dialogueDBData.dialogueDataArray.Length; i++)
                {
                    tempList.Add(dialogueDBData.dialogueDataArray[i].dialogueIDKey);
                }
                groupNames.AddRange(tempList);
                groupNames.Sort();
                groupNames.Insert(0, "-None-");
            }
            else
            {
                groupNames.Insert(0, "(DialogueDBData not in ResourceFolder)");
            }
            
            //여러그룹을 사용할떄 각각의 그룹에 대한걸 가지고 있기위한 기능을 가짐
            index = groupNames.IndexOf(property.stringValue);

            if (index == -1)
            {
                //에러발생시 처리                
                groupNames.Insert(0, property.stringValue);                
                index = groupNames.IndexOf(property.stringValue);
                index = EditorGUI.Popup(position, "DialogueID_Error", index, groupNames.ToArray());
                string groupErrorName = groupNames[index];

                //해당되는 프로퍼티에 대입
                property.stringValue = groupErrorName;
                return;
            }

            position.width -= 82;
            index = EditorGUI.Popup(position, "DialogueID", index, groupNames.ToArray());
            string groupName = groupNames[index];

            //해당되는 프로퍼티에 대입
            property.stringValue = groupName;
        }
    }
}
#endif
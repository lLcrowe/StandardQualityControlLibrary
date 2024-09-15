#if Doozy
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using lLCroweTool.QuestSystem;

namespace lLCroweTool.QC.EditorOnly
{
    [CustomPropertyDrawer(typeof(QuestBookGroupAttribute))]
    public class QuestBookGroupPropertyDrawer : PropertyDrawer
    {   
        private int index;
        private List<string> groupNames = new List<string>();
        private List<string> groupNameOrigins = new List<string>();



        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //가져오기
            //이걸로 가져올까 체크하기
            //DialogueDBObjectScript _dialogueDBData = (DialogueDBObjectScript)AssetDatabase.LoadAssetAtPath("Assets/Resources/" + fileName + ".asset", typeof(DialogueDBObjectScript));
            //QuestDBObjectScript questDBData = Resources.Load<QuestDBObjectScript>("ScrapNomadQuestDB");

            string path = "Assets/Resources";
            QuestDBObjectScript[] questDBObjectScriptArray = lLcroweUtilEditor.GetCustomFile<QuestDBObjectScript>.GetScriptableObjectFile(path, "*.asset");
            QuestDBObjectScript questDBData = null;
            if (questDBObjectScriptArray.Length != 0)
            {
                questDBData = questDBObjectScriptArray[0];
            }

            groupNames.Clear();
            groupNameOrigins.Clear();

            //그룹이름 세팅
            if (questDBData != null)
            {
                List<string> tempList = new List<string>();
                List<string> tempList2 = new List<string>();
                for (int i = 0; i < questDBData.questBookDataArray.Length; i++)
                {
                    tempList.Add(questDBData.questBookDataArray[i].questBookIDKey + "_제목:" + questDBData.questBookDataArray[i].questBookTitle);
                    tempList2.Add(questDBData.questBookDataArray[i].questBookIDKey);
                }
                groupNames.AddRange(tempList);
                groupNames.Sort();
                groupNames.Insert(0, "-None-");

                groupNameOrigins.AddRange(tempList2);
                groupNameOrigins.Sort();
                groupNameOrigins.Insert(0, "-None-");
            }
            else
            {
                groupNames.Insert(0, "(QuestDBData not in ResourceFolder)");
                groupNameOrigins.Insert(0, " (QuestDBData not in ResourceFolder)");
            }

            //여러그룹을 사용할떄 각각의 그룹에 대한걸 가지고 있기위한 기능을 가짐
            index = groupNameOrigins.IndexOf(property.stringValue);

            if (index == -1)
            {
                //에러발생시 처리
                groupNames.Insert(0, property.stringValue);
                groupNameOrigins.Insert(0, property.stringValue);
                index = groupNameOrigins.IndexOf(property.stringValue);
                index = EditorGUI.Popup(position, "QuestBookID_Error", index, groupNames.ToArray());
                string groupErrorName = groupNames[index];

                //해당되는 프로퍼티에 대입
                property.stringValue = groupErrorName.Split('_')[0];
                return;
            }

            position.width -= 82;
            
            //index = EditorGUI.Popup(position, label.text, index, groupNames.ToArray());
            index = EditorGUI.Popup(position, "QuestBookID", index, groupNames.ToArray());
            string groupName = groupNames[index];

            //해당되는 프로퍼티에 대입
            property.stringValue = groupName.Split('_')[0];
        }
    }
}
#endif
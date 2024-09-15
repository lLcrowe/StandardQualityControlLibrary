#if Localize
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


namespace lLCroweTool.QC.EditorOnly
{
    [CustomPropertyDrawer(typeof(LocalizeGroupAttribute))]
    public class LocalizeGroupPropertyDrawer : PropertyDrawer
    {
        private int index;
        private List<string> groupNames = new List<string>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var localizeDBData = LocalizingManager.Instance;
            groupNames.Clear();

            if (localizeDBData != null)
            {
                if (localizeDBData.localizeDBData != null)
                {
                    List<string> tempList = new List<string>();
                    foreach (var localizeData in localizeDBData.localizeDBData.localizeDataBible)
                    {
                        tempList.Add(localizeData.Value.string_ID);
                    }
                    groupNames.AddRange(tempList);
                    groupNames.Sort();
                    groupNames.Insert(0, "-None-");
                }
                else
                {
                    groupNames.Insert(0, "(LocalizingManager Data is Null)");
                }
            }
            else
            {
                groupNames.Insert(0, "(LocalizingManager not in Scene)");                
            }

            index = groupNames.IndexOf(property.stringValue);

            if (index == -1)
            {
                //에러발생시 처리                
                groupNames.Insert(0, property.stringValue);
                index = groupNames.IndexOf(property.stringValue);
                index = EditorGUI.Popup(position, "LocalizeID_Error", index, groupNames.ToArray());
                string groupErrorName = groupNames[index];

                //해당되는 프로퍼티에 대입
                property.stringValue = groupErrorName;
                return;
            }

            position.width -= 82;
            index = EditorGUI.Popup(position, "LocalizeID", index, groupNames.ToArray());
            string groupName = groupNames[index];

            //해당되는 프로퍼티에 대입
            property.stringValue = groupName;
        }
    }
}
#endif
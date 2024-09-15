using UnityEditor;
using UnityEngine;

namespace lLCroweTool.QC.EditorOnly
{
    //public abstract class CustomDataOpenWindowInspecterEditor<T1, T2> : Editor where T1 : ScriptableObject where T2 : CustomDataWindowEditor<ScriptableObject>
    //[CustomEditor(typeof(T1))]
    public abstract class CustomDataOpenWindowInspecterEditor<T1> : Editor where T1 : ScriptableObject
    {
        //인스팩터에디터에서 윈도우에디터를 열기위한 기능을 가짐
        //스크립터블데이터를 체크 
        private T1 data;
        private string contentName;
        private bool buttonUpPos;

        private void OnEnable()
        {
            data = (T1)target;
            contentName = typeof(T1).Name;
        }

        public sealed override void OnInspectorGUI()
        {
            SetButtonPos(ref buttonUpPos);

            if (buttonUpPos)
            {
                OpenDataWindow();
            }

            base.OnInspectorGUI();

            if (!buttonUpPos)
            {
                OpenDataWindow();
            }
        }

        private void OpenDataWindow()
        {
            if (GUILayout.Button($"{contentName}윈도우열기"))
            {
                //T2.targetData = data;
                ShowEditorWindow(data);
            }
        }

        public abstract void SetButtonPos(ref bool isUp);

        /// <summary>
        /// 에디터윈도우를 보여주는 함수(예시존재)
        /// </summary>
        /// <param name="targetData">타겟팅된 데이터</param>
        public abstract void ShowEditorWindow(T1 targetData);

        //예시        
        //public override void ShowEditorWindow(AbilityActionInfo targetData)
        //{   
        //    AbilityActionInfoEditor.SetLoadData(targetData);  
        //    AbilityActionInfoEditor.ShowWindow();//CustomDataWindowEditor를 상속받은 클래스의 ShowWindow
        //}
    }
}


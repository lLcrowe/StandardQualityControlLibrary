

#if UNITY_EDITOR && Doozy
using lLCroweTool.BuildingSystem.FuncStructure;
using System.Collections.Generic;
using UnityEditor;
#pragma warning disable 0618

namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(PipeStructureObject), true)]
    public class PipeStructureObjectInspectorEditor : Editor
    {

        private PipeStructureObject pipeStructureObject;

        private void OnEnable()
        {
            pipeStructureObject = (PipeStructureObject)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            int length = pipeStructureObject.GetPipeBridgeLength();
            string content = "";

            if (length == 0)
            {
                content = "파이프브릿지를 연결해주세요";
            }
            else
            {
                List<PipeBridgeStructureObject> tempList = pipeStructureObject.GetPipeBridgeList();
                for (int i = 0; i < tempList.Count; i++)
                {
                    content += (i + 1) + "번 파이프브릿지 연결상태 : 연결됨, 타겟 => " + tempList[i].name;
                    
                    if (i != length - 1)
                    {
                        content += "\n";
                    }
                }
            }
           
            EditorGUILayout.HelpBox(content, MessageType.Info);
        }
    }
}
#endif
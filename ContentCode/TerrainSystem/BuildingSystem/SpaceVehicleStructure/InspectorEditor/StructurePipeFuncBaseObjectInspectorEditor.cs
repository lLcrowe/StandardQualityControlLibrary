
#if UNITY_EDITOR && MEC
using UnityEngine;
using System.Collections.Generic;
using lLCroweTool.BuildingSystem.FuncStructure;
using lLCroweTool.DesireFuncSystem;

using UnityEditor;
#pragma warning disable 0618
namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(StructureFuncBaseObject), true)]
    public class StructurePipeFuncBaseObjectInspectorEditor : Editor
    {
        private StructureFuncBaseObject targetStructureFuncBaseObject;

        private void OnEnable()
        {
            targetStructureFuncBaseObject = (StructureFuncBaseObject)target;            
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            StructureFuncPipeBridgeInfo[] inputPipeBridgeArray = targetStructureFuncBaseObject.GetInputPipeBridgeArray();
            StructureFuncPipeBridgeInfo[] oututPipeBridgeArray = targetStructureFuncBaseObject.GetOutputPipeBridgeArray();

            ShowPipeBridgeInfo(inputPipeBridgeArray, "입력");
            ShowPipeBridgeInfo(oututPipeBridgeArray, "출력");

            //기능성건물 기초 관련
            if (ReferenceEquals(targetStructureFuncBaseObject.GetDesireSystem(), null))
            {
                if (GUILayout.Button("기능성건물 초기세팅"))
                {
                    targetStructureFuncBaseObject.SetDesireSystem(targetStructureFuncBaseObject.GetComponentInChildren<DesireSystem>());

                    if (ReferenceEquals(targetStructureFuncBaseObject.GetDesireSystem(), null))
                    {
                        GameObject gameObject = new GameObject();
                        gameObject.transform.parent = targetStructureFuncBaseObject.transform;
                        gameObject.name = "빌딩니드 모듈";
                        targetStructureFuncBaseObject.SetDesireSystem(gameObject.AddComponent<DesireSystem>());
                    }
                }
            }

            //인풋파이프브릿지만 존재하는 건축물
            if (!targetStructureFuncBaseObject.TryGetComponent(out StructureFuncTerminalObject terminalObject))
            {
                string content = "";
                //아웃풋 브릿지가 존재한지체크// 존재하면 없애라구 경고
                for (int i = 0; i < oututPipeBridgeArray.Length; i++)
                {
                    content += (i + 1) + "번 아웃풋 파이프브릿지 정보가 존재합니다. 인풋만 존재하는 단말건축물기능 컴포넌트입니다. 삭제해주세요.";

                    if (i != oututPipeBridgeArray.Length)
                    {
                        content += "\n";
                    }
                }
                if (oututPipeBridgeArray.Length != 0)
                {
                    EditorGUILayout.HelpBox(content, MessageType.Info);
                }
            }

            //제네레이터 관련//특정욕구카테고리 생성해주는 건물
            if (targetStructureFuncBaseObject.TryGetComponent(out GeneraterStuctureObject generaterStuctureObject))
            {
                
            }

            ClearPipeBridges();
        }


        private void ShowPipeBridgeInfo(StructureFuncPipeBridgeInfo[] targetInfoArray , string title)
        {
            //파이프상태 표시
            if (targetInfoArray.Length == 0)
            {
                EditorGUILayout.HelpBox( title + "파이프브릿지가 비어있습니다.\n 필요시 수동&추가버튼으로 세팅해주세요", MessageType.Info);
            }
            else
            {
                string content = "";
                for (int i = 0; i < targetInfoArray.Length; i++)
                {
                    if (targetInfoArray[i].GetPipeBridge() == null)
                    {
                        content += (i + 1) + "번 " + title + "파이프브릿지 연결상태 : 연결안됨, 타겟 => 없음." + " 압력량 : " + targetInfoArray[i].GetPressure();

                        if (GUILayout.Button((i + 1) + "번 " + title + "파이프브릿지 추가하기"))
                        {
                            PipeBridgeStructureObject pipeBridge = CreatePipeBridge(title, (i + 1) + "번");
                            pipeBridge.isBuildingFunBridge = true;
                        }
                    }
                    else
                    {
                        content += (i + 1) + "번 " + title + "파이프브릿지 연결상태 : 연결됨, 타겟 => " + targetInfoArray[i].GetPipeBridge().name + " 압력량 : " + targetInfoArray[i].GetPressure();

                        //여긴필요없을수 있음
                        PipeStructureObject[] pipeArray = targetInfoArray[i].GetPipeBridge().GetPipeArray();
                        for (int j = 0; j < pipeArray.Length; j++)
                        {
                            content += "    " + (j + 1) + "번 " + title + "파이프 연결상태 : 연결됨, 타겟 => " + pipeArray[j].name;

                        }
                    }

                    if (i != targetInfoArray.Length)
                    {
                        content += "\n";
                    }
                }
                EditorGUILayout.HelpBox(content, MessageType.Info);
            }
        }

        private PipeBridgeStructureObject CreatePipeBridge(string title, string content)
        {
            //새로제작
            GameObject gameObject = new GameObject();
            gameObject.transform.parent = targetStructureFuncBaseObject.transform;
            gameObject.name = title + " " + content + " PipeBridge";
            return gameObject.AddComponent<PipeBridgeStructureObject>();
        }

        private void ClearPipeBridges()
        {

            if (GUILayout.Button("파이브브릿지 연결 정리하기"))
            {
                //각각의 파이프브릿지를 가져와서 세팅해두기
                List<PipeBridgeStructureObject> pipeBridgeStructureList = new List<PipeBridgeStructureObject>();

                for (int i = 0; i < targetStructureFuncBaseObject.GetInputPipeBridgeArray().Length; i++)
                {
                    pipeBridgeStructureList.Add(targetStructureFuncBaseObject.GetInputPipeBridgeArray()[i].GetPipeBridge());
                }

                for (int i = 0; i < targetStructureFuncBaseObject.GetOutputPipeBridgeArray().Length; i++)
                {
                    pipeBridgeStructureList.Add(targetStructureFuncBaseObject.GetOutputPipeBridgeArray()[i].GetPipeBridge());
                }

                PipeBridgeStructureObject[] pipeBridgeStructures = targetStructureFuncBaseObject.GetComponentsInChildren<PipeBridgeStructureObject>();


                for (int i = 0; i < pipeBridgeStructures.Length; i++)
                {
                    if (!pipeBridgeStructureList.Contains(pipeBridgeStructures[i]))
                    {
                        DestroyImmediate(pipeBridgeStructures[i].gameObject);
                    }
                }
            }
        }
    }
}
#endif
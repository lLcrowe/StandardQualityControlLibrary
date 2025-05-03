#if MEC
using UnityEngine;
using System.Collections.Generic;
using MEC;
using lLCroweTool.AutoAnimation2D.SnakeBody;
using UnityEditor;

#pragma warning disable 0618

namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(SnakeBody))]
    [CanEditMultipleObjects]
    public class SnakeBodyInspectorEditor : Editor
    {
        private List<SnakeBody> snakeBodyList = new List<SnakeBody>();
        private static bool isOnlyThisParentChildFind = false;


        private void OnEnable()
        {
            snakeBodyList.Clear();
            for (int i = 0; i < targets.Length; i++)
            {
                snakeBodyList.Add(targets[i] as SnakeBody);
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            

            if (Application.isPlaying)
            {
                if (GUILayout.Button("삼킴 작동"))
                {
                    for (int i = 0; i < snakeBodyList.Count; i++)
                    {
                        SnakeBody targetSnakeBody = snakeBodyList[i];
                        Timing.RunCoroutine(SnakeBody.ActionVore(targetSnakeBody));
                    }
                }
                return;
            }

            string content = isOnlyThisParentChildFind ?"현재오브젝트 기준 자식오브젝트가져오기" :"모든자식오브젝트의 첫번째만 가져오기";
            EditorGUILayout.HelpBox(content, MessageType.Info);

            if (GUILayout.Button("Swap"))
            {
                isOnlyThisParentChildFind = !isOnlyThisParentChildFind;
            }

         

            if (GUILayout.Button("자식오브젝트들 가져와서 세팅하기"))
            {
                if (isOnlyThisParentChildFind)
                {
                    //현 뱀오브젝트 기준의 자식들만 가져오는 기능
                    for (int i = 0; i < snakeBodyList.Count; i++)
                    {
                        SnakeBody targetSnakeBody = snakeBodyList[i];
                        Transform[] transformArray = targetSnakeBody.GetComponentsInChildren<Transform>();
                        List<Transform> tempList = new List<Transform>();

                        for (int j = 0; j < transformArray.Length; j++)
                        {
                            if (transformArray[j] == null)
                            {
                                //비었으면 넘김
                            }
                            if (transformArray[j].parent == targetSnakeBody.transform)
                            {
                                tempList.Add(transformArray[j]);
                            }
                        }

                        targetSnakeBody.tailObjectArray = tempList.ToArray();
                    }
                }
                else
                {
                    //현 뱀오브젝트 기준의 자식들에서 첫번쨰만 가져오는 기능
                    for (int i = 0; i < snakeBodyList.Count; i++)
                    {
                        SnakeBody targetSnakeBody = snakeBodyList[i];
                        Transform[] transformArray = targetSnakeBody.GetComponentsInChildren<Transform>();
                        List<Transform> tempList = new List<Transform>();
                        Transform targetParent = targetSnakeBody.transform;//기준이 된 부모

                        for (int j = 0; j < transformArray.Length; j++)
                        {
                            if (targetParent.childCount != 0)
                            {
                                if (transformArray[j] == targetParent.GetChild(0))
                                {
                                    //첫번쨰 자식이 맞은가
                                    targetParent = transformArray[j];
                                    tempList.Add(transformArray[j]);
                                }
                            }
                        }
                        targetSnakeBody.tailObjectArray = tempList.ToArray();
                    }
                }
            }
        }
    }
}
#endif
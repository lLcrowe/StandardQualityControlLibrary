using System.Collections.Generic;
using UnityEngine;

namespace lLCroweTool.NodeMapSystem
{
    public class CustomNodeMap : MonoBehaviour
    {
        public List<CustomNode> childCustomNodeList = new List<CustomNode>();


        public CustomNode startNode;
        public CustomNode endNode;


        private static List<CustomNode> checkList = new List<CustomNode>(1000);//한번씩 확인한 대상들
        private static Queue<CustomNode> queue = new Queue<CustomNode>(1000);//확인할 대상들

        public void Action()
        {
            for (int i = 0; i < childCustomNodeList.Count; i++)
            {
                childCustomNodeList[i].SetNode(Color.white);
            }
            FloodFill(startNode, endNode, Color.red, 20);
        }
     
        //이름변경
        private static void FloodFill(CustomNode startPos, CustomNode endPos, Color newColor, int checkMaxNodeCount)
        {
            //시작위치가 갈수 있는 위치냐 체크
            if (!startPos.isCanVisitNode)
            {
                return;
            }

            checkList.Clear();//한번씩 확인한 대상들 초기화
            queue.Clear();//확인할 대상들 초기화
            bool isRoom = true;//방인지 체크여부
            int nodeCount = 0;//계산한 타일수

            //첫대상을 집어넣고 작동시킴
            queue.Enqueue(startPos);
            startPos.SetNode(newColor);

            do
            {
                //지형이 존재하는지 체크
                CustomNode targetNodePos = queue.Dequeue();

                if (checkList.Contains(targetNodePos))
                {
                    continue;
                }
                nodeCount++;
                checkList.Add(targetNodePos);//체크리스트에 등록

                //주변노드 체크                
                CustomNode[] tempPosArray = targetNodePos.GetConnectCustomNode();

                for (int i = 0; i < tempPosArray.Length; i++)
                {
                    //해당노드가 갈수있는 구역인지
                    if (!tempPosArray[i].isCanVisitNode)
                    {
                        //못가니 앞으로 체크안할곳이므로 체크리스트에 등록
                        if (!checkList.Contains(tempPosArray[i]))
                        {
                            checkList.Add(tempPosArray[i]);
                        }
                        //넘어가기
                        continue;
                    }

                    if (checkList.Contains(tempPosArray[i]))
                    {
                        //체크리스트에 등록되있으므로 더 체크할 필요없음
                        continue;
                    }

                    //만약 갈려는곳이면 정지하고 
                    if (tempPosArray[i] == endPos)
                    {
                        //갈려는곳이 맞다
                        endPos.SetNode(Color.blue);

                        break;
                    }

                    //갈수있고 체크한적이 없으면 큐에 추가하고 해당색깔을 변경
                    queue.Enqueue(tempPosArray[i]);
                    tempPosArray[i].SetNode(newColor);
                }

                //체크할 타일수량보다 더크면 멈춤
                if (nodeCount > checkMaxNodeCount)
                {
                    //방이 아니다 이말이야
                    isRoom = false;
                    break;
                }
            } while (queue.Count > 0);

            Debug.Log($"방 체크여부 => {isRoom }, 몇개를 체크했는가 => {nodeCount}");
        }






    }
}
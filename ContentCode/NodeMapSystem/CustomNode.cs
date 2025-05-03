using System.Collections.Generic;
using UnityEngine;

namespace lLCroweTool.NodeMapSystem
{
    public class CustomNode : MonoBehaviour
    {
        public bool isCanVisitNode = true;//갈수 있는지 여부
        public List<CustomNode> connectCustomNodeList = new List<CustomNode>();//연결된 노드들

        public SpriteRenderer sr;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
        }

        public void SetNode(Color newColor)
        {
            Color color = sr.color;
            color = newColor;
            sr.color = color;
        }


        /// <summary>
        /// 연결된 노드들을 가져오는 함수
        /// </summary>
        /// <returns>연결된 노드들</returns>
        public CustomNode[] GetConnectCustomNode()
        {
            return connectCustomNodeList.ToArray();
        }
    }
}
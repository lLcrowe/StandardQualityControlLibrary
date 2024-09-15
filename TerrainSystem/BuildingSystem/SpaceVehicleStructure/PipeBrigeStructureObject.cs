#if MEC

using lLCroweTool.DestroyManger;
using System.Collections.Generic;
using UnityEngine;

namespace lLCroweTool.BuildingSystem.FuncStructure
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class PipeBridgeStructureObject : MonoBehaviour
    {
        //파이프와 파이프를 연결시키는 건축물오브젝트
        //건축물월드오브젝트에 붙착시켜서 작동됨
        //원형콜라이더트리거 사용

        //파이프사용해서 작동하는 기능오브젝트는 브릿지를 사용함
        //기능형건물일시 브릿지는1개로 설정

        public bool isBuildingFunBridge = false;//건물기능용 브릿지이면 해체못시킴

        //브릿지 2개이상의 연결
        [Header("연결할수 있는 파이프갯수")]
        public int maxBrigeCount = 2;
        private int actionIndex = 0;
        [SerializeField] private List<PipeStructureObject> pipeList = new List<PipeStructureObject>();

        protected CoroutineTimerModule timerModule;
        protected virtual void Awake()
        {
            pipeList.Capacity = maxBrigeCount;

            timerModule = GetComponent<CoroutineTimerModule>();
            timerModule.SetTimer(0.5f);
            //timerModule.AddUnityEvent(UpdatePipe);
            timerModule.AddUnityEvent(delegate { PipeObjectManager.Instance.UpdatePipeBridge(this); });

        }

        public void ConnectPipe(PipeStructureObject pipeStructureObject)
        {
            if (!pipeList.Contains(pipeStructureObject))
            {
                if (pipeList.Count <= maxBrigeCount)
                {
                    pipeStructureObject.ConnectPipeBridge(this);//해당파이프에 브릿지연결
                    pipeList.Add(pipeStructureObject);                    
                }
            }
        }
        public void DeConnectPipe(PipeStructureObject pipeStructureObject)
        {
            if (pipeList.Contains(pipeStructureObject))
            {
                pipeStructureObject.DeConnectPipeBridge(this);
                pipeList.Remove(pipeStructureObject);
            }
        }

        /// <summary>
        /// 모든 연결된 파이프와의 연결해제
        /// </summary>
        public void AllDeconnectPipe()
        {
            for (int i = 0; i < pipeList.Count; i++)
            {
                pipeList[i].DeConnectPipeBridge(this);
                DestroyManager.Instance.AddDestoryGameObject(pipeList[i].gameObject);
            }
            pipeList.Clear();
        }
        
        //파이프오브젝트에서 있던 함수
        public bool GetPipe(ref PipeStructureObject _targetPipe)
        {
            if (pipeList.Count == 0)
            {
                return false;
            }

            _targetPipe = pipeList[actionIndex];
            actionIndex = actionIndex >= pipeList.Count ? 0 : actionIndex++;
            return true;
        }

        public PipeStructureObject GetPipe()
        {
            PipeStructureObject _targetPipe = pipeList[actionIndex];
            actionIndex = actionIndex >= pipeList.Count ? 0 : actionIndex++;
            return _targetPipe;
        }

        public void SetActionIndex(int _actionIndex)
        {
            actionIndex = _actionIndex;
        }
        public int GetActionIndex()
        {
            return actionIndex;
        }

        /// <summary>
        /// 연결된 파이프갯수를 가져오는 함수
        /// </summary>
        /// <returns>연결된 파이프 갯수</returns>
        public int GetPipeLength()
        {
            return pipeList.Count;
        }

        public PipeStructureObject[] GetPipeArray()
        {
            return pipeList.ToArray();
        }
    }
}
#endif
#if MEC

using lLCroweTool.DesireFuncSystem;
using lLCroweTool.TimerSystem;
using lLCroweTool.WorldObjectSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lLCroweTool.BuildingSystem.FuncStructure
{
    [System.Serializable]
    public class StructureFuncPipeBridgeInfo
    {
        public PipeBridgeStructureObject pipeBridge;//파이프브릿지
        public int pressure;//사용하는 압력

        public PipeBridgeStructureObject GetPipeBridge()
        {
            return pipeBridge;
        }
        public int GetPressure()
        {
            return pressure;
        }
    }
    [RequireComponent(typeof(UpdateTimerModule))]
    public abstract class StructureFuncBaseObject : MonoBehaviour
    {
        //부속품으로 변경
        //컴포넌트로 추가 사용
        //특정욕구를 사용하는 기능형 건축물오브젝트 기초 역할로 인풋 아웃풋 파이프연결이 포함되있음

        //각각의 인아웃파이프스트럭쳐를 게임시작전에 세팅후
        //인게임에서는 파이프끼리 연결할수 있게 해줘야함

        //상속받은 오브젝트에서 사용하는 파이프Ref 변수//Get할때 사용
        //특정욕구를 사용하는 기능형건축물 기초 역할로
        //인풋아웃풋 파이프를 상속받아서 업데이트를 이용하여 특정한 무언가를 하는 역할이 추가됨
        //현 클래스는 각각의 구조물의 기초기능을 가진다.
        //업데이트를 
        //사용하는 구조물의 설명및 능력에 따라 함수들을 변경가능하다
        

        [Header("인풋 파이프설정")]
        [SerializeField] private StructureFuncPipeBridgeInfo[] inputPipeBridgeInfoArray = new StructureFuncPipeBridgeInfo[0];//입력할 파이프브릿지
        private int inputActionIndex = 0;

        [Space]
        [Header("아웃풋 파이프설정")]        
        [SerializeField] private StructureFuncPipeBridgeInfo[] outputPipeBridgeInfoArray = new StructureFuncPipeBridgeInfo[0];//출력할 파이프브릿지        
        private int outputActionIndex = 0;

        protected StructureFuncPipeBridgeInfo refPipeBridgeInfo = null;
        protected PipeStructureObject refPipe = null;

        [Space]
        [Header("기능 작동관련")]
        [SerializeField] private bool isActive = false;//작동 상태인가?//업데이트와 별개
        /// <summary>
        /// 상속받은 클래스에서 시작시 설정함
        /// </summary>
        [SerializeField] protected bool isUseFuncUpdate = false;//업데이트사용여부

        //욕구니드시스템을 집어넣자
        //욕구에 전기, 연료 등등을 집어넣어 제작
        [SerializeField] private DesireSystem structureDesireSystem;//욕구시스템

        [Space]
        [Header("기능을 가진 건축물 월드오브젝트(자동세팅)")]
        [SerializeField] protected bool isExistStructureObject;
        [SerializeField] protected TestWorldStructureObject structureObject;

        private UpdateTimerModule timerModule;

        protected virtual void Awake()
        {
            AwakeInitStructure();
        }

        protected virtual void AwakeInitStructure()
        {   
            timerModule = GetComponent<UpdateTimerModule>();
            SetStructureObject(GetComponent<TestWorldStructureObject>());
            timerModule.AddUnityEvent(UpdateStructureFunc);

            for (int i = 0; i < inputPipeBridgeInfoArray.Length; i++)
            {
                inputPipeBridgeInfoArray[i].GetPipeBridge().isBuildingFunBridge = true;
            }

            for (int i = 0; i < outputPipeBridgeInfoArray.Length; i++)
            {
                outputPipeBridgeInfoArray[i].GetPipeBridge().isBuildingFunBridge = true;
            }
        }

        /// <summary>
        /// 타임모듈에 돌아가는 업데이트함수
        /// </summary>
        private void UpdateStructureFunc()
        {
            if (!isUseFuncUpdate)
            {
                return;
            }

            //if (CheckSatisfyDesire() == DesireKategorie.Nothing)
            //{
            //    UpdateStructureAction();
            //}
        }

        /// <summary>
        /// 상태가 만족하면 기능형건축물의 업데이트 작동하는 함수
        /// </summary>
        protected abstract void UpdateStructureAction();

        public void SetStructureObject(TestWorldStructureObject worldStructureObject)
        {
            structureObject = worldStructureObject;
            if (ReferenceEquals(structureObject, null))
            {
                isExistStructureObject = false;
            }
            else
            {
                isExistStructureObject = true;
            }
        }

        /// <summary>
        /// 건축물활성화가 되고 있는가 여부 체크함수
        /// </summary>
        /// <returns>활성화 여부</returns>
        public bool GetIsActive() 
        {
            return isActive; 
        }

        /// <summary>
        /// 건축물활성화 세팅함수
        /// </summary>
        /// <param name="onoff">활성화 값</param>
        public void SetIsActive(bool onoff)
        {
            if (isActive == onoff)
            {
                return;
            }

            isActive = onoff;

            if (isActive)
            {
                //활성화
                structureDesireSystem.enabled = true;
            }
            else
            {
                //비활성화
                structureDesireSystem.enabled = false;
            }
        }

        public bool GetIsUseUpdate()
        {
            return isUseFuncUpdate;
        }

        /// <summary>
        /// 현재 욕구치가 충족되는 지 체크해주는 함수. 업데이트 함수에서 기능작동전 사용함
        /// </summary>
        /// <returns>결핍된 욕구</returns>
        //private DesireKategorie CheckSatisfyDesire()
        //{
        //    bool isDone = false;
        //    DesireKategorie tempDesireKategorie = DesireKategorie.Nothing;
        //    structureDesireSystem.GetDesireDepletion(ref tempDesireKategorie);
        //    if (tempDesireKategorie == DesireKategorie.Nothing)
        //    {
        //        isDone = true;
        //    }
        //    SetIsActive(isDone);
        //    return tempDesireKategorie;
        //}

        /// <summary>
        /// 인풋파이프브릿지를 가져오는 함수//상속받은 refPipe 변수로 받을것
        /// </summary>
        /// <param name="_targetPipeBridge">refPipe변수</param>
        /// <returns>인풋파이프브릿지 존재여부</returns>
        public bool GetInputPipeBridgeInfo(ref StructureFuncPipeBridgeInfo _targetPipeBridge)
        {
            if (inputPipeBridgeInfoArray.Length == 0)
            {
                return false;
            }

            _targetPipeBridge = inputPipeBridgeInfoArray[inputActionIndex];
            inputActionIndex = inputActionIndex >= inputPipeBridgeInfoArray.Length ? 0 : inputActionIndex++;
            return true;
        }

        /// <summary>
        /// 아웃풋파이프브릿지를 가져오는 함수//상속받은 refPipe변수로 받을것
        /// </summary>
        /// <param name="_targetPipeBridge">refPipe변수</param>
        /// <returns>아웃풋파이프브릿지 존재여부</returns>
        public bool GetOutputPipeBridgeInfo(ref StructureFuncPipeBridgeInfo _targetPipeBridge)
        {
            if (outputPipeBridgeInfoArray.Length == 0)
            {
                return false;
            }

            _targetPipeBridge = outputPipeBridgeInfoArray[outputActionIndex];
            outputActionIndex = outputActionIndex >= outputPipeBridgeInfoArray.Length ? 0 : outputActionIndex++;
            return true;
        }


        //에디터에서만 사용됨
        public StructureFuncPipeBridgeInfo[] GetInputPipeBridgeArray()
        {
            return inputPipeBridgeInfoArray;
        }
        public StructureFuncPipeBridgeInfo[] GetOutputPipeBridgeArray()
        {
            return outputPipeBridgeInfoArray;
        }

        public void SetDesireSystem(DesireSystem _desireSystem)
        {
            structureDesireSystem = _desireSystem;
        }

        public DesireSystem GetDesireSystem()
        {
            return structureDesireSystem;
        }
    }
}

#endif
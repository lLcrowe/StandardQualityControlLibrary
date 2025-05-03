#if Doozy
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using lLCroweTool.TimerSystem;
using lLCroweTool.DesireFuncSystem;

namespace lLCroweTool.BuildingSystem.FuncStructure
{
    [RequireComponent(typeof(CoroutineTimerModule))]//건설할때 탐지용
    [RequireComponent(typeof(CircleCollider2D))]//건설할때 탐지용
    public class PipeStructureObject : MonoBehaviour
    {
        //모듈
        //파이프 건축물 오브젝트
        //특정 욕구들을 움직이게 해주는 역할        
        [Header("파이프끼리의 연결할수 있는 거리")]
        [SerializeField] private float pipeConnectMaxDistince = 5;//파이프를 연결할수 있는 최대길이//건설할때 사용    
        [Header("파이프끼리 통하는 용량")]
        [SerializeField] private int pipePressure;//파이프끼리 변경할수있는 최대량//현파이프 위치에서 다른방향 파이프까지의 거리
        
        [SerializeField] private DesireKategorie desireKategorie;//현재파이프가 가지는 욕구타입//없으면 다시 채울수 있음
        [Header("최대치 용량")]
        [SerializeField] private int maxValue = 1000;//파이프가 최대로 가질 용량
        [Header("현재치 용량")]
        [SerializeField] private int curValue;//현재 파이프의 용량//유압//전압//압력//파워//변경할때 필요한값
        
        public Line pipeLine;//파이프 랜더러//나중에 쉐이프의 라인랜더러로 처리해도 괜찮아보임//20220305
        private bool isChange = false;//변경됫으면 컬러변경

        //2개만
        [SerializeField] private List<PipeBridgeStructureObject> pipeBridgeList = new List<PipeBridgeStructureObject>(2);
       

        protected virtual void Awake()
        {   
            //색초기화
            if (curValue % 5 == 0)
            {
                //알파값변환
                float colorAlphaValue = (float)(curValue * 1 / (float)maxValue);
                //colorAlphaValue = (float)curOxygenValue * (float)maxOxygenValue * 0.01f;
                ChangeSpriteOpasity(colorAlphaValue);
            }

            CircleCollider2D collider2D = gameObject.AddComponent<CircleCollider2D>();
            collider2D.isTrigger = true;
            gameObject.layer = LayerMask.NameToLayer("PipeLayer");
        }

        /// <summary>
        /// 파이프끼리 교환하거나 제네레이터에서 집어넣을때사용하는 함수
        /// </summary>
        /// <param name="value">집어넣을 값</param>
        /// <param name="desireKategorie">집어넣을 욕구카테고리</param>
        public void AddDesireValueData(ref int value, DesireKategorie _desireKategorie)
        {
            //최대치 체크
            if (maxValue < value + curValue)
            {
                value = (value + curValue) - maxValue;//남는 더해줄값 설정
                curValue = maxValue;//현재값 수정
            }
            else
            {
                curValue += value;//현재값수정
                value = 0;//남는 더해줄값 설정
            }

            //사용하기전에 미리 욕구값을 확인하고 세팅함
            //0부터시작할 Nothing이나 추가할때 카테고리까지 체크하여 작동
            if (desireKategorie != _desireKategorie)
            {
                desireKategorie = _desireKategorie;
            }

            isChange = true;
        }

        /// <summary>
        /// 오직 제네레이터에서 작동시키는 함수. 파이프는 사용안함
        /// </summary>
        /// <param name="value">없애야할 값</param>        
        public void RemoveDesireValueData(ref int value)
        {
            //최소치체크
            if (0 > curValue - value)
            {
                value -= curValue;//남는 없앨값 설정
                curValue = 0;//현재값 수정
                desireKategorie = DesireKategorie.Nothing;
            }
            else
            {
                curValue -= value;//현재값 수정
                value = 0;//남는 없앨값 설정
            }

            isChange = true;
        }

        /// <summary>
        /// 파이프브릿지와 연결시키는 함수
        /// </summary>
        /// <param name="pipeBridge">파이프브릿지</param>
        public void ConnectPipeBridge(PipeBridgeStructureObject pipeBridge)
        {
            if (!pipeBridgeList.Contains(pipeBridge))
            {
                if (pipeBridgeList.Count <= 2)
                {                    
                    pipeBridgeList.Add(pipeBridge);
                }
            }
        }

        /// <summary>
        /// 파이프브릿지와 연결해제시키는 함수
        /// </summary>
        /// <param name="pipeBridge">파이프브릿지</param>
        public void DeConnectPipeBridge(PipeBridgeStructureObject pipeBridge)
        {
            if (pipeBridgeList.Contains(pipeBridge))
            {
                pipeBridgeList.Remove(pipeBridge);
            }
        }

        /// <summary>
        /// 연결된 모든 파이프브릿지와의 연결해체
        /// </summary>
        public void AllDeconnectPipeBridge()
        {
            for (int i = 0; i < pipeBridgeList.Count; i++)
            {
                pipeBridgeList[i].DeConnectPipe(this);
            }
            pipeBridgeList.Clear();
        }

        /// <summary>
        /// 연결된 파이프브릿지개수 가져오는 함수
        /// </summary>
        /// <returns>연결된 파이프브릿지개수</returns>
        public int GetPipeBridgeLength()
        {
            return pipeBridgeList.Count;
        }

        public List<PipeBridgeStructureObject> GetPipeBridgeList()
        {
            return pipeBridgeList;
        }


        //파이프스트럭쳐에 있는 욕구량을 보여줄 이미지의 알파값
        private void ChangeSpriteOpasity(float _alphaValue)
        {
            Color color = pipeLine.Color;
            color.a = _alphaValue;
            pipeLine.Color = color;
        }
        public void ChangePipeColor()
        {
            if (!isChange)
            {
                return;
            }
            if (curValue % 2 == 0)
            {
                //알파값변환
                float colorAlphaValue = (float)(curValue * 1 / (float)maxValue);
                //colorAlphaValue = (float)curOxygenValue * (float)maxOxygenValue * 0.01f;
                ChangeSpriteOpasity(colorAlphaValue);
            }
        }



        /// <summary>
        /// 현 파이프의 수량가져오기
        /// </summary>
        /// <returns></returns>
        public int GetCurValue()
        {
            return curValue;
        }

        public void SetCurValue(int value)
        {
            curValue = value;
        }

        public bool GetIsChange()
        {
            return isChange;
        }

        /// <summary>
        /// 현파이프에 탑재된 욕구카테고리 가져오기
        /// </summary>
        /// <returns></returns>
        public DesireKategorie GetDesireKategorie()
        {
            return desireKategorie;
        }

        public void SetDesireKategorie(DesireKategorie _desireKategorie)
        {
            desireKategorie = _desireKategorie;
        }

        public int GetPipePressure()
        {
            return pipePressure;
        }

        public void SetPipePressure(int _pipePressure)
        {
            pipePressure = _pipePressure;
        }

        public float GetPipeConnectMaxDistince()
        {
            return pipeConnectMaxDistince;
        }
    }
}
#endif
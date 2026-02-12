#if Doozy

using lLCroweTool.DesireFuncSystem;
using System.Collections;
using UnityEngine;

namespace lLCroweTool.BuildingSystem.FuncStructure
{
    public abstract class StructureFuncTerminalObject : StructureFuncBaseObject
    {
        //말단(단말) 기능성건축물 오브젝트
        //아웃풋 파이프는 존재하지않고
        //인풋파이프에서 필요한 욕구가 만족하면 특정행동이 작동되게 하는 함수
        //아웃풋이 존재하면 현 클래스를 상속받을 필요없음

        [Space]
        [Header("소모되는 욕구설정")]
        public DesireKategorie consumptionDesireKategorie;//소모할 욕구타입
        public int consumptionValue;//소모하는 연료량//인풋 압력으로 계산됨

        private bool isSatisfaction;//필요한 욕구가 다모여서 만족됫는지 체크


        /// <summary>
        /// 만족했을시 기능성단말 건축물의 작동 함수
        /// 완료되야지만 욕구가 소모됨
        /// </summary>
        /// <returns>특정작업이 완료된 여부</returns>
        protected abstract bool UpdateSatisfactionStructureFuncTerminal();

        protected override void UpdateStructureAction()
        {
            //제네레이터는 연료를 소모하여 특정한 욕구를 제작한다.
            //연결된 건축물들에서 필요한 욕구종류와 제작하는 욕구 종류를 비교후 교환해준다.

            //먼저 제네레이터가 작동되는지 여부 체크 
            //활성화됫으면
            if (GetIsActive())
            {
                //만족됫는지 체크
                if (isSatisfaction || consumptionDesireKategorie == DesireKategorie.Nothing)
                {
                    //만족됨
                    //터미널기능을 작동시킨후 작동이 되면 욕구를 소모하게 만듬
                    isSatisfaction = !UpdateSatisfactionStructureFuncTerminal();
                }
                else
                {
                    //만족안되있음

                    //인풋 존재하는지 체크
                    if (GetInputPipeBridgeInfo(ref refPipeBridgeInfo))
                    {
                        if (refPipeBridgeInfo.GetPipeBridge().GetPipe(ref refPipe))
                        {
                            //같은 욕구인지 체크
                            if (refPipe.GetDesireKategorie() == consumptionDesireKategorie)
                            {
                                //인풋 파이프내용물이 필요한 욕구수량만큼 존재하는지 체크
                                int tempConsumptionValue = consumptionValue;

                                //해당파이프의 내용물이 필요한 욕구수량보다 많거나 같으면
                                if (refPipe.GetCurValue() >= tempConsumptionValue)
                                {
                                    //소모시키고 
                                    refPipe.RemoveDesireValueData(ref tempConsumptionValue);
                                    Debug.Log(tempConsumptionValue, gameObject);//0이 나와야됨

                                    //만족
                                    isSatisfaction = true;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
#endif
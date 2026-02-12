
#if Doozy
using lLCroweTool.DesireFuncSystem;
using UnityEngine;
namespace lLCroweTool.BuildingSystem.FuncStructure
{
    public class GeneraterStuctureObject : StructureFuncBaseObject
    {
        //빌딩베이스를 상속받아 기초적인
        //각각시설물의 능력이 발동되도록 작동
        //생산건물 같은경우
        //소비할 욕구 <= 인풋에서 받아서 작동
        //생산할 욕구 <= 아웃풋으로 보냄
        //욕구변환기라고 생각하면 됨

        //제네레이터
        //해당되는 욕구를 생산을 하며 특정욕구연료를 사용함
        [Header("제작되는 욕구설정")]
        public DesireKategorie generaterDesireKategorie;//제작할 욕구타입
        public int generaterValue;//생산하는 량//동력//아웃풋 압력으로 계산됨

        [Space]
        [Header("소모되는 욕구설정")]
        public DesireKategorie consumptionDesireKategorie;//소모할 욕구타입
        public int consumptionValue;//소모하는 연료량//인풋 압력으로 계산됨

        protected bool isGenerater = false;//생산됫는지 체크
        protected int tempGeneraterValue = 0;//생산된 값을 부산물

        //함선이나 전초기지에서 관여함
        //빌딩의 행동
        protected override void AwakeInitStructure()
        {
            base.AwakeInitStructure();
            isUseFuncUpdate = true;
        }

        protected override void UpdateStructureAction()
        {
            //제네레이터는 연료를 소모하여 특정한 욕구를 제작한다.
            //연결된 건축물들에서 필요한 욕구종류와 제작하는 욕구 종류를 비교후 교환해준다.

            //먼저 제네레이터가 작동되는지 여부 체크 
            //활성화됫으면
            if (GetIsActive())
            {
                //생산됫는지 체크
                if (isGenerater)
                {
                    //생산됨
                    if (generaterValue <= 0)
                    {
                        return;
                    }

                    //아웃풋 존재하는지 체크
                    if (GetOutputPipeBridgeInfo(ref refPipeBridgeInfo))
                    {
                        if (refPipeBridgeInfo.GetPipeBridge().GetPipe(ref refPipe))
                        {
                            //같은 욕구인지 체크
                            if (refPipe.GetDesireKategorie() == generaterDesireKategorie || refPipe.GetDesireKategorie() == DesireKategorie.Nothing)
                            {
                                //부산물이 남아있는지 체크
                                if (tempGeneraterValue > 0)
                                {
                                    //남아있으면 그걸로 아웃풋파이프에 집어넣기

                                    //초기 계산
                                    int temp = tempGeneraterValue;
                                    refPipe.AddDesireValueData(ref temp, generaterDesireKategorie);

                                    //집어넣고 남은 값 최종계산
                                    tempGeneraterValue += temp;

                                    //체크
                                    if (tempGeneraterValue <= 0)
                                    {
                                        isGenerater = false;
                                        tempGeneraterValue = 0;
                                    }
                                }
                                //작동되는지 체크후 없앰
                                //else
                                //{
                                //    //남아있지않으면 새로 생성
                                //    tempGeneraterValue = generaterValue;
                                //}
                            }
                        }
                    }
                }
                else
                {
                    //생산안되있음

                    //필요한 압력이 0//필요한 요구치X 인지 체크
                    if (consumptionValue <= 0)
                    {
                        //생산
                        isGenerater = true;
                        tempGeneraterValue = generaterValue;
                    }
                    else if (GetInputPipeBridgeInfo(ref refPipeBridgeInfo))
                    {
                        if (refPipeBridgeInfo.GetPipeBridge().GetPipe(ref refPipe))
                        {
                            //인풋 존재하는지 체크
                            //같은 욕구인지 체크
                            if (refPipe.GetDesireKategorie() == consumptionDesireKategorie)
                            {
                                //특정한 압력만큼 해당 파이프의 값을 여기에 집어넣는다.
                                int tempConsumptionValue = consumptionValue;

                                //해당파이프의 내용물이 압력값보다 많거나 같으면
                                if (refPipe.GetCurValue() >= tempConsumptionValue)
                                {
                                    //소모시키고 
                                    refPipe.RemoveDesireValueData(ref tempConsumptionValue);
                                    Debug.Log(tempConsumptionValue, gameObject);//0이 나와야됨//잘나옴

                                    //생산
                                    isGenerater = true;
                                    tempGeneraterValue = generaterValue;
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
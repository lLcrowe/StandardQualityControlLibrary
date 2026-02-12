#if MEC
using UnityEngine;
using System.Collections;
using lLCroweTool.DesireFuncSystem;

namespace lLCroweTool.BuildingSystem.FuncStructure
{
    public class FuelTankStuctureObject : StructureFuncBaseObject
    {
        //연료탱크
        //연료를 채우는곳
        //보관 및 소모만 가능//채울때는 제네레이터에서는 안채움        
        [SerializeField] private DesireKategorie storageDesireCATType;//보관할 욕구타입
        [SerializeField] private int maxFualValue;
        [SerializeField] private int curFualValue;

        

        //터졋을때 이벤트로 현재 연료량만큼 폭발크기와 대미지를 선정해주기


        protected override void AwakeInitStructure()
        {
            base.AwakeInitStructure();
            
        }

        protected override void UpdateStructureAction()
        {
            //UpdateFualTank;
            //인풋 존재하는지 체크
            if (GetInputPipeBridgeInfo(ref refPipeBridgeInfo))
            {
                //같은 욕구인지 체크

                if (refPipeBridgeInfo.GetPipeBridge().GetPipe(ref refPipe))
                {
                    if (refPipe.GetDesireKategorie() == storageDesireCATType)
                    {
                        //특정한 압력만큼 해당 파이프의 값을 여기에 집어넣는다.
                        int pressurePower = refPipeBridgeInfo.GetPressure();

                        //초기 계산
                        curFualValue += pressurePower;
                        refPipe.RemoveDesireValueData(ref pressurePower);

                        //집어넣고 남은 값 최종계산
                        curFualValue -= pressurePower;
                    }
                }
            }

            //아웃풋 존재하는지 체크
            if (GetOutputPipeBridgeInfo(ref refPipeBridgeInfo))
            {
                if (refPipeBridgeInfo.GetPipeBridge().GetPipe(ref refPipe))
                {

                    //같은 욕구인지 체크
                    if (refPipe.GetDesireKategorie() == storageDesireCATType || refPipe.GetDesireKategorie() == DesireKategorie.Nothing)
                    {
                        //특정한 압력만큼 해당 파이프에 값을 집어넣는다.
                        int pressurePower = refPipeBridgeInfo.GetPressure();

                        //초기 계산
                        curFualValue -= pressurePower;
                        refPipe.AddDesireValueData(ref pressurePower, storageDesireCATType);


                        //집어넣고 남은 값 최종계산
                        curFualValue += pressurePower;
                    }
                }
            }
        }

        /// <summary>
        /// 연료추가 함수
        /// </summary>
        /// <param name="addValue">추가 값</param>
        /// <returns>추가하고 남은 연료값</returns>
        public int AddFual(int addValue)
        {
            int temp = 0;
            curFualValue += addValue;
            if (curFualValue > maxFualValue)
            {
                temp = curFualValue - maxFualValue;
            }
            CheckLimitValue();
            return temp;
        }

        /// <summary>
        /// 연료소모 함수
        /// </summary>
        /// <param name="removeValue">소모 값</param>
        public void RemoveFual(int removeValue)
        {
            curFualValue -= removeValue;
            CheckLimitValue();
        }

        /// <summary>
        /// 일정한 량의 연료가 존재하는지 확인하는 함수
        /// </summary>
        /// <param name="needValue">필요한 량</param>
        /// <returns>존재 여부</returns>
        public bool CheckExistMinFuel(int needValue)
        {
            bool isExist = false;
            if (curFualValue - needValue > 0)
            {
                isExist = true;
            }
            return isExist;
        }

        /// <summary>
        /// 일정한 량의 연료를 주입할수 있는지 체크하는 함수
        /// </summary>
        /// <param name="addValue">집어넣을 량</param>
        /// <returns>가능한지 여부</returns>
        public bool CheckExistMaxFuel(int addValue)
        {
            bool isExist = false;
            if (maxFualValue >= curFualValue + addValue)
            {
                isExist = true;
            }
            return isExist;
        }

        /// <summary>
        /// 연료량의 제한량 체크 함수
        /// </summary>
        private void CheckLimitValue()
        {
            if (curFualValue < 0)
            {
                curFualValue = 0;
            }
            else if (curFualValue > maxFualValue)
            {
                curFualValue = maxFualValue;
            }
        }

        //비교용

        /// <summary>
        /// 연료탱크가 저장할 욕구카테고리를 가져오는 함수
        /// </summary>
        /// <returns>욕구카테고리</returns>
        public DesireKategorie GetStorageDesireCATType()
        {
            return storageDesireCATType;
        }
    }
}
#endif

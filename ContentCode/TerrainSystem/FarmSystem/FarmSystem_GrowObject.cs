#if MEC

using lLCroweTool.DesireFuncSystem;
using lLCroweTool.InventorySystem;
using lLCroweTool.StatusModuleSystem;
using lLCroweTool.TimerSystem;
using lLCroweTool.WorldObjectSystem;
using System.Collections;
using UnityEngine;

namespace lLCroweTool.TerrainSystem.FarmSystem
{
    [RequireComponent(typeof(CoroutineTimerModule))]
    public class FarmSystem_GrowObject : MonoBehaviour, IActionInterectObject
    {
        //자라나는 농사,축산물 오브젝트
        //그로우오브젝트와 그로우플루트로 나눔
        //그로우오브젝트는 나무.풀.돼지.소.양 같은 존재이고
        //그로우플루트는 해당그로우오브젝트가 자란후 나오는 최종상품결과물이다
        //과일.채소.돼지고기.소고기.우유.털
        //데이터는 그로우 플루트가 가짐.
        //그로우오브젝트는 여러 욕구를 충족시켜 죽지않게 유지시텨줘야하며 일정시간뒤에 최종상품을 직접수거하거나 자동수거할수 있다.
        //나올수 있는상품은 각각 클래스로 지정하게하기




        //보이는 오브젝트 처리관련
        [Header("비쥬얼 오브젝트관련 처리")]
        public SpriteRenderer sr;



        [Header("주 자라나는 데이터")]
        //주 자라나는 데이터
        public FarmSystem_GrowObjectObjectScript growObjectData;
        public int growObjectCurAge = 0;//현재나이
        public GrowObjectProductStatus[] growObjectProductStatusArray = new GrowObjectProductStatus[0];

        //public ItemSpawnSetting itemSpawnSetting = new ItemSpawnSetting();

        [Header("타겟이 될 유닛")]
        public TestWorldObject unitObject;

        //농사시스템
        //자라나는 위치
        //해당위치에서 고정형 자라나는 오브젝트를 심을수 있다
        //식물종류

        private CoroutineTimerModule timerModule;


        private void Awake()
        {
            timerModule = GetComponent<CoroutineTimerModule>();
            timerModule.SetTimer(growObjectData.growingAgeTimer);
            timerModule.AddUnityEvent(delegate { UpdateGrowObject(this); });
            if (unitObject == null)
            {
                unitObject = GetComponent<TestWorldObject>();
            }
        }

        private static void UpdateGrowObject(FarmSystem_GrowObject growObject)
        {
            //나오는과실(상품)기능
            for (int i = 0; i < growObject.growObjectProductStatusArray.Length; i++)
            {
                //제작된 상품이 준비됫는가
                if (growObject.growObjectProductStatusArray[i].isProductionReady)
                {
                    //과실 나이추가
                    growObject.growObjectProductStatusArray[i].productCurAge++;

                    //딸수 있을 나이가 있는가
                    if (growObject.growObjectProductStatusArray[i].productCurAge >= growObject.growObjectData.growObjectProductInfoArray[i].productFullGrowAge)
                    {
                        //자동수확기능
                        if (growObject.growObjectData.growObjectProductInfoArray[i].isAutoHarvest)
                        {
                            //자동수확 확률체크
                            if (lLcroweUtil.ProbabilityCal(growObject.growObjectData.growObjectProductInfoArray[i].autoHarvestPersent))
                            {
                                //수확
                                growObject.ActionHarvest();
                                return;
                            }
                        }
                    }

                    //생산된 상품이 썩었는가?
                    if (growObject.growObjectProductStatusArray[i].isProductionRotten)
                    {
                        //썩은후에 리셋되는 상품인가?
                        if (growObject.growObjectData.growObjectProductInfoArray[i].isRottenToReset)
                        {
                            //리셋
                            growObject.growObjectProductStatusArray[i].isProductionRotten = false;
                            growObject.growObjectProductStatusArray[i].isProductionReady = false;
                        }
                    }
                    else
                    {
                        //안썩었으면
                        //썩을 수 있는 과실인가
                        if (growObject.growObjectData.growObjectProductInfoArray[i].productMaxAgeToRotton)
                        {
                            //최대나이인가
                            if (growObject.growObjectProductStatusArray[i].productCurAge >= growObject.growObjectData.growObjectProductInfoArray[i].productMaxAge)
                            {
                                //썩을 확률체크
                                if (lLcroweUtil.ProbabilityCal(growObject.growObjectData.growObjectProductInfoArray[i].productRottonPersentValue))
                                {
                                    //썩음
                                    growObject.growObjectProductStatusArray[i].isProductionRotten = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    //상품이준비가 안됫으면 체크

                    //해당 그로우오브젝트가 다 자랐는가
                    if (growObject.growObjectCurAge >= growObject.growObjectData.growObjectFullyGrowAge)
                    {
                        //과실 나올 확률체크
                        if (lLcroweUtil.ProbabilityCal(growObject.growObjectData.growObjectProductInfoArray[i].productionPercent))
                        {
                            growObject.growObjectProductStatusArray[i].isProductionReady = true;
                        }
                    }
                }
            }

            //그로우오브젝트 자라기
            growObject.growObjectCurAge++;
            
            if (growObject.unitObject.isUseStat)
            {
                //최대나이체크
                if (growObject.growObjectData.isUseMaxAgeDamage)
                {
                    //최대나이가 넘으면 대미지를 받음
                    if (growObject.growObjectCurAge >= growObject.growObjectData.growObjectMaxAge)
                    {                        
                        UnitStatusModuleManager.Instance.UnitStatusTakeDamaged(growObject.growObjectData.maxAgeDamage, growObject.unitObject.unitStatus, growObject.unitObject);
                    }
                }
                //욕구체크
                if (growObject.unitObject.unitStatus.unitStatusData.isUseDesireSystem)
                {
                    //욕구가 충족되지않으면 대미지를 받음
                    DesireKategorie desireKategorie = DesireKategorie.Nothing;
                    growObject.unitObject.unitStatus.unitDesireSystem.GetDesireDepletion(ref desireKategorie);
                    if (desireKategorie != DesireKategorie.Nothing)
                    {   
                        UnitStatusModuleManager.Instance.UnitStatusTakeDamaged(growObject.growObjectData.desireDepletionDamage, growObject.unitObject.unitStatus, growObject.unitObject);
                    }
                }
            }
        }
       
        /// <summary>
        /// 수확기능
        /// </summary>
        public void ActionHarvest()
        {
            for (int i = 0; i < growObjectProductStatusArray.Length; i++)
            {
                ActionHarvest(growObjectProductStatusArray[i], growObjectData.growObjectProductInfoArray[i], this);
            }
        }

        /// <summary>
        /// 수확기능
        /// </summary>
        /// <param name="growObjectProductStatus">상품스탯</param>
        /// <param name="growObjectProductInfo">상품정보</param>
        /// <param name="growObject">자라나는 오브젝트</param>
        private static void ActionHarvest(GrowObjectProductStatus growObjectProductStatus,GrowObjectProductInfo growObjectProductInfo ,FarmSystem_GrowObject growObject)
        {
            //준비안됫으면 수확불가능
            if (!growObjectProductStatus.isProductionReady)
            {
                return;
            }

            //안썩었으면 수확
            if (!growObjectProductStatus.isProductionRotten)
            {
                //아이템관련
                if (growObjectProductInfo.itemData != null)
                {
                    //확률체크
                    if (lLcroweUtil.ProbabilityCal(growObjectProductInfo.itemHarvestPersent))
                    {
                        //InventoryManager.Instance.CreateWorldItemForWorldMap(
                        //  growObjectProductInfo.itemData,
                        //  Random.Range(growObjectProductInfo.itemMinCount, growObjectProductInfo.itemMaxCount),
                        //  growObject.transform.parent,
                        //  growObject.transform,
                        //  growObject.itemSpawnSetting.offSet,
                        //  growObject.itemSpawnSetting.isRandom,
                        //  growObject.itemSpawnSetting.isBox,
                        //  growObject.itemSpawnSetting.dropSize
                        //  );
                    }
                }


                //오브젝트 관련
                if (growObjectProductInfo.productObject != null)
                {
                    //확률체크
                    if (lLcroweUtil.ProbabilityCal(growObjectProductInfo.productHarvestPersent))
                    {
                        int value = Random.Range(growObjectProductInfo.productMinCount, growObjectProductInfo.productMaxCount);
                        for (int j = 0; j < value; j++)
                        {
                            //GameObject targetObject = Instantiate(growObjectProductInfo.productObject, growObject.transform.parent);
                            ////위치선언
                            //if (growObject.itemSpawnSetting.isRandom)
                            //{
                            //    targetObject.transform.SetPositionAndRotation(InventoryManager.Instance.GetRandomSpawnPosition(growObject.itemSpawnSetting.isBox, growObject.transform, growObject.itemSpawnSetting.dropSize) + growObject.itemSpawnSetting.offSet, Quaternion.identity);
                            //}
                            //else
                            //{
                            //    targetObject.transform.SetPositionAndRotation((Vector2)growObject.transform.position + growObject.itemSpawnSetting.offSet, Quaternion.identity);
                            //}
                        }
                    }
                }
            }

            //리셋
            growObjectProductStatus.isProductionReady = false;
            growObjectProductStatus.isProductionRotten = false;
        }

        public TestWorldObject GetWorldObject()
        {
            return unitObject;
        }

        public bool CheckInterectObject(GameObject _targetObject)
        {
            return true;
        }

        public void InterectObjectAction(GameObject _targetObject)
        {
            //if (_targetObject.TryGetComponent(out Inventory inventory))
            //{
            //    FarmSystem_GrowObjectInterectUI.Instance.ShowTargetGrowObjectInterectionUI(this, inventory);
            //}
        }

        public string GetInterectText()
        {
            return "interection_GrowObject";
        }
    }
}
#endif
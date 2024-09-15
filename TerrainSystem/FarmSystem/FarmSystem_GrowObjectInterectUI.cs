#if Doozy
using System.Collections;
using UnityEngine;
using lLCroweTool.SingletonUI;
using Doozy.Engine.UI;
using TMPro;
using lLCroweTool.ObjectPool;
using lLCroweTool.DesireFuncSystem;
using lLCroweTool.InventorySystem;
using UnityEngine.UI;

namespace lLCroweTool.TerrainSystem.FarmSystem
{
    public class FarmSystem_GrowObjectInterectUI : MonoBehaviourSingletonUI<FarmSystem_GrowObjectInterectUI>
    {
        //자라나는 오브젝트와 상호작용할때 사용하는 UI

        //매니저화
        //어차피 팜시스템을 사용하는 대부분의 기능들은 인터렉션UI를 통해 작동될것이다.
        //그러니 기능을 여기다 쓔서박는게 나아보임


        //니드아이템을 집어넣어 특정욕구를 충족//소모품아이템에서 해당되는 욕구가 있으면 집어넣어서 욕구를 충족시킬수 있음.
        //or 아이템이 존재하면 상호작용을 체크

        //욕구를 해결하기위해 들어가있는 아이템데이터
        public TextMeshProUGUI desireCheckText;
        public GrowObjectDesireInterectSlot targetNeedSlot;//Setter
        public Button insertItemDesireButton;
        //public float drinkTime = 5f;//욕구가해결되는 아이템을 흡수하기까지 걸리는 시간


        public TextMeshProUGUI targetGrowObjectInfoTextObject;        
        public FarmSystem_GrowObject targetGrowObject;
        public Inventory targetInventory;

        public Transform desireUIPos;
        [System.Serializable]public class DesireUIPool : CustomObjectPool<DesireUI> { }
        public DesireUIPool desireUIPool = new DesireUIPool();

        protected override void Awake()
        {
            base.Awake();
            insertItemDesireButton.Button.onClick.AddListener(InsertItemDesire);
        }

        public void ShowTargetGrowObjectInterectionUI(FarmSystem_GrowObject growObject, Inventory inventory)
        {
            targetGrowObject = growObject;
            targetInventory = inventory;
            InitGrowObjectInfoText();
            ShowUIView();
        }

        /// <summary>
        /// 그로우오브젝트의 현재상태를 텍스트로 보여주는 함수
        /// </summary>
        private void InitGrowObjectInfoText()
        {
            //그로우오브젝트관련
            //나이
            string target = LocalizingManager.Instance.GetLocalLizeText("GrowObjectMaxAge") + " : " + targetGrowObject.growObjectData.growObjectMaxAge + "\n";
            target += LocalizingManager.Instance.GetLocalLizeText("GrowObjectCurAge") + " : " + targetGrowObject.growObjectCurAge + "\n";

            //생산품관련
            for (int i = 0; i < targetGrowObject.growObjectData.growObjectProductInfoArray.Length; i++)
            {
                //과실이 맺혓는지.
                if (targetGrowObject.growObjectProductStatusArray[i].isProductionReady)
                {
                    //아이템처리
                    if (targetGrowObject.growObjectData.growObjectProductInfoArray[i].itemData != null)
                    {
                        target += LocalizingManager.Instance.GetLocalLizeText(targetGrowObject.growObjectData.growObjectProductInfoArray[i].itemData.objectName) + "\n";
                    }

                    //오브젝트처리
                    if (targetGrowObject.growObjectData.growObjectProductInfoArray[i].productObject != null)
                    {
                        target += LocalizingManager.Instance.GetLocalLizeText(targetGrowObject.growObjectData.growObjectProductInfoArray[i].productObject.name) + "\n";
                    }

                    //썩었는지
                    if (targetGrowObject.growObjectProductStatusArray[i].isProductionRotten)
                    {
                        target += LocalizingManager.Instance.GetLocalLizeText("Rotten") + "\n";
                    }
                }
            }
            targetGrowObjectInfoTextObject.text = target;


            //욕구관련//유닛에 존재
            //desireUIPool.AllObjectDeActive();
            //if (targetGrowObject.unitObject.unitStatus.unitStatusData.isUseDesireSystem)
            //{
            //    DesireSystem desireSystem = targetGrowObject.unitObject.unitStatus.unitDesireSystem;
            //    for (int i = 0; i < desireSystem.GetDesires().Length; i++)
            //    {
            //        DesireUI desireUI = desireUIPool.RequestPrefab();
            //        desireUI.SetDesireUI(desireSystem.GetDesires()[i]);
            //        desireUI.SetParent(desireUIPos);
            //        desireUI.transform.position = desireUIPos.transform.position;
            //    }
            //}
        }

        /// <summary>
        /// 아이템으로부터 욕구가 충족되는지체크(순서1)
        /// </summary>
        /// <param name="itemData">어아탬대이터</param>        
        /// <returns>충족되는 여부</returns>
        public bool CheckDesire(ItemObjectScript itemData)
        {
            if (!targetGrowObject.GetWorldObject().isUseStat)
            {
                return false;
            }
            if (!targetGrowObject.GetWorldObject().unitStatus.unitStatusData.isUseDesireSystem)
            {
                return false;
            }

            //존재하는지 체크
            DesireSystem desireSystem = targetGrowObject.GetWorldObject().unitStatus.unitDesireSystem;
            bool isDone = false;
            for (int i = 0; i < itemData.targetDesires.Length; i++)
            {
                if (desireSystem.ExistDesire(itemData.targetDesires[i].desireKategorie))
                {
                    isDone = true;
                    break;
                }                
            }
            return isDone;
        }

        /// <summary>
        /// 욕구충족시키기 함수(순서2)
        /// </summary>
        public void InsertItemDesire()
        {
            //아이템에 있는 욕구들을 충족시킨후
            //해당슬롯의 아이템을 없애줌
            //해당되는 아이템을 다없애면 위의 슬롯에 있는 데이터도 없앰
            if (targetNeedSlot.GetItemData() == null)
            {
                return;
            }

            //욕구충전
            ItemObjectScript itemData = targetNeedSlot.GetItemData();
            DesireSystem desireSystem = targetGrowObject.GetWorldObject().unitStatus.unitDesireSystem;

           
            for (int i = 0; i < itemData.targetDesires.Length; i++)
            {
                desireSystem.AddSearchDesireCATCurValue(itemData.targetDesires[i].desireKategorie, itemData.targetDesireEffectValues[i]);
            }

            //한개없앰
            InventoryManager.Instance.AddItemDataToInventory(targetInventory, targetNeedSlot.GetItemData(), -1);

            //존재체크
            if (!InventoryManager.Instance.CheckExistItemDataToInventory(targetInventory, targetNeedSlot.GetItemData(), 1))
            {
                //존재하지않으면 세팅된거를 지움
                targetNeedSlot.SetSlotToData(null);
            }
        }
    }
}
#endif
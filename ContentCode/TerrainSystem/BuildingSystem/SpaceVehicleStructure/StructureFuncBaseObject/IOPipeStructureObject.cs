#if Doozy

using lLCroweTool.DesireFuncSystem;
using lLCroweTool.InventorySystem;
using lLCroweTool.TerrainSystem.BasementTileMap;
using System.Collections;
using UnityEngine;

namespace lLCroweTool.BuildingSystem.FuncStructure
{
    
    public class IOPipeStructureObject : StructureFuncBaseObject, IActionInterectObject
    {
        //연결된 파이프의 내용물을 꺼내거나 파이프에 집어넣는 역할을 가짐
        //연료탱크처럼 자체적인 타임모듈로 작동


        public IOType ioType;

        public DesireKategorie desireKategorie;//집어넣거나 뺄 타입

        //내용물을 꺼내거나 집어넣을때 사용할 무언가
        //1. 인벤토리
        //2. 아이템소환

        //꺼내거나 집어넣을 수있는 욕구타입체크
        //1. Electric => 아이템화 가능, 인벤토리 사용가능
        //2. Fuel => 아이템화 가능, 인벤토리 사용가능
        //3. Oxygen => 건축물에 해당된 산소에서 받아오거나 없앰       
        //4. Ammo => 아이템화 가능, 인벤토리 사용가능

        public Vector2 exitPos;//나오는 출구

        private bool isExistInventory;//사용여부에 따라 인벤토리에 저장해둘지 아니면 그대로 꺼내올지 체크//카테고리별 지정된 데이터를 세팅해줘야함//어디다 세팅해주지//인벤토리매니저에서 작업
        public Inventory targetInventory;
              

        //콜라이더가 있어야됨
        protected override void AwakeInitStructure()
        {
            base.AwakeInitStructure();
           
            SetInventory(targetInventory);           
        }

        protected override void UpdateStructureAction()
        {
            //UpdateIOPipe
            switch (ioType)
            {
                case IOType.IN_OneWay:
                    //인풋//파이프로 들어오는 구역
                    if (GetInputPipeBridgeInfo(ref refPipeBridgeInfo))
                    {
                        if (refPipeBridgeInfo.GetPipeBridge().GetPipe(ref refPipe))
                        {
                            //산소카테고리인지 체크
                            if (refPipe.GetDesireKategorie() == DesireKategorie.Oxygen)
                            {
                                if (isExistStructureObject)
                                {
                                    //산소이면 해당 공간에 산소를 가져와서 파이프에 옮김
                                    BasementTileMap basementTileMap = structureObject.GetBasementTileMap();
                                    Vector3Int pos = lLcroweUtil.GetWorldToCell(structureObject.transform.position, basementTileMap.GetTileMap());
                                    VoidRoomInfo voidRoomInfo = basementTileMap.GetVoidRoomInfo(pos);
                                    //산소 함수제작해야함
                                    //산소 들어갈공간이 충분한지 체크후 집어넣기
                                    //or 제네레이터 확인
                                    //일단 임시
                                    if (voidRoomInfo.GetCurOxygenValue() - refPipeBridgeInfo.GetPressure() >= 0)
                                    {
                                        int temp = refPipeBridgeInfo.GetPressure();
                                        //_oxygen.curOxygenValue -= temp;
                                        voidRoomInfo.RemoveOxygenValue(temp);
                                        refPipe.AddDesireValueData(ref temp, refPipe.GetDesireKategorie());
                                    }
                                }
                            }
                            else
                            {
                                ItemObjectScript _itemData = null;
                                switch (refPipe.GetDesireKategorie())
                                {
                                    case DesireKategorie.Electric:
                                        _itemData = InventoryManager.Instance.electricDesireItemData;
                                        break;
                                    case DesireKategorie.Fuel:
                                        _itemData = InventoryManager.Instance.fuelDesireItemData;
                                        break;
                                    case DesireKategorie.Ammo:
                                        _itemData = InventoryManager.Instance.ammoDesireItemData;
                                        break;

                                }
                                //인벤토리가 있는지 체크
                                if (isExistInventory)
                                {
                                    //있으면

                                    //해당되는 아이템이 있는지 체크
                                    if (InventoryManager.Instance.CheckExistItemDataToInventory(targetInventory, _itemData, refPipeBridgeInfo.GetPressure()))
                                    {
                                        //있으면 파이프에 집어넣고
                                        //계산
                                        //집어넣는 함수 제작
                                        int temp = refPipeBridgeInfo.GetPressure();
                                        bool isDone = false;
                                        refPipe.AddDesireValueData(ref temp, refPipe.GetDesireKategorie());
                                        //인벤토리에서 해당되는 아이템 계산
                                        InventoryManager.Instance.DealItemDataToInventory(targetInventory, _itemData, refPipeBridgeInfo.GetPressure(), ref isDone);
                                    }
                                }
                                else
                                {
                                    //없으면 
                                    //아무행동안함
                                }
                            }
                        }
                    }
                    break;

                case IOType.OUT_OneWay:
                    //아웃풋//파이프의 내용물을 밖으로 보내는구역
                    if (GetOutputPipeBridgeInfo(ref refPipeBridgeInfo))
                    {
                        if (refPipeBridgeInfo.GetPipeBridge().GetPipe(ref refPipe))
                        {
                            //파이프 내용물의 욕구카테고리를 체크
                            if (refPipe.GetDesireKategorie() != DesireKategorie.Nothing)
                            {
                                //산소카테고리인지 체크
                                if (refPipe.GetDesireKategorie() == DesireKategorie.Oxygen)
                                {
                                    if (isExistStructureObject)
                                    {
                                        //산소이면 해당 공간에 산소를 뿌려줌
                                        BasementTileMap basementTileMap = structureObject.GetBasementTileMap();
                                        Vector3Int pos = lLcroweUtil.GetWorldToCell(structureObject.transform.position, basementTileMap.GetTileMap());
                                        VoidRoomInfo _oxygen = basementTileMap.GetVoidRoomInfo(pos);
                                        //산소 함수제작해야함
                                        //산소 들어갈공간이 충분한지 체크후 집어넣기
                                        //or 제네레이터 확인
                                        //일단 임시
                                        if (_oxygen.GetCurOxygenValue() + refPipeBridgeInfo.GetPressure() <= _oxygen.GetMaxOxygenValue())
                                        {
                                            int temp = refPipeBridgeInfo.GetPressure();
                                            //_oxygen.curOxygenValue += temp;
                                            _oxygen.AddOxygenValue(temp);
                                            refPipe.RemoveDesireValueData(ref temp);
                                        }
                                    }
                                }
                                else
                                {
                                    ItemObjectScript _itemData = null;
                                    switch (refPipe.GetDesireKategorie())
                                    {
                                        case DesireKategorie.Electric:
                                            _itemData = InventoryManager.Instance.electricDesireItemData;
                                            break;
                                        case DesireKategorie.Fuel:
                                            _itemData = InventoryManager.Instance.fuelDesireItemData;
                                            break;
                                        case DesireKategorie.Ammo:
                                            _itemData = InventoryManager.Instance.ammoDesireItemData;
                                            break;

                                    }
                                    //인벤토리가 있는지 체크
                                    if (isExistInventory)
                                    {
                                        //있으면
                                        //해당 인벤토리의 빈슬롯에 파이프의 내용물을 저장함

                                        //아이템집어넣을 공간이 있는지 체크
                                        if (InventoryManager.Instance.CheckEmptyInventorySlot(targetInventory, _itemData, refPipeBridgeInfo.GetPressure()))
                                        {
                                            //있으면 인벤토리에 집어넣고
                                            //계산//추가아이템 다루지않음.
                                            InventoryManager.Instance.AddItemDataToInventory(targetInventory, _itemData, refPipeBridgeInfo.GetPressure());
                                        }
                                    }
                                    else
                                    {
                                        //없으면 
                                        //특정위치에 파이프의 내용물에 대한 아이템 생성
                                        InventoryManager.Instance.CreateWorldItemForWorldMap(_itemData, refPipeBridgeInfo.GetPressure(), transform.parent, transform, exitPos);
                                    }
                                }
                            }
                        }
                    }
                    break;
            }
        }
        
        public void SetInventory(Inventory _inventory)
        {
            targetInventory = _inventory;
            if (ReferenceEquals(targetInventory, null))
            {
                isExistInventory = false;
            }
            else
            {
                isExistInventory = true;
            }
        }

      
        public bool CheckInterectObject(GameObject _targetObject)
        {
            return !isExistInventory;
        }

        public void InterectObjectAction(GameObject _targetObject)
        {
            InventoryBoxUI.Instance.SetInventroyBoxUI(targetInventory, _targetObject);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(exitPos, 0.5f);
        }

        public string GetInterectText()
        {
            return "IOPipeStructrue";
        }
    }
}
#endif
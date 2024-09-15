#if Doozy
using lLCroweTool.InventorySystem;
using lLCroweTool.SlotSystem;
using System.Collections;
using UnityEngine;

namespace lLCroweTool.TerrainSystem.FarmSystem
{
    public class GrowObjectDesireInterectSlot : InventorySlot
    {   
        protected override bool CheckChangeSlot()
        {
            //변경시 해당되는 욕구가 있는지 체크한뒤 변경하게 바꿈
            ItemObjectScript itemData = DragSlotUICard.Instance.GetDragInventorySlot().GetItemData();
            return FarmSystem_GrowObjectInterectUI.Instance.CheckDesire(itemData);
        }

        protected override void SetSlotToDataAddOnActionFunc()
        {
            //아이템수량텍스트 비표기
            //세팅된 아이템수량을 한개로 바꿈
            SetSlotCount(1);
        }
    }
}
#endif
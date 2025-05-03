using UnityEngine;
using System.Collections.Generic;


namespace lLCroweTool.EquipmentSystem.ActionEquipment
{
    public class ExploreInterect : MonoBehaviour
    {   
        //폭발에셋과의 값과 동일시 해야 작동이 잘됨

        public float exploreRadius;//직접 설정할것
        private List<Collider2D> collider2DList = new List<Collider2D>();
        private ContactFilter2D ExploreInterectLayer;//레이어처리
        //public List<BuffObjectScript> applyBuffData = new List<BuffObjectScript>();//폭발됫을시 적용시킬버프들

        /// <summary>
        /// 폭파에셋의 폭발이벤트에 집어넣을 함수
        /// </summary>
        public void ExtendExploreChecker()//이벤트용
        //private void Activate()//자동으로 시전//싱글일때만 사용하기
        {
            if (Physics2D.OverlapCircle(transform.position, exploreRadius, ExploreInterectLayer, collider2DList) == 0)
            {
                return;
            }
            
            //for (int i = 0; i < collider2DList.Count; i++)
            //{
            //    if (collider2DList[i].TryGetComponent(out HitBox hitBox))
            //    {
            //        if (!hitBox.GetWorldObject().isUseStat)
            //        {
            //            continue;
            //        }
            //        UnitStatusModule unitStatus = hitBox.GetWorldObject().unitStatus;
            //        //버프요청
            //        //요청할때는 이름으로 매니저에서 찾아서 요청
            //        for (int j = 0; j < applyBuffData.Count; j++)
            //        {
            //            BuffManager.Instance.AddBuff(unitStatus, applyBuffData[j], null);
            //        }
            //    }
            //}
        }
      
        protected virtual void OnDestroy()
        {
            collider2DList.Clear();
            collider2DList = null;
            //applyBuffData.Clear();
            //applyBuffData = null;
        }
    }
}

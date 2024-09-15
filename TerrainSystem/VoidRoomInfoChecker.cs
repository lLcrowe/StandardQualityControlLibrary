#if Doozy
using lLCroweTool.TileMap;
using lLCroweTool.TimerSystem;
using UnityEngine;
using UnityEngine.Events;

namespace lLCroweTool.TerrainSystem.BasementTileMap
{   
    public class VoidRoomInfoChecker : MonoBehaviour
    {
        //산소체커
        //일정타임마다 산소가 있는지 체크한다
        private TimerModule_Element timerModule;
        [Header("산소를 한번에 얼마소모하는가")]        
        public int consumeOxygenValue;
        [Space]
        [Header("상호작용할 산소레이어")]
        public LayerMask targetOxygenLayer;//산소의 위치는 비어있는공간이다 TerrainLayer로 체크
        [Space]
        [Header("산소를 사용하는 욕구가 있으면 체크하여 참조시킬것")]
        //산소욕망을 상승치 양
        public int oxygenDesireIncreaseValue;
        //욕구시스템의 욕구안 이벤트에 increasevalue를 집어넣어야함
        [Tooltip("욕구시스템의 욕구안 이벤트에 increaseValue를 집어넣어야함")]
        public System.Action<int> desireIncreaseEvent;

        protected void Awake()
        {
            timerModule.SetTimer(1f);
        }

        public void Update()
        {
            if (!timerModule.CheckTimer())
            {
                return;
            }

            Collider2D collider2D = Physics2D.OverlapCircle(transform.position, 0.5f, targetOxygenLayer);
            if (collider2D != null)
            {
                //콜라이더체크
                if (collider2D.TryGetComponent(out BasementTileMap basementTileMap))
                {
                    Vector3Int pos = lLcroweRectTileMapUtil.GetWorldToCell(transform.position, basementTileMap.GetTileMap());
                    VoidRoomInfo voidRoomInfo = basementTileMap.GetVoidRoomInfo(pos);

                    //해당공간의 산소량이 필요산소량보다 많으면 체크
                    if (voidRoomInfo.GetCurOxygenValue() >= consumeOxygenValue)
                    {
                        //해당산소량 빼주기
                        //_targetOxygen.curOxygenValue -= decreaseOxygenValue;
                        //_targetOxygen.LimitOxygenValueCheck();
                        voidRoomInfo.RemoveOxygenValue(consumeOxygenValue);

                        //욕구시스템을 increasevalue를 이벤트에 집어넣어야함
                        //targetDesire.DecreaseDesireValue(oxygenDesireIncreaseValue);
                        desireIncreaseEvent.Invoke(oxygenDesireIncreaseValue);
                    }
                }
            }
        }
    }
}

#endif
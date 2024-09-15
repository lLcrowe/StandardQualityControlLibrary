#if Doozy
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using lLCroweTool.TerrainSystem.BasementTileMap;

namespace lLCroweTool.BuildingSystem.FuncStructure.Terminal
{
    /// <summary>
    /// 월드오브젝트 문 컨트롤러
    /// </summary>
    
    public class WorldObjectDoorController : StructureFuncTerminalObject, IActionInterectObject
    {
        //문짝이 열리게
        //건축물 오브젝트에 집어넣는 컴포넌트
        //문 타입마다 다르게 작동하고
        //열리는 시간 다르게

        public ContactFilter2D basementContactFilter2D = new ContactFilter2D();
        
        [Tooltip("열리는 상태")]
        public bool isOpen = false;//열리는 방향//초기세팅용으로 가능
        [Tooltip("잠금 여부")]
        public bool isLock = false;//잠금 여부
        [Tooltip("한번만 제대로 열리고 잠금처리 여부")]
        public bool isOneOpen = false;//한번만 제대로 열리고 잠금처리 여부
        [Tooltip("슬라이드 도어(옆으로 여닫는 문)인지 위아래로 닫는 문인지 체크")]
        public bool isSlidingDoor = false;//슬라이드 도어(옆으로 여닫는 문)인지 위아래로 닫는 문인지 체크

        [Space]
        [Tooltip("문을 다시여는데 걸리는 시간")]
        public float reActionTimer = 1f;//문을 다시여는데 걸리는 시간
        private float reActionTime;

        public GameObject leftDoorObject;//왼쪽으로 열리는 문//콜라이더 존재
        public GameObject rightDoorObject;//오른쪽으로 열리는 문//콜라이더 존재
                
        [SerializeField] private VoidRoomInfo leftDoorVoidRoom;//지정된 공간
        [SerializeField] private VoidRoomInfo rightDoorVoidRoom;//지정된 공간

        [Space]
        [Tooltip("열리는 최대치")]
        public float maxOpenValue = 1;//최대치
        [Tooltip("현대 변경치")]
        private float curOpenValue = 0;//현대 변경치
        [Tooltip("문이 열릴때 걸리는 시간")]
        public float openDuration = 2f;//문이 열릴때 걸리는 시간

        //사운드관련//미완
        [SoundGroup] [SerializeField] private string ActiveSound;
        [SoundGroup] [SerializeField] private string OxygenSound;


        private bool isChangeEvent = false;//문 열리는 방향이 변경됫을때 변경하여 이벤트를 체크
        private float time;

        protected override void AwakeInitStructure()
        {
            base.AwakeInitStructure();

            List<Collider2D> collider2DList = new List<Collider2D>(10);

            //체크
            if (Physics2D.OverlapBox(transform.position, Vector2.one, transform.rotation.z, basementContactFilter2D, collider2DList) > 0)
            {
                for (int i = 0; i < collider2DList.Count; i++)
                {
                    if (ReferenceEquals(collider2DList[i], null))
                    {
                        if (collider2DList[i].TryGetComponent(out BasementTileMap basementTileMap))
                        {
                            Vector3Int pos = lLcroweUtil.GetWorldToCell(leftDoorObject.transform.position, basementTileMap.GetTileMap());
                            leftDoorVoidRoom = basementTileMap.GetVoidRoomInfo(pos);

                            pos = lLcroweUtil.GetWorldToCell(rightDoorObject.transform.position, basementTileMap.GetTileMap());
                            rightDoorVoidRoom = basementTileMap.GetVoidRoomInfo(pos);
                            break;
                        }
                    }
                }
            }

            //열리는 값에 따른 설정
            curOpenValue = isOpen ? maxOpenValue : 0;
            reActionTime = Time.time;

            ActionDoor();

        }

        protected override bool UpdateSatisfactionStructureFuncTerminal()
        {
            //절반거리가 됫을시 문상태를 변경//이벤트변경들어가면 그때 확인됨
            if (isChangeEvent && Time.time > time)
            {
                //여기서 상태를 변경
                ChangeDoorState(isOpen);
                isChangeEvent = false;
                return true;
            }
            else
            {
                return false;
            }
        }

        //문을 열때 문에 있는 비어있는공간의 상태를 변경하는 함수
        //상태이상으로 문을 일정이상열게되면 작동하여 산소와 방해물을 변경
        private void ChangeDoorState(bool _isOpen)
        {
            //지정된 공간의 산소 세팅변경
            leftDoorVoidRoom.SetOxygenAction(_isOpen, BuildingType.Door);
            rightDoorVoidRoom.SetOxygenAction(_isOpen, BuildingType.Door);

            //방해물 세팅변경
            //현재세팅과 반대로 Bool로 세팅되야함
            //targetWorldStructureObjects[i].GetParentVoidRoom().SetActiveObstacleObject(!_isOpen);//<=움직여서 해당쪽에있는 방해물을 치워주니 그리필요하지는 않은거 같다
            AstarPath.active.Scan();
        }

        /// <summary>
        /// 잠금장치 세팅 함수
        /// </summary>
        /// <param name="_isLock">잠금 여부</param>
        public void SetLock(bool _isLock)
        {
            isLock = _isLock;
        }

        /// <summary>
        /// 설정된 세팅에 맞게 문을 여는 기능
        /// </summary>
        public void ActionDoor()
        {
            //문이 작동되는 방식
            if (isSlidingDoor)
            {
                //슬라이드식 방식

                //열리는 값에 따른 설정
                curOpenValue = isOpen ? maxOpenValue : 0;
            }
            else
            {
                //위아래로 여닫는 방식
                //애니메이션으로 할거 같은데 지금으로는 아직 생각이없음
                //미완 20210105
            }
            isChangeEvent = true;
            //문위치의 산소가 작동되기 위한 값//지속시간을 반으로 나누값 + 현재시간
            time = Time.time + (openDuration * 0.5f);

            rightDoorObject.transform.DOLocalMoveX(curOpenValue, openDuration);
            leftDoorObject.transform.DOLocalMoveX(-curOpenValue, openDuration);
        }

        public void InterectObjectAction(GameObject _targetObject)
        {
            //시간체크
            if (Time.time < reActionTimer + reActionTime)
            {
                //시간보다 작으면
                return;
            }

            reActionTime = Time.time;
            isOpen = !isOpen;

            //한번만 제대로 열리고 잠금처리 여부
            if (isOneOpen)
            {
                isLock = true;
            }

            //문작동
            ActionDoor();
        }

        public bool CheckInterectObject(GameObject _targetObject)
        {
            //잠금 상태이면 작동안하게            
            return !isLock;
        }

        public string GetInterectText()
        {
            return "OpenDoor";
        }
    }
}
#endif
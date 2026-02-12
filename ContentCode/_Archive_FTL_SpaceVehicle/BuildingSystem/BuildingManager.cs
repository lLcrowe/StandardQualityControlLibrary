#if Doozy
using UnityEngine;
using System.Collections.Generic;
using lLCroweTool.SingletonUI;
using lLCroweTool.Dictionary;
using lLCroweTool.ObjectPool;
using lLCroweTool.TerrainSystem.BasementTileMap;
using lLCroweTool.ToolTipSystem;
using lLCroweTool.WorldObjectSystem;
using UnityEngine.UI;

namespace lLCroweTool.BuildingSystem
{
    public class BuildingManager : MonoBehaviourSingletonUI<BuildingManager>
    {
        //미리세팅되어있어야할 매니저
        //Canvas안에서 세팅되있어야함UI관련


        //현재 건설가능한 빌딩 데이터들//빌딩연구트리시스템에서 작업하고 여기서 수동으로 집어넣어도 작동됨
        [SerializeField] private List<BuildingObjectScript> unLockBuildingDataList = new List<BuildingObjectScript>();

        //현재 건설된 빌딩데이터들//건물데이터, 건물수
        [System.Serializable] public class ConstructBuildingDataBible : CustomDictionary<BuildingObjectScript, int> { }

        public ConstructBuildingDataBible currentConstructBuildingDataBible = new ConstructBuildingDataBible();
        
        //버튼관련//오브젝트폴
        [System.Serializable]public class BuildingCreateButtonPool : CustomObjectPool<BuildingCreateButton> { }
        [SerializeField]public BuildingCreateButtonPool buildingCreateButtonPool = new BuildingCreateButtonPool();

        [Header("세팅할 건물카테고리 버튼들")]//해당버튼을 클릭시 해당되는 빌딩데이터가 온오프
        public Button frameButton;
        public Button floorButton;
        public Button wallButton;
        public Button doorButton;
        public Button buildingButton;
        public Button pipeButton;

        //체크용
        private bool isOn = false;//위의 버튼이 켜져있는지
        private BuildingType isOnType;//켜져있는타입은 뭔지
        [Space]
        [Header("세팅할 건물카테고리 이미지들")]
        //건물해체 이미지
        public Sprite dismantleBuildingImage;

        //바닥해체 이미지
        public Sprite dismantleFloorImage;

        //수리 이미지
        public Sprite fixImage;

        //연결 이미지
        public Sprite connectImage;
        [Space]
        [Header("건물건설로직 세팅")]
        //건축로직 세팅
        public BuildingConstructionType targetBuildingConstructionModeType;//건물건축타입
        public BuildLogicType targetBuildingLogicType;//건설로직타입

        public Transform targetPlayerObject;//플레이어캐릭터
        public float targetDistance = 2f;//플레이어캐릭터로부터의 건설상호작용거리

        public BasementTileMap targetBasementTileMap;//타겟팅될 전초기지타일맵
        [Space]
        [Header("빌딩툴팁")]
        //빌딩 툴팁
        public ToolTipUiView buildingtoolTipUiView;

        //리소스매니저합친구역-----------------
        //리소스는 총 3가지
        //연료
        //자재
        //탄약

        //전체적인 자원을 통괄해서 보여주는 곳
        //1. 따로 사용하는 창고를 이용? 
        //2. 특정 인벤토리를 이용? 
        //3. 모든 인벤토리를 이용?
        //4. 아예 별개로 작업

        //타겟이 될 메인 인벤토리
        //리소스의 사용처가 될것이다.
        //public Inventory targetMainInventory;


        //리소스매니저합친구역-----------------

        //큐시스템//특정명령을 수행하기위한 시스템//좀더체크후 다른곳으로 분리하기//20220328
        [Space]
        [Header("AI명령 큐시스템")]
        //최고높은큐
        public CommandQueue highCommandQueue = new CommandQueue();
        public AIGuideQueue highGuideStateQueue = new AIGuideQueue();

        //일반큐
        public CommandQueue middleCommandQueue = new CommandQueue();
        public AIGuideQueue middleGuideStateQueue = new AIGuideQueue();

        //제일낮은큐
        public CommandQueue lowCommandQueue = new CommandQueue();
        public AIGuideQueue lowGuideStateQueue = new AIGuideQueue();

        //큐시스템
        [System.Serializable] public class CommandQueue : QueueEventModule<TestWorldStructureObject> { }
        [System.Serializable] public class AIGuideQueue : QueueEventModule<BuildingConstructionType> { }


        protected override void Awake()
        {
            base.Awake();

            //카테고리 버튼마다 세팅
            frameButton.onClick.AddListener(delegate { CreateBuildingButtonFunc(frameButton, BuildingType.Frame); });
            floorButton.onClick.AddListener(delegate { CreateBuildingButtonFunc(floorButton, BuildingType.Floor); });
            wallButton.onClick.AddListener(delegate { CreateBuildingButtonFunc(wallButton, BuildingType.Wall); });
            doorButton.onClick.AddListener(delegate { CreateBuildingButtonFunc(doorButton, BuildingType.Door); });
            buildingButton.onClick.AddListener(delegate { CreateBuildingButtonFunc(buildingButton, BuildingType.Building); });
            pipeButton.onClick.AddListener(delegate { CreateBuildingButtonFunc(pipeButton, BuildingType.Pipe); });
        }

        /// <summary>
        /// 빌딩매니저 업데이트
        /// </summary>
        public void UpdateBuildingManager()
        {
            //if (Input.GetKeyDown(PlayerInPutKeySetting.Instance.BuildingConstructModeKeyBible[ECustomKeyCode.FixModeKey]))
            //{
            //    BuildingPointer.Instance.ResetBuildingPointer();
            //    targetBuildingConstructionModeType = BuildingConstructionType.Fix;
            //    BuildingPointer.Instance.InitBuildingPointer(null, targetBuildingConstructionModeType);
            //}
            //else if (Input.GetKeyDown(PlayerInPutKeySetting.Instance.BuildingConstructModeKeyBible[ECustomKeyCode.DismantleBuildingModeKey]))
            //{
            //    BuildingPointer.Instance.ResetBuildingPointer();
            //    targetBuildingConstructionModeType = BuildingConstructionType.BuildingDismantle;
            //    BuildingPointer.Instance.InitBuildingPointer(null, targetBuildingConstructionModeType);
            //}
            //else if (Input.GetKeyDown(PlayerInPutKeySetting.Instance.BuildingConstructModeKeyBible[ECustomKeyCode.DismantleFloorModeKey]))
            //{
            //    BuildingPointer.Instance.ResetBuildingPointer();
            //    targetBuildingConstructionModeType = BuildingConstructionType.FloorDismantle;
            //    BuildingPointer.Instance.InitBuildingPointer(null, targetBuildingConstructionModeType);
            //}
            //else if (Input.GetKeyDown(PlayerInPutKeySetting.Instance.BuildingConstructModeKeyBible[ECustomKeyCode.ConnectModeKey]))
            //{
            //    BuildingPointer.Instance.ResetBuildingPointer();
            //    targetBuildingConstructionModeType = BuildingConstructionType.Connect;
            //    BuildingPointer.Instance.InitBuildingPointer(null, targetBuildingConstructionModeType);
            //}


            //ECustomKeyCode[] eCustomKeyCodes = PlayerInPutKeySetting.Instance.buildingGroupSelectKeys;
            //for (int i = 0; i < eCustomKeyCodes.Length; i++)
            //{
            //    if (Input.GetKeyDown(PlayerInPutKeySetting.Instance.BuildingConstructModeKeyBible[eCustomKeyCodes[i]]))
            //    {
            //        switch (i)
            //        {
            //            case 0:
            //                frameButton.SelectButton();
            //                break;
            //            case 1:
            //                floorButton.SelectButton();
            //                break;
            //            case 2:
            //                wallButton.SelectButton();
            //                break;
            //            case 3:
            //                doorButton.SelectButton();
            //                break;
            //            case 4:
            //                buildingButton.SelectButton();
            //                break;
            //            case 5:
            //                pipeButton.SelectButton();
            //                break;
            //        }
            //    }
            //}
        }

        /// <summary>
        /// 건설로직 세팅해주는 함수. 게임할때의 세팅을 바꿀수 있음
        /// </summary>
        /// <param name="buildLogicType">건설로직타입</param>
        /// <param name="playerUnitObject">플레이어가 조종하는 유닛</param>
        /// <param name="distance">거리</param>
        public void SetBuildLogicTypeSetting(BuildLogicType buildLogicType, Transform playerUnitObject, float distance, BasementTileMap basementTileMap)
        {
            //건축모드            
            targetBuildingLogicType = buildLogicType;

            //건축모드간의 상호작용할 유닛
            targetPlayerObject = playerUnitObject;
            targetDistance = distance;

            //건설할 타일맵
            targetBasementTileMap = basementTileMap;
        }

        /// <summary>
        /// 빌딩매니저 보여주기
        /// </summary>
        public void ShowBuildingManager()
        {   
            //폴에 있는모든버튼끄기
            //buildingCreateButtonPool.AllObjectDeActive();
            ShowUIView();
        }

        /// <summary>
        /// 빌딩매니저 닫기
        /// </summary>
        public void OffBuildingManager()
        {
            OffUIView();
        }

        /// <summary>
        ///빌딩CAT 버튼푸쉬시 기능
        /// </summary>
        /// <param name="targetButton">버튼</param>
        /// <param name="buildingType">건물분류타입</param>
        private void CreateBuildingButtonFunc(Button targetButton, BuildingType buildingType)
        {
            //첫번째 하위오브젝트상태에 따라 켜고킴            
            if (!isOn)
            {
                //꺼져있으면 보여주기
                ShowBuildingButton(targetButton.transform, buildingType);
                isOn = true;
                return;
            }

            //켜져있으면
            //일단끄기
            //폴에 있는모든버튼끄기
            //buildingCreateButtonPool.AllObjectDeActive();
            isOn = false;

            //비교후 다르면
            if (buildingType != isOnType)
            {
                //켜기
                ShowBuildingButton(targetButton.transform, buildingType);
                isOnType = buildingType;
                isOn = true;
            }
        }
        

        /// <summary>
        /// 카테고리에 맞는 잠금해제된 빌딩버튼을 보여줌
        /// </summary>
        /// <param name="targetPos">타겟이 될 부모위치</param>
        /// <param name="buildingType">건물분류타입</param>
        private void ShowBuildingButton(Transform targetPos, BuildingType buildingType)
        {
            BuildingCreateButton targetBtn = null;
            //잠금해제된데이터인지 체크
            for (int i = 0; i < unLockBuildingDataList.Count; i++)
            {
                //해제된 데이터이면
                if (buildingType == unLockBuildingDataList[i].buildingType)
                {
                    targetBtn = buildingCreateButtonPool.RequestPrefab();
                    targetBtn.SetBuildingCreateButton(unLockBuildingDataList[i], BuildingConstructionType.Build);
                    targetBtn.transform.SetParent(targetPos);
                    targetBtn.gameObject.SetActive(true);
                }
            }

            //수리버튼마지막의 4번째에 집어넣기
            targetBtn = buildingCreateButtonPool.RequestPrefab();
            targetBtn.SetBuildingCreateButton(null, BuildingConstructionType.Fix);
            targetBtn.transform.SetParent(targetPos);
            targetBtn.gameObject.SetActive(true);

            //해제버튼마지막의 3번째에 집어넣기
            targetBtn = buildingCreateButtonPool.RequestPrefab();
            targetBtn.SetBuildingCreateButton(null, BuildingConstructionType.BuildingDismantle);
            targetBtn.transform.SetParent(targetPos);
            targetBtn.gameObject.SetActive(true);

            //해제버튼마지막의 2번째에 집어넣기
            targetBtn = buildingCreateButtonPool.RequestPrefab();
            targetBtn.SetBuildingCreateButton(null, BuildingConstructionType.FloorDismantle);
            targetBtn.transform.SetParent(targetPos);
            targetBtn.gameObject.SetActive(true);

            //연결버튼마지막에 집어넣기
            targetBtn = buildingCreateButtonPool.RequestPrefab();
            targetBtn.SetBuildingCreateButton(null, BuildingConstructionType.Connect);
            targetBtn.transform.SetParent(targetPos);
            targetBtn.gameObject.SetActive(true);
        }

        /// <summary>
        /// 현재 건설된 건물데이터를 등록해주는 함수
        /// </summary>
        /// <param name="buildData">건물데이터</param>
        public void AddConstructBuildingData(BuildingObjectScript buildData)
        {
            if (currentConstructBuildingDataBible.ContainsKey(buildData))
            {
                //존재하면
                //추가하기
                currentConstructBuildingDataBible[buildData]++;
            }
            else
            {
                //존재하지않으면
                //하나등록하기
                currentConstructBuildingDataBible.Add(buildData, 1);
            }
        }

        /// <summary>
        /// 현재 건설된 건물데이터를 등록해제하는 함수
        /// </summary>
        /// <param name="buildData">건물데이터</param>
        public void RemoveConstructBuildingData(BuildingObjectScript buildData)
        {
            if (currentConstructBuildingDataBible.ContainsKey(buildData))
            {
                //존재하면
                //빼기//0보다 작으면 안됨
                currentConstructBuildingDataBible[buildData] = currentConstructBuildingDataBible[buildData] < 0 ? 0 : currentConstructBuildingDataBible[buildData]--;
            }
            else
            {
                //존재하지않으면
                //등록만 해줌
                currentConstructBuildingDataBible.Add(buildData, 0);
            }
        }

        /// <summary>
        /// 잠금해제된 건물데이터를 등록하는함수
        /// </summary>
        /// <param name="buildData">건물데이터</param>
        public void AddUnLockBuildingData(BuildingObjectScript buildData)
        {
            if (!unLockBuildingDataList.Contains(buildData))
            {
                unLockBuildingDataList.Add(buildData);
            }
        }

        /// <summary>
        /// 잠금해제된 건물데이터를 빼는함수
        /// </summary>
        /// <param name="buildData">건물데이터</param>
        public void RemoveUnLockBuildingDataList(BuildingObjectScript buildData)
        {
            if (unLockBuildingDataList.Contains(buildData))
            {
                unLockBuildingDataList.Remove(buildData);
            }
        }


        //-------------------------------------------------------------
        //빌딩포인터와상호작용하는 함수관련
        //-------------------------------------------------------------

        /// <summary>
        /// 건물데이터에 있는 추가 필터를 확인하여 잠금여부를 확인해주는 함수
        /// </summary>
        /// <param name="buildingData">빌딩데이터</param>
        /// <returns>잠금여부</returns>
        public bool CheckCreateLockBuildingData(BuildingObjectScript buildingData)
        {
            bool isLock = false;
            if (buildingData.checkBuildings.Length != 0)
            {
                for (int i = 0; i < buildingData.checkBuildings.Length; i++)
                {
                    if (currentConstructBuildingDataBible.ContainsKey(buildingData.checkBuildings[i]))
                    {
                        //존재하면
                        //수량체크
                        switch (buildingData.checkComparisonOperatorTypes[i])
                        {
                            case ComparisonOperatorType.Greater:
                                isLock = buildingData.checkBuildAmounts[i] >= currentConstructBuildingDataBible[buildingData];
                                break;
                            case ComparisonOperatorType.Less:
                                isLock = buildingData.checkBuildAmounts[i] <= currentConstructBuildingDataBible[buildingData];
                                break;
                            case ComparisonOperatorType.Equal:
                                isLock = buildingData.checkBuildAmounts[i] != currentConstructBuildingDataBible[buildingData];
                                break;
                        }
                        if (isLock)
                        {
                            break;
                        }
                    }
                    else
                    {
                        //존재안하면 잠궈버리기
                        isLock = true;
                        break;
                    }
                }
            }
            return !isLock;
        }

        //리소스매니저 함수위치
        //자원아이템이 있는지 요청
        //체크후 있으면 리소스매니저에 세팅해버림
        //외부에서 사용하는 함수

        /// <summary>
        /// 자원아이템이 존재하는지 체크하는함수
        /// </summary>
        /// <param name="itemDatas">아이템데이터들</param>
        /// <param name="itemCounts">아이템수량들</param>
        /// <returns></returns>
        public bool RequestExistItemResource(/*ItemObjectScript[] itemDatas, int[] itemCounts*/)
        {
            bool isDone = true;
            //for (int i = 0; i < itemDatas.Length; i++)
            //{
            //    isDone = InventoryManager.Instance.CheckExistItemDataToInventory(targetMainInventory, itemDatas[i], itemCounts[i]);
            //    if (!isDone)
            //    {
            //        return false;
            //    }
            //}
            return isDone;
        }

        //참조할 메인인벤토리 세팅 함수
        //외부에서 사용하는 함수
        //public void SetMainResourceInventory(Inventory _inventory)
        //{
        //    targetMainInventory = _inventory;
        //}

        //참조한 인벤토리슬롯과 참조한 빌딩데이터를 계산하는 함수
        //빌딩포인터에서 작업
        //외부에서 사용하는 함수
        public void Deal(BuildingObjectScript _targetBuildingData)
        {
            bool check = false;
            bool[] checkReturnArray = new bool[_targetBuildingData.resourceDataArray.Length];
            for (int i = 0; i < _targetBuildingData.resourceDataArray.Length; i++)
            {
                //InventoryManager.Instance.DealItemDataToInventory(targetMainInventory, _targetBuildingData.resourceDatas[i], _targetBuildingData.resourceNeedAmounts[i], ref check);
                //checkReturnArray[i] = check;

                //if (!check)
                //{
                //    //결제실패할시
                //    //지금까지 결제한것들을 돌려줌
                //    for (int j = 0; j < i; j++)
                //    {
                //        InventoryManager.Instance.AddItemDataToInventory(targetMainInventory, _targetBuildingData.resourceDatas[j], _targetBuildingData.resourceNeedAmounts[j]);
                //    }
                //    break;
                //}
            }
        }
        //취소시키는 함수
        //골조구조에서 사용하는 함수
        //외부에서 사용하는 함수
        public void CancelDeal(BuildingObjectScript targetBuildingData, Vector2 pos, Transform parent)
        {
            //맵->공간->구조물
            //맵에 생성


        }

        private static Vector2 GetRandomSpawnPosition(Transform target)
        {   
            Vector2 RespawnPos;
            //월드포지션을 리턴시켜줌
            float x = Random.Range(target.position.x - 0.5f, target.position.x + 0.5f);
            float y = Random.Range(target.position.y - 0.5f, target.position.y + 0.5f);
            RespawnPos.x = x;
            RespawnPos.y = y;
            return RespawnPos;
        }

        //큐함수위치

        /// <summary>
        /// 현재 예약된 큐가 있는지 확인하는 함수
        /// </summary>
        /// <returns>존재여부 </returns>
        public bool GetIsBuildingQueueEvent()
        {
            if (highCommandQueue.Count > 0)
            {
                return true;
            }

            if (middleCommandQueue.Count > 0)
            {
                return true;
            }

            if (lowCommandQueue.Count > 0)
            {
                return true;
            }
            return false;
        }

        //큐에 들어간 특정 명령의 중요도를 업하는 함수
        public void ImportantUpQueue()
        {

        }

        //큐에 들어간 특정 명령의 중요도를 다운하는 함수
        public void ImportantDownQueue()
        {

        }

    }

    /// <summary>
    /// 골조구조물 작동타입
    /// </summary>
    public enum SkeletonStructureType
    {
        Time,
        Amount
    }

    /// <summary>
    /// 건물분류타입
    /// </summary>
    public enum BuildingType
    {
        Frame,//몸체 동체 HULL
        Floor,//바닥
        Wall,//벽
        Door,//문
        Building,//건물//골조        
        Pipe,//파이프
        PipeBridge,//파이프브릿지
    }

    /// <summary>
    /// 트리거 사용시 비교연산자 타입
    /// </summary>
    public enum ComparisonOperatorType
    {
        Greater,//크다
                //크고같다,//greater than or equal
        Less,//작다
             //작고같다,//less than or equal
        Equal,//같다
              //다르다,//not equal
    }

    

    /// <summary>
    /// 건물 건축타입
    /// </summary>
    public enum BuildingConstructionType
    {
        Build,//건설
        BuildingDismantle,//해체
        FloorDismantle,//해체
        Fix,//수리
        Connect,//연결
    }

    /// <summary>
    /// 건설 로직타입
    /// </summary>
    public enum BuildLogicType
    {
        Character,//플레이어컨트롤유닛을 기준삼아 특정거리내에 건설
        RTS,//화면상에 마우스클릭하여 건설
        DEV,//개발자모드
    }
}

#endif
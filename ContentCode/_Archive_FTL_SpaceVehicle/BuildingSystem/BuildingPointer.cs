//using UnityEngine;
//using lLCroweTool.Singleton;
//using System.Collections.Generic;
//using System;
//using Shapes;
//using lLCroweTool.LogSystem;
//using UnityEngine.Tilemaps;
//using lLCroweTool.TerrainSystem.BasementTileMap;
//using lLCroweTool.BuildingSystem.SignalSystem;
//using lLCroweTool.BuildingSystem.FuncStructure;
//using lLCroweTool.WorldObjectSystem;
//using lLCroweTool.DestroyManger;


//namespace lLCroweTool.BuildingSystem
//{
//    [RequireComponent(typeof(CircleCollider2D))]
//    [RequireComponent(typeof(Rigidbody2D))]
//    public class BuildingPointer : MonoBehaviourSingleton<BuildingPointer>
//    {
//        //스킬포인터와 비슷한 역할
//        //빌딩포인터
//        //빌딩포인터는 데이터를 가져와서 
//        //어디에 빌딩을 건설할지 정할 포인터이다.
//        //OxygenLayer에서만 충돌한다.=>빌딩은 어덯게 체크하는가
//        //==>밑에의 친구를 찾는다
//        //콜라이더는 트리거
//        /// <summary>
//        /// 빌딩포인터의 레이어는 voidRoom이 존재하는 레이어와 연동됨
//        /// 빌딩포인터컴포넌트를 가진 게임오브젝트도 같은 레이어로 사용됨
//        /// </summary>


//        //20220215개선
//        //1. RTS셀렉션박스와 빌딩 셀렉션박스를 개선시키기
//        //2. 건물미리보기 제작
//        //확인사항
//        //건물마다 다르지만
//        //2-1. 쭈욱해서 제작가능여부 + 가능하면 박스 or 막대형태로만 선택//건물타입에 따라 변경//개발자모드해도 동일하게
//        //3. 송신기 수신기 연결메커니즘 추가
//        //=>1차타겟 2차타겟
//        //4.캐릭터용빌드시스템제작후 차량을 수동으로 조립가능하게
//        //5.작동구조체크
//        //6.회전키
//        //7.스프라이트랜더러 마우스포인터체크

//        //20220228개선
//        //타일맵으로 변경
//        //오브젝트최대한 적게 쓰게 최적화

//        //20221011 추가 개선을 할수 있음
//        //메터리얼을 사용해서 투명하게 처리가능
//        //그리더에셋을 사용하기때문에 더이상 여기서 처리는 안할가능성이 존재

//        [Header("빌딩포인터 설정")]
//        public Rectangle selectBox;//선택박스
//        public SpriteRenderer buildingPointerRenderer;//빌딩포인터스프라이트랜더러오브젝트//현재오브젝트에 붙어있음//블루프린터용도
//        public Line pipeLine;//파이프표시라인

//        //빌딩시 프리팹을 블루프린터용도로 사용할때 필요한 것
//        private TestWorldObject bluePrintWorldObject;//현재 만들 빌딩
//        private Color[] bluePrintColorArray;//투명도 조절용
//        private List<Collider2D> buildingCollider2DList = new List<Collider2D>();//건설할건축물들의 콜라이더를 현빌딩포인터에 세팅후 체크하고 정리할용도//체크용도로 쓰이고 다쓰면 없앰
//        public bool isFindCollider = false;//콜라이더를 찾았는지//건축물 설치가능구역//false여야 지을수 있음
//        private List<TestWorldObject> checkCol2DWorldObjectList = new List<TestWorldObject>();//어떤한것을 체크했는 지확인용도//트리거했을시 걸리는//빌딩확인여부
//        private bool isChangeColor = false;//업데이트에서 가능한여부를 컬러로 표시할때 체크하는 용도

//        private Vector2 startMousePosition;//마우스시작다운클릭 위치
//        private Vector2 endMousePosition;//마우스마지막업 클릭위치
//        private Vector2 colliderBoxSize;//계산된 드래그한 박스사이즈
//        private Vector2 boxMousePos;//박스의 마우스위치
//        private bool check;//체크사항확인하는용도

//        //빌딩할시 필요한 것        
//        public BuildingObjectScript targetBuildingData;//빌딩데이터
//        private float buildRotationAngle = 0;//회전값
//        public ContactFilter2D worldLayer;//건축물을 건설할떄 체크할 월드레이어

//        private PipeStructureObject targetBuildPipeObject;//타겟팅할 건설용파이프오브젝트//프리팹상태라 따로 인스턴스해줘야됨
//        private PipeBridgeStructureObject selectPipeBridgeObject;//첫번째로 선택한 파이프브릿지
//        //public ContactFilter2D pipeLayer;//파이프and파이프브릿지체크용

//        private SignalTransmitter selectSignalTransmitter;//첫번째로 선택한 신호송신기
//        private SignalReceiver selectSignalReceiver;//첫번째로 선택한 신호수신기
//        public ContactFilter2D signalBuildingLayer; //건물레이어체크//송신기수신기체크용
//        private SignalTransmitter refSignalTransmitter;//리퍼용송신기
//        private SignalReceiver refSignalReceiver;//리퍼용수신기


//        public BasementTileMap targetBasementTileMap;//전초기지타일맵//박스콜라이더트리거로 처리//자동타겟팅
//        public ContactFilter2D basementTileMapLayer;//전초기지타일맵레이어레이어

//        private List<Collider2D> col2dList = new List<Collider2D>();

//        protected override void Awake()
//        {
//            base.Awake();
//            BluePrintObjectChangeColor(isChangeColor);
//            gameObject.SetActive(false);
//            TryGetComponent(out Rigidbody2D rb2d);
//            rb2d.isKinematic = true;

//            gameObject.layer = worldLayer.layerMask;

//            var temp = typeof(BuildingPointer);
//            LogManager.Register(temp, temp.Name, true, true);
//        }        

//        /// <summary>
//        /// 빌딩포인터 초기화
//        /// </summary>
//        /// <param name="buildingData">건물데이터</param>
//        /// <param name="buildingConstructionType">건물 건축타입</param>
//        public void InitBuildingPointer(BuildingObjectScript buildingData, BuildingConstructionType buildingConstructionType)
//        {           
//            targetBuildingData = buildingData;
//            //회전초기화
//            RotateBlueBuilding(0);
//            CircleCollider2D targetCol2D = null;

//            switch (buildingConstructionType)
//            {
//                case BuildingConstructionType.Build:

//                    switch (targetBuildingData.buildingType)
//                    {
//                        case BuildingType.Frame:
//                        case BuildingType.Floor:
//                        case BuildingType.Wall:
//                        case BuildingType.Door:
//                            buildingPointerRenderer.sprite = targetBuildingData.bluePrintBuildingImage;
//                            break;                        
//                        case BuildingType.Building:
//                        case BuildingType.PipeBridge:
//                            //오브젝트생성후 이미지와 콜라이더를 건드려서 블루프린트느낌과 충돌체크확인용으로
//                            bluePrintWorldObject = Instantiate(targetBuildingData.buildingObject, transform);
//                            if (bluePrintWorldObject.unitStatus != null)
//                            {
//                                bluePrintWorldObject.unitStatus.GetTimerModule().enabled = false;
//                            }

//                            //컬러들만 가져오기
//                            Renderer[] renderers = bluePrintWorldObject.GetComponentsInChildren<Renderer>();
//                            List<Color> colorList = new List<Color>();
//                            for (int i = 0; i < renderers.Length; i++)
//                            {
//                                if (renderers[i].TryGetComponent(out IEquatable<Color> target))
//                                {
//                                    colorList.Add((Color)target);
//                                }
//                            }
//                            bluePrintColorArray = colorList.ToArray();

//                            //기존콜라이더 오프//기존콜라이더복사
//                            Collider2D[] temps = bluePrintWorldObject.GetComponentsInChildren<Collider2D>();
//                            for (int i = 0; i < temps.Length; i++)
//                            {
//                                temps[i].enabled = false;
                                
//                                //각콜라이더2D를 하나 생성후 트리거변환
//                                if (temps[i].TryGetComponent(out BoxCollider2D box2D))
//                                {
//                                    box2D = lLcroweUtil.GetCopyOf(box2D, gameObject);
//                                    box2D.isTrigger = true;
//                                    buildingCollider2DList.Add(box2D);
//                                }
//                                else if(temps[i].TryGetComponent(out CircleCollider2D cir2D))
//                                {
//                                    cir2D = lLcroweUtil.GetCopyOf(cir2D, gameObject);
//                                    cir2D.isTrigger = true;
//                                    buildingCollider2DList.Add(cir2D);
//                                }
//                                else if (temps[i].TryGetComponent(out CapsuleCollider2D cap2D))
//                                {
//                                    cap2D = lLcroweUtil.GetCopyOf(cap2D, gameObject);
//                                    cap2D.isTrigger = true;
//                                    buildingCollider2DList.Add(cap2D);
//                                }
//                                else if (temps[i].TryGetComponent(out PolygonCollider2D poly2D))
//                                {
//                                    poly2D = lLcroweUtil.GetCopyOf(poly2D, gameObject);
//                                    poly2D.isTrigger = true;
//                                    buildingCollider2DList.Add(poly2D);
//                                }
//                            }
//                            break;
//                        case BuildingType.Pipe:
//                            if (buildingData.buildingObject.TryGetComponent(out targetBuildPipeObject))
//                            {
//                                buildingPointerRenderer.sprite = targetBuildingData.bluePrintBuildingImage;
//                                targetCol2D = gameObject.AddComponent<CircleCollider2D>();
//                                targetCol2D.radius = 0.5f;
//                                targetCol2D.isTrigger = true;
//                                buildingCollider2DList.Add(targetCol2D);
//                            }
//                            else
//                            {
//                                //작동안함
//                                return;
//                            }
//                            break;
//                    }
//                    break;
//                case BuildingConstructionType.BuildingDismantle:
//                    //건물미리보기 이미지를 사용하여 블루프린트느낌이 나게함                    
//                    buildingPointerRenderer.sprite = BuildingManager.Instance.dismantleBuildingImage;
//                    targetCol2D = gameObject.AddComponent<CircleCollider2D>();
//                    targetCol2D.radius = 0.5f;
//                    targetCol2D.isTrigger = true;
//                    buildingCollider2DList.Add(targetCol2D);
//                    break;
//                case BuildingConstructionType.FloorDismantle:
//                    //건물미리보기 이미지를 사용하여 블루프린트느낌이 나게함                    
//                    buildingPointerRenderer.sprite = BuildingManager.Instance.dismantleFloorImage;
//                    targetCol2D = gameObject.AddComponent<CircleCollider2D>();
//                    targetCol2D.radius = 0.5f;
//                    targetCol2D.isTrigger = true;
//                    buildingCollider2DList.Add(targetCol2D);
//                    break;
//                case BuildingConstructionType.Fix:
//                    //건물미리보기 이미지를 사용하여 블루프린트느낌이 나게함
//                    buildingPointerRenderer.sprite = BuildingManager.Instance.fixImage;
//                    targetCol2D = gameObject.AddComponent<CircleCollider2D>();
//                    targetCol2D.radius = 0.5f;
//                    targetCol2D.isTrigger = true;
//                    buildingCollider2DList.Add(targetCol2D);
//                    break;                
//                case BuildingConstructionType.Connect:
//                    //건물미리보기 이미지를 사용하여 블루프린트느낌이 나게함                    
//                    buildingPointerRenderer.sprite = BuildingManager.Instance.connectImage;
//                    //targetCol2D = gameObject.AddComponent<CircleCollider2D>();
//                    //targetCol2D.radius = 0.5f;
//                    //targetCol2D.isTrigger = true;
//                    //buildingCollider2DList.Add(targetCol2D);
//                    break;
//            }
//            SetBluePrintAlphaColor(0.7f);
//            transform.position = MousePointer.Instance.mouseWorldPosition;
//            gameObject.SetActive(true);
//        }

//        /// <summary>
//        /// 빌딩포인터 리셋
//        /// </summary>
//        public void ResetBuildingPointer()
//        {
//            bluePrintColorArray = null;
//            if (!ReferenceEquals(bluePrintWorldObject , null)) 
//            {
//                DestroyManager.Instance.AddDestoryGameObject(bluePrintWorldObject.gameObject);
//                bluePrintWorldObject = null;
//            }
//            targetBuildingData = null;
//            targetBasementTileMap = null;
//            gameObject.SetActive(false);
//            buildingPointerRenderer.sprite = null;

//            //콜라이더처리
//            Collider2D[] tempArray = buildingCollider2DList.ToArray();
//            for (int i = 0; i < tempArray.Length; i++)
//            {
//                Destroy(tempArray[i]);                
//            }
//            buildingCollider2DList.Clear();
//            checkCol2DWorldObjectList.Clear();

//            targetBuildPipeObject = null;
//            pipeLine.gameObject.SetActive(false);
//            selectBox.gameObject.SetActive(false);
//            selectSignalReceiver = null;
//            selectSignalTransmitter = null;
//        }
        
//        void Update()
//        {
//            //취소체크
//            if (Input.GetKeyDown(PlayerInPutKeySetting.Instance.NormalKeyBible[ECustomKeyCode.MouseRightButton]))
//            {
//                switch (BuildingManager.Instance.targetBuildingConstructionModeType)
//                {
//                    case BuildingConstructionType.Build:
//                        switch (targetBuildingData.buildingType)
//                        {
//                            case BuildingType.Frame:
//                            case BuildingType.Floor:
//                            case BuildingType.Wall:
//                            case BuildingType.Door:
//                            case BuildingType.Building:
//                            case BuildingType.PipeBridge:
//                                ResetBuildingPointer();
//                                break;
//                            case BuildingType.Pipe:
//                                if (selectPipeBridgeObject)
//                                {
//                                    //연결점 해제                                    
//                                    selectPipeBridgeObject = null;
//                                    //취소사운드
//                                }
//                                else
//                                {
                                    
//                                    ResetBuildingPointer();
//                                    //취소사운드
//                                }
//                                pipeLine.gameObject.SetActive(false);
//                                break;
//                        }

//                        break;
//                    case BuildingConstructionType.BuildingDismantle:
//                    case BuildingConstructionType.FloorDismantle:
//                    case BuildingConstructionType.Fix:
//                        ResetBuildingPointer();
//                        //취소사운드
//                        break;
//                    case BuildingConstructionType.Connect:
//                        if (selectSignalReceiver || selectSignalTransmitter)
//                        {
//                            selectSignalReceiver = null;
//                            selectSignalTransmitter = null;
//                            //취소사운드
//                        }
//                        else
//                        {
//                            ResetBuildingPointer();
//                            //취소사운드
//                        }
//                        break;
//                }
//            }

//            //UI위에있는지 체크            
//            if (CheckOnCanvas.onUIPanel)
//            {
//                return;
//            }

//            //타일맵체크
//            if (Physics2D.OverlapCircle(transform.position, 0.5f, basementTileMapLayer, col2dList) == 0)
//            {
//                return;
//            }

//            for (int i = 0; i < col2dList.Count; i++)
//            {
//                if (col2dList[i].TryGetComponent(out targetBasementTileMap))
//                {
//                    //존재하면
//                    break;
//                }
//            }

//            if (ReferenceEquals(targetBasementTileMap, null))
//            {
//                //경고메세지보내기
//                if (Input.GetKeyDown(PlayerInPutKeySetting.Instance.NormalKeyBible[ECustomKeyCode.MouseLeftButton]))
//                {
//                    LogManager.Log(typeof(BuildingPointer), "타일맵이 존재하지않습니다.", gameObject, LogManager.LogType.Waring);
//                    //NoticeUIManager.Instance.ShowNotice("NotInBasement", NoticeUIManager.NoticeType.Info);
//                }
//                return;
//            }

//            //마우스버튼클릭 시작위치
//            if (Input.GetKeyDown(PlayerInPutKeySetting.Instance.NormalKeyBible[ECustomKeyCode.MouseLeftButton]))
//            {
//                startMousePosition = MousePointer.Instance.mouseWorldPosition;
//            }

//            //회전과 선택박스드로잉
//            switch (BuildingManager.Instance.targetBuildingConstructionModeType)
//            {
//                case BuildingConstructionType.Build:
//                    switch (targetBuildingData.buildingType)
//                    {
//                        case BuildingType.Frame:
//                        case BuildingType.Floor:
//                            InputKeyDrawSelectBox(false);
//                            break;
//                        case BuildingType.Wall:
//                            InputKeyDrawSelectBox(true);
//                            break;
//                        case BuildingType.Door:
//                        case BuildingType.Building:
//                            InputKeyRotateBluePrint();
//                            break;
//                        case BuildingType.PipeBridge:
//                        case BuildingType.Pipe:
//                            //아무행동없음
//                            break;
//                    }
//                    break;
//                case BuildingConstructionType.BuildingDismantle:
//                case BuildingConstructionType.FloorDismantle:
//                case BuildingConstructionType.Fix:                
//                case BuildingConstructionType.Connect:
//                    //아무행동없음
//                    break;
//            }

//            //건물 건축모드로직 시작
//            switch (BuildingManager.Instance.targetBuildingLogicType)
//            {
//                case BuildLogicType.Character:
//                    //거리체크//일정거리에서멈춤
//                    //check = Vector2.Distance(bluePrintWorldObject.transform.position, BuildingManager.Instance.targetPlayerObject.position) <= BuildingManager.Instance.targetDistance;
//                    //if (!check)
//                    //{
//                    //    //컬러변경
//                    //    BluePrintObjectChangeColor(!check);
//                    //    return;                       
//                    //}
//                    //빌딩포인터위치 변경//일정거리내에서만 움직임
//                    transform.position = MousePointer.Instance.mouseWorldPosition;
//                    LimitMoveObjectToFixedObject(BuildingManager.Instance.targetPlayerObject, transform, BuildingManager.Instance.targetDistance);
//                    switch (targetBuildingData.buildingType)
//                    {
//                        case BuildingType.Frame:
//                        case BuildingType.Floor:
//                        case BuildingType.Wall:
//                        case BuildingType.Door:
//                            //거리체크
//                            if (lLcroweUtil.CheckDistance(BuildingManager.Instance.targetPlayerObject.position, transform.position, BuildingManager.Instance.targetDistance))
//                            {
//                                //스냅
//                                lLcroweUtil.SnapPosToGridPos(transform.position, targetBasementTileMap.GetTileMap());
//                            }
//                            break;
//                        case BuildingType.Building:
//                        case BuildingType.PipeBridge:
//                            break;
//                        case BuildingType.Pipe:
//                            if (selectPipeBridgeObject != null)
//                            {
//                                pipeLine.Start = selectPipeBridgeObject.transform.position;
//                                pipeLine.End = transform.position;
//                            }
//                            break;
//                    }

//                    //건설로직작동
//                    ActionBuildingConstructMode(BuildingManager.Instance.targetBuildingConstructionModeType, false);
//                    break;
//                case BuildLogicType.RTS:
//                    //빌딩포인터 위치변경//전방위//건설할거에 따라 변경됨
//                    switch (targetBuildingData.buildingType)
//                    {
//                        case BuildingType.Frame:
//                        case BuildingType.Floor:
//                        case BuildingType.Wall:
//                        case BuildingType.Door:
//                            //스냅
//                            lLcroweUtil.SnapPosToGridPos(MousePointer.Instance.mouseWorldPosition, targetBasementTileMap.GetTileMap());
//                            break;
//                        case BuildingType.Building:
//                        case BuildingType.PipeBridge:
//                            //자유롭게
//                            break;
//                        case BuildingType.Pipe:
//                            if (selectPipeBridgeObject != null)
//                            {
//                                pipeLine.Start = selectPipeBridgeObject.transform.position;
//                                pipeLine.End = transform.position;
//                            }
//                            break;
//                    }
//                    transform.position = MousePointer.Instance.mouseWorldPosition;
//                    //건설로직작동
//                    ActionBuildingConstructMode(BuildingManager.Instance.targetBuildingConstructionModeType, false);
//                    break;
//                case BuildLogicType.DEV:
//                    //데브모드면 자원 제한없이 지을수 있다.
//                    //빌딩포인터 위치변경//전방위//건설할거에 따라 변경됨                    
//                    switch (targetBuildingData.buildingType)
//                    {
//                        case BuildingType.Frame:
//                        case BuildingType.Floor:
//                        case BuildingType.Wall:
//                        case BuildingType.Door:
//                            //스냅
//                            lLcroweUtil.SnapPosToGridPos(MousePointer.Instance.mouseWorldPosition, targetBasementTileMap.GetTileMap());
//                            break;
//                        case BuildingType.Building:                        
//                        case BuildingType.PipeBridge:
//                            //자유롭게
//                            break;
//                        case BuildingType.Pipe:
//                            if (selectPipeBridgeObject != null)
//                            {
//                                pipeLine.Start = selectPipeBridgeObject.transform.position;
//                                pipeLine.End = transform.position;
//                            }
//                            break;
//                    }
//                    transform.position = MousePointer.Instance.mouseWorldPosition;
//                    //건설로직작동
//                    ActionBuildingConstructMode(BuildingManager.Instance.targetBuildingConstructionModeType, true);
//                    break;
//            }
//        }

//        /// <summary>
//        /// 키를 눌렸을시 선택박스 그리는함수
//        /// </summary>
//        /// <param name="limitBox">제한된 박스여부</param>
//        private void InputKeyDrawSelectBox(bool limitBox)
//        {            
//            if (Input.GetKeyDown(PlayerInPutKeySetting.Instance.NormalKeyBible[ECustomKeyCode.MouseLeftButton]))
//            {
//                selectBox.gameObject.SetActive(true);
//            }

//            if (Input.GetKey(PlayerInPutKeySetting.Instance.NormalKeyBible[ECustomKeyCode.MouseLeftButton]))
//            {
//                //선택박스 그리기                
//                //아마 시작위치를 옮겨야할것같음. 빈공간에 맞게 

//                endMousePosition = transform.position;
//                //Gizmos.DrawLine(startMousePosition, endMousePosition);

//                //드래그시 작동되는 동작
//                float _startMousePosX = startMousePosition.x;
//                float _startMousePosY = startMousePosition.y;
//                float _endMousePosX = endMousePosition.x;
//                float _endMousePosY = endMousePosition.y;

//                //박스크기
//                float _boxSizeX = Mathf.Abs(_startMousePosX - _endMousePosX);
//                float _boxSizeY = Mathf.Abs(_startMousePosY - _endMousePosY);

//                //제한된 선택박스 그리기//한줄짜리 직사각형
//                if (limitBox)
//                {
//                    //가로(hor)와 세로(ver) 중 누가긴지
//                    //Debug.Log("X:" + x + ",y:" + y);
//                    //길이체크하여 어디라인이긴지
//                    //여기까지는 잘됨
//                    if (_boxSizeX > _boxSizeY)
//                    {
//                        Debug.Log("X Long");
//                        //x축이 긴가                        
//                        //y축을 고정
//                        _boxSizeY = 1;//박스사이즈
//                        _endMousePosY = _endMousePosY > _startMousePosY ? _startMousePosY + 1 : _startMousePosY - 1; //마우스위치고정
//                    }
//                    else
//                    {
//                        Debug.Log("Y Long");
//                        //y축이 긴가
//                        //x축을 고정
//                        _boxSizeX = 1;//박스사이즈
//                        _endMousePosX = _endMousePosX > _startMousePosX ? _startMousePosX + 1 : _startMousePosX - 1; //마우스위치고정
//                    }

//                }

//                //박스크기 세팅
//                colliderBoxSize = new Vector2(_boxSizeX, _boxSizeY);

//                //박스위치//중간위치
//                float newX = (_startMousePosX + _endMousePosX) / 2;
//                float newY = (_startMousePosY + _endMousePosY) / 2;
//                boxMousePos = new Vector2(newX, newY);

//                selectBox.Width = colliderBoxSize.x;
//                selectBox.Height = colliderBoxSize.y;
//                selectBox.transform.position = boxMousePos;
//            }
//            else if (Input.GetKeyUp(PlayerInPutKeySetting.Instance.NormalKeyBible[ECustomKeyCode.MouseLeftButton]))
//            {
//                selectBox.gameObject.SetActive(false);
//            }
//        }

//        /// <summary>
//        /// 타읾맵에 맞게 선택박스를 그리는 함수
//        /// </summary>
//        /// <param name="tileMap">타일맵</param>
//        /// <param name="limitBox">제한된 박스여부</param>
//        private void InputKeyDrawSelectBoxToTileMap(Tilemap tileMap ,bool limitBox)
//        {
//            if (Input.GetKeyDown(KeyCode.Mouse0))
//            {
//                selectBox.gameObject.SetActive(true);
//                startMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//            }

//            if (Input.GetKey(KeyCode.Mouse0))
//            {
//                //선택박스 그리기
//                endMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//                //Gizmos.DrawLine(startMousePosition, endMousePosition);

//                //드래그시 작동되는 동작

//                Vector2 _startPos = lLcroweUtil.SnapPosToGridPos(startMousePosition, tileMap) - (Vector2.one * 0.5f);
//                Vector2 _endPos = lLcroweUtil.SnapPosToGridPos(endMousePosition, tileMap) - (Vector2.one * 0.5f);

//                //Vector3Int _startPos = floorTileMap.GetTileMap().WorldToCell(startMousePosition);
//                //Vector3Int _endPos = floorTileMap.GetTileMap().WorldToCell(endMousePosition);

//                //드래그시 작동되는 동작
//                float _startMousePosX = _startPos.x;
//                float _startMousePosY = _startPos.y;
//                float _endMousePosX = _endPos.x;
//                float _endMousePosY = _endPos.y;

//                //박스크기
//                float _boxSizeX = Mathf.Abs(_startMousePosX - _endMousePosX);
//                float _boxSizeY = Mathf.Abs(_startMousePosY - _endMousePosY);

//                //제한된 선택박스 그리기//한줄짜리 직사각형
//                if (limitBox)
//                {
//                    //가로(hor)와 세로(ver) 중 누가긴지
//                    //Debug.Log("X:" + x + ",y:" + y);
//                    //길이체크하여 어디라인이긴지
//                    //여기까지는 잘됨
//                    if (_boxSizeX > _boxSizeY)
//                    {
//                        Debug.Log("X Long");
//                        //x축이 긴가                        
//                        //y축을 고정
//                        _boxSizeY = 1;//박스사이즈
//                        _endMousePosY = _endMousePosY > _startMousePosY ? _startMousePosY + 1 : _startMousePosY - 1; //마우스위치고정
//                    }
//                    else
//                    {
//                        Debug.Log("Y Long");
//                        //y축이 긴가
//                        //x축을 고정
//                        _boxSizeX = 1;//박스사이즈
//                        _endMousePosX = _endMousePosX > _startMousePosX ? _startMousePosX + 1 : _startMousePosX - 1; //마우스위치고정
//                    }

//                }

//                //박스크기 세팅
//                colliderBoxSize = new Vector2(_boxSizeX, _boxSizeY);

//                //박스위치//중간위치
//                float newX = (_startMousePosX + _endMousePosX) / 2;
//                float newY = (_startMousePosY + _endMousePosY) / 2;
//                boxMousePos = new Vector2(newX, newY);

//                selectBox.Width = colliderBoxSize.x;
//                selectBox.Height = colliderBoxSize.y;
//                selectBox.transform.position = boxMousePos;
//            }
//            else if (Input.GetKeyUp(KeyCode.Mouse0))
//            {
//                selectBox.gameObject.SetActive(false);
//            }
//        }

//        /// <summary>
//        /// 키를 눌렷을시 선택한 건물 회전함수
//        /// </summary>
//        private void InputKeyRotateBluePrint()
//        {
//            if (Input.GetKeyDown(PlayerInPutKeySetting.Instance.BuildingConstructModeKeyBible[ECustomKeyCode.BuildRotateRight]))
//            {
//                buildRotationAngle += 90;
//                buildRotationAngle = 360 <= buildRotationAngle ? 0 : buildRotationAngle;
//                RotateBlueBuilding(buildRotationAngle);
                
//            }
//            else if (Input.GetKeyDown(PlayerInPutKeySetting.Instance.BuildingConstructModeKeyBible[ECustomKeyCode.BuildRotateLeft]))
//            {
//                buildRotationAngle -= 90;
//                buildRotationAngle = 0 > buildRotationAngle ? 270 : buildRotationAngle;
//                RotateBlueBuilding(buildRotationAngle);
//            }
//        }

//        /// <summary>
//        /// 블루프린트빌딩을 회전하는 함수
//        /// </summary>
//        /// <param name="angle">회전할 값</param>
//        private void RotateBlueBuilding(float angle)
//        {   
//            transform.Rotate(0, 0, buildRotationAngle);
//        }

//        /// <summary>
//        /// 고정물체로부터 일정거리만 움직이게하는 함수
//        /// </summary>
//        /// <param name="fixedObject">고정될 오브젝트</param>
//        /// <param name="moveObject">움직일 오브젝트</param>
//        /// <param name="distance">최대거리</param>
//        private void LimitMoveObjectToFixedObject(Transform fixedObject, Transform moveObject, float distance)
//        {
//            //잘됨
//            Vector2 fixPoint = fixedObject.position;
//            Vector2 diff = MousePointer.Instance.mouseWorldPosition - fixPoint;//방향구하기
//            //diff.y = Mathf.Min(0f, diff.y);//y축 제한//아래방향만 봐라봄//반대로하면 위방향으로 변함//쓸데가 있으면 가져가서 쓸것
            
//            diff = Vector2.ClampMagnitude(diff, distance);//해당방향으로 최대거리 제한함      
//            moveObject.transform.position = fixPoint + diff;//고정될위치와 제한된 방향을 더함
//        }

//        /// <summary>
//        /// 건축모드에 따른 건설로직함수
//        /// </summary>
//        /// <param name="buildingConstructionType">건설 건축모드</param>
//        /// <param name="isDevMode">개발자모드</param>
//        private void ActionBuildingConstructMode(BuildingConstructionType buildingConstructionType, bool isDevMode)
//        {
//            switch (buildingConstructionType)
//            {
//                case BuildingConstructionType.Build:
//                    //건설
//                    //체크사항//데브모드가 아닐시//데브모드이면 자원쪽은 필요없음

//                    //건설가능한지 체크
//                    check = CheckIsBuilding(targetBuildingData, transform.position, targetBasementTileMap);
//                    if (!check)
//                    {
//                        //컬러변경
//                        BluePrintObjectChangeColor(!check);
//                        return;
//                    }

//                    //자원체크
//                    check = BuildingManager.Instance.RequestExistItemResource(targetBuildingData.resourceDataArray, targetBuildingData.resourceNeedAmounts);
//                    if (!check && !isDevMode)
//                    {
//                        //컬러변경
//                        BluePrintObjectChangeColor(!check);
//                        return;
//                    }
//                    BluePrintObjectChangeColor(true);

//                    //건설로직작동
//                    //필요에따라 태스크에 넣어서 작동시킬수 있게변경할것//20220306
//                    if (Input.GetKeyUp(PlayerInPutKeySetting.Instance.NormalKeyBible[ECustomKeyCode.MouseLeftButton]))
//                    {
//                        BuildBuilding(targetBuildingData, transform.position, targetBasementTileMap, isDevMode);

//                        //컨트롤키를 안누르면 빌딩포인터 종료
//                        if (!Input.GetKey(KeyCode.LeftControl))
//                        {
//                            ResetBuildingPointer();
//                        }
//                    }
//                    break;
//                case BuildingConstructionType.BuildingDismantle:
//                    //건물해체
//                    //해체가능한지 체크 
//                    check = CheckIsDismantle(transform.position);
//                    if (!check && !isDevMode)
//                    {
//                        //컬러변경
//                        BluePrintObjectChangeColor(!check);
//                        return;
//                    }
//                    BluePrintObjectChangeColor(true);

//                    //건물해체로직작동
//                    //필요에따라 태스크에 넣어서 작동시킬수 있게변경할것//20220306
//                    if (Input.GetKeyUp(PlayerInPutKeySetting.Instance.NormalKeyBible[ECustomKeyCode.MouseLeftButton]))
//                    {
//                        //감지된 충돌체들을 확인함
//                        for (int i = 0; i < checkCol2DWorldObjectList.Count; i++)
//                        {
//                            if (checkCol2DWorldObjectList[i].TryGetComponent(out TestWorldStructureObject structureObject))
//                            {
//                                //첫번째 건물오브젝트만 해체처리
//                                DismantleBuilding(structureObject, targetBasementTileMap, isDevMode);
//                                break;
//                            }
//                        }
//                    }
                  
//                    break;
//                case BuildingConstructionType.FloorDismantle:
//                    //바닥해체
//                    //해체가능한지 체크
//                    check = CheckIsDismantle(transform.position, true, targetBasementTileMap.GetTileMap());
//                    if (!check && !isDevMode)
//                    {
//                        //컬러변경
//                        BluePrintObjectChangeColor(!check);
//                        return;
//                    }
//                    BluePrintObjectChangeColor(true);

//                    //바닥해체로직작동
//                    //필요에따라 태스크에 넣어서 작동시킬수 있게변경할것//20220306
//                    if (Input.GetKeyUp(PlayerInPutKeySetting.Instance.NormalKeyBible[ECustomKeyCode.MouseLeftButton]))
//                    {
//                        DismantleFloor(transform.position, targetBasementTileMap, isDevMode);
//                    }
//                    break;
//                case BuildingConstructionType.Fix:
//                    //수리
//                    //체크사항 

//                    //자원체크
//                    check = BuildingManager.Instance.RequestExistItemResource(targetBuildingData.fixResourceDataArray, targetBuildingData.fixResourceNeedAmounts);
//                    if (!check && !isDevMode)
//                    {
//                        //컬러변경
//                        BluePrintObjectChangeColor(!check);
//                        return;
//                    }
//                    BluePrintObjectChangeColor(true);

//                    //감지된 충돌체들을 확인함 //특정구간에 건축물충돌체 찾기
//                    TestWorldObject targetObject = null;
//                    check = false;

//                    for (int i = 0; i < checkCol2DWorldObjectList.Count; i++)
//                    {
//                        if (checkCol2DWorldObjectList[i].isUseStat)
//                        {
//                            //첫번째 건물오브젝트만 수리처리
//                            if (checkCol2DWorldObjectList[i].unitStatus.unitStatusData.worldUnitType == UnitType.Mech)
//                            {
//                               //해당건물 피가 최대체력보다 낮으면 고쳐야됨
//                                if (checkCol2DWorldObjectList[i].unitStatus.GetMaxHealthPoint() > checkCol2DWorldObjectList[i].unitStatus.GetHealthPoint())
//                                {
//                                    targetObject = checkCol2DWorldObjectList[i];
//                                    check = true;
//                                    break;
//                                }
//                            }
//                        }
//                    }

//                    if (!check)
//                    {
//                        //컬러변경
//                        BluePrintObjectChangeColor(!check);
//                        return;
//                    }
//                    BluePrintObjectChangeColor(true);

//                    //수리로직작동
//                    //필요에따라 태스크에 넣어서 작동시킬수 있게변경할것//20220306
//                    if (Input.GetKeyUp(PlayerInPutKeySetting.Instance.NormalKeyBible[ECustomKeyCode.MouseLeftButton]))
//                    {
//                        UnitStatusModuleManager.Instance.UnitStatusRecoveryDamaged(100, false, targetObject.unitStatus, PlayerUnitManager.Instance.GetPlayerControlUnit());
//                    }

//                    break;
//                case BuildingConstructionType.Connect:
//                    //송신기수신기 연결
//                    //체크사항 

//                    //특정구간에 건축물충돌체 찾기//파이프라인과 커넥터//존재해야됨
//                    check = false;
//                    bool isTransmitter = false;
//                    if (Physics2D.OverlapCircle(MousePointer.Instance.mouseWorldPosition, 0.1f, signalBuildingLayer, col2dList) > 0)
//                    {
//                        for (int i = 0; i < col2dList.Count; i++)
//                        {
//                            //송신기 찾기
//                            if (col2dList[i].TryGetComponent(out refSignalTransmitter))
//                            {
//                                check = true;
//                                isTransmitter = true;
//                                transform.position = refSignalTransmitter.transform.position;
//                                break;
//                            }
//                            //수신기 찾기
//                            else if (col2dList[i].TryGetComponent(out refSignalReceiver))
//                            {
//                                check = true;
//                                transform.position = refSignalReceiver.transform.position;
//                                break;
//                            }
//                        }
//                    }
                    
//                    if (!check)
//                    {
//                        //컬러변경
//                        BluePrintObjectChangeColor(!check);
//                        return;
//                    }

//                    //송신기수신기 연결로직작동
//                    if (Input.GetKeyUp(PlayerInPutKeySetting.Instance.NormalKeyBible[ECustomKeyCode.MouseLeftButton]))
//                    {                       
//                        if (isTransmitter)
//                        {
//                            //송신기이면
//                            selectSignalTransmitter = refSignalTransmitter;
//                            //이미세팅된 송신기이면 리셋
//                            if (selectSignalTransmitter.GetIsSetSignalReceiver())
//                            {
//                                selectSignalTransmitter.SetTargetSinalReceiver(null);
//                            }
//                        }
//                        else
//                        {
//                            //수신기이면
//                            selectSignalReceiver = refSignalReceiver;
//                        }
                        
                        
//                        //송신기 수신기 양쪽다 있는지
//                        if (selectSignalTransmitter && selectSignalReceiver)
//                        {
//                            //다있으면 연결시키기
//                            //같은 신호방식인지 체크
//                            if (selectSignalTransmitter.GetSignalType() == selectSignalReceiver.GetSignalType())
//                            {
//                                //같은신호방식일시에만 연결
//                                selectSignalTransmitter.SetTargetSinalReceiver(selectSignalReceiver);
//                                //연결이 끝났으면 선택한거 리셋
//                                selectSignalTransmitter = null;
//                            }
//                            selectSignalReceiver = null;
//                        }
//                    }
//                    break;
//            }
//        }

//        /// <summary>
//        /// 건설이 가능한지 체크해주는 함수
//        /// </summary>
//        /// <param name="buildingData">빌딩데이터</param>
//        /// <param name="pos">위치</param>
//        /// <returns>가능한지 여부</returns>
//        public bool CheckIsBuilding(BuildingObjectScript buildingData, Vector2 pos, BasementTileMap basementTileMap)
//        {
//            bool isRight = false;
//            //빌드데이터의 타입에 따른 분류
//            switch (buildingData.buildingType)
//            {
//                case BuildingType.Frame:
//                case BuildingType.Floor:
//                    //바닥종류
//                    //바닥이 존재하는지 체크
//                    //바닥이 존재하지않아야지 건설가능
//                    Vector3Int floorCellPos = lLcroweUtil.GetWorldToCell(pos, basementTileMap.GetTileMap());
//                    isRight = lLcroweUtil.GetIsExistTile(floorCellPos, basementTileMap.GetTileMap());
//                    break;
//                case BuildingType.Wall:
//                case BuildingType.Door:
//                    //건축물 종류
//                    //타일맵관련이면
//                    //바닥이 존재해야 건설 가능
//                    //충돌체가 없어야됨
//                    Vector3Int buildingCellPos = lLcroweUtil.GetWorldToCell(pos, basementTileMap.GetTileMap());
//                    bool condition = lLcroweUtil.GetIsExistTile(buildingCellPos, basementTileMap.GetTileMap());//바닥존재여부
//                    if (!condition)
//                    {
//                        return isRight;
//                    }
//                    isRight = !isFindCollider;//충돌체 존재여부
//                    break;
//                case BuildingType.Building:
//                case BuildingType.PipeBridge:
//                    //건축물 종류
//                    //해당위치에 설치
//                    //트리거에 걸리는지 체크
//                    //발견된 콜라이더가 없어야지 가능함
//                    isRight = !isFindCollider;
//                    break;
//                case BuildingType.Pipe:
//                    //건축물 종류

//                    //거리체크
//                    if (selectPipeBridgeObject != null)
//                    {
//                        //최대건설거리를 가져와서 비교후 체크
//                        if (!lLcroweUtil.CheckDistance(selectPipeBridgeObject.transform.position, pos, targetBuildPipeObject.GetPipeConnectMaxDistince()))
//                        {
//                            return false;
//                        }
//                    }

//                    //트리거에 걸리는지 체크
//                    if (isFindCollider)
//                    {
//                        //특정구간에 건축물충돌체가 없어야함
//                        //건축물충돌체가 있는지
//                        for (int i = 0; i < checkCol2DWorldObjectList.Count; i++)
//                        {
//                            if (checkCol2DWorldObjectList[i].TryGetComponent(out PipeBridgeStructureObject pipeBridge))
//                            {
//                                isRight = true;
//                                break;
//                            }
//                        }
//                    }
//                    else
//                    {
//                        isRight = isFindCollider;
//                    }
//                    break;
//            }
//            return isRight;
//        }

//        /// <summary>
//        /// 해체가 가능한지 체크해주는 함수
//        /// </summary>
//        /// <param name="pos">위치</param>
//        /// <param name="isFloor">바닥체크여부</param>
//        /// <param name="tileMap">체크할 타일맵</param>
//        /// <returns>가능한지 여부</returns>
//        public bool CheckIsDismantle(Vector2 pos, bool isFloor = false, Tilemap tileMap = null)
//        {
//            bool isRight = false;
//            if (isFloor)
//            {
//                //바닥종류
//                //바닥이 존재하는지 체크
//                //바닥이 존재해야지 해체가능
//                Vector3Int floorCellPos = lLcroweUtil.GetWorldToCell(pos, tileMap);
//                isRight = lLcroweUtil.GetIsExistTile(floorCellPos, tileMap);
//            }
//            else
//            {
//                //건축물 종류
//                //트리거에 걸리는지 체크
//                if (isFindCollider)
//                {
//                    //특정구간에 건축물충돌체가 없어야함
//                    //건축물충돌체가 있는지
//                    for (int i = 0; i < checkCol2DWorldObjectList.Count; i++)
//                    {
//                        if (checkCol2DWorldObjectList[i].TryGetComponent(out TestWorldStructureObject structureObject))
//                        {
//                            isRight = true;
//                            break;
//                        }
//                    }
//                }
//                else
//                {
//                    isRight = isFindCollider;
//                }
//            }
//            return isRight;
//        }

//        /// <summary>
//        /// 건물건설함수. 빈공간레이어참조(건축모드작동시 두번쨰 작동함수)
//        /// </summary>        
//        /// <param name="_targetBuildingObject">타겟 빌딩</param>
//        /// <param name="pos">위치</param>
//        /// <param name="basementTileMap">전초기지맵</param>
//        /// <param name="isDevMode">개발자모드 여부</param>

//        public void BuildBuilding(BuildingObjectScript buildingData, Vector2 pos, BasementTileMap basementTileMap, bool isDevMode)
//        {
//            Vector3Int cellPos;            
//            TestWorldStructureObject target = null;            

//            //빌드데이터의 타입에 따른 분류
//            switch (buildingData.buildingType)
//            {
//                case BuildingType.Frame:
//                case BuildingType.Floor:
//                    //바닥종류
//                    //산소관련처리
//                    //빌딩처리
//                    cellPos = lLcroweUtil.GetWorldToCell(pos, basementTileMap.GetTileMap());                    
//                    basementTileMap.AddVoidRoomInfo(cellPos, buildingData);
//                    BuildingManager.Instance.AddConstructBuildingData(buildingData);//빌딩매니저 현재빌딩리스트에 추가//바닥은 곧장들어감
//                    break;
//                case BuildingType.Wall:
//                case BuildingType.Door:
//                    //건축물 종류
//                    //타일맵관련이면
//                    //산소관련처리
//                    cellPos = lLcroweUtil.GetWorldToCell(pos, basementTileMap.GetTileMap());
//                    basementTileMap.AddVoidRoomInfo(cellPos, buildingData);

//                    //빌딩처리                    
//                    if (buildingData.buildingWorkNeedValue > 0 && !isDevMode)
//                    {
//                        target = Instantiate(buildingData.skeletonStructure);
//                    }
//                    else
//                    {
//                        target = Instantiate(buildingData.buildingObject);
//                        BuildingManager.Instance.AddConstructBuildingData(buildingData);//빌딩매니저 현재빌딩리스트에 추가
//                    }
//                    target.SetBuildingData(buildingData);
//                    target.transform.parent = basementTileMap.transform;
//                    target.transform.position = pos;
//                    target.transform.rotation = Quaternion.AngleAxis(buildRotationAngle + basementTileMap.transform.rotation.z, Vector3.forward);
//                    break;
//                case BuildingType.Building:
//                case BuildingType.PipeBridge:
//                    //건축물 종류
//                    //해당위치에 설치
//                    //빌딩처리                                         
//                    if (buildingData.buildingWorkNeedValue > 0 && !isDevMode)
//                    {
//                        target = Instantiate(buildingData.skeletonStructure);
//                    }
//                    else
//                    {
//                        target = Instantiate(buildingData.buildingObject);                        
//                        BuildingManager.Instance.AddConstructBuildingData(buildingData);
//                    }
//                    target.SetBuildingData(buildingData);
//                    target.transform.parent = basementTileMap.transform;
//                    target.transform.position = pos;
//                    target.transform.rotation = Quaternion.AngleAxis(buildRotationAngle + basementTileMap.transform.rotation.z, Vector3.forward);
//                    break;
//                case BuildingType.Pipe:
//                    //감지된 충돌체들을 확인함
//                    for (int i = 0; i < checkCol2DWorldObjectList.Count; i++)
//                    {
//                        if (checkCol2DWorldObjectList[i].TryGetComponent(out PipeBridgeStructureObject pipeBridge))
//                        {
//                            //해당브릿지 연결시킴
//                            if (selectPipeBridgeObject == null)
//                            {
//                                //없으면 세팅
//                                selectPipeBridgeObject = pipeBridge;
//                            }
//                            else
//                            {
//                                //존재하면 각커넥터에 연결

//                                //같지않아야 연결시킴//같으면 넘어감
//                                if (selectPipeBridgeObject != pipeBridge)
//                                {
//                                    //파이프를 양쪽애 연결
//                                    PipeStructureObject targetPipe = Instantiate(targetBuildPipeObject, basementTileMap.transform);
//                                    selectPipeBridgeObject.ConnectPipe(targetPipe);//첫번쨰파이프브릿지연결
//                                    pipeBridge.ConnectPipe(targetPipe);//두번쨰파이프브릿지연결
//                                    //파이프오브젝트 비쥬얼 맞추기 구역
//                                    PipeObjectManager.Instance.RefreshPipeRenderer(targetPipe, selectPipeBridgeObject, pipeBridge);

//                                    selectPipeBridgeObject = null;
//                                }
//                            }
//                            break;
//                        }
//                    }
//                    break;
//            }

//            if (!isDevMode)
//            {
//                BuildingManager.Instance.Deal(buildingData);//자원교환
//            }
//            AstarPath.active.Scan();
//        }

//        /// <summary>
//        /// 건물해체함수
//        /// </summary>
//        /// <param name="buildingData">빌딩데이터</param>
//        /// <param name="pos">위치</param>
//        /// <param name="basementTileMap">전초기지타일맵</param>
//        /// <param name="worldStructureObject">건물오브젝트</param>
//        /// <param name="isDevMode">개발자모드</param>
//        public void DismantleBuilding(TestWorldStructureObject worldStructureObject, BasementTileMap basementTileMap, bool isDevMode)
//        {   
//            //빌드데이터의 타입에 따른 분류
//            switch (worldStructureObject.GetBuildingData().buildingType)
//            {
//                case BuildingType.Frame:
//                case BuildingType.Floor:
//                    //해당사항없음
//                    break;
//                case BuildingType.Wall:
//                case BuildingType.Door:
//                    //건축물 종류
//                    //타일맵관련이면
//                    Vector3Int cellPos = lLcroweUtil.GetWorldToCell(worldStructureObject.transform.position, basementTileMap.GetTileMap());
//                    basementTileMap.RemoveVoidRoomInfo(cellPos, worldStructureObject.GetBuildingData());

//                    //해체처리
//                    DestroyManager.Instance.AddDestoryGameObject(worldStructureObject.gameObject);
//                    break;
//                case BuildingType.Building:
//                    //건축물 종류
//                    //해체처리
//                    DestroyManager.Instance.AddDestoryGameObject(worldStructureObject.gameObject);
//                    break;
//                case BuildingType.Pipe:
//                    if (worldStructureObject.TryGetComponent(out PipeStructureObject pipe))
//                    {
//                        pipe.AllDeconnectPipeBridge();
//                        DestroyManager.Instance.AddDestoryGameObject(worldStructureObject.gameObject);
//                    }
//                    DestroyManager.Instance.AddDestoryGameObject(worldStructureObject.gameObject);
//                    break;
//                case BuildingType.PipeBridge:
//                    if (worldStructureObject.TryGetComponent(out PipeBridgeStructureObject pipeBridge))
//                    {
//                        if (!pipeBridge.isBuildingFunBridge)
//                        {
//                            pipeBridge.AllDeconnectPipe();
//                            DestroyManager.Instance.AddDestoryGameObject(worldStructureObject.gameObject);
//                        }
//                    }
//                    break;

//            }
//            BuildingManager.Instance.RemoveConstructBuildingData(targetBuildingData);
//            if (!isDevMode)
//            {
//                BuildingManager.Instance.CancelDeal(worldStructureObject.GetBuildingData(), worldStructureObject.transform.position, basementTileMap.transform);
//            }

//            AstarPath.active.Scan();
//        }

//        /// <summary>
//        /// 바닥해체함수
//        /// </summary>
//        /// <param name="pos">위치</param>
//        /// <param name="basementTileMap">해체할 전초기지</param>
//        /// <param name="isDevMode">개발자모드</param>
//        public void DismantleFloor(Vector2 pos, BasementTileMap basementTileMap, bool isDevMode)
//        {
//            Vector3Int cellPos = lLcroweUtil.GetWorldToCell(pos, basementTileMap.GetTileMap());
//            BuildingObjectScript buildingData = basementTileMap.GetVoidRoomInfo(cellPos).GetBuildingData();
//            if (buildingData == null)
//            {
//                Debug.Log("해체할 바닥. 건물데이터가 없습니다.");
//                return;
//            }

//            basementTileMap.RemoveVoidRoomInfo(cellPos, buildingData);
          
//            BuildingManager.Instance.RemoveConstructBuildingData(buildingData);
//            if (!isDevMode)
//            {
//                BuildingManager.Instance.CancelDeal(buildingData, (Vector3)pos, basementTileMap.transform);
//            }

//            AstarPath.active.Scan();
//        }


//        public void FixBuilding(TestWorldStructureObject structureObject, BasementTileMap basementTileMap, bool isDevMode)
//        {
//            //clickCollider2DList
//        }

//        public void FixFloor(Vector2 pos, BasementTileMap basementTileMap, bool isDevMode)
//        {
//            //clickCollider2DList
//        }

//        public void ConnectPipeBuilding(bool isDevMode)
//        {
//            //clickCollider2DList
//        }

//        public void ConnectSignalBuilding(bool isDevMode)
//        {
//            //clickCollider2DList
//        }

//        /// <summary>
//        /// 투명도 조절 함수
//        /// </summary>
//        /// <param name="_alpha">알파값</param>
//        private void SetBluePrintAlphaColor(float _alpha)
//        {
//            Color color;
//            for (int i = 0; i < bluePrintColorArray.Length; i++)
//            {
//                color = bluePrintColorArray[i];
//                color.a = _alpha;
//                bluePrintColorArray[i] = color;
//            }
//            color = buildingPointerRenderer.color;
//            color.a = _alpha;
//            buildingPointerRenderer.color = color;
//        }
       
//        /// <summary>
//        /// 블루프린트오브젝트 컬러변경
//        /// </summary>
//        /// <param name="isPossible">건설가능 여부</param>
//        private void BluePrintObjectChangeColor(bool isPossible)
//        {
//            if (isChangeColor ==  isPossible)
//            {
//                return;
//            }

//            isChangeColor =  isPossible;
//            Color color = isChangeColor ? Color.white : Color.red;          
//            for (int i = 0; i < bluePrintColorArray.Length; i++)
//            {
//                bluePrintColorArray[i] = color;
//            }
//            buildingPointerRenderer.color = color;
//        }

//        private void OnTriggerEnter2D(Collider2D collision)
//        {
//            if (collision.isTrigger)
//            {
//                return;
//            }

//            //건설할떄만 확인함
//            //빈공간을 체크
//            //충돌체 체크
//            if (collision.TryGetComponent(out TestWorldObject worldObject))
//            {
                
//                if (!checkCol2DWorldObjectList.Contains(worldObject))
//                {
//                    checkCol2DWorldObjectList.Add(worldObject);
//                    isFindCollider = true;
//                }
//            }
//        }
//        private void OnTriggerExit2D(Collider2D collision)
//        {
//            if (collision.isTrigger)
//            {
//                return;
//            }

//            //건설할떄만 확인함
//            //빈공간을 체크
//            //충돌체 체크
//            if (collision.TryGetComponent(out TestWorldObject worldObject))
//            {
//                if (checkCol2DWorldObjectList.Contains(worldObject))
//                {
//                    checkCol2DWorldObjectList.Remove(worldObject);

//                    if (checkCol2DWorldObjectList.Count == 0)
//                    {
//                        isFindCollider = false;
//                    }
//                }
//            }
//        }

//        //#if UNITY_EDITOR
//        //        //기즈모
//        //        private void OnDrawGizmos()
//        //        {
//        //            if (!targetBuildModeType ==)
//        //            {
//        //                return;
//        //            }

//        //            Gizmos.DrawWireSphere(transform.position, 0.3f);
//        //            Gizmos.DrawLine(startMousePosition, endMousePosition);

//        //            //드래그시 작동되는 동작
//        //            float _startPosX = startMousePosition.x;
//        //            float _startPosY = startMousePosition.y;
//        //            float _endPosX = endMousePosition.x;
//        //            float _endPosY = endMousePosition.y;

//        //            float _startNewPos = _startPosX - _endPosX;
//        //            float _endNewPos = _startPosY - _endPosY;


//        //            Vector2 _boxSize = new Vector2(Mathf.Abs(_startNewPos), Mathf.Abs(_endNewPos));

//        //            float newX = (_startPosX + _endPosX) / 2;
//        //            float newY = (_startPosY + _endPosY) / 2;
//        //            Vector2 _mousePos = new Vector2(newX, newY);


//        //            Gizmos.DrawWireCube(_mousePos, _boxSize);

//        //        }
//        //#endif
//    }


//    public enum BuildLayerType
//    {
//        Building,
//        Floor,
//    }
//}

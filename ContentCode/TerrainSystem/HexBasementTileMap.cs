//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//using lLCroweTool;
//using UnityEngine.Tilemaps;
//using lLCroweTool.Dictionary;
//using lLCroweTool.TimerSystem;





//바꾸기






//public class HexBasementTileMap : UpdateTimerModule_Base
//{
//    //헥스타일맵
//    //타일에 무엇이있는지체크
//    //BFS도 같이 짜기//길찾기
//    //감염? 그것도 만들어보자


//    [SerializeField] protected Tilemap tilemap;

//    private int loopCount = 1000;
//    private int cashing = 0;
//    public Vector3Int[] vector3IntArray;


//    [Header("빈공간&산소 딕셔너리")]
//    //위치값은 중복되지않음
//    public HexAreaBible voidRoomBible = new HexAreaBible();

//    public int oxygenTileCount;//타일갯수
//    public int maxOxygenValue = 100;//최대산소값
//    public int outSideRemoveOxygenValue = 30;//사이드타일산소삭제값
//    public int oxygenChangeValue = 10;//산소변화값

//    //private TilemapCollider2D tilemapCol2D;//영역체크용//우주차량용애도 있는거 같은데 체크하기
//    //이게우선순위가 더 높은거 같음


//    [System.Serializable] public class HexAreaBible : CustomDictionary<Vector3Int, HexAreaInfo> { }


//    protected override void Awake()
//    {
//        //if (tilemap == null)
//        //{
//        //    tilemap = GetComponent<Tilemap>();
//        //}
//        //vector3IntArray = lLcroweUtil.GetAllTilePos(tilemap);//모든배치타일 가져오기

//        GetTilePos();

//        for (int i = 0; i < vector3IntArray.Length; i++)
//        {
//            lLcroweUtil.SetTile(vector3IntArray[i], TileFlags.None, tilemap);
//            float actionTime = Random.Range(0.01f, 0.5f);
//            bool isOutSide = lLcroweUtil.GetIsSideTile(vector3IntArray[i], tilemap);

//            HexAreaInfo testHexAreaInfo = new HexAreaInfo(isOutSide, true, maxOxygenValue, actionTime, vector3IntArray[i], this);
//            voidRoomBible.Add(vector3IntArray[i], testHexAreaInfo);
//        }

//        oxygenTileCount = vector3IntArray.Length;
//        loopCount = vector3IntArray.Length / 4;

//        //4천개까지 버터줄만한데//다른컨텐츠들을 생각하면 1~2천이 나아보임
//        if (loopCount > 2000)
//        {
//            loopCount = 2000;
//        }


//        SetTimer(0.001f);

//        AstarPath.active.Scan();
//    }



//    public void RefleshBasementTileMap()
//    {
//        GetTilePos();

//        for (int i = 0; i < vector3IntArray.Length; i++)
//        {
//            if (!voidRoomBible.ContainsKey(vector3IntArray[i]))
//            {
//                lLcroweUtil.SetTile(vector3IntArray[i], TileFlags.None, tilemap);
//                float actionTime = Random.Range(0.01f, 0.5f);
//                bool isOutSide = lLcroweUtil.GetIsSideTile(vector3IntArray[i], tilemap);
//                HexAreaInfo testHexAreaInfo = new HexAreaInfo(isOutSide, true, maxOxygenValue, actionTime, vector3IntArray[i], this);
//                voidRoomBible.Add(vector3IntArray[i], testHexAreaInfo);
//            }
//            else
//            {
//                HexAreaInfo testHexAreaInfo = voidRoomBible[vector3IntArray[i]];
//                testHexAreaInfo.isOutSideOxygen = lLcroweUtil.GetIsSideTile(vector3IntArray[i], tilemap);
//            }
//        }
//    }

//    [ButtonMethod]
//    public void GetTilePos()
//    {
//        if (tilemap == null)
//        {
//            tilemap = GetComponent<Tilemap>();
//        }
//        vector3IntArray = lLcroweUtil.GetAllTilePos(tilemap);//모든배치타일 가져오기
//    }

//    public override void UpdateTimerModuleFunc()
//    {
//        //이구간을 처리하면 프레임이 괜찬아질듯
//        //기본55~58, 회전이동46~51
//        //for (int i = 0; i < vector3IntArray.Length; i++)
//        //{
//        //    if (testOxygenBible.ContainsKey(vector3IntArray[i]))
//        //    {
//        //        testOxygenBible[vector3IntArray[i]].UpdateOxygen();
//        //    }
//        //}

//        //트릭
//        //총량의 절반//느낌좋음
//        //3분에 1//아직까지 괜찮음
//        //4분에 1//끓어지는 맛이있어 괜찮음

//        //기본95~99, 회전이동90~92

//        //Profiler.BeginSample("-=OxygenUpdateLoop!=-");
//        HexAreaInfo voidRoom = null;

//        for (int i = 0; i < loopCount; i++)
//        {
//            if (voidRoomBible.ContainsKey(vector3IntArray[cashing]))
//            {
//                //Profiler.BeginSample("-=OxygenUpdate!=-");
//                voidRoom = voidRoomBible[vector3IntArray[cashing]];
//                HexAreaInfo.UpdateOxygen(ref voidRoom);
//                //Profiler.EndSample();
//            }
//            cashing++;
//            cashing = cashing >= vector3IntArray.Length ? 0 : cashing;
//        }
//        //Profiler.EndSample();

//    }

//    /// <summary>
//    /// 해당위치의 빈공간정보를 가져옴
//    /// </summary>
//    /// <param name="pos">위치</param>
//    /// <returns>빈공간정보</returns>
//    public HexAreaInfo GetHexAreaInfo(Vector3Int pos)
//    {
//        if (voidRoomBible.ContainsKey(pos))
//        {
//            return voidRoomBible[pos];
//        }
//        return null;
//    }

//    /// <summary>
//    /// 빈공간 정보를 추가or세팅할떄 사용하는 함수
//    /// </summary>
//    /// <param name="pos">위치</param>
//    /// <param name="tile">바닥에쓸 건물데이터</param>
//    public void AddHexAreaInfo(Vector3Int pos, int buildingData)
//    {
//        //바닥건설시에만 작동됨
//        if (!voidRoomBible.ContainsKey(pos))
//        {
//            //산소설정
//            float actionTime = Random.Range(0.01f, 0.5f);
//            bool isOutSide = lLcroweUtil.GetIsSideTile(pos, tilemap);
//            HexAreaInfo voidRoomInfo = new HexAreaInfo(isOutSide, true, maxOxygenValue, actionTime, pos, this);
//            voidRoomBible.Add(pos, voidRoomInfo);

//            //건물데이터세팅후 산소로직세팅
//            voidRoomInfo.SetBuildingData(buildingData);
//            SetHexAreaInfoOxygenAction(voidRoomInfo, buildingData.buildingType, true);

//            //타일맵세팅
//            lLcroweUtil.SetTile(pos, TileFlags.None, tilemap);
//            lLcroweUtil.SetTile(pos, buildingData.buildingTile, tilemap);

//            //재설정
//            vector3IntArray = lLcroweUtil.GetAllTilePos(tilemap);//모든배치타일 가져오기
//            oxygenTileCount = vector3IntArray.Length;
//            loopCount = vector3IntArray.Length / 4;
//        }
//        else
//        {
//            //건물데이터세팅후 산소로직세팅
//            HexAreaInfo voidRoomInfo = voidRoomBible[pos];
//            voidRoomInfo.SetBuildingData(buildingData);
//            SetHexAreaInfoOxygenAction(voidRoomInfo, buildingData.buildingType, true);

//            //타일맵세팅
//            lLcroweUtil.SetTile(pos, buildingData.buildingTile, tilemap);
//        }
//    }

//    /// <summary>
//    /// 빈공간정보를 삭제하는 함수
//    /// </summary>
//    /// <param name="pos">위치</param>
//    /// <param name="isDeleteHexAreaInfo">빈공간사전에서 아예없애는 여부</param>
//    public void RemoveHexAreaInfo(Vector3Int pos, int buildingData, bool isDeleteHexAreaInfo = false)
//    {
//        //바닥해체시에만 작동됨
//        if (voidRoomBible.ContainsKey(pos))
//        {
//            HexAreaInfo voidRoomInfo = voidRoomBible[pos];
//            switch (buildingData.buildingType)
//            {
//                case BuildingType.Frame:
//                case BuildingType.Floor:
//                    voidRoomInfo.SetBuildingData(null);
//                    //타일맵세팅
//                    lLcroweUtil.SetTile(pos, null, tilemap);

//                    SetHexAreaInfoOxygenAction(voidRoomInfo, BuildingType.Floor, false);
//                    break;
//                case BuildingType.Wall:
//                case BuildingType.Door:
//                    SetHexAreaInfoOxygenAction(voidRoomInfo, BuildingType.Wall, false);
//                    break;
//                case BuildingType.Building:
//                case BuildingType.Pipe:
//                    //해당사항없음
//                    break;
//            }


//            //빈공간정보를 지우는 여부
//            if (isDeleteHexAreaInfo)
//            {
//                voidRoomBible.Remove(pos);
//                //재설정
//                vector3IntArray = lLcroweUtil.GetAllTilePos(tilemap);//모든배치타일 가져오기
//                oxygenTileCount = vector3IntArray.Length;
//                loopCount = vector3IntArray.Length / 4;
//            }
//        }
//    }

//    /// <summary>
//    /// 빈공간정보에 대한 산소로직을 설정해주는 함수//버그있을수 있음
//    /// </summary>
//    /// <param name="voidRoomInfo">타겟팅된 빈공간</param>
//    /// <param name="buildingType">빌딩타입</param>
//    /// <param name="isBuild">세팅여부</param>
//    private void SetHexAreaInfoOxygenAction(HexAreaInfo voidRoomInfo, BuildingType buildingType, bool isBuild)
//    {
//        switch (buildingType)
//        {
//            case BuildingType.Frame:
//            case BuildingType.Floor:
//                if (isBuild)
//                {
//                    //건설
//                    voidRoomInfo.SetOxygenAction(false, BuildingType.Floor);
//                }
//                else
//                {
//                    //파괴
//                    //산소로직변경
//                    voidRoomInfo.SetOxygenAction(true, BuildingType.Floor);
//                }

//                //근처산소재설정
//                Vector3Int[] tempArray = lLcroweUtil.GetSideTilePos(voidRoomInfo.GetTilePos(), tilemap);
//                for (int i = 0; i < tempArray.Length; i++)
//                {
//                    //존재여부 체크
//                    if (voidRoomBible.ContainsKey(tempArray[i]))
//                    {
//                        bool isOutSide = lLcroweUtil.GetIsSideTile(tempArray[i], tilemap);
//                        HexAreaInfo testHexAreaInfo = voidRoomBible[tempArray[i]];
//                        testHexAreaInfo.SetOxygenAction(isOutSide, BuildingType.Floor);
//                    }
//                }
//                break;
//            case BuildingType.Wall:
//            case BuildingType.Door:
//                if (isBuild)
//                {
//                    //건설
//                    //산소로직변경//해당구역산소없애기
//                    voidRoomInfo.SetOxygenAction(false, BuildingType.Wall);
//                    voidRoomInfo.RemoveOxygenValue(voidRoomInfo.GetCurOxygenValue());
//                }
//                else
//                {
//                    //파괴
//                    //산소로직변경
//                    voidRoomInfo.SetOxygenAction(true, BuildingType.Wall);
//                }
//                AstarPath.active.Scan();
//                break;
//            case BuildingType.Building:
//            case BuildingType.Pipe:
//                //해당사항없음
//                break;
//        }
//    }

//    /// <summary>
//    /// 타일맵 가져오기함수
//    /// </summary>
//    /// <returns>타일맵</returns>
//    public Tilemap GetTileMap()
//    {
//        return tilemap;
//    }

//    /// <summary>
//    /// 전초기지빈공간 작동원리 가져오는함수
//    /// </summary>
//    /// <returns>작동로직 타입</returns>
//    public GameWorldOperateLogic GetGameWorldOperateLogic()
//    {
//        return basementTileMapVoidRoomLogic;
//    }


//#if UNITY_EDITOR
//    private void OnDrawGizmos()
//    {
//        Vector3 origin = transform.position;
//        for (int i = 0; i < vector3IntArray.Length; i++)
//        {
//            Vector3 tempPos = vector3IntArray[i];
//            UnityEditor.Handles.Label(origin + tempPos, $"{tempPos}");
//        }
//    }
//#endif


//    public class HexAreaInfo
//    {
//        private static int randomIndex = 0;

//        private static List<int> indexList = new List<int>(6);
//        [Header("비어 있는 공간의 산소인가? 공기를 없애는 구간이면 true")]
//        public bool isOutSideOxygen = false;//최대 밖의 산소인가 -> 없애는 산소
//        [Header("변화가있는 산소인가? 벽이면 false")]
//        public bool isConstOxygen = true;//변화 있는 산소인가?/
//        //[SerializeField] private int maxOxygenValue;//최대 산소량
//        [SerializeField] private int curOxygenValue;//현재 산소량

//        //색관련
//        [Range(0.0f, 1.0f)]
//        [SerializeField] private float colorAlphaValue;
//        [SerializeField] private bool isChangeColor = false;

//        //랜덤변수 관련        
//        [HideInInspector] [SerializeField] private int[] indexArray;

//        //시간 관련
//        [SerializeField] private float actionTime;
//        private float time;

//        //해당되는 타일위치와 타일맵
//        [SerializeField] private Vector3Int targetTilePos;//현산소정보타일좌표
//        [SerializeField] private HexBasementTileMap targetBasementTilemap;//타일맵
//        [SerializeField] private GameObject targetBuildingData;//건설된 데이터//존재하는지

//        //산소 관련        
//        [SerializeField] private Vector3Int[] nearOxygenArray;//근처산소 위치
//        [SerializeField] private bool[] nearCheckOxygenArray;//산소가 있는지 여부//기본은 다 false

//        //산소를 없애는 구간에서 사용할 흡입력을 세팅해줄 변수들
//        [SerializeField] private bool isExistPulsingExpolostion = false;
//        [SerializeField] private PulsingExplosion targetPulsingExplosion;

//        public HexAreaInfo(bool outOxygen, bool constOxygen, int targetMaxOxygenValue, float timer, Vector3Int pos, HexBasementTileMap basementTileMap)
//        {
//            //testOxygenType = (TestOxygenType[])System.Enum.GetValues(typeof(TestOxygenType));
//            //value = new int[testOxygenType.Length];

//            isOutSideOxygen = outOxygen;
//            if (isOutSideOxygen)
//            {
//                lLcroweUtil.SetTile(pos, Color.red, basementTileMap.GetTileMap());
//            }
//            isConstOxygen = constOxygen;

//            //maxOxygenValue = targetMaxOxygenValue;
//            curOxygenValue = targetMaxOxygenValue;

//            colorAlphaValue = basementTileMap.GetTileMap().GetColor(pos).a;

//            actionTime = timer;
//            time = Time.time;

//            targetTilePos = pos;
//            targetBasementTilemap = basementTileMap;

//            nearOxygenArray = lLcroweUtil.GetSideTilePos(targetTilePos, basementTileMap.GetTileMap());
//            nearCheckOxygenArray = lLcroweUtil.GetSideTileIsHas(targetTilePos, basementTileMap.GetTileMap());

//            SetRandomIndexArray(ref indexArray);
//        }

//        /// <summary>
//        /// 빈공간 정보에 건물데이터를 세팅하는 함수.프레임,바닥종류만
//        /// </summary>
//        /// <param name="buildingData">빌딩데이터</param>
//        public void SetBuildingData(int buildingData)
//        {
//            targetBuildingData = buildingData;
//        }

//        /// <summary>
//        /// 빈공간 정보에 건물데이터를 가져오는 함수
//        /// </summary>
//        /// <returns>건물데이터</returns>
//        public int GetBuildingData()
//        {
//            return targetBuildingData;
//        }

//        /// <summary>
//        /// 빈공간에 세팅된 타일위치를 가져오는 함수
//        /// </summary>
//        /// <returns>타일위치</returns>
//        public Vector3Int GetTilePos()
//        {
//            return targetTilePos;
//        }

//        public void AddOxygenValue(int value)
//        {
//            curOxygenValue = targetBasementTilemap.maxOxygenValue < curOxygenValue + value ? targetBasementTilemap.maxOxygenValue : curOxygenValue += value;
//            isChangeColor = true;
//        }

//        public void RemoveOxygenValue(int value)
//        {
//            curOxygenValue = 0 > curOxygenValue - value ? 0 : curOxygenValue -= value;
//            isChangeColor = true;
//        }

//        /// <summary>
//        /// 산소 스프라이트 알파값변경
//        /// </summary>
//        private static void UpdateOxygenSprite(ref HexAreaInfo voidRoomInfo)
//        {
//            //return;

//            if (!voidRoomInfo.isChangeColor)
//            {
//                return;
//            }


//            //0.3추가
//            if (voidRoomInfo.curOxygenValue % 10 == 0)
//            {
//                //알파값변환
//                //colorAlphaValue = (float)(curOxygenValue * 1 / (float)maxOxygenValue);
//                voidRoomInfo.colorAlphaValue = voidRoomInfo.curOxygenValue / (float)voidRoomInfo.targetBasementTilemap.maxOxygenValue;

//                //colorAlphaValue = (float)curOxygenValue * (float)maxOxygenValue * 0.01f;

//                //색깔변경작동방식체크
//                if (voidRoomInfo.isOutSideOxygen)
//                {
//                    lLcroweUtil.SetTile(voidRoomInfo.targetTilePos, voidRoomInfo.colorAlphaValue, voidRoomInfo.targetBasementTilemap.GetTileMap());//알파만
//                }
//                else
//                {
//                    lLcroweUtil.SetTile(voidRoomInfo.targetTilePos, 1, voidRoomInfo.colorAlphaValue, voidRoomInfo.colorAlphaValue, 1, voidRoomInfo.targetBasementTilemap.GetTileMap());//컬러
//                }
//            }

//            ////알파값변환
//            //colorAlphaValue = (float)curOxygenValue * 1 / (float)maxOxygenValue;
//            ////colorAlphaValue = (float)curOxygenValue * (float)maxOxygenValue * 0.01f;
//            //ChangeSpriteOpasity(colorAlphaValue);

//            voidRoomInfo.isChangeColor = false;
//        }

//        /// <summary>
//        /// 산소 업데이트
//        /// </summary>
//        public static void UpdateOxygen(ref HexAreaInfo voidRoomInfo)
//        {
//            if (Time.time < voidRoomInfo.actionTime + voidRoomInfo.time)
//            {
//                return;
//            }
//            voidRoomInfo.time = Time.time;

//            //작동 잘됨
//            //변화하지않는 산소인가 == 건축물등의 위치
//            if (!voidRoomInfo.isConstOxygen)
//            {
//                return;
//            }
//            else if (voidRoomInfo.isOutSideOxygen)
//            {
//                //빈공간의 산소인가 == 진공
//                //curOxygenValue -= 30;
//                //isChange = true;
//                //RemoveOxygenValue(30);
//                voidRoomInfo.RemoveOxygenValue(voidRoomInfo.targetBasementTilemap.outSideRemoveOxygenValue);

//                if (voidRoomInfo.isExistPulsingExpolostion)
//                {
//                    //현재 산소량이 0 초과인가
//                    if (0 < voidRoomInfo.curOxygenValue)
//                    {
//                        voidRoomInfo.targetPulsingExplosion.loopForever = true;
//                        voidRoomInfo.targetPulsingExplosion.Activate();
//                    }
//                    else
//                    {
//                        voidRoomInfo.targetPulsingExplosion.loopForever = false;
//                    }
//                }

//                //LimitOxygenValueCheck();
//                UpdateOxygenSprite(ref voidRoomInfo);
//                return;
//            }

//            //산소이미지 변경
//            UpdateOxygenSprite(ref voidRoomInfo);

//            //산소랜덤작용인덱스
//            int index = 0;
//            HexAreaInfo tempHexAreaInfo = null;
//            switch (voidRoomInfo.targetBasementTilemap.GetGameWorldOperateLogic())
//            {
//                case GameWorldOperateLogic.TopView:
//                    //탑뷰
//                    //모든좌표를 랜덤으로 체크
//                    index = Random.Range(0, 4);

//                    //근처 단일산소에 계산//연산 좋음
//                    //산소타일이 존재하는지 체크
//                    if (voidRoomInfo.nearCheckOxygenArray[index])
//                    {
//                        //있으면 메커니즘 작동
//                        //tmpOxygen = nearOxygens[indexArray[i]];                    
//                        tempHexAreaInfo = voidRoomInfo.targetBasementTilemap.GetHexAreaInfo(voidRoomInfo.nearOxygenArray[index]);
//                        voidRoomInfo.CalVoidRoom(tempHexAreaInfo);
//                    }

//                    //근처모든 산소에 계산//연산더먹는데 느낌은 더 좋다
//                    //근처 산소 계산
//                    //BuildingManager.Instance.SetRandomIndexArray(ref indexArray);
//                    //for (int i = 0; i < nearOxygenArray.Length; i++)
//                    //{
//                    //    //해당 배열에 근처의 산소가 있는가
//                    //    //if (nearCheckOxygen[indexArray[i]])
//                    //    if (nearCheckOxygenArray[indexArray[i]])
//                    //    {
//                    //        //있으면 메커니즘 작동
//                    //        //tmpOxygen = nearOxygens[indexArray[i]];                    
//                    //        HexAreaInfo tempHexAreaInfo = targetBaseMentTilemap.GetHexAreaInfo(nearOxygenArray[indexArray[i]]);
//                    //        CalVoidRoom(tempHexAreaInfo);
//                    //    }
//                    //}
//                    break;
//                case GameWorldOperateLogic.SideView:
//                    //사이드뷰
//                    //아래를 먼저확인후
//                    //좌우를 랜덤으로 확인후
//                    //위를 확인

//                    //위아래좌우로 받아왔음
//                    //0 1 2 3
//                    //각 위치확인

//                    //아래확인
//                    if (voidRoomInfo.nearCheckOxygenArray[1])
//                    {
//                        tempHexAreaInfo = voidRoomInfo.targetBasementTilemap.GetHexAreaInfo(voidRoomInfo.nearOxygenArray[1]);
//                        if (tempHexAreaInfo.isConstOxygen)
//                        {
//                            //해당산소최대치보다 산소현재량이 낮으면 옮김//나중에 로직변경시킬것
//                            voidRoomInfo.CalVoidRoom(tempHexAreaInfo);
//                            return;
//                        }
//                    }

//                    index = Random.Range(2, 4);

//                    //좌우 확인
//                    if (voidRoomInfo.nearCheckOxygenArray[index])
//                    {
//                        tempHexAreaInfo = voidRoomInfo.targetBasementTilemap.GetHexAreaInfo(voidRoomInfo.nearOxygenArray[index]);
//                        if (tempHexAreaInfo.isConstOxygen)
//                        {
//                            //해당산소최대치보다 산소현재량이 낮으면 옮김//나중에 로직변경시킬것
//                            voidRoomInfo.CalVoidRoom(tempHexAreaInfo);
//                            return;
//                        }
//                    }

//                    //위확인
//                    if (voidRoomInfo.nearCheckOxygenArray[0])
//                    {
//                        tempHexAreaInfo = voidRoomInfo.targetBasementTilemap.GetHexAreaInfo(voidRoomInfo.nearOxygenArray[0]);
//                        if (tempHexAreaInfo.isConstOxygen)
//                        {
//                            //해당산소최대치보다 산소현재량이 낮으면 옮김//나중에 로직변경시킬것
//                            voidRoomInfo.CalVoidRoom(voidRoomInfo);
//                            return;
//                        }
//                    }
//                    break;
//                default:
//                    //아무행동없음
//                    //이상태면 문제있는거
//                    return;
//            }

//        }

//        /// <summary>
//        /// 랜덤한 산소처리 인덱스를 반환하는 구역
//        /// </summary>
//        /// <param name="indexArray">참조할 인덱스</param>
//        private static void SetRandomIndexArray(ref int[] indexArray)
//        {
//            indexList.Clear();
//            //랜덤 중복되지않는 수 채우기
//            while (true)
//            {
//                randomIndex = Random.Range(0, 4);
//                if (!indexList.Contains(randomIndex))
//                {
//                    indexList.Add(randomIndex);
//                    if (indexList.Count >= 4)
//                    {
//                        indexArray = indexList.ToArray();
//                        break;
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// 빈공간정보를 계산하는 함수
//        /// </summary>
//        /// <param name="voidRoomInfo">타겟이 될빈공간</param>
//        private void CalVoidRoom(HexAreaInfo voidRoomInfo)
//        {
//            //완벽히 계산하지않음//오차는 그냥 넘겨버렷음

//            //Debug.Log("넘버: " + number + nearCheckOxygenArray[indexArray[i]] +""+ voidRoomInfo.targetTilePos + "타겟");

//            //해당산소가 변화가 있는 산소인가?
//            if (voidRoomInfo.isConstOxygen)
//            {
//                //타겟 산소량이 현산소량보다 작으면 변경
//                if (curOxygenValue > voidRoomInfo.curOxygenValue)
//                {
//                    //증가 감소                       
//                    //targetOxygen.curOxygenValue--;
//                    //tmpOxygen.curOxygenValue++;

//                    //curOxygenValue -= 10;
//                    //isChange = true;
//                    //LimitOxygenValueCheck();
//                    //RemoveOxygenValue(10);
//                    RemoveOxygenValue(targetBasementTilemap.oxygenChangeValue);


//                    //tmpOxygen.curOxygenValue += 10;
//                    //tmpOxygen.SetIsChange(true);
//                    //tmpOxygen.LimitOxygenValueCheck();
//                    //voidRoomInfo.AddOxygenValue(10);
//                    voidRoomInfo.AddOxygenValue(targetBasementTilemap.oxygenChangeValue);

//                    //산소 업데이트
//                    //UpdateOxygenSprite();//자신산소 랜더링업데이트
//                    UpdateOxygenSprite(ref voidRoomInfo);//상대방산소 랜더링업데이트
//                }
//            }
//        }


//        ////최소값과 최대값을 체크하여 현재숫자를 변환
//        //public void LimitOxygenValueCheck()
//        //{
//        //    if (curOxygenValue > maxOxygenValue)
//        //    {
//        //        curOxygenValue = maxOxygenValue;
//        //    }
//        //    else if (curOxygenValue < 0)
//        //    {
//        //        curOxygenValue = 0;
//        //    }
//        //}

//        //건축물에서 작업하는 함수
//        //체력이 변하거나 빌딩타입마다 다루는게 다름
//        //플로어이면 해당장소를 빈공간으로 만들어버려야함


//        /// <summary>
//        /// 건축물의 상태에 따른 산소로직 작동방식변경함수
//        /// </summary>
//        /// <param name="isDestroy">파괴여부</param>
//        /// <param name="buildingType">건물타입</param>
//        public void SetOxygenAction(bool isDestroy, BuildingType buildingType)
//        {
//            switch (buildingType)
//            {
//                case BuildingType.Frame:
//                case BuildingType.Floor:
//                    //파괴되었는가?
//                    isOutSideOxygen = isDestroy;
//                    isExistPulsingExpolostion = isDestroy;
//                    isConstOxygen = true;

//                    if (isDestroy)
//                    {
//                        targetPulsingExplosion = SuctionPowerManager.Instance.CallSuctionPower(targetBasementTilemap.transform, (Vector3)targetTilePos);
//                    }
//                    else
//                    {
//                        SuctionPowerManager.Instance.ReturnSuctionPower(targetPulsingExplosion);
//                    }
//                    break;
//                case BuildingType.Wall:
//                case BuildingType.Door:
//                    //파괴되었는가?
//                    //파괴되었으면 산소가 움직이고 아니면 정지
//                    isConstOxygen = isDestroy;
//                    break;
//                case BuildingType.Building:
//                case BuildingType.Pipe:
//                    //해당사항없음
//                    break;
//            }

//            if (isOutSideOxygen)
//            {
//                lLcroweUtil.SetTile(targetTilePos, Color.red, targetBasementTilemap.GetTileMap());
//            }
//            //else
//            //{
//            //    lLcroweUtil.SetTile(targetTilePos, Color.white, targetBasementTilemap.GetTileMap());
//            //}
//        }

//        public int GetMaxOxygenValue()
//        {
//            return targetBasementTilemap.maxOxygenValue;
//        }

//        public int GetCurOxygenValue()
//        {
//            return curOxygenValue;
//        }
//    }
//}

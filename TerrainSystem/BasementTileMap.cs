#if Doozy

using UnityEngine;
using UnityEngine.Tilemaps;
using lLCroweTool.Dictionary;
using System.Collections.Generic;
using lLCroweTool.TimerSystem;
using lLCroweTool.TerrainSystem.SuctionPower;
using lLCroweTool.BuildingSystem;
using lLCroweTool.TileMap;

namespace lLCroweTool.TerrainSystem.BasementTileMap
{
    /// <summary>
    /// 게임월드에서 작동되는 기준을 칭함
    /// </summary>
    public enum GameWorldOperateLogic
    {
        /// <summary>
        /// 탑뷰 => 오브젝트들을 상단에서보는 기준
        /// </summary>
        TopView,
        /// <summary>
        /// 사이드뷰 => 오브젝트들을 옆에서 보는 기준
        /// </summary>
        SideView,
    }
        
    public class BasementTileMap : UpdateTimerModule_Base
    {
        //전초기지 타일맵 기초
        //전초기지를 구성할때 사용하는 타일맵       
        //사용처 우주선, 차량, 전초기지

        [SerializeField] protected Tilemap tilemap;
        [SerializeField] private GameWorldOperateLogic basementTileMapVoidRoomLogic;

        private int loopCount = 1000;        
        private int cashing= 0;
        public Vector3Int[] vector3IntArray;

        
        [Header("빈공간&산소 딕셔너리")]
        //위치값은 중복되지않음
        public VoidRoomBible voidRoomBible = new VoidRoomBible();

        public int oxygenTileCount;//타일갯수
        public int maxOxygenValue = 100;//최대산소값
        public int outSideRemoveOxygenValue = 30;//사이드타일산소삭제값
        public int oxygenChangeValue = 10;//산소변화값

        //private TilemapCollider2D tilemapCol2D;//영역체크용//우주차량용애도 있는거 같은데 체크하기
        //이게우선순위가 더 높은거 같음

     
        [System.Serializable] public class VoidRoomBible : CustomDictionary<Vector3Int, VoidRoomInfo> { }

      
        protected override void Awake()
        {
            //if (tilemap == null)
            //{
            //    tilemap = GetComponent<Tilemap>();
            //}
            //vector3IntArray = lLcroweUtil.GetAllTilePos(tilemap);//모든배치타일 가져오기

            base.Awake();
            GetTilePos();

            for (int i = 0; i < vector3IntArray.Length; i++)
            {
                lLcroweRectTileMapUtil.SetTile(vector3IntArray[i], TileFlags.None, tilemap);
                float actionTime = Random.Range(0.01f, 0.5f);
                bool isOutSide = lLcroweRectTileMapUtil.GetIsSideTile(vector3IntArray[i], tilemap);

                VoidRoomInfo testVoidRoomInfo = new VoidRoomInfo(isOutSide, true, maxOxygenValue, actionTime, vector3IntArray[i], this);
                voidRoomBible.Add(vector3IntArray[i], testVoidRoomInfo);
            }

            oxygenTileCount = vector3IntArray.Length;
            loopCount = vector3IntArray.Length / 4;

            //4천개까지 버터줄만한데//다른컨텐츠들을 생각하면 1~2천이 나아보임
            if (loopCount > 2000)
            {
                loopCount = 2000;
            }

            
            SetTimer(0.001f);            

            AstarPath.active.Scan();
        }

        public void RefleshBasementTileMap()
        {
            GetTilePos();

            for (int i = 0; i < vector3IntArray.Length; i++)
            {
                if (!voidRoomBible.ContainsKey(vector3IntArray[i]))
                {
                    lLcroweRectTileMapUtil.SetTile(vector3IntArray[i], TileFlags.None, tilemap);
                    float actionTime = Random.Range(0.01f, 0.5f);
                    bool isOutSide = lLcroweRectTileMapUtil.GetIsSideTile(vector3IntArray[i], tilemap);
                    VoidRoomInfo testVoidRoomInfo = new VoidRoomInfo(isOutSide, true, maxOxygenValue, actionTime, vector3IntArray[i], this);
                    voidRoomBible.Add(vector3IntArray[i], testVoidRoomInfo);
                }
                else
                {
                    VoidRoomInfo testVoidRoomInfo = voidRoomBible[vector3IntArray[i]];
                    testVoidRoomInfo.isOutSideOxygen = lLcroweRectTileMapUtil.GetIsSideTile(vector3IntArray[i], tilemap);
                }
            }
        }

        [ButtonMethod]
        public void GetTilePos()
        {
            if (tilemap == null)
            {
                tilemap = GetComponent<Tilemap>();
            }
            vector3IntArray = lLcroweRectTileMapUtil.GetAllTilePos(tilemap);//모든배치타일 가져오기
        }

        public override void UpdateTimerModuleFunc()
        {
            //이구간을 처리하면 프레임이 괜찬아질듯
            //기본55~58, 회전이동46~51
            //for (int i = 0; i < vector3IntArray.Length; i++)
            //{
            //    if (testOxygenBible.ContainsKey(vector3IntArray[i]))
            //    {
            //        testOxygenBible[vector3IntArray[i]].UpdateOxygen();
            //    }
            //}

            //트릭
            //총량의 절반//느낌좋음
            //3분에 1//아직까지 괜찮음
            //4분에 1//끓어지는 맛이있어 괜찮음

            //기본95~99, 회전이동90~92

            //Profiler.BeginSample("-=OxygenUpdateLoop!=-");
            VoidRoomInfo voidRoom = null;

            for (int i = 0; i < loopCount; i++)
            {
                if (voidRoomBible.ContainsKey(vector3IntArray[cashing]))
                {
                    //Profiler.BeginSample("-=OxygenUpdate!=-");
                    voidRoom = voidRoomBible[vector3IntArray[cashing]];
                    VoidRoomInfo.UpdateOxygen(ref voidRoom);
                    //Profiler.EndSample();
                }
                cashing++;
                cashing = cashing >= vector3IntArray.Length ? 0 : cashing;
            }
            //Profiler.EndSample();

        }

        /// <summary>
        /// 해당위치의 빈공간정보를 가져옴
        /// </summary>
        /// <param name="pos">위치</param>
        /// <returns>빈공간정보</returns>
        public VoidRoomInfo GetVoidRoomInfo(Vector3Int pos)
        {
            if (voidRoomBible.ContainsKey(pos))
            {
                return voidRoomBible[pos];
            }
            return null;
        }

        /// <summary>
        /// 빈공간 정보를 추가or세팅할떄 사용하는 함수
        /// </summary>
        /// <param name="pos">위치</param>
        /// <param name="tile">바닥에쓸 건물데이터</param>
        public void AddVoidRoomInfo(Vector3Int pos, BuildingObjectScript buildingData)
        {
            //바닥건설시에만 작동됨
            if (!voidRoomBible.ContainsKey(pos))
            {
                //산소설정
                float actionTime = Random.Range(0.01f, 0.5f);
                bool isOutSide = lLcroweRectTileMapUtil.GetIsSideTile(pos, tilemap);
                VoidRoomInfo voidRoomInfo = new VoidRoomInfo(isOutSide, true, maxOxygenValue, actionTime, pos, this);                
                voidRoomBible.Add(pos, voidRoomInfo);

                //건물데이터세팅후 산소로직세팅
                voidRoomInfo.SetBuildingData(buildingData);
                SetVoidRoomInfoOxygenAction(voidRoomInfo, buildingData.buildingType, true);

                //타일맵세팅
                lLcroweRectTileMapUtil.SetTile(pos, TileFlags.None, tilemap);
                lLcroweRectTileMapUtil.SetTile(pos, buildingData.buildingTile, tilemap);

                //재설정
                vector3IntArray = lLcroweRectTileMapUtil.GetAllTilePos(tilemap);//모든배치타일 가져오기
                oxygenTileCount = vector3IntArray.Length;
                loopCount = vector3IntArray.Length / 4;
            }
            else
            {
                //건물데이터세팅후 산소로직세팅
                VoidRoomInfo voidRoomInfo = voidRoomBible[pos];
                voidRoomInfo.SetBuildingData(buildingData);
                SetVoidRoomInfoOxygenAction(voidRoomInfo, buildingData.buildingType, true);

                //타일맵세팅
                lLcroweRectTileMapUtil.SetTile(pos, buildingData.buildingTile, tilemap);
            }
        }

        /// <summary>
        /// 빈공간정보를 삭제하는 함수
        /// </summary>
        /// <param name="pos">위치</param>
        /// <param name="isDeleteVoidRoomInfo">빈공간사전에서 아예없애는 여부</param>
        public void RemoveVoidRoomInfo(Vector3Int pos, BuildingObjectScript buildingData, bool isDeleteVoidRoomInfo = false)
        {
            //바닥해체시에만 작동됨
            if (voidRoomBible.ContainsKey(pos))
            {
                VoidRoomInfo voidRoomInfo = voidRoomBible[pos];
                switch (buildingData.buildingType)
                {
                    case BuildingType.Frame:
                    case BuildingType.Floor:
                        voidRoomInfo.SetBuildingData(null);
                        //타일맵세팅
                        lLcroweRectTileMapUtil.SetTile(pos, null, tilemap);

                        SetVoidRoomInfoOxygenAction(voidRoomInfo, BuildingType.Floor, false);
                        break;
                    case BuildingType.Wall:
                    case BuildingType.Door:
                        SetVoidRoomInfoOxygenAction(voidRoomInfo, BuildingType.Wall, false);
                        break;
                    case BuildingType.Building:
                    case BuildingType.Pipe:
                        //해당사항없음
                        break;                    
                }
                

                //빈공간정보를 지우는 여부
                if (isDeleteVoidRoomInfo)
                {
                    voidRoomBible.Remove(pos);
                    //재설정
                    vector3IntArray = lLcroweRectTileMapUtil.GetAllTilePos(tilemap);//모든배치타일 가져오기
                    oxygenTileCount = vector3IntArray.Length;
                    loopCount = vector3IntArray.Length / 4;
                }
            }
        }

        /// <summary>
        /// 빈공간정보에 대한 산소로직을 설정해주는 함수//버그있을수 있음
        /// </summary>
        /// <param name="voidRoomInfo">타겟팅된 빈공간</param>
        /// <param name="buildingType">빌딩타입</param>
        /// <param name="isBuild">세팅여부</param>
        private void SetVoidRoomInfoOxygenAction(VoidRoomInfo voidRoomInfo, BuildingType buildingType, bool isBuild)
        {
            switch (buildingType)
            {
                case BuildingType.Frame:
                case BuildingType.Floor:
                    if (isBuild)
                    {
                        //건설
                        voidRoomInfo.SetOxygenAction(false, BuildingType.Floor);
                    }
                    else
                    {
                        //파괴
                        //산소로직변경
                        voidRoomInfo.SetOxygenAction(true, BuildingType.Floor);
                    }

                    //근처산소재설정
                    //Vector3Int[] tempArray = lLcroweRectTileMapUtil.GetSideTilePos(voidRoomInfo.GetTilePos(), tilemap);//원본
                    Vector3Int[] tempArray = lLcroweRectTileMapUtil.GetSideTilePos(voidRoomInfo.GetTilePos());
                    for (int i = 0; i < tempArray.Length; i++)
                    {
                        //존재여부 체크
                        if (voidRoomBible.ContainsKey(tempArray[i]))
                        {
                            bool isOutSide = lLcroweRectTileMapUtil.GetIsSideTile(tempArray[i], tilemap);
                            VoidRoomInfo testVoidRoomInfo = voidRoomBible[tempArray[i]];
                            testVoidRoomInfo.SetOxygenAction(isOutSide, BuildingType.Floor);
                        }
                    }
                    break;
                case BuildingType.Wall:
                case BuildingType.Door:
                    if (isBuild)
                    {
                        //건설
                        //산소로직변경//해당구역산소없애기
                        voidRoomInfo.SetOxygenAction(false, BuildingType.Wall);
                        voidRoomInfo.RemoveOxygenValue(voidRoomInfo.GetCurOxygenValue());
                    }
                    else
                    {
                        //파괴
                        //산소로직변경
                        voidRoomInfo.SetOxygenAction(true, BuildingType.Wall);
                    }
                    AstarPath.active.Scan();
                    break;
                case BuildingType.Building:
                case BuildingType.Pipe:
                    //해당사항없음
                    break;
            }
        }

        /// <summary>
        /// 타일맵 가져오기함수
        /// </summary>
        /// <returns>타일맵</returns>
        public Tilemap GetTileMap()
        {
            return tilemap;
        }

        /// <summary>
        /// 전초기지빈공간 작동원리 가져오는함수
        /// </summary>
        /// <returns>작동로직 타입</returns>
        public GameWorldOperateLogic GetGameWorldOperateLogic()
        {
            return basementTileMapVoidRoomLogic;
        }


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Vector3 origin = transform.position;
            for (int i = 0; i < vector3IntArray.Length; i++)
            {
                Vector3 tempPos = vector3IntArray[i];
                UnityEditor.Handles.Label(origin + tempPos, $"{tempPos}");
            }
        }
#endif

    }

    //public Sprite sprite
    //타일에 대해 렌더링되는 스프라이트입니다.
    //public Color color
    //타일에 사용되는 스프라이트를 틴트하는 컬러입니다.
    //public Matrix4x4 transform
    //타일의 최종 위치를 결정하는 데 사용되는 트랜스폼 행렬(transform matrix)입니다.이 구문을 수정하여 타일에 회전이나 스케일을 추가할 수 있습니다.
    //public GameObject gameobject
    //타일을 타일맵에 추가할 때 인스턴스화되는 게임 오브젝트입니다.
    //public TileFlags flags
    //타일 동작을 제어하는 플래그입니다.자세한 내용은 TileFlags를 참조하십시오.
    //public Tile.ColliderType colliderType

    //빈공간은 곳 산소가 있고 빈공간 정보    
    //산소와 건설정보것을 볼수 있는공간
    //

    [System.Serializable]
    public class VoidRoomInfo
    {
           //산소관련
        //랜덤변수 관련
        private static int randomIndex = 0;
        //private int[] indexArray = new int[4] { 0, 1, 2, 3 };
        private static List<int> indexList = new List<int>(4);

        //나중에여러개의 산소처리할떄 사용
        //public enum TestOxygenType
        //{
        //    Oxygen1,
        //    Oxygen2,
        //    Oxygen3,
        //    Oxygen4,
        //}        
        //산소
        //산소의 콜라이더는 트리거상태이며
        //크기는 0.5 0.5 스케일대비 절반으로 작성해야함
        [Header("비어 있는 공간의 산소인가? 공기를 없애는 구간이면 true")]
        public bool isOutSideOxygen = false;//최대 밖의 산소인가 -> 없애는 산소
        [Header("변화가있는 산소인가? 벽이면 false")]        
        public bool isConstOxygen = true;//변화 있는 산소인가?
        //[SerializeField] private int maxOxygenValue;//최대 산소량
        [SerializeField] private int curOxygenValue;//현재 산소량

        //색관련
        [Range(0.0f, 1.0f)]
        [SerializeField] private float colorAlphaValue;
        [SerializeField] private bool isChangeColor = false;

        //랜덤변수 관련        
        [HideInInspector] [SerializeField] private int[] indexArray;

        //시간 관련
        [SerializeField] private float actionTime;
        private float time;

        //해당되는 타일위치와 타일맵
        [SerializeField] private Vector3Int targetTilePos;//현산소정보타일좌표
        [SerializeField] private BasementTileMap targetBasementTilemap;//타일맵
        [SerializeField] private BuildingObjectScript targetBuildingData;//건설된 데이터

        //산소 관련        
        [SerializeField] private Vector3Int[] nearOxygenArray;//근처산소 위치
        [SerializeField] private bool[] nearCheckOxygenArray;//산소가 있는지 여부//기본은 다 false

        //산소를 없애는 구간에서 사용할 흡입력을 세팅해줄 변수들
        [SerializeField] private bool isExistPulsingExpolostion = false;
        [SerializeField] private PulsingExplosion targetPulsingExplosion;

        public VoidRoomInfo(bool outOxygen, bool constOxygen, int targetMaxOxygenValue, float timer, Vector3Int pos, BasementTileMap basementTileMap)
        {
            //testOxygenType = (TestOxygenType[])System.Enum.GetValues(typeof(TestOxygenType));
            //value = new int[testOxygenType.Length];

            isOutSideOxygen = outOxygen;
            if (isOutSideOxygen)
            {
                lLcroweRectTileMapUtil.SetTile(pos, Color.red, basementTileMap.GetTileMap());
            }
            isConstOxygen = constOxygen;

            //maxOxygenValue = targetMaxOxygenValue;
            curOxygenValue = targetMaxOxygenValue;

            colorAlphaValue = basementTileMap.GetTileMap().GetColor(pos).a;

            actionTime = timer;
            time = Time.time;

            targetTilePos = pos;
            targetBasementTilemap = basementTileMap;

            nearOxygenArray = lLcroweRectTileMapUtil.GetSideTilePos(targetTilePos);
            nearCheckOxygenArray = lLcroweRectTileMapUtil.GetSideTileIsHas(targetTilePos, basementTileMap.GetTileMap());

            SetRandomIndexArray(ref indexArray);
        }

        /// <summary>
        /// 빈공간 정보에 건물데이터를 세팅하는 함수.프레임,바닥종류만
        /// </summary>
        /// <param name="buildingData">빌딩데이터</param>
        public void SetBuildingData(BuildingObjectScript buildingData)
        {
            targetBuildingData = buildingData;
        }

        /// <summary>
        /// 빈공간 정보에 건물데이터를 가져오는 함수
        /// </summary>
        /// <returns>건물데이터</returns>
        public BuildingObjectScript GetBuildingData()
        {
            return targetBuildingData;
        }

        /// <summary>
        /// 빈공간에 세팅된 타일위치를 가져오는 함수
        /// </summary>
        /// <returns>타일위치</returns>
        public Vector3Int GetTilePos()
        {
            return targetTilePos;
        }

        public void AddOxygenValue(int value)
        {
            curOxygenValue = targetBasementTilemap.maxOxygenValue < curOxygenValue + value ? targetBasementTilemap.maxOxygenValue : curOxygenValue += value;
            isChangeColor = true;
        }

        public void RemoveOxygenValue(int value)
        {
            curOxygenValue = 0 > curOxygenValue - value ? 0 : curOxygenValue -= value;
            isChangeColor = true;
        }

        /// <summary>
        /// 산소 스프라이트 알파값변경
        /// </summary>
        private static void UpdateOxygenSprite(ref VoidRoomInfo voidRoomInfo)
        {
            //return;

            if (!voidRoomInfo.isChangeColor)
            {
                return;
            }


            //0.3추가
            if (voidRoomInfo.curOxygenValue % 10 == 0)
            {
                //알파값변환
                //colorAlphaValue = (float)(curOxygenValue * 1 / (float)maxOxygenValue);
                voidRoomInfo.colorAlphaValue = voidRoomInfo.curOxygenValue / (float)voidRoomInfo.targetBasementTilemap.maxOxygenValue;                

                //colorAlphaValue = (float)curOxygenValue * (float)maxOxygenValue * 0.01f;

                //색깔변경작동방식체크
                if (voidRoomInfo.isOutSideOxygen)
                {
                    lLcroweRectTileMapUtil.SetTile(voidRoomInfo.targetTilePos, voidRoomInfo.colorAlphaValue, voidRoomInfo.targetBasementTilemap.GetTileMap());//알파만
                }
                else
                {
                    lLcroweRectTileMapUtil.SetTile(voidRoomInfo.targetTilePos, 1, voidRoomInfo.colorAlphaValue, voidRoomInfo.colorAlphaValue, 1, voidRoomInfo.targetBasementTilemap.GetTileMap());//컬러
                }
            }

            ////알파값변환
            //colorAlphaValue = (float)curOxygenValue * 1 / (float)maxOxygenValue;
            ////colorAlphaValue = (float)curOxygenValue * (float)maxOxygenValue * 0.01f;
            //ChangeSpriteOpasity(colorAlphaValue);

            voidRoomInfo.isChangeColor = false;
        }

        /// <summary>
        /// 산소 업데이트
        /// </summary>
        public static void UpdateOxygen(ref VoidRoomInfo voidRoomInfo)
        {
            if (Time.time < voidRoomInfo.actionTime + voidRoomInfo.time)
            {
                return;
            }
            voidRoomInfo.time = Time.time;

            //작동 잘됨
            //변화하지않는 산소인가 == 건축물등의 위치
            if (!voidRoomInfo.isConstOxygen)
            {
                return;
            }
            else if (voidRoomInfo.isOutSideOxygen)
            {
                //빈공간의 산소인가 == 진공
                //curOxygenValue -= 30;
                //isChange = true;
                //RemoveOxygenValue(30);
                voidRoomInfo.RemoveOxygenValue(voidRoomInfo.targetBasementTilemap.outSideRemoveOxygenValue);

                if (voidRoomInfo.isExistPulsingExpolostion)
                {
                    //현재 산소량이 0 초과인가
                    if (0 < voidRoomInfo.curOxygenValue)
                    {
                        voidRoomInfo.targetPulsingExplosion.loopForever = true;
                        voidRoomInfo.targetPulsingExplosion.Activate();
                    }
                    else
                    {
                        voidRoomInfo.targetPulsingExplosion.loopForever = false;
                    }
                }
                
                //LimitOxygenValueCheck();
                UpdateOxygenSprite(ref voidRoomInfo);
                return;
            }
            
            //산소이미지 변경
            UpdateOxygenSprite(ref voidRoomInfo);
            
            //산소랜덤작용인덱스
            int index = 0;
            VoidRoomInfo tempVoidRoomInfo = null;
            switch (voidRoomInfo.targetBasementTilemap.GetGameWorldOperateLogic())
            {
                case GameWorldOperateLogic.TopView:
                    //탑뷰
                    //모든좌표를 랜덤으로 체크
                    index = Random.Range(0, 4);

                    //근처 단일산소에 계산//연산 좋음
                    //산소타일이 존재하는지 체크
                    if (voidRoomInfo.nearCheckOxygenArray[index])
                    {
                        //있으면 메커니즘 작동
                        //tmpOxygen = nearOxygens[indexArray[i]];                    
                        tempVoidRoomInfo = voidRoomInfo.targetBasementTilemap.GetVoidRoomInfo(voidRoomInfo.nearOxygenArray[index]);
                        voidRoomInfo.CalVoidRoom(tempVoidRoomInfo);
                    }

                    //근처모든 산소에 계산//연산더먹는데 느낌은 더 좋다
                    //근처 산소 계산
                    //BuildingManager.Instance.SetRandomIndexArray(ref indexArray);
                    //for (int i = 0; i < nearOxygenArray.Length; i++)
                    //{
                    //    //해당 배열에 근처의 산소가 있는가
                    //    //if (nearCheckOxygen[indexArray[i]])
                    //    if (nearCheckOxygenArray[indexArray[i]])
                    //    {
                    //        //있으면 메커니즘 작동
                    //        //tmpOxygen = nearOxygens[indexArray[i]];                    
                    //        VoidRoomInfo tempVoidRoomInfo = targetBaseMentTilemap.GetVoidRoomInfo(nearOxygenArray[indexArray[i]]);
                    //        CalVoidRoom(tempVoidRoomInfo);
                    //    }
                    //}
                    break;
                case GameWorldOperateLogic.SideView:
                    //사이드뷰
                    //아래를 먼저확인후
                    //좌우를 랜덤으로 확인후
                    //위를 확인

                    //위아래좌우로 받아왔음
                    //0 1 2 3
                    //각 위치확인
                    
                    //아래확인
                    if (voidRoomInfo.nearCheckOxygenArray[1])
                    {
                        tempVoidRoomInfo = voidRoomInfo.targetBasementTilemap.GetVoidRoomInfo(voidRoomInfo.nearOxygenArray[1]);
                        if (tempVoidRoomInfo.isConstOxygen)
                        {
                            //해당산소최대치보다 산소현재량이 낮으면 옮김//나중에 로직변경시킬것
                            voidRoomInfo.CalVoidRoom(tempVoidRoomInfo);
                            return;
                        }
                    }

                    index = Random.Range(2, 4);

                    //좌우 확인
                    if (voidRoomInfo.nearCheckOxygenArray[index])
                    {
                        tempVoidRoomInfo = voidRoomInfo.targetBasementTilemap.GetVoidRoomInfo(voidRoomInfo.nearOxygenArray[index]);
                        if (tempVoidRoomInfo.isConstOxygen)
                        {
                            //해당산소최대치보다 산소현재량이 낮으면 옮김//나중에 로직변경시킬것
                            voidRoomInfo.CalVoidRoom(tempVoidRoomInfo);
                            return;
                        }
                    }

                    //위확인
                    if (voidRoomInfo.nearCheckOxygenArray[0])
                    {
                        tempVoidRoomInfo = voidRoomInfo.targetBasementTilemap.GetVoidRoomInfo(voidRoomInfo.nearOxygenArray[0]);
                        if (tempVoidRoomInfo.isConstOxygen)
                        {
                            //해당산소최대치보다 산소현재량이 낮으면 옮김//나중에 로직변경시킬것
                            voidRoomInfo.CalVoidRoom(voidRoomInfo);
                            return;
                        }
                    }
                    break;
                default:
                    //아무행동없음
                    //이상태면 문제있는거
                    return;
            }

        }

        /// <summary>
        /// 랜덤한 산소처리 인덱스를 반환하는 구역
        /// </summary>
        /// <param name="indexArray">참조할 인덱스</param>
        private static void SetRandomIndexArray(ref int[] indexArray)
        {
            indexList.Clear();
            //랜덤 중복되지않는 수 채우기
            while(true)
            {
                randomIndex = Random.Range(0, 4);
                if (!indexList.Contains(randomIndex))
                {
                    indexList.Add(randomIndex);
                    if (indexList.Count >= 4)
                    {
                        indexArray = indexList.ToArray();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 빈공간정보를 계산하는 함수
        /// </summary>
        /// <param name="voidRoomInfo">타겟이 될빈공간</param>
        private void CalVoidRoom(VoidRoomInfo voidRoomInfo)
        {
            //완벽히 계산하지않음//오차는 그냥 넘겨버렷음

            //Debug.Log("넘버: " + number + nearCheckOxygenArray[indexArray[i]] +""+ voidRoomInfo.targetTilePos + "타겟");

            //해당산소가 변화가 있는 산소인가?
            if (voidRoomInfo.isConstOxygen)
            {
                //타겟 산소량이 현산소량보다 작으면 변경
                if (curOxygenValue > voidRoomInfo.curOxygenValue)
                {
                    //증가 감소                       
                    //targetOxygen.curOxygenValue--;
                    //tmpOxygen.curOxygenValue++;

                    //curOxygenValue -= 10;
                    //isChange = true;
                    //LimitOxygenValueCheck();
                    //RemoveOxygenValue(10);
                    RemoveOxygenValue(targetBasementTilemap.oxygenChangeValue);


                    //tmpOxygen.curOxygenValue += 10;
                    //tmpOxygen.SetIsChange(true);
                    //tmpOxygen.LimitOxygenValueCheck();
                    //voidRoomInfo.AddOxygenValue(10);
                    voidRoomInfo.AddOxygenValue(targetBasementTilemap.oxygenChangeValue);

                    //산소 업데이트
                    //UpdateOxygenSprite();//자신산소 랜더링업데이트
                    UpdateOxygenSprite(ref voidRoomInfo);//상대방산소 랜더링업데이트
                }
            }
        }


        ////최소값과 최대값을 체크하여 현재숫자를 변환
        //public void LimitOxygenValueCheck()
        //{
        //    if (curOxygenValue > maxOxygenValue)
        //    {
        //        curOxygenValue = maxOxygenValue;
        //    }
        //    else if (curOxygenValue < 0)
        //    {
        //        curOxygenValue = 0;
        //    }
        //}

        //건축물에서 작업하는 함수
        //체력이 변하거나 빌딩타입마다 다루는게 다름
        //플로어이면 해당장소를 빈공간으로 만들어버려야함


        /// <summary>
        /// 건축물의 상태에 따른 산소로직 작동방식변경함수
        /// </summary>
        /// <param name="isDestroy">파괴여부</param>
        /// <param name="buildingType">건물타입</param>
        public void SetOxygenAction(bool isDestroy, BuildingType buildingType)
        {
            switch (buildingType)
            {
                case BuildingType.Frame:                    
                case BuildingType.Floor:
                    //파괴되었는가?
                    isOutSideOxygen = isDestroy;
                    isExistPulsingExpolostion = isDestroy;
                    isConstOxygen = true;

                    if (isDestroy)
                    {   
                        targetPulsingExplosion = SuctionPowerManager.Instance.CallSuctionPower(targetBasementTilemap.transform, (Vector3)targetTilePos);
                    }
                    else
                    {
                        SuctionPowerManager.Instance.ReturnSuctionPower(targetPulsingExplosion);
                    }
                    break;
                case BuildingType.Wall:
                case BuildingType.Door:
                    //파괴되었는가?
                    //파괴되었으면 산소가 움직이고 아니면 정지
                    isConstOxygen = isDestroy;
                    break;
                case BuildingType.Building:
                case BuildingType.Pipe:
                    //해당사항없음
                    break;
            }

            if (isOutSideOxygen)
            {
                lLcroweRectTileMapUtil.SetTile(targetTilePos, Color.red, targetBasementTilemap.GetTileMap());
            }
            //else
            //{
            //    lLcroweRectTileMapUtil.SetTile(targetTilePos, Color.white, targetBasementTilemap.GetTileMap());
            //}
        }

        public int GetMaxOxygenValue()
        {
            return targetBasementTilemap.maxOxygenValue;
        }

        public int GetCurOxygenValue()
        {
            return curOxygenValue;
        } 
    }
}
#endif
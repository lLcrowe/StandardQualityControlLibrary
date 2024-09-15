using Pathfinding;
using Pathfinding.RVO;
using System.Collections.Generic;
using UnityEngine;

namespace lLCroweTool.PathFinder
{

    //진형처리를 위치들
    public enum FormationType
    {
        OnePoint,//한 위치로
        //Pack,//해당위치에서 그대로 이동
        Rect,//사각형처리
        Circle,//원형처리
        SemiCircle,//반원형처리
        Charge,//쐐기형태
    }

    //길찾기에셋을 체크하고 래핑한구역//5.0버전
    public static class lLcrowePathfindingUtil
    {
        #region Pathfinder(그래프컴포넌트) 값조절관련
        //-=Pathfinder//길찾기할 영역지정(그래프처리)//건들것들만
        //Max Nearest Node Distance:한 점에서 가장 가까운 노드를 검색할 때 허용되는 거리제한
        //
        //Batch Graph Updates(최적화용):그래프 업데이트를 제한하고 일괄 처리하여 성능을 향상시킵니다. 호출빈도수
        #endregion
        //Awake 중이 아닌 OnEnable 중에 스캔이 발생
        #region 이동스크립트4가지에서 선택관련 (AIPath, FollowerEntity를 권장)
        //1.FollowerEntity(AIPath와 AILerp의 중ㄱ간)
        //2D,3D지원
        //코드 복잡성이 높음
        //지역회피가능
        //Grid,navmesh,recast
        //물리학처리

        //2.AIPath
        //2D,3D지원
        //모든그래프유형
        //지역회피가능
        //물리학처리

        //3. RichAI
        //3D지원
        //navmesh, recast용
        //지역회피가능
        //물리학처리

        //4.AILerp
        //2D,3D지원
        //코드 복잡성이 낮음
        //선형보간처리로 인해 지역회피없음
        //물리학불가
        #endregion
        #region 경로검색 작업관련
        //경로검색
        //-=Seeker.StartPath()//Seeker는 한 번에 하나의 길 찾기 요청만 처리.
        //이전 경로가 완료되기 전에 새로운 경로를 요청하면 이전 경로 요청이 취소.
        //-=StartPath 호출은 경로를 대기열에만 넣는것. 연산은 나중에
        //-=p.BlockUntilCalculated();//즉시경로계산
        //-=path.WaitForPath()//코루틴대기
        #endregion
        //패키지에 포함된 모든 이동 스크립트는 Pathfinding.IAstarAI 인터페이스를 구현되있으니 해당인터페이스로 사용하기
        //4.0버전//Seeker위주였음
        //5.0버전//IAstarAI.SetPath를 사용하기(필수로 변경)//찾는거 컴플리트액션은 따로 뺴고 캐싱해서 제작(IAstar는 이동스크립트에만 존재)
        #region 그리드그래프 게임시작 스캐닝최적화
        //가능하다면 해상도를 낮추십시오.
        //완전히 평평한 세상이라면 높이 테스트를 비활성화하세요.
        //성능 오버헤드가 적으므로 침식을 비활성화합니다.
        //필요하지 않은 경우 충돌 테스트를 비활성화합니다.
        #endregion
        #region 리캐스트 그래프 게임시작 스캐닝최적화
        //셀 크기를 늘립니다.이렇게 하면 그래프가 세계를 복셀화하는 해상도가 감소합니다.
        //타일링을 사용하고 적당한 크기의 타일을 사용하십시오. 64~256복셀 사이의 값이 권장됩니다.
        //타일을 병렬로 스캔할 수 있으므로 멀티코어 컴퓨터에서 속도가 크게 향상될 수 있습니다.
        //그러나 각 타일에는 약간의 오버헤드가 있으므로 타일 크기가 너무 작으면 스캔 속도가 느려집니다.
        //메시 대신 충돌체를 래스터화합니다. 충돌체는 일반적으로 메시보다 훨씬 간단하므로 래스터화하는 속도가 더 빠릅니다.
        //또한 물리 엔진을 효율적으로 사용하기 위해 충돌체를 쿼리할 수 있지만 메시는 쿼리할 수 없으므로 그래프 업데이트 성능이 향상됩니다.
        //필요한 세계 부분만 포함하도록 그래프의 경계 상자를 줄입니다.
        #endregion
        //RVOSquareObstacle에 대한 지원이 제거
        //AIPath 및 RichAI 스크립트는 더 이상 Update 및 FixUpdate 메서드를 사용하지 않음

        #region 참고 스크립트
        //TargetMover//길찾기처리
        //MecanimBridge2D//애님과길찾기컴포넌트 연동을 위한 처리
        //PathTypesDemo//Path관련알아보기 위한것
        //Local
        #endregion
        #region 많이 쓸만한 컴포넌트들
        //DynamicGridObstacle//동적장애물처리
        //PathFinder//맵 길찾기 처리
        //AIPath,FollowerEntity//길찾기 유닛
        //RVO Simulator와 RVO컨트롤러//로컬회피
        //RVO Light//3만개까지 가능//ECS사용
        //ProceduralGraphMover//그래프를 특정위치이동후 새롭게 갱신하는 컴포넌트
        #endregion

        //진형에 대한 경로처리
        //var destinations = PathUtilities.FormationDestinations(ais, newPosition, formationMode, 0.5f);

        //case DemoMode.ABPath:GUILayout.Label("기본 경로. 점 A에서 점 B로 경로를 찾습니다."); break;
        //case DemoMode.MultiTargetPath:GUILayout.Label("다중 대상 경로. 단일 검색에서 한 지점에서 여러 지점으로 빠르게 경로를 찾습니다."); break;
        //case DemoMode.RandomPath:GUILayout.Label("랜덤 경로. 지정된 길이의 경로를 무작위 방향으로 또는 큰 목표 강도를 사용할 때 일부 지점으로 편향하여 찾습니다."); break;
        //case DemoMode.FleePath: GUILayout.Label("도망 경로. 특정 지점에서 도망가려고 시도합니다. 도망 강도를 설정하는 것을 잊지 마세요!"); break;
        //case DemoMode.ConstantPath:GUILayout.Label("일정 값보다 낮은 비용으로 도달할 수 있는 모든 노드를 찾습니다."); break;
        //case DemoMode.FloodPath:GUILayout.Label("특정 지점에서 그래프 전체를 탐색합니다. 그런 다음 FloodPathTracer를 사용하여 해당 지점까지 빠르게 경로를 찾을 수 있습니다."); break;
        //case DemoMode.FloodPathTracer:GUILayout.Label("FloodPath가 시작된 위치로 경로를 추적합니다. 이 경로의 계산 시간을 ABPath와 비교하세요!\nTD 게임에 적합합니다."); break;

        public enum PathType
        {
            ABPath,//A-B로 이동          
            RandomPath,//랜덤//
            FleePath,//타겟에서부터의 도망
            ConstantPath,//청각처리용. 거리감지후 해당위치 길찾기로 닫는지 체크하는방식//또는 막혔는지//3500 == 3 세팅값//턴베이스?
            FloodPath,//연산해서 길들을 다 미리찾아버리는 경로
            FloodPathTracer,//FloodPath로 연산한길들을 사용하여 빠르게 경로를 찾을수있음//실시간연산보다는 미리계산된 데이터에서 처리//AB보다 3.00ms빠름//타워디펜스용
        }




        //방황타입//2번과 4번을 주로 쓸듯
        public enum WanderingAIType
        {
            /// <summary>
            /// 원안에서의 방황
            ///-=장점
            ///매우 빠릅니다.
            ///간단한 코드.
            ///모든 그래프 유형에 대해 합리적으로 잘 작동합니다.
            ///-=단점
            ///지점까지의 실제 거리를 무시합니다. 즉, 세계가 매우 열려 있지 않으면 매우 긴 경로가 생성될 수 있음을 의미합니다(위 비디오 참조).
            ///장애물 내부에 생성된 모든 점은 그래프에서 가장 가까운 점에 맞춰져야 하므로 벽/장애물에 가까운 점 쪽으로 약간의 편향이 있습니다.
            ///포인트를 선택할 때 페널티를 무시합니다(단, 포인트로 가는 경로를 검색할 때는 페널티가 고려됩니다).
            /// </summary>
            RandomInsidePos,

            /// <summary>
            ///-=장점
            ///매우 빠름(위의 일반적인 접근 방식을 사용하지 않는 한)
            ///간단한 코드의 단점
            ///경로 길이에 대한 제어가 거의 없습니다.기타
            ///포인트를 선택할 때 페널티를 무시합니다(단, 포인트로 가는 경로를 검색할 때는 페널티가 고려됩니다).
            /// </summary>
            RandomNodeForGraph,

            RandomPath,//이거위주가 괜찮을듯
            ConstantPath,//제일써야하는거
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ai"></param>
        /// <param name="targetPos"></param>
        /// <param name="onPathComplete"></param>
        public static void SetABPath(this IAstarAI ai, Vector3 targetPos, OnPathDelegate onPathComplete = null)
        {
            var path = ABPath.Construct(ai.position, targetPos, onPathComplete);
            ai.SetPath(path);
        }
        public static void SetABPath(this IAstarAI ai, Vector3 startPos, Vector3 targetPos, OnPathDelegate onPathComplete = null)
        {
            var path = ABPath.Construct(startPos, targetPos, onPathComplete);
            ai.SetPath(path);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ai"></param>
        /// <param name="targetPos"></param>
        /// <param name="searchLength"></param>
        /// <param name="spread"></param>
        /// <param name="aimStrength"></param>
        /// <param name="onPathComplete"></param>
        public static void SetRandomPath(this IAstarAI ai, Vector3 targetPos, int searchLength, int spread, float aimStrength, OnPathDelegate onPathComplete = null)
        {
            var randomPath = RandomPath.Construct(ai.position, searchLength, onPathComplete);
            randomPath.spread = spread;
            randomPath.aimStrength = aimStrength;
            randomPath.aim = targetPos;
            ai.SetPath(randomPath);
        }


        /// <summary>
        /// 접근가능경로인지 확인하는 함수
        /// </summary>
        /// <param name="start">시작위치</param>
        /// <param name="end">도착위치</param>
        /// <returns>접근가능여부</returns>
        public static bool CheckIsReachPath(Vector3 start, Vector3 end)
        {
            //node1 에서 node2 까지 걸어갈 수 있는 경로가 있는지 여부를 반환합니다.
            //이 방법은 미리 계산된 정보만 사용하기 때문에 매우 빠릅니다.
            var node1 = AstarPath.active.GetNearest(start).node;
            var node2 = AstarPath.active.GetNearest(end).node;
            if (node1 == null || node2 == null)
            {
                return false;
            }
            return PathUtilities.IsPathPossible(node1, node2);
        }

        /// <summary>
        /// 소리가 감지됫는지 확인하는 함수
        /// </summary>
        /// <param name="astarAI">길찾기대상</param>
        /// <param name="end">도착위치</param>
        /// <param name="soundDetectOffset">소리감지 오프셋</param>
        /// <returns>소리가 감지됫는지 여부</returns>
        public static bool CheckIsListenPath(this IAstarAI astarAI, Vector3 end, float soundDetectOffset = 2)
        {
            //사운드감지거리//2가 디폴트//3부터는 잘듣는사람
            float soundDetectDistance = lLcroweUtil.GetDistance(astarAI.position, end) + soundDetectOffset;
            float pathDistance = astarAI.remainingDistance;
            return soundDetectDistance > pathDistance;
        }

        /// <summary>
        /// 소리
        /// </summary>
        /// <param name="astarAI">길찾기대상</param>
        /// <param name="end">도착위치</param>
        /// <param name="soundDetectOffset">소리감지 오프셋</param>
        /// <returns>소리가 감지됫는지 여부</returns>
        public static bool CheckIsListenPath(this IAstarAI astarAI, float soundDetectDistance, float soundDetectOffset = 2)
        {   
            float pathDistance = astarAI.remainingDistance;
            return soundDetectDistance + soundDetectOffset > pathDistance;
        }

        //이 메서드를 호출하면 지정된 그래프가 다시 계산됩니다.이 방법은 매우 느리므로(물론 그래프 유형 및 그래프 복잡성에 따라 다름) 가능할 때마다 더 작은 그래프 업데이트를 사용하는 것이 좋습니다.
        public static void MapToAllScan()
        {
            // 모든그래프 재계산
            AstarPath.active.Scan();

            // 첫번째 그래프만 계산
            var graphToScan = AstarPath.active.data.gridGraph;
            AstarPath.active.Scan(graphToScan);

            // 특정그래프계산
            var graphsToScan = new[] { AstarPath.active.data.graphs[0], AstarPath.active.data.graphs[2] };
            AstarPath.active.Scan(graphsToScan);
        }

        public static void MapToScan(Collider collider)
        {
            //길찾기맵을 소규모로 스캔해주는 함수
            AstarPath.active.UpdateGraphs(collider.bounds);
        }

        private static FloodPath lastFloodPath;
        /// <summary>
        /// 맵스캔//타워디팬스용//맵길이 변경되지않는다면 이걸사용해서 처리하기
        /// </summary>
        /// <param name="mapScanPos"></param>
        /// <param name="onPathComplete"></param>
        public static void InitToScanMap(Vector3 mapScanPos, OnPathDelegate onPathComplete = null)
        {
            lastFloodPath = FloodPath.Construct(mapScanPos, onPathComplete);
        }
        /// <summary>
        /// 스캔된맵을 토대로 작동
        /// </summary>
        /// <param name="ai"></param>
        /// <param name="targetPos"></param>
        /// <param name="onPathComplete"></param>
        public static void SetPathToScanMap(this IAstarAI ai, Vector3 targetPos, OnPathDelegate onPathComplete = null)
        {
            if (lastFloodPath == null)
            {
                return;
            }
            FloodPathTracer fpt = FloodPathTracer.Construct(targetPos, lastFloodPath, onPathComplete);
            ai.SetPath(fpt);
        }

        public static void UpdateRVO(this RVOController rvoController, Vector3 dirNormalize, float speed)
        {
            //NPC가 플레이어 제어(또는 외부 제어) 캐릭터를 피하도록 하는 방법
            // RVOController의 속도를 재정의. 이렇게 하면 하나의 시뮬레이션 단계에 대한 로컬 회피 계산이 비활성화됩니다.
            var v = dirNormalize * speed;//dir
            rvoController.velocity = v;
        }

        public static void SetAIRotateValue(this IAstarAI astarAI, bool isUseRotate)
        {
            if (astarAI is AIBase aiBase)
            { aiBase.updateRotation = isUseRotate; return; }
            else if (astarAI is AILerp aiLerp) { aiLerp.updateRotation = isUseRotate; return; }
            else if (astarAI is FollowerEntity follower){follower.updateRotation = isUseRotate; return;}
        }

        public static void EnableAIComponent(this IAstarAI aI, bool value)
        {
            if (aI is AIBase aiBase) aiBase.enabled = value;
            else if (aI is AILerp aiLerp) aiLerp.enabled = value;
            else if (aI is FollowerEntity follower)follower.enabled = value;
        }

        /// <summary>
        /// 현재위치들 그대로 신규위치들로 반환하는 함수 
        /// </summary>
        /// <param name="curPosList">현재위치들</param>
        /// <param name="batchPos">배치위치</param>
        /// <param name="rot">회전값</param>
        /// <returns>신규위치들</returns>
        public static List<Vector3> CalPackFormation(List<Vector3> posList, List<Vector3> curPosList, Vector3 batchPos, Quaternion rot,Vector3 offset)
        {
            //20240407
            //팩은 일단빼버리거자
            //작동이상함
            posList.Clear();
            Vector3 newPos = Vector3.zero;
            var amount = curPosList.Count;
            for (int i = 0; i < curPosList.Count; i++)
            {
               
                newPos = curPosList[i];
                newPos /= amount;
                newPos += batchPos + offset;
                //newPos = lLcroweUtil.GetRotatePosForPivot(newPos, batchPos, rot);

                Debug.Log($"{batchPos},{curPosList[0]},{newPos}");
                posList.Add(newPos);
            }
            
            //계산하고 체크하자


            return posList;
        }

        /// <summary>
        /// 사각형진형으로 위치들을 반환하는 함수
        /// </summary>
        /// <param name="batchPos">배치위치</param>
        /// <param name="rot">회전값</param>
        /// <param name="unitAmount">유닛수</param>
        /// <param name="lineUnitAmount">라인에 있을 유닛수</param>
        /// <param name="spacing">간격</param>
        /// <param name="offset">오프셋</param>
        /// <returns>신규위치들</returns>
        public static List<Vector3> CalRectFormation(List<Vector3> posList, Vector3 batchPos, Quaternion rot, int unitAmount, int lineUnitAmount, float spacing, Vector3 offset)
        {
            int col = unitAmount / lineUnitAmount;
            int row = unitAmount - col;
            if (col == 0 || row == 0)
            {
                return posList;
            }
            return CalRectFormation(posList, batchPos, rot, unitAmount, row, col, spacing, offset);
        }

        /// <summary>
        /// 사각형진형으로 위치들을 반환하는 함수
        /// </summary>
        /// <param name="batchPos">배치위치</param>
        /// <param name="rot">회전값</param>
        /// <param name="unitAmount">유닛수</param>
        /// <param name="numRows">열(넓이)</param>
        /// <param name="numCols">행(높이)</param>
        /// <param name="spacing">간격</param>
        /// <param name="offset">오프셋</param>
        /// <returns>신규위치들</returns>
        public static List<Vector3> CalRectFormation(List<Vector3> posList, Vector3 batchPos, Quaternion rot, int unitAmount, int numRows, int numCols, float spacing, Vector3 offset)
        {
            if (numCols == 0 || numRows == 0)
            {
                return posList;
            }
            posList.Clear();

            //col이 먼저 채워짐
            //numrow&numCols를 재체크하는 수식필요
            var value = unitAmount % numCols;
            var divideValue = unitAmount / numCols;
            numRows = value == 0 ? divideValue : divideValue + 1;
            //Debug.Log($"{numRows},{numCols}");


            // 진형의 가로 길이와 세로 길이 계산//재체크한거에서 해야지 원하는 센터값이 나옴
            float totalWidth = (numCols - 1) * spacing;
            float totalHeight = (numRows - 1) * spacing;

            // 진형의 시작 위치
            Vector3 center = batchPos - new Vector3(totalHeight / 2, totalWidth / 2, 0f);

            // 진형을 구성하면서 각 위치 계산
            int count = 0;
            for (int row = 0; row < numRows; row++)
            {
                if (count >= unitAmount)
                {
                    break;
                }
                for (int col = 0; col < numCols; col++)
                {
                    Vector3 position = center + new Vector3(row* spacing, col * spacing, 0f) + offset;
                    position = lLcroweUtil.GetRotatePosForPivot(position, batchPos, rot);
                    posList.Add(position);
                    count++;
                    
                    if (count >= unitAmount)
                    {
                        break;
                    }
                }
            }
            return posList;
        }

        /// <summary>
        /// 쐐기형진형으로 위치들을 반환하는 함수
        /// </summary>
        /// <param name="batchPos">배치위치</param>
        /// <param name="rot">회전값</param>
        /// <param name="numUnits">유닛수</param>
        /// <param name="angle">쐐기각</param>
        /// <param name="spacing">간격</param>
        /// <returns>신규위치들</returns>
        public static List<Vector3> CalChargeFormation(List<Vector3> posList, Vector3 batchPos, Quaternion rot, int numUnits, float angle, float spacing, Vector3 offset)
        {
            posList.Clear();
            Vector3 currentPosition = batchPos;
            var conSideTrueQ = Quaternion.AngleAxis(angle, Vector3.forward) * -Vector3.up * spacing;
            var conSideFalseQ = Quaternion.AngleAxis(-angle, Vector3.forward) * -Vector3.up * spacing;
            int PositiveSideIndex = 1;
            int negativeSideIndex = 1;
            bool coneSide = true;

            posList.Add(currentPosition + offset);
            for (int i = 1; i < numUnits; i++)
            {

                if (coneSide)
                {
                    currentPosition = (conSideTrueQ * PositiveSideIndex) + batchPos;
                    PositiveSideIndex += 1;
                    coneSide = false;
                }
                else
                {
                    currentPosition = (conSideFalseQ * negativeSideIndex) + batchPos;
                    negativeSideIndex += 1;
                    coneSide = true;
                }
                currentPosition = lLcroweUtil.GetRotatePosForPivot(currentPosition + offset, batchPos, rot);
                posList.Add(currentPosition);
            }

            return posList;
        }

        /// <summary>
        /// 원형진형으로 위치들을 반환하는 함수
        /// </summary>
        /// <param name="batchPos">배치위치</param>
        /// <param name="rot">회전값</param>
        /// <param name="addAngle">추가회전값</param>
        /// <param name="radius">반원</param>
        /// <param name="numUnits">유닛수</param>
        /// <returns>신규위치들</returns>
        public static List<Vector3> CalCircleFormation(List<Vector3> posList, Vector3 batchPos, Quaternion rot, float addAngle, float radius, int numUnits, Vector3 offset)
        {
            posList.Clear();
            //각도간격 계산
            float angleStep = 360f / numUnits;

            //위치 계산
            for (int i = 0; i < numUnits; i++)
            {
                float angle = i * angleStep + 90 + addAngle;
                float radian = angle * Mathf.Deg2Rad;
                float x = batchPos.x + radius * Mathf.Cos(radian);
                float y = batchPos.y + radius * Mathf.Sin(radian);

                Vector3 position = new Vector3(x, y, batchPos.z) + offset;
                position = lLcroweUtil.GetRotatePosForPivot(position, batchPos, rot);
                posList.Add(position);
            }

            return posList;
        }

        /// <summary>
        /// 반원진형으로 위치들을 반환하는 함수
        /// </summary>
        /// <param name="batchPos">배치위치</param>
        /// <param name="rot">회전값</param>
        /// <param name="addAngle">추가회전값</param>
        /// <param name="radius">반경</param>
        /// <param name="numUnits">유닛수</param>
        /// <returns>신규위치들</returns>
        public static List<Vector3> CalSemiCircleFormation(List<Vector3> posList, Vector3 batchPos, Quaternion rot, float addAngle, float radius, int numUnits, Vector3 offset)
        {
            posList.Clear();

            if (numUnits == 1)
            {
                posList.Add(batchPos);
                return posList;
            }

            //각도간격 계산
            float angleStep = 180f / (numUnits - 1);

            //위치 계산
            for (int i = 0; i < numUnits; i++)
            {
                float angle = i * angleStep + 0 + addAngle;
                float radian = angle * Mathf.Deg2Rad;
                float x = batchPos.x + radius * Mathf.Cos(radian);
                float y = batchPos.y + radius * Mathf.Sin(radian);

                Vector3 position = new Vector3(x, y, batchPos.z) + offset;
                position = lLcroweUtil.GetRotatePosForPivot(position, batchPos, rot);
                posList.Add(position);
            }

            return posList;
        }

        //길찾기 래핑유틸//여기수정해야됨//Path로 처리
        public static Vector3 PickRandomPoint3D(Vector3 pos, float radius)
        {
            pos += Random.insideUnitSphere * radius;
            pos.y = 0;
            return pos;
        }
        public static Vector2 PickRandomPoint2D(Vector2 pos, float radius)
        {
            pos += Random.insideUnitCircle * radius;
            return pos;
        }

        public static void Stop(IAstarAI ai)
        {
            //정지
            ai.destination = ai.position;
            ai.SearchPath();
        }
        public static void Move(IAstarAI ai)
        {
            //이동

            //ai.
            //ai.SearchPath();
        }


        //방황AI 이동
        public static void WandonMove(IAstarAI ai, WanderingAIType wanderingAIType)
        {
            //switch (wanderingAIType)
            //{
            //    case WanderingAIType.RandomInsidePos:
            //        if (!ai.pathPending && (ai.reachedEndOfPath || !ai.hasPath))
            //        {
            //            ai.destination = PickRandomPoint3D(ai.position, 3);
            //            ai.SearchPath();
            //        }
            //        break;
            //    case WanderingAIType.RandomNodeForGraph:
            //        GraphNode randomNode;

            //        // For grid graphs
            //        var grid = AstarPath.active.data.gridGraph;

            //        randomNode = grid.nodes[Random.Range(0, grid.nodes.Length)];

            //        // For point graphs
            //        var pointGraph = AstarPath.active.data.pointGraph;
            //        randomNode = pointGraph.nodes[Random.Range(0, pointGraph.nodes.Length)];

            //        // Works for ANY graph type, however this is much slower
            //        var graph = AstarPath.active.data.graphs[0];
            //        // Add all nodes in the graph to a list
            //        List<GraphNode> nodes = new List<GraphNode>();
            //        graph.GetNodes((System.Action<GraphNode>)nodes.Add);
            //        randomNode = nodes[Random.Range(0, nodes.Count)];

            //        // Use the center of the node as the destination for example
            //        var destination1 = (Vector3)randomNode.position;
            //        // Or use a random point on the surface of the node as the destination.
            //        // This is useful for navmesh-based graphs where the nodes are large.
            //        var destination2 = randomNode.RandomPointOnSurface();
            //        break;
            //    case WanderingAIType.RandomPath:
            //        // Disable the agent's internal automatic path recalculation
            //        ai.canSearch = false;
            //        var searchLength = 5; //최소 경로 비용설저
            //        RandomPath path = RandomPath.Construct(ai.position, searchLength);
            //        path.spread = 5;
            //        // Give the agent a path to calculate and follow
            //        ai.SetPath(path);
            //        break;
            //    case WanderingAIType.ConstantPath:
            //        //일 경로 요청으로 여러 임의 지점을 선택
            //        //RandomPath 유형 보다 약간 빠름.
            //        //많은 노드를 검색해야 하면 속도가 느려질 수 있습니다.

            //        ConstantPath path = ConstantPath.Construct(transform.position, searchLength);

            //        AstarPath.StartPath(path);
            //        path.BlockUntilCalculated();
            //        var singleRandomPoint = PathUtilities.GetPointsOnNodes(path.allNodes, 1)[0];
            //        var multipleRandomPoints = PathUtilities.GetPointsOnNodes(path.allNodes, 100);

            //        break;
            //        case WanderingAIType.Cons
            //}
        }


    }
}

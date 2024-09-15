#if MEC
using lLCroweTool.Dictionary;
using MEC;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;
using lLCroweTool.TileMap;

namespace lLCroweTool.TerrainSystem.TestFloodFill
{
    public class ConnectPartDestroyForMainFrame : MonoBehaviour
    {
        //타일맵을 사용한 부서짐 구현 체크하기
        //연결부위가 박살나면 or 두개의 큰 오브젝트가 분리 됫을때
        //자료조사가 더필요해보긴한데
        //1.특정타일이 있으면 파괴가 안되게(선택됨)
        //2.분리된 두개의 타일맵을 체크했을때 누가 더큰지. 작은 타일맵은 파괴or 분리

        //바닥이 박살나면 위에건물은 무조건 박살나게 된다고 치면
        //바닥을 기준으로 할수밖에 없는거 같은데


        //타일맵에 붙착시키는 컴포넌트

        //메인프레임관련
        public Tilemap targetTileMap;//바닥이 될 타일맵
        public Tile targetMainFrameTile;//특정타일에 연결되있으면 안부셔짐

        //파괴했을시 변경될색깔과 타일//일단은 색깔만
        public Color[] partDestroyColor;

        //외부컴포넌트
        public UniversalExplosion universalExplosion;//태그필터사용해서 특정태그만 처리하게 해줘야됨//30거리10파워

        private static List<Vector3Int> refPosList= new List<Vector3Int>(1000);//특정 타일과 연결됫는지 확인하는
        private static List<Vector3Int> checkList = new List<Vector3Int>(1000);//한번씩 확인한 대상들
        private static Queue<Vector3Int> queue = new Queue<Vector3Int>(1000);//확인할 대상들


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                var mousePos = InPutKeySystem.Instance.mouseWorldPosition;
                Vector3Int temp = lLcroweRectTileMapUtil.GetWorldToCell(mousePos, targetTileMap);//월드 투 셀 위치로 변환

                //아마 작동될떄 해당 위치의 타일이 파괴되고 주변의 타일들을 체크함
                //Timing.RunCoroutine(CheckConnectPartDestroyForMainFrame_Coroutine(temp, this));

                CheckConnectPartDestroyForMainFrame(temp, this);

                universalExplosion.transform.position = mousePos;
                universalExplosion.Invoke("Activate", 0.1f);
                universalExplosion.Activate();
            }
        }

        //20221214
        //이 부분을 코루틴으로 처리해도될거 같기도하고 아닌거 같기도한데
        //실제 처리해야될 부분이 타일맵의 부서진 위치에서 상하좌우를 각각체크해야되서 코루틴처리하는게 맞아보이는거 같기도한
        //테스트해봣는데 현재 최대 0.05정도 부하가 있긴함//코루틴으로 할가능성도 높음

        /// <summary>
        /// 특정위치가 파괴되었을때 근처의 연결부위들을 체크하는 함수 코루틴
        /// </summary>
        /// <param name="destoryPos">파괴된 위치</param>
        /// <param name="mainFrame">타겟팅할 메인프레임</param>
        public static IEnumerator<float> CheckConnectPartDestroyForMainFrame_Coroutine(Vector3Int destoryPos, ConnectPartDestroyForMainFrame mainFrame)
        {
            //주변타일들을 가져와서 체크                                                                                                      
            Vector3Int[] tempArray = lLcroweRectTileMapUtil.GetSideTilePos(destoryPos);
            for (int i = 0; i < tempArray.Length; i++)
            {
                ActionConnectDestroyForMainFrame(tempArray[i], mainFrame);
                yield return Timing.WaitForOneFrame;
                //yield return Timing.WaitForSeconds(0.1f);
            }
        }

        /// <summary>
        /// 특정위치가 파괴되었을때 근처의 연결부위들을 체크하는 함수
        /// </summary>
        /// <param name="destoryPos">파괴된 위치</param>
        /// <param name="mainFrame">타겟팅할 메인프레임</param>
        public static void CheckConnectPartDestroyForMainFrame(Vector3Int destoryPos, ConnectPartDestroyForMainFrame mainFrame)
        {
            //주변타일들을 가져와서 체크                                                                                                      
            Vector3Int[] tempArray = lLcroweRectTileMapUtil.GetSideTilePos(destoryPos);
            for (int i = 0; i < tempArray.Length; i++)
            {
                ActionConnectDestroyForMainFrame(tempArray[i], mainFrame);
            }
        }

        /// <summary>
        /// 함선 연결부위 파괴에 의한 파츠분리 구현
        /// </summary>
        /// <param name="checkPos">확인할 위치</param>
        /// <param name="mainFrame">메인프레임</param>
        public static void ActionConnectDestroyForMainFrame(Vector3Int checkPos, ConnectPartDestroyForMainFrame mainFrame)
        {
            Tilemap targetTileMap = mainFrame.targetTileMap;
            if (!CheckConnectTileMap(checkPos, mainFrame.targetMainFrameTile, targetTileMap, ref refPosList))
            {
                //연결이 안되있으면 복사 및 파츠분리

                //데브리가 될 새타일맵 한개 생성
                GameObject go = new GameObject();
                Tilemap newTileMap = go.AddComponent<Tilemap>();
                TilemapRenderer tilemapRenderer = go.AddComponent<TilemapRenderer>();
                //자동적으로 만들어짐//박스도 테스트해봣지만 비쥬얼적으로 이게 좋음
                Collider2D collider2D = go.AddComponent<TilemapCollider2D>();                
                Rigidbody2D rb2d = go.AddComponent<Rigidbody2D>();
                go.transform.parent = targetTileMap.transform.parent;

                //천천히 사라지게
                Color color = new Color(1, 1, 1, 0);
                tilemapRenderer.material.DOColor(color, Random.Range(5f, 8f));

                //랜덤 트리거
                collider2D.isTrigger = Random.Range(0, 2) == 0 ? true : false;

                //크기에 따른 회전체크
                //회전 저항 높히기//기본 0.05
                rb2d.angularDrag = refPosList.Count * 0.05f;

                //원래타일맵에서 지정된 위치의 타일들을 복사
                Vector3Int[] OriginPosArray = refPosList.ToArray();
                Vector3Int[] newPosArray = refPosList.ToArray();

                //물리작동을 위한 위치 지정
                //원점기준 새롭게//복제한 타일들의 중심점 - 새로나올 위치 = 원점
                Vector2 centerPos = lLcroweUtil.Centroid(newPosArray);
                for (int i = 0; i < newPosArray.Length; i++)
                {
                    newPosArray[i] -= new Vector3Int((int)centerPos.x, (int)centerPos.y);
                }

                //원점에 대한 위치지정
                Vector3Int newPos = new Vector3Int((int)centerPos.x, (int)centerPos.y);
                go.transform.position = newPos;

                //색깔처리
                Color[] colorArray = mainFrame.partDestroyColor;
                int colorLen = colorArray.Length;
                
                //복사할 위치들을 집어넣음.
                for (int i = 0; i < newPosArray.Length; i++)
                {
                    //해당타일의 특정위치에 있는 타일가져옴
                    TileBase tile = lLcroweRectTileMapUtil.GetTile(OriginPosArray[i], mainFrame.targetTileMap);

                    //복제한 타일맵에 타일세팅
                    lLcroweRectTileMapUtil.SetTile(newPosArray[i], tile, newTileMap);
                    lLcroweRectTileMapUtil.SetTile(newPosArray[i], colorArray[Random.Range(0, colorLen)], newTileMap);
                    
                }

                //물리작동이 필요 없을시 위 로직은 넘기고 원래 위치만 가져오면됨
                //DuplicationTileMap(targetTileMap, newTilemap, posArray);//원본만 복사됨

                //삭제//원래타일에서 지워야할 위치들을 지정
                DeleteTileMap(targetTileMap, OriginPosArray);

                //물리작동//외부에서 할 가능성이 높긴한데
                //내부에서 하는것도 괜찬아보임
            }
        }

        /// <summary>
        /// 시작위치와 특정타일이 연결됫는지 체크하는 함수
        /// </summary>
        /// <param name="startPos">검색 시작위치</param>
        /// <param name="checkTile">찾아볼 타일</param>
        /// <param name="terrainTilemap">타겟팅할 타일맵</param>
        /// <param name="refPosList">연결된 구간들을 체크</param>
        /// <returns>연결됫는지 체크</returns>
        private static bool CheckConnectTileMap(Vector3Int startPos, TileBase checkTile, Tilemap terrainTilemap, ref List<Vector3Int> refPosList)
        {
            //바닥이 빈칸인지 체크
            if (!lLcroweRectTileMapUtil.GetIsExistTile(startPos, terrainTilemap))
            {
                //빈칸이면 아무행동못하게 True로 반환
                return true;
            }

            //시작구역이 찾는 타일인지 체크
            if (lLcroweRectTileMapUtil.GetTile(startPos, terrainTilemap) == checkTile)
            {
                //찾아야될 구역이 있으면 //파괴하지않음
                return true;
            }
          

            checkList.Clear();//한번씩 확인한 대상들 초기화
            queue.Clear();//확인할 대상들 초기화
            refPosList.Clear();//참조할 대상들 초기화

            int tileCount = 0;//계산한 타일수
            bool isCheck = false;
            //Color newColor = Random.ColorHSV();

            //첫대상을 집어넣고 작동시킴
            queue.Enqueue(startPos);
            //lLcroweUtil.SetTile(startPos, newColor, terrainTilemap);

            do
            {
                //지형이 존재하는지 체크
                Vector3Int targetTerrainPos = queue.Dequeue();

                if (checkList.Contains(targetTerrainPos))
                {
                    continue;
                }
                tileCount++;
                checkList.Add(targetTerrainPos);//체크리스트에 등록
                refPosList.Add(targetTerrainPos);

                //주변타일 체크
                Vector3Int[] tempPosArray = lLcroweRectTileMapUtil.GetSideTilePos(targetTerrainPos);
                bool[] tempArray = lLcroweRectTileMapUtil.GetSideTileIsHas(tempPosArray, terrainTilemap);

                for (int i = 0; i < tempArray.Length; i++)
                {
                    //타일이 존재하는지 체크
                    if (!tempArray[i])
                    {
                        //존재하지 않았으니 앞으로 체크안할곳이므로 체크리스트에 등록
                        if (!checkList.Contains(tempPosArray[i]))
                        {
                            checkList.Add(tempPosArray[i]);
                        }
                        //넘어가기
                        continue;
                    }

                    if (checkList.Contains(tempPosArray[i]))
                    {
                        //체크리스트에 등록되있으므로 더 체크할 필요없음
                        continue;
                    }

                    if (lLcroweRectTileMapUtil.GetTile(tempPosArray[i], terrainTilemap)  == checkTile)
                    {
                        //찾아야될 구역이 있으면 //파괴하지않음
                        isCheck = true;
                        break;
                    }

                    //검색추가
                    queue.Enqueue(tempPosArray[i]);
                    //lLcroweUtil.SetTile(tempPosArray[i], newColor, terrainTilemap);
                }
            } while (queue.Count > 0);

            return isCheck;
        }

        //위치를 체크하는 함수
        ///// <summary>
        ///// 시작위치와 연결될 위치가 연결됫는지 체크하는 함수
        ///// </summary>
        ///// <param name="startPos">검색 시작위치</param>
        ///// <param name="checkTilePos">찾아볼 위치</param>
        ///// <param name="terrainTilemap">타겟팅할 타일맵</param>
        ///// <param name="refPosList">연결된 구간들을 체크</param>
        ///// <returns>연결됫는지 체크</returns>
        //public bool CheckConnectTileMap(Vector3Int startPos, Vector3Int checkTilePos, Tilemap terrainTilemap, ref List<Vector3Int> refPosList)
        //{
        //    //바닥이 빈칸인지 체크
        //    if (!lLcroweUtil.GetIsExistTile(startPos, terrainTilemap))
        //    {
        //        //빈칸이면 아무행동못하게 True로 반환
        //        return true;
        //    }

        //    //시작구역이 찾는 타일인지 체크
        //    if (startPos == checkTilePos)
        //    {
        //        //찾아야될 구역이 있으면 //파괴하지않음
        //        return true;
        //    }

        //    checkList.Clear();//한번씩 확인한 대상들 초기화
        //    queue.Clear();//확인할 대상들 초기화
        //    refPosList.Clear();//참조할 대상들 초기화

        //    int tileCount = 0;//계산한 타일수
        //    bool isCheck = false;

        //    //첫대상을 집어넣고 작동시킴
        //    queue.Enqueue(startPos);
        //    //

        //    do
        //    {
        //        //지형이 존재하는지 체크
        //        Vector3Int targetTerrainPos = queue.Dequeue();

        //        if (checkList.Contains(targetTerrainPos))
        //        {
        //            continue;
        //        }
        //        tileCount++;
        //        checkList.Add(targetTerrainPos);//체크리스트에 등록
        //        refPosList.Add(targetTerrainPos);

        //        //주변타일 체크
        //        Vector3Int[] tempPosArray = lLcroweUtil.GetSideTilePos(targetTerrainPos, terrainTilemap);
        //        bool[] tempArray = lLcroweUtil.GetSideTileIsHas(tempPosArray, terrainTilemap);

        //        for (int i = 0; i < tempArray.Length; i++)
        //        {   
        //            //타일이 존재하는지 체크
        //            if (!tempArray[i])
        //            {
        //                //존재하지 않았으니 앞으로 체크안할곳이므로 체크리스트에 등록
        //                if (!checkList.Contains(tempPosArray[i]))
        //                {
        //                    checkList.Add(tempPosArray[i]);
        //                }
        //                //넘어가기
        //                continue;
        //            }

        //            if (checkList.Contains(tempPosArray[i]))
        //            {
        //                //체크리스트에 등록되있으므로 더 체크할 필요없음
        //                continue;
        //            }

        //            if (tempPosArray[i] == checkTilePos)
        //            {
        //                //찾아야될 구역이 있으면 //파괴하지않음
        //                isCheck = true;
        //                break;
        //            }

        //            //검색추가
        //            queue.Enqueue(tempPosArray[i]);
        //        }           
        //    } while (queue.Count > 0);

        //    return isCheck;
        //}

        /// <summary>
        /// 타일맵에서 지정한 위치삭제
        /// </summary>
        /// <param name="tilemap">타겟팅할 타일맵</param>
        /// <param name="posArray">지정할 위치배열들</param>
        public static void DeleteTileMap(Tilemap tilemap, Vector3Int[] posArray)
        {
            //타일맵에서 특정 위치들을 단체로 없애버리기 위함
            for (int i = 0; i < posArray.Length; i++)
            {
                lLcroweRectTileMapUtil.SetTile(posArray[i], null, tilemap);
            }
        }

        /// <summary>
        /// 타일맵에서 지정한 위치들을 복제
        /// </summary>
        /// <param name="dupTilemap">복제할 타일맵</param>
        /// <param name="newTileMap">복제한거를 붙일 타일맵</param>
        /// <param name="posArray">지정할 위치배열들</param>
        /// <returns>신규타일맵</returns>
        public static Tilemap DuplicationTileMap(Tilemap dupTilemap, Tilemap newTileMap, Vector3Int[] posArray)
        {   
            //복사할 위치들을 집어넣음.
            for (int i = 0; i < posArray.Length; i++)
            {
                Vector3Int tempPos = posArray[i];
                //해당타일의 특정위치에 있는 타일가져옴
                TileBase tile = lLcroweRectTileMapUtil.GetTile(tempPos, dupTilemap);

                //복제한 타일맵에 타일세팅
                lLcroweRectTileMapUtil.SetTile(tempPos, tile, newTileMap);
            }
            return newTileMap;
        }
    }
    [System.Serializable] public class TileBible : CustomDictionary<Vector3Int, CustomTileInfo> { }
    [System.Serializable]
    public class CustomTileInfo
    {
        public TerrainType terrainType;
        public WallType wallType;
        public RoofType roofType;
    }

    public enum TerrainType
    {
        Empty,//아무것도 없음
        Land,//땅
    }

    public enum WallType
    {
        Empty,//아무것도없음
        Wall,//벽//
        Door,//문//방체크할때는 통과안함//산소였을때는..생각해봐야겠네
    }

    public enum RoofType
    {
        Clear,//아무것도 없음
        Roof,//지붕
    }
}
#endif
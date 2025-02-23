using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using lLCroweTool.Dictionary;
using lLCroweTool.TileMap;
using lLCroweTool.InputKey;

namespace lLCroweTool.TerrainSystem.TestFloodFill
{

    public class TestFloodFill : MonoBehaviour
    {
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
            Moutain,//산
        }


        public TileBible tileBible = new TileBible();

        public Tilemap terrainTilemap;//지형이 있는 타일맵
        public List<Vector3Int> terrainTilePosList = new List<Vector3Int>();//현재 
        public Tilemap wallTilemap;//벽이 지어져있는 타일맵
        public List<Vector3Int> wallTilePosList = new List<Vector3Int>();//현재 



        //최대 체크칸//칸수로 몇개이하일떄 칸으로 인정받는
        public int checkMaxTileCount = 10;//10칸으로 지정
        public float delay;

        //타일을 두개사용하는게 좋아보임

        //FloodFill//홍수채우기
        //4방항 8방향 
        //4방향 선택

        //DFS//BFS//BFS가 더선호됨//대충봐도 BFS가 더맘에 듬
        //결론은 채우는 알고리즘이네 체크

        //음 이론보다는 절차로 푸는게 더 재밌따.//재밌는게 짱이다
        //https://kangworld.tistory.com/46
        //여기가 제일 맘에듬
        //1. Queue는 비어있고 그래프, 타겟이 있다
        //2. 0번 정점부터 시작한다면 0번을 큐에 삽입한다
        //3. Queue에서 0번을 빼고 인접한 번호(1,3)들을 Queue에 삽입
        //4. Queue에 1을 빼고 인접한 2를 집어넣음()

        //임시로 주석처리하고 


        /// <summary>
        /// 타일맵 플로이드필 색갈변경 알고리즘
        /// </summary>
        /// <param name="terrainTilemap">색칠할 지형타일맵</param>
        /// <param name="wallTilemap">방해물이 있는 벽타일맵</param>
        /// <param name="startPos">시작하는 위치</param>
        /// <param name="newColor">새로운 색상 지정</param>
        /// <param name="checkMaxTileCount">체크할 최대타일수</param>
        /// <param name="delay">딜레이</param>
        private IEnumerator FloodFill_coroutine(Tilemap terrainTilemap, Tilemap wallTilemap, Vector3Int startPos, Color newColor, int checkMaxTileCount, float delay)
        {
            //빈칸인지 체크
            if (lLcroweRectTileMapUtil.GetIsExistTile(startPos, wallTilemap))
            {
                yield break;
            }

            WaitForSeconds wait = new WaitForSeconds(delay);
            List<Vector3Int> checkList = new List<Vector3Int>();//한번씩 확인한 대상들
            Queue<Vector3Int> queue = new Queue<Vector3Int>();//확인할 대상들
                                                              //bool isRoom = true;//방인지 체크여부
            int tileCount = 0;//계산한 타일수

            //첫대상을 집어넣고 작동시킴
            queue.Enqueue(startPos);
            lLcroweRectTileMapUtil.SetTile(startPos, newColor, terrainTilemap);

            do
            {
                //지형과 벽이 있는데
                //지형을 먼저 체크하고 벽이있는지 체크
                Vector3Int targetTerrainPos = queue.Dequeue();

                if (checkList.Contains(targetTerrainPos))
                {
                    continue;
                }
                tileCount++;
                checkList.Add(targetTerrainPos);//체크리스트에 등록

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

                    //지형이 존재하면 벽이 있는지 체크
                    //벽이 있으면 큐에 추가안하고 넘어감
                    if (!lLcroweRectTileMapUtil.GetIsExistTile(tempPosArray[i], wallTilemap))
                    {
                        //벽이 없고 체크한적이 없으면 큐에 추가하고 해당색깔을 변경
                        queue.Enqueue(tempPosArray[i]);
                        lLcroweRectTileMapUtil.SetTile(tempPosArray[i], newColor, terrainTilemap);
                    }
                    else
                    {
                        //근처벽까지 체크
                        lLcroweRectTileMapUtil.SetTile(tempPosArray[i], Color.red, terrainTilemap);
                    }
                }
                yield return wait;

                //체크할 타일수량보다 더크면 멈춤
                if (tileCount > checkMaxTileCount)
                {
                    //방이 아니다 이말이야
                    //isRoom = false;
                    break;
                }
            } while (queue.Count > 0);

            //Debug.Log($"방 체크여부 => {isRoom }, 몇개를 체크했는가 => {tileCount}");
        }

        private static List<Vector3Int> checkList = new List<Vector3Int>(1000);//한번씩 확인한 대상들
        private static Queue<Vector3Int> queue = new Queue<Vector3Int>(1000);//확인할 대상들

        /// <summary>
        /// 타일맵 플로이드필 색갈변경 알고리즘
        /// </summary>
        /// <param name="terrainTilemap">색칠할 지형타일맵</param>
        /// <param name="wallTilemap">방해물이 있는 벽타일맵</param>
        /// <param name="startPos">시작하는 위치</param>
        /// <param name="newColor">새로운 색상 지정</param>
        /// <param name="checkMaxTileCount">체크할 최대타일수</param>
        private void FloodFill(Tilemap terrainTilemap, Tilemap wallTilemap, Vector3Int startPos, Color newColor, int checkMaxTileCount)
        {
            //빈칸인지 체크
            if (lLcroweRectTileMapUtil.GetIsExistTile(startPos, wallTilemap))
            {
                return;
            }

            checkList.Clear();//한번씩 확인한 대상들 초기화
            queue.Clear();//확인할 대상들 초기화
                          //bool isRoom = true;//방인지 체크여부
            int tileCount = 0;//계산한 타일수

            //첫대상을 집어넣고 작동시킴
            queue.Enqueue(startPos);
            lLcroweRectTileMapUtil.SetTile(startPos, newColor, terrainTilemap);

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

                    //지형이 존재하면 벽이 있는지 체크
                    //벽이 있으면 큐에 추가안하고 넘어감
                    if (!lLcroweRectTileMapUtil.GetIsExistTile(tempPosArray[i], wallTilemap))
                    {
                        //벽이 없고 체크한적이 없으면 큐에 추가하고 해당색깔을 변경
                        queue.Enqueue(tempPosArray[i]);
                        lLcroweRectTileMapUtil.SetTile(tempPosArray[i], newColor, terrainTilemap);
                    }
                }

                //체크할 타일수량보다 더크면 멈춤
                if (tileCount > checkMaxTileCount)
                {
                    //방이 아니다 이말이야
                    //isRoom = false;
                    break;
                }
            } while (queue.Count > 0);

            //Debug.Log($"방 체크여부 => {isRoom }, 몇개를 체크했는가 => {tileCount}");
        }

        /// <summary>
        /// Room인 여부를 체크하는 함수
        /// </summary>
        /// <param name="terrainTilemap">지형타일맵</param>
        /// <param name="wallTilemap">벽타일맵</param>
        /// <param name="startPos">시작위치</param>
        /// <param name="checkMaxTileCount">최대 체크할 타일수</param>
        /// <returns>Room 여부</returns>
        public bool CheckTileMapForRoom(Tilemap terrainTilemap, Tilemap wallTilemap, Vector3Int startPos, int checkMaxTileCount)
        {
            //빈칸인지 체크
            if (lLcroweRectTileMapUtil.GetIsExistTile(startPos, wallTilemap))
            {
                return false;
            }

            checkList.Clear();//한번씩 확인한 대상들 초기화
            queue.Clear();//확인할 대상들 초기화
            bool isRoom = true;//방인지 체크여부
            int tileCount = 0;//계산한 타일수

            //첫대상을 집어넣고 작동시킴
            queue.Enqueue(startPos);

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

                    //지형이 존재하면 벽이 있는지 체크
                    //벽이 있으면 큐에 추가안하고 넘어감
                    if (!lLcroweRectTileMapUtil.GetIsExistTile(tempPosArray[i], wallTilemap))
                    {
                        //벽이 없고 체크한적이 없으면 큐에 추가하고 해당색깔을 변경
                        queue.Enqueue(tempPosArray[i]);
                    }
                }

                //체크할 타일수량보다 더크면 멈춤
                if (tileCount > checkMaxTileCount)
                {
                    //방이 아니다 이말이야
                    isRoom = false;
                    break;
                }
            } while (queue.Count > 0);

            //Debug.Log($"방 체크여부 => {isRoom }, 몇개를 체크했는가 => {tileCount}");
            return isRoom;
        }

        /// <summary>
        /// Room인 여부를 체크하는 함수
        /// </summary>
        /// <param name="terrainTilemap">지형타일맵</param>
        /// <param name="wallTilemap">벽타일맵</param>
        /// <param name="startPos">시작위치</param>
        /// <param name="checkMaxTileCount">최대 체크할 타일수</param>
        /// <param name="refGetTilePosList">타일맵</param>
        /// <returns>Room인 여부</returns>
        public bool CheckTileMapForRoom(Tilemap terrainTilemap, Tilemap wallTilemap, Vector3Int startPos, int checkMaxTileCount, ref List<Vector3Int> refGetTilePosList)
        {
            //빈칸인지 체크
            if (lLcroweRectTileMapUtil.GetIsExistTile(startPos, wallTilemap))
            {
                return false;
            }

            checkList.Clear();//한번씩 확인한 대상들 초기화
            queue.Clear();//확인할 대상들 초기화
            refGetTilePosList.Clear();
            bool isRoom = true;//방인지 체크여부
            int tileCount = 0;//계산한 타일수

            //첫대상을 집어넣고 작동시킴
            queue.Enqueue(startPos);
            refGetTilePosList.Add(startPos);

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

                    //지형이 존재하면 벽이 있는지 체크
                    //벽이 있으면 큐에 추가안하고 넘어감
                    if (!lLcroweRectTileMapUtil.GetIsExistTile(tempPosArray[i], wallTilemap))
                    {
                        //벽이 없고 체크한적이 없으면 큐에 추가하고 해당색깔을 변경
                        queue.Enqueue(tempPosArray[i]);
                        refGetTilePosList.Add(tempPosArray[i]);
                        //lLcroweUtil.SetTile(tempPosArray[i], newColor, terrainTilemap);
                    }
                }

                //체크할 타일수량보다 더크면 멈춤
                if (tileCount > checkMaxTileCount)
                {
                    //방이 아니다 이말이야
                    isRoom = false;
                    break;
                }
            } while (queue.Count > 0);

            //Debug.Log($"방 체크여부 => {isRoom }, 몇개를 체크했는가 => {tileCount}");
            return isRoom;
        }

        //플로이드필을 사용해서 비슷한걸 해결할수 있을것이다 체크 여긴나중에//20221128
        //제네릭 화처리



        private void Awake()
        {
            //OK 모든타일맵은 같은 타일위치체크

            terrainTilePosList.AddRange(lLcroweRectTileMapUtil.GetAllTilePos(terrainTilemap));
            wallTilePosList.AddRange(lLcroweRectTileMapUtil.GetAllTilePos(wallTilemap));
        }

        private void Update()
        {
            Vector3Int cellPos = terrainTilemap.WorldToCell(InPutKeySystem.Instance.mouseWorldPosition);//월드 투 셀 위치로 변환    
            bool isRoom = CheckTileMapForRoom(terrainTilemap, wallTilemap, cellPos, checkMaxTileCount);
            Debug.Log($"방 체크여부 => {isRoom }");


            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (!lLcroweRectTileMapUtil.GetIsExistTile(InPutKeySystem.Instance.mouseWorldPosition, wallTilemap))
                {
                    Vector3Int temp = lLcroweRectTileMapUtil.GetWorldToCell(InPutKeySystem.Instance.mouseWorldPosition, terrainTilemap);
                    StartCoroutine(FloodFill_coroutine(terrainTilemap, wallTilemap, temp, Random.ColorHSV(), checkMaxTileCount, delay));

                    //FloodFill(temp, Random.ColorHSV());
                }
            }

            ////매프레임 체크
            ////다른작동방식을 체크
            //if (!lLcroweUtil.GetIsExistTile(MousePointer.Instance.mouseWorldPosition, wallTilemap))
            //{
            //    Vector3Int temp = lLcroweUtil.GetWorldToCell(MousePointer.Instance.mouseWorldPosition, terrainTilemap);
            //    //StartCoroutine(FloodFill(temp, Random.ColorHSV()));


            //    //무언가 표시는 추가적인 타일맵을 사용하여 알파값을 사용하여 온오프시키자
            //    FloodFill(temp, Color.black);
            //}
        }
    }

}

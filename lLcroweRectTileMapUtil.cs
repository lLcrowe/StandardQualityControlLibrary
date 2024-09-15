using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace lLCroweTool.TileMap
{
    public static class lLcroweRectTileMapUtil
    {
        //타일맵관련

        /// <summary>
        /// 타일맵에 배치되있는 포지션을 모두가져오기. 매프레임으로 돌리지 말기
        /// </summary>
        /// <returns>타일이 있는 위치들</returns>
        public static Vector3Int[] GetAllTilePos(Tilemap tilemap)
        {
            //모든타일가져오기
            BoundsInt bounds = tilemap.cellBounds;
            List<Vector3Int> posList = new List<Vector3Int>();

            //int xMax = floorTileMap.GetTileMap().cellBounds.xMax;
            //int xMin = floorTileMap.GetTileMap().cellBounds.xMin;
            //int yMax = floorTileMap.GetTileMap().cellBounds.yMax;
            //int yMin = floorTileMap.GetTileMap().cellBounds.yMin;
            //Debug.Log(xMax + ", " + yMin + ", " + xMax + ", " + yMin + "");

            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                for (int x = bounds.xMin; x < bounds.xMax; x++)
                {
                    Vector3Int target = new Vector3Int(x, y, 0);

                    TileBase tile = tilemap.GetTile(target);
                    if (tile != null)
                    {
                        //Debug.Log("x:" + x + " y:" + y + " tile:" + tile.name);
                        posList.Add(new Vector3Int(x, y, 0));
                        //count++;
                    }
                    else
                    {
                        // Debug.Log("x:" + x + " y:" + y + " tile: (null)");
                    }
                }
            }
            return posList.ToArray();
        }

        //-===========박스(네모)타일용

        /// <summary>
        /// 해당타일위치의 근처타일위치를 가져오는 함수
        /// </summary>
        /// <param name="tilePos">타일위치</param>
        /// <returns>위, 아래, 좌, 우  위치반환</returns>
        public static Vector3Int[] GetSideTilePos(Vector3Int tilePos)
        {
            Vector3Int upTilePos = new Vector3Int(tilePos.x, tilePos.y + 1, tilePos.z);
            Vector3Int downTilePos = new Vector3Int(tilePos.x, tilePos.y - 1, tilePos.z);
            Vector3Int leftTilePos = new Vector3Int(tilePos.x - 1, tilePos.y, tilePos.z);
            Vector3Int rightTilePos = new Vector3Int(tilePos.x + 1, tilePos.y, tilePos.z);

            Vector3Int[] sidePosArray = { upTilePos, downTilePos, leftTilePos, rightTilePos };

            return sidePosArray;
        }

        /// <summary>
        /// 해당타일위치의 근처타일을 가져오는 함수
        /// </summary>
        /// <param name="tilePos">타일위치</param>
        /// <returns>위, 아래, 좌, 우  타일반환</returns>
        public static Tile[] GetSideTile(Vector3Int tilePos, Tilemap tilemap)
        {
            Vector3Int[] sidePosArray = GetSideTilePos(tilePos);
            //upTilePos = new Vector3Int(tilePos.x, tilePos.y + 1, tilePos.z);
            //downTilePos = new Vector3Int(tilePos.x, tilePos.y - 1, tilePos.z);
            //leftTilePos = new Vector3Int(tilePos.x - 1, tilePos.y, tilePos.z);
            //rightTilePos = new Vector3Int(tilePos.x + 1, tilePos.y, tilePos.z);
            Tile upTile = tilemap.GetTile<Tile>(sidePosArray[0]);
            Tile downTile = tilemap.GetTile<Tile>(sidePosArray[1]);
            Tile leftTile = tilemap.GetTile<Tile>(sidePosArray[2]);
            Tile rightTile = tilemap.GetTile<Tile>(sidePosArray[3]);

            Tile[] sideTileArray = { upTile, downTile, leftTile, rightTile };

            return sideTileArray;
        }

        /// <summary>
        /// 해당타일위치의 근처타일이 존재하는 여부를 가져오는 함수
        /// </summary>
        /// <param name="tilePos">체크할 위치</param>
        /// <returns>위, 아래, 좌, 우  타일존재여부반환</returns>
        public static bool[] GetSideTileIsHas(Vector3Int tilePos, Tilemap tilemap)
        {
            Vector3Int[] sidePosArray = GetSideTilePos(tilePos);

            bool upTile = tilemap.HasTile(sidePosArray[0]);
            bool downTile = tilemap.HasTile(sidePosArray[1]);
            bool leftTile = tilemap.HasTile(sidePosArray[2]);
            bool rightTile = tilemap.HasTile(sidePosArray[3]);

            bool[] sideIsHasArray = { upTile, downTile, leftTile, rightTile };

            return sideIsHasArray;
        }

        /// <summary>
        /// 해당타일위치의 근처타일이 존재하는 여부를 가져오는 함수
        /// </summary>
        /// <param name="sidePosArray">위, 아래, 좌, 우 타일위치</param>
        /// <returns>위, 아래, 좌, 우  타일존재여부반환</returns>
        public static bool[] GetSideTileIsHas(Vector3Int[] sidePosArray, Tilemap tilemap)
        {
            bool upTile = tilemap.HasTile(sidePosArray[0]);
            bool downTile = tilemap.HasTile(sidePosArray[1]);
            bool leftTile = tilemap.HasTile(sidePosArray[2]);
            bool rightTile = tilemap.HasTile(sidePosArray[3]);

            bool[] sideIsHasArray = { upTile, downTile, leftTile, rightTile };

            return sideIsHasArray;
        }


        /// <summary>
        /// 좌표에 있는 타일이 사이드타일인지 확인하는 함수
        /// </summary>
        /// <param name="tilePos">타일위치</param>
        /// <returns>사이드타일여부</returns>
        public static bool GetIsSideTile(Vector3Int tilePos, Tilemap tilemap)
        {
            bool[] checkArray = GetSideTileIsHas(tilePos, tilemap);

            for (int i = 0; i < checkArray.Length; i++)
            {
                if (!checkArray[i])
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 월드위치의 타일맵셀위치로 반환
        /// </summary>
        /// <param name="pos">월드위치</param>
        /// <param name="tilemap">타일맵</param>
        /// <returns>타일맵 셀위치</returns>
        public static Vector3Int GetWorldToCell(Vector2 pos, Tilemap tilemap)
        {
            //Vector3Int cellLocalPos = custom3DHexTileMap.LocalToCell(pos);//로컬 투 셀 위치로 변환//사용안함                    
            //Debug.Log("셀 위치:" + cellPos + ",로컬셀 위치:" + cellLocalPos + ",마우스위치:" + pos);        
            return tilemap.WorldToCell(pos);//월드 투 셀 위치로 변환                
        }

        //셀 위치:(-1, 3, 0),로컬셀 위치:(-1, 3, 0),마우스위치:(-0.4, 3.2, 0.0)
        //셀 위치:(-1, 3, 0),로컬셀 위치:(-1, 3, 0),마우스위치:(-0.6, 3.4, 0.0)
        //셀 위치:(2, 4, 0),로컬셀 위치:(-2, 1, 0),마우스위치:(-1.2, 1.6, 0.0)
        //셀 위치:(-1, 3, 0),로컬셀 위치:(-5, 0, 0),마우스위치:(-4.2, 0.9, 0.0)
        //월드포지션으로 하느게 맞아보임
        //로컬테스트 =>작동안됨      

        /// <summary>
        /// 특정타일을 위치에 세팅하는 함수
        /// </summary>
        /// <param name="pos">위치</param>
        /// <param name="targetTile">타일</param>
        public static void SetTile(Vector3Int pos, TileBase targetTile, Tilemap tilemap)
        {
            tilemap.SetTileFlags(pos, TileFlags.None);
            tilemap.SetTile(pos, targetTile);
        }

        /// <summary>
        /// 타일맵상에 존재여부를 체크후 특정타일을 위치에 세팅하는 함수
        /// </summary>
        /// <param name="pos">위치</param>
        /// <param name="targetTile">타일</param>
        /// <param name="targetTileMap">확인할 타일맵</param>
        /// <param name="isExist">확인할 존재여부</param>
        public static void SetTile(Vector3Int pos, TileBase targetTile, Tilemap originTilemap, Tilemap targetTileMap, bool isExist)
        {
            if (targetTileMap.HasTile(pos) == isExist)//존재여부 체크
            {
                originTilemap.SetTile(pos, targetTile);
            }
        }

        /// <summary>
        /// 해당위치 색깔을 세팅해주는 함수
        /// </summary>
        /// <param name="pos">위치</param>
        /// <param name="color">색깔</param>
        public static void SetTile(Vector3Int pos, Color color, Tilemap tilemap)
        {
            tilemap.SetTileFlags(pos, TileFlags.None);
            tilemap.SetColor(pos, color);
        }

        /// <summary>
        /// 해당위치 색깔을 세팅해주는 함수
        /// </summary>
        /// <param name="pos">위치</param>
        /// <param name="alpha">알파값</param>
        public static void SetTile(Vector3Int pos, float alpha, Tilemap tilemap)
        {
            Color color = tilemap.GetColor(pos);
            color.a = alpha;
            //Debug.Log(custom3DHexTileMap.GetTile(cellPos));
            //Debug.Log(custom3DHexTileMap.GetTileFlags(cellPos));
            //타일플래그 설정
            //custom3DHexTileMap.SetTileFlags(cellPos, TileFlags.LockTransform);
            tilemap.SetColor(pos, color);
        }

        /// <summary>
        /// 해당위치 색깔을 세팅해주는 함수
        /// </summary>
        /// <param name="pos">위치</param>
        /// <param name="red">레드값</param>
        /// <param name="green">그린값</param>
        /// <param name="blue">블루값</param>
        /// <param name="alpha">알파값</param>
        public static void SetTile(Vector3Int pos, float red, float green, float blue, float alpha, Tilemap tilemap)
        {
            Color color = tilemap.GetColor(pos);
            color.r = red;
            color.g = green;
            color.b = blue;
            color.a = alpha;

            //Debug.Log(custom3DHexTileMap.GetTile(cellPos));
            //Debug.Log(custom3DHexTileMap.GetTileFlags(cellPos));
            //타일플래그 설정
            //custom3DHexTileMap.SetTileFlags(cellPos, TileFlags.LockTransform);
            tilemap.SetColor(pos, color);
        }

        /// <summary>
        /// 해당위치의 타일작동조건설정을 세팅하는 함수
        /// </summary>
        /// <param name="pos">위치</param>
        /// <param name="tileFlags">타일플래그</param>
        public static void SetTile(Vector3Int pos, TileFlags tileFlags, Tilemap tilemap)
        {
            tilemap.SetTileFlags(pos, tileFlags);
        }

        /// <summary>
        /// 해당위치의 타일을 가져오는함수
        /// </summary>
        /// <param name="pos">위치</param>
        /// <returns>위치의 타일</returns>
        public static TileBase GetTile(Vector3Int pos, Tilemap tilemap)
        {
            return tilemap.GetTile(pos);
        }

        /// <summary>
        /// 박스형태로 채워주기 함수
        /// </summary>
        /// <param name="targetStartPos">시작위치</param>
        /// <param name="targetEndPos">마지막위치</param>
        /// <param name="tile">배치할 타일</param>
        public static void BoxFill(Vector2 targetStartPos, Vector2 targetEndPos, TileBase tile, Tilemap tilemap)
        {
            Vector3Int startPos = tilemap.WorldToCell(targetStartPos);
            Vector3Int endPos = tilemap.WorldToCell(targetEndPos);

            //방향처리
            var xDir = startPos.x < endPos.x ? 1 : -1;
            var yDir = startPos.y < endPos.y ? 1 : -1;

            //놓을 타일수
            int xcolumn = 1 + Mathf.Abs(startPos.x - endPos.x);
            int ycolumn = 1 + Mathf.Abs(startPos.y - endPos.y);

            //그리기시작
            for (var x = 0; x < xcolumn; x++)
            {
                for (var y = 0; y < ycolumn; y++)
                {
                    var tilePos = startPos + new Vector3Int(x * xDir, y * yDir, 0);
                    tilemap.SetTile(tilePos, tile);
                }
            }
        }

        /// <summary>
        /// 박스형태로 채워주기 함수
        /// </summary>
        /// <param name="targetStartPos">시작위치</param>
        /// <param name="targetEndPos">마지막위치</param>
        /// <param name="tile">배치할 타일</param>
        public static void BoxFill(Vector3Int targetStartPos, Vector3Int targetEndPos, TileBase tile, Tilemap tilemap)
        {
            //Determine directions on X and Y axis
            //방향처리
            var xDir = targetStartPos.x < targetEndPos.x ? 1 : -1;
            var yDir = targetStartPos.y < targetEndPos.y ? 1 : -1;
            //How many tiles on each axis?
            //놓을 타일수
            int xcolumn = 1 + Mathf.Abs(targetStartPos.x - targetEndPos.x);
            int ycolumn = 1 + Mathf.Abs(targetStartPos.y - targetEndPos.y);
            //Start painting
            //놓기시작
            for (var x = 0; x < xcolumn; x++)
            {
                for (var y = 0; y < ycolumn; y++)
                {
                    var tilePos = targetStartPos + new Vector3Int(x * xDir, y * yDir, 0);
                    tilemap.SetTile(tilePos, tile);
                }
            }
        }

        /// <summary>
        /// 그리드에 맞게 좌표를 스냅하는 함수
        /// </summary>
        /// <param name="position">원본 좌표</param>
        /// <returns>그리드에 맞게 스냅된 좌표</returns>
        public static Vector2 SnapPosToGridPos(Vector2 position, Tilemap tilemap)
        {
            Vector3Int cellPos = tilemap.WorldToCell(position);
            position = tilemap.GetCellCenterWorld(cellPos);
            return position;
        }

        /// <summary>
        /// 특정영역파괴
        /// </summary>
        /// <param name="radius">반경</param>
        /// <param name="position">위치</param>
        public static void DestroyArea(float radius, Vector2 position, Tilemap tilemap)
        {
            int radiusInt = Mathf.RoundToInt(radius) + 1;//1개의 크기를 더확인해줌//위치확인용

            for (int i = -radiusInt; i <= radiusInt; i++)
            {
                for (int j = -radiusInt; j <= radiusInt; j++)
                {
                    //새위치를 지정
                    Vector2 newPos = new Vector2(position.x + i, position.y + j);

                    //해당위치에서부터 거리체크
                    if (Vector2.Distance(newPos, position) <= radius)
                    //if (Vector3.Distance(targetDestroyPos, position) - 0.001f <= radius) 
                    {
                        //파괴로직
                        Vector3Int targetDestroyPos = tilemap.WorldToCell(newPos);
                        tilemap.SetTile(targetDestroyPos, null);
                        //추가처리
                    }
                }
            }
        }

        //https://playground10.tistory.com/62
        //DDA 알고리즘
        public static void DDALine(Vector2 targetStartPos, Vector2 targetEndPos, Tile tile, Tilemap tilemap, bool fillGaps)
        {

            Vector3Int startPos = tilemap.WorldToCell(targetStartPos);
            Vector3Int endPos = tilemap.WorldToCell(targetEndPos);


            foreach (Vector3Int point in GetPointsOnLine(startPos, endPos, fillGaps))
            {
                Vector3Int paintPos = new Vector3Int(point.x, point.y, point.z);
                tilemap.SetTile(paintPos, tile);
            }



            //GetPointsOnLine(startPos, endPos, lineBrush.fillGaps)
            ////초기값
            //Vector3Int startPos = custom3DHexTileMap.WorldToCell(targetStartPos);
            //Vector3Int endPos = custom3DHexTileMap.WorldToCell(targetEndPos);

            ////방향처리
            //var xDir = startPos.x < endPos.x ? 1 : -1;
            //var yDir = startPos.y < endPos.y ? 1 : -1;

            ////놓을 타일수
            //int xcolumn = 1 + Mathf.Abs(startPos.x - endPos.x);
            //int ycolumn = 1 + Mathf.Abs(startPos.y - endPos.y);


            //int x = xcolumn;
            //int y = ycolumn;
            //int w = endPos.x - startPos.x;
            //int h = endPos.y - startPos.y;
            //int f = 2 * h - w;

            ////각 판별식 공식
            //int dF1 = 2 * h;
            //int dF2 = 2 * (h - w);

            //for (x = startPos.x; x <= endPos.x; x++)
            //{
            //    //점 그리기
            //    custom3DHexTileMap.SetTile(new Vector3Int(x, y), tile);

            //    if (f < 0)
            //    {
            //        //0보다 작으면 그에 맞는 공식으로 판별식 갱신, y값은 그대로 
            //        f += dF1;
            //    }
            //    else
            //    {
            //        //0보다 크거나 같으면
            //        //그에 맞는 공식으로 반별식 갱신, y값은 증가
            //        ++y;
            //        f += dF2;
            //    }
            //}
        }

        /// <summary>
        /// Enumerates all the points between the start and end position which are
        /// linked diagonally or orthogonally.
        /// </summary>
        /// <param name="startPos">Start position of the line.</param>
        /// <param name="endPos">End position of the line.</param>
        /// <param name="fillGaps">Fills any gaps between the start and end position so that
        /// all points are linked only orthogonally.</param>
        /// <returns>Returns an IEnumerable which enumerates all the points between the start and end position which are
        /// linked diagonally or orthogonally.</returns>
        public static IEnumerable<Vector3Int> GetPointsOnLine(Vector3Int startPos, Vector3Int endPos, bool fillGaps)
        {
            var points = GetPointsOnLine(startPos, endPos);
            if (fillGaps)
            {
                var rise = endPos.y - startPos.y;
                var run = endPos.x - startPos.x;

                if (rise != 0 || run != 0)
                {
                    var extraStart = startPos;
                    var extraEnd = endPos;


                    if (Mathf.Abs(rise) >= Mathf.Abs(run))
                    {
                        // up
                        if (rise > 0)
                        {
                            extraStart.y += 1;
                            extraEnd.y += 1;
                        }
                        // down
                        else // rise < 0
                        {

                            extraStart.y -= 1;
                            extraEnd.y -= 1;
                        }
                    }
                    else // Mathf.Abs(rise) < Mathf.Abs(run)
                    {

                        // right
                        if (run > 0)
                        {
                            extraStart.x += 1;
                            extraEnd.x += 1;
                        }
                        // left
                        else // run < 0
                        {
                            extraStart.x -= 1;
                            extraEnd.x -= 1;
                        }
                    }

                    var extraPoints = GetPointsOnLine(extraStart, extraEnd);
                    extraPoints = extraPoints.Except(new[] { extraEnd });
                    points = points.Union(extraPoints);
                }

            }

            return points;
        }

        /// <summary>
        /// Gets an enumerable for all the cells directly between two points
        /// http://ericw.ca/notes/bresenhams-line-algorithm-in-csharp.html
        /// </summary>
        /// <param name="p1">A starting point of a line</param>
        /// <param name="p2">An ending point of a line</param>
        /// <returns>Gets an enumerable for all the cells directly between two points</returns>
        private static IEnumerable<Vector3Int> GetPointsOnLine(Vector3Int p1, Vector3Int p2)
        {
            int x0 = p1.x;
            int y0 = p1.y;
            int x1 = p2.x;
            int y1 = p2.y;

            bool steep = Mathf.Abs(y1 - y0) > Mathf.Abs(x1 - x0);
            if (steep)
            {
                int t;
                t = x0; // swap x0 and y0
                x0 = y0;
                y0 = t;
                t = x1; // swap x1 and y1
                x1 = y1;
                y1 = t;
            }
            if (x0 > x1)
            {
                int t;
                t = x0; // swap x0 and x1
                x0 = x1;
                x1 = t;
                t = y0; // swap y0 and y1
                y0 = y1;
                y1 = t;
            }
            int dx = x1 - x0;
            int dy = Mathf.Abs(y1 - y0);
            int error = dx / 2;
            int ystep = (y0 < y1) ? 1 : -1;
            int y = y0;
            for (int x = x0; x <= x1; x++)
            {
                yield return new Vector3Int((steep ? y : x), (steep ? x : y));
                error = error - dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }
            yield break;
        }

        /// <summary>
        /// 해당위치에 타일이 존재하는지 확인하는 함수
        /// </summary>
        /// <param name="pos">위치</param>
        /// <returns>존재여부</returns>
        public static bool GetIsExistTile(Vector3 pos, Tilemap tilemap)
        {
            Vector3Int cellPos = tilemap.WorldToCell(pos);//월드 투 셀 위치로 변환    
            return tilemap.HasTile(cellPos);
            //TileBase target = custom3DHexTileMap.GetTile(cellPos);
            //Debug.Log(target);        
        }

        /// <summary>
        /// 해당위치에 타일이 존재하는지 확인하는 함수
        /// </summary>
        /// <param name="pos">위치</param>
        /// <returns>존재여부</returns>
        public static bool GetIsExistTile(Vector3Int pos, Tilemap tilemap)
        {
            return tilemap.HasTile(pos);
            //TileBase target = custom3DHexTileMap.GetTile(cellPos);
            //Debug.Log(target);        
        }

        /// <summary>
        /// 타일맵 새로고침함수
        /// </summary>
        public static void RefreshTileMap(Tilemap tilemap)
        {
            tilemap.RefreshAllTiles();
        }

        /// <summary>
        /// 타일맵 새로고침함수
        /// </summary>
        /// <param name="pos">위치</param>
        public static void RefreshTileMap(Vector3Int pos, Tilemap tilemap)
        {
            tilemap.RefreshTile(pos);
        }

        /// <summary>
        /// 타일맵 리셋시키는 함수
        /// </summary>
        public static void ResetTilemap(Tilemap tilemap)
        {
            tilemap.ClearAllTiles();
        }

        /// <summary>
        /// 배열로 받은 맵을 회전시키는 함수
        /// </summary>
        /// <param name="angle">회전각</param>
        /// <param name="isClockwise">시계방향 여부</param>
        /// <param name="posArray">맵으로 되있는 배열</param>
        /// <returns>회전시킨 배열</returns>
        public static Vector3Int[] RotateVector3IntArray(float angle, bool isClockwise, Vector3Int[] posArray)
        {
            //(3by3)이렇게 되있는게 쓰기 편하다

            angle = isClockwise ? -angle : angle;
            Quaternion rotationQuaternion = Quaternion.AngleAxis(angle, Vector3.forward);
            Matrix4x4 rotationMatrix = Matrix4x4.Rotate(rotationQuaternion);

            for (int i = 0; i < posArray.Length; i++)
            {
                //현재 타일
                Vector3Int curTilePos = posArray[i];
                if (curTilePos == Vector3Int.zero)
                {
                    continue;
                }

                //회전
                Vector3 rotatedVector = rotationMatrix.MultiplyPoint3x4(curTilePos);

                //대입
                posArray[i] = Vector3Int.CeilToInt(rotatedVector);
            }
            return posArray;
        }

        /// <summary>
        /// 배열로 받은 맵을 회전시키는 함수.
        /// 원본이 변경되니 주의하기.
        /// </summary>
        /// <param name="angle">회전각</param>
        /// <param name="isClockwise">시계방향 여부</param>
        /// <param name="posArray">맵으로 되있는 배열</param>
        /// <returns>회전시킨 배열</returns>
        public static Vector2Int[] RotateVector2IntArray(float angle, bool isClockwise, Vector2Int[] posArray)
        {
            //(3by3)이렇게 되있는게 쓰기 편하다
            angle = isClockwise ? -angle : angle;
            Quaternion rotationQuaternion = Quaternion.AngleAxis(angle, Vector3.forward);
            Matrix4x4 rotationMatrix = Matrix4x4.Rotate(rotationQuaternion);

            for (int i = 0; i < posArray.Length; i++)
            {
                //현재 타일
                Vector3Int curTilePos = (Vector3Int)posArray[i];
                if (curTilePos == Vector3Int.zero)
                {
                    continue;
                }

                //회전
                Vector3 rotatedVector = rotationMatrix.MultiplyPoint3x4(curTilePos);

                //대입
                posArray[i] = new Vector2Int(rotatedVector.x.RoundToInt(), rotatedVector.y.RoundToInt());
            }
            return posArray;
        }


        /// <summary>
        /// 원본타일맵을 다른타일맵 회전하여 전달하는 함수
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="isClockwise"></param>
        /// <param name="otherTileMap"></param>
        public static void RotateTileMap(float angle, bool isClockwise, Vector3Int[] posArray, Tilemap originTileMap, Tilemap otherTileMap)
        {
            angle = isClockwise ? -angle : angle;
            Quaternion rotationQuaternion = Quaternion.AngleAxis(angle, Vector3.forward);
            Matrix4x4 rotationMatrix = Matrix4x4.Rotate(rotationQuaternion);

            for (int i = 0; i < posArray.Length; i++)
            {
                //현재 타일
                Vector3Int curTilePos = posArray[i];
                if (curTilePos == Vector3Int.zero)
                {
                    continue;
                }
              
                TileBase tile = GetTile(posArray[i], originTileMap);

                //회전
                Vector3 rotatedVector = rotationMatrix.MultiplyPoint3x4(curTilePos);

                //다른타일
                Vector3Int newPos = GetWorldToCell(rotatedVector + originTileMap.tileAnchor, otherTileMap);
                SetTile(newPos, tile, otherTileMap);
            }
        }


    }
}
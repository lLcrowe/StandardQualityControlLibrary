
#if UNITY_EDITOR && Doozy
using lLCroweTool.BuildingSystem;
using lLCroweTool.TerrainSystem.BasementTileMap;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace lLCroweTool.QC.EditorOnly
{

    [RequireComponent(typeof(SpriteRenderer))]
    public class VoidRoomSetterEditor : MonoBehaviour
    {
        public enum VoidRoomSetterMode
        {
            Build,
            Dismantle,
            OxygenAdd,
            OxygenRemove,
            OxygenAddBox,
            OxygenRemoveBox,
            OxygenAddLine,
            OxygenRemoveLine,
            OutSideOxygen_True,
            OutSideOxygen_False,
            ConstOxygen_True,
            ConstOxygen_False,
        }
        [Header("사용법 : 좌클릭과 12345로 세팅변경")]

        [Header("세팅설정")]
        public VoidRoomSetterMode voidRoomSetterMode;
        private Vector3 startPos;

        [Header("건설관련")]
        public BuildingObjectScript targetBuildingData;    
        private bool check = false;
        private bool isChangeColor = false;
        private SpriteRenderer sr;

        [Header("산소타일")]
        public Tile oxygenTile;

        [Header("산소관련")]
        public int oxygenValue = 50;    

        [Header("타겟팅돤 전초기지 타일맵")]
        public BasementTileMap targetBasementTileMap;

        /// <summary>
        /// Ensures that there are orthogonal connections of Tiles from the start of the line to the end.
        /// </summary>
        public bool fillGaps;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();        
            gameObject.name = "-=" + typeof(VoidRoomSetterEditor).Name + "=-";
            voidRoomSetterMode = VoidRoomSetterMode.Dismantle;
        }

        private void Update()
        {
            transform.position = InPutKeySystem.Instance.mouseWorldPosition;
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                startPos = transform.position;
            }
            else if (Input.GetKey(KeyCode.Mouse0))
            {
                Vector3Int cellPos;
                switch (voidRoomSetterMode)
                {
                    case VoidRoomSetterMode.Build:
                        //건설
                        check = BuildingPointer.Instance.CheckIsBuilding(targetBuildingData, transform.position, targetBasementTileMap);
                        if (!check)
                        {
                            //컬러변경
                            BluePrintObjectChangeColor(!check);
                            return;
                        }
                        BluePrintObjectChangeColor(true);

                        BuildingPointer.Instance.BuildBuilding(targetBuildingData, transform.position, targetBasementTileMap, true);
                        break;
                    case VoidRoomSetterMode.Dismantle:
                        //해체
                        check = BuildingPointer.Instance.CheckIsDismantle(transform.position, true, targetBasementTileMap.GetTileMap());
                        if (!check)
                        {
                            //컬러변경
                            BluePrintObjectChangeColor(!check);
                            return;
                        }
                        BluePrintObjectChangeColor(true);

                        BuildingPointer.Instance.DismantleFloor(transform.position, targetBasementTileMap, true);
                        break;
                    case VoidRoomSetterMode.OxygenAdd:
                        //산소추가
                        cellPos = lLcroweUtil.GetWorldToCell(transform.position, targetBasementTileMap.GetTileMap());

                        if (lLcroweUtil.GetIsExistTile(cellPos, targetBasementTileMap.GetTileMap()))
                        {
                            VoidRoomInfo voidRoomInfo = targetBasementTileMap.GetVoidRoomInfo(cellPos);
                            voidRoomInfo.AddOxygenValue(oxygenValue);
                        }
                        break;
                    case VoidRoomSetterMode.OxygenRemove:
                        //산소빼기
                        cellPos = lLcroweUtil.GetWorldToCell(transform.position, targetBasementTileMap.GetTileMap());

                        if (lLcroweUtil.GetIsExistTile(cellPos, targetBasementTileMap.GetTileMap()))
                        {
                            VoidRoomInfo voidRoomInfo = targetBasementTileMap.GetVoidRoomInfo(cellPos);
                            voidRoomInfo.RemoveOxygenValue(oxygenValue);
                        }
                        break;               
                    case VoidRoomSetterMode.OutSideOxygen_True:
                    case VoidRoomSetterMode.OutSideOxygen_False:
                        //공기를 없애는 구간을 설정
                        cellPos = lLcroweUtil.GetWorldToCell(transform.position, targetBasementTileMap.GetTileMap());

                        if (lLcroweUtil.GetIsExistTile(cellPos, targetBasementTileMap.GetTileMap()))
                        {
                            VoidRoomInfo voidRoomInfo = targetBasementTileMap.GetVoidRoomInfo(cellPos);
                            if (voidRoomSetterMode == VoidRoomSetterMode.OutSideOxygen_True)
                            {
                                voidRoomInfo.isOutSideOxygen = true;
                                lLcroweUtil.SetTile(cellPos, Color.red, targetBasementTileMap.GetTileMap());
                            }
                            else
                            {
                                voidRoomInfo.isOutSideOxygen = false;

                                float colorAlphaValue = voidRoomInfo.GetCurOxygenValue() / (float)voidRoomInfo.GetMaxOxygenValue();
                                lLcroweUtil.SetTile(cellPos, 1, colorAlphaValue, colorAlphaValue, 1, targetBasementTileMap.GetTileMap());//컬러
                            }
                        }
                        break;
                    case VoidRoomSetterMode.ConstOxygen_True:
                    case VoidRoomSetterMode.ConstOxygen_False:
                        //변화하는 산소인지 설정
                        cellPos = lLcroweUtil.GetWorldToCell(transform.position, targetBasementTileMap.GetTileMap());

                        if (lLcroweUtil.GetIsExistTile(cellPos, targetBasementTileMap.GetTileMap()))
                        {
                            VoidRoomInfo voidRoomInfo = targetBasementTileMap.GetVoidRoomInfo(cellPos);                        

                            if (voidRoomSetterMode == VoidRoomSetterMode.ConstOxygen_True)
                            {
                                voidRoomInfo.isConstOxygen = true;
                            }
                            else
                            {
                                voidRoomInfo.isConstOxygen = false;
                            }
                        }
                        break;
                }
            }
            else if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                switch (voidRoomSetterMode)
                {
                    case VoidRoomSetterMode.OxygenAddBox:
                        lLcroweUtil.BoxFill(startPos, transform.position, oxygenTile, targetBasementTileMap.GetTileMap());
                        break;
                    case VoidRoomSetterMode.OxygenRemoveBox:
                        lLcroweUtil.BoxFill(startPos, transform.position, null, targetBasementTileMap.GetTileMap());
                        break;
                    case VoidRoomSetterMode.OxygenAddLine:
                        lLcroweUtil.DDALine(startPos, transform.position, oxygenTile, targetBasementTileMap.GetTileMap(), fillGaps);
                        break;
                    case VoidRoomSetterMode.OxygenRemoveLine:
                        lLcroweUtil.DDALine(startPos, transform.position, null, targetBasementTileMap.GetTileMap(), fillGaps);
                        break;
                }
                targetBasementTileMap.RefleshBasementTileMap();
            }


            if (Input.GetKeyUp(KeyCode.Alpha1))
            {
                if (targetBuildingData == null)
                {
                    Debug.Log("건설할 건물데이터가 없슴");
                    return;
                }
                voidRoomSetterMode = VoidRoomSetterMode.Build;
                sr.sprite = targetBuildingData.bluePrintBuildingImage;

            }
            else if (Input.GetKeyUp(KeyCode.Alpha2))
            {
                voidRoomSetterMode = VoidRoomSetterMode.Dismantle;
                sr.sprite = null;
            }
            else if (Input.GetKeyUp(KeyCode.Alpha3))
            {
                if (voidRoomSetterMode != VoidRoomSetterMode.OxygenAdd)
                {
                    voidRoomSetterMode = VoidRoomSetterMode.OxygenAdd;
                }
                else
                {
                    voidRoomSetterMode = VoidRoomSetterMode.OxygenRemove;
                }
            
                sr.sprite = null;
            }
            else if (Input.GetKeyUp(KeyCode.Alpha4))
            {
                if (voidRoomSetterMode != VoidRoomSetterMode.OxygenAddBox)
                {
                    voidRoomSetterMode = VoidRoomSetterMode.OxygenAddBox;
                }
                else
                {
                    voidRoomSetterMode = VoidRoomSetterMode.OxygenRemoveBox;
                }

                sr.sprite = null;
            }
            else if (Input.GetKeyUp(KeyCode.Alpha5))
            {
                if (voidRoomSetterMode != VoidRoomSetterMode.OxygenAddLine)
                {
                    voidRoomSetterMode = VoidRoomSetterMode.OxygenAddLine;
                }
                else
                {
                    voidRoomSetterMode = VoidRoomSetterMode.OxygenRemoveLine;
                }

                sr.sprite = null;
            }
            else if (Input.GetKeyUp(KeyCode.Alpha6))
            {
                if (voidRoomSetterMode != VoidRoomSetterMode.OutSideOxygen_True)
                {
                    voidRoomSetterMode = VoidRoomSetterMode.OutSideOxygen_True;
                }
                else
                {
                    voidRoomSetterMode = VoidRoomSetterMode.OutSideOxygen_False;
                }

                sr.sprite = null;
            }
            else if (Input.GetKeyUp(KeyCode.Alpha7))
            {
                if (voidRoomSetterMode != VoidRoomSetterMode.ConstOxygen_True)
                {
                    voidRoomSetterMode = VoidRoomSetterMode.ConstOxygen_True;
                }
                else
                {
                    voidRoomSetterMode = VoidRoomSetterMode.ConstOxygen_False;
                }
                sr.sprite = null;
            }
        }
        private void OnGUI()
        {
            GUI.Box(new Rect(10, 10, 150, 20), voidRoomSetterMode.ToString());        
        }

        private void BluePrintObjectChangeColor(bool isPossible)
        {
            if (isChangeColor == isPossible)
            {
                return;
            }

            isChangeColor = isPossible;
            Color color = isChangeColor ? Color.white : Color.red;
            sr.color = color;
        }
    }
}

#endif
#if Shape

using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Shapes;
using lLCroweTool.BuildingSystem.FuncStructure;
using lLCroweTool.ToolTipSystem;

namespace lLCroweTool.NodeMapSystem
{
    public class VoyageMap : MonoBehaviour
    {
        private static VoyageMap instance;
        public static VoyageMap Instance
        {
            get
            {
                if (ReferenceEquals(instance, null))
                {
                    instance = FindObjectOfType<VoyageMap>();
                    //if (ReferenceEquals(instance, null))
                    if (ReferenceEquals(instance, null))
                    {
                        GameObject gameObject = new GameObject();
                        instance = gameObject.AddComponent<VoyageMap>();
                        gameObject.name = "-=SpaceVoyageMap=-";
                    }
                }
                return instance;
            }
        }
        //항해맵 : 마커들은 여기에
        //오퍼레이터와 연동됨
        //여러 시설물과 UI를 계승하며
        //플레이어가 체크할수 있는 여러 요소를 현 클레스에 집어넣는다.
        //현스크립트구조
        //하단의 맵마커와 상호작용

        //참고할 다른 스크립트
        //함선... 그냥 함선에서 우주항해맵을 참조하는게 좋아보임
        private SpaceShipOperation spaceShip;

        //현재이동할 마커를 체크
        [Header("게임월드 상에서 체크하는것")]
        public MapMarker targetMarker;//타겟이 될 마커//기본적으로 비어잇음//맵을 틀고 세팅해줄수 있음
        public MapMarker currentPlaceMarker;//현재 위치의 마커//기본적으로 비어있음//초기 세팅할때 다른것에서 세팅
        public GameObject spaceShipMarker;//함선의 위치 마커//거리를체크해서 연료량을 구한다                                          
        public SpriteRenderer markerSelecter;//현재 위치를 알려주는 스프라이트 오브젝트(월드오브젝트상의 맵마커에 띄움)//갈 타겟의 위치를 알려주는 오브젝트

        //UI 관련
        [Header("UI 상에서 체크하는것")]
        public Camera SpaceVoyageCamera;
        //public Canvas spaceVoyageMapCanvas;//해당캔버스에 항해맵툴팁을 뛰운다       
        public TextMeshProUGUI targetMarkerUIText;//갈곳의 전초기지 UI//현재 전초기지위치 UI 표시하는 텍스트
        
        




        //통합툴팁 집어넣는곳



        //[Header("맵에서 체크하는것")]
        //public OutPostMap outPostMap;//전초기지맵과 연결해주는코드
        [Header("현 오브젝트 하위에 있는 모든마커를 가져와 등록")]
        public List<MapMarker> mapMarkers = new List<MapMarker>();//마커들 체크용
        //찾은 마커들 체크//메모리관리상 그냥 해당마커에 bool값으로 체크
        public List<MapMarker> searchMapMarkers = new List<MapMarker>();
        [Header("현재마커와 연결되있는 마커를 표시하는 라인오브젝트")]
        public Line curConnectMarkerLinePrefab;
        private List<Line> curConnectLineList = new List<Line>();
        [Header("현재 마커와 갈수있는 마커를 표시하는 라인오브젝트")]
        public Line backGroundConnectMarkerLinePrefab;
        private List<Line> backGroundConnectLineList = new List<Line>();

        [Header("현재 마커와 갈수있는 마커를 표시하는 라인오브젝트, 경고텍스트용")]
        public ToolTipUiView spaceVoyageMapToolTip;//툴팁//주의 텍스트

        public enum LineType
        {
            BackGround,
            Front,
        }
        

        //[Header("카메라 외부에서 사용하여 활성화 비활성화")]
        //public Camera SpaceVoyageMapCamera;//dhl

        private void Awake()
        {
            instance = this;
            StartSettingSpaceVoyageMap();
        }

        public void StartSettingSpaceVoyageMap()
        {
            //가져오는곳
          
            //outPostMap = FindObjectOfType<OutPostMap>();

            //마커들을 항해맵에 집어넣기
            MapMarker[] markers = transform.GetComponentsInChildren<MapMarker>();
            for (int i = 0; i < markers.Length; i++)
            {
                //세팅과 관리를 위한 리스트집어넣기 구역
                markers[i].SetSpaceVoyageMap(this);
                mapMarkers.Add(markers[i]);
            }

            //전체 항해맵 그리기
            DrawMarkerWay();

            //백그라운드 마커연결관계 그리기
            DrawBackGroundMarkerWay();

            //항해맵에서 현재 마커말고는 다 비활성화하기
            OnEnableCurrentMarker();

            //현재 마커말고는 나머지마커들 
            //알파값조절
            ChangeMapMarkerAlphaValue();

            //현재 마커위치에 셀렉터옮기기
            MoveMarkerSelecter(currentPlaceMarker.transform);



            //UI초기화 
            targetMarkerUIText.text = "";
            Canvas.ForceUpdateCanvases();
            spaceVoyageMapToolTip.OffText();
            spaceVoyageMapToolTip.ClearText();
            spaceVoyageMapToolTip.MoveToolTip(transform);
        }
      

        //[ButtonMethod]
        //재갱신할떄 쓰는 업데이트 함수
        //씬넘어갈떄 쓰는 함수
        public void UpdateSpaceVoyageMap()
        {
            //항해맵에서 현재 마커말고는 다 비활성화하기
            OnEnableCurrentMarker();

            //현재 마커말고는 나머지마커들 
            //알파값조절
            ChangeMapMarkerAlphaValue();

            //현재 마커위치에 셀렉터옮기기
            MoveMarkerSelecter(currentPlaceMarker.transform);
        }

        //클릭해서 위치를 선정할떄 비쥬얼로 보여주는 함수
        public void UpdateSpaceVoyageMapUI()
        {
            //초기화
            targetMarkerUIText.text = "";



            targetMarkerUIText.text = "설정한 마커 목적지 : \n" + targetMarker.designatedMapMarkerData.objectName;
            targetMarkerUIText.text += "현재있는 마커 : " + currentPlaceMarker.designatedMapMarkerData.objectName;
        }

        //버튼에 집어넣어야함
        //출항하는 함수
        //Check starting procedure
        public bool CheckSequenceDepartureShip()
        {
            bool isGreenLamp = false;
            //목적지체크
            if (targetMarker.Equals(null))
            {
                spaceVoyageMapToolTip.ShowText("<align=\"center\">알림", "다음목적지가 안정해져있습니다.");
                return isGreenLamp;
            }
          
            //우주맵 스트럭쳐 체크
            if (!spaceShip.CheckSpaceVoyageStructure())
            {
                spaceVoyageMapToolTip.ShowText("<align=\"center\">알림", "시설물이 고장나 있어서 작동이 안됩니다.");                
                return isGreenLamp;
            }

            //다체크했으면 
            //출항가능하다

            isGreenLamp = true;
            return isGreenLamp;
        }

        //마커들사이의 방향표 표시
        public void DrawMarkerWay()
        {
            //라인만들기
            Line lineObject;

            //마커수만큼 
            for (int i = 0; i < mapMarkers.Count; i++)
            {
                //자기자신한테 표식그리기
                //어떤 표식이라면 전초기지가 있다는 겉테두리 표식

                //마커안의 다른마커와 연결된 마커한테 방향표 그리기
                for (int j = 0; j < mapMarkers[i].connectMapMarkerList.Count; j++)
                {
                    //마커라인제작
                    lineObject = RequestLinePrefabObject(LineType.Front);
                    lineObject.transform.parent = mapMarkers[i].transform;

                    //UI에서 Shape에셋을 사용하여 작업했을시 코드
                    //lineObject.Start = Camera.main.ScreenToWorldPoint(mapMarkers[i].transform.position);
                    //lineObject.End = Camera.main.ScreenToWorldPoint(mapMarkers[i].connectMapMarkerList[j].transform.position);

                    //Shape에셋을 사용하여 작업했을시 코드
                    //lineObject.Start = mapMarkers[i].transform.position;
                    //lineObject.End = mapMarkers[i].connectMapMarkerList[j].transform.position;

                    lineObject.Start = transform.InverseTransformPoint(mapMarkers[i].transform.position);
                    lineObject.End = transform.InverseTransformPoint(mapMarkers[i].connectMapMarkerList[j].transform.position);

                    //유니티 컴포넌트로 지급하는 라인컴포넌트로 작업했을시 코드
                    //lineRenderer.SetPosition(0, mapMarkers[i].transform.position);
                    //lineRenderer.SetPosition(1, mapMarkers[i].connectMapMarkerList[j].transform.position);

                    mapMarkers[i].frontMarkerLineObjectList.Add(lineObject);
                }
            }
        }

        //백배경마커들사이의 연결관계 표시
        public void DrawBackGroundMarkerWay()
        {
            //라인만들기
            Line lineObject;

            //마커수만큼 
            for (int i = 0; i < mapMarkers.Count; i++)
            {
                //자기자신한테 표식그리기
                //어떤 표식이라면 전초기지가 있다는 겉테두리 표식

                //마커안의 다른마커와 연결된 마커한테 방향표 그리기
                for (int j = 0; j < mapMarkers[i].connectMapMarkerList.Count; j++)
                {
                    //마커라인제작
                    lineObject = RequestLinePrefabObject(LineType.BackGround);
                    lineObject.transform.parent = mapMarkers[i].transform;

                    lineObject.Start = transform.InverseTransformPoint(mapMarkers[i].transform.position);
                    lineObject.End = transform.InverseTransformPoint(mapMarkers[i].connectMapMarkerList[j].transform.position);

                    mapMarkers[i].BackMarkerLineObjectList.Add(lineObject);
                }
            }
        }

        //현재 마커말고는 비활성화 함수
        public void OnEnableCurrentMarker()
        {
            for (int i = 0; i < mapMarkers.Count; i++)
            {                
                mapMarkers[i].CheckCurrnetMarkerConnectOff();
            }
            currentPlaceMarker.CheckCurrnetMarkerConnectOn();
        }

        //모든마커활성화
        public void OnEnableAllMarker()
        {
            for (int i = 0; i < mapMarkers.Count; i++)
            {
                mapMarkers[i].CheckCurrnetMarkerConnectOn();
            }
        }

        //현재 마커말고는 나머지마커들 알파값조절
        public void ChangeMapMarkerAlphaValue()
        {
            for (int i = 0; i < mapMarkers.Count; i++)
            {
                if (searchMapMarkers.Contains(mapMarkers[i]))
                {
                    mapMarkers[i].sr.color = new Color(1, 1, 1, 1.0f);
                }
                else
                {
                    mapMarkers[i].sr.color = new Color(1, 1, 1, 0.5f);
                }
                if (mapMarkers[i].isSearchMapMarker)
                {
                    mapMarkers[i].sr.color = new Color(1, 1, 1, 1.0f);
                }
                else
                {
                    mapMarkers[i].sr.color = new Color(1, 1, 1, 0.5f);
                }
            }
        }

        //우주배 세팅
        public void SetSpaceShip(SpaceShipOperation _spaceShip)
        {
            spaceShip = _spaceShip;
        }
        //마커움직이기
        public void MoveMarkerSelecter(Transform _pos)
        {
            markerSelecter.transform.position = _pos.transform.position;
        }

        /// <summary>
        ///해당되는 마커를 찾은 후  해당 마커를 트루로 만들어줌. 인벤토리가 사용
        /// </summary>
        /// <param name="newMarkerData">찾을 마커데이터</param>
        public void SearchMapMarker(MapMarkerObjectScript newMarkerData)
        {
            for (int i = 0; i < mapMarkers.Count; i++)
            {
                if (mapMarkers[i].designatedMapMarkerData.objectName.Equals(newMarkerData.objectName))
                {
                    mapMarkers[i].isSearchMapMarker = true;
                }
            }
        }

        /// <summary>
        /// UIView에서 Show에서 사용함. 온오프하여 지도를 보여주게 하는함수 
        /// </summary>
        /// <param name="onOff">온오프 여부</param>
        public void ShowSpaceVoyageMap(bool onOff)
        {
            spaceVoyageMapToolTip.gameObject.SetActive(onOff);
        }


        private Line RequestLinePrefabObject(LineType _lineType)
        {
            //초기화
            bool isFind = false;
            Line targetObject = null;

            switch (_lineType)
            {
                case LineType.Front:
                    //로직작동
                    for (int i = 0; i < curConnectLineList.Count; i++)
                    {
                        if (!curConnectLineList[i].gameObject.activeSelf)
                        {
                            isFind = true;
                            targetObject = curConnectLineList[i];
                            //리셋구간
                            break;
                        }
                    }
                    break;
                case LineType.BackGround:
                    //로직작동
                    for (int i = 0; i < backGroundConnectLineList.Count; i++)
                    {
                        if (!backGroundConnectLineList[i].gameObject.activeSelf)
                        {
                            isFind = true;
                            targetObject = backGroundConnectLineList[i];
                            //리셋구간
                            break;
                        }
                    }
                    break;
            }

            //찾은게 없다면 오브젝트 하나를 만들어준다.
            if (!isFind)
            {
                targetObject = Instantiate(GetLinePrefab(_lineType), transform.position, Quaternion.identity, transform);
                switch (_lineType)
                {
                    case LineType.Front:
                        curConnectLineList.Add(targetObject);
                        break;
                    case LineType.BackGround:
                        backGroundConnectLineList.Add(targetObject);
                        break;
                }

                //리셋구간
                //targetObject.gameObject.SetActive(false);
                //해당오브젝트의 부모를 현재오브젝트에 지정한다.
                if (targetObject.TryGetComponent(out ISetParent setParent))
                {
                    setParent.SetParent(transform);
                }
            }
            return targetObject;
        }

        /// <summary>
        /// 원하는 타입에 따른 해당 타입라인의 프리팹을 반환해줌
        /// </summary>
        /// <param name="_lineType">라인 타입</param>
        /// <returns></returns>
        //(관리하기 위해 리스트추가랑은 별개로 제작함)
        private Line GetLinePrefab(LineType _lineType)
        {
            Line lineObject = null;

            switch (_lineType)
            {
                case LineType.Front:
                    lineObject = curConnectMarkerLinePrefab;
                    break;
                case LineType.BackGround:
                    lineObject = backGroundConnectMarkerLinePrefab;
                    break;
            }
            return lineObject;
        }
    }
}
#endif
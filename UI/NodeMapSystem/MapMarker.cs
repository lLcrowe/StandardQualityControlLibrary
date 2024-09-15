#if Shape
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

namespace lLCroweTool.NodeMapSystem
{
    public class MapMarker : MonoBehaviour
    {
        //우주항해맵에 쓰일 마커오브젝트이다
        //맵 프리팹을 소지하며 우주선에서 마커를 클릭하여 
        //해당맵으로 이동할수 있게
        //링커역할관계
        //mapmarker 관리자에서 업데이트시켜서 작동하게
        //상단의 항해맵과의 상호작용

        //관리자에서 초기 세팅할때 사용하는 함수들
        //

        //웨이포인트를 체크해서 비쥬얼적으로 어덯게 보여줘야하는지 찾아보자
        //연결된 마커리스트
        public List<MapMarker> connectMapMarkerList = new List<MapMarker>();
        //연결한 라인 랜더러
        public List<Line> frontMarkerLineObjectList = new List<Line>();//앞쪽라인오브젝트
        public List<Line> BackMarkerLineObjectList = new List<Line>();//뒤쪽라인오브젝트
        public GameObject MapPrefab;//해당되는맵
        public SpriteRenderer sr;//마커표시를 위한 스프라이트
        public BoxCollider2D box2d;//콜라이더용

        //상위함수에서 집어넣어야할구역        
        private VoyageMap voyageMap;//우주항해맵
        [Space]
        public bool isSearchMapMarker; //현재 맵마커를 찾았는가?


        [Space]
        //지정된 마커데이터
        public MapMarkerObjectScript designatedMapMarkerData;
       
        [Space]
        public string sceneName;
        



        //collider가 있어야지 마우스이벤트발동됨 
        //카메라에 있는 오브젝트여야함


        //해당맵 마커와 연결되있는지 체크해주는 함수
        public bool IsExistconnectMapMarker(MapMarker targetMapMarker)
        {
            bool isExist = false;
            for (int i = 0; i < voyageMap.currentPlaceMarker.connectMapMarkerList.Count; i++)
            {
                //해당되는 맵마커인지?
                if (voyageMap.currentPlaceMarker.connectMapMarkerList[i].Equals(targetMapMarker))
                {
                    //해당 마커가 찾은상태인지?
                    if (voyageMap.currentPlaceMarker.connectMapMarkerList[i].isSearchMapMarker)
                    {
                        isExist = true;
                        break;
                    }
                    //isExist = true;
                    //break;
                }
            }
            return isExist;
        }

        //마커가 가지고 있는 맵을 가져올때 쓰는 것
        public void MakeTheMap(Transform targetPos)
        {
            GameObject go = Instantiate(MapPrefab, null);
            go.transform.position = targetPos.position;
        }

        //항해맵의 다음 목적지를 정하는 함수
        public void SetNextMarker()
        {
            voyageMap.targetMarker = this;
        }

        //마커와 연결된 마커들을 체크하여 해당되는 라인들을 활성화시켜서
        //강조하는 함수
        public void CheckCurrnetMarkerConnectOn()
        {
            for (int i = 0; i < frontMarkerLineObjectList.Count; i++)
            {
                if (connectMapMarkerList[i].isSearchMapMarker)
                {
                    frontMarkerLineObjectList[i].gameObject.SetActive(true);
                    BackMarkerLineObjectList[i].gameObject.SetActive(true);
                    connectMapMarkerList[i].box2d.enabled = true;
                }
            }
            box2d.enabled = true;
        }

        public void CheckCurrnetMarkerConnectOff()
        {
            for (int i = 0; i < frontMarkerLineObjectList.Count; i++)
            {
                frontMarkerLineObjectList[i].gameObject.SetActive(false);
                //BackMarkerLineObjectList[i].gameObject.SetActive(false);
                connectMapMarkerList[i].box2d.enabled = false;
            }
            box2d.enabled = false;
        }

        //[ButtonMethod]
        //맵마커 데이터 집어넣기
        public void InitMapMaker()
        {
            if (ReferenceEquals(sr,null))
            {
                sr = GetComponent<SpriteRenderer>();
            }
            if (ReferenceEquals(designatedMapMarkerData, null))
            {
                Debug.Log("집어넣으신 데이터가 없습니다.");
                return;
            }

            //데이터 집어넣는곳
            //mapMarkerIcon = mapMarkerData.unitSprite;            
            //mapMarkerName = mapMarkerData.unitName;
            //mapMarkerShortDescription = mapMarkerData.unitShortDescription;
            //mapMarkerDescription = mapMarkerData.unitDescription;

            //보여주는 데이터들 집어넣기
            sr.sprite = designatedMapMarkerData.objectSprite;
            gameObject.name = designatedMapMarkerData.objectName;
        }

        //상위 클래스에서 쓰는 함수
        //우주 항해 맵 세팅
        public void SetSpaceVoyageMap(VoyageMap spaceVoyageMap)
        {
            voyageMap = spaceVoyageMap;
        }

        //마우스버튼을 클릭했을시
        private void OnMouseDown()
        {
            //Debug.Log(gameObject.name + "OnMouseDown이벤트");
            //사운드발생

            //자기자신을 클릭하면 아무행동안함
            if (voyageMap.currentPlaceMarker.Equals(this))
            {
                return;
            }

            //존재하는 마커맵인가
            if (IsExistconnectMapMarker(this))
            {   
                SetNextMarker();
                voyageMap.UpdateSpaceVoyageMapUI();
                voyageMap.spaceVoyageMapToolTip.MoveToolTip(transform);
                voyageMap.spaceVoyageMapToolTip.ClearText();
                voyageMap.spaceVoyageMapToolTip.ShowText("<align=\"center\">알림", "갈수있습니다.", null, "항해맵이정표에 등록되었습니다.", true);
            }
            else
            {
                voyageMap.spaceVoyageMapToolTip.MoveToolTip(transform);
                voyageMap.spaceVoyageMapToolTip.ClearText();
                voyageMap.spaceVoyageMapToolTip.ShowText("<align=\"center\">알림", "갈수없습니다.");
            }
        }

        //마우스버튼을 클릭한상태로 움직였을시
        //private void OnMouseDrag()
        //{
        //    Debug.Log(gameObject.name + "OnMouseDrag이벤트");
        //}

        //마우스가 콜라이더안쪽으로 들어왔을시
        private void OnMouseEnter()
        {
            //Debug.Log(gameObject.name + "OnMouseEnter이벤트");
            //애니메이션작동
            voyageMap.spaceVoyageMapToolTip.MoveToolTip(transform);
           

            if (isSearchMapMarker)
            {
                voyageMap.spaceVoyageMapToolTip.ShowText("명칭 : " + designatedMapMarkerData.objectName, "내용 : " + designatedMapMarkerData.objectShortDescription);
            }
            else
            {
                voyageMap.spaceVoyageMapToolTip.ShowText("명칭 : 알 수 없음.", "내용 : 알 수 없음.");
            }
            //스타2내의 캠페인미션
            //임무
            //임무목표
            //자금보상
            //연구기회
            //행성이름
            //행성소개
            //좀더생각해보자
            Canvas.ForceUpdateCanvases();
        }


        //마우스가 콜라이더안쪽으로 나갔을시
        private void OnMouseExit()
        {
            //Debug.Log(gameObject.name + "OnMouseExit 이벤트");
            //애니메이션작동
            voyageMap.spaceVoyageMapToolTip.OffText();
            voyageMap.spaceVoyageMapToolTip.ClearText();
        }


        //마우스가 콜라이더안쪽으로 들어와서 그대로 있을시
        //private void OnMouseOver()
        //{
        //    Debug.Log(gameObject.name + "OnMouseOver 이벤트");
        //}

        //마우스버튼을 올렸을시
        //private void OnMouseUp()
        //{
        //    Debug.Log(gameObject.name + "OnMouseUp 이벤트");
        //}

    }

}
#endif

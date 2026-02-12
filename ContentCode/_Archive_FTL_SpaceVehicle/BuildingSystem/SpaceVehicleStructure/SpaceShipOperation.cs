#if Doozy
using lLCroweTool.BuildingSystem.FuncStructure.Terminal;
using lLCroweTool.NodeMapSystem;
using lLCroweTool.TimerSystem;
using lLCroweTool.UI.Scene;
using System.Collections.Generic;
using UnityEngine;
//using Doozy.Engine.UI;

namespace lLCroweTool.BuildingSystem.FuncStructure
{
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(UpdateTimerModule))]
    public class SpaceShipOperation : MonoBehaviour, IActionInterectObject
    {
        //스페이스쉽오퍼레이션은 현게임오브젝트 하위에 대한 기능건축물 객체를 관리해주는 역할을 가짐
        //현 오브젝트가 욕구를 가진 건축물들을 업데이트 시켜줌
        //현 컴포넌트는 건물들이 있는맵을 상위오브젝트에 배치해줌
        //건축물은 아니고 현황표에서 살펴볼수 있게함
        //GameObject        (SpaceShipOperation)
        //  structure       (Func)
        //  MapFloor

        [SerializeField] private List<StructureFuncBaseObject> structureFucBaseList = new List<StructureFuncBaseObject>();
        //함선시설 보고를 위한 UI 따로 제작할것
        //public List<UIImage> builingOperationIcon = new List<UIImage>();

        //[SerializeField] private List<PipeStructureObject> pipeStructureObjectList = new List<PipeStructureObject>();

        [Space]

        //우주항해맵
        public VoyageMap targetSpaceVoyage;//타겟팅된 항해맵

        //FTL장치를 가동시키기 위한 설정들
        public bool FTLStart;//FTL작동시켯을때

        //FTL에 필요한 장치들

        //우주항해에 참조하는 시설물들
        //직접 시켜야함
        //제네레이터
        public GeneraterStuctureObject mainGenerater;//타겟이 될 메인 제네레이터
        //FTL 엔진
        public FTLEngineStuctureObject fTLEngine;//각 함선에 한개만 있음
        public ThrusterStuctureObject[] thrusterStuctures;//출력기(노즐)//여러개중 한개라도 있어야됨
                                   //조종기//조종기가 현 스페이스쉽 오퍼레이션을 확인함



        //함선장치의 업데이트관련
        private UpdateTimerModule timerModule;
       

        private void Awake()
        {
            targetSpaceVoyage.SetSpaceShip(this);

            StructureFuncBaseObject[] fucs = transform.GetComponentsInChildren<StructureFuncBaseObject>();
            for (int i = 0; i < fucs.Length; i++)
            {
                structureFucBaseList.Add(fucs[i]);
            }
                        
            timerModule.SetTimer(1.0f);
            timerModule.AddUnityEvent(delegate { UpdateSpaceShipOperation(); });
        }

        //함선안의 업데이트를 작동할떄 씀
        //업데이트 모듈 이벤트에 현 함수를 집어넣음
        //안에는 업데이트하고자하는 빌딩액션등을 집어넣음
        public void UpdateSpaceShipOperation()
        {
            //generater.BuildingAction();//제네레이터 행동
            //fTLEngine.BuildingAction();//엔진 행동
            //oxygenGenerater.BuildingAction();//산소발생기 행동

            //기능을 가진 오브젝트들의 업데이트를 작동시킴
            //업데이트기능변경됨

            //for (int i = 0; i < structureFucBaseList.Count; i++)
            //{
            //    if (structureFucBaseList[i].GetIsUseUpdate())
            //    {
            //        structureFucBaseList[i].UpdateStuctureFuncAction();
            //    }
            //}

            //파이프업데이트//파이프오브젝트에서 업데이트됨
            //for (int i = 0; i < pipeStructureObjectList.Count; i++)
            //{
            //    pipeStructureObjectList[i].UpdatePipe();
            //}

            //FTL 준비가 되면 씬작업으로 넘어감
            if (FTLStart)
            {
                if (fTLEngine.GetReadyToFTLEngine())
                {
                    //씬넘어가는 코드
                    //타겟 맵마커의 씬이름을 가져오기
                    GameSceneLoadManager.Instance.CallScene(targetSpaceVoyage.targetMarker.sceneName);

                    //초기화
                    FTLStart = false;
                    fTLEngine.FTLEnineReset();
                }
            }
        }

        //이벤트에 사용할 용도
        //전초기지에서 정보를찾아서 입력시키면 
        //업로드 되는 함수
        public void SetNewMarkerData(MapMarker findMapMarkerData)
        {
            if (targetSpaceVoyage.mapMarkers.Contains(findMapMarkerData))
            {
                findMapMarkerData.isSearchMapMarker = true;
            }
            targetSpaceVoyage.UpdateSpaceVoyageMap();
        }

        //항해시작을 눌렸을시
        public void StartVoyage()
        {
            //목적지체크
            //우주선체크
            if (targetSpaceVoyage.CheckSequenceDepartureShip())
            {
                //출발가능하니 출발
                FTLStart = true;
            }
            else
            {
                //출발불가능하니 알람
            }
        }

       

        //연료체크
        //전원체크
        //등등체크
        public bool CheckSpaceVoyageStructure()
        {
            //하나라도 체크했을때 안되면 X
            bool isGreenLamp = true;
            //구조물 체크
            //체크할 구조물
            //조종간
            //if (driverController.IsActiveBuilding())
            //{
            //    isGreenLamp = false;
            //}
            //연료탱크//체크안함
            //출력기(노즐)FTL 엔진
            if (!fTLEngine.GetIsActive())
            {
                isGreenLamp = false;
            }
            //제네레이터
            if (!mainGenerater.GetIsActive())
            {
                isGreenLamp = false;
            }
         

            return isGreenLamp;
        }

        //함선시설 보고 함수
        public void OperationSpaceShipStructure()
        {
            
        }

        //항해맵 세팅하는것
        public void SetTargetSpaceVoyage(VoyageMap _target)
        {
            targetSpaceVoyage = _target;
        }

        private void OnOffSpaceVoyageMap()
        {
            if (targetSpaceVoyage.gameObject.activeSelf)
            {
                targetSpaceVoyage.gameObject.SetActive(false);
            }
            else
            {
                targetSpaceVoyage.gameObject.SetActive(true);
            }
        }

        public bool CheckInterectObject(GameObject _targetObject)
        {
            return true;
        }

        public void InterectObjectAction(GameObject _targetObject)
        {
            OnOffSpaceVoyageMap();
        }

        public string GetInterectText()
        {
            return "탑승하시겠습니까?";
        }
    }
}
#endif

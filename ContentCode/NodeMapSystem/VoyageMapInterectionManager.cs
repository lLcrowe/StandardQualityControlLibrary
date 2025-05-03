#if MEC

using lLCroweTool.Singleton;
using UnityEngine;
namespace lLCroweTool.NodeMapSystem
{   
    public class VoyageMapInterectionManager : MonoBehaviourSingleton<VoyageMapInterectionManager>
    {
        //20220205신규
        //항해맵과 맵상의 시작위치에 대해 상호작용해주는 클래스


        //전초기지와 함선을 연관시켜주는 기능을 가진 클래스
        //함선오퍼레이션에서 씬매니저와 작업해야함
        //20200315
        //추가 내용
        //현 매니저의 기능은 게임씬로드 매니저가 씬을 로드뒤에
        //해당씬이 게임맵에 쓰이는 씬이면
        //함선과 함선안에 있는 오브젝트와 선원들을 새로 불러오는 맵의
        //특정위치에 배치하는 용도
        //현 매니저 같은 경우 DontDestoryObject를 사용하지 않는다
        //현 매니저의 사용법은 해당 씬이 게임맵씬일 경우
        //해당 씬에 현 매니저를 하나 배치해줘야 한다.
        //게임로드씬에서 현재 매니저가 있느냐 없느냐에 따라
        //로드하고 난 뒤 배치가 되느냐 없느냐를 작업할 예정
        //로드씬오기전에 함선오브젝트 부모를 눌로 만들어줘야함
        //MoveToLine 에셋 구매함
        //쉽게 작업할수 있음. 사용법을 좀더 알아봐야함
        //사용법 체크 패스파인더도 있지만 아직은 안쓸 예정



        //자동으로 움직이게 해줄 오브젝트(함선이나 적군 오브젝트여야함)
        //계속 변환됨
        public GameObject targetSpaceShip;//함선 타겟팅      
        public Rigidbody2D targetRigidbody2D;
        public Transform targetBasement;//전초기지 타겟팅
        //씬이 시작할시 함선이 배치될 위치
        public Transform enterSpaceShipPosition;
        
        [Space]
        private CoroutineTimerModule coroutineTimer;
        //일정 시간마다 함선이 옮겨다닐 위치
        //public LineGenerate[] mapLandingPointDatas;
        //[Space]
        //public LineGenerate curMapLandingLineData;//현재 맵랜딩 위치
        //public LineGenerate targetMapLandingLineData;//타겟 맵랜딩
        private bool isDone = false;//도착여부

        //우주선이 움직일때 기지와의 상호작용을 피하기 위한 방법
        public LayerMask spaceLayer;//우주레이어
        public LayerMask baseLayer;//기지가 있는레이어
        

        

        protected override void Init()
        {
            coroutineTimer = GetComponent<CoroutineTimerModule>();
            coroutineTimer.SetTimer(0.02f);
            //coroutineTimer.AddUnityEvent(delegate { MoveLineToTarget(); });
            coroutineTimer.enabled = false;
        }

        //게임시작할때 몇가지 오브젝트를 해당 위치에 배치
        //GameScene에서 작업하는 용도(외부함수)
        public void StartGameBatch(GameObject targetObject)
        {
            //회전 위치 부모
            //설정
            targetObject.transform.position = enterSpaceShipPosition.position;
            targetObject.transform.rotation = enterSpaceShipPosition.rotation;
            targetObject.transform.parent = targetBasement;
            targetRigidbody2D = targetObject.GetComponent<Rigidbody2D>();
        }

       
        //[ButtonMethod]
        ////함선을 움직이게 만드는 함수
        //public void MoveSpaceShip()
        //{
        //    if (ReferenceEquals(targetMapLandingLineData, null))
        //    {
        //        DebugManager.Instance.UseDebug("타겟이 된 맵 랜딩데이터가 없습니다");
        //        return;
        //    }
        //    if (targetMapLandingLineData.Equals(curMapLandingLineData))
        //    {
        //        return;
        //    }

        //    //해당 위치로 움직이게함            
        //    coroutineTimer.enabled = true;
        //    //MoveTargetPosition(moveSpaceShipPosition[index]);
        //}

        //[ButtonMethod]
        ////함선을 랜덤으로 움직이게 만드는 함수
        //public void RandomMoveSpaceShip()
        //{
        //    //랜덤 위치 선정
        //    int index = GetRandomPosition();
        //    targetMapLandingLineData = mapLandingPointDatas[index];

        //    if (targetMapLandingLineData.Equals(curMapLandingLineData))
        //    {
        //        return;
        //    }
        //    //해당 위치로 움직이게함            
        //    coroutineTimer.enabled = true;
        //    //MoveTargetPosition(moveSpaceShipPosition[index]);
        //}

        //public int GetRandomPosition()
        //{
        //    return Random.Range(0, mapLandingPointDatas.Length);
        //}

        ////타겟이 된 위치로 일정하게 곡선이나 직선으로 움직이게 해주는 함수
        ////코루틴에서 작업
        //private void MoveLineToTarget()
        //{
        //    if (ReferenceEquals(targetMapLandingLineData, null))
        //    {
        //        coroutineTimer.enabled = false;
        //        //해당 맵데이터 없음
        //        //정지
        //        return;
        //    }

        //    //도착안했으면 계속 이동
        //    //우주선의 이동속도와 회전속도를 집어넣을것
        //    targetMapLandingLineData.Move(targetSpaceShip.transform, 10f, MoveAlignment.transformZ, 2, 10f);
        //    if (!targetMapLandingLineData.IsMove(targetSpaceShip.transform))
        //    {
        //        curMapLandingLineData = targetMapLandingLineData;
        //        targetMapLandingLineData = null;
        //        isDone = true;
        //        coroutineTimer.enabled = false;
        //    }
        //}
    }
}

#endif
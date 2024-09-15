#if Doozy
using UnityEngine;
using System;
using System.Collections.Generic;
using lLCroweTool.NodeMapSystem;
using lLCroweTool.InventorySystem;
using lLCroweTool.WorldObjectSystem;
using lLCroweTool.StatusModuleSystem;
using lLCroweTool.SkillSystem;

namespace lLCroweTool.QuestSystem
{
    [Serializable]
    public class QuestBookData
    {
        //퀘스트북은 퀘스트들로 이루어져있다. 해당퀘스트들은 특정임무들이 있으며 임무가 완료되야지 퀘스트가 완료된다.

        public ActorInfoObjectScript actorInfoData;//액터
        [TextArea] public string questBookIntroduce;//퀘스트 소개창//개발자가 보고 판단할거
        public string questBookTitle;//퀘스트북 제목//현지화와 연동
        public string questBookContentInfo;//퀘스트북내용 안내창//현지화와 연동
        public string questBookIDKey = "";//현재 퀘스트키//유니크 해야함
        public string[] precedeQuestBookIDKeyArray = new string[0];//선행퀘스트키들//해당퀘스트가 클리어되야지 표시됨

        //퀘스트북설정 상태
        public QuestMissionCATType questType = QuestMissionCATType.Main;//테마
        public bool isShowToUser;//유저한테 보여주는 퀘스트인지//일단 대기
        public bool isGiveUpQuest;//포기가능한지//포기했을시 실패로 체크
        public bool isLoopQuest;//반복퀘스트인지
        
        //노드데이터들
        public QuestNodeData[] questNodeDataArray = new QuestNodeData[0];
        public DialogueEventNodeData[] dialogueEventNodeDataArray = new DialogueEventNodeData[0];
        public LinkedQuestBookNodeData[] linkedQuestBookNodeDataArray = new LinkedQuestBookNodeData[0];        

        //시작노드와이어진 첫번째노드
        public string firstContactNodeGUID = "";
        public CustomQuestNodeData[] nodeDataArray = new CustomQuestNodeData[0];

        //보상 Reward 
        public QuestRewardGiveType questRewardGiveType;
        public RewardData[] questBookRewardDataArray = new RewardData[0];
    }

    [Serializable]
    public class QuestNodeData 
    {
        //관계도도 필요할거 같음
        //퀘스트가 어떤 대화랑 이어지는지 어떤퀘스트랑 이어지는
        public string nodeGUID;//노드의 고유번호
        public string questName;//퀘스트의 이름 => 노드로만은 해석이 불가능하니 이걸로 유추가능하게//씬상의 인스팩터에 퀘스트오브젝트에서 선택함//현지화와 연동
        public string questContentInfo;//퀘스트내용 안내창//현지화와 연동

        [TextArea]        
        public string questHUDContent;//퀘스트HUD에 뿌릴 텍스트내용//현지화와 연동

        //추가적인 클리어조건
        public bool isUseTimer;
        public int time;//제한시간

        //해당 퀘스트들의 조건들
        //대화 1.누군가에게 대화 2.전달      //액터와의 상호작용
        //전투 1.처치or파괴 2.HP체크        //게임월드오브젝트의 스탯과 상호작용
        //생존 1.생존 2.보호 3.호위         //게임월드오브젝트의 스탯과 상호작용
        //이동 1.이동 2.잠입 3.미행     
        //유인 1.유인 => 특정대상을 지역 or 지점으로부터 거리 체크                   //게임월드의 상호작용을
        //이동 //특정위치로 이동
        //오브젝트or아이템 1.채집 2.조작  1.아이템사용 2.의상착용 3.스킬사용           //게임월드의 상호작용 여부
        //맵지역도착//맵데이터의 이름과 현재씬의 이름
        public DialogueMissionData[] dialogueMissionDataArray = new DialogueMissionData[0];
        public BattleMissionData[] battleMissionDataArray = new BattleMissionData[0];
        public SurviveMissionData[] surviveMissionDataArray = new SurviveMissionData[0];
        public AttractMoveMissionData[] attractMoveMissionDataArray = new AttractMoveMissionData[0];        
        public ItemGatherMissionData[] itemGatherMissionDataArray = new ItemGatherMissionData[0];
        public ArriveMapMissionData[] arriveMapMissionDataArray = new ArriveMapMissionData[0];
    }

    /// <summary>
    /// 특정대화이벤트를 불려오는 데이터
    /// </summary>
    [Serializable]
    public class DialogueEventNodeData
    {
        public string nodeGUID;//노드의 고유번호
        public string dialogueID;//해당되는 아이디를 체크
    }

    /// <summary>
    /// 특정퀘스트북을 연계해주는 데이터
    /// </summary>
    [Serializable]
    public class LinkedQuestBookNodeData
    {
        public string nodeGUID;//노드의 고유번호
        public string questBookID;//해당퀘스트를 깻을시 곧장 이어지는 연계퀘스트들
    }

    //대화 1.누군가에게 대화 2.전달      //액터와의 상호작용
    //전투 1.처치or파괴 2.HP체크        //게임월드오브젝트의 스탯과 상호작용
    //생존 1.생존 2.보호 3.호위         //게임월드오브젝트의 스탯과 상호작용    
    //유인 1.유인 => 특정대상을 지역 or 지점으로부터 거리 체크                   //게임월드의 상호작용을
    //이동 //특정위치로 이동
    //오브젝트or아이템 1.채집 2.조작  1.아이템사용 2.의상착용 3.스킬사용           //게임월드의 상호작용 여부
    //맵지역도착//맵데이터의 이름과 현재씬의 이름

    /// <summary>
    /// 대화미션 데이터
    /// 특정액터와 대화를 해야함
    /// </summary>
    [Serializable]
    public class DialogueMissionData
    {
        //클리어조건
        //UI에 표시 : 액터와 대화하라
        public ActorInfoObjectScript actorInfoData;
    }

    /// <summary>
    /// 전투미션 데이터
    /// 특정 월드오브젝트를 파괴하거나 특정피 미만으로 만들어야함
    /// </summary>
    [Serializable]
    public class BattleMissionData
    {
        //클리어조건
        //UI에 표시 : 타겟을 파괴하라
        public string missionTargetID = "";//타겟과의 연결고리
        public int value;//파괴할 수량

        //후에 추가기능 수정사항있을거 같음
    }

    /// <summary>
    /// 생존미션 데이터
    /// 특정 월드오브젝트가 임무가 끝날때까지 생존하거나 숫자를 유지해야함
    /// </summary>
    [Serializable]
    public class SurviveMissionData
    {
        //클리어조건
        //UI에 표시 : 타겟을 살려라
        public string missionTargetID = "";//타겟과의 연결고리
        public int value;//살려야할 수량

        //후에 추가기능 수정사항있을거 같음
    }

    /// <summary>
    /// 유인이동미션 데이터
    /// 특정팀, 유닛을 특정위치로 유인해야함
    /// </summary>
    [Serializable]
    public class AttractMoveMissionData
    {
        //클리어조건
        //UI에 표시 : 아군 => 위치로 움직여라 적군 => 위치로 유인하라        
        public UnitTeamType unitTeamType;//특정 팀
        public bool isUseTargetUnitStat;//유닛 선별 여부
        public UnitStatusObjectScript unitStatusData;//선별되는 유닛
    }

    /// <summary>
    /// 아이템수집미션
    /// 특정한 아이템을 특정수량만큼 수집해야함
    /// </summary>
    [Serializable]
    public class ItemGatherMissionData
    {
        //클리어조건
        //UI에 표시 : 아이템을 모아라
        public ItemObjectScript itemData;
        public int itemCount;
    }

    /// <summary>
    /// 특정맵 도착미션
    /// 특정한맵에 도착해야하는 미션
    /// </summary>
    [Serializable]
    public class ArriveMapMissionData
    {
        //클리어조건
        //UI에 표시 : 지역에 도달하라
        public MapMarkerObjectScript mapMarkerData;
    }

    ///<summary>
    ///보상에 대한 모든걸 정의해주는곳
    ///</summary>
    [Serializable]
    public class RewardData
    {
        //아이템 보상
        public ItemObjectScript itemData;
        public int itemCount;

        //유닛 보상
        public TestWorldUnitObject targetUnitObject;        
        public int unitCount;

        //경험치 보상
        public GiveType unitExprienceGiveType;
        public int unitExprience;

        //스킬포인트 보상
        public GiveType skillPointGiveType;
        public int skillPoint;

        //스킬보상
        public GiveType skillDataGiveType;
        public SkillObjectScript skillData;

        //오브젝트 보상
        public ObjectDeScription_Base objectDeScriptionData;
        public GameObject targetObject;
    }

    /// <summary>
    /// 커스텀 퀘스트노드데이터
    /// </summary>
    [Serializable]
    public class CustomQuestNodeData
    {
        public QuestNodeType nodeType;//노드타입에 따라 작동할수 있는게 다름
        public string nodeGUID;//노드의 고유번호
        public Vector2 position;//그래프 뷰 용

        //다음으로 이어질 노드아이디들        
        public List<string> nextNodeGUIDList;
    }

    /// <summary>
    /// 퀘스트 및 미션테마
    /// 메인퀘스트   : 주가되는 퀘스트, 실패하면 게임오버가 됨
    /// 서브퀘스트   : 서브가 되는 퀘스트, 추가되는 이득을 줌. 실패해도 상관없음
    /// 스페셜퀘스트  : 희귀한 퀘스트, 상황을 뒤바꿀수 있는 퀘스트. 실패하거나 성공할시 게임오버는 안하지만 강력한 무언가를 줌
    /// 돌발 퀘스트 : 갑자기 생긴 , 예상치 못한 퀘스트로 도움또는 리스크를 질수밖에 없음
    /// </summary>
    public enum QuestMissionCATType
    {
        Main,
        Sub,
        Special,
        Unexpected,
    }

    /// <summary>
    /// 퀘스트노드 타입
    /// </summary>
    public enum QuestNodeType
    {
        QuestNode,//퀘스트노드
        DialogueEvent,//링크된 대화작동
        LinkedQuestBook,//다음 퀘스트를 곧장 발행
        RandomBranch,//랜덤        
        StartPoint,
    }

    /// <summary>
    /// 퀘스트미션에 대한 타입들(그룹용)
    /// </summary>
    public enum QuestMissionType
    {
        Dialogue,
        Battle,
        Survive,
        AttractMove,        
        ItemGather,
        ArriveMap,
    }

    public enum QuestRewardGiveType
    {
        All,
        Select,
        Random,
    }

    public enum GiveType
    {
        AllUnit,
        PlayerUnit,
        PartnerUnit,
    }

    /// <summary>
    /// 카운트체커타입
    /// </summary>
    public enum CountCheckerType
    {
        Success,
        Failed,
    }

    /// <summary>
    /// 발주처 퀘스트북
    /// </summary>
    [Serializable]
    public class OrderQuestMissionData
    {
        public string questBookID = "";//퀘스트북아이디
        //[QuestNameGroup] public string questName;//퀘스트이름
        public string questName = "";//퀘스트이름
        //그룹화하기
        public QuestMissionType questMissionType = QuestMissionType.Dialogue;//해당미션의타입으로 퀘스트들에서 체크후 밑의 미션아이디를 출력
        public string missionID = "";//미션아이디
    }
}

#endif
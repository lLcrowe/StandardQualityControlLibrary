#if Doozy
using lLCroweTool.QuestSystem;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace lLCroweTool.QC.EditorOnly
{
    /// <summary>
    /// 그래프의 퀘스트노드
    /// </summary>
    public class QuestUINode : Node
    {
        public string GUID;//임의의 유니크노드값
        public string nodeName;//노드이름        
        public QuestNodeType nodeType;//노드타입
    }

    /// <summary>
    /// 퀘스트 노드
    /// </summary>
    public class QuestNode : QuestUINode
    {
        public TextField questNameTextField;
        public TextField questContentInfoTextField;
        public TextField questHUDContentTextField;
        
        public Toggle isUseTimerField;
        public IntegerField timeField;

        public Label dialogueMissionNoticeLabel;
        public ScrollView dialogueMissionScrollView;
        public Label battleMissionNoticeLabel;
        public ScrollView battleMissionScrollView;
        public Label surviveMissionNoticeLabel;
        public ScrollView surviveMissionScrollView;
        public Label attractMoveMissionNoticeLabel;
        public ScrollView attractMoveMissionScrollView;        
        public Label itemMissionNoticeLabel;
        public ScrollView itemMissionScrollView;
        public Label arriveMissionNoticeLabel;
        public ScrollView arriveMapMissionScrollView;       

        //실질적인 데이터구역
        public string questName;
        public string questContentInfo;
        public string questHUDContent;
        
        public bool isUseTimer;
        public int time;

        //필드말고 데이터로 처리해야할듯함
        public List<DialogueMissionDataField> dialogueMissionDataFieldList = new List<DialogueMissionDataField>();
        public List<BattleMissionDataField> battleMissionDataFieldList = new List<BattleMissionDataField>();
        public List<SurviveMissionDataField> surviveMissionDataFieldList = new List<SurviveMissionDataField>();
        public List<AttractMoveMissionDataField> attractMoveMissionDataFieldList = new List<AttractMoveMissionDataField>();        
        public List<ItemGatherMissionDataField> itemGatherDataFieldList = new List<ItemGatherMissionDataField>();
        public List<ArriveMapMissionDataField> arriveMapMissionDataFieldList = new List<ArriveMapMissionDataField>();
    }

    /// <summary>
    /// 대화이벤트 노드
    /// </summary>
    public class DialogueEventNode : QuestUINode
    {
        public PopupField<string> dialogueIDField;
    }

    /// <summary>
    /// 연계퀘스트북 노드
    /// </summary>
    public class LinkedQuestBookNode : QuestUINode
    {
        public PopupField<string> questBookIDField;
    }

    /// <summary>
    /// 랜덤 분기노드//연결되있는 포트를 랜덤으로 가져와서넘김
    /// </summary>
    public class RandomBranchQuestNode : QuestUINode
    {

    }

    /// <summary>
    /// 엔드 노드//실패와 성공여부를 마추어줌
    /// </summary>
    public class EndNode : QuestUINode
    {
        public Toggle successField;
    }
}
#endif
#if Doozy

using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace lLCroweTool.DialogueSystem
{
    /// <summary>
    ///대화 데이터
    /// </summary>
    [Serializable]
    public class DialogueData
    {
        public string dialogueIDKey = "";//유니크키//프로퍼티로 작동
        public bool isStopTime = false;//시간정지후 작동되는지
        public bool isSkippable = false;//스킵가능한지 여부//사용하는가?//필요한가?
        //오토스킵과는 다른 해당대화를 스킵하겠다는 소리//20210727//나중에 작업
        //컨트롤만 누르면 매프레임마다 해당 대화내용데이터를 자동스킵(?)
        //20220612제작완료

        //커스텀 프로퍼티(변수)//스크립터블저장용//리드온리
        public CustomPropertyBool[] customPropertyBoolArray = new CustomPropertyBool[0];
        public CustomPropertyFloat[] customPropertyFloatArray = new CustomPropertyFloat[0];
        public CustomPropertyInt[] customPropertyIntArray = new CustomPropertyInt[0];
        public CustomPropertyString[] customPropertyStringArray = new CustomPropertyString[0];

        //노드관련
        public DialogueContentData[] dialogueContentDataArray = new DialogueContentData[0];//대화내용들
        public SetPropertyData[] setPropertyDataArray = new SetPropertyData[0];//프로퍼티세팅내용들
        public ConditionBranchNodeData[] conditionBranchNodeDataArray = new ConditionBranchNodeData[0];//조건분기노드
        public SelectBrandNodeData[] selectBrandNodeDataArray = new SelectBrandNodeData[0];//선택분기노드
        public SelectQuestBrandNodeData[] selectQuestBrandNodeDataArray = new SelectQuestBrandNodeData[0];//퀘스트선택분기노드        
        public QuestEventData[] questEventDataArray = new QuestEventData[0];//퀘스트이벤트노드
        public LinkedDialogueData[] linkedDialogueDataArray = new LinkedDialogueData[0];//대화링크노드

        //노드관련 
        //public List<NodeLinkData> nodeLinks = new List<NodeLinkData>();
        //시작노드와이어진 첫번째노드
        public string firstContactNodeGUID = "";
        public CustomNodeData[] nodeDataArray = new CustomNodeData[0];
    }

    /// <summary>
    /// 대화내용이 있는 데이터
    /// </summary>
    [Serializable]
    public class DialogueContentData
    {
        public string nodeGUID = "";//노드의 고유번호
        //실질적인 데이터구역
        public ActorInfoObjectScript targetActorInfoData;//타겟이 될 액터(배우)//스트링으로 바꿀것 DB에 액터배열이 존재함

        public ActorShowBeginType actorShowBegin;//시작시 액터보여주는 방법
        public ActorShowPos actorShowPos;//보여줄때 위치
        public ActorShowEndType actorShowEnd;//끝날시 액터보여주는 방법

        [LocalizeGroup] public string localizeID = "";//대화 내용이 있을 로컬라이징데이터
        public string dialogueContent = "";//대화 내용
        public AnimamtionPortraitMessageType animamtionPortraitMessageType;//액터의 포트레이트애니메이션 작동을 위한 이름
        

        public DialogueChatType dialogueChatType;//대화가 어떤방식으로 보여줄지

        public TMP_FontAsset overrideFontAsset;//대화에 사용할 덮어쓰기폰트

        public GameObject focusObject;//대화중에 보여줄 추가 오브젝트를 지정//프리팹으로 가져야 될것
    }


    //유니티 애니메이터 파라미터값을 보고 제작함
    /// <summary>
    /// 대화데이터에 있는 커스텀 파라미터값// 노출된 변수
    /// </summary>    
    [Serializable]
    public class CustomPropertyBool
    {
        public string propertyName = "";//이름
        public bool propertyBoolValue;//값
    }
    [Serializable]
    public class CustomPropertyFloat
    {
        public string propertyName = "";
        public float propertyFloatValue;
    }
    [Serializable]
    public class CustomPropertyInt
    {
        public string propertyName = "";        
        public int propertyIntValue;
    }
    [Serializable]
    public class CustomPropertyString 
    {
        public string propertyName = "";
        public string propertyStringValue;     
    }

    [Serializable]
    public class SetPropertyData
    {
        public string nodeGUID = "";//노드의 고유번호
        public PropertyType propertyType;//변수타입
        public string propertyName = "";//변수이름
        public PropertySetType propertySetType;//int와 Float일때 세팅할것인지 더해줄것인지 확인용도

        //변수타입마다 값주고받아오는구역
        public bool boolValue;
        public float floatVaule;
        public int intValue;
        public string stringValue = "";
    }

    [Serializable]
    public class ConditionBranchNodeData
    {
        public string nodeGUID = "";//노드의 고유번호
        public PropertyType targetPropertyType;//변수타입
        public string targetPropertyName;//변수이름

        public ComparisonOperatorType operatorType;//비교할 연산자
        //변수타입마다 값주고받아오는구역
        public bool boolValue;
        public float floatVaule;
        public int intValue;
        public string stringValue = "";
    }

    [Serializable]
    public class SelectBrandNodeData
    {
        public string nodeGUID = "";//노드의 고유번호
        public string[] selectContentArray;
        public string[] localizeArray;
    }

    [Serializable]
    public class SelectQuestBrandNodeData
    {
        public string nodeGUID = "";//노드의 고유번호
        public string[] questBookIDKeyArray;        
    }

    [Serializable]
    public class GameWorldEventData
    {
        public string nodeGUID = "";//노드의 고유번호
        public string gameWorldEventName;//게임월드이벤트이름
    }

    [Serializable]
    public class QuestEventData
    {
        public string nodeGUID = "";//노드의 고유번호
        public string questBookID;//퀘스트북아이디와 연동//다이렉트로 작동
    }

    [Serializable]
    public class LinkedDialogueData
    {
        public string nodeGUID = "";//노드의 고유번호
        public string dialogueID;//대화아이디와 연동//다이렉트로 작동
    }

    /// <summary>
    /// 커스텀 노드데이터
    /// </summary>
    [Serializable]
    public class CustomNodeData
    {
        public DialogueNodeType nodeType;//노드타입에 따라 작동할수 있는게 다름
        public string nodeGUID = "";//노드의 고유번호
        public Vector2 position;//그래프 뷰 용

        //다음으로 이어질 노드아이디들
        public List<string> nextNodeGUIDList;
    }

    /// <summary>
    /// 노드링크데이터(선)
    /// </summary>
    //[Serializable]
    //public class NodeLinkData
    //{
    //    //아웃풋포트에서 인풋포트간의 연결을 뜻함
    //    //아웃풋이 현재노드 인풋은 타겟노드
    //    public string baseNodeGUID;
    //    public DialogueNodeType baseNodeType;
    //    public string portName;
    //    public DialogueNodeType targetNodeType;
    //    public string targetNodeGUID;
    //}

    /// <summary>
    /// 분기타입
    /// </summary>
    public enum BranchType
    {
        Select,//선택
        Random,//랜덤
        Trigger,//둘중하나
    }


    /// <summary>
    /// 트리거 사용시 비교연산자 타입
    /// </summary>
    public enum ComparisonOperatorType
    {
        Greater,//크다
        //크고같다,//greater than or equal
        Less,//작다
        //작고같다,//less than or equal
        Equal,//같다
        //다르다,//not equal
    }
    public enum PropertyType
    {
        Bool,//세팅
        Float,//더해줌
        Int,//더해줌
        String,//세팅
    }

    public enum PropertySetType
    {
        Set,
        Add,
    }

    /// <summary>
    /// 9개의 노드타입을 가짐
    /// </summary>
    public enum DialogueNodeType
    {
        Dialogue,
        SetProperty,
        ConditionBranch,
        SelectBranch,
        RandomBranch,
                
        QuestSelectBranch,
        QuestEvent,
        LinkedDialogue,

        StartPoint,
    }
      
    public enum DialogueChatType
    {
        Default,//기본//다음 대화와 다음대화간의 사이가 곧장 교체됨
        Sequence_Char,//순서//다음대화와 다음대화간의 사이가 한글자씩 교체됨
        Sequence_Word,//순서//다음대화와 다음대화간의 사이가 단어씩 교체됨
        Sequence_Sentence//순서//다음대화와 다음대화간의 사이가 문장씩 교체됨
    }
}
#endif
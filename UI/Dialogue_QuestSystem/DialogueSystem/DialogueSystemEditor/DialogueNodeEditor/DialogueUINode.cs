#if Doozy

using lLCroweTool.DialogueSystem;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace lLCroweTool.QC.EditorOnly
{
    /// <summary>
    /// 그래프의 대화노드
    /// </summary>
    public class DialogueUINode : Node
    {
        public string GUID;//임의의 유니크노드값
        public string nodeName;//노드이름        
        public DialogueNodeType nodeType;//노드타입
    }

    /// <summary>
    /// 대화 노드
    /// </summary>
    public class  DialogueNode : DialogueUINode
    {
        //필드
        //데이터캐싱용(로드)
        public ObjectField targetActorInfoDataObjectField;
        public EnumField actorShowBeginTypeEnumField;
        public EnumField actorShowPosTypeEnumField;
        public EnumField actorShowEndTypeEnumField;

        public TextField localizeIDTextField;
        public TextField dialogueContentTextField;
        public EnumField animationPortraitMessageTypeEnumField;
        public EnumField dialogueChatTypeEnumField;
        public ObjectField overideFontAssetObjectField;
        public ObjectField focusObjectObjectField;


        //실질적인 데이터구역
        public ActorInfoObjectScript targetActorInfoData;//타겟이 될 액터(배우)

        public ActorShowBeginType actorShowBegin;//시작시 액터보여주는 방법
        public ActorShowPos actorShowPos;//보여줄때 위치
        public ActorShowEndType actorShowEnd;//끝날시 액터보여주는 방법

        [LocalizeGroup] public string localizeID;//대화 내용이 있을 로컬라이징데이터
        public string dialogueContent;//대화 내용
        public AnimamtionPortraitMessageType animamtionPortraitMessageType;//액터의 포트레이트애니메이션 작동을 위한 이름
        
        public DialogueChatType dialogueChatType;//대화가 어떤방식으로 보여줄지

        public TMP_FontAsset overideFontAsset;//대화에 사용할 덮어쓰기폰트

        public GameObject focusObject;//대화중에 보여줄 추가 오브젝트를 지정//프리팹으로 가져야 될것 
    }

    /// <summary>
    /// 프로퍼티 세팅 노드
    /// </summary>
    public class SetCustomPropertyNode : DialogueUINode
    {
        public EnumField customPropertyTypeEnumField;
        public PopupField<string> testField;
        public Toggle boolField;
        public EnumField customPropertySetTypeEnumField;
        public FloatField floatField;
        public EnumField customPropertySetTypeEnumField1;
        public IntegerField intField;
        public TextField stringField;


        //실질적인 데이터구역
        public PropertyType propertyType;//변수타입
        public string propertyName;//변수이름        
        public PropertySetType propertySetType;//int와 Float일때 세팅할것인지 더해줄것인지 확인용도

        //변수타입마다 값주고받아오는구역
        public bool boolValue;
        public float floatVaule;
        public int intValue;
        public string stringValue;

        //에디터에서 사용할 프로퍼티필드
        public PropertyField propertyField;
    }

    /// <summary>
    /// 조건 분기노드 특정파리미터값을 true false로 체크함
    /// </summary>
    public class ConditionBranchNode : DialogueUINode
    {
        public EnumField customPropertyTypeEnumField;
        public PopupField<string> testField;
        public EnumField customValueTypeEnumField;
        public Toggle boolField;
        public FloatField floatField;
        public IntegerField intField;
        public TextField stringField;

        //실질적인 데이터구역
        public PropertyType targetPropertyType;//변수타입
        public string targetPropertyName;//변수이름

        public ComparisonOperatorType operatorType;//비교할 연산자
        public bool boolValue;
        public float floatVaule;
        public int intValue;
        public string stringValue;

        //에디터에서 사용할 프로퍼티필드
        public PropertyField propertyField;
    }

    /// <summary>
    /// 선택 분기노드//연결되있는 포트를 선택하여 넘김
    /// </summary>
    public class SelectBranchNode : DialogueUINode
    {
       //실질적인 데이터구역
       public List<TextField> selectContentList = new List<TextField>();
        public List<TextField> localizeList = new List<TextField>();
    }
    public class QuestSelectBranchNode : DialogueUINode
    {
        //실질적인 데이터구역
        public List<PopupField<string>> questBookIDFieldList = new List<PopupField<string>>();
    }

    /// <summary>
    /// 랜덤 분기노드//연결되있는 포트를 랜덤으로 가져와서넘김
    /// </summary>
    public class RandomBranchNode : DialogueUINode
    {
        
    }

    /// <summary>
    /// 게임월드이벤트이벤트 노드// 해당 이벤트를 작동시키고 다음노드로 작동
    /// </summary>
    public class GameWorldEventNode : DialogueUINode
    {
        public PopupField<string> gameWorldEventNameField;
    }
    

    /// <summary>
    /// 퀘스트이벤트노드//특정퀘스트가 작동되게하는 노드이다.
    /// </summary>
    public class QuestEventNode : DialogueUINode
    {
        public PopupField<string> questBookIDField;
    }

    /// <summary>
    /// 
    /// </summary>
    public class LinkedDialogueNode : DialogueUINode
    {
        public PopupField<string> dialogueIDField;
    }
}
#endif
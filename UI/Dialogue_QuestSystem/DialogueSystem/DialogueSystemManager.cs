#if MEC && Doozy

using UnityEngine;
using System.Collections.Generic;
using MEC;
using Doozy.Engine.UI;
using Febucci.UI;
using TMPro;
using lLCroweTool.Dictionary;
using lLCroweTool.ObjectPool;
using lLCroweTool.DestroyManger;
using lLCroweTool.TimerSystem;
using lLCroweTool.QuestSystem;
using lLCroweTool.UI.Bar;

namespace lLCroweTool.DialogueSystem
{
    public class UIButtonObjectPool : CustomObjectPool<UIButton> { }
    public class DialogueSystemManager : MonoBehaviour
    {
        private static DialogueSystemManager instance;
        public static DialogueSystemManager Instance
        {
            get
            {
                if (ReferenceEquals(instance, null))
                {
                    instance = FindObjectOfType<DialogueSystemManager>();
                    //if (ReferenceEquals(instance, null))
                    if (ReferenceEquals(instance, null))
                    {
                        GameObject gameObject = new GameObject();
                        instance = gameObject.AddComponent<DialogueSystemManager>();
                        gameObject.name = "-=DialogueSystemManager=-";

                        // ReSharper disable once ArrangeStaticMemberQualifier
                        //_instance = (MasterAudio)GameObject.FindObjectOfType(typeof(MasterAudio));
                        //return _instance;
                    }
                }
                return instance;
            }
        }
        //대화시스템 매니저

        /// <summary>
        /// 적용시킬 대화시스템 DB
        /// </summary>
        public DialogueDBObjectScript dialogueDBData;

        //실질적으로 들어갈 리소스 데이터들
        //
        //대화대상이름을 표시해줄 오브젝트
        //
        //선택버튼 오브젝트
        public Transform selectButtonPos;
        public UIButton selectButtonPrefab;
        private UIButtonObjectPool uIButtonObjectPool = new UIButtonObjectPool();

        //상단에 들어갈 기능버튼
        public UIButton skipButton;
        public UIButton autoStoryOnOffButton;
        public UIButton autoStorySpeedButton;
        public UIButton typingSpeedButton;

        [Space]
        //사용할 대화텍스트 박스//대화박스 오브젝트
        //public DialogueTextBox dialogueTextBox;
        public TextAnimatorPlayer textAnimatorPlayer;
        public TextMeshProUGUI targetTextObject;
        public UIView dialogueBoxUIView;

        private bool isAutoStory = false;//오토스토리진행여부
        private bool skipEvent = false;//버튼에 따른 스킵이벤트용도
        public SpeedType autoSkipSpeed = SpeedType.Normal;
        public SpeedType typingSpeed = SpeedType.Normal;

        [Space]
        //포트레이트위치
        public Transform portraitLeftPos;//좌측
        public Transform portraitMiddlePos;//중간
        public Transform portraitRightPos;//우측

        [Space]
        //현재활성화된 포트레이트들
        //대화DB데이터안의 액터들의 초상화들을 순서되로 집어넣고 
        //검색은 DB에서 표현은 여기서 꺼내서 보여줌
        private PortraitUICard[] portraitUICardArray = new PortraitUICard[0];

        //활성화된 포트레이트카드 리스트들
        //해당되는 대화라인이 다끝나면 비활성화시키고 클리어시키기
        private List<PortraitUICard> activePortraitUICardList = new List<PortraitUICard>();
        
        [Space]
        //추가 오브젝트 나타날 위치
        public Transform focusObjectPos;
        //타이밍바 => 자동스킵이 온일시 다음대화로 넘어갈 시간을 알려주는 용도
        public UIBar_Base timingBar;

        [System.Serializable]
        public class DialogueInterectLogBible : CustomDictionary<string, DialogueInterectLogData> { }//
        [Space]
        [Header("대화기록")]
        public DialogueInterectLogBible dialogueLogBible = new DialogueInterectLogBible();//대화키, 대화상호작용로그데이터

        //캐싱구역
        private bool isUseDialogueSystem = false;//대화시스템이 작동되는지여부//이게 아닌후에 작동시키면 멈추지않고 그냥 진행하는 대화        
        private CoroutineHandle actionCoroutine;//대화시스템 작동 코루틴

        //노드선택시 사용하는 변수
        private bool isSelectNodeClick = false;
        private int selectNum = 0;

        private void Awake()
        {
            instance = this;

            if (dialogueDBData != null)
            {    
                //중복되지않는값들 체크
                List<PortraitUICard> portraitUICardsList = new List<PortraitUICard>();
                List<ActorInfoObjectScript> actorInfoList = new List<ActorInfoObjectScript>();
                for (int i = 0; i < dialogueDBData.dialogueDataArray.Length; i++)
                {
                    DialogueData dialogueData = dialogueDBData.dialogueDataArray[i];

                    for (int j = 0; j < dialogueData.dialogueContentDataArray.Length; j++)
                    {
                        if (!actorInfoList.Contains(dialogueData.dialogueContentDataArray[j].targetActorInfoData))
                        {
                            if (dialogueData.dialogueContentDataArray[j].targetActorInfoData != null)
                            {
                                PortraitUICard portraitUICard = Instantiate(dialogueData.dialogueContentDataArray[j].targetActorInfoData.portraitUICardPrefab, transform);
                                portraitUICard.actorInfoData = dialogueData.dialogueContentDataArray[j].targetActorInfoData;
                                actorInfoList.Add(dialogueData.dialogueContentDataArray[j].targetActorInfoData);
                                portraitUICardsList.Add(portraitUICard);
                            }
                        }
                    }
                    
                }
                portraitUICardArray = portraitUICardsList.ToArray();
            }

            uIButtonObjectPool.SetPrefab(selectButtonPrefab);

            //상단버튼 이벤트집어넣기
            autoStoryOnOffButton.Button.onClick.AddListener(ChangeAutoTime);
            autoStoryOnOffButton.SetLabelText(isAutoStory.ToString());
            autoStorySpeedButton.Button.onClick.AddListener(ChangeAutoSkipSpeed);
            typingSpeedButton.Button.onClick.AddListener(ChangeTypingSpeed);
            skipButton.Button.onClick.AddListener(ChangeSkipEvent);

            timingBar.SetCurValue(0);
        }

        /// <summary>
        /// 대화ID 대화시스템 실행을 요청함
        /// </summary>
        /// <param name="_dialogueIDKey">대화 키</param>
        public void RequestDialogueData(string _dialogueIDKey)
        {
            //이미 작동되고 있으면 더이상 작동안되게
            if (actionCoroutine.IsRunning)
            {
                return;
            }

            CustomNodeData targetNodeData = null;
            DialogueData targetDialogueData = null;
            //진입점은 노드링크에서 찾음
            //대화데이터들을 찾아서 체크
            for (int i = 0; i < dialogueDBData.dialogueDataArray.Length; i++)
            {
                if (dialogueDBData.dialogueDataArray[i].dialogueIDKey == _dialogueIDKey)
                {
                    //키가 동일하면 해당 대화를 진행
                    //대화데이터를 이용하여 대화를 시작
                    //첫번째 시작 노드데이터를 찾음
                    targetNodeData = GetFirstData(dialogueDBData.dialogueDataArray[i]);
                    targetDialogueData = dialogueDBData.dialogueDataArray[i];
                    break;
                }
            }

            if (targetNodeData == null)
            {
                Debug.LogWarning("시작다음 대화데이터가 없습니다.");
                return;
            }

            //찾은 후 해당 노드부터진행
            //코루틴으로 진행
            //1.노드 타입에 따라진행
            //2.다음노드찾음
            //3.없으면 끝

            //로그처리
            if (dialogueLogBible.ContainsKey(_dialogueIDKey))
            {
                dialogueLogBible[_dialogueIDKey].dialogueActionCount++;//작동된 총수
            }
            else
            {
                DialogueInterectLogData dialogueInterectLogData = new DialogueInterectLogData();
                dialogueInterectLogData.dialogueIDKey = _dialogueIDKey;
                dialogueInterectLogData.dialogueActionCount = 0;//작동된 총수

                //bool값
                List<CustomPropertyBool> tempBoolList = new List<CustomPropertyBool>();
                for (int i = 0; i < targetDialogueData.customPropertyBoolArray.Length; i++)
                {
                    CustomPropertyBool temp = new CustomPropertyBool();
                    CustomPropertyBool changeTarget = targetDialogueData.customPropertyBoolArray[i];
                    temp.propertyName = changeTarget.propertyName;
                    temp.propertyBoolValue = changeTarget.propertyBoolValue;
                    tempBoolList.Add(temp);
                }
                dialogueInterectLogData.customPropertyBoolArray = tempBoolList.ToArray();

                //float값
                List<CustomPropertyFloat> tempFloatList = new List<CustomPropertyFloat>();
                for (int i = 0; i < targetDialogueData.customPropertyFloatArray.Length; i++)
                {
                    CustomPropertyFloat temp = new CustomPropertyFloat();
                    CustomPropertyFloat changeTarget = targetDialogueData.customPropertyFloatArray[i];
                    temp.propertyName = changeTarget.propertyName;
                    temp.propertyFloatValue = changeTarget.propertyFloatValue;
                    tempFloatList.Add(temp);
                }
                dialogueInterectLogData.customPropertyFloatArray = tempFloatList.ToArray();

                //int값
                List<CustomPropertyInt> tempIntList = new List<CustomPropertyInt>();
                for (int i = 0; i < targetDialogueData.customPropertyIntArray.Length; i++)
                {
                    CustomPropertyInt temp = new CustomPropertyInt();
                    CustomPropertyInt changeTarget = targetDialogueData.customPropertyIntArray[i];
                    temp.propertyName = changeTarget.propertyName;
                    temp.propertyIntValue = changeTarget.propertyIntValue;
                    tempIntList.Add(temp);
                }
                dialogueInterectLogData.customPropertyIntArray = tempIntList.ToArray();

                List<CustomPropertyString> tempStringList = new List<CustomPropertyString>();
                for (int i = 0; i < targetDialogueData.customPropertyStringArray.Length; i++)
                {
                    CustomPropertyString temp = new CustomPropertyString();
                    CustomPropertyString changeTarget = targetDialogueData.customPropertyStringArray[i];
                    temp.propertyName = changeTarget.propertyName;
                    temp.propertyStringValue = changeTarget.propertyStringValue;
                    tempStringList.Add(temp);
                }
                dialogueInterectLogData.customPropertyStringArray = tempStringList.ToArray();

                dialogueLogBible.Add(_dialogueIDKey, dialogueInterectLogData);
            }

            //시작노드데이터 //대화데이터
            actionCoroutine = Timing.RunCoroutine(ActionDialogueCoroutine(targetNodeData, targetDialogueData));//한번만작동
        }

        /// <summary>
        /// 대화시스템 작동할때 코루틴
        /// </summary>
        /// <param name="_startNodeData">시작할 노드데이터</param>
        /// <param name="_dialogueData">대화데이터</param>
        /// <param name="_dialogueTargetWorldObject">대화 타겟</param>
        /// <returns></returns>
        private IEnumerator<float> ActionDialogueCoroutine(CustomNodeData _startNodeData, DialogueData _dialogueData)
        {
            isUseDialogueSystem = true;
            if (_dialogueData.isStopTime)
            {
                TimerModuleManager.Instance.GamePause();
            }
            

            CustomNodeData _nodeData = _startNodeData;//작동될 노드데이터
            do
            {
                //1. 노드타입에 따라 진행
                //노드에 따른 작동
                bool nodeFindAction = false;
                int _nextNodeDataNum = -1;

                switch (_nodeData.nodeType)
                {
                    case DialogueNodeType.Dialogue:
                        //대화후 넘김
                        DialogueContentData targetContentData = FindDialogueData(_nodeData, _dialogueData);

                        //완료 될떄까지 대기
                        yield return Timing.WaitUntilDone(StartDialogue(targetContentData, _dialogueData));

                        _nextNodeDataNum = 0;
                        nodeFindAction = true;

                        break;
                    case DialogueNodeType.SetProperty:
                        //변수를 세팅후 넘김
                        SetPropertyData targetSetPropertyData = FindSetPropertyData(_nodeData, _dialogueData);

                        //타입에 따른 설정
                        switch (targetSetPropertyData.propertyType)
                        {
                            case PropertyType.Bool:
                                CustomPropertyBool propertyBool = FindCustomPropertyBool(_dialogueData, targetSetPropertyData.propertyName);
                                propertyBool.propertyBoolValue = targetSetPropertyData.boolValue;
                                break;
                            case PropertyType.Float:
                                CustomPropertyFloat propertyFloat = FindCustomPropertyFloat(_dialogueData, targetSetPropertyData.propertyName);
                                switch (targetSetPropertyData.propertySetType)
                                {
                                    case PropertySetType.Set:
                                        propertyFloat.propertyFloatValue = targetSetPropertyData.floatVaule;
                                        break;
                                    case PropertySetType.Add:
                                        propertyFloat.propertyFloatValue += targetSetPropertyData.floatVaule;
                                        break;
                                }
                                break;
                            case PropertyType.Int:
                                CustomPropertyInt propertyInt = FindCustomPropertyInt(_dialogueData, targetSetPropertyData.propertyName);
                                switch (targetSetPropertyData.propertySetType)
                                {
                                    case PropertySetType.Set:
                                        propertyInt.propertyIntValue = targetSetPropertyData.intValue;
                                        break;
                                    case PropertySetType.Add:
                                        propertyInt.propertyIntValue += targetSetPropertyData.intValue;
                                        break;
                                }
                                break;
                            case PropertyType.String:
                                CustomPropertyString propertyString = FindCustomPropertyString(_dialogueData, targetSetPropertyData.propertyName);
                                propertyString.propertyStringValue = targetSetPropertyData.stringValue;
                                break;
                        }
                        _nextNodeDataNum = 0;
                        nodeFindAction = true;
                        break;
                    case DialogueNodeType.ConditionBranch:
                        //조건을 체크후 넘김
                        ConditionBranchNodeData targetConditionBranchNodeData = FindConditionBranchNodeData(_nodeData, _dialogueData);

                        //조건체크
                        bool isTrue = false;
                        switch (targetConditionBranchNodeData.targetPropertyType)
                        {
                            case PropertyType.Bool:
                                CustomPropertyBool propertyBool = FindCustomPropertyBool(_dialogueData, targetConditionBranchNodeData.targetPropertyName);
                                if (propertyBool.propertyBoolValue == targetConditionBranchNodeData.boolValue)
                                {
                                    isTrue = true;
                                }
                                break;
                            case PropertyType.Int:
                                CustomPropertyInt propertyInt = FindCustomPropertyInt(_dialogueData, targetConditionBranchNodeData.targetPropertyName);
                                isTrue = CheckOperator(targetConditionBranchNodeData.operatorType, propertyInt.propertyIntValue, targetConditionBranchNodeData.intValue);
                                break;
                            case PropertyType.Float:
                                CustomPropertyFloat propertyFloat = FindCustomPropertyFloat(_dialogueData, targetConditionBranchNodeData.targetPropertyName);
                                isTrue = CheckOperator(targetConditionBranchNodeData.operatorType, propertyFloat.propertyFloatValue, targetConditionBranchNodeData.floatVaule);
                                break;
                            case PropertyType.String:
                                CustomPropertyString propertyString = FindCustomPropertyString(_dialogueData, targetConditionBranchNodeData.targetPropertyName);

                                if (propertyString.propertyStringValue == targetConditionBranchNodeData.stringValue)
                                {
                                    isTrue = true;
                                }
                                break;
                        }

                        //최종체크
                        //방향체크
                        if (isTrue)
                        {
                            //0
                            _nextNodeDataNum = 0;
                        }
                        else
                        {
                            //1
                            _nextNodeDataNum = 1;
                        }
                        nodeFindAction = true;
                        break;
                    case DialogueNodeType.RandomBranch:
                        //랜덤노드
                        //다음노드만 설정
                        int temp = _nodeData.nextNodeGUIDList.Count;
                        if (temp > 1)
                        {
                            _nextNodeDataNum = Random.Range(0, temp);
                        }
                        else if (temp == 1)
                        {
                            _nextNodeDataNum = 0;
                        }
                        nodeFindAction = true;
                        break;
                    case DialogueNodeType.SelectBranch:
                        //선택분기노드
                        SelectBrandNodeData targetSelectBrandNodeData = FindSelectBrandNodeData(_nodeData, _dialogueData);

                        //선택할 키가존재하는지 체크
                        if (targetSelectBrandNodeData.selectContentArray.Length == 0)
                        {   
                            _nextNodeDataNum = -1;
                        }
                        else
                        {
                            //완료 될떄까지 대기
                            yield return Timing.WaitUntilDone(StartSelect(targetSelectBrandNodeData, _dialogueData.dialogueIDKey));

                            _nextNodeDataNum = selectNum;
                        }
                        nodeFindAction = true;

                        break;
                    case DialogueNodeType.QuestSelectBranch:
                        //퀘스트선택분기노드
                        //선택분기후에 퀘스트연동으로 가도 괜찮음//일단은 그냥둠
                        SelectQuestBrandNodeData targetSelectQuestBrandNodeData = FindSelectQuestBrandNodeData(_nodeData, _dialogueData);

                        //선택할 키가존재하는지 체크
                        if (targetSelectQuestBrandNodeData.questBookIDKeyArray.Length == 0)
                        {
                            _nextNodeDataNum = -1;
                        }
                        else
                        {
                            //완료 될떄까지 대기
                            yield return Timing.WaitUntilDone(StartSelectQuest(targetSelectQuestBrandNodeData, _dialogueData.dialogueIDKey));

                            _nextNodeDataNum = selectNum;
                        }
                        nodeFindAction = true;
                        break;
                    //case DialogueNodeType.GameWorldEvent:
                    //    //게임월드이벤토노드                        
                    //    //단말이다//더이상이어지는 노드가 없음
                    //    GameWorldEventData gameWorldEventData = FindGameWorldEventData(_nodeData, _dialogueData);
                    //    GameWorldEventSystemManager.Instance.RequestWolrdEvent(gameWorldEventData.gameWorldEventName);
                    //    _nextNodeDataNum = -1;
                    //    nodeFindAction = true;
                    //    break;
                    case DialogueNodeType.QuestEvent:
                        //퀘스트이벤트노드
                        //단말이다//더이상이어지는 노드가 없음
                        QuestEventData questEventData = FindQuestEventData(_nodeData, _dialogueData);
                        QuestSystemManager.Instance.RequestQuestBook(questEventData.questBookID);
                        _nextNodeDataNum = -1;
                        nodeFindAction = true;
                        break;

                    case DialogueNodeType.LinkedDialogue:
                        //대화링크
                        //단말이다//더이상이어지는 노드가 존재하지않음
                        LinkedDialogueData linkedDialogueData = FindLinkedDialogueData(_nodeData, _dialogueData);
                        nodeFindAction = false;

                        //링크대화데이터의 대화키값을 찾아서 기존에 작동되던 대화를 변경시킴
                        for (int i = 0; i < dialogueDBData.dialogueDataArray.Length; i++)
                        {
                            if (dialogueDBData.dialogueDataArray[i].dialogueIDKey == linkedDialogueData.dialogueID)
                            { 
                                _nodeData = GetFirstData(dialogueDBData.dialogueDataArray[i]);//첫시작 노드
                                _dialogueData = dialogueDBData.dialogueDataArray[i];//대화데이터변경
                                break;
                            }
                        }
                        break;
                }


                //2. 다음노드찾기   
                //최대수치 체크
                if (nodeFindAction)
                {
                    if (_nodeData.nextNodeGUIDList.Count == 0 || _nodeData.nextNodeGUIDList.Count - 1 < _nextNodeDataNum)
                    {
                        _nextNodeDataNum = -1;
                    }

                    //다음노드를 세팅후 넘겨줌
                    //다음에 실행할 노드데이터가 없으면 정지//있으면 계속진행
                    if (!GetNextNodeData(ref _nodeData, _dialogueData.nodeDataArray, _nextNodeDataNum))
                    {
                        //3. 없으면 끝
                        break;
                    }
                }              
                yield return Timing.WaitForOneFrame;
            } while (true);

            isUseDialogueSystem = false;
            if (_dialogueData.isStopTime)
            {
                TimerModuleManager.Instance.GamePause();
            }

            //활성화된 모든 초상화오브젝트 비활성화하기
            for (int i = 0; i < activePortraitUICardList.Count; i++)
            {
                activePortraitUICardList[i].OffPortraitUICard();
                activePortraitUICardList[i].transform.parent = transform;
                activePortraitUICardList[i].transform.position = transform.position;
            }
            activePortraitUICardList.Clear();
            //텍스트박스 비활성화
            dialogueBoxUIView.Hide();
            //Debug.Log("대화시스템 종료");
        }
       

        /// <summary>
        /// 대화노드 시작시 작동
        /// </summary>
        private IEnumerator<float> StartDialogue(DialogueContentData _dialogueContentData, DialogueData _dialogueData)
        {
            //폰트세팅
            if (_dialogueContentData.overrideFontAsset == null)
            {
                //기본값으로 세팅
                targetTextObject.font = dialogueDBData.defaultFontAsset;
            }
            else
            {
                //덮어쓰기
                targetTextObject.font = _dialogueContentData.overrideFontAsset;
            }

            //텍스트보내기
            //textAnimatorPlayer.textAnimator.ShowAllCharacters(false);//에러=>null 뜸
            //textAnimatorPlayer.ShowText(LocalizingManager.Instance.GetLocalLizeText(_dialogueDataID + "_" + _dialogueContentData.localizeID));
            textAnimatorPlayer.SetTypewriterSpeed(GetTypingSpeed());
            string content = LocalizingManager.Instance.GetLocalLizeText(_dialogueData.dialogueIDKey + "_" + _dialogueContentData.localizeID);
            textAnimatorPlayer.ShowText(content);
            //textAnimatorPlayer.ShowText(_dialogueContentData.dialogueContent);
            //textAnimator.SetText(LocalizingManager.Instance.GetLocalLizeText(_dialogueDataID + "_" + _dialogueContentData.localizeID), false);



            //추가오브젝트 생성체크
            GameObject focus = null;
            if (_dialogueContentData.focusObject)
            {
                focus = Instantiate(_dialogueContentData.focusObject, focusObjectPos);
            }            

            //텍스트박스 활성화//오픈
            dialogueBoxUIView.Show();

            //포트레이트 활성화
            PortraitUICard targetPortraitUICard = SearchActorPortrait(_dialogueContentData.targetActorInfoData);           

            if (targetPortraitUICard)
            {
                //활성화된 초상화에 집어넣기
                if (!activePortraitUICardList.Contains(targetPortraitUICard))
                {
                    activePortraitUICardList.Add(targetPortraitUICard);
                }

                //위치 체크
                switch (_dialogueContentData.actorShowPos)
                {
                    case ActorShowPos.Left:
                        targetPortraitUICard.transform.parent = portraitLeftPos;
                        targetPortraitUICard.transform.position = portraitLeftPos.position;
                        break;
                    case ActorShowPos.Middle:
                        targetPortraitUICard.transform.parent = portraitMiddlePos;
                        targetPortraitUICard.transform.position = portraitMiddlePos.position;
                        break;
                    case ActorShowPos.Right:
                        targetPortraitUICard.transform.parent = portraitRightPos;
                        targetPortraitUICard.transform.position = portraitRightPos.position;
                        break;
                }

                //보여주는 방법체크
                switch (_dialogueContentData.actorShowBegin)
                {
                    case ActorShowBeginType.Show:
                        targetPortraitUICard.SetShowType();
                        targetPortraitUICard.ShowPortraitUICard();
                        break;
                    case ActorShowBeginType.HalfShow:
                        targetPortraitUICard.SetShowType();
                        targetPortraitUICard.ShowPortraitUICard();
                        //투명도조절
                        break;
                }
            }

            //시간체크
            float time = Time.realtimeSinceStartup;
            timingBar.SetMaxValue((int)(time + GetAutoSkipTime()) * 10);
            timingBar.SetMinValue((int)time * 10);

            //자동스토리진행여부
            timingBar.gameObject.SetActive(isAutoStory);

            //스킵가능여부체크
            skipButton.gameObject.SetActive(_dialogueData.isSkippable);

            //로직
            do
            {
                //스토리스킵
                if (skipEvent)
                {
                    break;
                }

                //텍스트애니메이션이 끝났는가?                
                if (textAnimatorPlayer.textAnimator.allLettersShown)
                {
                    //자동 넘긴 활성화여부
                    if (isAutoStory)
                    {
                        //타이밍바관련
                        //Debug.Log("오토스킵중 =>" + GetAutoSkipTime() + time);
                        //Debug.Log(Time.realtimeSinceStartup);
                        if (Time.realtimeSinceStartup > GetAutoSkipTime() + time)
                        {
                            //시간이 되면 자동적으로 넘김
                            //루프문해제
                            Debug.Log("오토완료");                           
                            break;
                        }
                        //타이밍바관련
                        timingBar.SetCurValue((int)Time.realtimeSinceStartup * 10);
                    }

                    //수동 넘김
                    //클릭시
                    if (isUseDialogueSystem && Input.GetKeyDown(PlayerInPutKeySetting.Instance.NormalKeyBible[ECustomKeyCode.MouseLeftButton]) && !CheckOnCanvas.onUIPanel)
                    {
                        //루프문 해제
                        break;
                    }
                }
                else
                {
                    //안끝났으면
                    //클릭시
                    if (isUseDialogueSystem && Input.GetKeyDown(PlayerInPutKeySetting.Instance.NormalKeyBible[ECustomKeyCode.MouseLeftButton]) && !CheckOnCanvas.onUIPanel)
                    {
                        //스킵
                        textAnimatorPlayer.textAnimator.ShowAllCharacters(true);
                        //시간체크
                        float tempTime = Time.realtimeSinceStartup;
                        timingBar.SetMaxValue((int)(tempTime + GetAutoSkipTime()) * 10);
                        timingBar.SetMinValue((int)tempTime * 10);
                    }
                }

                yield return Timing.WaitForOneFrame;

            } while (true);

            //뒷정리
            DestroyManager.Instance.AddDestoryGameObject(focus);

            if (targetPortraitUICard)
            {
                switch (_dialogueContentData.actorShowEnd)
                {
                    case ActorShowEndType.HalfShow:
                        targetPortraitUICard.SetShowType();
                        break;
                    case ActorShowEndType.Keep:
                        //그대로둠
                        break;
                    case ActorShowEndType.Hide:
                        targetPortraitUICard.OffPortraitUICard();
                        break;
                }
            }
            timingBar.gameObject.SetActive(false);
            //Debug.Log("-=대화완룟");
        }

        /// <summary>
        /// 선택분기노드 시작시 작동
        /// </summary>
        /// <returns></returns>
        private IEnumerator<float> StartSelect(SelectBrandNodeData _selectBrandNodeData, string _dialogueDataID)
        {
            //선택사항중에 선택한게 있으면 작동끝
            //버튼 생성후 함수 집어넣기
            //활성화된 포트레이트들 절반보여주기로 변경
            for (int i = 0; i < activePortraitUICardList.Count; i++)
            {
                activePortraitUICardList[i].SetShowType();
            }            
            
            for (int i = 0; i < _selectBrandNodeData.localizeArray.Length; i++)
            {
                int index = i;
                UIButton targetObject = RequestSelectButtonPrefabObject();
                targetObject.transform.parent = selectButtonPos;
                targetObject.transform.position = selectButtonPos.position;
                targetObject.gameObject.SetActive(true);
                targetObject.Button.onClick.AddListener(delegate { SelectBtnEvent(index); });
                targetObject.SetLabelText(LocalizingManager.Instance.GetLocalLizeText(_dialogueDataID + "_" + _selectBrandNodeData.localizeArray[index]));
                
            }

            isSelectNodeClick = false;
            selectNum = 0;

            do
            {
                if (isSelectNodeClick)
                {
                    break;
                }

                yield return Timing.WaitForOneFrame;

            } while (true);

            //정리
            ClearRequestSelectButtonPrefabObject();
        }

        /// <summary>
        /// 퀘스트선택분기노드 시작시 작동
        /// </summary>
        /// <returns></returns>
        private IEnumerator<float> StartSelectQuest(SelectQuestBrandNodeData _selectQuestBrandNodeData, string _dialogueDataID)
        {
            //선택사항중에 선택한게 있으면 작동끝
            //버튼 생성후 함수 집어넣기
            //활성화된 포트레이트들 절반보여주기로 변경
            for (int i = 0; i < activePortraitUICardList.Count; i++)
            {
                activePortraitUICardList[i].SetShowType();
            }

            for (int i = 0; i < _selectQuestBrandNodeData.questBookIDKeyArray.Length; i++)
            {
                int index = i;
                UIButton targetObject = RequestSelectButtonPrefabObject();
                targetObject.transform.parent = selectButtonPos;
                targetObject.transform.position = selectButtonPos.position;
                targetObject.gameObject.SetActive(true);
                targetObject.Button.onClick.AddListener(delegate { SelectBtnEvent(index); });
                targetObject.SetLabelText(LocalizingManager.Instance.GetLocalLizeText(_dialogueDataID + "_" + _selectQuestBrandNodeData.questBookIDKeyArray[index]));
            }

            isSelectNodeClick = false;
            selectNum = 0;

            do
            {
                if (isSelectNodeClick)
                {
                    break;
                }

                yield return Timing.WaitForOneFrame;

            } while (true);

            //정리
            ClearRequestSelectButtonPrefabObject();
        }

        /// <summary>
        /// 선택분기&퀘스트분기노드 작동시 나오는 버튼에 사용하는 함수
        /// </summary>
        /// <param name="_index">번호</param>
        private void SelectBtnEvent(int _index)
        {
            isSelectNodeClick = true;
            selectNum = _index;
        }

        /// <summary>
        /// 대화데이터에서 첫번째 시작대화를 찾음
        /// </summary>        
        /// <param name="targetDialogueData">타겟이 될 대화데이터</param>
        /// <returns></returns>
        private CustomNodeData GetFirstData(DialogueData _targetDialogueData)
        {
            CustomNodeData temp = null;
            
            for (int i = 0; i < _targetDialogueData.nodeDataArray.Length; i++)
            {
                //노드데이터리스트에서 시작노드데이터 GUID를 찾음
                if (_targetDialogueData.firstContactNodeGUID == _targetDialogueData.nodeDataArray[i].nodeGUID)
                {
                    temp = _targetDialogueData.nodeDataArray[i];
                    break;
                }
            }

            return temp;
        }

        /// <summary>
        /// 다음연결된 노드 찾는 함수
        /// </summary>
        /// <param name="currentNodeData">현재노드이자 다음녿,</param>
        /// <param name="_findNodeDataList">연결된 노드리스트</param>
        /// <param name="_nextNodeDataNum">다음 타겟번호</param>
        /// <returns></returns>
        private bool GetNextNodeData(ref CustomNodeData currentNodeData, CustomNodeData[] _findNodeDataArray, int _nextNodeDataNum)
        {
            bool isNextNodeStep = false;
              
            if (_nextNodeDataNum != -1)
            {
                for (int i = 0; i < _findNodeDataArray.Length; i++)
                {
                    //int index = i;
                    //Debug.Log(index);
                    if (currentNodeData.nextNodeGUIDList[_nextNodeDataNum] == _findNodeDataArray[i].nodeGUID)
                    {
                        //다음노드 캐싱
                        currentNodeData = _findNodeDataArray[i];
                        isNextNodeStep = true;
                        break;
                    }
                }
            }
            return isNextNodeStep;
        }

        //데이터 찾기부류
        /// <summary>
        /// 대화내용 데이터 찾는 함수
        /// </summary>
        /// <param name="_nodeData">타겟이된 노드데이터</param>
        /// <param name="_dialogueData">대화데이터</param>
        /// <returns>대화내용 데이터</returns>
        private DialogueContentData FindDialogueData(CustomNodeData _nodeData, DialogueData _dialogueData)
        {
            DialogueContentData temp = null;
            for (int i = 0; i < _dialogueData.dialogueContentDataArray.Length; i++)
            {
                if (_nodeData.nodeGUID == _dialogueData.dialogueContentDataArray[i].nodeGUID)
                {
                    temp = _dialogueData.dialogueContentDataArray[i];
                    break;
                }
            }
            return temp;
        }

        private SetPropertyData FindSetPropertyData(CustomNodeData _nodeData, DialogueData _dialogueData)
        {
            SetPropertyData temp = null;

            for (int i = 0; i < _dialogueData.setPropertyDataArray.Length; i++)
            {
                if (_nodeData.nodeGUID == _dialogueData.setPropertyDataArray[i].nodeGUID)
                {
                    temp = _dialogueData.setPropertyDataArray[i];
                    break;
                }
            }

            return temp;
        }


        private ConditionBranchNodeData FindConditionBranchNodeData(CustomNodeData _nodeData, DialogueData _dialogueData)
        {
            ConditionBranchNodeData temp = null;

            for (int i = 0; i < _dialogueData.conditionBranchNodeDataArray.Length; i++)
            {
                if (_nodeData.nodeGUID == _dialogueData.conditionBranchNodeDataArray[i].nodeGUID)
                {
                    temp = _dialogueData.conditionBranchNodeDataArray[i];
                    break;
                }
            }

            return temp;
        }

        private SelectBrandNodeData FindSelectBrandNodeData(CustomNodeData _nodeData, DialogueData _dialogueData)
        {
            SelectBrandNodeData temp = null;

            for (int i = 0; i < _dialogueData.selectBrandNodeDataArray.Length; i++)
            {
                if (_nodeData.nodeGUID == _dialogueData.selectBrandNodeDataArray[i].nodeGUID)
                {
                    temp = _dialogueData.selectBrandNodeDataArray[i];
                    break;
                }
            }

            return temp;
        }

        private SelectQuestBrandNodeData FindSelectQuestBrandNodeData(CustomNodeData _nodeData, DialogueData _dialogueData)
        {
            SelectQuestBrandNodeData temp = null;

            for (int i = 0; i < _dialogueData.selectQuestBrandNodeDataArray.Length; i++)
            {
                if (_nodeData.nodeGUID == _dialogueData.selectQuestBrandNodeDataArray[i].nodeGUID)
                {
                    temp = _dialogueData.selectQuestBrandNodeDataArray[i];
                    break;
                }
            }

            return temp;
        }

        private QuestEventData FindQuestEventData(CustomNodeData _nodeData, DialogueData _dialogueData)
        {
            QuestEventData temp = null;

            for (int i = 0; i < _dialogueData.questEventDataArray.Length; i++)
            {
                if (_nodeData.nodeGUID == _dialogueData.questEventDataArray[i].nodeGUID)
                {
                    temp = _dialogueData.questEventDataArray[i];
                    break;
                }
            }

            return temp;
        }

        private LinkedDialogueData FindLinkedDialogueData(CustomNodeData _nodeData, DialogueData _dialogueData)
        {
            LinkedDialogueData temp = null;

            for (int i = 0; i < _dialogueData.linkedDialogueDataArray.Length; i++)
            {
                if (_nodeData.nodeGUID == _dialogueData.linkedDialogueDataArray[i].nodeGUID)
                {
                    temp = _dialogueData.linkedDialogueDataArray[i];
                    break;
                }
            }

            return temp;
        }
        private CustomPropertyBool FindCustomPropertyBool(DialogueData _dialogueData, string _propertyName)
        {
            //데이터로그쪽에서 가져옴
            CustomPropertyBool temp = null;
            //for (int i = 0; i < _dialogueData.customPropertyBoolArray.Length; i++)
            //{
            //    if (_dialogueData.customPropertyBoolArray[i].propertyName == _propertyName)
            //    {
            //        temp = _dialogueData.customPropertyBoolArray[i];
            //        break;
            //    }
            //}

            DialogueInterectLogData logData = dialogueLogBible[_dialogueData.dialogueIDKey];
            for (int i = 0; i < logData.customPropertyBoolArray.Length; i++)
            {
                if (logData.customPropertyBoolArray[i].propertyName == _propertyName)
                {
                    temp = logData.customPropertyBoolArray[i];
                    break;
                }
            }
            return temp;
        }

        private CustomPropertyFloat FindCustomPropertyFloat(DialogueData _dialogueData, string _propertyName)
        {
            //데이터로그쪽에서 가져옴
            CustomPropertyFloat temp = null;
            //for (int i = 0; i < _dialogueData.customPropertyFloatArray.Length; i++)
            //{
            //    if (_dialogueData.customPropertyFloatArray[i].propertyName == _propertyName)
            //    {
            //        temp = _dialogueData.customPropertyFloatArray[i];
            //        break;
            //    }
            //}

            DialogueInterectLogData logData = dialogueLogBible[_dialogueData.dialogueIDKey];
            for (int i = 0; i < logData.customPropertyFloatArray.Length; i++)
            {
                if (logData.customPropertyFloatArray[i].propertyName == _propertyName)
                {
                    temp = logData.customPropertyFloatArray[i];
                    break;
                }
            }
            return temp;
        }

        private CustomPropertyInt FindCustomPropertyInt(DialogueData _dialogueData, string _propertyName)
        {
            //데이터로그쪽에서 가져옴
            CustomPropertyInt temp = null;
            //for (int i = 0; i < _dialogueData.customPropertyIntArray.Length; i++)
            //{
            //    if (_dialogueData.customPropertyIntArray[i].propertyName == _propertyName)
            //    {
            //        temp = _dialogueData.customPropertyIntArray[i];
            //        break;
            //    }
            //}

            DialogueInterectLogData logData = dialogueLogBible[_dialogueData.dialogueIDKey];
            for (int i = 0; i < logData.customPropertyIntArray.Length; i++)
            {
                if (logData.customPropertyIntArray[i].propertyName == _propertyName)
                {
                    temp = logData.customPropertyIntArray[i];
                    break;
                }
            }
            return temp;
        }

        private CustomPropertyString FindCustomPropertyString(DialogueData _dialogueData, string _propertyName)
        {
            //데이터로그쪽에서 가져옴
            CustomPropertyString temp = null;
            //for (int i = 0; i < _dialogueData.customPropertyStringArray.Length; i++)
            //{
            //    if (_dialogueData.customPropertyStringArray[i].propertyName == _propertyName)
            //    {
            //        temp = _dialogueData.customPropertyStringArray[i];
            //        break;
            //    }
            //}

            DialogueInterectLogData logData = dialogueLogBible[_dialogueData.dialogueIDKey];
            for (int i = 0; i < logData.customPropertyStringArray.Length; i++)
            {
                if (logData.customPropertyStringArray[i].propertyName == _propertyName)
                {
                    temp = logData.customPropertyStringArray[i];
                    break;
                }
            }
            return temp;
        }

        /// <summary>
        /// 조건분기 연산자에서 float int 변수를 위한 함수
        /// </summary>
        /// <param name="_comparisonOperatorType">비교할 연산자</param>
        /// <param name="propertyValue">전역변수 값</param>
        /// <param name="conditionValue">조건분기 값</param>
        /// <returns>맞는가</returns>
        private bool CheckOperator(ComparisonOperatorType _comparisonOperatorType, float propertyValue, float conditionValue)
        {
            bool isTrue = false;
            switch (_comparisonOperatorType)
            {
                case ComparisonOperatorType.Equal:
                    if (propertyValue == conditionValue)
                    {
                        isTrue = true;
                    }
                    break;
                case ComparisonOperatorType.Less:
                    if (propertyValue < conditionValue)
                    {
                        isTrue = true;
                    }
                    break;
                case ComparisonOperatorType.Greater:
                    if (propertyValue > conditionValue)
                    {
                        isTrue = true;
                    }
                    break;
            }

            return isTrue;
        }

        /// <summary>
        /// 인물데이터를 검색후 찾아서 초상화를 주는 함수
        /// </summary>
        /// <param name="actorInfoData">액터(인물)데이터</param>
        /// <returns>초상화오브젝트</returns>
        private PortraitUICard SearchActorPortrait(ActorInfoObjectScript _actorInfoData)
        {
            PortraitUICard portraitUICard = null;

            for (int i = 0; i < portraitUICardArray.Length; i++)
            {
                if (_actorInfoData == portraitUICardArray[i].actorInfoData)
                {
                    portraitUICard = portraitUICardArray[i];
                    break;
                }
            }
            return portraitUICard;
        }

        public void SetIsUseDialogueSystem(bool _onOff)
        {
            isUseDialogueSystem = _onOff;
        }

        public bool GetIsUseDialogueSystem()
        {
            return isUseDialogueSystem;
        }

        private UIButton RequestSelectButtonPrefabObject()
        {
            return uIButtonObjectPool.RequestPrefab();
        }

        private void ClearRequestSelectButtonPrefabObject()
        {
            uIButtonObjectPool.AllObjectDeActive();
        }

        //오토스킵 스킵속도 텍스트타이핑속도 관련 기능

        /// <summary>
        /// 오토스킵을 변경
        /// </summary>
        private void ChangeAutoTime()
        {
            isAutoStory = !isAutoStory;
            autoStoryOnOffButton.SetLabelText(isAutoStory.ToString());
            if (isAutoStory)
            {
                timingBar.gameObject.SetActive(true);
            }
            else
            {
                timingBar.gameObject.SetActive(false);
            }
        }

        private void ChangeAutoSkipSpeed()
        {
            switch (autoSkipSpeed) 
            {
                case SpeedType.Normal:
                    autoSkipSpeed = SpeedType.Slow;
                    break;
                case SpeedType.Slow:
                    autoSkipSpeed = SpeedType.Fast;
                    break;
                case SpeedType.Fast:
                    autoSkipSpeed = SpeedType.Normal;
                    break;
            }
            float time = Time.realtimeSinceStartup;
            timingBar.InitUIBar((int)time * 10, (int)(GetAutoSkipTime() + time) * 10, (int)Time.realtimeSinceStartup * 10);
        }

        private void ChangeTypingSpeed()
        {   
            switch (typingSpeed)
            {
                case SpeedType.Normal:
                    typingSpeed = SpeedType.Fast;
                    break;
                case SpeedType.Slow:
                    typingSpeed = SpeedType.Normal;
                    break;
                case SpeedType.Fast:
                    typingSpeed = SpeedType.Slow;
                    break;
            }
            textAnimatorPlayer.SetTypewriterSpeed(GetTypingSpeed());
        }

        private void ChangeSkipEvent()
        {
            skipEvent = !skipEvent;
        }

        /// <summary>
        /// 타이핑 속도를 가져오는 함수
        /// </summary>       
        /// <returns>값</returns>
        public float GetTypingSpeed()
        {
            float _speed = 1;
            switch (typingSpeed)
            {
                case SpeedType.Slow:
                    //_speed = 0.5f;
                    _speed = 0.01f;
                    break;
                case SpeedType.Normal:
                    _speed = 1f;
                    break;
                case SpeedType.Fast:
                    //_speed = 2f;
                    _speed = 20f;
                    break;
            }
            return _speed;
        }

        /// <summary>
        /// 오토스킵속도를 가져오는 함수
        /// </summary>        
        /// <returns>값</returns>
        private float GetAutoSkipTime()
        {
            float _speed = 1;
            switch (autoSkipSpeed)
            {
                case SpeedType.Slow:
                    _speed = 10f;
                    break;
                case SpeedType.Normal:
                    _speed = 6f;
                    break;
                case SpeedType.Fast:
                    _speed = 2f;
                    break;
            }
            return _speed;
        }
    }

    public enum SpeedType
    {
        Normal,
        Slow,
        Fast,        
    }
    public enum TypingAnimationType
    {
        Normal,//전체 그냥 보여주기
        LetterStep,//한글자씩 보여주기        
        TextLetterStepAndWordStep,//한개씩 보여주기와 단어식 보여주기 합친것
        WordStep,//단어씩 보여주기       
    }
    public enum ActorShowBeginType
    {
        //대화노드 시작시 보여주는 타입
        Show,//보여준다
        HalfShow,//투명도를 절반만 보여준다                
    }
    public enum ActorShowEndType
    {
        //대화노드 끝난뒤 보여주는 타입
        Keep,//유지시킨다.
        HalfShow,//투명도를 절반만 보여준다        
        Hide,//숨긴다
    }

    public enum ActorShowPos
    {
        //대화노드 시작시 어디 위치에 출현할지
        Left,
        Middle,
        Right,
    }
    
    /// <summary>
    /// 대화시스템 이벤트 인터페이스, 대화에 있는 이벤트를 활용하여 대화를 잠시 멈출수 있게함
    /// </summary>
    public interface IDialogueSystemEvent
    {
        /// <summary>
        /// 대화대상 월드오브젝트 세팅
        /// </summary>
        /// <param name="dialogueTargetWorldObject">대화대상</param>
        void SetDialogueTargetWorldObject(DialogueObjectTarget _dialogueTargetWorldObject);

        /// <summary>
        /// 특정로직이 발동후 대화대상의 EventDone을 작동시켜줘야됨
        /// </summary>
        void TriggerDialogueEvent();
    }
}
#endif
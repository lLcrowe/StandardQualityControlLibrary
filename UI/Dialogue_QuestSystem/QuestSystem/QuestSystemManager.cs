#if Doozy

using System.Collections.Generic;
using UnityEngine;
using Micosmo.SensorToolkit;
using lLCroweTool.LogSystem;
using lLCroweTool.Dictionary;
using lLCroweTool.TimerSystem;
using lLCroweTool.GameWorldTimeSystem;
using lLCroweTool.WorldObjectSystem;
using lLCroweTool.InventorySystem;
using lLCroweTool.TreeUI;
using lLCroweTool.DialogueSystem;

namespace lLCroweTool.QuestSystem
{
    public class QuestSystemManager : MonoBehaviour
    {
        private static QuestSystemManager instance;
        public static QuestSystemManager Instance
        {
            get
            {
                if (ReferenceEquals(instance, null))
                {
                    instance = FindObjectOfType<QuestSystemManager>();
                    //if (ReferenceEquals(instance, null))
                    if (ReferenceEquals(instance, null))
                    {
                        GameObject gameObject = new GameObject();
                        instance = gameObject.AddComponent<QuestSystemManager>();
                        gameObject.name = "-=QuestSystemManager=-";

                        // ReSharper disable once ArrangeStaticMemberQualifier
                        //_instance = (MasterAudio)GameObject.FindObjectOfType(typeof(MasterAudio));
                        //return _instance;
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 퀘스트시스템 DB
        /// </summary>
        public QuestDBObjectScript questDBData;

        //퀘스트 선택창//리퀘스트게시판
        [Header("퀘스트일지창")]
        public QuestDiaryViewUI questDiaryViewUI;

        [Header("경험치&스킬포인트 설명데이터")]
        public ObjectDeScription_Base unitExprienceDeScriptionData;
        public ObjectDeScription_Base skillPointDeScriptionData;//체크후 Private

        //퀘스트 타입에 따른 이미지//메인 서브 특별 등등
        [Header("퀘스트타입 스프라이트이미지")]
        public Sprite mainQuestMarkSprite;
        public Sprite subQuestMarkSprite;
        public Sprite specialQuestMarkSprite;

        //-------퀘스트표시 HUD Text & 퀘스트실시간 표시용 UI & 퀘스트 타이머-------
        //리퀘스트용
        [Header("리퀘스트UI")]
        public RequestUI targetRequestUIPrefab;
        public List<RequestUI> targetRequestUIPrefabList = new List<RequestUI>();//체크후 Private

        //리퀘스트의 미션체크용
        [Header("미션체커UI")]
        public QuestMissionCheckerUI targetMissionCheckerUIPrefab;
        public List<QuestMissionCheckerUI> targetMissionCheckerUIPrefabList = new List<QuestMissionCheckerUI>();//체크후 Private        

        //퀘스트 보상창//보상선택을 위한
        [Header("퀘스트보상창")]
        public RewardSelectViewUI questRewardView;

        //퀘스트보상선택버튼용
        [Header("퀘스트보상 선택버튼")]
        public RewardSelectButton targetQuestRewardBtnPrefab;
        public List<RewardSelectButton> targetQuestRewardBtnPrefabList = new List<RewardSelectButton>();//체크후 Private
        private bool isFind = false;

        //시작시 코드로 미리 세팅해주는구역
        [System.Serializable] public class QuestBookDataBible : CustomDictionary<string, QuestBookData> { }
        [System.Serializable] public class CustomQuestNodeDataBible : CustomDictionary<string, CustomQuestNodeData> { }
        [System.Serializable] public class QuestNodeDataBible : CustomDictionary<string, QuestNodeData> { }
        [System.Serializable] public class DialogueEventNodeDataBible : CustomDictionary<string, DialogueEventNodeData> { }
        [System.Serializable] public class LinkedQuestBookNodeDataBible : CustomDictionary<string, LinkedQuestBookNodeData> { }
        [System.Serializable] public class RequestBible : CustomDictionary<QuestNodeData, Request> { }
        [System.Serializable] public class SceneQuestRewardGiverBible : CustomDictionary<ActorInfoObjectScript, QuestRewardGiver> { }

        /// <summary>
        /// 퀘스트북키, 퀘스트북 데이터
        /// </summary>
        [SerializeField] private QuestBookDataBible questBookDataBible = new QuestBookDataBible();
        
        /// <summary>
        /// GUID, 해당되는 커스텀퀘스트노드데이터
        /// </summary>
        [SerializeField] private CustomQuestNodeDataBible customQuestNodeDataBible = new CustomQuestNodeDataBible();
        
        /// <summary>
        /// GUID, 해당되는 퀘스트노드데이터
        /// </summary>
        [SerializeField] private QuestNodeDataBible questNodeDataBible = new QuestNodeDataBible();
      
        /// <summary>
        /// GUID, 해당되는 대화이벤트노드데이터
        /// </summary>
        [SerializeField] private DialogueEventNodeDataBible dialogueEventNodeDataBible = new DialogueEventNodeDataBible();
       
        /// <summary>
        /// GUID, 해당되는 링크퀘스트노드데이터
        /// </summary>
        [SerializeField] private LinkedQuestBookNodeDataBible linkedQuestBookNodeDataBible = new LinkedQuestBookNodeDataBible();

        /// <summary>
        /// 현재작동되는 리퀘스트들
        /// </summary>
        [SerializeField] private List<Request> activeRequestList = new List<Request>();
       
        /// <summary>
        /// 현재 호출된 모든 리퀘스트들 (없어지지않음)
        /// </summary>
        [SerializeField] private RequestBible requestBible = new RequestBible();
        
        //씬이 변할때마다 현재 씬에 있는 퀘스트기버들을 체크하여 캐싱해놔야할듯하다
        /// <summary>
        /// 씬에 있는 퀘스트보상 기버들
        /// </summary>
        [SerializeField] private SceneQuestRewardGiverBible sceneQuestRewardGiverBible = new SceneQuestRewardGiverBible();

        //캐싱
        private CoroutineTimerModule timerModule;
        private WorldTime zeroTime;//0시를 향함
        [System.Serializable] public class ClearQuestQueue : QueueEventModule<QuestBookData> { }
        public ClearQuestQueue clearQuestBookQueue = new ClearQuestQueue();//완료된 퀘스트를 위한 큐이벤트        

        private void Awake()
        {   
            instance = this;
            timerModule = GetComponent<CoroutineTimerModule>();
            timerModule.SetTimer(1.0f);
            timerModule.AddUnityEvent(UpdateRequest);

            clearQuestBookQueue.SetTimer(1.0f);

            zeroTime = new WorldTime();
            zeroTime.SetWorldTime(0);

            for (int i = 0; i < questDBData.questBookDataArray.Length; i++)
            {
                //퀘스트북키등록
                questBookDataBible.Add(questDBData.questBookDataArray[i].questBookIDKey, questDBData.questBookDataArray[i]);

                //커스텀퀘스트노드 등록
                for (int j = 0; j < questDBData.questBookDataArray[i].nodeDataArray.Length; j++)
                {
                    customQuestNodeDataBible.Add(questDBData.questBookDataArray[i].nodeDataArray[j].nodeGUID, questDBData.questBookDataArray[i].nodeDataArray[j]);
                }
                //퀘스트노드
                for (int j = 0; j < questDBData.questBookDataArray[i].questNodeDataArray.Length; j++)
                {
                    questNodeDataBible.Add(questDBData.questBookDataArray[i].questNodeDataArray[j].nodeGUID, questDBData.questBookDataArray[i].questNodeDataArray[j]);
                }
                //대화이벤트
                for (int j = 0; j < questDBData.questBookDataArray[i].dialogueEventNodeDataArray.Length; j++)
                {
                    dialogueEventNodeDataBible.Add(questDBData.questBookDataArray[i].dialogueEventNodeDataArray[j].nodeGUID, questDBData.questBookDataArray[i].dialogueEventNodeDataArray[j]);
                }
                //퀘스트링크
                for (int j = 0; j < questDBData.questBookDataArray[i].linkedQuestBookNodeDataArray.Length; j++)
                {
                    linkedQuestBookNodeDataBible.Add(questDBData.questBookDataArray[i].linkedQuestBookNodeDataArray[j].nodeGUID, questDBData.questBookDataArray[i].linkedQuestBookNodeDataArray[j]);
                }
            }

            var temp = typeof(QuestSystemManager);
            LogManager.Register(temp, temp.Name, true, true);
        }

        /// <summary>
        /// 퀘스트를 발행해주는 함수
        /// </summary>
        /// <param name="questBookIDKey">퀘스트북ID</param>
        public void RequestQuestBook(string questBookIDKey)
        {
            //해당 퀘스트북키가 있는지
            if (questBookDataBible.ContainsKey(questBookIDKey))
            {
                //존재하면 시작GUID를 사용하여 첫번쨰시작노드를 가져옴
                string target = questBookDataBible[questBookIDKey].firstContactNodeGUID;
                if (customQuestNodeDataBible.ContainsKey(target))
                {
                    bool check = false;
                    //진행중인 퀘스트북인지 체크
                    for (int i = 0; i < activeRequestList.Count; i++)
                    {
                        if (activeRequestList[i].targetQuestBookIDKey == questBookIDKey)
                        {
                            check = true;
                            break;
                        }
                    }

                    if (!check)
                    {
                        //첫번째 노드 작동
                        CustomQuestNodeData targetQuestnode = customQuestNodeDataBible[target];
                        ActionQuestNode(questBookIDKey, targetQuestnode);
                    }
                    else
                    {
                        Debug.Log("작동되는 같은 퀘스트북키가 있다.");
                    }
                }
            }
            else
            {
                Debug.Log("시작다음 퀘스트북데이터가 없습니다.");
            }
        }

        /// <summary>
        /// 퀘스트노드데이터를 참조하여 동작하는 함수
        /// </summary>
        /// <param name="customQuestNodeData">퀘스트노드데이터</param>
        private void ActionQuestNode(string questBookIDKey, CustomQuestNodeData customQuestNodeData)
        {
            switch (customQuestNodeData.nodeType)
            {
                case QuestNodeType.QuestNode:
                    QuestNodeData questNodeData = questNodeDataBible[customQuestNodeData.nodeGUID];

                    //해당되는 리퀘스트가 있는지 체크후 새로제작
                    Request request = null;
                    if (requestBible.ContainsKey(questNodeData))
                    {
                        request = requestBible[questNodeData];
                    }
                    else
                    {
                        request = new Request();
                        requestBible.Add(questNodeData, request);
                    }
                    request.InitQuestData(questBookIDKey, questNodeData);
                    request.ActiveRequest();
                    activeRequestList.Add(request);
                    break;
                case QuestNodeType.DialogueEvent:
                    //대화시스템에 해당대화를 요청
                    DialogueEventNodeData dialogueEventNodeData = dialogueEventNodeDataBible[customQuestNodeData.nodeGUID];
                    DialogueSystemManager.Instance.RequestDialogueData(dialogueEventNodeData.dialogueID);
                    break;
                case QuestNodeType.LinkedQuestBook:
                    //퀘스트시스템에 해당퀘스트를 요청
                    LinkedQuestBookNodeData linkedQuestBookNodeData = linkedQuestBookNodeDataBible[customQuestNodeData.nodeGUID];
                    RequestQuestBook(linkedQuestBookNodeData.questBookID);
                    break;
                case QuestNodeType.RandomBranch:
                    //랜덤노드
                    //다음노드만 설정
                    int temp = customQuestNodeData.nextNodeGUIDList.Count;
                    int nextNodeDataNum = -1;
                    if (temp > 1)
                    {
                        nextNodeDataNum = Random.Range(0, temp);
                    }
                    else if (temp == 1)
                    {
                        nextNodeDataNum = 0;
                    }

                    if (nextNodeDataNum != -1)
                    {
                        CustomQuestNodeData tempNode = customQuestNodeDataBible[customQuestNodeData.nextNodeGUIDList[nextNodeDataNum]];
                        ActionQuestNode(questBookIDKey, tempNode);
                    }
                    break;
            }
        }

        /// <summary>
        /// 퀘스트 다음내역처리
        /// </summary>
        /// <param name="isSuccess">성공여부</param>
        ///<param name="request">타겟이된 리퀘스트</param>
        public void QuestNextSequence(bool isSuccess, Request request)
        {
            CustomQuestNodeData targetQuestNodeData = customQuestNodeDataBible[request.targetQuestNodeData.nodeGUID];
            int selectNum = isSuccess ? 0 : 1;//선택

            targetQuestNodeData = customQuestNodeDataBible[targetQuestNodeData.nextNodeGUIDList[selectNum]];//다음 노드데이터선택

            //다음퀘스트노드 데이터체크
            if (targetQuestNodeData == null)
            {
                QuestBookData questBookData = questBookDataBible[request.targetQuestBookIDKey];
                //없으면 성공여부에 따라 보상을 줌
                if (isSuccess)
                {
                    //성공처리                    
                    request.CheckFinal(true);
                    clearQuestBookQueue.Enqueue(questBookData);
                }
                else
                {
                    //실패처리
                    request.CheckFinal(false);
                    if (questBookData.questType == QuestMissionCATType.Main)
                    {
                        LogManager.Log(typeof(QuestSystemManager), "메인퀘스트실패", gameObject, LogManager.LogType.Info);
                        //게임월드이벤트로 이벤트보내서 실패처리
                        GameWorldTimeManager.Instance.RequestWolrdEvent("MainQuest_Fail");
                    }
                }
                //작동되는 리퀘스트에서 빼버림
                activeRequestList.Remove(request);
            }
            else
            {
                //있으면 작동
                ActionQuestNode(request.targetQuestBookIDKey, targetQuestNodeData);
            }
        }

        /// <summary>
        /// 작동되고 있는 리퀘스트 업데이트
        /// </summary>
        private void UpdateRequest()
        {
            for (int i = 0; i < activeRequestList.Count; i++)
            {

                //타이머갱신
                if (activeRequestList[i].targetQuestNodeData.isUseTimer)
                {
                    if (Time.time > activeRequestList[i].GetTime() + 1)
                    {
                        activeRequestList[i].worldTime.SetWorldTime(--activeRequestList[i].timeValue);
                        activeRequestList[i].SetTime(Time.time);

                        //시간체크
                        if (activeRequestList[i].worldTime.CheckEqualTime(zeroTime))
                        {
                            //시간안에 성공을 못했으면
                            if (!activeRequestList[i].CheckSuccessReQuest())
                            {
                                //실패처리
                                QuestNextSequence(false, activeRequestList[i]);
                            }
                        }
                        else
                        {
                            //미션실패여부
                            if (activeRequestList[i].CheckFailedReQuest())
                            {
                                //실패처리
                                QuestNextSequence(false, activeRequestList[i]);
                            }
                            else if (activeRequestList[i].CheckSuccessReQuest())
                            {
                                //성공처리
                                QuestNextSequence(true, activeRequestList[i]);
                            }
                        }
                    }
                }
                else
                {
                    //미션실패여부
                    if (activeRequestList[i].CheckFailedReQuest())
                    {
                        //실패처리
                        QuestNextSequence(false, activeRequestList[i]);
                    }
                    else if (activeRequestList[i].CheckSuccessReQuest())
                    {
                        //성공처리
                        QuestNextSequence(true, activeRequestList[i]);
                    }
                }
            }

            //성공한 퀘스트들 체크 후 보상처리
            if (clearQuestBookQueue.CheckActionTime())
            {
                //여기 트리거한개 안만들면 문제생길거 같다//20220207

                QuestBookData questBookData = clearQuestBookQueue.Dequeue();
                RewardSelectButton rewardSelectButton = null;
                switch (questBookData.questRewardGiveType)
                {
                    case QuestRewardGiveType.All:
                        //확인창 있음
                        //선택창 없음
                        questRewardView.ShowQuestRewardViewUI();
                        questRewardView.confirmWindow.SetConfirmWindow(LocalizingManager.Instance.GetLocalLizeText("RewardSelect"), delegate 
                        {
                            for (int i = 0; i < questBookData.questBookRewardDataArray.Length; i++)
                            {
                                GiveQuestReward(questBookData.questBookRewardDataArray[i], GetRewardGiver(questBookData.actorInfoData));
                            }
                        }, true);
                        questRewardView.confirmWindow.ShowConfirmWindowUIView();

                        for (int i = 0; i < questBookData.questBookRewardDataArray.Length; i++)
                        {
                            rewardSelectButton = RequestRewardSelectButton();
                            rewardSelectButton.transform.SetParent(questRewardView.horizontalLayout.transform);
                            rewardSelectButton.SetQuestRewardButton(questBookData.questBookRewardDataArray[i], false, "", delegate{});
                        }

                        break;
                    case QuestRewardGiveType.Select:
                        //확인창 없음
                        //선택창 있음
                        questRewardView.ShowQuestRewardViewUI();

                        for (int i = 0; i < questBookData.questBookRewardDataArray.Length; i++)
                        {
                            rewardSelectButton = RequestRewardSelectButton();
                            rewardSelectButton.transform.SetParent(questRewardView.horizontalLayout.transform);
                            rewardSelectButton.SetQuestRewardButton(questBookData.questBookRewardDataArray[i], true, LocalizingManager.Instance.GetLocalLizeText("RewardSelect"), delegate
                              {
                                  GiveQuestReward(questBookData.questBookRewardDataArray[i], GetRewardGiver(questBookData.actorInfoData));
                                  rewardSelectButton.gameObject.SetActive(false);
                                  rewardSelectButton.transform.SetParent(transform);
                                  questRewardView.OffQuestRewardViewUI();
                              });
                        }
                        break;
                    case QuestRewardGiveType.Random:
                        //확인창 있음
                        //선택창 없음
                        //보상선택

                        int index = Random.Range(0, questBookData.questBookRewardDataArray.Length);
                        RewardData rewardData = questBookData.questBookRewardDataArray[index];

                        questRewardView.ShowQuestRewardViewUI();
                        questRewardView.confirmWindow.SetConfirmWindow(LocalizingManager.Instance.GetLocalLizeText("RewardSelect"), delegate
                        {
                            GiveQuestReward(rewardData, GetRewardGiver(questBookData.actorInfoData));
                        }, true);
                        questRewardView.confirmWindow.ShowConfirmWindowUIView();

                        rewardSelectButton = RequestRewardSelectButton();
                        rewardSelectButton.transform.SetParent(questRewardView.horizontalLayout.transform);
                        rewardSelectButton.SetQuestRewardButton(rewardData, false, "", delegate { });
                        break;
                }
            }
        }

        /// <summary>
        ///리퀘스트미션업데이트 함수
        /// </summary>
        /// <param name="questBookID">퀘스트북ID</param>
        /// <param name="questName">퀘스트이름</param>
        /// <param name="missionID">미션ID</param>
        public void UpdateReQuestMission(string questBookID, string questName, string missionID)
        {
            for (int i = 0; i < activeRequestList.Count; i++)
            {
                if (activeRequestList[i].GetTargetQuestBookIDKey() == questBookID)
                {
                    if (activeRequestList[i].targetQuestNodeData.questName == questName)
                    {
                        activeRequestList[i].UpdateQuestData(missionID);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 유인이동미션데이터 가져오기
        /// </summary>
        /// <param name="questBookID">퀘스트북키</param>
        /// <param name="questName">퀘스트이름</param>
        /// <param name="missionID">미션아이디</param>
        /// <returns>유인이동 미션데이터</returns>
        public AttractMoveMissionData GetAttractMoveMissionData(string questBookID, string questName, string missionID)
        {
            //좀느리긴한데 캐싱해놓을때 사용하면 됨
            QuestBookData questBookData = questBookDataBible[questBookID];
            QuestNodeData questNodeData = null;
            for (int i = 0; i < questBookData.questNodeDataArray.Length; i++)
            {
                if (questBookData.questNodeDataArray[i].questName == questName)
                {
                    questNodeData = questBookData.questNodeDataArray[i];
                    break;
                }
            }

            AttractMoveMissionData temp = null;
            for (int i = 0; i < questNodeData.attractMoveMissionDataArray.Length; i++)
            {
                AttractMoveMissionData attractMoveMissionData = questNodeData.attractMoveMissionDataArray[i];
                string _missionID;
                if (attractMoveMissionData.isUseTargetUnitStat)
                {
                    if (attractMoveMissionData.unitStatusData == null)
                    {
                        _missionID = attractMoveMissionData.unitTeamType.ToString() + "_unitStatDataIsNull";
                    }
                    else
                    {
                        _missionID = attractMoveMissionData.unitTeamType.ToString() + "_" + attractMoveMissionData.unitStatusData.name;
                    }
                }
                else
                {
                    _missionID = attractMoveMissionData.unitTeamType.ToString();
                }

                //미션아이디가 동일한지 체크
                if (_missionID == missionID)
                {
                    temp = attractMoveMissionData;
                    break;
                }
            }
            return temp;
        }

        /// <summary>
        /// 퀘스트북데이터를 가져오는 함수
        /// </summary>
        /// <param name="questBookID">퀘스트북아이디</param>
        /// <returns>퀘스트북데이터</returns>
        public QuestBookData GetQuestBookData(string questBookID)
        {
            QuestBookData questBookData = null;
            if (questBookDataBible.ContainsKey(questBookID))
            {
                questBookData = questBookDataBible[questBookID];
            }
            return questBookData;
        }

        /// <summary>
        /// 퀘스트 기버가 사용하는 함수
        /// </summary>
        /// <param name="gameObject">탐지된 오브젝트</param>
        /// <param name="sensor">센서</param>
        /// <param name="unitTeamType">유닛의 팀타입</param>
        /// <param name="isOnce">한번만 작동되는지</param>
        /// <returns></returns>
        public bool CheckDetectWorldUnitObject(GameObject gameObject, Sensor sensor, UnitTeamType unitTeamType, bool isOnce)
        {
            bool isDetect = false;
            //유닛오브젝트가 있으며 해당유닛이 플레이어팀이면 퀘스트를 발동
            //그후에 작동이 안되게 센서를 꺼버림
            if (gameObject.TryGetComponent(out TestWorldUnitObject unitObject))
            {
                if (unitObject.unitTeamType == unitTeamType)
                {
                    isDetect = true;
                    if (isOnce)
                    {
                        sensor.enabled = false;
                    }
                }
            }
            return isDetect;
        }

        /// <summary>
        /// 퀘스트보상주는 함수
        /// </summary>
        /// <param name="rewardData">보상데이터</param>
        /// <param name="pos">나올 위치</param>
        private void GiveQuestReward(RewardData rewardData, Transform pos)
        {
            //캐싱용
            ResearchTreeStore _researchTreeStore = null;
            TestWorldUnitObject testWorldUnitObject = null;
            TestWorldUnitObject[] testWorldUnitObjectArray = null;

            //아이템데이터
            //처리방법=> 아이템을 액터근처에 놓아줌
            if (rewardData.itemData != null)
            {
                InventoryManager.Instance.CreateWorldItemForWorldMap(rewardData.itemData, rewardData.itemCount, pos, pos, pos.up);
            }

            //유닛오브젝트
            //처리방법=> 유닛을 액터근처에 놓아줌
            if (rewardData.targetUnitObject != null)
            {
                for (int i = 0; i < rewardData.unitCount; i++)
                {
                    testWorldUnitObject = Instantiate(rewardData.targetUnitObject, pos);
                    testWorldUnitObject.transform.position = InventoryManager.Instance.GetRandomSpawnPosition(false, pos, 3);
                }
            }

            //경험치
            //처리방법=>플레이어유닛으로 등록된 모든 유닛에게 경험치를 집어넣어줌
            if (rewardData.unitExprience != 0)
            {
                switch (rewardData.unitExprienceGiveType)
                {
                    case GiveType.AllUnit:
                        testWorldUnitObject = PlayerUnitManager.Instance.GetPlayerControlUnit();
                        if (testWorldUnitObject != null)
                        {
                            if (testWorldUnitObject.TryGetComponent(out _researchTreeStore))
                            {
                                _researchTreeStore.AddExperienceValue(rewardData.unitExprience);
                            }
                        }

                        testWorldUnitObjectArray = PlayerUnitManager.Instance.GetUnitObjectArray();
                        for (int i = 0; i < testWorldUnitObjectArray.Length; i++)
                        {
                            if (testWorldUnitObjectArray[i].TryGetComponent(out _researchTreeStore))
                            {
                                _researchTreeStore.AddExperienceValue(rewardData.unitExprience);
                            }
                        }
                        break;
                    case GiveType.PlayerUnit:
                        testWorldUnitObject = PlayerUnitManager.Instance.GetPlayerControlUnit();
                        if (testWorldUnitObject != null)
                        {
                            if (testWorldUnitObject.TryGetComponent(out _researchTreeStore))
                            {
                                _researchTreeStore.AddExperienceValue(rewardData.unitExprience);
                            }
                        }
                        break;
                    case GiveType.PartnerUnit:
                        testWorldUnitObjectArray = PlayerUnitManager.Instance.GetUnitObjectArray();
                        for (int i = 0; i < testWorldUnitObjectArray.Length; i++)
                        {
                            if (testWorldUnitObjectArray[i].TryGetComponent(out _researchTreeStore))
                            {
                                _researchTreeStore.AddExperienceValue(rewardData.unitExprience);
                            }
                        }
                        break;                   
                }

              
            }

            //스킬포인트
            //처리방법=>플레이어유닛으로 등록된 모든 유닛에게 스킬포인트를 집어넣어줌or 플레이어조종유닛에게만 집어넣기
            if (rewardData.skillPoint != 0)
            {
                switch (rewardData.skillPointGiveType)
                {
                    case GiveType.AllUnit:
                        testWorldUnitObject = PlayerUnitManager.Instance.GetPlayerControlUnit();
                        if (testWorldUnitObject != null)
                        {
                            if (testWorldUnitObject.TryGetComponent(out _researchTreeStore))
                            {
                                _researchTreeStore.SetCost(_researchTreeStore.GetCost() + rewardData.skillPoint);
                            }
                        }

                        testWorldUnitObjectArray = PlayerUnitManager.Instance.GetUnitObjectArray();
                        for (int i = 0; i < testWorldUnitObjectArray.Length; i++)
                        {
                            if (testWorldUnitObjectArray[i].TryGetComponent(out _researchTreeStore))
                            {
                                _researchTreeStore.SetCost(_researchTreeStore.GetCost() + rewardData.skillPoint);
                            }
                        }
                        break;
                    case GiveType.PlayerUnit:
                        testWorldUnitObject = PlayerUnitManager.Instance.GetPlayerControlUnit();
                        if (testWorldUnitObject != null)
                        {
                            if (testWorldUnitObject.TryGetComponent(out _researchTreeStore))
                            {
                                _researchTreeStore.SetCost(_researchTreeStore.GetCost() + rewardData.skillPoint);
                            }
                        }
                        break;
                    case GiveType.PartnerUnit:
                        testWorldUnitObjectArray = PlayerUnitManager.Instance.GetUnitObjectArray();
                        for (int i = 0; i < testWorldUnitObjectArray.Length; i++)
                        {
                            if (testWorldUnitObjectArray[i].TryGetComponent(out _researchTreeStore))
                            {
                                _researchTreeStore.SetCost(_researchTreeStore.GetCost() + rewardData.skillPoint);
                            }
                        }
                        break;
                }
              
            }

            //스킬데이터
            //처리방법=>플레이어유닛으로 등록된 모든 유닛에게 스킬포인트를 집어넣어줌or 플레이어조종유닛에게만 집어넣기
            if (rewardData.skillData != null)
            {
                switch (rewardData.skillDataGiveType)
                {
                    case GiveType.AllUnit:
                        testWorldUnitObject = PlayerUnitManager.Instance.GetPlayerControlUnit();
                        if (testWorldUnitObject != null)
                        {
                            if (testWorldUnitObject.TryGetComponent(out _researchTreeStore))
                            {
                                _researchTreeStore.CheckUnLockSkillData(rewardData.skillData, out bool isExist, out bool isLock);
                                if (isExist && !isLock)
                                {
                                    //해당되는 스킬데이터 변경
                                    _researchTreeStore.UnLockSkillData(rewardData.skillData);
                                }
                                else
                                {
                                    _researchTreeStore.SetCost(_researchTreeStore.GetCost() + rewardData.skillPoint);
                                }
                            }
                        }

                        testWorldUnitObjectArray = PlayerUnitManager.Instance.GetUnitObjectArray();
                        for (int i = 0; i < testWorldUnitObjectArray.Length; i++)
                        {
                            _researchTreeStore.CheckUnLockSkillData(rewardData.skillData, out bool isExist, out bool isLock);
                            if (isExist && !isLock)
                            {
                                //해당되는 스킬데이터 변경
                                _researchTreeStore.UnLockSkillData(rewardData.skillData);
                            }
                            else
                            {
                                _researchTreeStore.SetCost(_researchTreeStore.GetCost() + rewardData.skillPoint);
                            }
                        }
                        break;
                    case GiveType.PlayerUnit:
                        testWorldUnitObject = PlayerUnitManager.Instance.GetPlayerControlUnit();
                        if (testWorldUnitObject != null)
                        {
                            if (testWorldUnitObject.TryGetComponent(out _researchTreeStore))
                            {
                                _researchTreeStore.CheckUnLockSkillData(rewardData.skillData, out bool isExist, out bool isLock);
                                if (isExist && !isLock)
                                {
                                    //해당되는 스킬데이터 변경
                                    _researchTreeStore.UnLockSkillData(rewardData.skillData);
                                }
                                else
                                {
                                    _researchTreeStore.SetCost(_researchTreeStore.GetCost() + rewardData.skillPoint);
                                }
                            }
                        }
                        break;
                    case GiveType.PartnerUnit:
                        testWorldUnitObjectArray = PlayerUnitManager.Instance.GetUnitObjectArray();
                        for (int i = 0; i < testWorldUnitObjectArray.Length; i++)
                        {
                            _researchTreeStore.CheckUnLockSkillData(rewardData.skillData, out bool isExist, out bool isLock);
                            if (isExist && !isLock)
                            {
                                //해당되는 스킬데이터 변경
                                _researchTreeStore.UnLockSkillData(rewardData.skillData);
                            }
                            else
                            {
                                _researchTreeStore.SetCost(_researchTreeStore.GetCost() + rewardData.skillPoint);
                            }
                        }
                        break;
                }
            }

            //게임오브젝트
            //처리방법=>게임오브젝트를 액터근처에 놓아줌
            if (rewardData.objectDeScriptionData != null)
            {
                GameObject gameObject = Instantiate(rewardData.targetObject, pos);
                gameObject.transform.position = InventoryManager.Instance.GetRandomSpawnPosition(false, pos, 3);
            }
        }

        /// <summary>
        /// 씬에 있는 퀘스트보상기버를 돌려주는 함수
        /// </summary>
        /// <param name="findActorInfoData">찾을 액터데이터</param>
        /// <returns>해당되는 액터데이터 위치</returns>
        private Transform GetRewardGiver(ActorInfoObjectScript findActorInfoData)
        {
            Transform targetPos = null;

            if (sceneQuestRewardGiverBible.ContainsKey(findActorInfoData))
            {
                targetPos = sceneQuestRewardGiverBible[findActorInfoData].transform;
            }
            if (targetPos = null)
            {
                LogManager.Log(typeof(QuestSystemManager), "존재하지않습니다", null, LogManager.LogType.Error);
            }
            return targetPos;
        }

        /// <summary>
        /// 작동되고 있는 리퀘스트에 존재하는 지 
        /// </summary>
        /// <param name="request">점검할 리퀘스트</param>
        /// <returns>작동여부</returns>
        public bool ExistActiveRequest(Request request)
        {
            return activeRequestList.Contains(request);
        }

        /// <summary>
        /// 현재 작동되고 있는 리퀘스트들을 불려온다
        /// </summary>
        /// <returns></returns>
        public Request[] GetActiveRequestArray()
        {
            return activeRequestList.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="questMissionCATType"></param>
        /// <returns></returns>
        public Sprite GetQuestMissionTypeImage(QuestMissionCATType questMissionCATType)
        {
            Sprite sprite = null;
            switch (questMissionCATType)
            {
                case QuestMissionCATType.Main:
                    sprite = mainQuestMarkSprite;
                    break;
                case QuestMissionCATType.Sub:
                    sprite = subQuestMarkSprite;
                    break;
                case QuestMissionCATType.Special:
                    sprite = specialQuestMarkSprite;
                    break;
                case QuestMissionCATType.Unexpected:
                    break;
            }
            return sprite;
        }

        /// <summary>
        /// 씬에있는 퀘스트기버를 등록하는 함수
        /// </summary>
        /// <param name="actorInfoData">액터데이터</param>
        /// <param name="questRewardGiver">퀘스트보상기버</param>
        public void AddSceneQuestRewardGiver(ActorInfoObjectScript actorInfoData, QuestRewardGiver questRewardGiver)
        {
            sceneQuestRewardGiverBible.Add(actorInfoData, questRewardGiver);
        }

        /// <summary>
        /// 씬에있는 퀘스트기버를 삭제하는 함수
        /// </summary>
        /// <param name="actorInfoData">액터데이터</param>        
        public void RemoveSceneQuestRewardGiver(ActorInfoObjectScript actorInfoData)
        {
            sceneQuestRewardGiverBible.Remove(actorInfoData);
        }

        /// <summary>
        /// 리퀘스트바이블에 미션내역을 추가해주는 함수
        /// </summary>
        /// <param name="questNodeData">퀘스트미션이 있는 데이터</param>
        /// <param name="request">리퀘스트</param>
        public void AddRequestBible(QuestNodeData questNodeData, Request request)
        {
            if (!requestBible.ContainsKey(questNodeData))
            {
                request = new Request();
                requestBible.Add(questNodeData, request);
            }
        }

        
        /// <summary>
        /// 퀘스트노드에 해당되는 리퀘스트를 불려오는 함수
        /// </summary>
        /// <param name="questNodeData">퀘스트노드데이터</param>
        /// <returns>해당되는 리퀘스트</returns>
        public Request GetRequestBible(QuestNodeData questNodeData)
        {
            Request request = null;
            if (requestBible.ContainsKey(questNodeData))
            {
                request = requestBible[questNodeData];
            }
            return request;
        }

        /// <summary>
        /// 퀘스트북데이터의 선행퀘스트를 다했는지 체크해주는 함수
        /// </summary>
        /// <param name="questBookData">확인할 퀘스트북데이터</param>
        /// <returns>다했는지 여부</returns>
        public bool CheckPrecedeQuestBookIDKey(QuestBookData questBookData)
        {
            bool isDone = true;
            //리퀘스트에서 확인해줘야됨
            //퀘스트바이블에서 먼저 찾아봄

            for (int i = 0; i < questBookData.precedeQuestBookIDKeyArray.Length; i++)
            {
                string questBookID = questBookData.precedeQuestBookIDKeyArray[i];

                //해당되는 퀘스트북ID가 존재하는지
                if (questBookDataBible.ContainsKey(questBookID))
                {
                    //존재하면 퀘스트북데이터에서 마지막미션의 리퀘스트를 찾아서 한번이상성공했는지
                    Request request = requestBible[questBookDataBible[questBookID].questNodeDataArray[questBookDataBible[questBookID].questNodeDataArray.Length]];
                    if (request == null)
                    {
                        //없으면 캔슬
                        isDone = false;
                        break;
                    }
                    else
                    {
                        if (request.questSuccessedCount > 0)
                        {
                            //확인후 넘어갈것
                            Debug.Log(request.targetQuestBookIDKey + "_" + request.targetQuestNodeData.questName + "선행퀘스트완료");
                        }
                        else
                        {
                            //안했으면 캔슬
                            isDone = false;
                            break;
                        }
                    }
                }
                else
                {
                    //없으면 캔슬
                    isDone = false;
                    break;
                }
            }
            return isDone;
        }

        /// <summary>
        /// 리퀘스트UI 요청 함수
        /// </summary>
        /// <returns>RequestUI오브젝트</returns>
        public RequestUI RequestRequestUI()
        {
            //초기화
            isFind = false;
            RequestUI targetRequestUIObject = null;

            //로직작동
            for (int i = 0; i < targetRequestUIPrefabList.Count; i++)
            {

                if (!targetRequestUIPrefabList[i].gameObject.activeSelf)
                {
                    isFind = true;
                    targetRequestUIObject = targetRequestUIPrefabList[i];
                    //리셋구간
                    break;
                }

            }
            //찾은게 없다면 오브젝트 하나를 만들어준다.
            if (!isFind)
            {
                targetRequestUIObject = Instantiate(targetRequestUIPrefab, transform.position, Quaternion.identity, transform);
                //리셋구간
                targetRequestUIPrefabList.Add(targetRequestUIObject);
                targetRequestUIObject.gameObject.SetActive(false);
            }
            return targetRequestUIObject;
        }

        /// <summary>
        /// 미션체커UI 요청 함수
        /// </summary>
        /// <returns>MissionCheckerUI오브젝트</returns>
        public QuestMissionCheckerUI RequestMissionCheckerUI()
        {
            //초기화
            isFind = false;
            QuestMissionCheckerUI targetMissionCheckerUIObject = null;

            //로직작동
            for (int i = 0; i < targetMissionCheckerUIPrefabList.Count; i++)
            {

                if (!targetMissionCheckerUIPrefabList[i].gameObject.activeSelf)
                {
                    isFind = true;
                    targetMissionCheckerUIObject = targetMissionCheckerUIPrefabList[i];
                    //리셋구간
                    break;
                }

            }
            //찾은게 없다면 오브젝트 하나를 만들어준다.
            if (!isFind)
            {
                targetMissionCheckerUIObject = Instantiate(targetMissionCheckerUIPrefab, transform.position, Quaternion.identity, transform);
                //리셋구간
                targetMissionCheckerUIPrefabList.Add(targetMissionCheckerUIObject);
                targetMissionCheckerUIObject.gameObject.SetActive(false);
            }
            return targetMissionCheckerUIObject;
        }

        /// <summary>
        /// 퀘스트보상선택버튼 요청 함수
        /// </summary>
        /// <returns>RewardSelectButton</returns>
        public RewardSelectButton RequestRewardSelectButton()
        {
            //초기화
            isFind = false;
            RewardSelectButton targetQuestRewardBtnObject = null;

            //로직작동
            for (int i = 0; i < targetQuestRewardBtnPrefabList.Count; i++)
            {

                if (!targetQuestRewardBtnPrefabList[i].gameObject.activeSelf)
                {
                    isFind = true;
                    targetQuestRewardBtnObject = targetQuestRewardBtnPrefabList[i];
                    //리셋구간
                    break;
                }

            }
            //찾은게 없다면 오브젝트 하나를 만들어준다.
            if (!isFind)
            {
                targetQuestRewardBtnObject = Instantiate(targetQuestRewardBtnPrefab, transform.position, Quaternion.identity, transform);
                //리셋구간
                targetQuestRewardBtnPrefabList.Add(targetQuestRewardBtnObject);
                targetQuestRewardBtnObject.gameObject.SetActive(false);
            }
            return targetQuestRewardBtnObject;
        }
    }
}
#endif
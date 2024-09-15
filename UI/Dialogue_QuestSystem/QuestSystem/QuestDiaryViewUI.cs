
#if Doozy && Enhance

using Doozy.Engine.UI;
using UnityEngine.UI;
using UnityEngine;
using EnhancedUI.EnhancedScroller;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;
using lLCroweTool.UI.Confirm;
using lLCroweTool.TimerSystem;

namespace lLCroweTool.QuestSystem
{
    /// <summary>
    /// 퀘스트일지 뷰
    /// </summary>
    public class QuestDiaryViewUI : UpdateTimerModule_Base, IEnhancedScrollerDelegate, IOnlyOnePlayerUI
    {
        //퀘스트목록스크롤
        //제목,퀘스트타입
        //시간, 니드
        [Header("퀘스트목록컨텐츠설정")]
        public TextMeshProUGUI questListTitleTextObject;
        public EnhancedScroller questListScroll;
        public QuestSelectButton questSelectButtonPrefab;

        //퀘스트정보스크롤
        //제목,퀘스트타입
        //내용
        //시간, 니드, 보상, 위치
        [Space]
        [Header("퀘스트정보콘텐츠 설정-스크롤")]
        public TextMeshProUGUI questInfoTitleTextObject;
        public ScrollRect questInfoScroll;
        //구조
        //정보스크롤오브젝트의 Content에 verticallayout을 집어넣고
        //해당안쪽에 여러오브젝트를 배치

        //image
        //verticalLayout control childSize = width height
        //conetentsize fitter vertical fit = preferred size

        [Header("퀘스트정보콘텐츠 설정-내용")]
        //처리방법에 따라 다름//타입이미지 + 제목
        public Image questTypeImage;
        public TextMeshProUGUI questBookTitleText;
        public TextMeshProUGUI questBookContentInfoText;
        public TextMeshProUGUI questTimeText;
        public Transform missionCheckerUIPos;//미션체커 위치지정
        public QuestMissionCheckerUI[] missionCheckerUIArray;
        public TextMeshProUGUI questBookRewardGiveTypeText;

        //퀘스트보상선택버튼을 집어넣어주는
        public Transform questRewardPos;
        //image
        //conetentsize fitter vertical fit = preferred size
        //gridlayoutgroup upperleft horizontal upperleft flexible
        //cellsize = 142 50 

        [Space]
        [Header("확인창설정")]
        public Sprite questConfirmNoticeImage;//퀘스트확인창 주의이미지
        public ConfirmWindow confirmWindow;//확인창

        [Space]
        [Header("버튼설정")]
        public UIButton selectInterectionButton;//수락 포기 버튼
        public UIButton dealButton;//퀘스트 납품 버튼//게시판일시만 가능
        private UIView targetUIView;

        //상향된스크롤용 캐싱
        [SerializeField] private List<QuestBookData> questBookDataList = new List<QuestBookData>();
        [SerializeField] private List<Request> requestDataList = new List<Request>();
        [SerializeField] private List<RewardSelectButton> activeRewardSelectButton = new List<RewardSelectButton>();//보상 보여주는용
        private int scrollIndexSize;

        private bool boardOpen;
        private UnityEvent UpdateTextEvent;


        protected override void Awake()
        {   
            targetUIView = GetComponent<UIView>();
            questListScroll.Delegate = this;
            lLcroweUtil.GetAddUnitEvent(ref UpdateTextEvent);
        }
        
        /// <summary>
        /// 퀘스트 일지를 여는 함수
        /// </summary>
        /// <param name="isBoardOpen"></param>
        /// <param name="questGiver_Board"></param>
        public void OpenQuestDiaryViewUI(bool isBoardOpen, QuestGiver_Board questGiver_Board)
        {
            if (PlayerUIManager.Instance.RecordIOnlyOnePlayerUI(this))
            {
                return;
            }

            //생각해보니 퀘스트노드들을 체크하는것
            questBookDataList.Clear();
            requestDataList.Clear();
            ResetQuestInfo();
            boardOpen = isBoardOpen;
            scrollIndexSize = 0;
            selectInterectionButton.gameObject.SetActive(false);
            dealButton.gameObject.SetActive(false);


            //게시판에서 열었는가
            if (isBoardOpen)
            {
                //게시판에서 열음
                //게시판에 있는 퀘스트북을 등록해줌
                string[] questBookIDArray = questGiver_Board.questBookID;
                for (int i = 0; i < questBookIDArray.Length; i++)
                {   
                    //퀘스트북체크
                    QuestBookData questBookData = QuestSystemManager.Instance.GetQuestBookData(questBookIDArray[i]);

                    if (questBookData != null)
                    {
                        //해당되는 리퀘스트가 있는지 체크
                        //로직체크구역

                        //해당되는 퀘스트북데이터의 마지막 미션에 대한 데이터를 가져와서 체크해야함
                        //버그상태 퀘스트미션노드가 한개도없는데 진행할려고하니 버그걸린것
                        if (questBookData.questNodeDataArray.Length != 0)
                        {
                            Request request = QuestSystemManager.Instance.GetRequestBible(questBookData.questNodeDataArray[questBookData.questNodeDataArray.Length - 1]);
                            if (request == null)
                            {
                                //존재하지않으면
                                //선행퀘스트가 다 완료됫는지에 따라보여줌
                                if (QuestSystemManager.Instance.CheckPrecedeQuestBookIDKey(questBookData))
                                {
                                    //해당되는 퀘스트노드에 대하여 리퀘스트 추가
                                    request = new Request();
                                    request.InitQuestData(questBookIDArray[i], questBookData.questNodeDataArray[0]);
                                    QuestSystemManager.Instance.AddRequestBible(questBookData.questNodeDataArray[0], request);
                                    questBookDataList.Add(questBookData);
                                    requestDataList.Add(request);
                                    scrollIndexSize++;
                                }
                            }
                            else
                            {
                                //존재하면
                                //루프할수 있는지 체크
                                if (questBookData.isLoopQuest)
                                {
                                    //퀘스트재발행 가능
                                    questBookDataList.Add(questBookData);
                                    requestDataList.Add(request);
                                    scrollIndexSize++;
                                }
                                else
                                {
                                    //퀘스트재발행 불가
                                    //작동될 수량 체크//0보다크면 작동안함
                                    if (request.questActionCount == 0)
                                    {
                                        questBookDataList.Add(questBookData);
                                        requestDataList.Add(request);
                                        scrollIndexSize++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                //플레이어의 퀘스트일지
                //현재 작동중인 리퀘스트들과 퀘스트북을 등록해줌
                Request[] requestArray = QuestSystemManager.Instance.GetActiveRequestArray();//후에 퀘스트일지에 통합시킬것//현재상태로는 멀티를 못함
                for (int i = 0; i < requestArray.Length; i++)
                {   
                    QuestBookData questBookData = QuestSystemManager.Instance.GetQuestBookData(requestArray[i].targetQuestBookIDKey);
                    questBookDataList.Add(questBookData);
                    requestDataList.Add(requestArray[i]);
                    scrollIndexSize++;
                }
            }
            questListScroll.ReloadData();
            targetUIView.Show();            
        }

        //버튼에 등록하는함수
        /// <summary>
        /// 퀘스트정보에 있는 텍스트등을 리셋해주는 함수
        /// </summary>
        private void ResetQuestInfo()
        {
            dealButton.Button.onClick.RemoveAllListeners();
            selectInterectionButton.Button.onClick.RemoveAllListeners();
            questTypeImage.sprite = null;
            questBookTitleText.text = "";
            questBookContentInfoText.text = "";
            questTimeText.text = "";
            for (int i = 0; i < missionCheckerUIArray.Length; i++)
            {
                missionCheckerUIArray[i].gameObject.SetActive(false);
            }
            missionCheckerUIArray = new QuestMissionCheckerUI[0];
            UpdateTextEvent.RemoveAllListeners();
            questBookRewardGiveTypeText.text = "";
            for (int i = 0; i < activeRewardSelectButton.Count; i++)
            {
                activeRewardSelectButton[i].gameObject.SetActive(false);
            }
        }

        //버튼에 등록하는함수
        /// <summary>
        /// 퀘스트정보를 보여주는 함수
        /// </summary>
        /// <param name="questBookData"></param>
        /// <param name="request"></param>
        private void ShowQuestInfo(QuestBookData questBookData, Request request)
        {
            //제목//퀘스트타입
            questTypeImage.sprite = QuestSystemManager.Instance.GetQuestMissionTypeImage(questBookData.questType);
            questBookTitleText.text = questBookData.questBookTitle;

            //내용
            questBookContentInfoText.text = questBookData.questBookContentInfo;

            //보상주는타입
            questBookRewardGiveTypeText.text = LocalizingManager.Instance.GetLocalLizeText(questBookData.questRewardGiveType.ToString());

            //보상
            for (int i = 0; i < questBookData.questBookRewardDataArray.Length; i++)
            {
                RewardSelectButton rewardSelectButton = QuestSystemManager.Instance.RequestRewardSelectButton();
                activeRewardSelectButton.Add(rewardSelectButton);
                rewardSelectButton.SetQuestRewardButton(questBookData.questBookRewardDataArray[i], false, "", null);
                rewardSelectButton.transform.SetParent(questRewardPos);
            }

            //처리해줘야되는 것
            //1. 수락 포기 수납 버튼처리            
            //2. 남은시간
            //3. 미션체커            
            if (boardOpen)
            {
                //게시판으로 열림
                //리퀘스트의 작동여부로 수락 포기 수납버튼들과 남은시간 미션체커 확인하기                
                if (request.GetIsActive())
                {
                    //리퀘스트작동상태
                    //해당리퀘스트를 완료했는지 체크

                    //수락 포기 수납 버튼처리

                    //리퀘스트의 완료가능한 여부
                    if (request.CheckSuccessReQuest())
                    {
                        dealButton.gameObject.SetActive(true);
                        dealButton.SetLabelText(LocalizingManager.Instance.GetLocalLizeText("퀘스트완료"));
                        dealButton.Button.onClick.AddListener(delegate { DealFunc(request); });
                    }
                    else
                    {
                        dealButton.gameObject.SetActive(false);
                    }

                    QuestBookData tempData = QuestSystemManager.Instance.GetQuestBookData(request.targetQuestBookIDKey);
                    if (tempData.isGiveUpQuest)
                    {
                        selectInterectionButton.gameObject.SetActive(true);
                        selectInterectionButton.SetLabelText(LocalizingManager.Instance.GetLocalLizeText("퀘스트포기"));
                        selectInterectionButton.Button.onClick.AddListener(delegate { GiveUpFunc(request); });
                    }
                    else
                    {
                        selectInterectionButton.gameObject.SetActive(false);                        
                    }
                    
                    //남은시간
                    if (request.targetQuestNodeData.isUseTimer)
                    {
                        questTimeText.gameObject.SetActive(true);
                        questTimeText.text = request.worldTime.GetWorldTime();
                    }
                    else
                    {
                        questTimeText.gameObject.SetActive(false);
                    }

                    //미션체커
                    ShowMissionChecker(request);
                }
                else
                {
                    //리퀘스트작동안하는상태
                    //수락 포기 수납 버튼처리
                    dealButton.gameObject.SetActive(false);
                    selectInterectionButton.gameObject.SetActive(true);
                    selectInterectionButton.SetLabelText(LocalizingManager.Instance.GetLocalLizeText("퀘스트수락"));
                    selectInterectionButton.Button.onClick.AddListener(delegate { AcceptFunc(request); });

                    //남은시간
                    if (request.targetQuestNodeData.isUseTimer)
                    {
                        questTimeText.gameObject.SetActive(true);
                        questTimeText.text = request.worldTime.GetWorldTime();
                    }
                    else
                    {
                        questTimeText.gameObject.SetActive(false);
                    }

                    //미션체커
                    ShowMissionChecker(request);
                }
            }
            else
            {
                //퀘스트일지로 열림
                //보이는 리퀘스트들은 작동되고 있는 상태

                //수락 포기 수납 버튼처리
                dealButton.gameObject.SetActive(false);

                QuestBookData tempData = QuestSystemManager.Instance.GetQuestBookData(request.targetQuestBookIDKey);
                if (tempData.isGiveUpQuest)
                {
                    selectInterectionButton.gameObject.SetActive(true);
                    selectInterectionButton.SetLabelText(LocalizingManager.Instance.GetLocalLizeText("퀘스트포기"));
                    selectInterectionButton.Button.onClick.AddListener(delegate { GiveUpFunc(request); });
                }
                else
                {
                    selectInterectionButton.gameObject.SetActive(false);
                }

                //남은시간
                if (request.targetQuestNodeData.isUseTimer)
                {
                    questTimeText.gameObject.SetActive(true);
                    questTimeText.text = request.worldTime.GetWorldTime();
                }
                else
                {
                    questTimeText.gameObject.SetActive(false);
                }

                //미션체커
                ShowMissionChecker(request);

                //업데이트하여 갱신해야할것들                
                UpdateTextEvent.AddListener(delegate { UpdateRequestMissionChecker(request); });
            }
        }

        /// <summary>
        /// 리퀘스트이 체커를 보여주는 함수
        /// </summary>
        /// <param name="request">리퀘스트</param>
        private void ShowMissionChecker(Request request)
        {   
            List<QuestMissionCheckerUI> tempList = new List<QuestMissionCheckerUI>();
            string[] questHUDTextArray = request.GetMissionHUDTextArray();
            for (int i = 0; i < questHUDTextArray.Length; i++)
            {
                QuestMissionCheckerUI missionCheckerUI = QuestSystemManager.Instance.RequestMissionCheckerUI();
                missionCheckerUI.gameObject.SetActive(true);
                missionCheckerUI.transform.SetParent(missionCheckerUIPos);
                tempList.Add(missionCheckerUI);
                //로컬라이징해서 가져옴
                string resultText = "";
                string[] tempArray = questHUDTextArray[i].Split('_');
                for (int j = 0; j < tempArray.Length; j++)
                {
                    string temp = LocalizingManager.Instance.GetLocalLizeText(tempArray[j]);
                    resultText += temp;
                }
                missionCheckerUI.SetMissionNameText(resultText);
                missionCheckerUI.SetMissionCheckText(request.GetMissionCheckerArray()[i].GetCheckValue());
                missionCheckerUIArray[i].SetMissionCountValue(request.GetMissionCheckerArray()[i].GetCount());
            }
            missionCheckerUIArray = tempList.ToArray();
        }

        /// <summary>
        /// 리퀘스트에서 업데이트되서 보여줄거를 세팅해주는 함수
        /// </summary>
        /// <param name="request"></param>
        private void UpdateRequestMissionChecker(Request request)
        {
            //남은시간
            if (request.targetQuestNodeData.isUseTimer)
            {
                questTimeText.text = request.worldTime.GetWorldTime();
            }

            //미션체커
            for (int i = 0; i < missionCheckerUIArray.Length; i++)
            {   
                if (request.GetMissionCheckerArray()[i].GetCount() != missionCheckerUIArray[i].GetCountValue())
                {
                    missionCheckerUIArray[i].SetMissionCountValue(request.GetMissionCheckerArray()[i].GetCount());
                }
            }
        }

        /// <summary>
        /// 리퀘스트 미션을 완료하는 함수
        /// </summary>
        private void DealFunc(Request request)
        {
            confirmWindow.SetConfirmWindow("Quest_DealNotice",
            delegate
            {
                QuestSystemManager.Instance.QuestNextSequence(true, request);
                ResetQuestInfo();
            }, false, "Quest_Deal", "Quest_No" , questConfirmNoticeImage);
            confirmWindow.ShowConfirmWindowUIView();
        }

        /// <summary>
        /// 퀘스트 수락기능
        /// </summary>
        private void AcceptFunc(Request request)
        {
            confirmWindow.SetConfirmWindow("Quest_AcceptNotice",
            delegate
            {
                QuestSystemManager.Instance.RequestQuestBook(request.targetQuestBookIDKey);
                ResetQuestInfo();
                ShowQuestInfo(QuestSystemManager.Instance.GetQuestBookData(request.targetQuestBookIDKey), request);
            }, false, "Quest_Accept", "Quest_No", questConfirmNoticeImage);
            confirmWindow.ShowConfirmWindowUIView();
        }

        /// <summary>
        /// 퀘스트 포기기능
        /// </summary>
        private void GiveUpFunc(Request request)
        {
            confirmWindow.SetConfirmWindow("Quest_GiveUpNotice",
            delegate
            {
                request.CheckFinal(false, true);
                ResetQuestInfo();
            }, false, "Quest_GiveUp", "Quest_No", questConfirmNoticeImage);
            confirmWindow.ShowConfirmWindowUIView();
        }

        /// <summary>
        /// 퀘스트 일지를 닫는 함수
        /// </summary>
        public void CloseQuestDiaryViewUI()
        {
            //QuestSystemManager.Instance.ResetQuestSelectButtonList();           
            ResetQuestInfo();
            targetUIView.Hide();
        }

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return scrollIndexSize;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            //셀사이즈 처리
            return (dataIndex % 2 == 0 ? 30f : 100f);
        }

        /// <summary>
        /// 표시할 셀을 가져옵니다. 셀 유형이 다양하여 목록에 다양성을 부여할 수 있습니다.
        /// 헤더, 바닥글 및 기타 그룹화 셀을 예로 들 수 있습니다.
        /// </summary>
        /// <param name="scroller">셀을 요청하는 스크롤러</param>
        /// <param name="dataIndex">스크롤러가 요청하고 있는 데이터의 인덱스</param>
        /// <param name="cellIndex">록의 인덱스입니다. 스크롤러가 루핑되는 경우 dataIndex와 다를 수 있습니다.</param>
        /// <returns>스크롤러가 사용할 셀입니다.</returns>
        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            //셀 오브젝트폴 등록
            QuestSelectButton questSelectButton = (QuestSelectButton)scroller.GetCellView(questSelectButtonPrefab);
            questSelectButton.ResetQuestSelectButton();
            questSelectButton.name = "Cell " + dataIndex.ToString();

            //데이터가져오기
            QuestBookData questBookData = questBookDataList[dataIndex];
            Request requestData = requestDataList[dataIndex];            

            UnityAction unityAction = delegate
            {
                ResetQuestInfo();
                ShowQuestInfo(questBookData, requestData);
            };
            questSelectButton.SetQuestSelectButton(questBookData, unityAction);

            return questSelectButton;
        }

        public void OffOnlyOnePlayerUI()
        {
            CloseQuestDiaryViewUI();
        }

        public override void UpdateTimerModuleFunc()
        {
            UpdateTextEvent?.Invoke();
        }
       
        public GameObject GetGameObject()
        {
            return gameObject;
        }
    }
}
#endif
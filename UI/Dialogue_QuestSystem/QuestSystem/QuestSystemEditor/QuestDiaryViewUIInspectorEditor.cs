#if Doozy
using Doozy.Engine.UI;
using UnityEngine.UI;
using UnityEngine;
using EnhancedUI.EnhancedScroller;
using EnhancedUI;
using UnityEngine.Events;
using TMPro;
using lLCroweTool.QuestSystem;
#if UNITY_EDITOR
using UnityEditor;
#pragma warning disable 0618
namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(QuestDiaryViewUI))]
    public class QuestDiaryViewUIInspectorEditor : Editor
    {
        //인스팩터창 에디터

        //현재 등록된 기초정보들과 들어간 데이터량들을 표시
        //윈도우에디터를 켜서 해당 DB 데이터를 볼수 있게 세팅        
        private QuestDiaryViewUI questDiaryViewUI;


        private void OnEnable()
        {
            questDiaryViewUI = (QuestDiaryViewUI)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            bool isDone = true;

            if (questDiaryViewUI.questListTitleTextObject == null)
            {
                EditorGUILayout.HelpBox("퀘스트리스트 제목텍스트오브젝트가 필요합니다.", MessageType.Warning);
                isDone = false;
            }
            if (questDiaryViewUI.questListScroll == null)
            {
                EditorGUILayout.HelpBox("퀘스트리스트 상향된스크롤이 필요합니다.", MessageType.Warning);
                isDone = false;
            }
            if (questDiaryViewUI.questSelectButtonPrefab == null)
            {
                EditorGUILayout.HelpBox("퀘스트선택버튼이 필요합니다.(수동설정)", MessageType.Warning);                
            }

            
            if (questDiaryViewUI.questInfoTitleTextObject == null)
            {
                EditorGUILayout.HelpBox("퀘스트정보 제목텍스트오브젝트가 필요합니다.", MessageType.Warning);
                isDone = false;
            }
            if (questDiaryViewUI.questInfoScroll == null)
            {
                EditorGUILayout.HelpBox("퀘스트정보 스크롤이 필요합니다.(수동설정)", MessageType.Warning);
                isDone = false;
            }
            else
            {
                if (questDiaryViewUI.questInfoScroll.content == null)
                {
                    EditorGUILayout.HelpBox("퀘스트정보 스크롤의 컨텐츠 오브젝트가 필요합니다.(수동설정)", MessageType.Warning);
                    isDone = false;
                }
                else
                {
                    if (!questDiaryViewUI.questInfoScroll.content.TryGetComponent(out VerticalLayoutGroup verticalLayoutGroup))
                    {
                        EditorGUILayout.HelpBox("퀘스트정보 스크롤의 컨텐츠에 새로레이아웃 그룹컴포넌트가 필요합니다.", MessageType.Warning);
                        isDone = false;
                    }
                    if (!questDiaryViewUI.questInfoScroll.content.TryGetComponent(out ContentSizeFitter contentSizeFitter))
                    {
                        EditorGUILayout.HelpBox("퀘스트정보 스크롤의 컨텐츠에 컨텐츠 필터 컴포넌트가가 필요합니다.", MessageType.Warning);
                        isDone = false;
                    }
                }
            }

            if (questDiaryViewUI.questTypeImage == null)
            {
                EditorGUILayout.HelpBox("퀘스트타입 이미지오브젝트가 필요합니다.(퀘스트정보 스크롤필요)", MessageType.Warning);
                isDone = false;
            }
            if (questDiaryViewUI.questBookTitleText == null)
            {
                EditorGUILayout.HelpBox("퀘스트북제목 텍스트오브젝트가 필요합니다.(퀘스트정보 스크롤필요)", MessageType.Warning);
                isDone = false;
            }
            if (questDiaryViewUI.questBookContentInfoText == null)
            {
                EditorGUILayout.HelpBox("퀘스트북내용정보 텍스트오브젝트가 필요합니다.(퀘스트정보 스크롤필요)", MessageType.Warning);
                isDone = false;
            }
            if (questDiaryViewUI.questTimeText == null)
            {
                EditorGUILayout.HelpBox("퀘스트시간 텍스트오브젝트가 필요합니다.(퀘스트정보 스크롤필요)", MessageType.Warning);
                isDone = false;
            }
            if (questDiaryViewUI.missionCheckerUIPos == null)
            {
                EditorGUILayout.HelpBox("퀘스트체커 위치가 필요합니다.(퀘스트정보 스크롤필요)", MessageType.Warning);
                isDone = false;
            }
            else
            {
                if (!questDiaryViewUI.missionCheckerUIPos.TryGetComponent(out VerticalLayoutGroup verticalLayoutGroup))
                {
                    EditorGUILayout.HelpBox("퀘스트체커 표시를 위한 가로위치그룹이 필요합니다.(수동설정)", MessageType.Warning);
                    isDone = false;
                }
            }

            if (questDiaryViewUI.questBookRewardGiveTypeText == null)
            {
                EditorGUILayout.HelpBox("퀘스트보상 주는타입 텍스트오브젝트가 필요합니다.(퀘스트정보 스크롤필요)", MessageType.Warning);
                isDone = false;
            }
            if (questDiaryViewUI.questRewardPos == null)
            {
                EditorGUILayout.HelpBox("퀘스트보상 위치가 필요합니다.(퀘스트정보 스크롤필요)", MessageType.Warning);
                isDone = false;
            }

            if (questDiaryViewUI.confirmWindow == null)
            {
                EditorGUILayout.HelpBox("확인창이 필요합니다.(수동설정)", MessageType.Warning);
            }
            if (questDiaryViewUI.selectInterectionButton == null)
            {
                EditorGUILayout.HelpBox("퀘스트상호작용 버튼이 필요합니다.(수동설정)", MessageType.Warning);
            }
            if (questDiaryViewUI.dealButton == null)
            {
                EditorGUILayout.HelpBox("퀘스트상납 버튼이 필요합니다.(수동설정)", MessageType.Warning);
            }
            if (!questDiaryViewUI.TryGetComponent(out UIView uIView))
            {
                EditorGUILayout.HelpBox("UIView가 필요합니다.", MessageType.Warning);
                isDone = false;
            }

            if (!isDone)
            {
                if (GUILayout.Button("초기 자동설정"))
                {
                    if (questDiaryViewUI.questListTitleTextObject == null)
                    {
                        GameObject gameObject = new GameObject();
                        gameObject.transform.parent = questDiaryViewUI.transform;
                        gameObject.transform.position = questDiaryViewUI.transform.position;
                        gameObject.name = "QuestListTitleTextObject";
                        questDiaryViewUI.questListTitleTextObject = gameObject.AddComponent<TextMeshProUGUI>();
                        questDiaryViewUI.questListTitleTextObject.text = "QuestList";
                    }
                    if (questDiaryViewUI.questListScroll == null)
                    {
                        GameObject gameObject = new GameObject();
                        gameObject.transform.parent = questDiaryViewUI.transform;
                        gameObject.transform.position = questDiaryViewUI.transform.position;
                        gameObject.name = "questListScroll";
                        questDiaryViewUI.questListScroll = gameObject.AddComponent<EnhancedScroller>();
                        gameObject.AddComponent<Image>();
                        if (!gameObject.TryGetComponent(out ScrollRect scrollRect))
                        {
                            gameObject.AddComponent<ScrollRect>();
                        }
                    }
                    if (questDiaryViewUI.questInfoTitleTextObject == null)
                    {
                        GameObject gameObject = new GameObject();
                        gameObject.transform.parent = questDiaryViewUI.transform;
                        gameObject.transform.position = questDiaryViewUI.transform.position;
                        gameObject.name = "QuestInfoTitleTextObject";
                        questDiaryViewUI.questInfoTitleTextObject = gameObject.AddComponent<TextMeshProUGUI>();
                        questDiaryViewUI.questInfoTitleTextObject.text = "QuestInfo";
                    }
                    if (questDiaryViewUI.questInfoScroll != null)
                    {
                        if (questDiaryViewUI.questInfoScroll.content != null)
                        {
                            if (!questDiaryViewUI.questInfoScroll.content.TryGetComponent(out VerticalLayoutGroup verticalLayoutGroup))
                            {
                                verticalLayoutGroup = questDiaryViewUI.questInfoScroll.content.gameObject.AddComponent<VerticalLayoutGroup>();
                                verticalLayoutGroup.childControlWidth = true;
                                verticalLayoutGroup.childControlHeight = true;
                            }
                            if (!questDiaryViewUI.questInfoScroll.content.TryGetComponent(out ContentSizeFitter contentSizeFitter))
                            {
                                contentSizeFitter = questDiaryViewUI.questInfoScroll.content.gameObject.AddComponent<ContentSizeFitter>();
                                contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                            }

                            if (questDiaryViewUI.questTypeImage == null)
                            {
                                GameObject gameObject = new GameObject();
                                gameObject.transform.parent = questDiaryViewUI.questInfoScroll.content;
                                gameObject.transform.position = questDiaryViewUI.transform.position;
                                gameObject.name = "QuestTypeImage";
                                questDiaryViewUI.questTypeImage = gameObject.AddComponent<Image>();
                            }
                            if (questDiaryViewUI.questBookTitleText == null)
                            {
                                GameObject gameObject = new GameObject();
                                gameObject.transform.parent = questDiaryViewUI.questInfoScroll.content;
                                gameObject.transform.position = questDiaryViewUI.transform.position;
                                gameObject.name = "QuestBookTitleText";
                                questDiaryViewUI.questBookTitleText = gameObject.AddComponent<TextMeshProUGUI>();
                                questDiaryViewUI.questBookTitleText.text = "QuestBookTitleText";
                            }
                            if (questDiaryViewUI.questBookContentInfoText == null)
                            {
                                GameObject gameObject = new GameObject();
                                gameObject.transform.parent = questDiaryViewUI.questInfoScroll.content;
                                gameObject.transform.position = questDiaryViewUI.transform.position;
                                gameObject.name = "QuestBookContentInfoText";
                                questDiaryViewUI.questBookContentInfoText = gameObject.AddComponent<TextMeshProUGUI>();
                                questDiaryViewUI.questBookContentInfoText.text = "QuestBookContentInfoText";
                            }
                            if (questDiaryViewUI.questTimeText == null)
                            {
                                GameObject gameObject = new GameObject();
                                gameObject.transform.parent = questDiaryViewUI.questInfoScroll.content;
                                gameObject.transform.position = questDiaryViewUI.transform.position;
                                gameObject.name = "QuestTimeText";
                                questDiaryViewUI.questTimeText = gameObject.AddComponent<TextMeshProUGUI>();
                                questDiaryViewUI.questTimeText.text = "QuestTimeText";
                            }
                            if (questDiaryViewUI.missionCheckerUIPos == null)
                            {
                                GameObject gameObject = new GameObject();
                                gameObject.transform.parent = questDiaryViewUI.questInfoScroll.content;
                                gameObject.transform.position = questDiaryViewUI.transform.position;
                                gameObject.name = "MissionCheckerUIPos";
                                questDiaryViewUI.missionCheckerUIPos = gameObject.transform;
                                gameObject.AddComponent<VerticalLayoutGroup>();
                            }
                            if (questDiaryViewUI.questBookRewardGiveTypeText == null)
                            {
                                GameObject gameObject = new GameObject();
                                gameObject.transform.parent = questDiaryViewUI.questInfoScroll.content;
                                gameObject.transform.position = questDiaryViewUI.transform.position;
                                gameObject.name = "QuestRewardGiveTypeText";
                                questDiaryViewUI.questBookRewardGiveTypeText = gameObject.AddComponent<TextMeshProUGUI>();
                                questDiaryViewUI.questBookRewardGiveTypeText.text = "QuestRewardGiveTypeText";
                            }
                            if (questDiaryViewUI.questRewardPos == null)
                            {
                                GameObject gameObject = new GameObject();
                                gameObject.transform.parent = questDiaryViewUI.questInfoScroll.content;
                                gameObject.transform.position = questDiaryViewUI.transform.position;
                                gameObject.name = "QuestRewardPos";                                                    
                                ContentSizeFitter contentSize = gameObject.AddComponent<ContentSizeFitter>();
                                contentSize.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                                GridLayoutGroup gridLayoutGroup = gameObject.AddComponent<GridLayoutGroup>();
                                gridLayoutGroup.startCorner = GridLayoutGroup.Corner.UpperLeft;
                                gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
                                gridLayoutGroup.childAlignment = TextAnchor.UpperLeft;
                                gridLayoutGroup.constraint = GridLayoutGroup.Constraint.Flexible;
                                questDiaryViewUI.questRewardPos = gameObject.transform;
                            }
                        }
                    }
                }
            }
        }
    }
}
#endif
#endif
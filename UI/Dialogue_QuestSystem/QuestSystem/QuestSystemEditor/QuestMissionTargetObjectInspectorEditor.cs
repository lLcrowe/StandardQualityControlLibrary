#if Doozy
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.Events;
using Micosmo.SensorToolkit;
using lLCroweTool.WorldObjectSystem;
using lLCroweTool.InventorySystem;
using lLCroweTool.QuestSystem;
using lLCroweTool.DialogueSystem;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#pragma warning disable 0618
namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(QuestMissionTargetObject), true)]
    public class QuestMissionTargetObjectInspectorEditor : Editor
    {   
        private QuestMissionTargetObject targetQuestMissionObject;
        private List<QuestMissionTargetField> questMissionTargetFieldList = new List<QuestMissionTargetField>();
        private Label label;
        public static bool checkError = false;
        public static string errorText = "";
     
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            bool isDialogue = false;
            bool isUnit = false;
            bool isSensor = false;
            bool isItem = false;
           
            for (int i = 0; i < targetQuestMissionObject.questMissionType.Length; i++)
            {
                switch (targetQuestMissionObject.questMissionType[i])
                {
                    case QuestMissionType.Dialogue:
                        isDialogue = true;
                        break;
                    case QuestMissionType.Battle:
                    case QuestMissionType.Survive:
                        isUnit = true;
                        break;
                    case QuestMissionType.AttractMove:
                    case QuestMissionType.ArriveMap:
                        isSensor = true;
                        break;
                    case QuestMissionType.ItemGather:
                        isItem = true;
                        break;
                }
            }

            if (isDialogue)
            {
                if (!targetQuestMissionObject.TryGetComponent(out DialogueObjectTarget dialogueTargetWorldObject))
                {   
                    EditorGUILayout.HelpBox("DialogueTargetWorldObject 컴포넌트가 필요합니다.", MessageType.Warning);
                    if (GUILayout.Button("DialogueTargetWorldObject컴포넌트 추가"))
                    {
                        targetQuestMissionObject.gameObject.AddComponent<DialogueObjectTarget>();
                    }
                }
            }
            if (isUnit)
            {
                if (!targetQuestMissionObject.TryGetComponent(out TestWorldObject testWorldObject))
                {
                    EditorGUILayout.HelpBox("TestWorldObject 컴포넌트가 필요합니다\n", MessageType.Warning);
                    if (GUILayout.Button("WorldObject컴포넌트 추가"))
                    {
                        targetQuestMissionObject.gameObject.AddComponent<TestWorldObject>();
                    }
                }
            }
            if (isSensor)
            {
                if (!targetQuestMissionObject.TryGetComponent(out Sensor sensor))
                {
                    EditorGUILayout.HelpBox("Sensor를 상속받은 컴포넌트가 필요합니다\n", MessageType.Warning);
                    EditorGUILayout.HelpBox("-SensorComponent추가할것-\nRangeSensor2D\nRaySensor2D\nTriggerSensor2D", MessageType.Info);
                }
            }
            if (isItem)
            {
                if (!targetQuestMissionObject.TryGetComponent(out WorldItem worldItem))
                {   
                    EditorGUILayout.HelpBox("WorldItem 컴포넌트가 필요합니다", MessageType.Warning);
                    if (GUILayout.Button("WorldItem컴포넌트 추가"))
                    {
                        targetQuestMissionObject.gameObject.AddComponent<WorldItem>();
                    }
                }
            }
        }

        public override VisualElement CreateInspectorGUI()
        {
            targetQuestMissionObject = (QuestMissionTargetObject)target;
            QuestDBObjectScript questDBData = Resources.Load<QuestDBObjectScript>("ScrapNomadQuestDB");
            VisualElement rootElement = new VisualElement();
            rootElement.Add(new IMGUIContainer(OnInspectorGUI));//VisualElement에서 기존의 IMGUI를 탑재해서 작동시키게할수 있는 기능

            label = new Label();
            rootElement.Add(label);
            
            //보여주기만 하느곳
            if (targetQuestMissionObject.questBookID.Length == 0)
            {
                label.text = "데이터가 비어있습니다.";
            }
            else
            {
                //라벨쓰기
                label.text = "-입력된 데이터-\n";
                for (int i = 0; i < targetQuestMissionObject.questBookID.Length; i++)
                {
                    label.text += i + 1 + ". 퀘스트북ID : " + targetQuestMissionObject.questBookID[i] + " / 퀘스트이름 : " + targetQuestMissionObject.questName[i] + " / 미션타입 : " + targetQuestMissionObject.questMissionType[i].ToString() + " / 미션ID : " + targetQuestMissionObject.missionID[i];
                    if (i != targetQuestMissionObject.questBookID.Length)
                    {
                        label.text += "\n";
                    }
                }
            }

            //미션추가 버튼
            var newQuestMissionTargetButton = new Button(() =>
            {
                new QuestMissionTargetField(questMissionTargetFieldList, rootElement, questDBData);
            });
            newQuestMissionTargetButton.text = "연동할 미션추가";
            rootElement.Add(newQuestMissionTargetButton);

            //데이터 동기화
            var syncDataButton = new Button(() =>
            {   
                int index = questMissionTargetFieldList.Count;
                targetQuestMissionObject.questBookID = new string[index];
                targetQuestMissionObject.questName = new string[index];
                targetQuestMissionObject.questMissionType = new QuestMissionType[index];
                targetQuestMissionObject.missionID = new string[index];
                for (int i = 0; i < index; i++)
                {
                    targetQuestMissionObject.questBookID[i] = questMissionTargetFieldList[i].GetOrderQuestMissionData().questBookID;
                    targetQuestMissionObject.questName[i] = questMissionTargetFieldList[i].GetOrderQuestMissionData().questName;
                    targetQuestMissionObject.questMissionType[i] = questMissionTargetFieldList[i].GetOrderQuestMissionData().questMissionType;
                    targetQuestMissionObject.missionID[i] = questMissionTargetFieldList[i].GetOrderQuestMissionData().missionID;
                }

                //라벨쓰기
                label.text = "-입력된 데이터-\n";
                for (int i = 0; i < targetQuestMissionObject.questBookID.Length; i++)
                {
                    label.text += i + 1 + ". 퀘스트북ID : " + targetQuestMissionObject.questBookID[i] + " / 퀘스트이름 : " + targetQuestMissionObject.questName[i] + " / 미션타입 : " + targetQuestMissionObject.questMissionType[i].ToString() + " / 미션ID : " + targetQuestMissionObject.missionID[i];
                    if (i != targetQuestMissionObject.questBookID.Length)
                    {
                        label.text += "\n";
                    }
                }
            });
            syncDataButton.text = "데이터동기화";
            rootElement.Add(syncDataButton);

            //설정구역 추가 로드
            errorText = "";
            for (int i = 0; i < targetQuestMissionObject.questBookID.Length; i++)
            {
                OrderQuestMissionData orderQuestMissionData = new OrderQuestMissionData();
                orderQuestMissionData.questBookID = targetQuestMissionObject.questBookID[i];
                orderQuestMissionData.questName = targetQuestMissionObject.questName[i];
                orderQuestMissionData.questMissionType = targetQuestMissionObject.questMissionType[i];
                orderQuestMissionData.missionID = targetQuestMissionObject.missionID[i];
                QuestMissionTargetField questMissionTargetField = new QuestMissionTargetField(questMissionTargetFieldList, rootElement, questDBData, true, orderQuestMissionData);
            }
            if (checkError)
            {
                EditorUtility.DisplayDialog("집어넣을 값에 문제가 있으니 초기화됨", errorText, "OK");
                checkError = false;
            }
            return rootElement;
        }

        class QuestMissionTargetField
        {
            private VisualElement parentElement;//부모
            private PropertyField propertyField;//바닥

            private PopupField<string> questBookIDField;
            private List<string> questBookIDList;
            private PopupField<string> questNameIDField;
            private List<string> questNameList;
            private EnumField questMissionTypeField;
            private PopupField<string> missionIDField;
            private List<string> missionIDList;
            private Button deleteButton;

            //캐싱
            private QuestDBObjectScript questDBData;
            private QuestBookData questBookData;
            private OrderQuestMissionData orderQuestMissionData;

            /// <summary>
            /// 초기화
            /// </summary>
            /// <param name="questMissionTargetFieldList"></param>
            /// <param name="parentElement"></param>
            /// <param name="deleteAction"></param>
            public QuestMissionTargetField(List<QuestMissionTargetField> questMissionTargetFieldList, VisualElement parentElement, QuestDBObjectScript questDBData, bool isLoad = false, OrderQuestMissionData loadData = null)
            {
                questBookIDList = new List<string>();
                questNameList = new List<string>();
                missionIDList = new List<string>();
                orderQuestMissionData = new OrderQuestMissionData();
                this.questDBData = questDBData;

                if (!questMissionTargetFieldList.Contains(this))
                {
                    questMissionTargetFieldList.Add(this);
                }

                this.parentElement = parentElement;
                propertyField = new PropertyField();
                //propertyField.style.borderColor = Color.gray;
                propertyField.style.borderBottomColor = Color.gray;
                propertyField.style.borderLeftColor = Color.gray;
                propertyField.style.borderRightColor = Color.gray;
                propertyField.style.borderTopColor = Color.gray;
                propertyField.style.borderTopWidth = 2;
                propertyField.style.borderBottomWidth = 2;
                propertyField.style.borderRightWidth = 1;
                propertyField.style.borderLeftWidth = 1;

                deleteButton = new Button();
                deleteButton.Add(new Button(() => { RemoveQuestMissionTargetDataField(questMissionTargetFieldList); }) { text = "퀘스트북데이터삭제" });

                questBookIDField = GetQuestBookIDField(propertyField);
                questNameIDField = GetQuestNameField(propertyField);
                questMissionTypeField = GetQuestMissionTypeField(propertyField);
                missionIDField = GetMissionIDField(propertyField);

                //로드체크
                if (isLoad)
                {
                    //데이터캐싱
                    orderQuestMissionData.questBookID = loadData.questBookID;
                    orderQuestMissionData.questName = loadData.questName;
                    orderQuestMissionData.questMissionType = loadData.questMissionType;
                    orderQuestMissionData.missionID = loadData.missionID;

                    //새로고침
                    string temp = "None";

                    //초기화    
                    for (int i = 0; i < questDBData.questBookDataArray.Length; i++)
                    {
                        if (questDBData.questBookDataArray[i].questBookIDKey == loadData.questBookID)
                        {
                            temp = questDBData.questBookDataArray[i].questBookIDKey + "_제목:" + questDBData.questBookDataArray[i].questBookTitle;
                            break;
                        }
                    }
                    try
                    {   
                        questBookIDField.value = temp;
                        RefreshSearchQuestName();
                        questNameIDField.value = loadData.questName;
                        questMissionTypeField.value = loadData.questMissionType;
                        RefreshSearchMissionID();
                        missionIDField.value = loadData.missionID;
                    }
                    catch (System.Exception e)
                    {
                        checkError = true;
                        errorText += e.Message + "\n";
                      
                        questBookIDField.value = temp;
                        RefreshSearchQuestName();
                        questNameIDField.value = questNameList[0];
                        questMissionTypeField.value = loadData.questMissionType;
                        RefreshSearchMissionID();
                        missionIDField.value = missionIDList[0];

                        //데이터캐싱                        
                        orderQuestMissionData.questName = questNameList[0];
                        orderQuestMissionData.questMissionType = loadData.questMissionType;
                        orderQuestMissionData.missionID = missionIDList[0];
                    }
                }
                propertyField.Add(deleteButton);
                parentElement.Add(propertyField);
            }

            public void RemoveQuestMissionTargetDataField(List<QuestMissionTargetField> questMissionTargetFieldList)
            {
                questMissionTargetFieldList.Remove(this);
                propertyField.Remove(questBookIDField);
                propertyField.Remove(questNameIDField);
                propertyField.Remove(questMissionTypeField);
                propertyField.Remove(missionIDField);
                propertyField.Remove(deleteButton);
                parentElement.Remove(propertyField);

                questBookData = null;
                questBookData = null;
                orderQuestMissionData = null;
            }
            private PopupField<string> GetQuestBookIDField(VisualElement visualElement)
            {
                //검색부분
                RefreshSearchQuestBookID();

                PopupField<string> tempField = new PopupField<string>("QuestBookID", questBookIDList, 0);
                string temp = questBookIDList[0].Split('_')[0];
                orderQuestMissionData.questBookID = temp;
                tempField.RegisterValueChangedCallback(evt =>
                {
                    temp = evt.newValue.Split('_')[0];
                    orderQuestMissionData.questBookID = temp;
                    //이름필드를 첫번째걸로 바꿈
                    RefreshSearchQuestName();
                    questNameIDField.value = questNameList[0];
                    //미션필드를 첫번째걸로 바꿈
                    RefreshSearchMissionID();
                    missionIDField.value = missionIDList[0];
                });
                //tempField.value = questBookIDList[0];
                visualElement.Add(tempField);
                return tempField;
            }

            private PopupField<string> GetQuestNameField(VisualElement visualElement)
            {
                //검색부분
                RefreshSearchQuestName();

                PopupField<string> tempField = new PopupField<string>("퀘스트이름", questNameList, 0);
                orderQuestMissionData.questName = questNameList[0];
                tempField.RegisterValueChangedCallback(evt =>
                {
                    orderQuestMissionData.questName = evt.newValue;
                    //미션필드를 첫번째걸로 바꿈
                    RefreshSearchMissionID();
                    missionIDField.value = missionIDList[0];
                });
                visualElement.Add(tempField);
                return tempField;
            }

            private EnumField GetQuestMissionTypeField(VisualElement visualElement)
            {
                var tempField = new EnumField("퀘스트미션타입", orderQuestMissionData.questMissionType);
                tempField.RegisterValueChangedCallback(evt =>
                {
                    orderQuestMissionData.questMissionType = (QuestMissionType)evt.newValue;
                    //미션필드를 첫번째걸로 바꿈
                    RefreshSearchMissionID();
                    missionIDField.value = missionIDList[0];
                });
                visualElement.Add(tempField);
                return tempField;
            }

            private PopupField<string> GetMissionIDField(VisualElement visualElement)
            {
                //검색부분
                RefreshSearchMissionID();

                PopupField<string> tempField = new PopupField<string>("MissionID", missionIDList, 0);
                orderQuestMissionData.missionID = missionIDList[0];
                tempField.RegisterValueChangedCallback(evt =>
                {
                    orderQuestMissionData.missionID = evt.newValue;
                });
                visualElement.Add(tempField);
                return tempField;
            }

            /// <summary>
            /// 퀘스트DB를 보고 퀘스트북을 세팅
            /// </summary>
            private void RefreshSearchQuestBookID()
            {
                questBookIDList.Clear();
                if (questDBData != null)
                {
                    for (int i = 0; i < questDBData.questBookDataArray.Length; i++)
                    {
                        questBookIDList.Add(questDBData.questBookDataArray[i].questBookIDKey + "_제목:" + questDBData.questBookDataArray[i].questBookTitle);
                    }
                    questBookIDList.Sort();
                    questBookIDList.Insert(0, "None");
                }
                else
                {
                    questBookIDList.Insert(0, "QuestDBObjectScript not in ResourceFolder");
                }
            }

            /// <summary>
            /// 참조된 퀘스트북을 보고 퀘스트이름을 세팅
            /// </summary>
            private void RefreshSearchQuestName()
            {
                questNameList.Clear();
                if (string.IsNullOrEmpty(orderQuestMissionData.questBookID))
                {
                    questNameList.Insert(0, "questBookID string varrible not find");
                }
                else
                {
                    for (int i = 0; i < questDBData.questBookDataArray.Length; i++)
                    {
                        if (questDBData.questBookDataArray[i].questBookIDKey == orderQuestMissionData.questBookID)
                        {
                            questBookData = questDBData.questBookDataArray[i];
                            break;
                        }
                    }
                }

                if (questBookData != null)
                {
                    for (int i = 0; i < questBookData.questNodeDataArray.Length; i++)
                    {
                        questNameList.Add(questBookData.questNodeDataArray[i].questName);
                    }
                    questNameList.Sort();
                    questNameList.Insert(0, "None selectQuestName");
                }
                else
                {
                    questNameList.Insert(0, "QuestBookData not in QuestDBData");
                }
            }

            /// <summary>
            /// 퀘스트이름을 설정하면 세팅된 퀘스트노드와 미션타입을 보고 세팅
            /// </summary>
            private void RefreshSearchMissionID()
            {
                missionIDList.Clear();
                QuestNodeData questNodeData = null;

                if (questBookData == null)
                {
                    missionIDList.Add("선택된 퀘스트북 데이터가 없습니다.");
                }
                else
                {
                    for (int j = 0; j < questBookData.questNodeDataArray.Length; j++)
                    {
                        if (questBookData.questNodeDataArray[j].questName == orderQuestMissionData.questName)
                        {
                            questNodeData = questBookData.questNodeDataArray[j];
                            break;
                        }
                    }

                    if (questNodeData != null)
                    {
                        switch (orderQuestMissionData.questMissionType)
                        {
                            case QuestMissionType.Dialogue:
                                if (questNodeData.dialogueMissionDataArray.Length == 0)
                                {
                                    missionIDList.Add("해당되는 대화미션데이터가 없습니다");
                                }
                                else
                                {
                                    for (int i = 0; i < questNodeData.dialogueMissionDataArray.Length; i++)
                                    {
                                        missionIDList.Add(questNodeData.dialogueMissionDataArray[i].actorInfoData.actorName + "_TalkActor");
                                    }
                                }
                                break;
                            case QuestMissionType.Battle:
                                if (questNodeData.battleMissionDataArray.Length == 0)
                                {
                                    missionIDList.Add("해당되는 전투미션데이터가 없습니다");
                                }
                                else
                                {
                                    for (int i = 0; i < questNodeData.battleMissionDataArray.Length; i++)
                                    {
                                        missionIDList.Add(questNodeData.battleMissionDataArray[i].missionTargetID + "_DestroyTarget");
                                    }
                                }
                                break;
                            case QuestMissionType.Survive:
                                if (questNodeData.surviveMissionDataArray.Length == 0)
                                {
                                    missionIDList.Add("해당되는 생존미션데이터가 없습니다");
                                }
                                else
                                {
                                    for (int i = 0; i < questNodeData.surviveMissionDataArray.Length; i++)
                                    {
                                        missionIDList.Add(questNodeData.surviveMissionDataArray[i].missionTargetID + "_SurviveTarget");
                                    }
                                }
                                break;
                            case QuestMissionType.AttractMove:
                                if (questNodeData.attractMoveMissionDataArray.Length == 0)
                                {
                                    missionIDList.Add("해당되는 유인이동미션데이터가 없습니다");
                                }
                                else
                                {
                                    for (int i = 0; i < questNodeData.attractMoveMissionDataArray.Length; i++)
                                    {
                                        AttractMoveMissionData tempData = questNodeData.attractMoveMissionDataArray[i];
                                        string missionID;
                                        if (tempData.isUseTargetUnitStat)
                                        {
                                            if (tempData.unitStatusData == null)
                                            {
                                                missionID = tempData.unitTeamType.ToString() + "_unitStatDataIsNull";
                                            }
                                            else
                                            {
                                                missionID = tempData.unitTeamType.ToString() + "_" + tempData.unitStatusData.objectName;
                                            }
                                        }
                                        else
                                        {
                                            missionID = tempData.unitTeamType.ToString();
                                        }
                                        missionIDList.Add(missionID);
                                    }
                                }
                                break;
                            case QuestMissionType.ItemGather:
                                if (questNodeData.itemGatherMissionDataArray.Length == 0)
                                {
                                    missionIDList.Add("해당되는 아이템모으기미션데이터가 없습니다");
                                }
                                else
                                {
                                    for (int i = 0; i < questNodeData.itemGatherMissionDataArray.Length; i++)
                                    {
                                        missionIDList.Add(questNodeData.itemGatherMissionDataArray[i].itemData.objectName + "_ItemGather");
                                    }
                                }
                                break;
                            case QuestMissionType.ArriveMap:
                                if (questNodeData.arriveMapMissionDataArray.Length == 0)
                                {
                                    missionIDList.Add("해당되는 맵도착미션데이터가 없습니다");
                                }
                                else
                                {
                                    for (int i = 0; i < questNodeData.arriveMapMissionDataArray.Length; i++)
                                    {
                                        missionIDList.Add(questNodeData.arriveMapMissionDataArray[i].mapMarkerData.objectName + "_Arrive");
                                    }
                                }
                                break;
                        }
                    }
                    else
                    {
                        missionIDList.Insert(0, "None QuestNodeData");
                    }
                }
            }

            public OrderQuestMissionData GetOrderQuestMissionData()
            {
                return orderQuestMissionData;
            }
        }
    }
}
#endif
#endif
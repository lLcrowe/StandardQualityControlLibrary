#if Doozy

using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using lLCroweTool.QuestSystem;
using lLCroweTool.DialogueSystem;

namespace lLCroweTool.QC.EditorOnly
{
    public class QuestBookDataNodeGraphView : GraphView
    {
        public readonly Vector2 defaultNodeSize = new Vector2(300, 200);
        public ScrollView scrollView;
        public List<RewardDataField> questBookRewardDataFieldList = new List<RewardDataField>();

        /// <summary>
        /// UI 그래프뷰 생성자//건들거 없음
        /// </summary>
        public QuestBookDataNodeGraphView()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            //기초 박스구조
            //AddElement(GenerateEntryPointNode());//진입점 제작
        }

        //노드연결 관련 함수//건들거 없음
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            //인풋아웃풋 노드에 따라 연결할수 있는걸 결정하게 만들어줌//위에 있는것보다 더 좋음
            var compatiblePorts = new List<Port>();

            foreach (var port in ports.ToList())
            {
                // 포트가 시작이면 넘김
                if (startPort.node == port.node) continue;

                // 인풋은 인풋, 아웃풋은 아웃풋일시 넘김
                if (startPort.direction == port.direction) continue;

                // 다른포트와는 연결안되게
                if (startPort.portType != port.portType) continue;

                compatiblePorts.Add(port);
            }

            return compatiblePorts;
        }

        /// <summary>
        /// 포트 생성함수
        /// </summary>
        /// <param name="_node">포트를 추가시킬 노드</param>
        /// <param name="nodeDirection">방향</param>
        /// <param name="capacity">포트연결용량 </param>
        /// <returns>제작된 포트</returns>
        private Port GeneratePort(QuestUINode node, Direction nodeDirection, Port.Capacity capacity = Port.Capacity.Single)
        {
            //return _node.InstantiatePort(Orientation.Horizontal, nodeDirection, capacity, typeof(float));//임의의 타입
            Port port = Port.Create<Edge>(Orientation.Horizontal, nodeDirection, capacity, typeof(Port));//임의의 타입
            switch (nodeDirection)
            {
                case Direction.Input:
                    node.inputContainer.Add(port);
                    break;
                case Direction.Output:
                    node.outputContainer.Add(port);
                    break;
            }
            return port;
        }


        /// <summary>
        /// 진입점 노드생성 
        /// </summary>
        /// <returns>생성된 진입점</returns>
        public QuestUINode GenerateEntryPointNode()
        {
            var node = new QuestUINode
            {
                title = "Start(진입점)",//노드 박스이름
                GUID = Guid.NewGuid().ToString(),//임의의 문자열 생성
                nodeName = "StartNode",//텍스트이름
                //entryPoint = true,//진입점 여부
                nodeType = QuestNodeType.StartPoint,
            };


            //아웃풋포트추가
            var port = GeneratePort(node, Direction.Output);
            port.name = "아웃풋";
            port.portName = "시작포트";

            node.RefreshExpandedState();
            node.RefreshPorts();

            //포지션정해줌
            node.SetPosition(new Rect(x: 100, y: 200, width: 100, height: 200));
            return node;
        } /// <summary>
          /// 중간노드 제작
          /// </summary>
          /// <param name="nodeName"></param>
        public QuestUINode CreateMiddeNode(string _nodeName, Vector2 _pos, QuestNodeType _nodeType)
        {
            QuestUINode node = null;
            Port port = null;
            switch (_nodeType)
            {
                case QuestNodeType.QuestNode:
                    //대화노드
                    //인풋1개
                    //아웃1개
                    QuestNode questNode = new QuestNode
                    {
                        title = _nodeName,//노드 박스이름
                        GUID = Guid.NewGuid().ToString(),//임의의 문자열 생성
                        nodeName = _nodeName,//텍스트이름
                    };
                    node = questNode;

                    var body1 = new VisualElement();
                    var body2 = new VisualElement();
                    node.Add(body1);
                    node.Add(body2);
                    body2.style.flexDirection = FlexDirection.Row;
                    body2.style.alignItems = Align.FlexEnd;


                    //필드 추가
                    var questNameTextField = new TextField("퀘스트 이름");
                    DialogueDBWindowEditor.InitTextField(questNameTextField);
                    questNode.questNameTextField = questNameTextField;
                    questNameTextField.RegisterValueChangedCallback(evt =>
                    {
                        questNode.questName = evt.newValue;
                        QuestDBWindowEditor.saveCheck = true;
                    });

                    var questContentInfoTextField = new TextField("퀘스트정보 내용");
                    DialogueDBWindowEditor.InitTextField(questContentInfoTextField);
                    questNode.questContentInfoTextField = questContentInfoTextField;
                    questContentInfoTextField.RegisterValueChangedCallback(evt =>
                    {
                        questNode.questContentInfo = evt.newValue;
                        QuestDBWindowEditor.saveCheck = true;
                    });

                    var questHUDTextContentTextField = new TextField("퀘스트HUD에 보여질 내용");
                    DialogueDBWindowEditor.InitTextField(questHUDTextContentTextField);
                    questNode.questHUDContentTextField = questHUDTextContentTextField;
                    questHUDTextContentTextField.RegisterValueChangedCallback(evt =>
                    {
                        questNode.questHUDContent = evt.newValue;
                        QuestDBWindowEditor.saveCheck = true;
                    });

                    var isUseTimerBoolField = new Toggle("타이머 사용여부");
                    questNode.isUseTimerField = isUseTimerBoolField;
                    isUseTimerBoolField.RegisterValueChangedCallback(boolEvt =>
                    {
                        questNode.isUseTimer = boolEvt.newValue;
                    });
                    questNode.isUseTimer = isUseTimerBoolField.value;



                    var timeIntField = new IntegerField("타이머");
                    questNode.timeField = timeIntField;
                    timeIntField.RegisterValueChangedCallback(floatEvt =>
                    {
                        questNode.time = floatEvt.newValue;
                        QuestDBWindowEditor.saveCheck = true;
                    });
                    questNode.time = timeIntField.value;


                    var dialogueMissionView = new PropertyField();
                    var dialogueNoticeLabel = new Label();
                    questNode.dialogueMissionNoticeLabel = dialogueNoticeLabel;
                    var dialogueMissionViewScroll = GetScrollView();
                    questNode.dialogueMissionScrollView = dialogueMissionViewScroll;

                    dialogueMissionView.Add(dialogueNoticeLabel);
                    dialogueMissionView.Add(dialogueMissionViewScroll);
                    dialogueMissionView.Add(new Button(() =>
                    {
                        DialogueMissionData dialogueMissionData = new DialogueMissionData();
                        DialogueMissionDataField dialogueMissionDataField = new DialogueMissionDataField(dialogueMissionData, questNode.dialogueMissionDataFieldList, dialogueMissionViewScroll, questNode.dialogueMissionNoticeLabel);
                        dialogueMissionDataField.CheckOverLap(questNode.dialogueMissionDataFieldList, questNode.dialogueMissionNoticeLabel);
                    })
                    { text = "Add DialogueMission" });

                    var battleMissionView = new PropertyField();
                    var battleNoticeLabel = new Label();
                    questNode.battleMissionNoticeLabel = battleNoticeLabel;
                    var battleMissionViewScroll = GetScrollView();
                    questNode.battleMissionScrollView = battleMissionViewScroll;

                    battleMissionView.Add(battleNoticeLabel);
                    battleMissionView.Add(battleMissionViewScroll);
                    battleMissionView.Add(new Button(() =>
                    {
                        BattleMissionData battleMissionData = new BattleMissionData();
                        battleMissionData.missionTargetID = questNode.battleMissionDataFieldList.Count.ToString();
                        BattleMissionDataField battleMissionDataField = new BattleMissionDataField(battleMissionData, questNode.battleMissionDataFieldList, battleMissionViewScroll, questNode.battleMissionNoticeLabel);
                        battleMissionDataField.CheckOverLap(questNode.battleMissionDataFieldList, questNode.battleMissionNoticeLabel);
                    })
                    { text = "Add BattleMission" });


                    var surviveMissionView = new PropertyField();
                    var surviveNoticeLabel = new Label();
                    questNode.surviveMissionNoticeLabel = surviveNoticeLabel;
                    var surviveMissionViewScroll = GetScrollView();
                    questNode.surviveMissionScrollView = surviveMissionViewScroll;

                    surviveMissionView.Add(surviveNoticeLabel);
                    surviveMissionView.Add(surviveMissionViewScroll);
                    surviveMissionView.Add(new Button(() =>
                    {
                        SurviveMissionData surviveMissionData = new SurviveMissionData();
                        surviveMissionData.missionTargetID = questNode.surviveMissionDataFieldList.Count.ToString();
                        SurviveMissionDataField surviveMissionDataField = new SurviveMissionDataField(surviveMissionData, questNode.surviveMissionDataFieldList, surviveMissionViewScroll, questNode.surviveMissionNoticeLabel);
                        surviveMissionDataField.CheckOverLap(questNode.surviveMissionDataFieldList, questNode.surviveMissionNoticeLabel);
                    })
                    { text = "Add SurviveMission" });


                    var attractMoveMissionView = new PropertyField();
                    var attractNoticeLabel = new Label();
                    questNode.attractMoveMissionNoticeLabel = attractNoticeLabel;
                    var attractMissionViewScroll = GetScrollView();
                    questNode.attractMoveMissionScrollView = attractMissionViewScroll;

                    attractMoveMissionView.Add(attractNoticeLabel);
                    attractMoveMissionView.Add(attractMissionViewScroll);
                    attractMoveMissionView.Add(new Button(() =>
                    {
                        AttractMoveMissionData attractMoveMissionData = new AttractMoveMissionData();
                        AttractMoveMissionDataField attractMoveMissionDataField = new AttractMoveMissionDataField(attractMoveMissionData, questNode.attractMoveMissionDataFieldList, attractMissionViewScroll, questNode.attractMoveMissionNoticeLabel);
                        attractMoveMissionDataField.CheckOverLap(questNode.attractMoveMissionDataFieldList, questNode.attractMoveMissionNoticeLabel);
                    })
                    { text = "Add AttractMoveMission" });


                    var itemMissionView = new PropertyField();
                    var itemNoticeLabel = new Label();
                    questNode.itemMissionNoticeLabel = itemNoticeLabel;
                    var itemMissionViewScroll = GetScrollView();
                    questNode.itemMissionScrollView = itemMissionViewScroll;

                    itemMissionView.Add(itemNoticeLabel);
                    itemMissionView.Add(itemMissionViewScroll);
                    itemMissionView.Add(new Button(() =>
                    {
                        ItemGatherMissionData itemGatherMissionData = new ItemGatherMissionData();
                        ItemGatherMissionDataField itemGatherMissionDataField = new ItemGatherMissionDataField(itemGatherMissionData, questNode.itemGatherDataFieldList, itemMissionViewScroll, questNode.itemMissionNoticeLabel);
                        itemGatherMissionDataField.CheckOverLap(questNode.itemGatherDataFieldList, questNode.itemMissionNoticeLabel);
                    })
                    { text = "Add ItemGatherMission" });

                    var arriveMapMissionView = new PropertyField();
                    var arriveNoticeLabel = new Label();
                    questNode.arriveMissionNoticeLabel = arriveNoticeLabel;
                    var arriveMapMissionViewScroll = GetScrollView();
                    questNode.arriveMapMissionScrollView = arriveMapMissionViewScroll;

                    arriveMapMissionView.Add(arriveNoticeLabel);
                    arriveMapMissionView.Add(arriveMapMissionViewScroll);
                    arriveMapMissionView.Add(new Button(() =>
                    {
                        ArriveMapMissionData arriveMapMissionData = new ArriveMapMissionData();
                        ArriveMapMissionDataField arriveMapMissionDataField = new ArriveMapMissionDataField(arriveMapMissionData, questNode.arriveMapMissionDataFieldList, arriveMapMissionViewScroll, questNode.arriveMissionNoticeLabel);
                        arriveMapMissionDataField.CheckOverLap(questNode.arriveMapMissionDataFieldList, questNode.arriveMissionNoticeLabel);
                    })
                    { text = "Add ArriveMapMission" });

                    body1.Add(questNameTextField);
                    body1.Add(questContentInfoTextField);
                    body1.Add(questHUDTextContentTextField);
                    body1.Add(isUseTimerBoolField);
                    body1.Add(timeIntField);

                    body2.Add(dialogueMissionView);
                    body2.Add(battleMissionView);
                    body2.Add(surviveMissionView);
                    body2.Add(attractMoveMissionView);
                    body2.Add(itemMissionView);
                    body2.Add(arriveMapMissionView);
                

                    //인풋포트추가
                    port = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
                    port.name = "퀘스트노드인풋";
                    port.portName = "Input_Multi";

                    //아웃포트추가
                    //True
                    port = GeneratePort(node, Direction.Output, Port.Capacity.Single);
                    port.name = "퀘스트노드True아웃풋";
                    port.portName = "Output_Single_True";

                    //False
                    port = GeneratePort(node, Direction.Output, Port.Capacity.Single);
                    port.name = "퀘스트노드False아웃풋";
                    port.portName = "Output_Single_False";
                    break;
                case QuestNodeType.DialogueEvent:
                    //대화링크노드
                    //인풋1개
                    //아웃풋0개
                    DialogueEventNode dialogueEventNode = new DialogueEventNode
                    {
                        title = _nodeName,//노드 박스이름
                        GUID = Guid.NewGuid().ToString(),//임의의 문자열 생성
                        nodeName = _nodeName,//텍스트이름

                    };
                    node = dialogueEventNode;


                    //씬에서 작동되게 변경
                    //여기서는 이름만
                    //var label = new Label("게임월드이벤트타입");
                    //node.Add(label);

                    //새로고침체크                    
                    DialogueDBObjectScript dialogueDBData = QuestDBWindowEditor.GetQuestDBData().dialogueDBData;
                    List<string> dialogueDataList = new List<string>();
                    if (dialogueDBData == null)
                    {
                        dialogueDataList.Add("대화DB가 존재하지않음");
                    }
                    else
                    {

                        for (int i = 0; i < dialogueDBData.dialogueDataArray.Length; i++)
                        {
                            dialogueDataList.Add(dialogueDBData.dialogueDataArray[i].dialogueIDKey);
                        }
                    }

                    var textLabel1 = new Label("대화이벤트ID");
                    var dialogueIDField = new PopupField<string>(dialogueDataList, dialogueDataList[0]);
                    dialogueEventNode.dialogueIDField = dialogueIDField;
                    dialogueIDField.RegisterValueChangedCallback(evt =>
                    {
                        //dialogueEventNode.dialogueID = evt.newValue;
                        QuestDBWindowEditor.saveCheck = true;

                        //새로고침체크
                        if (dialogueDBData != null)
                        {
                            dialogueDataList.Clear();
                            for (int i = 0; i < dialogueDBData.dialogueDataArray.Length; i++)
                            {
                                dialogueDataList.Add(dialogueDBData.dialogueDataArray[i].dialogueIDKey);
                            }
                        }
                    });
                    dialogueIDField.index = 0;

                    node.Add(textLabel1);
                    node.Add(dialogueIDField);

                    //인풋포트추가
                    port = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
                    port.name = "대화이벤트인풋";
                    port.portName = "Input_Multi";

                    //아웃포트추가
                    //port = GenratePort(node, Direction.Output, Port.Capacity.Single);
                    //port.name = "대화이벤트아웃풋";
                    //port.portName = "Output_Single";
                    //node.outputContainer.Add(port);
                    break;
                case QuestNodeType.LinkedQuestBook:
                    //링크퀘스트북 노드
                    //인풋1개
                    //아웃풋0개
                    LinkedQuestBookNode linkedQuestBookNode = new LinkedQuestBookNode
                    {
                        title = _nodeName,//노드 박스이름
                        GUID = Guid.NewGuid().ToString(),//임의의 문자열 생성
                        nodeName = _nodeName,//텍스트이름
                    };
                    node = linkedQuestBookNode;

                    //새로고침체크
                    List<QuestBookData> questBookDataList = QuestDBWindowEditor.GetQuestBookDataList();
                    List<string> questBookDataIDList = new List<string>();
                    for (int i = 0; i < questBookDataList.Count; i++)
                    {
                        questBookDataIDList.Add(questBookDataList[i].questBookIDKey);
                    }
                    if (questBookDataIDList.Count == 0)
                    {
                        questBookDataIDList.Add("퀘스트데이터가 존재하지않음");
                    }

                    var textLabel2 = new Label("퀘스트북ID");
                    var questBookIDField = new PopupField<string>(questBookDataIDList, questBookDataIDList[0]);
                    linkedQuestBookNode.questBookIDField = questBookIDField;
                    
                    questBookIDField.RegisterValueChangedCallback(evt =>
                    {
                        //linkedQuestBookNode.questBookID = evt.newValue;
                        QuestDBWindowEditor.saveCheck = true;

                        //새로고침체크
                        questBookDataIDList.Clear();
                        for (int i = 0; i < questBookDataList.Count; i++)
                        {
                            questBookDataIDList.Add(questBookDataList[i].questBookIDKey);
                        }
                        if (questBookDataIDList.Count == 0)
                        {
                            questBookDataIDList.Add("퀘스트데이터가 존재하지않음");
                        }
                    });
                    questBookIDField.index = 0;

                    node.Add(textLabel2);
                    node.Add(questBookIDField);

                    //인풋포트추가
                    port = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
                    port.name = "연계이벤트인풋";
                    port.portName = "Input_Multi";

                    //아웃포트추가
                    //port = GenratePort(node, Direction.Output, Port.Capacity.Single);
                    //port.name = "연계이벤트아웃풋";
                    //port.portName = "Output_Single";
                    //node.outputContainer.Add(port);
                    break;
                case QuestNodeType.RandomBranch:
                    //랜덤분기 노드
                    //인풋1개
                    //아웃X개
                    RandomBranchQuestNode randomBranchNode = new RandomBranchQuestNode
                    {
                        title = _nodeName,//노드 박스이름
                        GUID = Guid.NewGuid().ToString(),//임의의 문자열 생성
                        nodeName = _nodeName,//텍스트이름
                    };
                    node = randomBranchNode;

                    //씬에서 작동되게 변경
                    //여기서는 표시만  


                    //인풋포트추가
                    port = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
                    port.name = "랜덤분기인풋";
                    port.portName = "Input_Multi";

                    //아웃포트추가
                    //새로운 포트추가 버튼
                    var button1 = new Button(() => { AddChoicePort(node, ""); })
                    {
                        text = "Add Choice"
                    };
                    button1.text = "신규포트 추가";
                    node.titleContainer.Add(button1);

                    break;
            }

            node.nodeType = _nodeType;
            node.RefreshExpandedState();
            node.RefreshPorts();
            node.SetPosition(new Rect(_pos, defaultNodeSize));
            return node;
        }

        //노드제작
        public QuestUINode CreateNode(string _nodeName, Vector2 _pos, QuestNodeType _nodeType)
        {
            QuestUINode node = CreateMiddeNode(_nodeName, _pos, _nodeType);
            AddElement(node);
            return node;
        }

        //선택할수 있는 포트 추가
        //
        public void AddChoicePort(QuestUINode nodeCache, string overriddenPortName)
        {
            Port generatedPort = GeneratePort(nodeCache, Direction.Output, Port.Capacity.Single);

            int outputPortCount = nodeCache.outputContainer.Query("connector").ToList().Count();
            string outputPortName = string.IsNullOrEmpty(overriddenPortName)
                   ? $"Option {outputPortCount + 1}"
                   : overriddenPortName;
            Label portLabel = generatedPort.contentContainer.Q<Label>("type");
            generatedPort.contentContainer.Remove(portLabel);


            //포트순서
            var lavel = new Label($"Port -{outputPortCount + 1}- .");
            generatedPort.contentContainer.Add(lavel);

            //포트삭제 버튼 추가
            var deleteButton = new Button(() => RemovePort(nodeCache, generatedPort))
            {
                text = "X"
            };
            generatedPort.contentContainer.Add(deleteButton);
            generatedPort.name = "랜덤분기아웃풋";
            generatedPort.portName = outputPortName;
            nodeCache.RefreshPorts();
            nodeCache.RefreshExpandedState();
        }

        //포트삭제
        private void RemovePort(QuestUINode node, Port socket)
        {
            var targetEdge = edges.ToList()
                .Where(x => x.output.portName == socket.portName && x.output.node == socket.node);
            if (targetEdge.Any())
            {
                var edge = targetEdge.First();
                edge.input.Disconnect(edge);
                RemoveElement(targetEdge.First());
            }

            node.outputContainer.Remove(socket);
            node.RefreshPorts();
            node.RefreshExpandedState();
        }

        private ScrollView GetScrollView()
        {
            var scrollView = new ScrollView();
            scrollView.style.height = 200;
            scrollView.showVertical = true;
            scrollView.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            return scrollView;
        }
    }
}
#endif
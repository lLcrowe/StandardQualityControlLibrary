#if Doozy
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using lLCroweTool.DialogueSystem;
using lLCroweTool.QuestSystem;

namespace lLCroweTool.QC.EditorOnly
{ 
    public class DialogueDataNodeGraphView : GraphView
    {
        public readonly Vector2 defaultNodeSize = new Vector2(300, 200);

        public List<CustomPropertyBool> customPropertyBoolList = new List<CustomPropertyBool>();
        public List<CustomPropertyFloat> customPropertyFloatList = new List<CustomPropertyFloat>();
        public List<CustomPropertyInt> customPropertyIntList = new List<CustomPropertyInt>();
        public List<CustomPropertyString> customPropertyStringList = new List<CustomPropertyString>();

        public ScrollView scrollView = null;
        
        /// <summary>
        /// UI 그래프뷰 생성자//건들거 없음
        /// </summary>
        public DialogueDataNodeGraphView()
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
        //public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        //{   
        //    var compatiblePorts = new List<Port>();
        //    var startPortView = startPort;

        //    ports.ForEach((port) =>
        //    {
        //        var portView = port;
        //        if (startPortView != portView && startPortView.node != portView.node)
        //            compatiblePorts.Add(port);
        //    });

        //    return compatiblePorts;
        //}
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            //포트를 드래그했을시 작동됨
            //인풋아웃풋 노드에 따라 연결할수 있는걸 결정하게 만들어줌//위에 있는것보다 더 좋음
            List<Port> compatiblePorts = new List<Port>();

            foreach (Port port in ports.ToList())
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
        /// <param name="node">포트를 추가시킬 노드</param>
        /// <param name="nodeDirection">방향</param>
        /// <param name="capacity">포트연결용량 </param>
        /// <returns>제작된 포트</returns>
        private Port GeneratePort(DialogueUINode node, Direction nodeDirection, Port.Capacity capacity = Port.Capacity.Single)
        {
            //return _node.InstantiatePort(Orientation.Horizontal, nodeDirection, capacity, typeof(bool));//임의의 타입
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
        public DialogueUINode GenerateEntryPointNode()
        {
            var node = new DialogueUINode
            {
                title = "Start(진입점)",//노드 박스이름
                GUID = Guid.NewGuid().ToString(),//임의의 문자열 생성
                nodeName = "StartNode",//텍스트이름
                //entryPoint = true,//진입점 여부
                nodeType = DialogueNodeType.StartPoint,
            };


            //아웃풋포트추가
            var port = GeneratePort(node, Direction.Output);
            port.name = "아웃풋";
            port.portName = "시작포트";

            node.RefreshExpandedState();
            node.RefreshPorts();

            //포지션정해줌
            node.SetPosition(new Rect(100, 200, 100, 200));
            return node;
        }

        /// <summary>
        /// 중간노드 제작
        /// </summary>
        /// <param name="nodeName">대화UI노드</param>
        public DialogueUINode CreateMiddeNode(string _nodeName, Vector2 _pos, DialogueNodeType _nodeType)
        {
            DialogueUINode node = null;
            Port port = null;
            switch (_nodeType) 
            {
                case DialogueNodeType.Dialogue:
                    //대화노드
                    //인풋1개
                    //아웃1개
                    DialogueNode dialogueNode = new DialogueNode 
                    {
                        title = _nodeName,//노드 박스이름
                        GUID = Guid.NewGuid().ToString(),//임의의 문자열 생성
                        nodeName = _nodeName,//텍스트이름
                    };                    
                    node = dialogueNode;


                    //필드 추가
                    var targetActorInfoDataObjectField = new ObjectField("말하는 액터(인물)정보")
                    {
                        objectType = typeof(ActorInfoObjectScript),
                    };
                    dialogueNode.targetActorInfoDataObjectField = targetActorInfoDataObjectField;
                    targetActorInfoDataObjectField.RegisterValueChangedCallback(evt =>
                    {
                        dialogueNode.targetActorInfoData = (ActorInfoObjectScript)evt.newValue;
                        DialogueDBWindowEditor.saveCheck = true;
                    });

                    var actorShowBeginTypeEnumField = new EnumField("시작시 보여주는 방식", dialogueNode.actorShowBegin);
                    dialogueNode.actorShowBeginTypeEnumField = actorShowBeginTypeEnumField;
                    actorShowBeginTypeEnumField.RegisterValueChangedCallback(evt =>
                    {
                        dialogueNode.actorShowBegin = (ActorShowBeginType)evt.newValue;
                        DialogueDBWindowEditor.saveCheck = true;
                    });

                    var actorShowPosTypeEnumField = new EnumField("시작시 보여줄 위치", dialogueNode.actorShowPos);
                    dialogueNode.actorShowPosTypeEnumField = actorShowPosTypeEnumField;
                    actorShowPosTypeEnumField.RegisterValueChangedCallback(evt =>
                    {
                        dialogueNode.actorShowPos = (ActorShowPos)evt.newValue;
                        DialogueDBWindowEditor.saveCheck = true;
                    });

                    var actorShowEndTypeEnumField = new EnumField("끝날시 보여주는 방식", dialogueNode.actorShowEnd);
                    dialogueNode.actorShowEndTypeEnumField = actorShowEndTypeEnumField;
                    actorShowEndTypeEnumField.RegisterValueChangedCallback(evt =>
                    {
                        dialogueNode.actorShowEnd = (ActorShowEndType)evt.newValue;
                        DialogueDBWindowEditor.saveCheck = true;
                    });

                    var localizeIDTextField = new TextField("로컬라이징ID");
                    DialogueDBWindowEditor.InitTextField(localizeIDTextField);
                    dialogueNode.localizeIDTextField = localizeIDTextField;
                    localizeIDTextField.RegisterValueChangedCallback(evt =>
                    {
                        dialogueNode.localizeID = evt.newValue;
                        DialogueDBWindowEditor.saveCheck = true;
                    });

                    var dialogueContentTextField = new TextField("대화내용");
                    DialogueDBWindowEditor.InitTextField(dialogueContentTextField);
                    dialogueNode.dialogueContentTextField = dialogueContentTextField;
                    dialogueContentTextField.RegisterValueChangedCallback(evt =>
                     {
                         dialogueNode.dialogueContent = evt.newValue;
                         DialogueDBWindowEditor.saveCheck = true;
                     });

                    var animationPortraitMessageTypeEnumField = new EnumField("액터애니메이션이벤트 타입", dialogueNode.animamtionPortraitMessageType);
                    dialogueNode.animationPortraitMessageTypeEnumField = animationPortraitMessageTypeEnumField;
                    animationPortraitMessageTypeEnumField.RegisterValueChangedCallback(evt =>
                    {
                        dialogueNode.animamtionPortraitMessageType = (AnimamtionPortraitMessageType)evt.newValue;
                        DialogueDBWindowEditor.saveCheck = true;
                    });

                    var dialogueChatTypeEnumField = new EnumField("대화방식", dialogueNode.dialogueChatType);
                    dialogueNode.dialogueChatTypeEnumField = dialogueChatTypeEnumField;
                    dialogueChatTypeEnumField.RegisterValueChangedCallback(evt =>
                    {
                        dialogueNode.dialogueChatType = (DialogueChatType)evt.newValue;
                        DialogueDBWindowEditor.saveCheck = true;
                    });

                    var overideFontAssetObjectField = new ObjectField("덮어쓸 폰트")
                    {
                        objectType = typeof(TMP_FontAsset),
                    };
                    dialogueNode.overideFontAssetObjectField = overideFontAssetObjectField;
                    overideFontAssetObjectField.RegisterValueChangedCallback(evt =>
                    {
                        dialogueNode.overideFontAsset = (TMP_FontAsset)evt.newValue;
                        DialogueDBWindowEditor.saveCheck = true;
                    });

                    var focusObjectObjectField = new ObjectField("대화중에 보여줄 오브젝트")
                    {
                        objectType = typeof(GameObject),
                    };
                    dialogueNode.focusObjectObjectField = focusObjectObjectField;
                    focusObjectObjectField.RegisterValueChangedCallback(evt =>
                    {
                        dialogueNode.focusObject = (GameObject)evt.newValue;
                        DialogueDBWindowEditor.saveCheck = true;
                    });
                   
                    node.Add(targetActorInfoDataObjectField);
                    node.Add(actorShowBeginTypeEnumField);
                    node.Add(actorShowPosTypeEnumField);
                    node.Add(actorShowEndTypeEnumField);
                    node.Add(localizeIDTextField);
                    node.Add(dialogueContentTextField);
                    node.Add(animationPortraitMessageTypeEnumField);                    
                    node.Add(dialogueChatTypeEnumField);
                    node.Add(overideFontAssetObjectField);
                    node.Add(focusObjectObjectField);

                    //인풋포트추가
                    port = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
                    port.name = "대화노드인풋";
                    port.portName = "Input_Multi";

                    //아웃포트추가
                    port = GeneratePort(node, Direction.Output, Port.Capacity.Single);
                    port.name = "대화노드아웃풋";
                    port.portName = "Output_Single";
                    break;
                case DialogueNodeType.SetProperty:
                    //변수세팅 노드
                    //인풋1개
                    //아웃1개
                    SetCustomPropertyNode setCustomPropertyNode = new SetCustomPropertyNode
                    {
                        title = _nodeName,//노드 박스이름
                        GUID = Guid.NewGuid().ToString(),//임의의 문자열 생성
                        nodeName = _nodeName,//텍스트이름
                    };
                    node = setCustomPropertyNode;

                    UpdateSetProperty(setCustomPropertyNode);

                    //인풋포트추가
                    port = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
                    port.name = "변수세팅인풋";
                    port.portName = "Input_Multi";

                    //아웃포트추가
                    port = GeneratePort(node, Direction.Output, Port.Capacity.Single);
                    port.name = "변수세팅아웃풋";
                    port.portName = "Output_Single";
                    break;
                case DialogueNodeType.ConditionBranch:
                    //조건분기 노드
                    //인풋1개
                    //아웃2개
                    ConditionBranchNode conditionBranchNode = new ConditionBranchNode
                    {
                        title = _nodeName,//노드 박스이름
                        GUID = Guid.NewGuid().ToString(),//임의의 문자열 생성
                        nodeName = _nodeName,//텍스트이름
                    };
                    node = conditionBranchNode;

                    UpdateConditionBrach(conditionBranchNode);

                    //인풋포트추가
                    port = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
                    port.name = "조건분기인풋";
                    port.portName = "Input_Multi";

                    //아웃포트추가
                    //True
                    port = GeneratePort(node, Direction.Output, Port.Capacity.Single);
                    port.name = "조건분기True";
                    port.portName = "Output_Single_True";

                    //False
                    port = GeneratePort(node, Direction.Output, Port.Capacity.Single);
                    port.name = "조건분기False";
                    port.portName = "Output_Single_False";
                    break;
                case DialogueNodeType.SelectBranch:
                    //선택분기 노드
                    //인풋1개
                    //아웃X개
                    SelectBranchNode selectBranchNode = new SelectBranchNode
                    {
                        title = _nodeName,//노드 박스이름
                        GUID = Guid.NewGuid().ToString(),//임의의 문자열 생성
                        nodeName = _nodeName,//텍스트이름
                    };
                    node = selectBranchNode;
                    node.style.flexDirection = FlexDirection.Column;                    

                    //인풋포트추가
                    port = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
                    port.name = "선택분기인풋";
                    port.portName = "Input_Multi";

                    //신규
                    var newbutton = new Button(() =>
                    {
                        AddSelectPoint(selectBranchNode);     
                    })
                    {
                        text = "Add SelectPoint"
                    };
                    newbutton.text = "선택지 추가";
                    node.titleContainer.Add(newbutton);
                    break;
                case DialogueNodeType.QuestSelectBranch:
                    //선택분기 노드
                    //인풋1개
                    //아웃X개
                    QuestSelectBranchNode selectQuestBranchNode = new QuestSelectBranchNode
                    {
                        title = _nodeName,//노드 박스이름
                        GUID = Guid.NewGuid().ToString(),//임의의 문자열 생성
                        nodeName = _nodeName,//텍스트이름
                    };
                    node = selectQuestBranchNode;
                    node.style.flexDirection = FlexDirection.Column;

                    //인풋포트추가
                    port = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
                    port.name = "선택퀘스트분기인풋";
                    port.portName = "Input_Multi";

                    //신규
                    var newQuestbutton = new Button(() =>
                    {
                        AddQuestSelectPoint(selectQuestBranchNode);
                    })
                    {
                        text = "Add SelectPoint"
                    };
                    newQuestbutton.text = "퀘스트선택지 추가";
                    node.titleContainer.Add(newQuestbutton);
                    break;
                case DialogueNodeType.RandomBranch:
                    //랜덤분기 노드
                    //인풋1개
                    //아웃X개
                    RandomBranchNode randomBranchNode = new RandomBranchNode
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
                case DialogueNodeType.QuestEvent:
                    //퀘스트이벤트노드
                    //인풋1개
                    //아웃풋2~3개
                    QuestEventNode questEventNode = new QuestEventNode
                    {
                        title = _nodeName,//노드 박스이름
                        GUID = Guid.NewGuid().ToString(),//임의의 문자열 생성
                        nodeName = _nodeName,//텍스트이름
                    };
                    node = questEventNode;


                    //새로고침 체크
                    QuestDBObjectScript questDBData = DialogueDBWindowEditor.GetDialogueDBData().questDBData;
                    List<string> questBookDataIDList = new List<string>();
                    if (questDBData == null)
                    {
                        questBookDataIDList.Add("퀘스트DB가 존재하지않음");
                    }
                    else
                    {
                        for (int i = 0; i < questDBData.questBookDataArray.Length; i++)
                        {
                            questBookDataIDList.Add(questDBData.questBookDataArray[i].questBookIDKey);
                        }
                    }

                    var textLabel2 = new Label("퀘스트북 아이디");
                    var questBookIDField = new PopupField<string>(questBookDataIDList, questBookDataIDList[0]);
                    questEventNode.questBookIDField = questBookIDField;
                    questBookIDField.RegisterValueChangedCallback(evt =>
                    {
                        //questEventNode.questBookID = evt.newValue;
                        DialogueDBWindowEditor.saveCheck = true;

                        //새로고침체크
                        if (questDBData != null)
                        {
                            questBookDataIDList.Clear();
                            for (int i = 0; i < questDBData.questBookDataArray.Length; i++)
                            {
                                questBookDataIDList.Add(questDBData.questBookDataArray[i].questBookIDKey);
                            }
                        }
                    });
                    questBookIDField.index = 0;
                    //questEventNode.questBookID = questBookIDField.value;

                    node.Add(textLabel2);
                    node.Add(questBookIDField);

                    //인풋포트추가
                    port = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
                    port.name = "퀘스트이벤트인풋";
                    port.portName = "Input_Multi";
                    break;
                case DialogueNodeType.LinkedDialogue:
                    //대화링크노드
                    //인풋1개
                    //아웃풋0개
                    LinkedDialogueNode linkedDialogueNode = new LinkedDialogueNode
                    {
                        title = _nodeName,//노드 박스이름
                        GUID = Guid.NewGuid().ToString(),//임의의 문자열 생성
                        nodeName = _nodeName,//텍스트이름

                    };
                    node = linkedDialogueNode;


                    //씬에서 작동되게 변경
                    //여기서는 이름만
                    //var label = new Label("게임월드이벤트타입");
                    //node.Add(label);

                    //새로고침체크                    
                    List<DialogueData> dialogueDataList = DialogueDBWindowEditor.GetDialogueDataList();
                    List<string> dialogueDataIDList = new List<string>();
                    for (int i = 0; i < dialogueDataList.Count; i++)
                    {
                        dialogueDataIDList.Add(dialogueDataList[i].dialogueIDKey);
                    }
                    if (dialogueDataIDList.Count == 0)
                    {
                        dialogueDataIDList.Add("대화데이터가 존재하지않음");
                    }

                    var textLabel3 = new Label("대화이벤트ID");
                    var dialogueIDField = new PopupField<string>(dialogueDataIDList, dialogueDataIDList[0]);
                    linkedDialogueNode.dialogueIDField = dialogueIDField;
                    dialogueIDField.RegisterValueChangedCallback(evt =>
                    {
                        //dialogueEventNode.dialogueID = evt.newValue;
                        QuestDBWindowEditor.saveCheck = true;

                        //새로고침체크
                        dialogueDataIDList.Clear();
                        for (int i = 0; i < dialogueDataList.Count; i++)
                        {
                            dialogueDataIDList.Add(dialogueDataList[i].dialogueIDKey);
                        }
                        if (dialogueDataIDList.Count == 0)
                        {
                            dialogueDataIDList.Add("대화데이터가 존재하지않음");
                        }
                    });
                    dialogueIDField.index = 0;

                    node.Add(textLabel3);
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
            }
            
            node.nodeType = _nodeType;
            node.RefreshExpandedState();
            node.RefreshPorts();
            node.SetPosition(new Rect(_pos, defaultNodeSize));
            return node;
        }

        //노드제작
        public DialogueUINode CreateNode(string _nodeName, Vector2 _pos, DialogueNodeType _nodeType)
        {   
            DialogueUINode node = CreateMiddeNode(_nodeName, _pos, _nodeType);
            AddElement(node);
            return node;
        }

        //선택할수 있는 포트 추가
        public void AddChoicePort(DialogueUINode nodeCache, string overriddenPortName)
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
        private void RemovePort(DialogueUINode node, Port socket)
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

        public void AddSelectPoint(SelectBranchNode selectBranchNode, string overriddenPortName = "")
        {
            int num = selectBranchNode.selectContentList.Count + 1;           
            string outputPortName = string.IsNullOrEmpty(overriddenPortName)
                   ? "선택포트 -" + num.ToString() + "-"
                   : overriddenPortName;

            var textField = new TextField("SelectContent(" + num.ToString() + ")")
            {
                name = string.Empty,
                value = "Talk " + num.ToString()
            };
            DialogueDBWindowEditor.InitTextField(textField);
            textField.multiline = true;

            var localizeTextField = new TextField("ContentLocalizeID")
            {
                name = string.Empty,
            };
            DialogueDBWindowEditor.InitTextField(localizeTextField);

            //노드에 데이터 캐싱
            selectBranchNode.selectContentList.Add(textField);
            selectBranchNode.localizeList.Add(localizeTextField);
            //포트추가
            Port generatedPort = GeneratePort(selectBranchNode, Direction.Output, Port.Capacity.Single);            
            generatedPort.portName = outputPortName;
            generatedPort.name = "선택분기아웃풋";

            //포트삭제 버튼 추가
            var deleteButton = new Button()
            {
                text = "해당선택지 삭제",
            };
            deleteButton.clickable.clicked += () => {
                selectBranchNode.selectContentList.Remove(textField);
                selectBranchNode.localizeList.Remove(localizeTextField);
                selectBranchNode.Remove(textField);
                selectBranchNode.Remove(localizeTextField);
                RemovePort(selectBranchNode, generatedPort);
                selectBranchNode.Remove(deleteButton);
            };

            selectBranchNode.Add(textField);
            selectBranchNode.Add(localizeTextField);
            selectBranchNode.Add(deleteButton);            
            selectBranchNode.RefreshPorts();
            selectBranchNode.RefreshExpandedState();
        }

        public void AddQuestSelectPoint(QuestSelectBranchNode selectQuestBranchNode, string overriddenPortName = "")
        {
            int num = selectQuestBranchNode.questBookIDFieldList.Count + 1;
            string outputPortName = string.IsNullOrEmpty(overriddenPortName)
                   ? "퀘스트선택포트 -" + num.ToString() + "-"
                   : overriddenPortName;

            QuestDBObjectScript questDBData = DialogueDBWindowEditor.GetDialogueDBData().questDBData;
            List<string> questBookDataIDList = new List<string>();
            if (questDBData == null)
            {
                questBookDataIDList.Add("퀘스트DB가 존재하지않음");
            }
            else
            {
                for (int i = 0; i < questDBData.questBookDataArray.Length; i++)
                {
                    questBookDataIDList.Add(questDBData.questBookDataArray[i].questBookIDKey);
                }
            }

            var SelectQuestContentField = new PopupField<string>("SelectQuestContent(" + num.ToString() + ")", questBookDataIDList, questBookDataIDList[0]);
            SelectQuestContentField.RegisterValueChangedCallback(evt =>
            {   
                DialogueDBWindowEditor.saveCheck = true;

                //새로고침체크
                if (questDBData != null)
                {
                    questBookDataIDList.Clear();
                    for (int i = 0; i < questDBData.questBookDataArray.Length; i++)
                    {
                        questBookDataIDList.Add(questDBData.questBookDataArray[i].questBookIDKey);
                    }
                }
            });
            SelectQuestContentField.index = 0;
            
            //포트추가
            Port generatedPort = GeneratePort(selectQuestBranchNode, Direction.Output, Port.Capacity.Single);
            generatedPort.portName = outputPortName;
            generatedPort.name = "퀘스트선택아웃풋";

            //포트삭제 버튼 추가
            var deleteButton = new Button()
            {
                //text = "해당퀘스트선택지 삭제",
                text = "X",
            };
            deleteButton.clickable.clicked += () => {
                selectQuestBranchNode.questBookIDFieldList.Remove(SelectQuestContentField);
                generatedPort.Remove(SelectQuestContentField);
                generatedPort.Remove(deleteButton);
                RemovePort(selectQuestBranchNode, generatedPort);                
            };

            //노드에 데이터 캐싱
            selectQuestBranchNode.questBookIDFieldList.Add(SelectQuestContentField);
            selectQuestBranchNode.Add(SelectQuestContentField);
            selectQuestBranchNode.Add(deleteButton);
            selectQuestBranchNode.RefreshPorts();
            selectQuestBranchNode.RefreshExpandedState();
        }

        private void UpdateSetProperty(SetCustomPropertyNode customPropertyNode)
        {
            var customPropertyTypeEnumField = new EnumField("변경할 타입", customPropertyNode.propertyType);
            customPropertyNode.customPropertyTypeEnumField = customPropertyTypeEnumField;
            customPropertyNode.Add(customPropertyTypeEnumField);
            customPropertyTypeEnumField.RegisterValueChangedCallback(evt =>
            {
                customPropertyNode.propertyType = (PropertyType)evt.newValue;
                DialogueDBWindowEditor.saveCheck = true;
            });
            customPropertyNode.propertyType = PropertyType.Bool;


            var customValueCreateButton = new Button(clickEvent: () => {
                SetPropertyButton(customPropertyNode);
            });
            customValueCreateButton.text = "-Init-";
            customPropertyNode.Add(customValueCreateButton);
        }


        private void UpdateConditionBrach(ConditionBranchNode conditionBranchNode)
        {
            var customPropertyTypeEnumField = new EnumField("비교할 타입", conditionBranchNode.targetPropertyType);
            conditionBranchNode.customPropertyTypeEnumField = customPropertyTypeEnumField;
            conditionBranchNode.Add(customPropertyTypeEnumField);
            customPropertyTypeEnumField.RegisterValueChangedCallback(evt =>
            {
                conditionBranchNode.targetPropertyType = (PropertyType)evt.newValue;
                DialogueDBWindowEditor.saveCheck = true;
            });


            var customValueCreateButton = new Button(clickEvent: () =>
            {
                ConditionBranchButton(conditionBranchNode);
            });

            customValueCreateButton.text = "-Init-";
            conditionBranchNode.Add(customValueCreateButton);
        }

        public void GetTargetValueList(List<string> _tempList, PropertyType _propertyType)
        {
            switch (_propertyType)
            {
                case PropertyType.Bool:
                    if (customPropertyBoolList.Count > 0)
                    {
                        //팝업설정
                        for (int i = 0; i < customPropertyBoolList.Count; i++)
                        {
                            _tempList.Add(customPropertyBoolList[i].propertyName);
                        }
                    }
                    break;
                case PropertyType.Float:
                    if (customPropertyFloatList.Count > 0)
                    {
                        //팝업설정
                        for (int i = 0; i < customPropertyFloatList.Count; i++)
                        {
                            _tempList.Add(customPropertyFloatList[i].propertyName);
                        }
                    }
                    break;
                case PropertyType.Int:
                    if (customPropertyIntList.Count > 0)
                    {
                        //팝업설정
                        for (int i = 0; i < customPropertyIntList.Count; i++)
                        {
                            _tempList.Add(customPropertyIntList[i].propertyName);
                        }
                    }
                    break;
                case PropertyType.String:
                    if (customPropertyStringList.Count > 0)
                    {
                        //팝업설정
                        for (int i = 0; i < customPropertyStringList.Count; i++)
                        {
                            _tempList.Add(customPropertyStringList[i].propertyName);
                        }
                    }
                    break;
            }
        }

        public void SetPropertyButton(SetCustomPropertyNode customPropertyNode)
        {
            if (customPropertyNode.propertyField != null)
            {
                customPropertyNode.Remove(customPropertyNode.propertyField);
                customPropertyNode.propertyField = null;
            }

            List<string> tempList = new List<string>();
            GetTargetValueList(tempList, customPropertyNode.propertyType);

            //프로퍼티필드 설정
            if (tempList.Count > 0)
            {
                var testField = new PopupField<string>("타겟 변수", tempList, 0);
                customPropertyNode.testField = testField;
                var setPropertyField = new PropertyField();
                setPropertyField.style.backgroundColor = new Color(0.1f, 0.1f, 0.1f);
                setPropertyField.Add(testField);
                customPropertyNode.propertyField = setPropertyField;
                customPropertyNode.Add(setPropertyField);
                customPropertyNode.propertyName = tempList[0];

                switch (customPropertyNode.propertyType)
                {
                    case PropertyType.Bool:
                        var boolField = new Toggle("변경할 bool값");
                        customPropertyNode.boolField = boolField;
                        boolField.RegisterValueChangedCallback(boolEvt =>
                        {
                            customPropertyNode.boolValue = boolEvt.newValue;
                        });
                        customPropertyNode.boolValue = boolField.value;
                        setPropertyField.Add(boolField);

                        break;
                    case PropertyType.Float:
                        var customPropertySetTypeEnumField = new EnumField("더해줄지 세팅할지", customPropertyNode.propertySetType);
                        customPropertyNode.customPropertySetTypeEnumField = customPropertySetTypeEnumField;
                        customPropertyNode.Add(customPropertySetTypeEnumField);
                        customPropertySetTypeEnumField.RegisterValueChangedCallback(evt =>
                        {
                            customPropertyNode.propertySetType = (PropertySetType)evt.newValue;
                            DialogueDBWindowEditor.saveCheck = true;
                        });
                        customPropertyNode.propertySetType = PropertySetType.Add;
                        setPropertyField.Add(customPropertySetTypeEnumField);

                        var floatField = new FloatField("추가할 float값");
                        customPropertyNode.floatField = floatField;
                        floatField.RegisterValueChangedCallback(floatEvt =>
                        {
                            customPropertyNode.floatVaule = floatEvt.newValue;
                            DialogueDBWindowEditor.saveCheck = true;
                        });
                        customPropertyNode.floatVaule = floatField.value;
                        setPropertyField.Add(floatField);
                        break;
                    case PropertyType.Int:
                        var customPropertySetTypeEnumField1 = new EnumField("더해줄지 세팅할지", customPropertyNode.propertySetType);
                        customPropertyNode.customPropertySetTypeEnumField1 = customPropertySetTypeEnumField1;
                        customPropertyNode.Add(customPropertySetTypeEnumField1);
                        customPropertySetTypeEnumField1.RegisterValueChangedCallback(evt =>
                        {
                            customPropertyNode.propertySetType = (PropertySetType)evt.newValue;
                            DialogueDBWindowEditor.saveCheck = true;
                        });
                        customPropertyNode.propertySetType = PropertySetType.Add;
                        setPropertyField.Add(customPropertySetTypeEnumField1);

                        var intField = new IntegerField("추가할 int값");
                        customPropertyNode.intField = intField;
                        intField.RegisterValueChangedCallback(intEvt =>
                        {
                            customPropertyNode.intValue = intEvt.newValue;
                            DialogueDBWindowEditor.saveCheck = true;
                        });
                        customPropertyNode.intValue = intField.value;
                        setPropertyField.Add(intField);
                        break;
                    case PropertyType.String:
                        var stringField = new TextField("변경할 string값");
                        DialogueDBWindowEditor.InitTextField(stringField);
                        customPropertyNode.stringField = stringField;
                        stringField.RegisterValueChangedCallback(stringEvt =>
                        {
                            customPropertyNode.stringValue = stringEvt.newValue;
                            DialogueDBWindowEditor.saveCheck = true;
                        });
                        customPropertyNode.stringValue = stringField.value;
                        setPropertyField.Add(stringField);
                        break;
                }
                testField.RegisterValueChangedCallback(evnt =>
                {
                    customPropertyNode.propertyName = evnt.newValue;
                    DialogueDBWindowEditor.saveCheck = true;
                });
            }
        }

        public void ConditionBranchButton(ConditionBranchNode conditionBranchNode)
        {
            if (conditionBranchNode.propertyField != null)
            {
                conditionBranchNode.Remove(conditionBranchNode.propertyField);
                conditionBranchNode.propertyField = null;
            }

            List<string> tempList = new List<string>();
            GetTargetValueList(tempList, conditionBranchNode.targetPropertyType);
            var testField = new PopupField<string>("타겟 변수", tempList, 0);
            conditionBranchNode.testField = testField;
            testField.RegisterValueChangedCallback(changeEvt => {
                conditionBranchNode.targetPropertyName = changeEvt.newValue;
                DialogueDBWindowEditor.saveCheck = true;
            });

            //프로퍼티필드 설정
            if (tempList.Count > 0)
            {
                var setPropertyField = new PropertyField();
                setPropertyField.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
                setPropertyField.Add(testField);
                conditionBranchNode.propertyField = setPropertyField;
                conditionBranchNode.Add(setPropertyField);
                conditionBranchNode.targetPropertyName = tempList[0];

                var customValueTypeEnumField = new EnumField("비교연산자 타입", conditionBranchNode.operatorType);
                conditionBranchNode.customValueTypeEnumField = customValueTypeEnumField;
                customValueTypeEnumField.RegisterValueChangedCallback(customEvt =>
                {
                    conditionBranchNode.operatorType = (ComparisonOperatorType)customEvt.newValue;
                    if (conditionBranchNode.targetPropertyType == PropertyType.Bool || conditionBranchNode.targetPropertyType == PropertyType.String)
                    {
                        conditionBranchNode.operatorType = ComparisonOperatorType.Equal;
                        customValueTypeEnumField.value = ComparisonOperatorType.Equal;
                    }
                    DialogueDBWindowEditor.saveCheck = true;
                });
                conditionBranchNode.operatorType = ComparisonOperatorType.Equal;
                customValueTypeEnumField.value = ComparisonOperatorType.Equal;

                setPropertyField.Add(customValueTypeEnumField);

                switch (conditionBranchNode.targetPropertyType)
                {
                    case PropertyType.Bool:
                        var boolField = new Toggle("비교할 bool값");
                        conditionBranchNode.boolField = boolField;
                        boolField.RegisterValueChangedCallback(boolEvt =>
                        {
                            conditionBranchNode.boolValue = boolEvt.newValue;
                            DialogueDBWindowEditor.saveCheck = true;
                        });
                        conditionBranchNode.boolValue = boolField.value;
                        setPropertyField.Add(boolField);
                        break;
                    case PropertyType.Float:
                        var floatField = new FloatField("비교할 float값");
                        conditionBranchNode.floatField = floatField;
                        floatField.RegisterValueChangedCallback(floatEvt =>
                        {
                            conditionBranchNode.floatVaule = floatEvt.newValue;
                            DialogueDBWindowEditor.saveCheck = true;
                        });
                        conditionBranchNode.floatVaule = floatField.value;
                        setPropertyField.Add(floatField);
                        break;
                    case PropertyType.Int:
                        var intField = new IntegerField("비교할 int값");
                        conditionBranchNode.intField = intField;
                        intField.RegisterValueChangedCallback(intEvt =>
                        {
                            conditionBranchNode.intValue = intEvt.newValue;
                            DialogueDBWindowEditor.saveCheck = true;
                        });
                        conditionBranchNode.intValue = intField.value;
                        setPropertyField.Add(intField);
                        break;
                    case PropertyType.String:
                        var stringField = new TextField("비교할 string값");
                        DialogueDBWindowEditor.InitTextField(stringField);
                        conditionBranchNode.stringField = stringField;
                        stringField.RegisterValueChangedCallback(stringEvt =>
                        {
                            conditionBranchNode.stringValue = stringEvt.newValue;
                            DialogueDBWindowEditor.saveCheck = true;
                        });
                        conditionBranchNode.stringValue = stringField.value;
                        setPropertyField.Add(stringField);
                        break;
                }
                testField.RegisterValueChangedCallback(evnt =>
                {
                    conditionBranchNode.targetPropertyName = evnt.newValue;
                    DialogueDBWindowEditor.saveCheck = true;
                });
            }
        }
    }
}
#endif
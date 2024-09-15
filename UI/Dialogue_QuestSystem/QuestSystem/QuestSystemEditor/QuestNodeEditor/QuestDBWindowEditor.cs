#if Doozy

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Events;
using TMPro;
using lLCroweTool.StatusModuleSystem;
using lLCroweTool.InventorySystem;
using lLCroweTool.NodeMapSystem;
using lLCroweTool.WorldObjectSystem;
using lLCroweTool.SkillSystem;
using lLCroweTool.QuestSystem;
using lLCroweTool.DialogueSystem;
using lLCroweTool.Localize;
#if UNITY_EDITOR
using UnityEditor;
#pragma warning disable 0618
namespace lLCroweTool.QC.EditorOnly
{
    public class QuestDBWindowEditor : EditorWindow
    {
        //대화DB를 세팅해줄 윈도우 데이터
        //대화DB를 세팅해줄 윈도우 데이터
        private static QuestDBObjectScript questDBData;
        private static bool inspectorOpen = false;
        private ObjectField questDBDataField;

        private QuestBookDataNodeGraphView _graphView;
        private List<Edge> Edges => _graphView.edges.ToList();//그래프뷰의 연결된 선들
        private List<QuestUINode> Nodes => _graphView.nodes.ToList().Cast<QuestUINode>().ToList();//그래프뷰의 노드들                

        private VisualElement bodyElement;//그래프가 들어갈 최상단 부모앨리먼트
        private VisualElement bodyElement2;//그래프가 들어갈 상단 부모앨리먼트
        private static Vector2 questBookDBWindowEditorWindowSize = new Vector2(1500, 700);//사이즈

        //DB파일관련 데이터
        //끝나면 초기화해야됨
        private string fileName;
        private TextField fileNameField;
        private string version;
        private TextField versionField;
        private string author;
        private TextField authorField;
        private string description;
        private TextField descriptionField;
        private DialogueDBObjectScript dialogueDBData;
        private ObjectField dialogueDBDataField;
        private TMP_FontAsset defaultFontAsset;
        private ObjectField defaultFontAssetObjectField;

        private PropertyField propertyDBDataField;

        //퀘스트북데이터 찾는 아이디
        private string findDataID;

        //퀘스트북데이터 스크롤뷰
        private ScrollView questBookDataScrollView;
        private static List<QuestBookDataField> questBookDataFieldList = new List<QuestBookDataField>();
        private QuestBookData targetQuestBookData;
        private Label selectNameLabelField;

        public static bool saveCheck = false;//트루여야지 경고창이 나옴


        [MenuItem("lLcroweTool/QuestBookDBWindowEditor")]
        public static void ShowWindow()
        {
            // 생성되어있는 윈도우를 가져온다. 없으면 새로 생성한다. 싱글턴 구조인듯하다.
            QuestDBWindowEditor editorWindow = (QuestDBWindowEditor)GetWindow(typeof(QuestDBWindowEditor));
            editorWindow.titleContent.text = "퀘스트북데이터 노드그래프 에디터";
            editorWindow.minSize = questBookDBWindowEditorWindowSize;
            editorWindow.maxSize = questBookDBWindowEditorWindowSize;
        }

        private void OnEnable()
        {
            //노드그래프쪽
            bodyElement = new VisualElement();

            rootVisualElement.Add(bodyElement);
            //좌측에서 우측 순으로
            bodyElement.style.flexDirection = FlexDirection.RowReverse;
            //상단에 붙어서 
            bodyElement.style.alignItems = Align.FlexStart;

            //툴바생성
            GenerateToolbar();
            bodyElement.StretchToParentSize();

            //===================================================================
            //작업중안 구역
            //우측지점의 DB에 있는 대화데이터를 표시해줄 리스트표시
            UnityEngine.UIElements.Box customValueBox = new UnityEngine.UIElements.Box();

            customValueBox.style.width = 300;
            customValueBox.style.height = questBookDBWindowEditorWindowSize.y;
            rootVisualElement.Add(customValueBox);

            //버튼들이 있는 프로퍼티         

            //퀘스트북DB관련 버튼
            propertyDBDataField = new PropertyField();
            propertyDBDataField.style.width = 300;
            propertyDBDataField.style.height = 420;
            customValueBox.Add(propertyDBDataField);

            questDBDataField = new ObjectField("집어넣을 퀘스트북DB데이터")
            {
                objectType = typeof(QuestDBObjectScript),
            };
            questDBDataField.value = questDBData;
            questDBDataField.RegisterValueChangedCallback(questBookDBDataEvt =>
            {
                questDBData = (QuestDBObjectScript)questBookDBDataEvt.newValue;
            });
            propertyDBDataField.Add(questDBDataField);

            //퀘스트북DB 저장 로드 관련
            propertyDBDataField.Add(new Button(() =>
            {
                RequestDBDataOperation(true);

                //Sync LocalizeData//로컬라이징 데이터와 동기화
                if (questDBData == null)
                {
                    return;
                }
                //_dialogueID + "_" + _dialogueContentData.localizeID

                List<string> tempLocalizeIDList = new List<string>();
                List<string> overLapDebugList = new List<string>();//중복처리됫을때 보여주는 문자
                //일단은 로컬라이즈ID 추가//로컬라이징필요한거 집어넣기//기초작업끝나면 작업하기//20211127
                //20220620작업마무리
                for (int i = 0; i < questDBData.questBookDataArray.Length; i++)
                {
                    string tempDebug = questDBData.questBookDataArray[i].questBookIDKey;
                    //로컬라이즈ID가 필요한곳은 두종류다
                    //1.퀘스트북 제목
                    string temp = questDBData.questBookDataArray[i].questBookTitle;

                    if (!string.IsNullOrEmpty(temp))
                    {
                        if (!tempLocalizeIDList.Contains(tempDebug + "_" + temp))
                        {
                            tempLocalizeIDList.Add(tempDebug + "_" + temp);
                        }
                        else
                        {
                            overLapDebugList.Add(temp + " <= " + tempDebug + " 대화내용 중복로컬아이디");
                        }
                    }

                    //2.퀘스트북내용 안내창
                    temp = questDBData.questBookDataArray[i].questBookContentInfo;
                    if (!string.IsNullOrEmpty(temp))
                    {
                        if (!tempLocalizeIDList.Contains(tempDebug + "_" + temp))
                        {
                            tempLocalizeIDList.Add(tempDebug + "_" + temp);
                        }
                        else
                        {
                            overLapDebugList.Add(temp + " <= " + tempDebug + " 선택분기 중복로컬아이디");
                        }
                    }
                }

                //중복체크
                if (overLapDebugList.Count != 0)
                {
                    string overLapText = "";
                    for (int i = 0; i < overLapDebugList.Count; i++)
                    {
                        overLapText += overLapDebugList[i] + "\n";
                    }
                    overLapText += "의 아이디가 중복되었습니다. 확인하고 바꿔주세요";

                    EditorUtility.DisplayDialog("중복체크 확인창", overLapText, "OK");
                    return;
                }

                //로컬라이징아이디 데이터추가
                string path = "Assets/Resources";
                LocalizeDBObjectScript[] localizeDBObjectScriptArrray = lLcroweUtilEditor.GetCustomFile<LocalizeDBObjectScript>.GetScriptableObjectFile(path, "*.asset");
                if (localizeDBObjectScriptArrray.Length != 0)
                {
                    lLcroweUtilEditor.SyncLocalizeData(localizeDBObjectScriptArrray[0], tempLocalizeIDList.ToArray());
                    EditorUtility.DisplayDialog("로컬라이징 데이터 확인창", localizeDBObjectScriptArrray[0].name + "에 싱크됫습니다.", "OK");
                }

            })
            { text = "Save DBData" });
            propertyDBDataField.Add(new Button(() =>
            {
                if (saveCheck)
                {
                    //bool res2 = EditorUtility.DisplayDialog("변경내용 확인창", "변경된 내용이 감지되었습니다 저장버튼을 눌려주세요!", "OK", "Cancel");
                    bool res2 = EditorUtility.DisplayDialog("변경내용 확인창", "변경된 내용이 감지되었습니다 저장버튼을 눌려주세요!", "OK");
                }
                else
                {
                    RequestDBDataOperation(false);
                }

            })
            { text = "Load DBData" });
            fileNameField = new TextField("파일이름");
            InitTextField(fileNameField);
            fileNameField.RegisterValueChangedCallback(fileNameEvt =>
            {
                fileName = fileNameEvt.newValue;
                saveCheck = true;
            });

            versionField = new TextField("버전");
            InitTextField(versionField);
            versionField.RegisterValueChangedCallback(versionEvt =>
            {
                version = versionEvt.newValue;
                saveCheck = true;
            });

            authorField = new TextField("작가");
            InitTextField(authorField);
            authorField.RegisterValueChangedCallback(authorEvt =>
            {
                author = authorEvt.newValue;
                saveCheck = true;
            });

            descriptionField = new TextField("설명");
            InitTextField(descriptionField);
            descriptionField.RegisterValueChangedCallback(descriptionEvt =>
            {
                description = descriptionEvt.newValue;
                saveCheck = true;
            });

            dialogueDBDataField = new ObjectField("참조하는 대화데이터")
            {
                objectType = typeof(DialogueDBObjectScript),
            };

            dialogueDBDataField.RegisterValueChangedCallback(evt =>
            {
                dialogueDBData = (DialogueDBObjectScript)evt.newValue;
                saveCheck = true;
            });

            defaultFontAssetObjectField = new ObjectField("기본값 폰트")
            {
                objectType = typeof(TMP_FontAsset),
            };

            defaultFontAssetObjectField.RegisterValueChangedCallback(evt =>
            {
                defaultFontAsset = (TMP_FontAsset)evt.newValue;
                saveCheck = true;
            });

            propertyDBDataField.Add(fileNameField);
            propertyDBDataField.Add(versionField);
            propertyDBDataField.Add(authorField);
            propertyDBDataField.Add(descriptionField);
            propertyDBDataField.Add(dialogueDBDataField);
            propertyDBDataField.Add(defaultFontAssetObjectField);

            //로컬라이징매니저의 데이터에 추가해주는 작업
            //propertyDBDataField.Add(new Button(() => 
            //{


            //}) { text = "Sync LocalizeData" });

            //표시용
            selectNameLabelField = new Label();
            selectNameLabelField.style.color = Color.white;
            selectNameLabelField.text = "-=선택한게 없음=-";
            propertyDBDataField.Add(selectNameLabelField);


            //퀘스트북DB데이터 관련 버튼
            //찾기
            TextField textField = new TextField("찾을 퀘스트북데이터 이름을 쓰세요");
            //한글이 안쳐질떄 사용법
            InitTextField(textField);
            textField.RegisterValueChangedCallback(findEvt =>
            {
                findDataID = findEvt.newValue;
                saveCheck = true;
            });
            propertyDBDataField.Add(textField);
            propertyDBDataField.Add(new Button(() => FindQuestBookData(findDataID)) { text = "Find QuestBookData" });


            //다있을거니 찾아서 변경해줘야됨 삭제만 없네 버튼과 삭제버튼, 저장 ,로드            
            //생성 (완료)

            //퀘스트북데이터필드에 존재
            //삭제, 선택 (완료)

            propertyDBDataField.Add(new Button(() =>
            {
                if (questDBData == null)
                {
                    return;
                }
                QuestBookData questBookData = new QuestBookData();
                questBookData.questBookIDKey = questBookDataFieldList.Count.ToString();
                QuestBookDataField questBookDataField = new QuestBookDataField(questBookData, questBookDataFieldList, questBookDataScrollView, SelectQuestBookDataField, deleteQuestBookDataField);
                saveCheck = true;
            })
            { text = "Add QuestBookData" });

            //퀘스트북데이터의 리스트가 떠야됨
            //리스트생성 함수필요
            //퀘스트북데이터들이 있는 스크롤 뷰
            questBookDataScrollView = new ScrollView();
            questBookDataScrollView.style.height = questBookDBWindowEditorWindowSize.y;
            questBookDataScrollView.showVertical = true;
            questBookDataScrollView.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            customValueBox.Add(questBookDataScrollView);

            //노드그래프 생성
            bodyElement2 = new VisualElement();
            bodyElement2.style.width = questBookDBWindowEditorWindowSize.x - 300;
            bodyElement2.style.height = questBookDBWindowEditorWindowSize.y;
            bodyElement2.style.flexDirection = FlexDirection.RowReverse;
            bodyElement2.style.alignItems = Align.FlexEnd;
            bodyElement.Add(bodyElement2);
            ConstructGraphView(bodyElement2);

            //인스팩터에서 열기
            if (inspectorOpen)
            {
                fileName = questDBData.name;
                version = questDBData.version;
                author = questDBData.author;
                description = questDBData.description;
                dialogueDBData = questDBData.dialogueDBData;
                defaultFontAsset = questDBData.defaultFontAsset;

                RequestDBDataOperation(false);
                inspectorOpen = false;
            }
        }

        private void OnDisable()
        {
            //지우기           
            questBookDataFieldList.Clear();
            questBookDataScrollView.Clear();

            propertyDBDataField.Remove(questDBDataField);
            propertyDBDataField.Remove(fileNameField);
            propertyDBDataField.Remove(versionField);
            propertyDBDataField.Remove(authorField);
            propertyDBDataField.Remove(descriptionField);
            propertyDBDataField.Remove(dialogueDBDataField);
            propertyDBDataField.Remove(defaultFontAssetObjectField);
            propertyDBDataField.Remove(selectNameLabelField);

            bodyElement2.Remove(_graphView);
            bodyElement.Remove(bodyElement2);
            rootVisualElement.Remove(bodyElement);

            questDBData = null;
            saveCheck = false;
        }

        public static List<QuestBookData> GetQuestBookDataList()
        {
            List<QuestBookData> _questBookDataList = new List<QuestBookData>();
            for (int i = 0; i < questBookDataFieldList.Count; i++)
            {
                _questBookDataList.Add(questBookDataFieldList[i].GetQuestBookData());
            }
            return _questBookDataList;
        }

        /// <summary>
        /// 그래프뷰생성
        /// </summary>
        private void ConstructGraphView(VisualElement targetElement)
        {
            //그려주는 위치
            _graphView = new QuestBookDataNodeGraphView
            {
                name = "UI노드데이터 에디터관리자"
            };

            targetElement.Add(_graphView);
            _graphView.StretchToParentSize();

            //우측지점에 변수데이터
            UnityEngine.UIElements.Box customValueBox = new UnityEngine.UIElements.Box();
            customValueBox.style.width = 250;
            customValueBox.style.height = questBookDBWindowEditorWindowSize.y - 20;

            ScrollView m_TasksContainer = new ScrollView();
            _graphView.scrollView = m_TasksContainer;
            m_TasksContainer.style.height = questBookDBWindowEditorWindowSize.y;
            m_TasksContainer.showVertical = true;
            m_TasksContainer.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f);

            var questBookClearRewardCreateButton = new Button(clickEvent: () =>
            {

                if (questDBData == null)
                {
                    return;
                }
                RewardData rewardData = new RewardData();
                AddQuestBookReward(rewardData);
                saveCheck = true;
            });
            questBookClearRewardCreateButton.text = "Create -QuestBookReward-";

            //리스트뷰에 그래프뷰의 커스텀밸류 리스트를 집어넣는다
            //리스트를 집어넣을때 해당뷰에서 삭제버튼을 눌려서 가능하게 제작

            //propertyField.style.color = new Color(0.1f, 0.1f, 0.1f);
            //propertyField.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            ////propertyField.style.backgroundColor = Color.white;
            //propertyField.style.flexDirection = FlexDirection.Row;
            //propertyField.style.justifyContent = Justify.Center;
            //propertyField.style.alignSelf = Align.Stretch;

            customValueBox.Add(m_TasksContainer);
            customValueBox.Add(questBookClearRewardCreateButton);
            targetElement.Add(customValueBox);
        }

        private void AddQuestBookReward(RewardData _rewardData)
        {
            RewardDataField rewardDataField = new RewardDataField(_rewardData, _graphView.questBookRewardDataFieldList, _graphView.scrollView);
        }


        /// <summary>
        /// 툴바생성
        /// </summary>
        private void GenerateToolbar()
        {
            var toolbar = new Toolbar();
            toolbar.style.flexDirection = FlexDirection.RowReverse;
            toolbar.style.justifyContent = Justify.Center;

            //그래프 노드 생성버튼관련
            //var propertyField = new PropertyField();

            //propertyField.style.height = 30;
            ////propertyField.style.color = new Color(0.1f, 0.1f, 0.1f);
            //propertyField.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            //propertyField.style.flexDirection = FlexDirection.Row;
            //propertyField.style.justifyContent = Justify.Center;
            //propertyField.style.alignSelf = Align.Stretch;

            var questNodeCreateButton = new Button(clickEvent: () =>
            {
                if (questDBData == null)
                {
                    return;
                }
                Vector2 _pos = (Vector2)_graphView.viewTransform.position * -1 + (_graphView.contentRect.size / 2);
                _graphView.CreateNode("QuestNode", _pos, QuestNodeType.QuestNode);
                saveCheck = true;
            });
            questNodeCreateButton.style.width = 200;
            questNodeCreateButton.text = "Create -QuestNode-";

            var dialogueEventCreateButton = new Button(clickEvent: () =>
            {
                if (questDBData == null)
                {
                    return;
                }
                Vector2 _pos = (Vector2)_graphView.viewTransform.position * -1 + (_graphView.contentRect.size / 2);
                _graphView.CreateNode("DialogueEvent", _pos, QuestNodeType.DialogueEvent);
                saveCheck = true;
            });
            dialogueEventCreateButton.style.width = 200;
            dialogueEventCreateButton.text = "Create -DialogueEvent-";

            var linkedQuestBookCreateButton = new Button(clickEvent: () =>
            {
                if (questDBData == null)
                {
                    return;
                }
                Vector2 _pos = (Vector2)_graphView.viewTransform.position * -1 + (_graphView.contentRect.size / 2);
                _graphView.CreateNode("LinkedQuestBook", _pos, QuestNodeType.LinkedQuestBook);
                saveCheck = true;
            });
            linkedQuestBookCreateButton.style.width = 200;
            linkedQuestBookCreateButton.text = "Create -LinkedQuestBook-";


            var randomBranchNodeCreateButton = new Button(clickEvent: () =>
            {
                if (questDBData == null)
                {
                    return;
                }
                Vector2 _pos = (Vector2)_graphView.viewTransform.position * -1 + (_graphView.contentRect.size / 2);
                _graphView.CreateNode("RandomBranchNode", _pos, QuestNodeType.RandomBranch);
                saveCheck = true;
            });
            randomBranchNodeCreateButton.style.width = 200;
            randomBranchNodeCreateButton.text = "Create -RandomBranchNode-";

            toolbar.Add(randomBranchNodeCreateButton);
            toolbar.Add(linkedQuestBookCreateButton);
            toolbar.Add(dialogueEventCreateButton);
            toolbar.Add(questNodeCreateButton);

            rootVisualElement.Add(toolbar);
        }

        /// <summary>
        /// 퀘스트DB 저장 로드관련 함수
        /// </summary>
        /// <param name="save"></param>
        private void RequestDBDataOperation(bool save)
        {
            if (save)
            {
                if (fileName == null || fileName == "")
                {
                    Debug.Log("파일이름을 적어주세요");
                    return;
                }

                saveCheck = false;

                //존재하는지 체크
                //QuestDBObjectScript _questDBData = (QuestDBObjectScript)AssetDatabase.LoadAssetAtPath("Assets/Resources/" + fileName + ".asset", typeof(QuestDBObjectScript));
                QuestDBObjectScript _questDBData = new QuestDBObjectScript();
                //if (_questDBData == null)
                //{
                //    _questDBData = new QuestDBObjectScript();
                //}


                // Make sure the file name is unique, in case an existing Prefab has the same name.
                //localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
                // Create the new Prefab.
                //각각의 트리슬롯들을 프리팹화해주기 //트리슬롯등록
                //SaveAsPrefabAssetAndConnect=>경로에 프리팹을 지정후 씬상에 있는 프리팹과 연동 시켜줌
                //SaveAsPrefabAsset=>경로에 프리팹을 지정후 끝  씬상에 있는 프리팹과는 연동 안시켜줌
                //researchTreeData.researchTreeSystem = PrefabUtility.SaveAsPrefabAssetAndConnect(targetingResearchTreeSystem.gameObject, localPath, InteractionMode.UserAction, out isdone).GetComponent<ResearchTreeSystem>();

                //현재 제작중인 대상대화데이터 저장
                if (targetQuestBookData != null)
                {
                    SaveQuestDataGraph(targetQuestBookData);
                }

                //데이터 캐싱
                _questDBData.version = version;
                _questDBData.author = author;
                _questDBData.description = description;
                _questDBData.dialogueDBData = dialogueDBData;
                _questDBData.defaultFontAsset = defaultFontAsset;


                //퀘스트북데이터들 캐싱              
                List<QuestBookData> tempQuestBookDataList = new List<QuestBookData>();

                for (int i = 0; i < questBookDataFieldList.Count; i++)
                {
                    QuestBookData tempDialogueData = questBookDataFieldList[i].GetQuestBookData();
                    tempDialogueData.precedeQuestBookIDKeyArray = questBookDataFieldList[i].GetPrecedeQuestBookIDKeyArray();
                    tempQuestBookDataList.Add(tempDialogueData);
                }

                _questDBData.questBookDataArray = tempQuestBookDataList.ToArray();
                questDBData = _questDBData;
                questDBDataField.value = questDBData;

                lLcroweUtilEditor.CreateResourceDataObject(_questDBData, fileName, "");

                //AssetDatabase.CreateAsset(questDBData, "Assets/Resources/" + fileName + ".asset");
            }
            else
            {
                if (questDBData == null)
                {
                    Debug.Log("로드할 대상이 없습니다");
                    return;
                }

                //로드
                //대화데이터목록 초기화               
                questBookDataFieldList.Clear();
                questBookDataScrollView.Clear();

                //그래프초기화
                ClearGraph();

                //퀘스트데이터목록들을 보여주기
                List<QuestBookDataField> tempList = new List<QuestBookDataField>();
                for (int i = 0; i < questDBData.questBookDataArray.Length; i++)
                {
                    QuestBookDataField questBookDataField = new QuestBookDataField(questDBData.questBookDataArray[i], questBookDataFieldList, questBookDataScrollView, SelectQuestBookDataField, deleteQuestBookDataField);
                    tempList.Add(questBookDataField);
                }

                for (int i = 0; i < questDBData.questBookDataArray.Length; i++)
                {
                    tempList[i].LoadPrecadeQuest(questDBData.questBookDataArray[i]);
                }

                fileNameField.value = questDBData.name;
                versionField.value = questDBData.version;
                authorField.value = questDBData.author;
                descriptionField.value = questDBData.description;
                dialogueDBDataField.value = questDBData.dialogueDBData;
                defaultFontAssetObjectField.value = questDBData.defaultFontAsset;
                saveCheck = false;
            }
        }

        //퀘스트북데이터 필드를 선택할때마다 작동되는 함수
        private void SelectQuestBookDataField(QuestBookData _questBookData)
        {
            //대화데이터를 버튼으로 처리해서 버튼을 누를때마다 저장, 로드 처리하게
            //선택했을시 기존꺼는 저장

            //기존꺼 저장
            if (targetQuestBookData != null)
            {
                //Debug.Log("대화데이터 저장됨 " + targetQuestBookData.dialogueIDKey);
                SaveQuestDataGraph(targetQuestBookData);
            }

            //선택한 대상은 로드
            targetQuestBookData = _questBookData;
            selectNameLabelField.text = "-= 선택된 대화데이터 => " + targetQuestBookData.questBookIDKey + " =-";
            LoadQuestData(targetQuestBookData);

            //Debug.Log("대화데이터 선택됨 " + _dialogueData.dialogueIDKey);
        }

        //대화데이터필드를 삭제할때마다 작동되는 함수
        private void deleteQuestBookDataField()
        {
            targetQuestBookData = null;
        }

        /// <summary>
        /// 대화데이터들에서 특정아이디를 찾는 함수
        /// </summary>
        /// <param name="dataID"></param>
        private void FindQuestBookData(string dataID)
        {
            if (questDBData == null)
            {
                return;
            }
            for (int i = 0; i < questBookDataFieldList.Count; i++)
            {
                if (questBookDataFieldList[i].GetQuestBookData().questBookIDKey == dataID)
                {
                    SelectQuestBookDataField(questBookDataFieldList[i].GetQuestBookData());
                    break;
                }
            }
        }

        public static void SetQuestDBData(QuestDBObjectScript _questDBData)
        {
            questDBData = _questDBData;
            inspectorOpen = true;
        }   //노드링크 연결

        public static QuestDBObjectScript GetQuestDBData()
        {
            return questDBData;
        }

        /// <summary>
        ///퀘스트데이터 노드그래프 저장
        /// </summary>        
        public void SaveQuestDataGraph(QuestBookData _nodeContainerData)
        {

            //노드가 한개라도 연결이 안되있으면 작동안함
            if (!Edges.Any())
            {
                //Debug.Log("노드가 연결이 안되있음");
                return;
            }

            //보상저장
            List<RewardData> rewardQuestBookDataList = new List<RewardData>();
            for (int i = 0; i < _graphView.questBookRewardDataFieldList.Count; i++)
            {
                rewardQuestBookDataList.Add(_graphView.questBookRewardDataFieldList[i].GetRewardData());
            }
            _nodeContainerData.questBookRewardDataArray = rewardQuestBookDataList.ToArray();

            //각각의 데이터 저장
            List<QuestNodeData> questNodeDataList = new List<QuestNodeData>(); ;//퀘스트노드
            List<DialogueEventNodeData> dialogueEventNodeDataList = new List<DialogueEventNodeData>();//대화이벤트 노드
            List<LinkedQuestBookNodeData> linkedQuestBookNodeDataList = new List<LinkedQuestBookNodeData>();//연계퀘스트노드

            //노드 저장
            List<CustomQuestNodeData> nodeDataList = new List<CustomQuestNodeData>();

            //노드 & 데이터 저장로직
            foreach (var node in Nodes.Where(node => node.nodeType != QuestNodeType.StartPoint))
            {
                //노드의 기본적인거 저장
                CustomQuestNodeData customNodeData = new CustomQuestNodeData()
                {
                    nodeType = node.nodeType,
                    nodeGUID = node.GUID,
                    position = node.GetPosition().position,
                    nextNodeGUIDList = new List<string>()
                };

                //노드타입에 따른 내용 저장
                switch (node.nodeType)
                {
                    case QuestNodeType.QuestNode:
                        //캐싱
                        QuestNodeData questNodeData = new QuestNodeData();
                        QuestNode questNode = (QuestNode)node;
                        questNodeData.nodeGUID = questNode.GUID;

                        questNodeData.questName = questNode.questName;
                        questNodeData.questContentInfo = questNode.questContentInfo;
                        questNodeData.questHUDContent = questNode.questHUDContent;
                        questNodeData.isUseTimer = questNode.isUseTimer;
                        questNodeData.time = questNode.time;

                        List<DialogueMissionData> dialogueMissionDataList = new List<DialogueMissionData>();
                        for (int i = 0; i < questNode.dialogueMissionDataFieldList.Count; i++)
                        {
                            dialogueMissionDataList.Add(questNode.dialogueMissionDataFieldList[i].GetDialogueMissionData());
                        }
                        questNodeData.dialogueMissionDataArray = dialogueMissionDataList.ToArray();

                        List<BattleMissionData> battleMissionDataList = new List<BattleMissionData>();
                        for (int i = 0; i < questNode.battleMissionDataFieldList.Count; i++)
                        {
                            battleMissionDataList.Add(questNode.battleMissionDataFieldList[i].GetBattleMissionData());
                        }
                        questNodeData.battleMissionDataArray = battleMissionDataList.ToArray();

                        List<SurviveMissionData> surviveMissionDataList = new List<SurviveMissionData>();
                        for (int i = 0; i < questNode.surviveMissionDataFieldList.Count; i++)
                        {
                            surviveMissionDataList.Add(questNode.surviveMissionDataFieldList[i].GetSurviveMissionData());
                        }
                        questNodeData.surviveMissionDataArray = surviveMissionDataList.ToArray();

                        List<AttractMoveMissionData> attractMoveMissionDataList = new List<AttractMoveMissionData>();
                        for (int i = 0; i < questNode.attractMoveMissionDataFieldList.Count; i++)
                        {
                            attractMoveMissionDataList.Add(questNode.attractMoveMissionDataFieldList[i].GetAttractMissionData());
                        }
                        questNodeData.attractMoveMissionDataArray = attractMoveMissionDataList.ToArray();

                        List<ItemGatherMissionData> itemGatherMissionDataList = new List<ItemGatherMissionData>();
                        for (int i = 0; i < questNode.itemGatherDataFieldList.Count; i++)
                        {
                            itemGatherMissionDataList.Add(questNode.itemGatherDataFieldList[i].GetItemGatherMissionData());
                        }
                        questNodeData.itemGatherMissionDataArray = itemGatherMissionDataList.ToArray();

                        List<ArriveMapMissionData> arriveMapMissionDataList = new List<ArriveMapMissionData>();
                        for (int i = 0; i < questNode.arriveMapMissionDataFieldList.Count; i++)
                        {
                            arriveMapMissionDataList.Add(questNode.arriveMapMissionDataFieldList[i].GetArriveMapMissionData());
                        }
                        questNodeData.arriveMapMissionDataArray = arriveMapMissionDataList.ToArray();

                        //등록
                        questNodeDataList.Add(questNodeData);
                        break;
                    case QuestNodeType.DialogueEvent:
                        //캐싱
                        DialogueEventNodeData dialogueEventNodeData = new DialogueEventNodeData();
                        DialogueEventNode dialogueEventNode = (DialogueEventNode)node;
                        dialogueEventNodeData.nodeGUID = dialogueEventNode.GUID;
                        dialogueEventNodeData.dialogueID = dialogueEventNode.dialogueIDField.value;

                        //등록
                        dialogueEventNodeDataList.Add(dialogueEventNodeData);
                        break;
                    case QuestNodeType.LinkedQuestBook:
                        //캐싱
                        LinkedQuestBookNodeData linkedQuestBookNodeData = new LinkedQuestBookNodeData();
                        LinkedQuestBookNode linkedQuestBookNode = (LinkedQuestBookNode)node;
                        linkedQuestBookNodeData.nodeGUID = linkedQuestBookNode.GUID;
                        linkedQuestBookNodeData.questBookID = linkedQuestBookNode.questBookIDField.value;

                        //등록
                        linkedQuestBookNodeDataList.Add(linkedQuestBookNodeData);
                        break;

                    case QuestNodeType.RandomBranch:
                        //존재하지않음
                        //노드타입으로 체크함
                        break;
                }

                nodeDataList.Add(customNodeData);
            }

            //최종집어넣기
            _nodeContainerData.questNodeDataArray = questNodeDataList.ToArray();
            _nodeContainerData.dialogueEventNodeDataArray = dialogueEventNodeDataList.ToArray();
            _nodeContainerData.linkedQuestBookNodeDataArray = linkedQuestBookNodeDataList.ToArray();
            _nodeContainerData.nodeDataArray = nodeDataList.ToArray();

            //시작노드저장
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].nodeType == QuestNodeType.StartPoint)
                {
                    //현재노드가 시작노드이면 다음은 시작노드와이어진 첫번째노드
                    //전에 저장한게 있으면 해당 노드를 리셋시켜줘야 제대로 찾아줄거 같음
                    Port port = (Port)Nodes[i].outputContainer[0];
                    Edge edge = port.connections.Cast<Edge>().ToArray()[0];//엣찌가 여러개 나옴//문제원인//해결법=>진입점 노드까지 다 지워버리고 다시만드니 한개만 나옴
                    QuestUINode tempNode = (QuestUINode)edge.input.node;

                    //Debug.Log(tempNode.GUID);
                    foreach (var targetnode in Nodes)
                    {
                        if (targetnode.GUID == tempNode.GUID)
                        {
                            //Debug.Log("동일한게 존재");
                            break;
                        }
                    }
                    _nodeContainerData.firstContactNodeGUID = tempNode.GUID;
                    break;
                }
            }


            //링크연결 저장
            var connectedSockets = Edges.Where(x => x.input.node != null).ToArray();
            for (var i = 0; i < connectedSockets.Count(); i++)
            {
                var outputNode = (connectedSockets[i].output.node as QuestUINode);
                var inputNode = (connectedSockets[i].input.node as QuestUINode);
                //nodeContainerData.nodeLinks.Add(new NodeLinkData
                //{
                //    baseNodeGUID = outputNode.GUID,
                //    baseNodeType = outputNode.nodeType,
                //    portName = connectedSockets[i].output.portName,
                //    targetNodeGUID = inputNode.GUID,
                //    targetNodeType = inputNode.nodeType
                //});

                //연결되있는 노드들을 찾아서 포트리스트에다 저장
                for (int j = 0; j < _nodeContainerData.nodeDataArray.Length; j++)
                {
                    //현재노드의 아이디와 현재노드 아웃포트에 존재된 노드의 아이디를 체크
                    if (_nodeContainerData.nodeDataArray[j].nodeGUID == outputNode.GUID)
                    {
                        //같으면
                        //인풋포트의 노드가 다음행선지이므로 등록
                        _nodeContainerData.nodeDataArray[j].nextNodeGUIDList.Add(inputNode.GUID);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 퀘스트데이터 노드그래프 로드 함수
        /// </summary>
        /// <param name="_dialogueData"></param>
        public void LoadQuestData(QuestBookData _dialogueData)
        {
            if (_dialogueData == null)
            {
                EditorUtility.DisplayDialog("퀘스트데이터를 못찾았습니다", "존재하지않습니다. 코드를 확인해주세요!", "OK");
                return;
            }

            ClearGraph();//그래프의 노드정리
            GenerateQuestNodes(_dialogueData);//노드 재생
            ConnectQuestNodes(_dialogueData);//노드끼리 링크연결
        }

        /// <summary>
        /// Set Entry point GUID then Get All Nodes, remove all and their edges. Leave only the entrypoint node. (Remove its edge too)
        /// </summary>
        private void ClearGraph()
        {
            _graphView.scrollView.Clear();

            //Nodes.Find(x => x.entryPoint).GUID = _nodeContainer.nodeLinks[0].baseNodeGUID;
            foreach (var perNode in Nodes)
            {
                //Edges.Where(x => x.input.node == perNode).ToList().ForEach(edge => _graphView.RemoveElement(edge));//엣찌지우기
                _graphView.RemoveElement(perNode);//노드지우기
            }


            foreach (var perEdge in Edges)
            {
                _graphView.RemoveElement(perEdge);
            }
            //Debug.Log(Nodes.Count + "//" + Edges.Count);
        }

        //저장한 노드들을 생성
        private void GenerateQuestNodes(QuestBookData _nodeContainer)
        {
            //퀘스트북 보상생성
            for (int i = 0; i < _nodeContainer.questBookRewardDataArray.Length; i++)
            {
                RewardData rewardData = new RewardData();
                RewardData targetRewardData = _nodeContainer.questBookRewardDataArray[i];
                rewardData.itemData = targetRewardData.itemData;
                rewardData.itemCount = targetRewardData.itemCount;

                rewardData.targetUnitObject = targetRewardData.targetUnitObject;
                rewardData.unitCount = targetRewardData.unitCount;

                rewardData.unitExprience = targetRewardData.unitExprience;
                rewardData.skillPoint = targetRewardData.skillPoint;
                rewardData.skillData = targetRewardData.skillData;
                rewardData.targetObject = targetRewardData.targetObject;

                AddQuestBookReward(rewardData);
            }

            //진입점 생성
            _graphView.AddElement(_graphView.GenerateEntryPointNode());

            //노드데이터 입력
            foreach (var perNodeData in _nodeContainer.nodeDataArray)
            {
                //노드타입 체크후 집어넣기
                //노드생성후 포트 생성후 데이터 삽입해야함
                QuestUINode tempNode = null;
                switch (perNodeData.nodeType)
                {
                    case QuestNodeType.QuestNode:
                        tempNode = _graphView.CreateNode("QuestNode", perNodeData.position, QuestNodeType.QuestNode);
                        QuestNode questNode = (QuestNode)tempNode;
                        QuestNodeData targetDialogueContentData = null;

                        //데이터에서 같은 아이디인지 체크
                        for (int i = 0; i < _nodeContainer.questNodeDataArray.Length; i++)
                        {
                            if (perNodeData.nodeGUID == _nodeContainer.questNodeDataArray[i].nodeGUID)
                            {
                                targetDialogueContentData = _nodeContainer.questNodeDataArray[i];
                                break;
                            }
                        }

                        //데이터캐싱

                        //변수
                        questNode.questName = targetDialogueContentData.questName;
                        questNode.questContentInfo = targetDialogueContentData.questContentInfo;
                        questNode.questHUDContent = targetDialogueContentData.questHUDContent;
                        questNode.isUseTimer = targetDialogueContentData.isUseTimer;
                        questNode.time = targetDialogueContentData.time;

                        //필드
                        questNode.questNameTextField.value = targetDialogueContentData.questName;
                        questNode.questContentInfoTextField.value = targetDialogueContentData.questContentInfo;
                        questNode.questHUDContentTextField.value = targetDialogueContentData.questHUDContent;
                        questNode.isUseTimerField.value = targetDialogueContentData.isUseTimer;
                        questNode.timeField.value = targetDialogueContentData.time;

                        //필드재생
                        for (int i = 0; i < targetDialogueContentData.dialogueMissionDataArray.Length; i++)
                        {
                            DialogueMissionDataField dialogueMissionDataField = new DialogueMissionDataField(targetDialogueContentData.dialogueMissionDataArray[i], questNode.dialogueMissionDataFieldList, questNode.dialogueMissionScrollView, questNode.dialogueMissionNoticeLabel);
                            dialogueMissionDataField.CheckOverLap(questNode.dialogueMissionDataFieldList, questNode.dialogueMissionNoticeLabel);
                        }
                        for (int i = 0; i < targetDialogueContentData.battleMissionDataArray.Length; i++)
                        {
                            BattleMissionDataField battleMissionDataField = new BattleMissionDataField(targetDialogueContentData.battleMissionDataArray[i], questNode.battleMissionDataFieldList, questNode.battleMissionScrollView, questNode.battleMissionNoticeLabel);
                            battleMissionDataField.CheckOverLap(questNode.battleMissionDataFieldList, questNode.battleMissionNoticeLabel);
                        }
                        for (int i = 0; i < targetDialogueContentData.surviveMissionDataArray.Length; i++)
                        {
                            SurviveMissionDataField surviveMissionDataField = new SurviveMissionDataField(targetDialogueContentData.surviveMissionDataArray[i], questNode.surviveMissionDataFieldList, questNode.surviveMissionScrollView, questNode.surviveMissionNoticeLabel);
                            surviveMissionDataField.CheckOverLap(questNode.surviveMissionDataFieldList, questNode.surviveMissionNoticeLabel);
                        }
                        for (int i = 0; i < targetDialogueContentData.attractMoveMissionDataArray.Length; i++)
                        {
                            AttractMoveMissionDataField attractMoveMissionDataField = new AttractMoveMissionDataField(targetDialogueContentData.attractMoveMissionDataArray[i], questNode.attractMoveMissionDataFieldList, questNode.attractMoveMissionScrollView, questNode.attractMoveMissionNoticeLabel);
                            attractMoveMissionDataField.CheckOverLap(questNode.attractMoveMissionDataFieldList, questNode.attractMoveMissionNoticeLabel);
                        }
                        for (int i = 0; i < targetDialogueContentData.itemGatherMissionDataArray.Length; i++)
                        {
                            ItemGatherMissionDataField itemGatherMissionDataField = new ItemGatherMissionDataField(targetDialogueContentData.itemGatherMissionDataArray[i], questNode.itemGatherDataFieldList, questNode.itemMissionScrollView, questNode.itemMissionNoticeLabel);
                            itemGatherMissionDataField.CheckOverLap(questNode.itemGatherDataFieldList, questNode.itemMissionNoticeLabel);
                        }
                        for (int i = 0; i < targetDialogueContentData.arriveMapMissionDataArray.Length; i++)
                        {
                            ArriveMapMissionDataField arriveMapMissionDataField = new ArriveMapMissionDataField(targetDialogueContentData.arriveMapMissionDataArray[i], questNode.arriveMapMissionDataFieldList, questNode.arriveMapMissionScrollView, questNode.arriveMissionNoticeLabel);
                            arriveMapMissionDataField.CheckOverLap(questNode.arriveMapMissionDataFieldList, questNode.arriveMissionNoticeLabel);
                        }
                        break;
                    case QuestNodeType.DialogueEvent:
                        tempNode = _graphView.CreateNode("DialogueEvent", perNodeData.position, QuestNodeType.DialogueEvent);
                        DialogueEventNode dialogueEventNode = (DialogueEventNode)tempNode;

                        DialogueEventNodeData targetDialogueEventNodeData = null;

                        //데이터에서 같은 아이디인지 체크
                        for (int i = 0; i < _nodeContainer.dialogueEventNodeDataArray.Length; i++)
                        {
                            if (perNodeData.nodeGUID == _nodeContainer.dialogueEventNodeDataArray[i].nodeGUID)
                            {
                                targetDialogueEventNodeData = _nodeContainer.dialogueEventNodeDataArray[i];
                                break;
                            }
                        }

                        //데이터캐싱
                        try
                        {
                            //필드
                            dialogueEventNode.dialogueIDField.value = targetDialogueEventNodeData.dialogueID;
                        }
                        catch (System.Exception e)
                        {
                            EditorUtility.DisplayDialog("집어넣을 값에 문제가 있으니 초기화됨", e.Message, "OK");
                            dialogueEventNode.dialogueIDField.index = 0;
                        }
                        break;
                    case QuestNodeType.LinkedQuestBook:
                        tempNode = _graphView.CreateNode("LinkedQuestBook", perNodeData.position, QuestNodeType.LinkedQuestBook);
                        LinkedQuestBookNode linkedQuestBookNode = (LinkedQuestBookNode)tempNode;
                        LinkedQuestBookNodeData targetLinkedQuestBookNodeData = null;

                        //데이터에서 같은 아이디인지 체크
                        for (int i = 0; i < _nodeContainer.linkedQuestBookNodeDataArray.Length; i++)
                        {
                            if (perNodeData.nodeGUID == _nodeContainer.linkedQuestBookNodeDataArray[i].nodeGUID)
                            {
                                targetLinkedQuestBookNodeData = _nodeContainer.linkedQuestBookNodeDataArray[i];
                                break;
                            }
                        }

                        //데이터캐생                        
                        try
                        {
                            //필드
                            linkedQuestBookNode.questBookIDField.value = targetLinkedQuestBookNodeData.questBookID;

                        }
                        catch (System.Exception e)
                        {
                            EditorUtility.DisplayDialog("집어넣을 값에 문제가 있으니 초기화됨", e.Message, "OK");
                            linkedQuestBookNode.questBookIDField.index = 0;
                        }

                        break;

                    case QuestNodeType.RandomBranch:
                        tempNode = _graphView.CreateNode("RandomBranchNode", perNodeData.position, QuestNodeType.RandomBranch);

                        //포트생성
                        for (int i = 0; i < _nodeContainer.nodeDataArray.Length; i++)
                        {
                            if (perNodeData.nodeGUID == _nodeContainer.nodeDataArray[i].nodeGUID)
                            {
                                for (int j = 0; j < _nodeContainer.nodeDataArray[i].nextNodeGUIDList.Count; j++)
                                {
                                    _graphView.AddChoicePort(tempNode, "");
                                }
                                break;
                            }
                        }

                        //데이터캐생
                        //할게없음

                        break;
                }
                tempNode.GUID = perNodeData.nodeGUID;
                _graphView.AddElement(tempNode);
            }
        }

        private void ConnectQuestNodes(QuestBookData _nodeContainer)
        {
            //var k = i; //Prevent access to modified closure
            //var connections = _nodeContainer.nodeLinks.Where(x => x.baseNodeGUID == Nodes[k].GUID).ToList();

            //제작된 노드수만큼 반복
            for (var i = 0; i < Nodes.Count; i++)
            {
                var targetOutputNode = Nodes[i];

                if (targetOutputNode.nodeType == QuestNodeType.StartPoint)
                {
                    //시작포인트이면 첫번째노드와 이어줌
                    //이을 노드를 찾음
                    bool isFind = false;

                    for (int k = 0; k < Nodes.Count; k++)
                    {
                        //같은 노드아이디값이 있는가?
                        if (_nodeContainer.firstContactNodeGUID == Nodes[k].GUID)
                        {
                            //연결시킴
                            //Debug.Log(targetOutputNode.nodeName + " 연결됨=> " + Nodes[k].nodeName);
                            LinkNodesTogether((Port)targetOutputNode.outputContainer[0], (Port)Nodes[k].inputContainer[0]);
                            isFind = true;
                            break;
                        }
                    }
                    if (!isFind)
                    {
                        if (Nodes.Count == 1)
                        {
                            //Debug.Log(targetOutputNode.nodeName + "만 존재함.");
                            EditorUtility.DisplayDialog("알림", targetOutputNode.nodeName + "만 존재함.", "확인");
                        }
                        else
                        {
                            //Debug.Log(targetOutputNode.nodeName + "연결안됨\n" + targetOutputNode.GUID);
                            EditorUtility.DisplayDialog("경고", targetOutputNode.nodeName + "연결안됨\n" + targetOutputNode.GUID, "확인");
                        }
                    }
                }
                else
                {
                    //다음이어질노드를 다음노드GUID리스트에서 찾음
                    for (int j = 0; j < _nodeContainer.nodeDataArray.Length; j++)
                    {
                        if (targetOutputNode.GUID == _nodeContainer.nodeDataArray[j].nodeGUID)
                        {
                            var nextInputNodeList = _nodeContainer.nodeDataArray[j].nextNodeGUIDList;
                            for (int k = 0; k < nextInputNodeList.Count; k++)
                            {
                                //노드들 이름 검색후 맞으면 이어줌
                                for (int l = 0; l < Nodes.Count; l++)
                                {
                                    //노드GUID와 다음 노드 GUID를 비교
                                    if (Nodes[l].GUID == nextInputNodeList[k])
                                    {
                                        //Debug.Log(targetOutputNode.nodeName + " => " + Nodes[l].nodeName);

                                        //링크연결
                                        LinkNodesTogether((Port)targetOutputNode.outputContainer[k], (Port)Nodes[l].inputContainer[0]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        //노드 링크선 생성
        private void LinkNodesTogether(Port outputSocket, Port inputSocket)
        {
            var tempEdge = new Edge()
            {
                output = outputSocket,
                input = inputSocket
            };
            tempEdge?.input.Connect(tempEdge);
            tempEdge?.output.Connect(tempEdge);
            _graphView.Add(tempEdge);
        }

        //텍스트필드 초기화함수
        public static void InitTextField(TextField _textField)
        {
            _textField.RegisterCallback<FocusInEvent>(evt => { Input.imeCompositionMode = IMECompositionMode.On; });
            _textField.RegisterCallback<FocusOutEvent>(evt => { Input.imeCompositionMode = IMECompositionMode.Auto; });

        }
    }

    //대화미션필드
    public class DialogueMissionDataField
    {
        private VisualElement parentElement;//부모
        private PropertyField propertyField;//바닥
        private Button deleteButton;//삭제버튼

        private DialogueMissionData dialogueMissionData;

        private ObjectField actorInfoDataField;

        public DialogueMissionDataField(DialogueMissionData _dialogueMissionData, List<DialogueMissionDataField> _dialogueMissionDataFieldList, VisualElement _parentElement, Label noticeLabel)
        {
            dialogueMissionData = _dialogueMissionData;
            if (!_dialogueMissionDataFieldList.Contains(this))
            {
                _dialogueMissionDataFieldList.Add(this);
            }

            parentElement = _parentElement;
            propertyField = new PropertyField();
            //propertyField.style.borderColor = Color.white;
            propertyField.style.borderBottomColor = Color.white;
            propertyField.style.borderLeftColor = Color.white;
            propertyField.style.borderRightColor = Color.white;
            propertyField.style.borderTopColor = Color.white;

            propertyField.style.borderTopWidth = 1;
            propertyField.style.borderBottomWidth = 1;

            deleteButton = new Button();
            deleteButton.Add(new Button(() => { RemoveDialogueMissionDataField(_dialogueMissionDataFieldList); CheckOverLap(_dialogueMissionDataFieldList, noticeLabel); }) { text = "삭제" });

            var textlabel = new Label("액터 데이터");
            actorInfoDataField = new ObjectField()
            {
                objectType = typeof(ActorInfoObjectScript),
            };
            actorInfoDataField.RegisterValueChangedCallback(evt =>
            {
                dialogueMissionData.actorInfoData = (ActorInfoObjectScript)evt.newValue;
                QuestDBWindowEditor.saveCheck = true;

                CheckOverLap(_dialogueMissionDataFieldList, noticeLabel);
            });
            actorInfoDataField.value = dialogueMissionData.actorInfoData;

            propertyField.Add(deleteButton);
            propertyField.Add(textlabel);
            propertyField.Add(actorInfoDataField);

            _parentElement.Add(propertyField);
        }

        public void RemoveDialogueMissionDataField(List<DialogueMissionDataField> _dialogueMissionDataFieldList)
        {
            _dialogueMissionDataFieldList.Remove(this);
            propertyField.Remove(deleteButton);
            propertyField.Remove(actorInfoDataField);
            parentElement.Remove(propertyField);
            dialogueMissionData = null;
        }

        public void CheckOverLap(List<DialogueMissionDataField> _dialogueMissionDataFieldList, Label noticeLabel)
        {
            if (_dialogueMissionDataFieldList.Count == 0)
            {
                noticeLabel.text = "데이터가 없습니다.";
            }
            else
            {
                //중복체크
                bool check = false;
                bool isNull = false;
                for (int i = 0; i < _dialogueMissionDataFieldList.Count; i++)
                {
                    if (_dialogueMissionDataFieldList[i] != this)
                    {
                        if (_dialogueMissionDataFieldList[i].GetDialogueMissionData().actorInfoData == null)
                        {
                            check = true;
                            isNull = true;
                            break;
                        }
                        else
                        {


                            if (_dialogueMissionDataFieldList[i].GetDialogueMissionData().actorInfoData == dialogueMissionData.actorInfoData)
                            {
                                check = true;
                                break;
                            }

                        }
                    }
                }
                if (check)
                {
                    if (isNull)
                    {
                        noticeLabel.text = "비어있는 데이터가 존재합니다";
                    }
                    else
                    {
                        noticeLabel.text = "중복된 데이터가 존재합니다";
                    }
                }
                else
                {
                    noticeLabel.text = "데이터에 이상이 없습니다.";
                }
            }
        }

        public DialogueMissionData GetDialogueMissionData()
        {
            return dialogueMissionData;
        }
    }

    //배틀미션필드
    public class BattleMissionDataField
    {
        private VisualElement parentElement;//부모
        private PropertyField propertyField;//바닥
        private Button deleteButton;//삭제버튼

        private BattleMissionData battleMissionData;

        private TextField missionIDField;
        private IntegerField valueField;
        public BattleMissionDataField(BattleMissionData _battleMissionData, List<BattleMissionDataField> _battleMissionDataFieldList, VisualElement _parentElement, Label noticeLabel)
        {
            battleMissionData = _battleMissionData;

            if (!_battleMissionDataFieldList.Contains(this))
            {
                _battleMissionDataFieldList.Add(this);
            }

            parentElement = _parentElement;
            propertyField = new PropertyField();
            //propertyField.style.borderColor = Color.white;
            propertyField.style.borderBottomColor = Color.white;
            propertyField.style.borderLeftColor = Color.white;
            propertyField.style.borderRightColor = Color.white;
            propertyField.style.borderTopColor = Color.white;

            propertyField.style.borderTopWidth = 1;
            propertyField.style.borderBottomWidth = 1;

            deleteButton = new Button();
            deleteButton.Add(new Button(() => { RemoveBattleMissionDataField(_battleMissionDataFieldList); CheckOverLap(_battleMissionDataFieldList, noticeLabel); }) { text = "삭제" });

            var missionIDFieldLabel = new Label("전투미션ID");
            missionIDField = new TextField();
            QuestDBWindowEditor.InitTextField(missionIDField);
            missionIDField.RegisterValueChangedCallback(evt =>
            {
                battleMissionData.missionTargetID = evt.newValue;
                QuestDBWindowEditor.saveCheck = true;

                CheckOverLap(_battleMissionDataFieldList, noticeLabel);
            });
            missionIDField.value = battleMissionData.missionTargetID;

            var valueFieldLabel = new Label("파괴할 수");
            valueField = new IntegerField();
            valueField.RegisterValueChangedCallback(intEvt =>
            {
                battleMissionData.value = intEvt.newValue;
                QuestDBWindowEditor.saveCheck = true;
            });
            valueField.value = battleMissionData.value;

            propertyField.Add(deleteButton);
            propertyField.Add(missionIDFieldLabel);
            propertyField.Add(missionIDField);
            propertyField.Add(valueFieldLabel);
            propertyField.Add(valueField);
            _parentElement.Add(propertyField);
        }

        public void RemoveBattleMissionDataField(List<BattleMissionDataField> _battleMissionDataFieldList)
        {
            _battleMissionDataFieldList.Remove(this);
            propertyField.Remove(deleteButton);
            propertyField.Remove(missionIDField);
            propertyField.Remove(valueField);
            parentElement.Remove(propertyField);
            battleMissionData = null;
        }

        public void CheckOverLap(List<BattleMissionDataField> _battleMissionDataFieldList, Label noticeLabel)
        {
            if (_battleMissionDataFieldList.Count == 0)
            {
                noticeLabel.text = "데이터가 없습니다.";
            }
            else
            {
                //중복체크
                bool check = false;
                bool isNull = false;
                for (int i = 0; i < _battleMissionDataFieldList.Count; i++)
                {
                    if (_battleMissionDataFieldList[i] != this)
                    {
                        if (string.IsNullOrEmpty(_battleMissionDataFieldList[i].GetBattleMissionData().missionTargetID))
                        {
                            check = true;
                            isNull = true;
                            break;
                        }
                        else
                        {
                            if (_battleMissionDataFieldList[i].GetBattleMissionData().missionTargetID == battleMissionData.missionTargetID)
                            {
                                check = true;
                                break;
                            }
                        }
                    }
                }
                if (check)
                {
                    if (isNull)
                    {
                        noticeLabel.text = "비어있는 데이터가 존재합니다";
                    }
                    else
                    {
                        noticeLabel.text = "중복된 데이터가 존재합니다";
                    }
                }
                else
                {
                    noticeLabel.text = "데이터에 이상이 없습니다.";
                }
            }

        }

        public BattleMissionData GetBattleMissionData()
        {
            return battleMissionData;
        }
    }
    //생존미션필드
    public class SurviveMissionDataField
    {
        private VisualElement parentElement;//부모
        private PropertyField propertyField;//바닥
        private Button deleteButton;//삭제버튼

        private SurviveMissionData surviveMissionData;

        private TextField missionIDField;
        private IntegerField valueField;

        public SurviveMissionDataField(SurviveMissionData _surviveMissionData, List<SurviveMissionDataField> _surviveMissionDataFieldList, VisualElement _parentElement, Label noticeLabel)
        {
            surviveMissionData = _surviveMissionData;

            if (!_surviveMissionDataFieldList.Contains(this))
            {
                _surviveMissionDataFieldList.Add(this);
            }

            parentElement = _parentElement;
            propertyField = new PropertyField();
            //propertyField.style.borderColor = Color.white;
            propertyField.style.borderBottomColor = Color.white;
            propertyField.style.borderLeftColor = Color.white;
            propertyField.style.borderRightColor = Color.white;
            propertyField.style.borderTopColor = Color.white;
            propertyField.style.borderTopWidth = 1;
            propertyField.style.borderBottomWidth = 1;

            deleteButton = new Button();
            deleteButton.Add(new Button(() => { RemoveSurviveMissionDataField(_surviveMissionDataFieldList); CheckOverLap(_surviveMissionDataFieldList, noticeLabel); }) { text = "삭제" });

            var missionIDFieldLabel = new Label("생존미션ID");
            missionIDField = new TextField();
            QuestDBWindowEditor.InitTextField(missionIDField);
            missionIDField.RegisterValueChangedCallback(evt =>
            {
                surviveMissionData.missionTargetID = evt.newValue;
                QuestDBWindowEditor.saveCheck = true;

                CheckOverLap(_surviveMissionDataFieldList, noticeLabel);
            });
            missionIDField.value = surviveMissionData.missionTargetID;

            var valueFieldLabel = new Label("살아남기 수");
            valueField = new IntegerField();
            valueField.RegisterValueChangedCallback(intEvt =>
            {
                surviveMissionData.value = intEvt.newValue;
                QuestDBWindowEditor.saveCheck = true;
            });
            valueField.value = surviveMissionData.value;

            propertyField.Add(deleteButton);
            propertyField.Add(missionIDFieldLabel);
            propertyField.Add(missionIDField);
            propertyField.Add(valueFieldLabel);
            propertyField.Add(valueField);
            _parentElement.Add(propertyField);
        }

        public void RemoveSurviveMissionDataField(List<SurviveMissionDataField> _surviveMissionDataField)
        {
            _surviveMissionDataField.Remove(this);
            propertyField.Remove(deleteButton);
            propertyField.Remove(missionIDField);
            propertyField.Remove(valueField);
            parentElement.Remove(propertyField);
            surviveMissionData = null;
        }

        public void CheckOverLap(List<SurviveMissionDataField> _surviveMissionDataFieldList, Label noticeLabel)
        {
            if (_surviveMissionDataFieldList.Count == 0)
            {
                noticeLabel.text = "데이터가 없습니다.";
            }
            else
            {
                //중복체크
                bool check = false;
                bool isNull = false;
                for (int i = 0; i < _surviveMissionDataFieldList.Count; i++)
                {
                    if (_surviveMissionDataFieldList[i] != this)
                    {
                        if (string.IsNullOrEmpty(_surviveMissionDataFieldList[i].GetSurviveMissionData().missionTargetID))
                        {
                            check = true;
                            isNull = true;
                            break;
                        }
                        else
                        {
                            if (_surviveMissionDataFieldList[i].GetSurviveMissionData().missionTargetID == surviveMissionData.missionTargetID)
                            {
                                check = true;
                                break;
                            }
                        }
                    }
                }
                if (check)
                {
                    if (isNull)
                    {
                        noticeLabel.text = "비어있는 데이터가 존재합니다";
                    }
                    else
                    {
                        noticeLabel.text = "중복된 데이터가 존재합니다";
                    }
                }
                else
                {
                    noticeLabel.text = "데이터에 이상이 없습니다.";
                }
            }
        }

        public SurviveMissionData GetSurviveMissionData()
        {
            return surviveMissionData;
        }
    }

    //유인이동미션필드
    public class AttractMoveMissionDataField
    {
        private VisualElement parentElement;//부모
        private PropertyField propertyField;//바닥
        private Button deleteButton;//삭제버튼

        private AttractMoveMissionData attractMoveMissionData;

        private EnumField unitTeamField;
        private Toggle isUseTargetUnitStatField;
        private ObjectField unitstatField;

        public AttractMoveMissionDataField(AttractMoveMissionData _attractMoveMissionData, List<AttractMoveMissionDataField> _attractMissionDataFieldList, VisualElement _parentElement, Label noticeLabel)
        {
            attractMoveMissionData = _attractMoveMissionData;

            if (!_attractMissionDataFieldList.Contains(this))
            {
                _attractMissionDataFieldList.Add(this);
            }

            parentElement = _parentElement;
            propertyField = new PropertyField();
            //propertyField.style.borderColor = Color.white;
            propertyField.style.borderBottomColor = Color.white;
            propertyField.style.borderLeftColor = Color.white;
            propertyField.style.borderRightColor = Color.white;
            propertyField.style.borderTopColor = Color.white;

            propertyField.style.borderTopWidth = 1;
            propertyField.style.borderBottomWidth = 1;

            deleteButton = new Button();
            deleteButton.Add(new Button(() => { RemoveAttractMissionDataField(_attractMissionDataFieldList); CheckOverLap(_attractMissionDataFieldList, noticeLabel); }) { text = "삭제" });

            var unitTeamFieldLabel = new Label("유닛팀타입");
            unitTeamField = new EnumField(attractMoveMissionData.unitTeamType);
            unitTeamField.RegisterValueChangedCallback(evt =>
            {
                attractMoveMissionData.unitTeamType = (UnitTeamType)evt.newValue;
                QuestDBWindowEditor.saveCheck = true;

                CheckOverLap(_attractMissionDataFieldList, noticeLabel);
            });
            unitTeamField.value = attractMoveMissionData.unitTeamType;

            isUseTargetUnitStatField = new Toggle("타겟스탯 사용여부");
            isUseTargetUnitStatField.RegisterValueChangedCallback(evt =>
            {
                attractMoveMissionData.isUseTargetUnitStat = evt.newValue;
                QuestDBWindowEditor.saveCheck = true;

                CheckOverLap(_attractMissionDataFieldList, noticeLabel);
            });
            isUseTargetUnitStatField.value = attractMoveMissionData.isUseTargetUnitStat;

            var unitstatFieldLabel = new Label("타겟이 될 스탯데이터");
            unitstatField = new ObjectField()
            {
                objectType = typeof(UnitStatusObjectScript),
            };
            unitstatField.RegisterValueChangedCallback(evt =>
            {
                attractMoveMissionData.unitStatusData = (UnitStatusObjectScript)evt.newValue;
                QuestDBWindowEditor.saveCheck = true;

                CheckOverLap(_attractMissionDataFieldList, noticeLabel);
            });
            unitstatField.value = attractMoveMissionData.unitStatusData;

            propertyField.Add(deleteButton);
            propertyField.Add(unitTeamFieldLabel);
            propertyField.Add(unitTeamField);
            propertyField.Add(isUseTargetUnitStatField);
            propertyField.Add(unitstatFieldLabel);
            propertyField.Add(unitstatField);
            _parentElement.Add(propertyField);
        }

        public void RemoveAttractMissionDataField(List<AttractMoveMissionDataField> _attractMissionDataFieldList)
        {
            _attractMissionDataFieldList.Remove(this);
            propertyField.Remove(deleteButton);
            propertyField.Remove(unitTeamField);
            propertyField.Remove(isUseTargetUnitStatField);
            propertyField.Remove(unitstatField);
            parentElement.Remove(propertyField);
            attractMoveMissionData = null;
        }

        public void CheckOverLap(List<AttractMoveMissionDataField> _attractMissionDataFieldList, Label noticeLabel)
        {
            if (_attractMissionDataFieldList.Count == 0)
            {
                noticeLabel.text = "데이터가 없습니다.";
            }
            else
            {
                //중복체크
                bool check = false;
                bool isNull = false;

                for (int i = 0; i < _attractMissionDataFieldList.Count; i++)
                {
                    if (_attractMissionDataFieldList[i] != this)
                    {
                        //유닛체크
                        if (attractMoveMissionData.isUseTargetUnitStat)
                        {
                            if (_attractMissionDataFieldList[i].GetAttractMissionData().unitStatusData == null)
                            {
                                check = true;
                                isNull = true;
                                break;
                            }
                            else
                            {


                                if (_attractMissionDataFieldList[i].GetAttractMissionData().unitStatusData == attractMoveMissionData.unitStatusData)
                                {
                                    check = true;
                                    break;
                                }

                            }
                        }
                        else
                        {
                            //해당 데이터도 유닛스탯을 사용안하고 같은 팀이면
                            if (!_attractMissionDataFieldList[i].GetAttractMissionData().isUseTargetUnitStat)
                            {

                                if (_attractMissionDataFieldList[i].GetAttractMissionData().unitTeamType == attractMoveMissionData.unitTeamType)
                                {
                                    check = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (check)
                {
                    if (isNull)
                    {
                        noticeLabel.text = "비어있는 데이터가 존재합니다";
                    }
                    else
                    {

                        noticeLabel.text = "중복된 데이터가 존재합니다";
                    }
                }
                else
                {
                    noticeLabel.text = "데이터에 이상이 없습니다.";
                }
            }
        }
        public AttractMoveMissionData GetAttractMissionData()
        {
            return attractMoveMissionData;
        }
    }

    //아이템모으기필드
    public class ItemGatherMissionDataField
    {
        private VisualElement parentElement;//부모
        private PropertyField propertyField;//바닥
        private Button deleteButton;//삭제버튼

        private ItemGatherMissionData itemGatherMissionData;

        private ObjectField itemDataField;
        private IntegerField itemCountField;
        public ItemGatherMissionDataField(ItemGatherMissionData _itemGatherMissionData, List<ItemGatherMissionDataField> _itemGatherMissionDataFieldList, VisualElement _parentElement, Label noticeLabel)
        {
            itemGatherMissionData = _itemGatherMissionData;

            if (!_itemGatherMissionDataFieldList.Contains(this))
            {
                _itemGatherMissionDataFieldList.Add(this);
            }

            parentElement = _parentElement;
            propertyField = new PropertyField();
            //propertyField.style.borderColor = Color.white;
            propertyField.style.borderBottomColor = Color.white;
            propertyField.style.borderLeftColor = Color.white;
            propertyField.style.borderRightColor = Color.white;
            propertyField.style.borderTopColor = Color.white;
            propertyField.style.borderTopWidth = 1;
            propertyField.style.borderBottomWidth = 1;

            deleteButton = new Button();
            deleteButton.Add(new Button(() => { RemoveItemGatherMissionDataField(_itemGatherMissionDataFieldList); CheckOverLap(_itemGatherMissionDataFieldList, noticeLabel); }) { text = "삭제" });

            var itemDataFieldLabel = new Label("아이템 데이터");
            itemDataField = new ObjectField()
            {
                objectType = typeof(ItemObjectScript),
            };
            itemDataField.RegisterValueChangedCallback(evt =>
            {
                itemGatherMissionData.itemData = (ItemObjectScript)evt.newValue;
                QuestDBWindowEditor.saveCheck = true;

                CheckOverLap(_itemGatherMissionDataFieldList, noticeLabel);
            });
            itemDataField.value = itemGatherMissionData.itemData;

            var itemCountFieldLabel = new Label("아이템 수량");
            itemCountField = new IntegerField();
            itemCountField.RegisterValueChangedCallback(intEvt =>
            {
                itemGatherMissionData.itemCount = intEvt.newValue;
                QuestDBWindowEditor.saveCheck = true;
            });
            itemCountField.value = itemGatherMissionData.itemCount;

            propertyField.Add(deleteButton);
            propertyField.Add(itemDataFieldLabel);
            propertyField.Add(itemDataField);
            propertyField.Add(itemCountFieldLabel);
            propertyField.Add(itemCountField);
            _parentElement.Add(propertyField);
        }

        public void RemoveItemGatherMissionDataField(List<ItemGatherMissionDataField> _itemGatherMissionDataFieldList)
        {
            _itemGatherMissionDataFieldList.Remove(this);
            propertyField.Remove(deleteButton);
            propertyField.Remove(itemDataField);
            propertyField.Remove(itemCountField);
            parentElement.Remove(propertyField);
            itemGatherMissionData = null;
        }

        public void CheckOverLap(List<ItemGatherMissionDataField> _itemGatherMissionDataFieldList, Label noticeLabel)
        {
            if (_itemGatherMissionDataFieldList.Count == 0)
            {
                noticeLabel.text = "데이터가 없습니다.";
            }
            else
            {
                //중복체크
                bool check = false;
                bool isNull = false;
                for (int i = 0; i < _itemGatherMissionDataFieldList.Count; i++)
                {
                    if (_itemGatherMissionDataFieldList[i] != this)
                    {
                        if (_itemGatherMissionDataFieldList[i].GetItemGatherMissionData().itemData == null)
                        {
                            check = true;
                            isNull = true;
                            break;
                        }
                        else
                        {


                            if (_itemGatherMissionDataFieldList[i].GetItemGatherMissionData().itemData == itemGatherMissionData.itemData)
                            {
                                check = true;
                                break;
                            }

                        }
                    }
                }
                if (check)
                {
                    if (isNull)
                    {
                        noticeLabel.text = "비어있는 데이터가 존재합니다";
                    }
                    else
                    {
                        noticeLabel.text = "중복된 데이터가 존재합니다";
                    }
                }
                else
                {
                    noticeLabel.text = "데이터에 이상이 없습니다.";
                }
            }
        }
        public ItemGatherMissionData GetItemGatherMissionData()
        {
            return itemGatherMissionData;
        }
    }

    //도착맵필드
    public class ArriveMapMissionDataField
    {
        private VisualElement parentElement;//부모
        private PropertyField propertyField;//바닥
        private Button deleteButton;//삭제버튼

        private ArriveMapMissionData arriveMapMissionData;

        private ObjectField mapMarkerDataField;

        public ArriveMapMissionDataField(ArriveMapMissionData _arriveMapMissionData, List<ArriveMapMissionDataField> _arriveMapMissionDataFieldList, VisualElement _parentElement, Label noticeLabel)
        {
            arriveMapMissionData = _arriveMapMissionData;

            if (!_arriveMapMissionDataFieldList.Contains(this))
            {
                _arriveMapMissionDataFieldList.Add(this);
            }

            parentElement = _parentElement;
            propertyField = new PropertyField();
            //propertyField.style.borderColor = Color.white;
            propertyField.style.borderBottomColor = Color.white;
            propertyField.style.borderLeftColor = Color.white;
            propertyField.style.borderRightColor = Color.white;
            propertyField.style.borderTopColor = Color.white;
            propertyField.style.borderTopWidth = 1;
            propertyField.style.borderBottomWidth = 1;

            deleteButton = new Button();
            deleteButton.Add(new Button(() => { RemoveArriveMapMissionField(_arriveMapMissionDataFieldList); CheckOverLap(_arriveMapMissionDataFieldList, noticeLabel); }) { text = "삭제" });

            var mapMarkerDataFieldLabel = new Label("맵마커 데이터");
            mapMarkerDataField = new ObjectField()
            {
                objectType = typeof(MapMarkerObjectScript),
            };
            mapMarkerDataField.RegisterValueChangedCallback(evt =>
            {
                arriveMapMissionData.mapMarkerData = (MapMarkerObjectScript)evt.newValue;
                QuestDBWindowEditor.saveCheck = true;

                CheckOverLap(_arriveMapMissionDataFieldList, noticeLabel);
            });
            mapMarkerDataField.value = arriveMapMissionData.mapMarkerData;

            propertyField.Add(deleteButton);
            propertyField.Add(mapMarkerDataFieldLabel);
            propertyField.Add(mapMarkerDataField);
            _parentElement.Add(propertyField);
        }

        public void RemoveArriveMapMissionField(List<ArriveMapMissionDataField> _arriveMapMissionFieldList)
        {
            _arriveMapMissionFieldList.Remove(this);
            propertyField.Remove(deleteButton);
            propertyField.Remove(mapMarkerDataField);
            parentElement.Remove(propertyField);
            arriveMapMissionData = null;
        }

        public void CheckOverLap(List<ArriveMapMissionDataField> _arriveMapMissionDataFieldList, Label noticeLabel)
        {
            if (_arriveMapMissionDataFieldList.Count == 0)
            {
                noticeLabel.text = "데이터가 없습니다.";
            }
            else
            {
                //중복체크
                bool check = false;
                bool isNull = false;
                for (int i = 0; i < _arriveMapMissionDataFieldList.Count; i++)
                {
                    if (_arriveMapMissionDataFieldList[i] != this)
                    {
                        if (_arriveMapMissionDataFieldList[i].GetArriveMapMissionData().mapMarkerData == null)
                        {
                            check = true;
                            isNull = true;
                            break;
                        }
                        else
                        {


                            if (_arriveMapMissionDataFieldList[i].GetArriveMapMissionData().mapMarkerData == arriveMapMissionData.mapMarkerData)
                            {
                                check = true;
                                break;
                            }

                        }
                    }
                }
                if (check)
                {
                    if (isNull)
                    {
                        noticeLabel.text = "비어있는 데이터가 존재합니다";
                    }
                    else
                    {
                        noticeLabel.text = "중복된 데이터가 존재합니다";
                    }
                }
                else
                {
                    noticeLabel.text = "데이터에 이상이 없습니다.";
                }
            }
        }

        public ArriveMapMissionData GetArriveMapMissionData()
        {
            return arriveMapMissionData;
        }
    }

    //보상필드
    public class RewardDataField
    {
        //보상
        private VisualElement parentElement;//부모
        private PropertyField propertyField;//바닥
        private Button deleteButton;//삭제버튼

        private RewardData rewardData;

        private ObjectField itemDataField;
        private IntegerField itemCountField;

        private ObjectField targetUnitObjectField;
        private IntegerField unitCountField;

        private EnumField unitExprientceGiveField;
        private IntegerField unitExprientceField;

        private EnumField skillPointGiveField;
        private IntegerField skillPointField;

        private EnumField skillDataGiveField;
        private ObjectField skillDataField;

        private ObjectField objectDeScriptionDataField;
        private ObjectField targetObjectField;

        public RewardDataField(RewardData _rewardData, List<RewardDataField> _rewardDataFieldList, VisualElement _parentElement)
        {
            rewardData = _rewardData;

            if (!_rewardDataFieldList.Contains(this))
            {
                _rewardDataFieldList.Add(this);
            }

            parentElement = _parentElement;
            propertyField = new PropertyField();
            //propertyField.style.borderColor = Color.white;
            propertyField.style.borderBottomColor = Color.white;
            propertyField.style.borderLeftColor = Color.white;
            propertyField.style.borderRightColor = Color.white;
            propertyField.style.borderTopColor = Color.white;
            propertyField.style.borderTopWidth = 1;
            propertyField.style.borderBottomWidth = 1;

            deleteButton = new Button();
            deleteButton.Add(new Button(() => { RemoveRewardField(_rewardDataFieldList); }) { text = "삭제" });

            var itemDataFieldLabel = new Label("아이템 데이터");
            itemDataField = new ObjectField()
            {
                objectType = typeof(ItemObjectScript),
            };
            itemDataField.RegisterValueChangedCallback(evt =>
            {
                rewardData.itemData = (ItemObjectScript)evt.newValue;
                QuestDBWindowEditor.saveCheck = true;
            });
            itemDataField.value = rewardData.itemData;

            var itemCountFieldLabel = new Label("아이템 수");
            itemCountField = new IntegerField();
            itemCountField.RegisterValueChangedCallback(evt =>
            {
                rewardData.itemCount = evt.newValue;
                QuestDBWindowEditor.saveCheck = true;
            });
            itemCountField.value = rewardData.itemCount;

            var targetUnitObjectFieldLabel = new Label("유닛 데이터");
            targetUnitObjectField = new ObjectField()
            {
                objectType = typeof(TestWorldUnitObject),
            };
            targetUnitObjectField.RegisterValueChangedCallback(evt =>
            {
                rewardData.targetUnitObject = (TestWorldUnitObject)evt.newValue;
                QuestDBWindowEditor.saveCheck = true;
            });
            targetUnitObjectField.value = rewardData.targetUnitObject;

            var unitCountFieldLabel = new Label("유닛 수");
            unitCountField = new IntegerField();
            unitCountField.RegisterValueChangedCallback(evt =>
            {
                rewardData.unitCount = evt.newValue;
                QuestDBWindowEditor.saveCheck = true;
            });
            unitCountField.value = rewardData.unitCount;

            var unitExprientceGiveFieldLabel = new Label("경험치주는 방식");
            unitExprientceGiveField = new EnumField(rewardData.unitExprienceGiveType);
            unitExprientceGiveField.RegisterValueChangedCallback(evt =>
            {
                rewardData.unitExprienceGiveType = (GiveType)evt.newValue;
                QuestDBWindowEditor.saveCheck = true;
            });

            var unitExprientceFieldLabel = new Label("경험치량");
            unitExprientceField = new IntegerField();
            unitExprientceField.RegisterValueChangedCallback(evt =>
            {
                rewardData.unitExprience = evt.newValue;
                QuestDBWindowEditor.saveCheck = true;
            });
            unitExprientceField.value = rewardData.unitExprience;


            var skillPointGiveFieldLabel = new Label("스킬포인트주는 방식");
            skillPointGiveField = new EnumField(rewardData.skillPointGiveType);
            skillPointGiveField.RegisterValueChangedCallback(evt =>
            {
                rewardData.skillPointGiveType = (GiveType)evt.newValue;
                QuestDBWindowEditor.saveCheck = true;
            });

            var skillPointFieldLabel = new Label("스킬포인트");
            skillPointField = new IntegerField();
            skillPointField.RegisterValueChangedCallback(evt =>
            {
                rewardData.skillPoint = evt.newValue;
                QuestDBWindowEditor.saveCheck = true;
            });
            skillPointField.value = rewardData.skillPoint;

            var skillDataGiveFieldLabel = new Label("스킬데이터주는 방식");
            skillDataGiveField = new EnumField(rewardData.skillDataGiveType);
            skillDataGiveField.RegisterValueChangedCallback(evt =>
            {
                rewardData.skillDataGiveType = (GiveType)evt.newValue;
                QuestDBWindowEditor.saveCheck = true;
            });

            var skillDataFieldLabel = new Label("스킬데이터");
            skillDataField = new ObjectField()
            {
                objectType = typeof(SkillObjectScript),
            };
            skillDataField.RegisterValueChangedCallback(evt =>
            {
                rewardData.skillData = (SkillObjectScript)evt.newValue;
                QuestDBWindowEditor.saveCheck = true;
            });
            skillDataField.value = rewardData.skillData;

            var objectDeScriptionDataFieldLabel = new Label("게임오브젝트 설명데이터");
            objectDeScriptionDataField = new ObjectField()
            {
                objectType = typeof(ObjectDeScription_Base),
            };
            objectDeScriptionDataField.RegisterValueChangedCallback(evt =>
            {
                rewardData.objectDeScriptionData = (ObjectDeScription_Base)evt.newValue;
                QuestDBWindowEditor.saveCheck = true;
            });
            objectDeScriptionDataField.value = rewardData.objectDeScriptionData;

            var targetObjectFieldLabel = new Label("게임오브젝트");
            targetObjectField = new ObjectField()
            {
                objectType = typeof(GameObject),
            };
            targetObjectField.RegisterValueChangedCallback(evt =>
            {
                rewardData.targetObject = (GameObject)evt.newValue;
                QuestDBWindowEditor.saveCheck = true;
            });
            targetObjectField.value = rewardData.targetObject;

            propertyField.Add(deleteButton);
            propertyField.Add(itemDataFieldLabel);
            propertyField.Add(itemDataField);
            propertyField.Add(itemCountFieldLabel);
            propertyField.Add(itemCountField);
            propertyField.Add(targetUnitObjectFieldLabel);
            propertyField.Add(targetUnitObjectField);
            propertyField.Add(unitCountFieldLabel);
            propertyField.Add(unitCountField);
            propertyField.Add(unitExprientceGiveFieldLabel);
            propertyField.Add(unitExprientceGiveField);
            propertyField.Add(unitExprientceFieldLabel);
            propertyField.Add(unitExprientceField);
            propertyField.Add(skillPointGiveFieldLabel);
            propertyField.Add(skillPointGiveField);
            propertyField.Add(skillPointFieldLabel);
            propertyField.Add(skillPointField);
            propertyField.Add(skillPointGiveFieldLabel);
            propertyField.Add(skillPointGiveField);
            propertyField.Add(skillDataFieldLabel);
            propertyField.Add(skillDataField);
            propertyField.Add(objectDeScriptionDataFieldLabel);
            propertyField.Add(objectDeScriptionDataField);
            propertyField.Add(targetObjectFieldLabel);
            propertyField.Add(targetObjectField);
            _parentElement.Add(propertyField);
        }

        public void RemoveRewardField(List<RewardDataField> _rewardFieldList)
        {
            _rewardFieldList.Remove(this);
            //propertyField.Remove(deleteButton);
            //propertyField.Remove(itemDataField);
            //propertyField.Remove(itemCountField);
            //propertyField.Remove(targetUnitObjectField);
            //propertyField.Remove(unitCountField);
            //propertyField.Remove(unitExprientceGiveField);
            //propertyField.Remove(unitExprientceField);
            //propertyField.Remove(skillPointGiveField);
            //propertyField.Remove(skillPointField);
            //propertyField.Remove(skillDataGiveField);
            //propertyField.Remove(skillDataField);
            //propertyField.Remove(objectDeScriptionDataField);
            //propertyField.Remove(targetObjectField);
            parentElement.Remove(propertyField);
            rewardData = null;
        }

        public RewardData GetRewardData()
        {
            return rewardData;
        }
    }

    class PrecedeQuestBookField
    {
        //선행퀘스트        
        private VisualElement parentElement;//부모
        private PropertyField propertyField;//바닥        

        private Button deleteButton;//삭제버튼
        private PopupField<string> precedeQuestBookIDKeyField;//선행필드키        
        private string precedeQuestBookIDKey = "";

        public PrecedeQuestBookField(List<PrecedeQuestBookField> _precedeQuestBookFieldList, VisualElement _parentElement, List<QuestBookDataField> _questBookDataFieldList, string targetPrecadeQuestBookIDKey = "")
        {
            if (!_precedeQuestBookFieldList.Contains(this))
            {
                _precedeQuestBookFieldList.Add(this);
            }

            parentElement = _parentElement;
            propertyField = new PropertyField();
            //propertyField.style.borderColor = Color.white;
            propertyField.style.borderBottomColor = Color.white;
            propertyField.style.borderLeftColor = Color.white;
            propertyField.style.borderRightColor = Color.white;
            propertyField.style.borderTopColor = Color.white;
            propertyField.style.borderTopWidth = 2;
            propertyField.style.borderBottomWidth = 2;
            propertyField.style.borderRightWidth = 1;
            propertyField.style.borderLeftWidth = 1;

            deleteButton = new Button();
            deleteButton.Add(new Button(() => { RemovePrecedeQuestBookField(_precedeQuestBookFieldList); }) { text = "삭제" });

            //새로고침체크
            List<string> questBookIDList = new List<string>();
            for (int i = 0; i < _questBookDataFieldList.Count; i++)
            {
                questBookIDList.Add(_questBookDataFieldList[i].GetQuestBookData().questBookIDKey);
            }

            precedeQuestBookIDKeyField = new PopupField<string>("선행퀘스트ID", questBookIDList, 0);
            precedeQuestBookIDKeyField.RegisterValueChangedCallback(evt =>
            {
                precedeQuestBookIDKey = evt.newValue;
                //새로고침
                questBookIDList.Clear();
                for (int i = 0; i < _questBookDataFieldList.Count; i++)
                {
                    questBookIDList.Add(_questBookDataFieldList[i].GetQuestBookData().questBookIDKey);
                }
            });
            precedeQuestBookIDKeyField.value = questBookIDList[0];

            if (targetPrecadeQuestBookIDKey != "")
            {
                precedeQuestBookIDKey = targetPrecadeQuestBookIDKey;
                precedeQuestBookIDKeyField.value = targetPrecadeQuestBookIDKey;
            }

            propertyField.Add(deleteButton);
            propertyField.Add(precedeQuestBookIDKeyField);
            _parentElement.Add(propertyField);
        }

        public void RemovePrecedeQuestBookField(List<PrecedeQuestBookField> _precedeQuestBookFieldList)
        {
            _precedeQuestBookFieldList.Remove(this);
            propertyField.Remove(deleteButton);
            propertyField.Remove(precedeQuestBookIDKeyField);
            parentElement.Remove(propertyField);
        }

        public string GetPrecedeQuestBookID()
        {
            return precedeQuestBookIDKey;
        }
    }

    class QuestBookDataField
    {
        private VisualElement parentElement;//부모
        private PropertyField propertyField;//바닥
        private List<QuestBookDataField> questBookDataFieldList;

        private QuestBookData questBookData;//타겟이 된 대화데이터        
        private Button selectButton;//로드와 저장이 진행됨            
        private Button deleteButton;//삭제버튼    

        private ObjectField actorInfoDataField;

        private TextField questBookIntroduceField;
        private TextField questBookTitleField;
        private TextField questBookContentInfoField;
        private TextField questBookIDKeyField;

        private Button addPrecedeQuestButton;
        private ScrollView precedeQuestBookScrollView;
        private List<PrecedeQuestBookField> precedeQuestBookFieldList = new List<PrecedeQuestBookField>();

        private EnumField questTypeField;
        private Toggle isShowToUserField;
        private Toggle isGiveUpQuestField;
        private Toggle isLoopQuestField;

        public QuestBookDataField(QuestBookData _questBookData, List<QuestBookDataField> _questBookDataFieldList, VisualElement _parentElement, UnityAction<QuestBookData> selectAction, UnityAction deleteAction)
        {
            questBookData = _questBookData;
            questBookDataFieldList = _questBookDataFieldList;

            if (!_questBookDataFieldList.Contains(this))
            {
                _questBookDataFieldList.Add(this);
            }

            parentElement = _parentElement;
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

            selectButton = new Button();
            selectButton.Add(new Button(() => selectAction(questBookData)) { text = "선택 로드" });

            deleteButton = new Button();
            deleteButton.Add(new Button(() => { RemoveQuestBookDataField(questBookDataFieldList); deleteAction(); }) { text = "퀘스트북데이터삭제" });

            actorInfoDataField = new ObjectField("액터 데이터")
            {
                objectType = typeof(ActorInfoObjectScript),
            };

            actorInfoDataField.RegisterValueChangedCallback(evt =>
            {
                questBookData.actorInfoData = (ActorInfoObjectScript)evt.newValue;
                QuestDBWindowEditor.saveCheck = true;
            });

            questBookIntroduceField = new TextField("퀘스트북 소개");
            QuestDBWindowEditor.InitTextField(questBookIntroduceField);
            questBookIntroduceField.RegisterValueChangedCallback(dialogueIDKeyEvt =>
            {
                questBookData.questBookIntroduce = dialogueIDKeyEvt.newValue;
                QuestDBWindowEditor.saveCheck = true;
            });
            questBookIntroduceField.value = questBookData.questBookIntroduce;

            questBookTitleField = new TextField("퀘스트북 제목");
            QuestDBWindowEditor.InitTextField(questBookTitleField);
            questBookTitleField.RegisterValueChangedCallback(dialogueIDKeyEvt =>
            {
                questBookData.questBookTitle = dialogueIDKeyEvt.newValue;
                QuestDBWindowEditor.saveCheck = true;
            });
            questBookTitleField.value = questBookData.questBookTitle;

            questBookContentInfoField = new TextField("퀘스트북 내용정보");
            QuestDBWindowEditor.InitTextField(questBookContentInfoField);
            questBookContentInfoField.RegisterValueChangedCallback(dialogueIDKeyEvt =>
            {
                questBookData.questBookContentInfo = dialogueIDKeyEvt.newValue;
                QuestDBWindowEditor.saveCheck = true;
            });
            questBookContentInfoField.value = questBookData.questBookTitle;

            questBookIDKeyField = new TextField("퀘스트북ID");
            QuestDBWindowEditor.InitTextField(questBookIDKeyField);
            questBookIDKeyField.RegisterValueChangedCallback(dialogueIDKeyEvt =>
            {
                questBookData.questBookIDKey = dialogueIDKeyEvt.newValue;
                QuestDBWindowEditor.saveCheck = true;
            });
            questBookIDKeyField.value = questBookData.questBookIDKey;


            addPrecedeQuestButton = new Button();
            addPrecedeQuestButton.Add(new Button(() =>
            {
                PrecedeQuestBookField precedeQuestBookField = new PrecedeQuestBookField(precedeQuestBookFieldList, precedeQuestBookScrollView, questBookDataFieldList);
            })
            { text = "선행퀘스트 추가" });

            precedeQuestBookScrollView = new ScrollView();
            //dialogueDataScrollView.style.height = dialogueDBWindowEditorWindowSize.y;
            precedeQuestBookScrollView.showVertical = true;
            precedeQuestBookScrollView.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);

            var questTypeField = new EnumField("퀘스트미션타입", questBookData.questType);
            questTypeField.RegisterValueChangedCallback(evt =>
            {
                questBookData.questType = (QuestMissionCATType)evt.newValue;
                QuestDBWindowEditor.saveCheck = true;
            });

            isShowToUserField = new Toggle("유저에게 UI를 보여주는 여부");
            isShowToUserField.RegisterValueChangedCallback(isStopTimeEvt =>
            {
                questBookData.isShowToUser = isStopTimeEvt.newValue;
                QuestDBWindowEditor.saveCheck = true;
            });
            isShowToUserField.value = questBookData.isShowToUser;

            isGiveUpQuestField = new Toggle("포기가능한 퀘스트여부");
            isGiveUpQuestField.RegisterValueChangedCallback(isStopTimeEvt =>
            {
                questBookData.isGiveUpQuest = isStopTimeEvt.newValue;
                QuestDBWindowEditor.saveCheck = true;
            });
            isGiveUpQuestField.value = questBookData.isGiveUpQuest;

            isLoopQuestField = new Toggle("반복퀘스트 여부");
            isLoopQuestField.RegisterValueChangedCallback(isStopTimeEvt =>
            {
                questBookData.isLoopQuest = isStopTimeEvt.newValue;
                QuestDBWindowEditor.saveCheck = true;
            });
            isLoopQuestField.value = questBookData.isLoopQuest;


            propertyField.Add(selectButton);
            propertyField.Add(deleteButton);
            propertyField.Add(actorInfoDataField);
            propertyField.Add(questBookIntroduceField);
            propertyField.Add(questBookTitleField);
            propertyField.Add(questBookContentInfoField);
            propertyField.Add(questBookIDKeyField);
            propertyField.Add(addPrecedeQuestButton);
            propertyField.Add(precedeQuestBookScrollView);
            propertyField.Add(questTypeField);
            propertyField.Add(isShowToUserField);
            propertyField.Add(isGiveUpQuestField);
            propertyField.Add(isLoopQuestField);

            _parentElement.Add(propertyField);
        }

        public void LoadPrecadeQuest(QuestBookData _questBookData)
        {
            //선행퀘스트재생
            for (int i = 0; i < _questBookData.precedeQuestBookIDKeyArray.Length; i++)
            {
                PrecedeQuestBookField precedeQuestBookField = new PrecedeQuestBookField(precedeQuestBookFieldList, precedeQuestBookScrollView, questBookDataFieldList, _questBookData.precedeQuestBookIDKeyArray[i]);
            }
        }

        public void RemoveQuestBookDataField(List<QuestBookDataField> _questBookDataFieldList)
        {
            _questBookDataFieldList.Remove(this);
            //propertyField.Remove(selectButton);
            //propertyField.Remove(deleteButton);
            //propertyField.Remove(actorInfoDataField);
            //propertyField.Remove(questBookIntroduceField);
            //propertyField.Remove(questBookTitleField);
            //propertyField.Remove(questBookContentInfoField);
            //propertyField.Remove(questBookIDKeyField);
            //propertyField.Remove(addPrecedeQuestButton);
            //propertyField.Remove(precedeQuestBookScrollView);
            //propertyField.Remove(questTypeField);
            //propertyField.Remove(isShowToUserField);
            //propertyField.Remove(isGiveUpQuestField);
            //propertyField.Remove(isLoopQuestField);

            parentElement.Remove(propertyField);
            questBookData = null;
        }


        public string[] GetPrecedeQuestBookIDKeyArray()
        {
            List<string> tempStringList = new List<string>();
            for (int i = 0; i < precedeQuestBookFieldList.Count; i++)
            {
                tempStringList.Add(precedeQuestBookFieldList[i].GetPrecedeQuestBookID());
            }
            return tempStringList.ToArray();
        }

        public QuestBookData GetQuestBookData()
        {
            return questBookData;
        }
    }
}
#endif
#endif
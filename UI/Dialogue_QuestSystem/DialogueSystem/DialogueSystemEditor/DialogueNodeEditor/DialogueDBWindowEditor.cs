#if Doozy

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Events;
using TMPro;
using lLCroweTool.DialogueSystem;
using lLCroweTool.QuestSystem;
using lLCroweTool.Localize;
#if UNITY_EDITOR
using UnityEditor;
#pragma warning disable 0618
namespace lLCroweTool.QC.EditorOnly
{

    public class DialogueDBWindowEditor : EditorWindow
    {
        //대화DB를 세팅해줄 윈도우 데이터
        private static DialogueDBObjectScript dialogueDBData;
        private static bool inspectorOpen = false;
        private ObjectField dialogueDBDataField;

        private DialogueDataNodeGraphView _graphView;
        private List<Edge> Edges => _graphView.edges.ToList();//그래프뷰의 연결된 선들
        private List<DialogueUINode> Nodes => _graphView.nodes.ToList().Cast<DialogueUINode>().ToList();//그래프뷰의 노드들                
        
        private VisualElement bodyElement;//그래프가 들어갈 최상단 부모앨리먼트
        private VisualElement bodyElement2;//그래프가 들어갈 상단 부모앨리먼트
        private PropertyType propertyType;
        
        private static Vector2 dialogueDBWindowEditorWindowSize = new Vector2(1500, 700);//사이즈

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
        private TMP_FontAsset defaultFontAsset;
        private ObjectField defaultFontAssetObjectField;
        private QuestDBObjectScript questDBData;
        private ObjectField questDBDataField;
        private ObjectField gameWorldEventDBDataField;

        private PropertyField propertyDBDataField;

        //대화데이터 찾는 아이디
        private string findDataID;

        //대화데이터 스크롤뷰
        private ScrollView dialogueDataScrollView;
        private static List<DialogueDataField> dialogueDataFieldList = new List<DialogueDataField>();
        private DialogueData targetDialogueData;
        private Label selectNameLabelField;

        public static bool saveCheck = false;//트루여야지 경고창이 나옴


        [MenuItem("lLcroweTool/DialogueDBWindowEditor")]
        public static void ShowWindow()
        {
            // 생성되어있는 윈도우를 가져온다. 없으면 새로 생성한다. 싱글턴 구조인듯하다.
            DialogueDBWindowEditor editorWindow = (DialogueDBWindowEditor)GetWindow(typeof(DialogueDBWindowEditor));            
            editorWindow.titleContent.text = "대화데이터 노드그래프 에디터";
            editorWindow.minSize = dialogueDBWindowEditorWindowSize;
            editorWindow.maxSize = dialogueDBWindowEditorWindowSize;
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
            customValueBox.style.height = dialogueDBWindowEditorWindowSize.y;
            rootVisualElement.Add(customValueBox);

            //버튼들이 있는 프로퍼티         

           //대화DB관련 버튼
            propertyDBDataField = new PropertyField();
            propertyDBDataField.style.width = 300;
            propertyDBDataField.style.height = 510;
            customValueBox.Add(propertyDBDataField);

            dialogueDBDataField = new ObjectField("참조할 대화DB데이터")
            {
                objectType = typeof(DialogueDBObjectScript),
            };
            dialogueDBDataField.value = dialogueDBData;
            dialogueDBDataField.RegisterValueChangedCallback(dialogueDBDataEvt =>
            {
                dialogueDBData = (DialogueDBObjectScript)dialogueDBDataEvt.newValue;
            });
            propertyDBDataField.Add(dialogueDBDataField);

            //대화DB 저장 로드 관련
            propertyDBDataField.Add(new Button(() =>
            {
                RequestDBDataOperation(true);

                //Sync LocalizeData//로컬라이징 데이터와 동기화
                if (dialogueDBData == null)
                {
                    return;
                }
                //_dialogueDataID + "_" + _dialogueContentData.localizeID

                List<string> tempLocalizeIDList = new List<string>();
                List<string> overLapDebugList = new List<string>();//중복처리됫을때 보여주는 문자
                //일단은 로컬라이즈ID 추가
                for (int i = 0; i < dialogueDBData.dialogueDataArray.Length; i++)
                {
                    string tempDebug = dialogueDBData.dialogueDataArray[i].dialogueIDKey;
                    //로컬라이즈ID가 필요한곳은 두종류다
                    //1.대화내용데이터
                    for (int j = 0; j < dialogueDBData.dialogueDataArray[i].dialogueContentDataArray.Length; j++)
                    {
                        string temp = dialogueDBData.dialogueDataArray[i].dialogueContentDataArray[j].localizeID;

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
                    }

                    //2.선택분기노드의 데이터
                    for (int j = 0; j < dialogueDBData.dialogueDataArray[i].selectBrandNodeDataArray.Length; j++)
                    {
                        //선택분기의 로컬라이즈 배열들
                        for (int k = 0; k < dialogueDBData.dialogueDataArray[i].selectBrandNodeDataArray[j].localizeArray.Length; k++)
                        {
                            string temp = dialogueDBData.dialogueDataArray[i].selectBrandNodeDataArray[j].localizeArray[k];
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

            }) { text = "Save DBData" });
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
                
            }) { text = "Load DBData" });
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

            questDBDataField = new ObjectField("참조할 퀘스트DB 데이터")
            {
                objectType = typeof(QuestDBObjectScript),
            };

            questDBDataField.RegisterValueChangedCallback(evt =>
            {
                questDBData = (QuestDBObjectScript)evt.newValue;
                saveCheck = true;
            });

            defaultFontAssetObjectField = new ObjectField("기본으로 사용하는 폰트")
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
            propertyDBDataField.Add(questDBDataField);
            propertyDBDataField.Add(gameWorldEventDBDataField);
            propertyDBDataField.Add(defaultFontAssetObjectField);

            //표시용
            selectNameLabelField = new Label();
            selectNameLabelField.style.color = Color.white;
            selectNameLabelField.text = "-=선택한게 없음=-";
            propertyDBDataField.Add(selectNameLabelField);


            //대화데이터 관련 버튼
            //찾기
            TextField textField = new TextField("찾을 대화데이터 이름을 쓰세요");
            //한글이 안쳐질떄 사용법
            InitTextField(textField);
            textField.RegisterValueChangedCallback(findEvt =>
            {
                findDataID = findEvt.newValue;
                saveCheck = true;
            });
            propertyDBDataField.Add(textField);
            propertyDBDataField.Add(new Button(() => FindDialogueData(findDataID)) { text = "Find DialogueData" });


            //다있을거니 찾아서 변경해줘야됨 삭제만 없네 버튼과 삭제버튼, 저장 ,로드            
            //생성 (완료)

            //대화데이터필드에 존재
            //삭제, 선택 (완료)

            propertyDBDataField.Add(new Button(() => 
            {
                if (dialogueDBData == null)
                {
                    return;
                }
                DialogueData dialogueData = new DialogueData();
                dialogueData.dialogueIDKey = dialogueDataFieldList.Count.ToString();
                DialogueDataField dialogueDataField = new DialogueDataField(dialogueData, dialogueDataFieldList, dialogueDataScrollView, SelectDialogueDataField, deleteDialogueDataField);
                saveCheck = true;
            } ) { text = "Add DialogueData" });

            //대화데이터의 리스트가 떠야됨
            //리스트생성 함수필요
            //대화데이터들이 있는 스크롤 뷰
            dialogueDataScrollView = new ScrollView();
            dialogueDataScrollView.style.height = dialogueDBWindowEditorWindowSize.y;
            dialogueDataScrollView.showVertical = true;
            dialogueDataScrollView.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            customValueBox.Add(dialogueDataScrollView);          

            //노드그래프 생성
            bodyElement2 = new VisualElement();
            bodyElement2.style.width = dialogueDBWindowEditorWindowSize.x - 300;
            bodyElement2.style.height = dialogueDBWindowEditorWindowSize.y;
            bodyElement2.style.flexDirection = FlexDirection.RowReverse;
            bodyElement2.style.alignItems = Align.FlexEnd;
            bodyElement.Add(bodyElement2);
            ConstructGraphView(bodyElement2);

            //인스팩터에서 열기
            if (inspectorOpen)
            {
                fileName = dialogueDBData.name;
                version = dialogueDBData.version;
                author = dialogueDBData.author;
                description = dialogueDBData.description;
                questDBData = dialogueDBData.questDBData;
                //gameWorldEventDBData = dialogueDBData.gameWorldEventDBData;
                defaultFontAsset = dialogueDBData.defaultFontAsset;

                RequestDBDataOperation(false);
                inspectorOpen = false;
            }
        }

        private void OnDisable()
        {
            //지우기           
            dialogueDataFieldList.Clear();
            dialogueDataScrollView.Clear();

            propertyDBDataField.Remove(dialogueDBDataField);            
            propertyDBDataField.Remove(fileNameField);
            propertyDBDataField.Remove(versionField);
            propertyDBDataField.Remove(authorField);
            propertyDBDataField.Remove(descriptionField);
            propertyDBDataField.Remove(questDBDataField);
            propertyDBDataField.Remove(gameWorldEventDBDataField);
            propertyDBDataField.Remove(defaultFontAssetObjectField);
            propertyDBDataField.Remove(selectNameLabelField);

            bodyElement2.Remove(_graphView);            
            bodyElement.Remove(bodyElement2);            
            rootVisualElement.Remove(bodyElement);

            dialogueDBData = null;
            saveCheck = false;
        }

        public static List<DialogueData> GetDialogueDataList()
        {
            List<DialogueData> dialogueDataList = new List<DialogueData>();
            for (int i = 0; i < dialogueDataFieldList.Count; i++)
            {
                dialogueDataList.Add(dialogueDataFieldList[i].GetDialogueData());
            }
            return dialogueDataList;
        }

        /// <summary>
        /// 그래프뷰생성
        /// </summary>
        private void ConstructGraphView(VisualElement targetElement)
        {
            //그려주는 위치
            _graphView = new DialogueDataNodeGraphView
            {
                name = "UI노드데이터 에디터관리자"                
            };

            targetElement.Add(_graphView);
            _graphView.StretchToParentSize();

            //우측지점에 변수데이터
            UnityEngine.UIElements.Box customValueBox = new UnityEngine.UIElements.Box();
            customValueBox.style.width = 250;
            customValueBox.style.height = dialogueDBWindowEditorWindowSize.y - 40;
            

            ScrollView m_TasksContainer = new ScrollView();
            _graphView.scrollView = m_TasksContainer;
            m_TasksContainer.style.height = dialogueDBWindowEditorWindowSize.y;
            m_TasksContainer.showVertical = true;
            m_TasksContainer.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f);
            
            //리스트뷰에 그래프뷰의 커스텀밸류 리스트를 집어넣는다
            //리스트를 집어넣을때 해당뷰에서 삭제버튼을 눌려서 가능하게 제작

            //propertyField.style.color = new Color(0.1f, 0.1f, 0.1f);
            //propertyField.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            ////propertyField.style.backgroundColor = Color.white;
            //propertyField.style.flexDirection = FlexDirection.Row;
            //propertyField.style.justifyContent = Justify.Center;
            //propertyField.style.alignSelf = Align.Stretch;


            var customValueTypeEnumField = new EnumField("변수타입", propertyType);
            customValueTypeEnumField.RegisterValueChangedCallback(evt =>
            {   
                propertyType = (PropertyType)evt.newValue;
                saveCheck = true;
            });

            var customValueCreateButton = new Button(clickEvent: () => {

                if (dialogueDBData == null)
                {
                    return;
                }
                AddCustomPropertyListObjectButton(m_TasksContainer, propertyType);
                saveCheck = true;
            });            
            customValueCreateButton.text = "Create -CustomValue-";            

            customValueBox.Add(customValueTypeEnumField);
            customValueBox.Add(customValueCreateButton);
            customValueBox.Add(m_TasksContainer);
            targetElement.Add(customValueBox);
        }

        private void AddCustomPropertyListObjectButton(ScrollView _scrollView, PropertyType _propertyType, string overriddenValueName = "", bool _isOverrideData = false, bool _boolValue = false, float _floatValue = 0f, int _intValue = 0, string _stringValue = "")
        {
            if (targetDialogueData == null)
            {
                //Debug.Log("선택된 대화데이터가 없습니다. 선택해주세요");
                EditorUtility.DisplayDialog("확인창", "선택된 대화데이터가 없습니다. 선택해주세요", "OK");
                return;
            }


            CustomPropertyBool customPropertyBool = null;
            CustomPropertyFloat customPropertyFloat = null;
            CustomPropertyInt customPropertyInt = null;
            CustomPropertyString customPropertyString = null;

            var property = new PropertyField();
            property.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            //변수이름
            var _propertyBaseTextField = new TextField(_propertyType.ToString() + "변수이름");
            InitTextField(_propertyBaseTextField);
            _propertyBaseTextField.maxLength = 20;
            property.Add(_propertyBaseTextField);
            string target = "None";
            string propertyName = "";
            
            switch (_propertyType)
            {
                case PropertyType.Bool:
                    customPropertyBool = new CustomPropertyBool();
                    //customPropertyBool.propertyType = PropertyType.Bool;
                    target = target + "Bool" + _graphView.customPropertyBoolList.Count;
                    propertyName = string.IsNullOrEmpty(overriddenValueName)
               ? target : overriddenValueName;
                    customPropertyBool.propertyName = propertyName;

                    var boolField = new Toggle();
                    boolField.RegisterValueChangedCallback(boolEvt =>
                    {
                        customPropertyBool.propertyBoolValue = boolEvt.newValue;
                        saveCheck = true;
                    });
                    property.Add(boolField);

                    //값 덮어쓰기
                    if (_isOverrideData) 
                    {
                        boolField.value = _boolValue;
                    }

                    //등록
                    _graphView.customPropertyBoolList.Add(customPropertyBool);
                  
                    break;
                case PropertyType.Float:
                    customPropertyFloat = new CustomPropertyFloat();
                    //customPropertyFloat.propertyType = PropertyType.Float;
                    target = target + "Float" + _graphView.customPropertyFloatList.Count;
                    propertyName = string.IsNullOrEmpty(overriddenValueName)
               ? target : overriddenValueName;
                    customPropertyFloat.propertyName = propertyName;

                    var floatField = new FloatField();
                    floatField.RegisterValueChangedCallback(floatEvt =>
                    {
                        customPropertyFloat.propertyFloatValue = floatEvt.newValue;
                        saveCheck = true;
                    });
                    property.Add(floatField);

                    //값 덮어쓰기
                    if (_isOverrideData)
                    {
                        floatField.value = _floatValue;
                    }

                    //등록
                    _graphView.customPropertyFloatList.Add(customPropertyFloat);  
                    break;
                case PropertyType.Int:
                    customPropertyInt = new CustomPropertyInt();
                    //customPropertyInt.propertyType = PropertyType.Int;
                    target = target + "Int" + _graphView.customPropertyIntList.Count;
                    propertyName = string.IsNullOrEmpty(overriddenValueName)
               ? target : overriddenValueName;
                    customPropertyInt.propertyName = propertyName;

                    var intField = new IntegerField();
                    intField.RegisterValueChangedCallback(intEvt =>
                    {
                        customPropertyInt.propertyIntValue = intEvt.newValue;
                        saveCheck = true;
                    });
                    property.Add(intField);

                    //값 덮어쓰기
                    if (_isOverrideData)
                    {
                        intField.value = _intValue;
                    }

                    //등록
                    _graphView.customPropertyIntList.Add(customPropertyInt);                    
                    break;
                case PropertyType.String:
                    customPropertyString = new CustomPropertyString();
                    //customPropertyString.propertyType = PropertyType.String;
                    target = target + "String" + _graphView.customPropertyStringList.Count;
                    propertyName = string.IsNullOrEmpty(overriddenValueName)
               ? target : overriddenValueName;
                    customPropertyString.propertyName = propertyName;

                    var stringField = new TextField();
                    InitTextField(stringField);
                    stringField.RegisterValueChangedCallback(stringEvt =>
                    {
                        customPropertyString.propertyStringValue = stringEvt.newValue;
                        saveCheck = true;
                    });
                    property.Add(stringField);

                    //값 덮어쓰기
                    if (_isOverrideData)
                    {
                        stringField.value = _stringValue;
                    }

                    //등록
                    _graphView.customPropertyStringList.Add(customPropertyString);
                    break;
            }

            _propertyBaseTextField.RegisterValueChangedCallback(evt =>
            {
                propertyName = evt.newValue;
                switch (_propertyType)
                {
                    case PropertyType.Bool:
                        customPropertyBool.propertyName = propertyName;                        
                        break;
                    case PropertyType.Float:
                        customPropertyFloat.propertyName = propertyName;
                        break;
                    case PropertyType.Int:
                        customPropertyInt.propertyName = propertyName;
                        break;
                    case PropertyType.String:
                        customPropertyString.propertyName = propertyName;
                        break;
                }
                saveCheck = true;
            });
            _propertyBaseTextField.value = propertyName;

            //리스트삭제 버튼 추가
            var deleteButton = new Button()
            {
                text = "커스텀변수 삭제",
            };

            //리스트삭제 버튼 이벤트
            deleteButton.clickable.clicked += () => {
                switch (_propertyType)
                {
                    case PropertyType.Bool:
                        _graphView.customPropertyBoolList.Remove(customPropertyBool);
                        break;
                    case PropertyType.Float:
                        _graphView.customPropertyFloatList.Remove(customPropertyFloat);
                        break;
                    case PropertyType.Int:
                        _graphView.customPropertyIntList.Remove(customPropertyInt);
                        break;
                    case PropertyType.String:
                        _graphView.customPropertyStringList.Remove(customPropertyString);
                        break;
                }
                _scrollView.Remove(property);               
            };
            property.Add(deleteButton); 
            _scrollView.Add(property);            
        }

        /// <summary>
        /// 툴바생성
        /// </summary>
        private void GenerateToolbar()
        {
            var toolbar1 = new Toolbar();
            toolbar1.style.flexDirection = FlexDirection.Row;
            toolbar1.style.justifyContent = Justify.Center;
            var toolbar2 = new Toolbar();
            toolbar2.style.flexDirection = FlexDirection.Row;
            toolbar2.style.justifyContent = Justify.Center;

            //그래프 노드 생성버튼관련
            //var propertyField1 = new PropertyField();
            //propertyField1.style.height = 30;
            ////propertyField1.style.color = new Color(0.1f, 0.1f, 0.1f);
            //propertyField1.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            //propertyField1.style.flexDirection = FlexDirection.Row;
            //propertyField1.style.justifyContent = Justify.Center;
            //propertyField1.style.alignSelf = Align.Stretch;


            var dialogueNodeCreateButton = new Button(clickEvent: () => {
                if (dialogueDBData == null)
                {
                    return;
                }
                Vector2 _pos = (Vector2)_graphView.viewTransform.position * -1 + (_graphView.contentRect.size / 2);
                _graphView.CreateNode("DialogueNode", _pos, DialogueNodeType.Dialogue);
                saveCheck = true;
            });
            dialogueNodeCreateButton.style.width = 180;
            dialogueNodeCreateButton.text = "Create -DialogueNode-";

            var setPropertyCreateButton = new Button(clickEvent: () => {
                if (dialogueDBData == null)
                {
                    return;
                }
                Vector2 _pos = (Vector2)_graphView.viewTransform.position * -1 + (_graphView.contentRect.size / 2);
                _graphView.CreateNode("SetProperty", _pos, DialogueNodeType.SetProperty);
                saveCheck = true;
            });
            setPropertyCreateButton.style.width = 180;
            setPropertyCreateButton.text = "Create -SetProperty-";

            var conditionBranchCreateButton = new Button(clickEvent: () => {
                if (dialogueDBData == null)
                {
                    return;
                }
                Vector2 _pos = (Vector2)_graphView.viewTransform.position * -1 + (_graphView.contentRect.size / 2);
                _graphView.CreateNode("ConditionBranchNode", _pos, DialogueNodeType.ConditionBranch);
                saveCheck = true;
            });
            conditionBranchCreateButton.style.width = 180;
            conditionBranchCreateButton.text = "Create -ConditionNode-";

            var selectBranchNodeCreateButton = new Button(clickEvent: () => {
                if (dialogueDBData == null)
                {
                    return;
                }
                Vector2 _pos = (Vector2)_graphView.viewTransform.position * -1 + (_graphView.contentRect.size / 2);
                _graphView.CreateNode("SelectBranchNode", _pos, DialogueNodeType.SelectBranch);
                saveCheck = true;
            });
            selectBranchNodeCreateButton.style.width = 180;
            selectBranchNodeCreateButton.text = "Create -SelectNode-";

            var randomBranchNodeCreateButton = new Button(clickEvent: () => {
                if (dialogueDBData == null)
                {
                    return;
                }
                Vector2 _pos = (Vector2)_graphView.viewTransform.position * -1 + (_graphView.contentRect.size / 2);
                _graphView.CreateNode("RandomBranchNode", _pos, DialogueNodeType.RandomBranch);
                saveCheck = true;
            });
            randomBranchNodeCreateButton.style.width = 180;
            randomBranchNodeCreateButton.text = "Create -RandomNode-";

            //var gameWorldEventNodeCreateButton = new Button(clickEvent: () => {
            //    if (dialogueDBData == null)
            //    {
            //        return;
            //    }
            //    Vector2 _pos = (Vector2)_graphView.viewTransform.position * -1 + (_graphView.contentRect.size / 2);
            //    _graphView.CreateNode("GameWorldEventNode", _pos, DialogueNodeType.GameWorldEvent);
            //    saveCheck = true;
            //});
            //gameWorldEventNodeCreateButton.style.width = 180;
            //gameWorldEventNodeCreateButton.text = "Create -GameWorldEventNode-";

            var selectQuestBranchNodeCreateButton = new Button(clickEvent: () => {
                if (dialogueDBData == null)
                {
                    return;
                }
                Vector2 _pos = (Vector2)_graphView.viewTransform.position * -1 + (_graphView.contentRect.size / 2);
                _graphView.CreateNode("QuestSelectBranchNode", _pos, DialogueNodeType.QuestSelectBranch);
                saveCheck = true;
            });
            selectQuestBranchNodeCreateButton.style.width = 180;
            selectQuestBranchNodeCreateButton.text = "Create -QuestSelectNode-";

            var questEventNodeCreateButton = new Button(clickEvent: () => {
                if (dialogueDBData == null)
                {
                    return;
                }
                Vector2 _pos = (Vector2)_graphView.viewTransform.position * -1 + (_graphView.contentRect.size / 2);
                _graphView.CreateNode("QuestEventNode", _pos, DialogueNodeType.QuestEvent);
                saveCheck = true;
            });
            questEventNodeCreateButton.style.width = 180;
            questEventNodeCreateButton.text = "Create -QuestEventNode-";

            var LinkedDialogueNodeCreateButton = new Button(clickEvent: () => {
                if (dialogueDBData == null)
                {
                    return;
                }
                Vector2 _pos = (Vector2)_graphView.viewTransform.position * -1 + (_graphView.contentRect.size / 2);
                _graphView.CreateNode("LinkedDialogueNode", _pos, DialogueNodeType.LinkedDialogue);
                saveCheck = true;
            });
            LinkedDialogueNodeCreateButton.style.width = 180;
            LinkedDialogueNodeCreateButton.text = "Create -LinkedDialogueNode-";

            //toolbar.Add(dialogueNodeCreateButton);
            //toolbar.Add(setPropertyCreateButton);
            //toolbar.Add(conditionBranchCreateButton);
            //toolbar.Add(selectBranchNodeCreateButton);
            //toolbar.Add(randomBranchNodeCreateButton);
            //toolbar.Add(eventNodeCreateButton);

            toolbar1.Add(dialogueNodeCreateButton);
            toolbar1.Add(setPropertyCreateButton);
            toolbar1.Add(conditionBranchCreateButton);
            toolbar1.Add(selectBranchNodeCreateButton);
            toolbar1.Add(randomBranchNodeCreateButton);

            //toolbar2.Add(gameWorldEventNodeCreateButton);
            toolbar2.Add(selectQuestBranchNodeCreateButton);
            toolbar2.Add(questEventNodeCreateButton);
            toolbar2.Add(LinkedDialogueNodeCreateButton);
           
            rootVisualElement.Add(toolbar1);
            rootVisualElement.Add(toolbar2);
        }

        /// <summary>
        /// 대화DB 저장 로드관련 함수
        /// </summary>
        /// <param name="save"></param>
        private void RequestDBDataOperation(bool save)
        {
            if (save)
            {
                if (fileName == null || fileName == "")
                {
                    EditorUtility.DisplayDialog("확인창", "파일이름을 적어주세요", "OK");
                    //Debug.Log("파일이름을 적어주세요");
                    return;
                }

                saveCheck = false;

                //존재하는지 체크
                //DialogueDBObjectScript _dialogueDBData = (DialogueDBObjectScript)AssetDatabase.LoadAssetAtPath("Assets/Resources/" + fileName + ".asset", typeof(DialogueDBObjectScript));
                DialogueDBObjectScript _dialogueDBData = new DialogueDBObjectScript();
                //if (_dialogueDBData == null)
                //{
                //    _dialogueDBData = new DialogueDBObjectScript();
                //}


                // Make sure the file name is unique, in case an existing Prefab has the same name.
                //localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
                // Create the new Prefab.
                //각각의 트리슬롯들을 프리팹화해주기 //트리슬롯등록
                //SaveAsPrefabAssetAndConnect=>경로에 프리팹을 지정후 씬상에 있는 프리팹과 연동 시켜줌
                //SaveAsPrefabAsset=>경로에 프리팹을 지정후 끝  씬상에 있는 프리팹과는 연동 안시켜줌
                //researchTreeData.researchTreeSystem = PrefabUtility.SaveAsPrefabAssetAndConnect(targetingResearchTreeSystem.gameObject, localPath, InteractionMode.UserAction, out isdone).GetComponent<ResearchTreeSystem>();

                //현재 제작중인 대상대화데이터 저장
                if (targetDialogueData != null)
                {
                    SaveDialogueDataGraph(targetDialogueData);
                }

                //데이터 캐싱
                _dialogueDBData.version = version;
                _dialogueDBData.author = author;
                _dialogueDBData.description = description;
                _dialogueDBData.questDBData = questDBData;
                _dialogueDBData.defaultFontAsset = defaultFontAsset;


                //대화데이터들 캐싱
                //액터들 캐싱
                List<DialogueData> tempDialogueDataList = new List<DialogueData>();                

                for (int i = 0; i < dialogueDataFieldList.Count; i++)
                {
                    DialogueData tempDialogueData = dialogueDataFieldList[i].GetDialogueData();
                    tempDialogueDataList.Add(tempDialogueData);
                }
               
                _dialogueDBData.dialogueDataArray = tempDialogueDataList.ToArray();
                dialogueDBData = _dialogueDBData;
                dialogueDBDataField.value = dialogueDBData;

                lLcroweUtilEditor.CreateResourceDataObject(_dialogueDBData, fileName, "");

                //AssetDatabase.CreateAsset(_dialogueDBData, "Assets/Resources/" + fileName + ".asset");
            }
            else
            {
                if (dialogueDBData == null)
                {
                    EditorUtility.DisplayDialog("확인창", "로드할 대상이 없습니다", "OK");
                    //Debug.Log("로드할 대상이 없습니다");
                    return;
                }

                //로드
                //대화데이터목록 초기화               
                dialogueDataFieldList.Clear();
                dialogueDataScrollView.Clear();

                //그래프초기화
                ClearGraph();

                //대화데이터목록들을 보여주기
                for (int i = 0; i < dialogueDBData.dialogueDataArray.Length; i++)
                {
                    DialogueDataField dialogueDataField = new DialogueDataField(dialogueDBData.dialogueDataArray[i], dialogueDataFieldList, dialogueDataScrollView, SelectDialogueDataField, deleteDialogueDataField);
                }

                fileNameField.value = dialogueDBData.name;
                versionField.value = dialogueDBData.version;
                authorField.value = dialogueDBData.author;
                descriptionField.value = dialogueDBData.description;
                questDBDataField.value = dialogueDBData.questDBData;
                defaultFontAssetObjectField.value = dialogueDBData.defaultFontAsset;
                saveCheck = false;
            }
        }

        //대화데이터 필드를 선택할때마다 작동되는 함수
        private void SelectDialogueDataField(DialogueData _dialogueData)
        {
            //대화데이터를 버튼으로 처리해서 버튼을 누를때마다 저장, 로드 처리하게
            //선택했을시 기존꺼는 저장

            //기존꺼 저장
            if (targetDialogueData != null)
            {
                //Debug.Log("대화데이터 저장됨 " + targetDialogueData.dialogueIDKey);
                SaveDialogueDataGraph(targetDialogueData);
            }

            //선택한 대상은 로드            
            targetDialogueData = _dialogueData;
            selectNameLabelField.text = "-= 선택된 대화데이터 => " + targetDialogueData.dialogueIDKey + " =-";
            LoadDialogueData(targetDialogueData);
            
            //Debug.Log("대화데이터 선택됨 " + _dialogueData.dialogueIDKey);
        }

        //대화데이터필드를 삭제할때마다 작동되는 함수
        private void deleteDialogueDataField()
        {
            targetDialogueData = null;
        }

        /// <summary>
        /// 대화데이터들에서 특정아이디를 찾는 함수
        /// </summary>
        /// <param name="dataID"></param>
        private void FindDialogueData(string dataID)
        {
            if (dialogueDBData == null)
            {
                return;
            }
            for (int i = 0; i < dialogueDataFieldList.Count; i++)
            {
                if (dialogueDataFieldList[i].GetDialogueData().dialogueIDKey == dataID)
                {
                    SelectDialogueDataField(dialogueDataFieldList[i].GetDialogueData());
                    break;
                }
            }
        }

        public static void SetDialogueDBData(DialogueDBObjectScript _dialogueData)
        {
            dialogueDBData = _dialogueData;
            inspectorOpen = true;
        }

        public static DialogueDBObjectScript GetDialogueDBData()
        {
            return dialogueDBData;
        }

        /// <summary>
        ///대화데이터 노드그래프 저장
        /// </summary>        
        public void SaveDialogueDataGraph(DialogueData _nodeContainerData)
        {

            //노드가 한개라도 연결이 안되있으면 작동안함
            if (!Edges.Any())
            {
                //Debug.Log("노드가 연결이 안되있음");
                return;
            }

            //변수들 저장
            _nodeContainerData.customPropertyBoolArray = _graphView.customPropertyBoolList.ToArray();
            _nodeContainerData.customPropertyFloatArray = _graphView.customPropertyFloatList.ToArray();
            _nodeContainerData.customPropertyIntArray = _graphView.customPropertyIntList.ToArray();
            _nodeContainerData.customPropertyStringArray = _graphView.customPropertyStringList.ToArray();
            
            //각각의 데이터 저장
            List<DialogueContentData> dialogueContentDataList = new List<DialogueContentData>(); ;//대화내용들
            List<SetPropertyData> SetPropertyList = new List<SetPropertyData>();//셋프로퍼티
            List<ConditionBranchNodeData> conditionBranchNodeList = new List<ConditionBranchNodeData>();//조건
            List<SelectBrandNodeData> selectBrandNodeList = new List<SelectBrandNodeData>();
            List<SelectQuestBrandNodeData> selectQuestBrandNodeList = new List<SelectQuestBrandNodeData>();
            List<GameWorldEventData> gameWorldEventNodeList = new List<GameWorldEventData>();
            List<QuestEventData> questEventNodeList = new List<QuestEventData>();

            //노드 저장
            List<CustomNodeData> nodeDataList = new List<CustomNodeData>();

            //노드 & 데이터 저장로직
            foreach (var node in Nodes.Where(node => node.nodeType != DialogueNodeType.StartPoint))
            {
                //노드의 기본적인거 저장
                CustomNodeData customNodeData = new CustomNodeData()
                {
                    nodeType = node.nodeType,
                    nodeGUID = node.GUID,
                    position = node.GetPosition().position,
                    nextNodeGUIDList = new List<string>()
                };

                //노드타입에 따른 내용 저장
                switch (node.nodeType)
                {
                    case DialogueNodeType.Dialogue:
                        //캐싱
                        DialogueContentData dialogueContentData = new DialogueContentData();
                        DialogueNode dialogueNode = (DialogueNode)node;
                        dialogueContentData.nodeGUID = dialogueNode.GUID;
                        dialogueContentData.targetActorInfoData = dialogueNode.targetActorInfoData;
                        dialogueContentData.actorShowBegin = dialogueNode.actorShowBegin;
                        dialogueContentData.actorShowPos = dialogueNode.actorShowPos;
                        dialogueContentData.actorShowEnd = dialogueNode.actorShowEnd;
                        dialogueContentData.localizeID = dialogueNode.localizeID;
                        dialogueContentData.dialogueContent = dialogueNode.dialogueContent;
                        dialogueContentData.animamtionPortraitMessageType = dialogueNode.animamtionPortraitMessageType;
                        dialogueContentData.dialogueChatType = dialogueNode.dialogueChatType;
                        dialogueContentData.overrideFontAsset = dialogueNode.overideFontAsset;
                        dialogueContentData.focusObject = dialogueNode.focusObject;

                        //등록
                        dialogueContentDataList.Add(dialogueContentData);
                        break;
                    case DialogueNodeType.SetProperty:
                        //캐싱
                        SetPropertyData setPropertyData = new SetPropertyData();
                        SetCustomPropertyNode setCustomPropertyNode = (SetCustomPropertyNode)node;
                        setPropertyData.nodeGUID = setCustomPropertyNode.GUID;
                        setPropertyData.propertyType = setCustomPropertyNode.propertyType;
                        setPropertyData.propertyName = setCustomPropertyNode.propertyName;

                        switch (setPropertyData.propertyType)
                        {
                            case PropertyType.Bool:
                                setPropertyData.boolValue = setCustomPropertyNode.boolValue;
                                break;
                            case PropertyType.Float:
                                setPropertyData.floatVaule = setCustomPropertyNode.floatVaule;
                                setPropertyData.propertySetType = setCustomPropertyNode.propertySetType;
                                break;
                            case PropertyType.Int:
                                setPropertyData.intValue = setCustomPropertyNode.intValue;
                                setPropertyData.propertySetType = setCustomPropertyNode.propertySetType;
                                break;
                            case PropertyType.String:
                                setPropertyData.stringValue = setCustomPropertyNode.stringValue;
                                break;
                        }

                        //등록
                        SetPropertyList.Add(setPropertyData);
                        break;
                    case DialogueNodeType.ConditionBranch:
                        //캐싱
                        ConditionBranchNodeData conditionBranchNodeData = new ConditionBranchNodeData();
                        ConditionBranchNode conditionBranchNode = (ConditionBranchNode)node;
                        conditionBranchNodeData.nodeGUID = conditionBranchNode.GUID;
                        conditionBranchNodeData.targetPropertyType = conditionBranchNode.targetPropertyType;
                        conditionBranchNodeData.targetPropertyName = conditionBranchNode.targetPropertyName;
                        conditionBranchNodeData.operatorType = conditionBranchNode.operatorType;

                        switch (conditionBranchNodeData.targetPropertyType)
                        {
                            case PropertyType.Bool:
                                conditionBranchNodeData.boolValue = conditionBranchNode.boolValue;
                                break;
                            case PropertyType.Float:
                                conditionBranchNodeData.floatVaule = conditionBranchNode.floatVaule;
                                break;
                            case PropertyType.Int:
                                conditionBranchNodeData.intValue = conditionBranchNode.intValue;
                                break;
                            case PropertyType.String:
                                conditionBranchNodeData.stringValue = conditionBranchNode.stringValue;
                                break;
                        }

                        //등록
                        conditionBranchNodeList.Add(conditionBranchNodeData);
                        break;
                    case DialogueNodeType.SelectBranch:
                        //캐싱
                        SelectBrandNodeData selectBrandNodeData = new SelectBrandNodeData();
                        SelectBranchNode selectBranchNode = (SelectBranchNode)node;
                        selectBrandNodeData.nodeGUID = selectBranchNode.GUID;
                        List<string> tempContent = new List<string>();

                        for (int i = 0; i < selectBranchNode.selectContentList.Count; i++)
                        {
                            tempContent.Add(selectBranchNode.selectContentList[i].value);
                        }
                        selectBrandNodeData.selectContentArray = tempContent.ToArray();
                        tempContent.Clear();

                        for (int i = 0; i < selectBranchNode.localizeList.Count; i++)
                        {
                            tempContent.Add(selectBranchNode.localizeList[i].value);
                        }
                        selectBrandNodeData.localizeArray = tempContent.ToArray();
                        tempContent.Clear();

                        //등록
                        selectBrandNodeList.Add(selectBrandNodeData);
                        break;
                    case DialogueNodeType.QuestSelectBranch:
                        //캐싱
                        SelectQuestBrandNodeData selectQuestBrandNodeData = new SelectQuestBrandNodeData();
                        QuestSelectBranchNode selectQuestBranchNode = (QuestSelectBranchNode)node;
                        selectQuestBrandNodeData.nodeGUID = selectQuestBranchNode.GUID;
                        List<string> tempQuestContent = new List<string>();

                        for (int i = 0; i < selectQuestBranchNode.questBookIDFieldList.Count; i++)
                        {
                            tempQuestContent.Add(selectQuestBranchNode.questBookIDFieldList[i].value);
                        }
                        selectQuestBrandNodeData.questBookIDKeyArray = tempQuestContent.ToArray();
                        tempQuestContent.Clear();

                        //등록
                        selectQuestBrandNodeList.Add(selectQuestBrandNodeData);
                        break;
                    case DialogueNodeType.RandomBranch:
                        //존재하지않음
                        //노드타입으로 체크함
                        break;
                    //case DialogueNodeType.GameWorldEvent:
                    //    //캐싱
                    //    GameWorldEventData gameWorldEventData = new GameWorldEventData();
                    //    GameWorldEventNode gameWorldEventNode = (GameWorldEventNode)node;
                    //    gameWorldEventData.nodeGUID = gameWorldEventNode.GUID;
                    //    gameWorldEventData.gameWorldEventName = gameWorldEventNode.gameWorldEventNameField.value;

                    //    //등록
                    //    gameWorldEventNodeList.Add(gameWorldEventData);
                    //    break;
                    case DialogueNodeType.QuestEvent:
                        //캐싱
                        QuestEventData questEventData = new QuestEventData();
                        QuestEventNode questEventNode = (QuestEventNode)node;
                        questEventData.nodeGUID = questEventNode.GUID;
                        questEventData.questBookID = questEventNode.questBookIDField.value;
                        //등록
                        questEventNodeList.Add(questEventData);
                        break;
                }

                nodeDataList.Add(customNodeData);
            }

            //최종집어넣기
            _nodeContainerData.dialogueContentDataArray = dialogueContentDataList.ToArray();
            _nodeContainerData.setPropertyDataArray = SetPropertyList.ToArray();
            _nodeContainerData.conditionBranchNodeDataArray = conditionBranchNodeList.ToArray();
            _nodeContainerData.selectBrandNodeDataArray = selectBrandNodeList.ToArray();
            _nodeContainerData.selectQuestBrandNodeDataArray = selectQuestBrandNodeList.ToArray();
            //_nodeContainerData.gameWorldEventDataArray = gameWorldEventNodeList.ToArray();
            _nodeContainerData.questEventDataArray = questEventNodeList.ToArray();
            _nodeContainerData.nodeDataArray = nodeDataList.ToArray();

            //시작노드저장
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].nodeType == DialogueNodeType.StartPoint)
                {
                    //현재노드가 시작노드이면 다음은 시작노드와이어진 첫번째노드
                    //전에 저장한게 있으면 해당 노드를 리셋시켜줘야 제대로 찾아줄거 같음
                    Port port = (Port)Nodes[i].outputContainer[0];
                    Edge edge = port.connections.Cast<Edge>().ToArray()[0];//엣찌가 여러개 나옴//문제원인//해결법=>진입점 노드까지 다 지워버리고 다시만드니 한개만 나옴
                    DialogueUINode tempNode = (DialogueUINode)edge.input.node;

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
                var outputNode = (connectedSockets[i].output.node as DialogueUINode);
                var inputNode = (connectedSockets[i].input.node as DialogueUINode);
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
        /// 대화데이터 노드그래프 로드 함수
        /// </summary>
        /// <param name="_dialogueData"></param>
        public void LoadDialogueData(DialogueData _dialogueData)
        {               
            if (_dialogueData == null)
            {
                EditorUtility.DisplayDialog("대화데이터를 못찾았습니다", "존재하지않습니다. 코드를 확인해주세요!", "OK");
                return;
            }            

            ClearGraph();//그래프의 노드정리
            GenerateDialogueNodes(_dialogueData);//노드 재생
            ConnectDialogueNodes(_dialogueData);//노드끼리 링크연결
        }

        /// <summary>
        /// Set Entry point GUID then Get All Nodes, remove all and their edges. Leave only the entrypoint node. (Remove its edge too)
        /// </summary>
        private void ClearGraph()
        {
            _graphView.scrollView.Clear();
            _graphView.customPropertyBoolList.Clear();
            _graphView.customPropertyFloatList.Clear();
            _graphView.customPropertyIntList.Clear();
            _graphView.customPropertyStringList.Clear();

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
        private void GenerateDialogueNodes(DialogueData _nodeContainer)
        {
            //전역변수 생성
            for (int i = 0; i < _nodeContainer.customPropertyBoolArray.Length; i++)
            {
                AddCustomPropertyListObjectButton(_graphView.scrollView, PropertyType.Bool, _nodeContainer.customPropertyBoolArray[i].propertyName, true, _nodeContainer.customPropertyBoolArray[i].propertyBoolValue);
            }
            for (int i = 0; i < _nodeContainer.customPropertyFloatArray.Length; i++)
            {
                AddCustomPropertyListObjectButton(_graphView.scrollView, PropertyType.Float, _nodeContainer.customPropertyFloatArray[i].propertyName, true, false, _nodeContainer.customPropertyFloatArray[i].propertyFloatValue);
            }
            for (int i = 0; i < _nodeContainer.customPropertyIntArray.Length; i++)
            {
                AddCustomPropertyListObjectButton(_graphView.scrollView, PropertyType.Int, _nodeContainer.customPropertyIntArray[i].propertyName, true, false, 0, _nodeContainer.customPropertyIntArray[i].propertyIntValue);
            }
            for (int i = 0; i < _nodeContainer.customPropertyStringArray.Length; i++)
            {
                AddCustomPropertyListObjectButton(_graphView.scrollView, PropertyType.String, _nodeContainer.customPropertyStringArray[i].propertyName, true, false, 0, 0, _nodeContainer.customPropertyStringArray[i].propertyStringValue);
            }

            //진입점 생성
            _graphView.AddElement(_graphView.GenerateEntryPointNode());

            //노드데이터 입력
            List<string> tempList = new List<string>();
            foreach (var perNodeData in _nodeContainer.nodeDataArray)
            {
                //노드타입 체크후 집어넣기
                //노드생성후 포트 생성후 데이터 삽입해야함
                DialogueUINode tempNode = null;
                switch (perNodeData.nodeType)
                {
                    case DialogueNodeType.Dialogue:
                        tempNode = _graphView.CreateNode("DialogueNode", perNodeData.position, DialogueNodeType.Dialogue);
                        DialogueNode dialogueNode = (DialogueNode)tempNode;
                        DialogueContentData targetDialogueContentData = null;

                        //데이터에서 같은 아이디인지 체크
                        for (int i = 0; i < _nodeContainer.dialogueContentDataArray.Length; i++)
                        {
                            if (perNodeData.nodeGUID == _nodeContainer.dialogueContentDataArray[i].nodeGUID)
                            {
                                targetDialogueContentData = _nodeContainer.dialogueContentDataArray[i];
                                break;
                            }
                        }

                        //데이터캐생

                        //변수
                        dialogueNode.targetActorInfoData = targetDialogueContentData.targetActorInfoData;
                        dialogueNode.actorShowBegin = targetDialogueContentData.actorShowBegin;
                        dialogueNode.actorShowPos = targetDialogueContentData.actorShowPos;
                        dialogueNode.actorShowEnd = targetDialogueContentData.actorShowEnd;

                        dialogueNode.localizeID = targetDialogueContentData.localizeID;
                        dialogueNode.dialogueContent = targetDialogueContentData.dialogueContent;
                        dialogueNode.animamtionPortraitMessageType = targetDialogueContentData.animamtionPortraitMessageType;
                        
                        dialogueNode.dialogueChatType = targetDialogueContentData.dialogueChatType;
                        dialogueNode.overideFontAsset = targetDialogueContentData.overrideFontAsset;
                        dialogueNode.focusObject = targetDialogueContentData.focusObject;

                        //필드
                        dialogueNode.targetActorInfoDataObjectField.value = targetDialogueContentData.targetActorInfoData;
                        dialogueNode.actorShowBeginTypeEnumField.value = targetDialogueContentData.actorShowBegin;
                        dialogueNode.actorShowPosTypeEnumField.value = targetDialogueContentData.actorShowPos;
                        dialogueNode.actorShowEndTypeEnumField.value = targetDialogueContentData.actorShowEnd;

                        dialogueNode.localizeIDTextField.value = targetDialogueContentData.localizeID;
                        dialogueNode.dialogueContentTextField.value = targetDialogueContentData.dialogueContent;
                        dialogueNode.animationPortraitMessageTypeEnumField.value = targetDialogueContentData.animamtionPortraitMessageType;                        

                        dialogueNode.dialogueChatTypeEnumField.value = targetDialogueContentData.dialogueChatType;
                        dialogueNode.overideFontAssetObjectField.value = targetDialogueContentData.overrideFontAsset;
                        dialogueNode.focusObjectObjectField.value = targetDialogueContentData.focusObject;

                        
                        break;
                    case DialogueNodeType.SetProperty:
                        tempNode = _graphView.CreateNode("SetProperty", perNodeData.position, DialogueNodeType.SetProperty);
                        SetCustomPropertyNode setCustomPropertyNode = (SetCustomPropertyNode)tempNode;

                        SetPropertyData targetSetPropertyData = null;

                        //데이터에서 같은 아이디인지 체크
                        for (int i = 0; i < _nodeContainer.setPropertyDataArray.Length; i++)
                        {
                            if (perNodeData.nodeGUID == _nodeContainer.setPropertyDataArray[i].nodeGUID)
                            {
                                targetSetPropertyData = _nodeContainer.setPropertyDataArray[i];
                                break;
                            }
                        }

                        //데이터캐싱
                        //변수
                        setCustomPropertyNode.propertyType = targetSetPropertyData.propertyType;
                        setCustomPropertyNode.propertyName = targetSetPropertyData.propertyName;
                        setCustomPropertyNode.propertySetType = targetSetPropertyData.propertySetType;
                        setCustomPropertyNode.boolValue = targetSetPropertyData.boolValue;
                        setCustomPropertyNode.floatVaule = targetSetPropertyData.floatVaule;
                        setCustomPropertyNode.intValue = targetSetPropertyData.intValue;
                        setCustomPropertyNode.stringValue = targetSetPropertyData.stringValue;
                        //버튼생성
                        _graphView.SetPropertyButton(setCustomPropertyNode);

                        tempList.Clear();
                        _graphView.GetTargetValueList(tempList, setCustomPropertyNode.propertyType);
                       
                        if (tempList.Count > 0)
                        {
                            //필드
                            setCustomPropertyNode.customPropertyTypeEnumField.value = targetSetPropertyData.propertyType;
                            setCustomPropertyNode.testField.value = targetSetPropertyData.propertyName;

                            switch (setCustomPropertyNode.propertyType)
                            {
                                case PropertyType.Bool:
                                    setCustomPropertyNode.boolField.value = targetSetPropertyData.boolValue;
                                    break;
                                case PropertyType.Float:
                                    setCustomPropertyNode.customPropertySetTypeEnumField.value = targetSetPropertyData.propertySetType;
                                    setCustomPropertyNode.floatField.value = targetSetPropertyData.floatVaule;
                                    break;
                                case PropertyType.Int:
                                    setCustomPropertyNode.customPropertySetTypeEnumField1.value = targetSetPropertyData.propertySetType;
                                    setCustomPropertyNode.intField.value = targetSetPropertyData.intValue;
                                    break;
                                case PropertyType.String:
                                    setCustomPropertyNode.stringField.value = targetSetPropertyData.stringValue;
                                    break;
                            }
                        }
                        break;
                    case DialogueNodeType.ConditionBranch:
                        tempNode = _graphView.CreateNode("ConditionBranchNode", perNodeData.position, DialogueNodeType.ConditionBranch);
                        ConditionBranchNode conditionBranchNode = (ConditionBranchNode)tempNode;
                        ConditionBranchNodeData targetConditionBranchNodeData = null;

                        //데이터에서 같은 아이디인지 체크
                        for (int i = 0; i < _nodeContainer.conditionBranchNodeDataArray.Length; i++)
                        {
                            if (perNodeData.nodeGUID == _nodeContainer.conditionBranchNodeDataArray[i].nodeGUID)
                            {
                                targetConditionBranchNodeData = _nodeContainer.conditionBranchNodeDataArray[i];
                                break;
                            }
                        }

                        //데이터캐생
                        //변수
                        conditionBranchNode.targetPropertyType = targetConditionBranchNodeData.targetPropertyType;
                        conditionBranchNode.targetPropertyName = targetConditionBranchNodeData.targetPropertyName;
                        conditionBranchNode.operatorType = targetConditionBranchNodeData.operatorType;
                        conditionBranchNode.boolValue = targetConditionBranchNodeData.boolValue;
                        conditionBranchNode.floatVaule = targetConditionBranchNodeData.floatVaule;
                        conditionBranchNode.intValue = targetConditionBranchNodeData.intValue;
                        conditionBranchNode.stringValue = targetConditionBranchNodeData.stringValue;

                        //버튼생성
                        _graphView.ConditionBranchButton(conditionBranchNode);

                        tempList.Clear();
                        _graphView.GetTargetValueList(tempList, conditionBranchNode.targetPropertyType);

                        if (tempList.Count > 0)
                        {
                            //필드
                            conditionBranchNode.customPropertyTypeEnumField.value = targetConditionBranchNodeData.targetPropertyType;
                            conditionBranchNode.testField.value = targetConditionBranchNodeData.targetPropertyName;
                            conditionBranchNode.customValueTypeEnumField.value = targetConditionBranchNodeData.operatorType;

                            switch (conditionBranchNode.targetPropertyType)
                            {
                                case PropertyType.Bool:
                                    conditionBranchNode.boolField.value = targetConditionBranchNodeData.boolValue;
                                    break;
                                case PropertyType.Float:
                                    conditionBranchNode.floatField.value = targetConditionBranchNodeData.floatVaule;
                                    break;
                                case PropertyType.Int:
                                    conditionBranchNode.intField.value = targetConditionBranchNodeData.intValue;
                                    break;
                                case PropertyType.String:
                                    conditionBranchNode.stringField.value = targetConditionBranchNodeData.stringValue;
                                    break;
                            }
                        }
                        break;
                    case DialogueNodeType.SelectBranch:
                        tempNode = _graphView.CreateNode("SelectBranchNode", perNodeData.position, DialogueNodeType.SelectBranch);
                        SelectBranchNode selectBranchNode = (SelectBranchNode)tempNode;
                        //포트생성
                        for (int i = 0; i < _nodeContainer.nodeDataArray.Length; i++)
                        {
                            if (perNodeData.nodeGUID == _nodeContainer.nodeDataArray[i].nodeGUID)
                            {
                                for (int j = 0; j < _nodeContainer.nodeDataArray[i].nextNodeGUIDList.Count; j++)
                                {
                                    _graphView.AddSelectPoint((SelectBranchNode)tempNode, "");
                                }
                                break;
                            }
                        }
                        SelectBrandNodeData targetSelectBrandNodeData = null;

                        //데이터에서 같은 아이디인지 체크
                        for (int i = 0; i < _nodeContainer.selectBrandNodeDataArray.Length; i++)
                        {
                            if (perNodeData.nodeGUID == _nodeContainer.selectBrandNodeDataArray[i].nodeGUID)
                            {
                                targetSelectBrandNodeData = _nodeContainer.selectBrandNodeDataArray[i];
                                break;
                            }
                        }

                        if (targetSelectBrandNodeData == null)
                        {
                            return;
                        }

                        //데이터캐생
                        //필드=>초반부터 필드로 제작하여변수가 없음 나중에 다른노드의 변수없앨수 있을때 없앨것
                        for (int i = 0; i < selectBranchNode.selectContentList.Count; i++)
                        {
                            selectBranchNode.selectContentList[i].value = targetSelectBrandNodeData.selectContentArray[i];
                            selectBranchNode.localizeList[i].value = targetSelectBrandNodeData.localizeArray[i];
                        }

                        //새로고침
                        tempNode.RefreshPorts();
                        tempNode.RefreshExpandedState();
                        break;
                    case DialogueNodeType.QuestSelectBranch:
                        tempNode = _graphView.CreateNode("QuestSelectBranchNode", perNodeData.position, DialogueNodeType.QuestSelectBranch);
                        QuestSelectBranchNode selectQuestBranchNode = (QuestSelectBranchNode)tempNode;
                        //포트생성
                        for (int i = 0; i < _nodeContainer.nodeDataArray.Length; i++)
                        {
                            if (perNodeData.nodeGUID == _nodeContainer.nodeDataArray[i].nodeGUID)
                            {
                                for (int j = 0; j < _nodeContainer.nodeDataArray[i].nextNodeGUIDList.Count; j++)
                                {
                                    _graphView.AddQuestSelectPoint((QuestSelectBranchNode)tempNode, "");
                                }
                                break;
                            }
                        }
                        SelectQuestBrandNodeData targetSelectQuestBrandNodeData = null;

                        //데이터에서 같은 아이디인지 체크
                        for (int i = 0; i < _nodeContainer.selectBrandNodeDataArray.Length; i++)
                        {
                            if (perNodeData.nodeGUID == _nodeContainer.selectQuestBrandNodeDataArray[i].nodeGUID)
                            {
                                targetSelectQuestBrandNodeData = _nodeContainer.selectQuestBrandNodeDataArray[i];
                                break;
                            }
                        }

                        //데이터캐생
                        //필드=>초반부터 필드로 제작하여변수가 없음 나중에 다른노드의 변수없앨수 있을때 없앨것
                        for (int i = 0; i < selectQuestBranchNode.questBookIDFieldList.Count; i++)
                        {
                            try
                            {
                                selectQuestBranchNode.questBookIDFieldList[i].value = targetSelectQuestBrandNodeData.questBookIDKeyArray[i];
                            }
                            catch (System.Exception e)
                            {   
                                EditorUtility.DisplayDialog("집어넣을 값에 문제가 있으니 초기화됨", e.Message, "OK");
                                selectQuestBranchNode.questBookIDFieldList[i].index = 0;
                            }
                        }

                        //새로고침
                        tempNode.RefreshPorts();
                        tempNode.RefreshExpandedState();
                        break;
                    case DialogueNodeType.RandomBranch:
                        tempNode = _graphView.CreateNode("RandomBranchNode", perNodeData.position, DialogueNodeType.RandomBranch);

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
                    //case DialogueNodeType.GameWorldEvent:
                    //    tempNode = _graphView.CreateNode("GameWorldEventNode", perNodeData.position, DialogueNodeType.GameWorldEvent);
                    //    GameWorldEventNode gameWorldEventNode = (GameWorldEventNode)tempNode;
                    //    GameWorldEventData targetgameWorldEventData = null;


                    //    //데이터에서 같은 아이디인지 체크
                    //    for (int i = 0; i < _nodeContainer.gameWorldEventDataArray.Length; i++)
                    //    {
                    //        if (perNodeData.nodeGUID == _nodeContainer.gameWorldEventDataArray[i].nodeGUID)
                    //        {
                    //            targetgameWorldEventData = _nodeContainer.gameWorldEventDataArray[i];
                    //            break;
                    //        }
                    //    }

                    //    //데이터캐생
                    //    try
                    //    {   
                    //        //필드
                    //        gameWorldEventNode.gameWorldEventNameField.value = targetgameWorldEventData.gameWorldEventName;
                    //    }
                    //    catch (System.Exception e)
                    //    {
                    //        EditorUtility.DisplayDialog("집어넣을 값에 문제가 있으니 초기화됨", e.Message, "OK");
                    //        gameWorldEventNode.gameWorldEventNameField.index = 0;
                    //    }
                        
                    //    break;
                    case DialogueNodeType.QuestEvent:
                        tempNode = _graphView.CreateNode("QuestEventNode", perNodeData.position, DialogueNodeType.QuestEvent);
                        QuestEventNode eventNode = (QuestEventNode)tempNode;
                        QuestEventData targetQuestEventData = null;


                        //데이터에서 같은 아이디인지 체크
                        for (int i = 0; i < _nodeContainer.questEventDataArray.Length; i++)
                        {
                            if (perNodeData.nodeGUID == _nodeContainer.questEventDataArray[i].nodeGUID)
                            {
                                targetQuestEventData = _nodeContainer.questEventDataArray[i];
                                break;
                            }
                        }
                      
                        //데이터캐생
                        try
                        {   
                            //필드
                            eventNode.questBookIDField.value = targetQuestEventData.questBookID;
                        }
                        catch (System.Exception e)
                        {
                            EditorUtility.DisplayDialog("집어넣을 값에 문제가 있으니 초기화됨", e.Message, "OK");
                            eventNode.questBookIDField.index = 0;                         
                        }
                        break;
                    case DialogueNodeType.LinkedDialogue:
                        tempNode = _graphView.CreateNode("QuestEventNode", perNodeData.position, DialogueNodeType.LinkedDialogue);
                        LinkedDialogueNode linkedDialogueNode = (LinkedDialogueNode)tempNode;
                        LinkedDialogueData targetLinkedDialogueData = null;


                        //데이터에서 같은 아이디인지 체크
                        for (int i = 0; i < _nodeContainer.linkedDialogueDataArray.Length; i++)
                        {
                            if (perNodeData.nodeGUID == _nodeContainer.linkedDialogueDataArray[i].nodeGUID)
                            {
                                targetLinkedDialogueData = _nodeContainer.linkedDialogueDataArray[i];
                                break;
                            }
                        }

                        //데이터캐생
                        try
                        {
                            //필드
                            linkedDialogueNode.dialogueIDField.value = targetLinkedDialogueData.dialogueID;
                        }
                        catch (System.Exception e)
                        {
                            EditorUtility.DisplayDialog("집어넣을 값에 문제가 있으니 초기화됨", e.Message, "OK");
                            linkedDialogueNode.dialogueIDField.index = 0;
                        }
                        break;
                } 
                tempNode.GUID = perNodeData.nodeGUID;
                _graphView.AddElement(tempNode);
            }
        }

        //노드링크 연결
        private void ConnectDialogueNodes(DialogueData _nodeContainer)
        {
            //var k = i; //Prevent access to modified closure
            //var connections = _nodeContainer.nodeLinks.Where(x => x.baseNodeGUID == Nodes[k].GUID).ToList();

            //제작된 노드수만큼 반복
            for (var i = 0; i < Nodes.Count; i++)
            {
                var targetOutputNode = Nodes[i];

                if (targetOutputNode.nodeType == DialogueNodeType.StartPoint)
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
                            EditorUtility.DisplayDialog("노드 컨넥트 확인창", targetOutputNode.nodeName + "만 존재함.", "OK");
                            //Debug.Log(targetOutputNode.nodeName + "만 존재함.");
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("노드 컨넥트 확인창", targetOutputNode.nodeName + "연결안됨\n" + targetOutputNode.GUID, "OK");
                            //Debug.Log(targetOutputNode.nodeName + "연결안됨\n" + targetOutputNode.GUID);
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
            //FocusInEvent //요소가 포커스를 얻으려고 할 때 전송되는 이벤트입니다.
            _textField.RegisterCallback<FocusInEvent>(evt =>
            {
                Input.imeCompositionMode = IMECompositionMode.On;
                //UnityEngine.inputSystem  Keyboard.current.SetImeEnabled(true);
            });
            //FocusOutEvent //요소가 포커스를 잃으려고 할 때 전송되는 이벤트입니다.
            _textField.RegisterCallback<FocusOutEvent>(evt => { Input.imeCompositionMode = IMECompositionMode.Auto; });

        }
    }


    class DialogueDataField
    {
        private DialogueData dialogueData;//타겟이 된 대화데이터
        private PropertyField propertyField;//바닥
        private Button selectButton;//로드와 저장이 진행됨            
        private Button deleteButton;//삭제버튼    
        private TextField dialogueIDKeyField;
        private Toggle isStopTimeField;
        private Toggle isSkippableField;
        private VisualElement parentElement;

        public DialogueDataField(DialogueData _dialogueData, List<DialogueDataField> _dialogueDataFieldList, VisualElement _parentElement, UnityAction<DialogueData> selectAction, UnityAction deleteAction)
        {
            dialogueData = _dialogueData;

            if (!_dialogueDataFieldList.Contains(this))
            {
                _dialogueDataFieldList.Add(this);
            }

            parentElement = _parentElement;
            propertyField = new PropertyField();

            selectButton = new Button();
            selectButton.Add(new Button(() => selectAction(dialogueData)) { text = "선택 로드" });

            deleteButton = new Button();
            deleteButton.Add(new Button(() => { RemoveDialogueDataField(_dialogueDataFieldList); deleteAction(); }) { text = "대화데이터삭제" });

            dialogueIDKeyField = new TextField("대화데이터ID");
            DialogueDBWindowEditor.InitTextField(dialogueIDKeyField);
            dialogueIDKeyField.RegisterValueChangedCallback(dialogueIDKeyEvt =>
            {
                dialogueData.dialogueIDKey = dialogueIDKeyEvt.newValue;
                DialogueDBWindowEditor.saveCheck = true;
            });
            dialogueIDKeyField.value = dialogueData.dialogueIDKey;

            isStopTimeField = new Toggle("게임월드시간 정지여부");
            isStopTimeField.RegisterValueChangedCallback(isStopTimeEvt =>
            {
                dialogueData.isStopTime = isStopTimeEvt.newValue;
                DialogueDBWindowEditor.saveCheck = true;
            });
            isStopTimeField.value = dialogueData.isStopTime;

            isSkippableField = new Toggle("대화 스킵가능여부");
            isSkippableField.RegisterValueChangedCallback(isSkippableEvt =>
            {
                dialogueData.isSkippable = isSkippableEvt.newValue;
                DialogueDBWindowEditor.saveCheck = true;
            });
            isSkippableField.value = dialogueData.isSkippable;

            propertyField.Add(selectButton);
            propertyField.Add(deleteButton);
            propertyField.Add(dialogueIDKeyField);
            propertyField.Add(isStopTimeField);
            propertyField.Add(isSkippableField);
            _parentElement.Add(propertyField);
        }

        public void RemoveDialogueDataField(List<DialogueDataField> _dialogueDataFieldList)
        {
            _dialogueDataFieldList.Remove(this);
            propertyField.Remove(selectButton);
            propertyField.Remove(dialogueIDKeyField);
            propertyField.Remove(isStopTimeField);
            propertyField.Remove(isSkippableField);
            parentElement.Remove(propertyField);
            dialogueData = null;
        }

        public DialogueData GetDialogueData()
        {
            return dialogueData;
        }
    }
}
#endif
#endif
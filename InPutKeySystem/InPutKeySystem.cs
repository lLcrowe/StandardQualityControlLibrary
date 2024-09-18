using System.Collections.Generic;
using UnityEngine;
using lLCroweTool.Singleton;

namespace lLCroweTool
{
    /// <summary>
    /// 인풋키 타입
    /// </summary>
    public enum InputKeyType
    {
        GetKey,
        GetKeyDown,
        GetKeyUp,
        GetKeyDoubleClick,
    }

    /// <summary>
    /// 키데이터
    /// </summary>
    [System.Serializable]
    public class KeyData
    {
        [SerializeField] public string keyName;
        [SerializeField] public KeyCode keyCode;
        [SerializeField] public InputKeyType inputKeyType;
        public System.Action action;

        //더블클릭관련
        private static KeyCode inputKey;
        private static float lastInputTime;
        public static float doubleClickTimeThreshold = 0.3f;

        /// <summary>
        /// 키데이터
        /// </summary>
        /// <param name="keyCode">키코드</param>
        /// <param name="inputKeyType">인풋키 타입</param>
        /// <param name="action">액션</param>
        /// /// <param name="keyName">키이름</param>
        public KeyData(KeyCode keyCode, InputKeyType inputKeyType, System.Action action, string keyName)
        {   
            this.keyCode = keyCode;
            this.inputKeyType = inputKeyType;
            this.action = action;
            this.keyName = keyName;
        }

        /// <summary>
        /// 키데이터
        /// </summary>
        /// <param name="keyName">키이름</param>
        /// <param name="keyCode">키코드</param>
        /// <param name="inputKeyType">인풋키 타입</param>
        /// <param name="action">액션</param>
        public void SetKeyData(string keyName, KeyCode keyCode, InputKeyType inputKeyType, System.Action action)
        {
            if (!string.IsNullOrEmpty(keyName))
            {
                this.keyName = keyName;
            }
            this.keyCode = keyCode;
            this.inputKeyType = inputKeyType;
            this.action = action;
        }

        /// <summary>
        /// 업데이트 키세팅
        /// </summary>
        public void UpdateKey()
        {
            switch (inputKeyType)
            {
                case InputKeyType.GetKey:
                    if (!Input.GetKey(keyCode)) { return; }
                    break;
                case InputKeyType.GetKeyDown:
                    if (!Input.GetKeyDown(keyCode)) { return; }
                    break;
                case InputKeyType.GetKeyUp:
                    if (!Input.GetKeyUp(keyCode)) { return; }
                    break;
                case InputKeyType.GetKeyDoubleClick:
                    if (!Input.GetKeyDown(keyCode)) { return; }
                    if (doubleClickTimeThreshold < Time.time - lastInputTime || inputKey != keyCode) { inputKey = keyCode; lastInputTime = Time.time; return; }
                    break;
            }
            action?.Invoke();


//#if CommandKey
            
            
//#endif
        }

        /// <summary>
        /// 더블클릭 임계값을 세팅하는 함수
        /// </summary>
        /// <param name="timer">더블클릭 임계값</param>
        public static void SetDoubleClickThreshold(float timer)
        {
            doubleClickTimeThreshold = timer;
        }
    }


#if CommandKey
    //심볼로 처리
    public class CommandSystem
    {
        public TimerModule_Element timer;
        public List<CommandKey> keyCodes = new List<CommandKey>();//입력키코드
        public Dictionary<KeyCode, CommandKey> key = new();

        [System.Serializable] public class CommandBible : CustomDictionary<CommandKey[], System.Action> { };
        public CommandBible commandBible = new();

        public class CommandKeyData
        {
            public CommandKey[] commandKeyArray;
            public System.Action action;
        }


        //커맨드키 세팅
        //스킬시스템도 이걸 체크
        public enum CommandKey
        {
            //방향키
            Up,
            Down,
            Left,
            Right,

            //마우스버튼
            LeftMouse,
            RightMouse,
        }

        public void Init()
        {
            timer.SetTimer(0.3f);
            var temp = typeof(CommandSystem);
            LogSystem.LogManager.Register(temp, temp.Name, true, true);
        }

        public void SetCommandDataArray(CommandKeyData[] commandDataArray)
        {
            //키체크
            //커맨드 체크
            commandBible.Clear();
            for (int i = 0; i < commandDataArray.Length; i++)
            {
                commandBible.Add(commandDataArray[i].commandKeyArray, commandDataArray[i].action);
            }
        }

        public void InputKey(KeyCode keyCode)
        {
            keyCodes.Add(CommandKey.Right);
            timer.ResetTime();
        }

        public void Update()
        {
            if (!timer.CheckTimer())
            {
                return;
            }

            //분할처리

            ResetInputList(); 
            CheckCommandKey();
        }

        public void ResetInputList()
        {
            //if (keyCodes.Count < 3)
            //{
            //    return;
            //}

            //List<CommandKey> splitInputKey = new List<CommandKey>();//자르는 키값들
            //CommandKey[] keys = new CommandKey[3];//값비교를 위한 선언
            //bool isFindSkill = false;
            //int spriteNum = 3;
            ////입력한 키코드에서 3번째까지만 잘라서 집어넣는곳
            //for (int i = 0; i < keyCodes.Count; i++)
            //{
            //    if (i > spriteNum - 1)
            //    {
            //        break;
            //    }
            //    keys[i] = keyCodes[i];
            //    splitInputKey.Add(keyCodes[i]);
            //}

            ////커맨드리스트에 있는 커맨드스킬과 비교하여 같으면 스킬 발동
            ////리스트안의 값을 꺼내서 값을 비교함 //값비교 //잘됨
            //bool[] IsRights = new bool[3];
            //for (int i = 0; i < commandDataArray.Length; i++)
            //{
            //    //초기화
            //    isFindSkill = true;
            //    for (int j = 0; j < IsRights.Length; j++)
            //    {
            //        IsRights[j] = false;
            //    }

            //    //커맨드키 리스트값을 가져와서는 비교
            //    //해당키값마다 비교해서 맞는지 틀린지 체크하는 구역
            //    for (int j = 0; j < commandDataArray[i].commandkeys.Length; j++)
            //    {
            //        if (keys[j] == commandDataArray[i].commandkeys[j])
            //        {
            //            IsRights[j] = true;
            //        }
            //    }

            //    //집어넣은 키값들이 다 옳으면 스킬을 찾은걸로 간주
            //    for (int j = 0; j < IsRights.Length; j++)
            //    {
            //        if (!IsRights[j])
            //        {
            //            isFindSkill = false;
            //        }
            //    }

            //    //스킬을 찾았으면 빠져나오고 스킬발동
            //    if (isFindSkill)
            //    {
            //        //기능구현 //해당번호의 커맨드스킬 반환 or 작동
            //        //string 값을 반환시키는게 좋아보인다.
            //        //ActiveSkill(commandDataArray[i].targetSkillData.targetSkillPrefab);
            //        Debug.Log(i - 1 + " 번째에 스킬이 있습니다.");

            //        break;
            //    }
            //}
            //splitInputKey.Clear();

            ////커맨드타임입력종료&&초기화
            //keyCodes.Clear();
        }



        private void CheckCommandKey()
        {
            //커맨드키관련체크
            if (keyCodes.Count < 3)
            {
                return;
            }
            if (commandBible.ContainsKey(keyCodes.ToArray()))
            {
                //스킬작동
                //LogSystem.LogManager.Log(typeof(CommandSystemManager), "커맨드스킬작동", unitObject, LogManager.LogType.Info);

                //해당유닛으로부터 스킬이 작동되게
                Debug.Log("작동");
            }
            keyCodes.Clear();
        }
    }

#endif

    /// <summary>
    /// 게임에 사용하는 인풋키처리를 위한 시스템클래스
    /// </summary>
    public class InPutKeySystem : MonoBehaviourSingleton<InPutKeySystem>
    {
        //20240515
        //MousePointer와 InPutKeyManager 통합
        public Vector3 mouseWorldPosition;
        public Vector2 mouseWorld2DPosition;
        public float mouseScreenDistance;
        public Vector3 mouseScreenPosition;
        public Ray mouseRay;

        public static Vector3 MouseWorldPosition;
        public static Vector2 MouseWorld2DPosition;
        public static float MouseScreenDistance;
        public static Vector3 MouseScreenPosition;
        public static Ray MouseRay;

#pragma warning disable CS0108 // 멤버가 상속된 멤버를 숨깁니다. new 키워드가 없습니다.
        private Camera camera;
#pragma warning restore CS0108


        //최근에 만들게 제일베이스가 괜찮긴하다
        //쓰기편하게+관리편하게+이식하면 금방쓰게
        [SerializeField] private List<KeyData> normalKeyDataList = new List<KeyData>(10);
        [SerializeField] private List<KeyData> secondaryKeyDataList = new List<KeyData>(10);

        public delegate void UpdateAction(float deltaTime);
        public UpdateAction updateAction;

        internal List<KeyData> NormalKeyDataList {get => normalKeyDataList; }
        internal List<KeyData> SecondaryKeyDataList { get => secondaryKeyDataList; }

#if CommandKey
        //커맨드처리
        public CommandSystem commandSystem = new CommandSystem();
#endif

        protected override void Init()
        {
            gameObject.name = "InPutKey";
            camera = Camera.main;
            
            Update();

#if CommandKey
            commandSystem.Init();
#endif
        }

        private void Update()
        {
            //인풋마우스로부터 시작함//다른걸 스크린2뭐시갱이 정류에 넣지말기
#if ENABLE_INPUT_SYSTEM
            mouseScreenPosition = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
#elif ENABLE_LEGACY_INPUT_MANAGER
            mouseScreenPosition = Input.mousePosition;
#endif
            mouseRay = camera.ScreenPointToRay(mouseScreenPosition);
            mouseScreenPosition.z = mouseScreenDistance;
            mouseWorldPosition = camera.ScreenToWorldPoint(mouseScreenPosition);
            mouseWorld2DPosition = mouseWorldPosition;

            //Static에 이식
            MouseWorldPosition = mouseWorldPosition;
            MouseWorld2DPosition = mouseWorld2DPosition;
            MouseScreenDistance = mouseScreenDistance;
            MouseScreenPosition = mouseScreenPosition;
            MouseRay = mouseRay;


            //키처리구간

            //노말키
            for (int i = 0; i < normalKeyDataList.Count; i++)
            {
                var key = normalKeyDataList[i];
                key.UpdateKey();
            }

            //세컨드키
            for (int i = 0; i < secondaryKeyDataList.Count; i++)
            {
                var key = secondaryKeyDataList[i];
                key.UpdateKey();
            }

            //인풋용 업데이트//플레이어용 
            var deltaTime = Time.deltaTime;
            updateAction?.Invoke(deltaTime);

#if CommandKey
            commandSystem.Update();
#endif
        }

        /// <summary>
        /// 키 데이터추가 함수
        /// </summary>
        /// /// <param name="isNormalKey">노말키여부</param>
        /// <param name="keyName">키이름</param>
        /// <param name="keyCode">키코드</param>
        /// <param name="inputKeyType">인풋키 타입</param>
        /// <param name="action">액션</param>
        public void AddKeyData(bool isNormalKey, string keyName, KeyCode keyCode, InputKeyType inputKeyType, System.Action action)
        {
#if CommandKey
            var keyData = new KeyData(keyCode, inputKeyType, ()=> { action?.Invoke(); commandSystem.InputKey(keyCode); }, keyName);           
#else
            var keyData = new KeyData(keyCode, inputKeyType, action, keyName);
#endif
            var keyDataList = GetKeyDataList(isNormalKey);
            keyDataList.Add(keyData);
        }

        public void AddKeyData(bool isNormalKey, ICollection<KeyData> keyDataArray)
        {
            var keyDataList = GetKeyDataList(isNormalKey);
            keyDataList.AddRange(keyDataArray);
        }

        public KeyData GetKeyData(bool isNormalKey, string keyName)
        {
            if (string.IsNullOrEmpty(keyName))
            {
                return null;
            }

            var keyDataList = GetKeyDataList(isNormalKey);
            for (int i = 0; i < keyDataList.Count; i++)
            {
                var key = keyDataList[i];
                if (key.keyName != keyName)
                {
                    continue;
                }
                return key;
            }
            return null;
        }

        private List<KeyData> GetKeyDataList(bool isNormalKey)
        {
            var keyDataList = isNormalKey ? NormalKeyDataList : secondaryKeyDataList;
            return keyDataList;
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            camera = null;
        }
    }
}


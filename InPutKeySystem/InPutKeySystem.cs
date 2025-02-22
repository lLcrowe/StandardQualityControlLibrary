using System.Collections.Generic;
using UnityEngine;
using lLCroweTool.Singleton;

namespace lLCroweTool.InputKey
{
    /// <summary>
    /// 인풋키 타입
    /// </summary>
    public enum InputKeyType
    {
        KeyPress,
        KeyDown,
        KeyUp,

        
        KeyDownDoubleClick,
        KeyUpDoubleClick,
    }

    /// <summary>
    /// 키데이터
    /// </summary>
    [System.Serializable]
    public class KeyData
    {
        [SerializeField] public string keyName;
#if ENABLE_INPUT_SYSTEM

#elif ENABLE_LEGACY_INPUT_MANAGER
            
#endif

        [SerializeField] public KeyCode keyCode;



        [SerializeField] public InputKeyType inputKeyType;
        public System.Action action;

        //더블클릭관련
        private static KeyCode lastInputKey;
        private static float lastInputTime;
        public static float doubleClickTimeThreshold = 0.3f;

        /// <summary>
        /// 키데이터
        /// </summary>
        /// <param name="keyCode">키코드</param>
        /// <param name="inputKeyType">인풋키 타입</param>
        /// <param name="action">액션</param>
        /// /// <param name="keyName">키이름</param>
        public KeyData(string keyName, KeyCode keyCode, InputKeyType inputKeyType, System.Action action)
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
                    if (doubleClickTimeThreshold < Time.time - lastInputTime || lastInputKey != keyCode) { lastInputKey = keyCode; lastInputTime = Time.time; return; }
                    break;
            }
            action?.Invoke();


//#if CommandKey
            //커맨드시스템과 연동
            
//#endif
        }
    }

    #region 인풋처리 구역

    public static class KeyDataExtend
    {
        /// <summary>
        /// 아무 키를 눌렸을때 체크하는 함수
        /// </summary>
        /// <returns>눌렷는지 여부</returns>
        public static bool InputAnyKey()
        {
            var key = UnityEngine.InputSystem.Keyboard.current.anyKey.isPressed;
            //마우스쪽은 애니키관련된게 없음
            var leftMouse = InputKey(UnityEngine.InputSystem.LowLevel.MouseButton.Left, InputKeyType.KeyPress);
            var rightMouse = InputKey(UnityEngine.InputSystem.LowLevel.MouseButton.Right, InputKeyType.KeyPress);

            var result = key || leftMouse || rightMouse;
            //Debug.Log(result);
            return result;
        }

        /// <summary>
        /// 키를 눌렷을때 인풋키 타입에 따라 체크하는 함수
        /// </summary>
        /// <param name="keyCode">키</param>
        /// <param name="inputKeyType">인풋 키타입</param>
        /// <returns>눌렷는지 여부</returns>
        public static bool InputKey(this UnityEngine.InputSystem.Key keyCode, in InputKeyType inputKeyType)
        {
            var key = UnityEngine.InputSystem.Keyboard.current[keyCode];
            switch (inputKeyType)
            {
                case InputKeyType.KeyDown: return key.wasPressedThisFrame;
                case InputKeyType.KeyUp: return key.wasReleasedThisFrame;
                case InputKeyType.KeyPress: return key.isPressed;
            }
            return false;
        }

        /// <summary>
        /// 키를 눌렷을때 인풋키 타입에 따라 체크하는 함수
        /// </summary>
        /// <param name="keyCode">키</param>
        /// <param name="inputKeyType">인풋 키타입</param>
        /// <returns>눌렷는지 여부</returns>
        public static bool InputKey(this UnityEngine.InputSystem.LowLevel.MouseButton keyCode, in InputKeyType inputKeyType)
        {
            UnityEngine.InputSystem.Controls.ButtonControl key = null;
            switch (keyCode)
            {
                case UnityEngine.InputSystem.LowLevel.MouseButton.Left:
                    key = UnityEngine.InputSystem.Mouse.current.leftButton;
                    break;
                case UnityEngine.InputSystem.LowLevel.MouseButton.Right:
                    key = UnityEngine.InputSystem.Mouse.current.rightButton;
                    break;
                case UnityEngine.InputSystem.LowLevel.MouseButton.Middle:
                    key = UnityEngine.InputSystem.Mouse.current.middleButton;
                    break;
                case UnityEngine.InputSystem.LowLevel.MouseButton.Forward:
                case UnityEngine.InputSystem.LowLevel.MouseButton.Back:
                    //지원안함
                    return false;
            }

            switch (inputKeyType)
            {
                case InputKeyType.KeyDown: return key.wasPressedThisFrame;
                case InputKeyType.KeyUp: return key.wasReleasedThisFrame;
                case InputKeyType.KeyPress: return key.isPressed;
            }
            return false;
        }

        public static Vector2 GetMousePosition()
        {
            return UnityEngine.InputSystem.Mouse.current.position.ReadValue();
        }

        /// <summary>
        /// 아무 키를 눌렸을때 체크하는 함수
        /// </summary>
        /// <returns>눌렷는지 여부</returns>
        public static bool InputAnyKey()
        {
            return Input.anyKey;
        }

        /// <summary>
        /// 인풋키 타입에 따른 키값을 가져오는 함수
        /// </summary>
        /// <param name="keyCode">키</param>
        /// <param name="inputKeyType">인풋 키타입</param>
        /// <returns>눌렷는가</returns>
        public static bool InputKey(this KeyCode keyCode, in InputKeyType inputKeyType)
        {
            switch (inputKeyType)
            {
                case InputKeyType.KeyDown: return Input.GetKeyDown(keyCode);
                case InputKeyType.KeyUp: return Input.GetKeyUp(keyCode);
                case InputKeyType.KeyPress: return Input.GetKey(keyCode);
            }
            return false;
        }

        public static Vector2 GetMousePosition()
        {
            return Input.mousePosition;
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
    #endregion

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
            mouseScreenPosition = KeyDataExtend.GetMousePosition();
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


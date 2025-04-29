using System.Collections.Generic;
using UnityEngine;
using lLCroweTool.Singleton;
using lLCroweTool.Dictionary;
using lLCroweTool.InputKey.Define;


//테스트
#if ENABLE_LEGACY_INPUT_MANAGER
#elif UNITY_INPUT_SYSTEM_PACKAGE
#endif


//구조를 분리
//기존 시스템

namespace lLCroweTool.InputKey
{
    public class KeyData
    {
        //20250303
        //작동방식이 변경됨
        //초기에 KeyData에 해당되는 기능이름과 인풋타입 키를 집어넣고
        //후에 다른곳에서 기능이름에 기능을 집어넣는 방식으로 변경.        

        //20250429 
        //귀찮고 번거로운거 딱 질색이라 또 분리시키는 느낌


        /// <summary>
        /// 키이름_기능이름
        /// </summary>
        [SerializeField] public string keyName;
        /// <summary>
        /// 인풋키타입
        /// </summary>
        [SerializeField] public InputKeyType inputKeyType;
        /// <summary>
        /// 작동될 액션
        /// </summary>
        public System.Action action;

#if ENABLE_LEGACY_INPUT_MANAGER
        /// <summary>
        /// 키코드
        /// </summary>
        public KeyCode keyCode;
#elif UNITY_INPUT_SYSTEM_PACKAGE
        /// <summary>
        /// 키코드
        /// </summary>
        public UnityEngine.InputSystem.Key keyCode;

        public UnityEngine.InputSystem.Mouse mouseKeyCode;
#endif
    }

    [DefaultExecutionOrder(-1000)]
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

        //액션타임
        public delegate void UpdateAction(float deltaTime);
        public UpdateAction updateAction;

        //기능이름과 키데이터
        public class KeyDataBible : CustomDictionary<string, KeyData> { }
        [SerializeField] private KeyDataBible keyDataBible = new();

        private static List<string> defineKeyNameList;
        public static string[] GetDefineKeyName =>defineKeyNameList.ToArray();

        //[SerializeField] private List<KeyData> normalKeyDataList = new List<KeyData>(10);
        //[SerializeField] private List<KeyData> secondaryKeyDataList = new List<KeyData>(10);
        //internal List<KeyData> NormalKeyDataList { get => normalKeyDataList; }
        //internal List<KeyData> SecondaryKeyDataList { get => secondaryKeyDataList; }

#if CommandKey
        //커맨드처리
        public CommandSystem commandSystem = new CommandSystem();
#endif

#pragma warning disable CS0108 // 멤버가 상속된 멤버를 숨깁니다. new 키워드가 없습니다.
        private Camera camera;
#pragma warning restore CS0108

        protected override void Init()
        {
            gameObject.name = "InPutKey";
            camera = Camera.main;

            //인풋 세팅 초기화
            InitInputSetting();
            UpdateMouseData();

#if CommandKey
            //커맨드키 등록
            commandSystem.Init();
#endif
        }

        private void InitInputSetting()
        {
            //등록
            var keyDataArray = InputKeyDefine.keyDataArray;
            defineKeyNameList = new List<string>(keyDataArray.Length);
            foreach (var item in keyDataArray)
            {
                keyDataBible.Add(item.keyName, item);
                defineKeyNameList.Add(item.keyName);
            }
        }

        //함수들 추가
        //키코드
        //인풋키
        //액션

        public void SetKeyData(in string name, in KeyCode keyCode)
        {
            if (!keyDataBible.TryGetValue(name, out var keyData))
            {
                return;
            }

            //등록
            keyData.keyCode = keyCode;
        }

        public void SetKeyData(in string name, in InputKeyType inputKeyType)
        {
            if (!keyDataBible.TryGetValue(name, out var keyData))
            {
                return;
            }

            //등록
            keyData.inputKeyType = inputKeyType;
        }

        public void SetKeyData(string name, System.Action action)
        {   
            if (!keyDataBible.TryGetValue(name, out KeyData keyData))
            {
                return;
            }

            //등록
            keyData.action = action;
        }

        public void SetKeyData(string name, in KeyCode keyCode, in InputKeyType inputKeyType, System.Action action)
        {
            if (!keyDataBible.TryGetValue(name, out KeyData keyData))
            {
                return;
            }

            //등록
            keyData.keyCode = keyCode;
            keyData.inputKeyType = inputKeyType;
            keyData.action = action;
        }


        /// <summary>
        /// 마우스값들 업데이트
        /// </summary>
        private void UpdateMouseData()
        {
            //인풋마우스로부터 시작함//다른걸 스크린2뭐시갱이 정류에 넣지말기
            mouseScreenPosition = InputKeyUtil.GetMousePosition();
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
        }


        private void Update()
        {
            //마우스데이터 갱신
            UpdateMouseData();

            //키처리구간
            //노말키
            //for (int i = 0; i < normalKeyDataList.Count; i++)
            //{
            //    var key = normalKeyDataList[i];
            //    if (!key.UpdateKey())
            //    {
            //        continue;
            //    }
            //}



            //키처리구간//통합됨.//두개의 키이상은 커맨드에서 작동
            foreach (var keyData in keyDataBible)
            {
                var key = keyData.Key;
                var data = keyData.Value;
                
                if (!InputKey(data))
                {
                    continue;
                }
                data.action?.Invoke();

#if CommandKey
                commandSystem.InputKey(data.keyName);
#endif
            }

            //인풋용 업데이트//플레이어용 
            var deltaTime = Time.deltaTime;
            updateAction?.Invoke(deltaTime);

#if CommandKey
            commandSystem.Update();
#endif
        }

        private bool InputKey(in KeyData keyData)
        {   
            var keyCode = keyData.keyName;
            switch (keyData.inputKeyType)
            {
                case InputKeyType.KeyDown: return Input.GetKeyDown(keyCode);
                case InputKeyType.KeyUp: return Input.GetKeyUp(keyCode);
                case InputKeyType.KeyPress: return Input.GetKey(keyCode);
            }
            return false;
        }


        public KeyDataBible GetKeyDataBible()
        {
            return keyDataBible;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            camera = null;
        }
    }
}


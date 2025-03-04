using System.Collections.Generic;
using UnityEngine;
using lLCroweTool.Singleton;
using lLCroweTool.Dictionary;


//테스트
#if UNITY_INPUT_SYSTEM_PACKAGE
#elif ENABLE_LEGACY_INPUT_MANAGER
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


        [SerializeField] public string keyName;
        [SerializeField] public InputKeyType inputKeyType;
        public System.Action action;

        /// <summary>
        /// 키데이터
        /// </summary>
        /// <param name="keyName">키이름</param>
        /// <param name="inputKeyType">인풋키 타입</param>
        /// <param name="action">액션</param>
        public KeyData(string keyName, InputKeyType inputKeyType, System.Action action = null)
        {
            this.keyName = keyName;
            this.inputKeyType = inputKeyType;
            this.action = action;
        }
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

        //기능이름과 키코드//필요시에 여기만 변경됨
        public class KeyEventBible : CustomDictionary<string, KeyCode> { }

        [SerializeField] private KeyDataBible keyDataBible = new();
        [SerializeField] private KeyEventBible keyEventBible = new();

        //[SerializeField] private List<KeyData> normalKeyDataList = new List<KeyData>(10);
        //[SerializeField] private List<KeyData> secondaryKeyDataList = new List<KeyData>(10);
        //internal List<KeyData> NormalKeyDataList {get => normalKeyDataList; }
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
            Update();

#if CommandKey
            commandSystem.Init();
#endif
        }

        private void InitInputSetting()
        {
            //정의
            var keyDataArray = new KeyData[]
            {
                new KeyData("LeftKey",  InputKeyType.KeyPress),
                new KeyData("RightKey", InputKeyType.KeyPress),
                new KeyData("UpKey",  InputKeyType.KeyPress),
                new KeyData("DownKey",   InputKeyType.KeyPress),
                new KeyData("Fire", InputKeyType.KeyPress),
            };


            //등록
            foreach (var item in keyDataArray)
            {
                keyDataBible.Add(item.keyName, item);
                keyEventBible.Add(item.keyName, KeyCode.None);
            }

#if CommandKey
            //커맨드키 정의


            //등록

#endif
        }

        public void SetKeyData(in string name, in KeyCode keyCode, in System.Action action)
        {
            if (!keyDataBible.TryGetValue(name, out var keyData))
            {
                return;
            }

            //키액션, 키이벤트 재등록
            keyData.action = action;
            keyEventBible[name] = keyCode;
        }

        public void SetKeyDataAction(string name, System.Action action)
        {   
            if (!keyDataBible.TryGetValue(name,out KeyData keyData))
            {
                return;
            }
            keyData.action += action;
        }

        private void Update()
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


                if (!keyEventBible.TryGetValue(key, out var keyCode))
                {
                    continue;
                }
                
                if (!InputKey(data, keyCode))
                {
                    continue;
                }
                data.action?.Invoke();

#if CommandKey
                commandSystem.InputKey(keyCode);
#endif
            }

            //인풋용 업데이트//플레이어용 
            var deltaTime = Time.deltaTime;
            updateAction?.Invoke(deltaTime);

#if CommandKey
            commandSystem.Update();
#endif
        }

        private bool InputKey(in KeyData keyData, in KeyCode keyCode)
        {   
            var inputKeyType = keyData.inputKeyType;
            switch (inputKeyType)
            {
                case InputKeyType.KeyDown: return Input.GetKeyDown(keyCode);
                case InputKeyType.KeyUp: return Input.GetKeyUp(keyCode);
                case InputKeyType.KeyPress: return Input.GetKey(keyCode);
            }
            return false;
        }


        //액션 및 데이터 변경처리


        //        /// <summary>
        //        /// 키 데이터추가 함수
        //        /// </summary>
        //        /// /// <param name="isNormalKey">노말키여부</param>
        //        /// <param name="keyName">키이름</param>
        //        /// <param name="keyCode">키코드</param>
        //        /// <param name="inputKeyType">인풋키 타입</param>
        //        /// <param name="action">액션</param>
        //        /// 
        //#if UNITY_INPUT_SYSTEM_PACKAGE
        //        public void AddKeyData(bool isNormalKey, string keyName, UnityEngine.InputSystem.Key keyCode, InputKeyType inputKeyType, System.Action action)
        //#elif ENABLE_LEGACY_INPUT_MANAGER
        //        public void AddKeyData(bool isNormalKey, string keyName, KeyCode keyCode, InputKeyType inputKeyType, System.Action action)
        //#endif
        //        {
        //            var keyData = new KeyData(keyName, keyCode, inputKeyType);
        //            var keyDataList = GetKeyDataList(isNormalKey);
        //            keyDataList.Add(keyData);
        //        }

        //        public void AddKeyData(bool isNormalKey, ICollection<KeyData> keyDataArray)
        //        {
        //            var keyDataList = GetKeyDataList(isNormalKey);
        //            keyDataList.AddRange(keyDataArray);
        //        }


        //public KeyData GetKeyData(bool isNormalKey, string keyName)
        //{
        //    if (string.IsNullOrEmpty(keyName))
        //    {
        //        return null;
        //    }

        //    var keyDataList = GetKeyDataList(isNormalKey);
        //    for (int i = 0; i < keyDataList.Count; i++)
        //    {
        //        var key = keyDataList[i];
        //        if (key.keyName != keyName)
        //        {
        //            continue;
        //        }
        //        return key;
        //    }
        //    return null;
        //}


        //UI쪽에 옮기기
        //#if UNITY_INPUT_SYSTEM_PACKAGE
        //        public void SetButtonAction(in bool isNormalKey, in UnityEngine.InputSystem.Key keyCode, in CustomButton customButton)
        //#elif ENABLE_LEGACY_INPUT_MANAGER
        //        public void SetButtonAction(in bool isNormalKey, in KeyCode keyCode,in CustomButton customButton)
        //#endif
        //        {
        //            var keyDataList = GetKeyDataList(isNormalKey);
        //            for (int i = 0; i < keyDataList.Count; i++)
        //            {
        //                var key = keyDataList[i];
        //                if (key.keyCode != keyCode)
        //                {
        //                    continue;
        //                }

        //                customButton.onClickAction = key.action;
        //                break;
        //            }
        //        }

        //private List<KeyData> GetKeyDataList(in bool isNormalKey)
        //{
        //    var keyDataList = isNormalKey ? NormalKeyDataList : secondaryKeyDataList;
        //    return keyDataList;
        //}


        protected override void OnDestroy()
        {
            base.OnDestroy();
            camera = null;
        }
    }
}


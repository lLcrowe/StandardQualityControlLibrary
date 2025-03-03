//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

////신규인풋시스템
//namespace lLCroweTool.NewInputKey
//{
//#if UNITY_INPUT_SYSTEM_PACKAGE
//    /// <summary>
//    /// 키데이터
//    /// </summary>
//    [System.Serializable]
//    public class MouseData : EventKeyData
//    {
//        public UnityEngine.InputSystem.LowLevel.MouseButton keyCode;
//    }

//    public class KeyboardData: EventKeyData
//    {
//        public UnityEngine.InputSystem.Keyboard keyCode;

//         /// <summary>
//        /// 키데이터
//        /// </summary>
//        /// <param name="keyName">키이름</param>
//        /// <param name="keyCode">키코드</param>
//        /// <param name="inputKeyType">인풋키 타입</param>
//        /// <param name="action">액션</param>
//        public void SetKeyData(string keyName, UnityEngine.InputSystem.Key keyCode, InputKeyType inputKeyType, System.Action action)
//        {
//            if (!string.IsNullOrEmpty(keyName))
//            {
//                this.keyName = keyName;
//            }
//            this.keyCode = keyCode;
//            this.inputKeyType = inputKeyType;
//            this.action = action;
//        }
//    }
//#endif




//    //            for (int i = 0; i < mouseKeyDataList.Count; i++)
//    //            {
//    //                var key = mouseKeyDataList[i];
//    //                if (!key.UpdateKey())
//    //                {
//    //                    continue;
//    //                }
//    //#if CommandKey
//    //                commandSystem.InputKey(key.keyCode);
//    //#endif
//    //            }

//    //InPutKeySystem를 참고하여 체크
//    class NewInputSystem
//    {
//        //올드와는 따로 작동되게 변경

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
//#if UNITY_INPUT_SYSTEM_PACKAGE
//        public void AddMouseKeyData(bool isNormalKey, ICollection<MouseData> keyDataArray)
//        {
//            mouseKeyDataList.AddRange(keyDataArray);
//        }
//#endif

//        public KeyData GetKeyData(bool isNormalKey, string keyName)
//        {
//            if (string.IsNullOrEmpty(keyName))
//            {
//                return null;
//            }

//            var keyDataList = GetKeyDataList(isNormalKey);
//            for (int i = 0; i < keyDataList.Count; i++)
//            {
//                var key = keyDataList[i];
//                if (key.keyName != keyName)
//                {
//                    continue;
//                }
//                return key;
//            }
//            return null;
//        }


//        //public void SetButtonAction(in bool isNormalKey, in UnityEngine.InputSystem.Key keyCode, in CustomButton customButton)
//        //{
//        //    var keyDataList = GetKeyDataList(isNormalKey);
//        //    for (int i = 0; i < keyDataList.Count; i++)
//        //    {
//        //        var key = keyDataList[i];
//        //        if (key.keyCode != keyCode)
//        //        {
//        //            continue;
//        //        }

//        //        customButton.onClickAction = key.action;
//        //        break;
//        //    }
//        //}
//    }
//}

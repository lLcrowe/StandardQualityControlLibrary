using UnityEngine;

namespace lLCroweTool.InputKey
{
    /// <summary>
    /// 인풋키 타입
    /// </summary>
    public enum InputKeyType
    {
        //일반키
        KeyPress,
        KeyDown,
        KeyUp,

        //더블클릭
        KeyDownDoubleClick,
        KeyUpDoubleClick,
    }

    //인풋구역을 관리하는 유틸
    public static class InputKeyUtil
    {
        //더블클릭관련
        public static float lastInputTime;
        public static float doubleClickTimeThreshold = 0.3f;

#if UNITY_INPUT_SYSTEM_PACKAGE
        public static bool lastStateKey;
        public static UnityEngine.InputSystem.Key lastInputKey;
        public static UnityEngine.InputSystem.LowLevel.MouseButton lastInputMouseKey;

#elif ENABLE_LEGACY_INPUT_MANAGER
        public static KeyCode lastInputKey;
#endif

        /// <summary>
        /// 더블클릭 임계값을 세팅하는 함수
        /// </summary>
        /// <param name="timer">더블클릭 임계값</param>
        public static void SetDoubleClickThreshold(float timer)
        {
            doubleClickTimeThreshold = timer;
        }

        /// <summary>
        /// 더블클릭 시간체크하는 함수
        /// </summary>
        /// <param name="keyCode">키코드</param>
        /// <returns>더블클릭인지 여부</returns>
        public static bool CheckDoubleClickTime(KeyCode keyCode)
        {
            if (doubleClickTimeThreshold < Time.time - lastInputTime || lastInputKey != keyCode)
            {
                lastInputKey = keyCode; lastInputTime = Time.time;
                return false;
            }
            return true;
        }


        /// <summary>
        /// 업데이트 키세팅
        /// </summary>
        public static bool UpdateKey(KeyCode keyCode, InputKeyType inputKeyType)
        {
            switch (inputKeyType)
            {
                case InputKeyType.KeyPress:
                    if (!InputKey(keyCode, InputKeyType.KeyPress)) { return false; }
                    break;
                case InputKeyType.KeyDown:
                    if (!InputKey(keyCode, InputKeyType.KeyDown)) { return false; }
                    break;
                case InputKeyType.KeyUp:
                    if (!InputKey(keyCode, InputKeyType.KeyUp)) { return false; }
                    break;
                case InputKeyType.KeyDownDoubleClick:
                    if (!InputKey(keyCode, InputKeyType.KeyDown)) { return false; }
                    if (CheckDoubleClickTime(keyCode)) { return false; }
                    break;
                case InputKeyType.KeyUpDoubleClick:
                    if (!InputKey(keyCode, InputKeyType.KeyUp)) { return false; }
                    if (CheckDoubleClickTime(keyCode)) { return false; }
                    break;
            }

            //액션작동
            //action?.Invoke();
            return true;
        }

#if UNITY_INPUT_SYSTEM_PACKAGE
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
            var test = UnityEngine.InputSystem.Mouse.current;
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

#elif ENABLE_LEGACY_INPUT_MANAGER

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
#endif
    }
}


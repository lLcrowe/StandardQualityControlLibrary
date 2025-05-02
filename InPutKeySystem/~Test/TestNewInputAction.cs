#if UNITY_INPUT_SYSTEM_PACKAGE
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

namespace Assets.StandardQualityControlLibary.InPutKeySystem
{
	public class TestNewInputAction : MonoBehaviour
	{

        //바딩인처리이이긴한데..
        //흠 구지
        private InputAction inputAction;

        // Use this for initialization
        void Start()
		{
            //-=바인딩1
            // InputAction 생성
            inputAction = new InputAction("UnifiedInput");

            // 키보드 바인딩 (예: Space, A 키)
            inputAction.AddBinding("<Keyboard>/space");
            inputAction.AddBinding("<Keyboard>/a");

            // 마우스 바인딩 (예: 왼쪽 버튼, 오른쪽 버튼)
            inputAction.AddBinding("<Mouse>/leftButton");
            inputAction.AddBinding("<Mouse>/rightButton");

            // 이벤트 연결
            inputAction.performed += ctx => Debug.Log($"입력 발생: {ctx.control.path}");

            inputAction.Enable();

            //-=바인딩2
            inputAction = new InputAction("MyAction", binding: "<Keyboard>/space");

            inputAction.started += ctx => Debug.Log("🔹 키를 누르기 시작!");
            inputAction.performed += ctx => Debug.Log("✅ 키를 누르는 중!");
            inputAction.canceled += ctx => Debug.Log("❌ 키를 뗌!");

            inputAction.Enable();
        }

		// Update is called once per frame
		void Update()
		{

		}
	}
}
#endif
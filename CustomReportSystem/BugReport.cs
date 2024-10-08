using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;


namespace lLCroweTool
{
	public class BugReport : MonoBehaviour
	{
		//유니티 웹크롤링
		//주소 찾을려면 form action 검색
		const string BugReport_URL = "https://docs.google.com/forms/u/0/d/e/YOUR_GOOGLE_FORM_ID_HERE/formResponse";

		//public List<Sprite> bugSpriteList = new List<Sprite>();
		public TMP_Dropdown targetDropdown;
		public TMP_InputField targetInputField;
		public Button targetButton;
		
        private void Awake()
        {
			//드랍박스 초기화

			List<string> bugTypeList = new List<string>();

			//영어
   //         for (int i = 0; i < Enum.GetNames(typeof(BugReportType_ENG)).Length; i++)
   //         {
			//	bugTypeList.Add(Enum.GetName(typeof(BugReportType_ENG), i));
			//}

			//한국어
			for (int i = 0; i < Enum.GetNames(typeof(BugReportType_KOR)).Length; i++)
			{
				bugTypeList.Add(Enum.GetName(typeof(BugReportType_KOR), i));
			}

			targetDropdown.ClearOptions();
			//targetDropdown.AddOptions(bugSpriteList);
			targetDropdown.AddOptions(bugTypeList);

			targetButton.onClick.AddListener(SendReport);
		}

		/// <summary>
		/// 버그를 보내주는 함수
		/// </summary>
        public void SendReport()
		{
			WWWForm form = new WWWForm();
			//formField 찾을려면 entry 검색
			form.AddField("entry.YOUR_ENTRY_ID_HERE", "CAT : " + targetDropdown.itemText + "\n" + "" + targetInputField.text);


			UnityWebRequest www = UnityWebRequest.Post(BugReport_URL, form);
			www.SendWebRequest();

			//완료 했으면
			//알림 확인창 뛰우기
		}

		/// <summary>
		/// 스크린샷작동
		/// </summary>
		/// <param name="screenShotName"></param>
		public void ActionScreenShot(string screenShotName)
        {
			lLcroweUtil.ActionScreenShot(screenShotName);
		}

		/// <summary>
		/// 이미지 크기 변경 함수
		/// </summary>
		/// <param name="source"></param>
		/// <param name="targetWidth"></param>
		/// <param name="targetHeight"></param>
		/// <returns></returns>
		Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
		{
			Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
			Color[] rpixels = result.GetPixels(0);
			float incX = (1.0f / (float)targetWidth);
			float incY = (1.0f / (float)targetHeight);
			for (int px = 0; px < rpixels.Length; px++)
			{
				rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
			}
			result.SetPixels(rpixels, 0);
			result.Apply();
			return result;
		}


		public enum BugReportType_ENG
		{
			GamePlay,
			Story,
			UI,
			Graphic,
			Audio,
			ETC,
		}
		public enum BugReportType_KOR
		{
			게임플레이,
			이야기,
			유저인터페이스,
			그래픽,
			오디오,
			기타,
		}
	}
}


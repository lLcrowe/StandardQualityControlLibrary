#if Doozy
using UnityEngine;
using MEC;
using DarkTonic.MasterAudio;
using Doozy.Engine.UI;
using TMPro;
using System.Collections.Generic;

namespace lLCroweTool.DialogueSystem
{
    public class DialogueTextBox : MonoBehaviour
    {
        //대화텍스트 박스
        //대화시스템중의 일부로
        //대화데이터안의 컨텐츠를 보여주는 역할을 가짐
       
        [SoundGroup] public string soundName;  
        
        //캐싱
        private bool isAnimationTextOver = false;//텍스트가 다나왔지 체크
        private string targetText = "";
        public TextMeshProUGUI targetTextObject;
        private CoroutineHandle textRunHandle;

        private UIView targetUIView;


        private void Awake()
        {
            targetUIView = GetComponent<UIView>();
            //targetTextObject = GetComponent<TextMeshProUGUI>();
        }

        /// <summary>
        /// 텍스트박스 설정 초기화
        /// </summary>
        public void InitSetTextBox(TMP_FontAsset font)
        {
            targetTextObject.font = font;
        }

        /// <summary>
        /// 메세지를 텍스트박스에 보여주기
        /// </summary>
        /// <param name="msg">메세지</param>
        /// <param name="typingAnimationType">작동할 타이핑애니메이션타입</param>
        public void SetText(string msg, TypingAnimationType typingAnimationType = TypingAnimationType.Normal)
        {
            isAnimationTextOver = false;
            targetText = msg;
            targetTextObject.text = "";
            if (textRunHandle.IsRunning)
            {                
                Timing.KillCoroutines(textRunHandle);
            }
            switch (typingAnimationType)
            {
                case TypingAnimationType.Normal:
                    targetTextObject.text = targetText;
                    isAnimationTextOver = true;
                    break;
                case TypingAnimationType.LetterStep:                   
                    textRunHandle = Timing.RunCoroutine(AnimTextStep(targetText));
                    break;
                case TypingAnimationType.TextLetterStepAndWordStep:                 
                    textRunHandle = Timing.RunCoroutine(AnimTextStep(targetText));
                    break;
                case TypingAnimationType.WordStep:                   
                    textRunHandle = Timing.RunCoroutine(AnimTextStep(targetText));
                    break;
            }
        }


        //기본적인텍스트 작동//후에 기능추가
        public IEnumerator<float> AnimTextStep(string msg)
        {
            int index = 0;

            while (index < msg.Length)
            {
                //입력
                targetTextObject.text += msg[index];
                //소리재생
                //MasterAudio.PlaySound3DAtTransform(soundName, Camera.main.transform);
                index++;
                //대기
                yield return Timing.WaitForSeconds(4f / DialogueSystemManager.Instance.GetTypingSpeed());
            }
            isAnimationTextOver = true;
        }

        /// <summary>
        /// 텍스트스킵하기
        /// </summary>
        public void SkipText()
        {
            //작동코루틴 중지
            if (!ReferenceEquals(textRunHandle, null))
            {
                if (textRunHandle.IsRunning)
                {
                    Timing.KillCoroutines(textRunHandle);
                }
            }            
            targetTextObject.text = targetText;     //그대로 입력
            isAnimationTextOver = true;             //완료됫는지 체크
        }

        /// <summary>
        /// 텍스트 애니메이션이 진행이 끝났는지 체크
        /// </summary>
        /// <returns>끝났는지 여부</returns>
        public bool IsTextOver()
        {
            return isAnimationTextOver;
        }

        /// <summary>
        /// 다음으로 진행이 가능한지 체크하기위한 코루틴//필요없을듯
        /// </summary>
        /// <returns>진행가능</returns>
        public IEnumerator<float> IsTextOverCoroutine()
        {
            do
            {
                if (isAnimationTextOver)
                {
                    break;
                }
               
                yield return Timing.WaitForOneFrame;                
            } while (true);
        }

        /// <summary>
        /// 텍스트박스를 보여주는 함수
        /// </summary>
        public void ShowDialogueTextBox()
        {
            targetUIView.Show();
        }

        /// <summary>
        /// 텍스트박스를 닫는 함수
        /// </summary>
        public void OffDialogueTextBox()
        {
            targetUIView.Hide();
        }
    }
}
#endif
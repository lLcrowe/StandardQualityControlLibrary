#if Doozy
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using TMPro;
using lLCroweTool.Sound;

namespace lLCroweTool
{
    public class AnimationText : MonoBehaviour
    {
        //DialogueTextBox로 사용함 이건 그외의 용도로 쓰기


        //클래스 이름 변경할 가능성높음
        //devdog의 textAnimator 스크립트를 참조함
        public AnimationType animationType;
        public float animationSpeed = 10;//디폴트 10 //스피디함
        public SoundObjectScript[] stepAudioClips;
        private TextMeshProUGUI targetText;
        private CoroutineHandle coroutineHandle;


        private void Awake()
        {
            targetText = GetComponent<TextMeshProUGUI>();
        }

        //private void Update()
        //{
           




        //}


        public void AnimateText(string msg)
        {
            if (!ReferenceEquals(coroutineHandle, null))
            {
                if (coroutineHandle.IsRunning)
                {
                    Timing.KillCoroutines(coroutineHandle);
                }
            }
            targetText.text = "";
            
            switch (animationType)
            {
                case AnimationType.LetterStep_A:
                    coroutineHandle = Timing.RunCoroutine(_SetTextLetterStepA(msg));                    
                    break;
                case AnimationType.LetterStep_B:
                    coroutineHandle = Timing.RunCoroutine(_SetTextLetterStepB(msg));
                    break;
                case AnimationType.TextLetterStepAndWordStep:
                    coroutineHandle = Timing.RunCoroutine(_SetTextLetterStepAndWordStep(msg));
                    break;               
                case AnimationType.WordStep:
                    coroutineHandle = Timing.RunCoroutine(_SetTextWordStep(msg));
                    break;
                case AnimationType.FadeIn:
                    coroutineHandle = Timing.RunCoroutine(_SetTextFadeIn(msg));
                    break;
                case AnimationType.FadeOut:
                    coroutineHandle = Timing.RunCoroutine(_SetTextFadeOut(msg));
                    break;

            }
        }

        //private IEnumerator _SetTextLetterStep(string msg)
        //{
        //    var waitTime = new WaitForSeconds(1f / animationSpeed);

        //    int index = 0;
        //    while (index < msg.Length)
        //    {
        //        targetText.text += msg[index];
        //        PlayRandomStepClip();
        //        yield return waitTime;
        //        index++;
        //    }
        //}

        //private IEnumerator _SetTextWordStep(string msg)
        //{
        //    var waitTime = new WaitForSeconds(1f / animationSpeed);

        //    var words = msg.Split(' ');

        //    int index = 0;
        //    while (index < words.Length)
        //    {
        //        targetText.text += words[index] + " ";
        //        PlayRandomStepClip();
        //        yield return waitTime;
        //        index++;
        //    }
        //}


        //private IEnumerator _SetTextFadeIn(string msg)
        //{
        //    targetText.text = msg;

        //    var startColor = targetText.color;
        //    startColor.a = 0f;
        //    targetText.color = startColor;

        //    float time = 1f / animationSpeed;
        //    float timer = 0f;
        //    while (timer < time)
        //    {
        //        timer += Time.deltaTime;
        //        startColor.a += animationSpeed * Time.deltaTime;
        //        targetText.color = startColor;

        //        PlayRandomStepClip();
        //        yield return null;
        //    }
        //}

        //한개씩 보여주기A
        private IEnumerator<float> _SetTextLetterStepA(string msg)
        {
            var waitTime = Timing.WaitForSeconds(4f / animationSpeed);
            int index = 0;                     

            while (index < msg.Length)
            {
                targetText.text += msg[index];
                yield return waitTime;
                PlayRandomStepClip();
                yield return Timing.WaitForSeconds(1f / animationSpeed);
                index++;
            }
        }

        //한개씩 보여주기B
        private IEnumerator<float> _SetTextLetterStepB(string msg)
        {            
            int index = 0;

            while (index < msg.Length)
            {
                targetText.text += msg[index];
                PlayRandomStepClip();
                yield return Timing.WaitForSeconds(1f / animationSpeed);
                index++;
            }
        }

        //한개씩 보여주기와 단어식 보여주기 합친것
        private IEnumerator<float> _SetTextLetterStepAndWordStep(string msg)
        {
            int index = 0;

            while (index < msg.Length)
            {
                if (msg[index] == ' ')
                {
                    targetText.text += msg[index];
                    PlayRandomStepClip();
                    yield return Timing.WaitForSeconds(4f / animationSpeed);
                }
                else
                {
                    targetText.text += msg[index];
                    PlayRandomStepClip();
                    yield return Timing.WaitForSeconds(1f / animationSpeed);
                }
                index++;
            }

        }
      

        //단어로 보여주기
        private IEnumerator<float> _SetTextWordStep(string msg)
        {
            int index = 0;
            var words = msg.Split(' ');
            while (index < words.Length)
            {
                targetText.text += words[index] + " ";
                PlayRandomStepClip();
                yield return Timing.WaitForSeconds(1f / animationSpeed);
                index++;
            }
        }

        //서서히 보여주는
        private IEnumerator<float> _SetTextFadeIn(string msg)
        {             
            var startColor = targetText.color;
            startColor.a = 0f;
            targetText.color = startColor;
            targetText.text = msg;
            Timing.WaitForSeconds(1f / animationSpeed);
            PlayRandomStepClip();
            while (startColor.a <= 1)
            {                
                startColor.a += 1f / animationSpeed;
                targetText.color = startColor;
                //PlayRandomStepClip();
                yield return Timing.WaitForSeconds(0.02f);
            }
        }

        //서서히 가려주는
        private IEnumerator<float> _SetTextFadeOut(string msg)
        {           
            targetText.text = msg;
            var startColor = targetText.color;
            startColor.a = 1f;
            targetText.color = startColor;
            PlayRandomStepClip();
            while (startColor.a >= 0)
            {
                startColor.a -= 1f / animationSpeed;
                targetText.color = startColor;
                //PlayRandomStepClip();
                yield return Timing.WaitForSeconds(0.02f);
            }
        }

        protected void PlayRandomStepClip()
        {
            if (stepAudioClips.Length > 0)
            {
                int index = Random.Range(0, stepAudioClips.Length);
                //랜덤으로 해당 클립을 작동하게 만듬
                //AudioManager.AudioPlayOneShot(stepAudioClips[index]);
                SoundManager.Instance.PlaySound3DAtTransform(false, Camera.main.transform, stepAudioClips[index]);
            }
        }

        public TextMeshProUGUI GetAnimationText()
        {
            return targetText;
        }

        public enum AnimationType
        {
            LetterStep_A,
            LetterStep_B,
            TextLetterStepAndWordStep,           
            WordStep,
            FadeIn,
            FadeOut,
        }
    }

}

#endif
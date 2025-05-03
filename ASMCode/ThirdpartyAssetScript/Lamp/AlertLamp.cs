#if Light2D
using FunkyCode;
using lLCroweTool.TimerSystem;
using UnityEngine;

namespace lLCroweTool.LampSystem
{
    [RequireComponent(typeof(UpdateTimerModule))]
    //[RequireComponent(typeof(LightingSource2D))]
    public class AlertLamp : Lamp
    {

        UpdateTimerModule timerModule;
        //LightingSource2D lightingSource;
        [Range(0.1f, 5f)]
        public float lampTimer = 1f;//램프의 전체적인 시간
        public bool isLampOn = true;
        private float waitTime; //기다리는 시간    
        private float waitTimer = 0;//기다리는 시간 체크용
        private float timeValue = 0;


        private void Awake()
        {
            StartLampSetting();
        }
        public void StartLampSetting()
        {
            timerModule = GetComponent<UpdateTimerModule>();
            timerModule.AddUnityEvent(delegate { AlertLampUpdate(); });
            timerModule.SetTimer(0f);
            timerModule.enabled = false;

            lightingSource = GetComponent<Light2D>();

            timeValue = lightingSource.color.a;
            //if (isLampOn)
            //{
            //    lightingSource.lightAlpha = 1;
            //}
            //else
            //{
            //    lightingSource.lightAlpha = 0;
            //}
        }


        private void AlertLampUpdate()
        {
            //if (!LampWaitTimer())
            //{
            //    return;
            //}
            //램프가 완전히 켜졋을떄
            if (isLampOn)
            {
                timeValue += 1 / lampTimer * Time.deltaTime;
                if (timeValue >= 1)
                {
                    isLampOn = false;
                    waitTimer = 0;
                }
            }
            //램프가 완전히 꺼졋을때
            else
            {
                timeValue -= 1 / lampTimer * Time.deltaTime;
                if (timeValue <= 0)
                {
                    isLampOn = true;
                    waitTimer = 0;
                }
            }

            lightingSource.color.a = timeValue;
        }

        //private bool LampWaitTimer()
        //{
        //    bool isRight = false;
        //    waitTimer += 1 / waitTime * Time.deltaTime;
        //    if (waitTimer >= 1)
        //    {
        //        //언제작동되는지체크해야함
        //        Debug.Log("램프기다림타이머작동");
        //        isRight = true;
        //    }

        //    return isRight;
        //}

        //알람램프의 활성화 함수
        public void ActiveAlertLamp()
        {
            timerModule.enabled = !timerModule.enabled;
        }
        public void ActiveAlertLamp(bool onOff)
        {
            timerModule.enabled = onOff;
        }

        //알람램프의 리셋 함수
        public void ResetAlertLamp()
        {
            ActiveAlertLamp(false);
            if (isLampOn)
            {
                lightingSource.color.a = 1;
            }
            else
            {
                lightingSource.color.a = 0;
            }
        }

        public bool GetWaitRandomMoveCycleTime()
        {
            bool CheckTime = false;
            if (Time.time > waitTime + waitTimer + lampTimer)
            {
                CheckTime = true;
                waitTimer = Time.time;
            }
            return CheckTime;
        }
    }

}

#endif
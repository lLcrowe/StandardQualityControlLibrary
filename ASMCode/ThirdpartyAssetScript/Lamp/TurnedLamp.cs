#if Light2D
using UnityEngine;
using System.Collections;
using FunkyCode;
using lLCroweTool.TimerSystem;

namespace lLCroweTool.LampSystem
{
    [RequireComponent(typeof(UpdateTimerModule))]
    public class TurnedLamp : Lamp
    {
        private UpdateTimerModule timerModule;
        //LightingSource2D lightingSource;
        [Range(0.1f, 5f)]
        public float lampTimer = 1f;//램프의 전체적인 시간
        public bool isLampOn = true;
        float timeValue = 0;

        private void Awake()
        {
            StartLampSetting();
        }
        public void StartLampSetting()
        {
            timerModule = GetComponent<UpdateTimerModule>();
            timerModule.AddUnityEvent(delegate { TurnedLampUpdate(); });
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

        private void TurnedLampUpdate()
        {
            //램프가 완전히 켜졋을떄
            if (isLampOn)
            {
                timeValue += 1 / lampTimer * Time.deltaTime;
                if (timeValue >= 1)
                {
                    isLampOn = false;
                    timerModule.enabled = false;
                }
            }
            //램프가 완전히 꺼졋을때
            else
            {
                timeValue -= 1 / lampTimer * Time.deltaTime;
                if (timeValue <= 0)
                {
                    isLampOn = true;
                    timerModule.enabled = false;
                }
            }

            lightingSource.color.a = timeValue;
        }
        public void ActiveTurnedLamp()
        {
            //isLampOn = !isLampOn;
            if (timerModule.enabled == false)
            {
                timerModule.enabled = true;
            }
        }
        public void TurnedLampOff()
        {
            isLampOn = true;
            timerModule.enabled = false;
            lightingSource.color.a = 0;
        }
    }

}
#endif
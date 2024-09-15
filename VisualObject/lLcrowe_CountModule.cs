using UnityEngine;
using UnityEngine.Events;

namespace lLCroweTool.TimerSystem
{
    [RequireComponent(typeof(UpdateTimerModule))]
    public class lLcrowe_CountModule : MonoBehaviour
    {
        //업데이트모듈타이머를 사용하여
        //카운트모듈로 제작
        //말그대로 카운트모듈
        [SerializeField] private int countTimer;                     //타이머
        public bool isRepeat = false;       //반복 동작인지
        public int countNeedTime;           //원하는 작동시간

        public UnityEvent unityEvent;       //카운트모듈의 이벤트

        //업데이트타이머 모듈러
        UpdateTimerModule timerModule;

        //카운트 UI
        //필요하면 직접할당    
        public bool useUI = false;                 //UI사용여부
        public lLcrowe_CountModuleUI countModuleUI;//UI모듈

        private void Awake()
        {
            if (ReferenceEquals(timerModule, null))
            {
                timerModule = GetComponent<UpdateTimerModule>();
            }
            timerModule.AddUnityEvent(delegate { Count(); });
            timerModule.SetTimer(1);

            if (!ReferenceEquals(countModuleUI, null))
            {
                useUI = true;
            }
            else
            {
                useUI = false;
            }
        }

        //외부에서 해당카운트모듈을 사용할떄 쓰는 유일한 함수
        public void CountStart()
        {
            timerModule.enabled = true;
            countModuleUI.CountModuleUIONOFF(true);
        }

        //업데이트타이머 모듈에서 이벤트로호출
        //집어넣어준다
        private void Count()
        {
            countTimer++;
            if (countTimer > countNeedTime)
            {
                unityEvent.Invoke();

                countTimer = 1;
                if (!isRepeat)
                {
                    timerModule.enabled = false;
                    countModuleUI.CountModuleUIONOFF(false);
                }
            }
            CountUIUpdate();
        }

        //카운트UI모듈관련
        private void CountUIUpdate()
        {
            if (!useUI)
            {
                return;
            }
            //UI업데이트
            countModuleUI.CountModuleUIUpdate(countTimer, countNeedTime);
        }
    }

}


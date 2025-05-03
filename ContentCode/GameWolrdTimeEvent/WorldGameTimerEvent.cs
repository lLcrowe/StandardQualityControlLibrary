using lLCroweTool.TimerSystem;
using UnityEngine;
using UnityEngine.Events;

namespace lLCroweTool.GameWorldTimeSystem
{
    [RequireComponent(typeof(UpdateTimerModule))]
    public class WorldGameTimerEvent : MonoBehaviour
    {
        //옛날거임 좀더 체크해보기


        //업데이트타이머 모듈러
        private UpdateTimerModule timerModule;

        //월드시간에 대한 변수들
        public int worldDay = 0;//월드시간 (회차)
        public int worldDayNeedValue = 100;      //하루에 필요한 월드 값
        public int worldTimeValue;         //현재 월드값

        [Space]
        //낮과밤 등을 위한 월드시간 값
        //오전 오후로 할지 : 2가지분류됨
        //OR 밤시간 오후시간 아침시간
        public int worldNightTime;         //밤시간때
        public int worldDayTime;           //낮시간때

        [Space]
        //밤과 낮의 이벤트
        //밤
        public UnityEvent nightTimeEvent;
        [Range(0, 100)]
        public int nightTimeEventPromise = 50;
        //낮
        public UnityEvent dayTimeEvent;
        [Range(0, 100)]
        public int dayTimeEventPromise = 50;


        public bool eventTime;
        public bool nowTime;

        //UI관련
        //직접할당    
        public bool isUseUI;
        public WorldGameTimerEventUI worldGameTimerEvent;


        private void Awake()
        {
            //타이머모듈을 가져옴
            timerModule = GetComponent<UpdateTimerModule>();

            //초기세팅
            nowTime = DayOrNightCal();
            UpdateWorldTimerUI();

            if (nightTimeEvent == null)
            {
                nightTimeEvent = new UnityEvent();
            }

            if (dayTimeEvent == null)
            {
                dayTimeEvent = new UnityEvent();
            }
        }

        private void UpdateWorldTimerUI()
        {
            if (!isUseUI)
            {
                return;
            }
            worldGameTimerEvent.WorldDayUIUpdate(worldDay);
            worldGameTimerEvent.WorldTimeUIUpdate(worldTimeValue);
            worldGameTimerEvent.DayornightUIUpdate(nowTime);
        }

        //날짜 카운트다운
        //업데이트타이머모듈러에서 이벤트호출로 불러옴
        public void dayCountDown()
        {
            worldTimeValue++;
            if (worldTimeValue >= worldDayNeedValue)
            {
                worldDay++;
                worldTimeValue = 0;
            }
            nowTime = DayOrNightCal();

            //이벤트관련
            if (nowTime != eventTime)
            {
                WorldTimeEvent();
            }

            //UI관련
            UpdateWorldTimerUI();

        }

        //메뉴들어갈떄 또는 정지버튼을 눌렸을때 
        //버튼 이벤트로 호출
        public void UsePause()
        {
            //timerModule.enabled = !timerModule.enabled;
            TimerModuleManager.Instance.isPause = !TimerModuleManager.Instance.isPause;
        }

        //타임에 의한 이벤트시작
        public void WorldTimeEvent()
        {
            //밤시간일떄
            if (worldDayTime < worldTimeValue && worldTimeValue < worldNightTime)
            {
                //일정확률로 밤트리거 작동
                if (lLcroweUtil.ProbabilityCal(nightTimeEventPromise))
                {
                    nightTimeEvent.Invoke();
                }
            }
            //낮시간일떄 
            else if (0 < worldTimeValue && worldTimeValue < worldDayTime)
            {
                //일정확률로 낮트리거 작동
                if (lLcroweUtil.ProbabilityCal(dayTimeEventPromise))
                {
                    dayTimeEvent.Invoke();
                }
            }
        }

        //낮과 밤 시간계산
        public bool DayOrNightCal()
        {
            bool isDay = false;
            //밤시간일떄
            if (worldDayTime < worldTimeValue && worldTimeValue < worldNightTime)
            {
                return isDay;
            }
            //낮시간일떄 
            else if (0 <= worldTimeValue && worldTimeValue <= worldDayTime)
            {
                return isDay = true;
            }
            return isDay;
        }

        /// <summary>
        /// 화면크기에 맞게 트랜스폼 스케일값을 맞추어 주는 함수
        /// </summary>
        /// <param name="target"></param>
        public void FillGameObjectToCamera(SpriteRenderer sr, Transform target)
        {
            float spriteX = sr.sprite.bounds.size.x;
            float spriteY = sr.sprite.bounds.size.y;

            float height = Camera.main.orthographicSize * 2;
            float width = height / Screen.height * Screen.width;
            transform.localScale = new Vector2(Mathf.Ceil(width / spriteX), Mathf.Ceil(height / spriteY));
        }

        private void OnDestroy()
        {
            timerModule = null;
            nightTimeEvent = null;
            dayTimeEvent = null;
            worldGameTimerEvent = null;
        }
    }


    

}

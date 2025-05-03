using System;
using UnityEngine;
using UnityEngine.Events;
using lLCroweTool.Dictionary;
using lLCroweTool.TimerSystem;
using lLCroweTool.Singleton;

namespace lLCroweTool.GameWorldTimeSystem
{
    
    public class GameWorldTimeManager : MonoBehaviourSingleton<GameWorldTimeManager>
    {
        //20240525
        //나중에 타이머이벤트와 어이주기

        //게임내의 월드시간매니저        

        //게임월드상의 이벤트를 미리 구현시켜 작동되게 해주는 중간 매체
        //각각의 이벤트들을 계속찾아주는것보다 미리정의해주고 상호작용할수 있게 해주는 역할을 가짐

        //나온 이유 대화시스템에서 이벤트노드를 어덯게 처리할까 생각중이다가
        //중간중간 만들떄마다 컴포넌트 붙히고 정의하고 코드쓰고 작동되나 체크할려는 걸 좀 편리하게 할려고 제작

        //다른 게임월드이벤트도 여기서 처리하는게 편해보인다.

        //근데 현재 대화쪽만 볼려고 하는거니 좀더 띵킹해보기

        //대화이벤트 게임내에 모든이벤트의 관리를 여기서

        //설계 후 작업       

        //게임월드상의 시간 이벤트를 위한 조건식
        public int yearCondition = 5000;//년 단위의 최대조건
        public int monCondition = 12;//월 단위의 최대조건
        public int dayCondition = 30;//일 단위의 최대조건
        public int hourCondition = 24;//단위의 최대조건
        public int minCondition = 60;//초단위의 최대조건
        public int secCondition = 60;//초단위의 최대조건

        //낮과 밤을 체크해주는 변수관련
        //20210721 나중에하기

        public WorldTime gameWorldTime;//현재 게임월드의 시간이 얼마나 지났는지 체크해주는 함수
        public GameWorldEvent[] gameWorldEventArray = new GameWorldEvent[0];

        [System.Serializable]public class GameWorldEventBible : CustomDictionary<string, GameWorldEvent> { };
        [SerializeField] private GameWorldEventBible gameWorldEventBible = new GameWorldEventBible();//게임시작시 세팅해줌


        protected override void Init()
        {
            //초기 딕셔너리 세팅
            for (int i = 0; i < gameWorldEventArray.Length; i++)
            {
                GameWorldEvent tempEvent = gameWorldEventArray[i];
                gameWorldEventBible.Add(tempEvent.eventName, tempEvent);
            }
        }



        /// <summary>
        /// 월드이벤트 작동을 요청하는 함수
        /// </summary>
        /// <param name="gameWorldEvent">이벤트이름</param>
        public void RequestWolrdEvent(string eventName)
        {
            if (!gameWorldEventBible.ContainsKey(eventName))
            {
                Debug.Log("해당되는 게임월드이벤트가 존재하지않습니다.");
                return;
            }

            GameWorldEvent gameWorldEvent = gameWorldEventBible[eventName];
            //랜덤성체크
            if (gameWorldEvent.isRandom)
            {
                if (lLcroweUtil.ProbabilityCal(gameWorldEvent.randomPercentValue))
                {
                    gameWorldEvent.eventActionCount++;
                    gameWorldEvent.worldTimeValue.ResetTime();
                    gameWorldEvent.unityAction.Invoke();
                }
            }
            else
            {
                gameWorldEvent.eventActionCount++;
                gameWorldEvent.worldTimeValue.ResetTime();
                gameWorldEvent.unityAction.Invoke();
            }
        }

        /// <summary>
        /// 게임월드 이벤트들을 업데이트하여 체크해주는 함수
        /// 초당 작동됨.이벤트도 초당돌아가는걸로 집어넣기
        /// </summary>
        private void UpdateGameWorldEvent()
        {
            //게임월드시간 카운트
            gameWorldTime.CountTime();

            //등록된 이벤트들
            foreach (var gameWorldEvent in gameWorldEventBible)
            {
                //게임이벤트타입에 따른 월드타이머 처리
                switch (gameWorldEvent.Value.eventActionType)
                {
                    case GameWorldEventActionType.Fix:
                        //고정적
                        //해당이벤트는 특정 시간에만 작동되고 더이상 작동은 안됨
                        //아무행동안함

                        break;
                    case GameWorldEventActionType.Periodic:
                        //주기적
                        //해당이벤트는 주기적인 시간에 계속동작하며 해당                       

                        //해당이벤트의 주기적조건에 맞으면 작동후
                        //초기화
                        //이벤트 시간카운트
                        gameWorldEvent.Value.worldTimeValue.CountTime();

                        //이벤트들 조건 체크
                        if (gameWorldEvent.Value.worldTimeCondition.CheckEqualTime(gameWorldEvent.Value.worldTimeValue))
                        {
                            RequestWolrdEvent(gameWorldEvent.Value.eventName);
                        }
                        break;
                }
            }
        }
    }

    /// <summary>
    /// 고정이벤트인지 주기적이벤트인지
    /// </summary>
    public enum GameWorldEventActionType
    {
        Fix,        //고정형
        Periodic    //주기형
    }

    /// <summary>
    /// 게임월드 이벤트 조건들
    /// </summary>
    [Serializable]
    public class GameWorldEvent
    {   
        public int eventActionCount;//게임월드이벤트 작동횟수
        public string eventName;//게임월드이벤트이름

        public GameWorldEventActionType eventActionType;        

        public bool isRandom = false;//랜덤작동여부
        public int randomPercentValue;//랜덤작동 확률

        public WorldTime worldTimeCondition;//이벤트가 작동될 시간조건
        public WorldTime worldTimeValue;//체크해주는 시간

        public UnityAction unityAction;//작동될 이벤트 연동할것
    }

    /// <summary>
    /// 게임 월드 타이머
    /// </summary>
    [Serializable]
    public struct WorldTime
    {
        [SerializeField] private int year;
        [SerializeField] private int mon;
        [SerializeField] private int day;
        [SerializeField] private int hour;
        [SerializeField] private int min;
        [SerializeField] private int sec;

        //6개
        public int Year { get => year;  }
        public int Mon { get => mon;  }
        public int Day { get => day;  }
        public int Hour { get => hour;  }
        public int Min { get => min;  }
        public int Sec { get => sec; }

        public void SetWorldTime(int secValue)
        {   
            ResetTime();

            double quotient = secValue;//몫
            double remainder = 0;///나머지

            //초
            CheckTimeValueCheck(out quotient, out remainder, quotient, GameWorldTimeManager.Instance.secCondition);
            sec = (int)remainder;
            if (quotient <= 0) 
            {
                return;
            }

            //분            
            CheckTimeValueCheck(out quotient, out remainder, quotient, GameWorldTimeManager.Instance.minCondition);            
            min = (int)remainder;
            if (quotient <= 0)
            {
                return;
            }

            //시간            
            CheckTimeValueCheck(out quotient, out remainder, quotient, GameWorldTimeManager.Instance.hourCondition);            
            hour = (int)remainder;
            if (quotient <= 0)
            {
                return;
            }

            //일            
            CheckTimeValueCheck(out quotient, out remainder, quotient, GameWorldTimeManager.Instance.dayCondition);            
            day = (int)remainder;
            if (quotient <= 0)
            {
                return;
            }

            //월            
            CheckTimeValueCheck(out quotient, out remainder, quotient, GameWorldTimeManager.Instance.monCondition);            
            mon = (int)remainder;
            if (quotient <= 0)
            {
                return;
            }

            //년            
            CheckTimeValueCheck(out quotient, out remainder , quotient, GameWorldTimeManager.Instance.yearCondition);
            year = (int)remainder;
            if (quotient <= 0)
            {
                return;
            }

            //에러발생
            Debug.Log("year out 에러");
            return;
        }

        private void CheckTimeValueCheck(out double quotient, out double remainder, double value, double timeCondition)
        {
            quotient = Math.Truncate((double)(value / timeCondition));//몫
            remainder = value % timeCondition;//나머지
        }

        private int CheckLimitValue(int value, int limitValue)
        {
            return value > limitValue ? limitValue : value;
        }
       
        public void SetWorldTime(int _year, int _mon, int _hour, int _min, int _sec)
        {
            year = CheckLimitValue(_year, GameWorldTimeManager.Instance.yearCondition);
            mon = CheckLimitValue(_mon, GameWorldTimeManager.Instance.monCondition);
            hour = CheckLimitValue(_hour, GameWorldTimeManager.Instance.hourCondition);
            min = CheckLimitValue(_min, GameWorldTimeManager.Instance.minCondition);
            sec = CheckLimitValue(_sec, GameWorldTimeManager.Instance.secCondition);
        }

        public string GetWorldTime()
        {
            return lLcroweUtil.GetCombineString(Year.ToString(), " : ", Mon.ToString(), " : ", Day.ToString(), " : ", Hour.ToString(), " : ", Sec.ToString());
        }

        public void ResetTime()
        {
            year = 0;
            mon = 0;
            hour = 0;
            min = 0;
            sec = 0;
        }
        public void CountTime()
        {
            sec++;

            //초
            if (sec >= GameWorldTimeManager.Instance.secCondition)
            {
                sec = 0;
                min++;

                //분
                if (min >= GameWorldTimeManager.Instance.minCondition)
                {
                    min = 0;
                    hour++;

                    //시간
                    if (hour >= GameWorldTimeManager.Instance.hourCondition)
                    {
                        hour = 0;
                        day++;

                        //일
                        if (day >= GameWorldTimeManager.Instance.dayCondition)
                        {
                            day = 0;
                            mon++;

                            //달
                            if (mon >= GameWorldTimeManager.Instance.monCondition)
                            {
                                mon = 0;
                                year++;

                                //년 달
                                if (year >= GameWorldTimeManager.Instance.yearCondition)
                                {
                                    //year = 0;//년이 최대이면 더이상의 진행은 없음
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 월드타이머가 같은지 체크해주는 함수
        /// </summary>
        /// <param name="worldTimeA">체크해줄 월드타이머A</param>
        /// <param name="worldTimeB">체크해줄 월드타이머B</param>
        /// <returns>같은지 여부</returns>
        public bool CheckEqualTime(WorldTime targetWorldTime)
        {
            bool isDone = true;
            if (Year != targetWorldTime.Year)
            {
                isDone = false;
            }
            else if (Mon != targetWorldTime.Mon)
            {
                isDone = false;
            }
            else if (Day != targetWorldTime.Day)
            {
                isDone = false;
            }
            else if (Hour != targetWorldTime.Hour)
            {
                isDone = false;
            }
            else if (Min != targetWorldTime.Min)
            {
                isDone = false;
            }
            else if (Sec != targetWorldTime.Sec)
            {
                isDone = false;
            }
            return isDone;
        }

    }
}
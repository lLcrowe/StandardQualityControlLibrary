//using UnityEngine;
//using System.Collections;
//using UnityEngine.Events;

//namespace lLCroweTool
//{
//    public class MissionInterectObject : ConstraintsCondition
//    {
//        //interectObjectModule을 사용하여 작업
//        //인터렉티브레이어에서 작동함
//        //콜라이더 트리거로 작동
//        //
//        [Header("다음 미션을 향한 미션상호작용 오브젝트")]
//        public MissionInterectObject[] nextMissionObject;

//        [Header("현재 미션상호작용에 지정된 미션 컴퍼스")]
//        [SerializeField]private MissionCompass curTargetMissionCompass;
//        [Header("타이머 이용")]
//        //타이머
//        public bool isUseTimer;
//        public int timer;
//        private int time;
//        public CoroutineTimerModule timerModule;

//        public bool isUseEnter;        
//        public bool isExitEnter;

//        public UnityEvent missionEvent;//유니티 이벤트에  SetNewMission함수를 집어넣는다
//        private EnergyBar targetBar;

//        private void OnTriggerEnter2D(Collider2D collision)
//        {
//            if (isUseTimer)
//            {
//                if (lLcroweUtil.FitConditionAll(collision.gameObject, useLayerCondition, interectLayers, useTagCondition, interectTags))
//                {
//                    targetBar = MissionInterectObject_UI.Instance.RespawntimerBar();
//                    targetBar.transform.position = Camera.main.WorldToScreenPoint(transform.position);
//                    targetBar.SetValueMin(0);
//                    time = 0;
//                    targetBar.SetValueCurrent(time);
//                    targetBar.SetValueMax(timer);
//                    targetBar.gameObject.SetActive(true);
//                    timerModule.enabled = true;
//                    return;
//                }
//            }
//            if (!isUseEnter) return;            
//            if (lLcroweUtil.FitConditionAll(collision.gameObject, useLayerCondition, interectLayers, useTagCondition, interectTags))
//            {
//                MissionClear();
//            }
//        }

//        //코루틴타이머에 집어넣을것 이벤트함수
//        public void TimerCheck()
//        {
//            time++;
//            targetBar.SetValueCurrent(time);
//            if (time >= timer)
//            {
//                MissionClear();                
//                //targetBar.gameObject.SetActive(false);
//                //targetBar = null;
//                //timerModule.enabled = false;
//            }
//        }

//        private void OnTriggerExit2D(Collider2D collision)
//        {
//            if (isUseTimer)
//            {
//                if (lLcroweUtil.FitConditionAll(collision.gameObject, useLayerCondition, interectLayers, useTagCondition, interectTags))
//                {                    
//                    targetBar.gameObject.SetActive(false);
//                    targetBar = null;
//                    timerModule.enabled = false;
//                    return;
//                }
//            }
//            if (!isExitEnter) return;
//            if (lLcroweUtil.FitConditionAll(collision.gameObject, useLayerCondition, interectLayers, useTagCondition, interectTags))
//            {
//                MissionClear();
//            }
//        }

//        //미션 클리어
//        public void MissionClear()
//        {
//            //현재 지정된 미션컴퍼스를 숨기고 미션이벤트를 진행
//            if (!ReferenceEquals(curTargetMissionCompass, null))
//            {
//                curTargetMissionCompass.Hide();
//            }
//            gameObject.SetActive(false);
//            missionEvent.Invoke();

//            //새로운 미션컴퍼스갱신
//            //새로운 미션이 있을 지역 표시

//            for (int i = 0; i < nextMissionObject.Length; i++)
//            {
//                //다음미션오브젝트의 현재 작동되는 타겟미션컴퍼스를 세팅해준다.
//                //다음미션 진행해 사용할 미션컴퍼스 세팅
//                nextMissionObject[i].curTargetMissionCompass = MissionCompassManager.Instance.InitializeMission(nextMissionObject[i].transform);
//                nextMissionObject[i].gameObject.SetActive(true);
//            }
//            //퀘스트시스템이랑 연동구간

//        }
//    }

//}



#if Sensor
using UnityEngine;
using Micosmo.SensorToolkit;
using lLCroweTool.TimerSystem;

namespace lLCroweTool
{
    /// <summary>
    /// 다른모듈을 사용하여 트랩을 만들떄 쓰는 모듈
    /// </summary>
    [RequireComponent(typeof(CoolTimerModule))]
    public class TrapObject_Base : MonoBehaviour
    {
        //다른모듈=>무기모듈 버프상호작용오브젝트
        public ConstraintsCondition constraintsCondition = new ConstraintsCondition();

        /// <summary>쿨타이머</summary>
        private CoolTimerModule coolTimer;

        [Header("트랩 오브젝트 세팅")]
        [Tooltip("딜레이를 사용하는가")]
        ///<summary>딜레이를 사용하는가</summary>
        public bool isUseDelay;        

        [Tooltip("몇초뒤에 작동되는가")]
        ///<summary>몇초뒤에 작동되는가</summary>
        public float delayTime;
       
        
        [Tooltip("몇개가 들어오면 작동될것인가 체크")]
        /// <summary>몇개가 들어오면 작동될것인가 체크 콜라이더들</summary>
        public int colliderCount = 1;
        /// <summary>현재 들어온 콜라이더들</summary>
        [SerializeField] private int curColliderCount = 0;

        [Header("웨폰모듈 트랩의 유닛타입 조건")]
        public bool isUseTrapUnitCondition = false;
        //public UnitType targetUnitType = UnitType.Bio;

        public Sensor sensor;//탐지용 센서

        private void Awake()
        {
            coolTimer = GetComponent<CoolTimerModule>();
            sensor = GetComponent<Sensor>();
            sensor.OnDetected.AddListener(ActionTrap);
            //sensor.OnLostDetection.AddListener(ActionTrap);
        }

        /// <summary>
        /// 트랩오브젝트 작동 함수
        /// </summary>
        /// <param name="_gameObject">찾은 게임오브젝트</param>
        /// <param name="_sensor">찾은 센서</param>
        public void ActionTrap(GameObject _gameObject, Sensor _sensor)
        {
            if (_gameObject.FitConditionAll(constraintsCondition))
            {
                //if (col2d.TryGetComponent(out UnitStatusModule unitAbility))
                //{
                //    if (isUseTrapUnitCondition)
                //    {
                //        if (unitAbility.unitStatusData.worldUnitType == targetUnitType)
                //        {

                //            curColliderCount++;
                //            if (curColliderCount >= colliderCount)
                //            {
                //                coolTimer.coolTimerModule.StartSkill();
                //            }
                //        }
                //    }
                //    else
                //    {
                //        curColliderCount++;
                //        if (curColliderCount >= colliderCount)
                //        {
                //            coolTimer.coolTimerModule.StartSkill();
                //        }
                //    }
                //}
            }
        }

        public void OnDestroy()
        {
            coolTimer = null;
            sensor = null;
        }
    }
}
#endif

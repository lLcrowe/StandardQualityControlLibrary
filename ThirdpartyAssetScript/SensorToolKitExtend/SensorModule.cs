#if Sensor
using Micosmo.SensorToolkit;
using System.Collections.Generic;
using UnityEngine;

namespace lLCroweTool.SensorExtend
{   
    /// <summary>
    /// 작동하는 함수
    /// </summary>
    /// <typeparam name="T">Component</typeparam>
    /// <param name="component">컴포넌트</param>
    public delegate void ActionFunc<T>(T component) where T : Component;

    /// <summary>
    /// 센서 감지모듈
    /// </summary>
    /// <typeparam name="T">Component</typeparam>
    public class SensorModule<T> where T : Component
    {
        //20240427
        //센서툴킷을 설정을 특정값으로 변경하고 쓰길 권장        
        //PulseMode == FixedInternal
        //UpdateFunction == FixedUpdate
        //Pulse Interval == 맘대루

        private Sensor sensor;
        private List<T> detectList = new List<T>(10);

        public Sensor Sensor { get => sensor; }

        public void Init(MonoBehaviour behaviour)
        {
            behaviour.TryGetComponent(out sensor);
            detectList.Clear();
        }

        /// <summary>
        /// 센서모듈에게 대리로 작동시켜주는 함수
        /// </summary>
        /// <param name="actionFunc">작동 함수</param>
        public void Action(ActionFunc<T> actionFunc)
        {
            if (actionFunc == null)
            {
                return;
            }            

            //현재 탐색된 컴포넌트 가져온후&작동
            detectList = Sensor.GetDetectedComponentsByDistance(detectList);
            for (int i = 0; i < detectList.Count; i++)
            {
                var detect = detectList[i];
                actionFunc(detect);
            }
        }
    }
}
#endif
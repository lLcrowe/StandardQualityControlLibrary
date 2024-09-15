#if Sensor
using lLCroweTool;
using lLCroweTool.PathFinder;
using lLCroweTool.TimerSystem;
using Micosmo.SensorToolkit;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class SonudInfo
    {
        //사운드가 발생시 크게 들리게 하는 작업
        public AudioClip audioClip;
        public float soundDistance;
    }

    public class SoundPathDetectModule : MonoBehaviour
    {
        //사운드 모듈을 어덯게 처리할건지 체크
        //사운즈가 온됫을시와 사라질때 를 체크하는건가


        //소리를 들을떄 길찾기를 사용하여 소리의 감쇄를 통해 들음
        //근데 저게 있지않았나
        //스팀체크//이거 만들필요가?


        //20240403 아짜리님과 이야기하면서 나온 내용
        //소리를 감지하고 체크
        //기억하다가 지움

        //빛(광학), 음향(음향공학) ,
        //흉내만 낸다면 만들어도되고
        //리얼하게 한다면

        //가져다 쓸까 Steam Audio //주말에 체크해보자
        //flat 사운드클립

        //사운드가 달라지면 해상도가 달라짐

        public float soundDetectDistance;
        public float pathDistance;
        
        public float offset;

        public TimerModule_Element timer;
        public Sensor sensor;
        public List<SoundPathDetectModule> soundDetectModuleList = new();
        private IAstarAI astarAI;
        protected OnPathDelegate onPathComplete;

        public IAstarAI AstarAI { get => astarAI; }

        private void Awake()
        {
            timer.SetTimer(0.2f);
            sensor = GetComponent<Sensor>();
            astarAI = GetComponent<IAstarAI>();
        }
        
        void Update()
        {
            if (!timer.CheckTimer())
            {
                return;
            }

            soundDetectModuleList = sensor.GetDetectedComponentsByDistance(soundDetectModuleList);

            for (int i = 0; i < soundDetectModuleList.Count; i++)
            {
                var detect = soundDetectModuleList[i];

                astarAI.CheckIsListenPath(soundDetectDistance);
            }
        }

        IEnumerator Test(Vector3 start, Vector3 end)
        {
            var path = ABPath.Construct(transform.position, transform.position + transform.forward * 10, onPathComplete);
            // Wait... (may take some time depending on how complex the path is)
            // The rest of the game will continue to run while waiting
            yield return StartCoroutine(path.WaitForPath());
            // The path is calculated now

        }

        private void OnDrawGizmos()
        {
            ////상대방사운드 나오는 거리
            //Gizmos.color = Color.blue;
            //Gizmos.DrawWireSphere(start.position, soundDetectDistance);

            ////현재듣는거리
            //Gizmos.color = Color.red;
            //Gizmos.DrawWireSphere(start.position, pathDistance);//동일한디

            //Gizmos.color = Color.yellow;
            //Gizmos.DrawWireSphere(start.position, debugCheck);
        }
    }
}
#endif
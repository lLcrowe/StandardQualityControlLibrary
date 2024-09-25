#if MEC
using MEC;
#endif

using UnityEngine;
using System.Collections.Generic;
using lLCroweTool.TimerSystem;
using lLCroweTool.ObjectPool;

namespace lLCroweTool.RespawnSystem
{
    [RequireComponent(typeof(CoolTimerModule))]
    public class RespawnModule : MonoBehaviour
    {
        //리스폰 시스템
        //리스폰장소에서 원하는 무언가를 총체적으로 스폰시켜서 관리해주는 클래스
        //현 시스템을 사용해 지역 전체에 리스폰해주는 무언가를 만들수 있음.
        //리스폰이 될 위치 리스트들

        //20220709//신규기능 수정 예정

        //20230113//책정리하다가 나온정보 메모
        //몬스터위치정보
        //맵번호, 지명이름, 몬스터이름, 레벨, 좌표값
        //몬스터리젠 주기
        //전체몬스터 최대수


        //20240925
        //구조변경//기능정리
        //스폰


     





        [Header("위치")]        
        public Transform[] targetRespawnPositionArray = new Transform[0];//위치들//업방향 쪽으로 리스폰
        public TimerModule_Element timer;
        public bool isRot;

        public int spawnAmount = 1;//스폰수량
        System.Func<Vector3> randomPosAction;
        
        [Header("스폰관련")]
        public bool isUsePercentSpawn = false;//확률 사용여부
        public int percentValue = 50;//확률

        public RespawnTarget respawnTargetPrefab;
        public bool isRespawnLimit = false;//소환시키는 몹의 숫자에 대한 제한을 하는가//이미나온거는 상관안함
        public int respawnLimitCount = 20;
        public List<RespawnTarget> respawnTargetList = new();

        //리스폰배치처리
        [System.Serializable]
        public class CustomRespawnBatchData
        {
            public int index;
            public TimerModule_Element timer;
            public bool isRot;
        }
        [Header("커스텀")]
        public CustomRespawnBatchData[] customRespawnBatchDataArray = new CustomRespawnBatchData[0];

#if MEC
        //캐싱용
        private CoroutineHandle coroutineHandle;//코루틴캐싱

        private void Awake()
        {
            
            
        }
#else

        private void Update()
        {
            UpdateRespawnModule();
        }
#endif


        private void UpdateRespawnModule()
        {
            if (!timer.CheckTimer())
            {
                return;
            }
            StartRespawn(this);
        }


        /// <summary>
        /// 리스폰시작함수
        /// </summary>
        /// <param name="respawnModule">리스폰모듈</param>
        private static void StartRespawn(RespawnModule respawnModule)
        {
            //쿨타이머모듈에서 사용하는게 좋아보인다
            //오직한번만 소환 다른곳에서 계속소환할수있게 도와줘야함

            //제한스폰인가?
            if (respawnModule.isRespawnLimit)
            {
                //제한량 확인
                if (respawnModule.respawnLimitCount < respawnModule.respawnTargetList.Count)
                {
                    return;
                }
            }

            //수량체크
            if (respawnModule.spawnAmount <= 0)
            {
                return;
            }

            //스폰방식
            switch (respawnModule.spawnType)
            {
                case SpawnType.Single:
                    //싱글//단일위치에서만 나옴
                    Transform single = GetRandomRespawnPos(respawnModule);
                    for (int i = 0; i < respawnModule.spawnAmount; i++)
                    {
                        SpawnObject(respawnModule, single);
                        yield return Timing.WaitForSeconds(respawnModule.spawnIntervalTime);
                    }
                    break;
                case SpawnType.Multiple:
                    //멀티//여러위치에서 나옴
                    for (int i = 0; i < respawnModule.spawnAmount; i++)
                    {
                        Transform multiple = GetRandomRespawnPos(respawnModule);
                        SpawnObject(respawnModule, multiple);
                        yield return Timing.WaitForSeconds(respawnModule.spawnIntervalTime);
                    }
                    break;
            }
        }

        /// <summary>
        /// 리스폰모듈의 스폰확률 확인하는 함수
        /// </summary>
        /// <param name="respawnModule">리스폰모듈</param>
        /// <returns>스폰할것인가?</returns>
        private static bool CheckSpawnPercent(RespawnModule respawnModule)
        {
            bool isDone = true;
            //스폰확률을 활용하는가
            if (respawnModule.isUsePercentSpawn)
            {
                if (!lLcroweUtil.ProbabilityCal(respawnModule.percentValue))
                {
                    isDone = false;
                }
            }
            return isDone;
        }

        /// <summary>
        /// 오브젝트 스폰함수
        /// </summary>
        /// <param name="respawnModule">리스폰모듈</param>
        /// <param name="targetRespawnPos">타겟위치</param>
        private static void SpawnObject(RespawnModule respawnModule, Transform targetRespawnPos)
        {
            //소환확률 체크
            if (CheckSpawnPercent(respawnModule))
            {
                RespawnTarget targetRespawn = ObjectPoolManager.Instance.RequestDynamicComponentObject(respawnModule.respawnTargetPrefab);
                targetRespawn.transform.SetPositionAndRotation(lLcroweUtil.GetRandomCirclePosition(targetRespawnPos, respawnModule.randomSize, respawnModule.isUseRandomCircleSizePosition), targetRespawnPos.rotation);
                targetRespawn.transform.parent = respawnModule.transform.parent;
                targetRespawn.gameObject.SetActive(true);
            }
        }
   
        //랜덤 트랜스폼을 반환하는 함수
        private static Transform GetRandomRespawnPos(RespawnModule respawnModule)
        {
            int index = Random.Range(0, respawnModule.targetRespawnPositionArray.Length);
            return respawnModule.targetRespawnPositionArray[index];
        }
    }
}


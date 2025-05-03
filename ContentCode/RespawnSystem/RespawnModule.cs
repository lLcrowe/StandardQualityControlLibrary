#if MEC
using MEC;
#endif

using UnityEngine;
using System.Collections.Generic;
using lLCroweTool.TimerSystem;
using lLCroweTool.ObjectPool;
using System;

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
        public int curIndex = 0;
        [Header("스폰관련")]
        public TimerModule_Element timer;
        public int spawnAmount = 1;//스폰수량
        public bool isRot;
        System.Func<Vector3> randomPosAction;
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
            public int spawnAmount = 1;//스폰수량
            public bool isRot;            
            public bool isUsePercentSpawn = false;//확률 사용여부
            public int percentValue = 50;//확률
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
            StartRespawn();
        }


        /// <summary>
        /// 리스폰시작함수
        /// </summary>
        /// <param name="respawnModule">리스폰모듈</param>
        private void StartRespawn()
        {
            //쿨타이머모듈에서 사용하는게 좋아보인다
            //오직한번만 소환 다른곳에서 계속소환할수있게 도와줘야함

            //수량체크
            if (spawnAmount <= 0)
            {
                return;
            }

            //제한스폰인가?
            if (isRespawnLimit)
            {
                //제한량 확인
                if (respawnLimitCount < respawnTargetList.Count)
                {
                    return;
                }
            }
          
            //랜덤체크
            if (isUsePercentSpawn)
            {
                if (!lLcroweUtil.ProbabilityCal(percentValue))
                {
                    return;
                }
            }

           


          
        }

        private void Spawn()
        {
            if (!timer.CheckTimer())
            {
                return;
            }

            //스폰방식
            for (int i = 0; i < targetRespawnPositionArray.Length; i++)
            {
                if (CheckCustomBatch(i))
                {
                    continue;
                }
                var spawnPos = targetRespawnPositionArray[i];
                var targetRespawn = ObjectPoolManager.Instance.RequestDynamicComponentObject(respawnTargetPrefab);
                var tr = targetRespawn.transform;
                var pos = tr.position;

                //랜덤포스
                if (randomPosAction != null)
                {
                    var randomPos = randomPosAction.Invoke();
                    pos += randomPos;
                }

                var rot = isRot ? spawnPos.rotation : Quaternion.identity;
                tr.SetPositionAndRotation(pos, rot);
                tr.SetParent(transform.parent);
                targetRespawn.gameObject.SetActive(true);

            }
        }

        private bool CheckCustomBatch(int index)
        {
            var value = false;
            for (int i = 0; i < customRespawnBatchDataArray.Length; i++)
            {
                var target = customRespawnBatchDataArray[i];
                if (target.index == index)
                {
                    value = true;
                    break;
                }
            }
            return value;
        }

        
    }
}


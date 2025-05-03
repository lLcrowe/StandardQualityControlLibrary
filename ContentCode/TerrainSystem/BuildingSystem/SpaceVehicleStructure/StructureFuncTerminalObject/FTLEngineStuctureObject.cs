#if Doozy
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace lLCroweTool.BuildingSystem.FuncStructure.Terminal
{
    public class FTLEngineStuctureObject : StructureFuncBaseObject
    {
        //빌딩베이스를 상속받아 기초적인
        //각각시설물의 능력이 발동되도록 작동

        //엔진
        //애매하다
        //제네레이터 = 전기생산
        //노즐 = 출력
        //엔진 = ??
        //public float enginePower;
        //public float engineSpeed;

        public int maxPreheat;//예열 최대 수치
        public bool isDone;//충분한가 체크
        public int curPreheat = 0;//예열 현재 수치
        public int increasePreheat = 1;//예열 증가수치
        public int decreasePreheat = 1;//예열 감소수치
        public float startFTLCountTime = 5f;//5초후 //FTL 엔진작동 시간
        private float timer;
        private bool isReadyStartFTL;//FTL 엔진이 준비됫는지 체크용도

        protected override void AwakeInitStructure()
        {
            base.AwakeInitStructure();
            isUseFuncUpdate = true;
        }

        protected override void UpdateStructureAction()
        {
            if (!GetIsActive())
            {
                curPreheat -= decreasePreheat;
                if (curPreheat < 0)
                {
                    curPreheat = 0;
                }
                return;
            }

            //히트체크
            curPreheat += increasePreheat;
            if (maxPreheat < curPreheat)
            {
                curPreheat = maxPreheat;
            }

            //예열 수치체크
            if (maxPreheat - 5 < curPreheat)
            {
                isDone = true;
            }
            else
            {
                isDone = false;
            }

            //가능한지 체크
            if (isDone)
            {
                FTLEngineCount();
            }
        }

        public void FTLEngineCount()
        {
            timer-=0.01f;
            if (timer < 0)
            {
                isReadyStartFTL = true;
            }
        }

        public void FTLEnineReset()
        {
            timer = startFTLCountTime;
            isReadyStartFTL = false;
            curPreheat = 0;
            isDone = false;
        }

        public bool GetReadyToFTLEngine()
        {
            return isReadyStartFTL;
        }
    }
}

#endif
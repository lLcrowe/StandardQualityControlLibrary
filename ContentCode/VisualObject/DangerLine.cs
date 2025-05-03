#if Shape
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using lLCroweTool.TimerSystem;
using System;

namespace lLCroweTool.Visual.ShapeAsset
{
    [RequireComponent(typeof(UpdateTimerModule))]
    [RequireComponent(typeof(Line))]
    public class DangerLine : MonoBehaviour
    {
        //위험 경로 라인

        //경고선 제작
        UpdateTimerModule timerModule;//타임모듈
        Line dangerLine;//라인랜더러
        [Space]
        [Header("외부에서 바꿀수 있는 변수들")]
        [Space]
        [Header("isDone은 외부에서 확인후 발사할수 있게하는 체크형 변수")]
         public bool isDone;//외부에서 작동이 다끝났는지 체크하는 변수
        [Space]
        public bool isBackWard;//거꾸로 작동되는가
        public float lineWidth = 5;//선폭의 크기
        public float lineCheckWidth = 0.2f;//선폭이 어느정도 되면 멈출것인가
        public float lineSpeed = 1;//선폭이 줄어들거나 늘어나는 속도
        [Space]
        [Header("방향카테고리는 타겟을 정하는 함수를 쓸때 \n 랜지형 파라미터값이 있는것을 사용하면 작동됨")]
        public DirectionCAT directionCAT = DirectionCAT.Up;//탑뷰라 업으로 표시시킴
        [SerializeField] private float width;//폭
        [SerializeField] private Transform startTarget;//에임이 시작하는 위치
        [SerializeField] private Transform aimTarget;//에임이 끝날 위치

        private void Awake()
        {
            //StartSettingDangerLine();
        }

        //public override void CreateObjectInitSetting()
        //{
        //    if (GetIsInit())
        //    {
        //        Debug.Log("이미 초기세팅이 됫습니다. 다시 세팅할려면 리셋시켜주세요");
        //        return;
        //    }
        //    base.CreateObjectInitSetting();


        //    //게임오브젝트생성 시작과 에임
        //    GameObject gameObject = new GameObject();
        //    gameObject.transform.parent = transform;
        //    gameObject.name = "StartAnim";
        //    startTarget = gameObject.transform;

        //    gameObject = new GameObject();
        //    gameObject.transform.parent = transform;
        //    gameObject.name = "EndAnim";
        //    aimTarget = gameObject.transform;
        //}

        //dangerLine의 시작초기화세팅
        public void StartSettingDangerLine()
        {
            dangerLine = GetComponent<Line>();
            timerModule = GetComponent<UpdateTimerModule>();
            timerModule.SetTimer(0.02f);
            timerModule.AddUnityEvent(delegate { UpdateDangerLine(); });
        }

        //작동되는 종류를 나누자
        //시간초로 작동디는 대드라인 timer
        //그냥 표시식 대드라인 onoff

        //[ButtonMethod]
        public void ActionDangerLine()
        {
            dangerLine.enabled = true;
            ActionDangerLineTimer();
            if (isBackWard)
            {
                isDone = false;
                width = 0;
            }
            else
            {
                isDone = false;
                width = lineWidth;
            }
        }
        public void ActionDangerLineTimer()
        {
            timerModule.enabled = true;
        }
        public void ActionDangerLineTimer(bool onOff)
        {
            timerModule.enabled = onOff;
        }


        //업데이트 DangerLine
        //업데이트모듈에 집어넣는다.
        public void UpdateDangerLine()
        {
            dangerLine.Start = startTarget.position;
            dangerLine.End = aimTarget.position;

            if (isBackWard)
            {
                //뭔가 이펙트가 별로다.
                //하지만 포신같은데 사용하면 딱좋겠다
                width += lineSpeed * Time.deltaTime;
                dangerLine.Thickness = width;

                if (width > lineWidth - lineCheckWidth)//체크해봐야함
                {
                    isDone = true;
                    width = 0;
                    timerModule.enabled = false;
                    dangerLine.enabled = false;
                }
            }
            else
            {
                width -= lineSpeed * Time.deltaTime;
                dangerLine.Thickness = width;

                if (width < lineCheckWidth)
                {
                    isDone = true;
                    width = lineWidth;
                    timerModule.enabled = false;
                    dangerLine.enabled = false;
                }
            }
          
        }

        //라인의 위치를 세팅하는 함수
        //외부에서 사용하는 함수
        //에임이 시작하는 위치, 에임이 끝나는 위치
        //부모였다 아니었다로 판별시키자
        public void SetDangerLineTarget(Transform _startTarget, Transform _aimTarget)
        {
            //부모지정후
            startTarget.parent = _startTarget;
            aimTarget.parent = _aimTarget;

            //위치지정
            startTarget.position = _startTarget.position;
            aimTarget.position = _aimTarget.position;

        }
        //거리로 라인의 위치를 세팅하는함수
        public void SetDangerLineTarget(Transform _startTarget, float range)
        {
            //부모지정 후
            startTarget.parent = _startTarget;
            aimTarget.parent = transform;

            //위치지정
            startTarget.position = _startTarget.position;
            //방향에 따른 위치 변환
            switch (directionCAT)
            {
                case DirectionCAT.Left:
                    aimTarget.position = -_startTarget.right * range;
                    break;
                case DirectionCAT.Right:
                    aimTarget.position = _startTarget.right * range;
                    break;
                case DirectionCAT.Up:
                    aimTarget.position = _startTarget.up * range;
                    break;
                case DirectionCAT.Down:
                    aimTarget.position = -_startTarget.up * range;
                    break;
            }
        }

        public void ResetDangerLineTarget()
        {
            //부모를 현재오브젝트로 지정해버리고
            startTarget.parent = transform;
            aimTarget.parent = transform;

            //위치를 현재 오브젝트 위치로 변경
            startTarget.position = transform.position;
            aimTarget.position = transform.position;
        }

        public enum DirectionCAT
        {
            Left,Right,Up,Down
        }

    }
}

#endif
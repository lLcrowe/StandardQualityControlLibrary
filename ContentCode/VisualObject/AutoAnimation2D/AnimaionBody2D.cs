using System.Collections;
using UnityEngine;

namespace lLCroweTool.AutoAnimation2D
{
    public class AnimaionBody2D : MonoBehaviour
    {
        //몸체에 달아주는 부위
        //바디의 중심을 체크
        //다리의 수는 짝수로 맞출것//무조건//강제
        
        public Vector2 directionPower;//외부에서 해당 방향으로 힘주기//최대 1~-1
        private Vector2 cycleDirection;//한싸이클이 다돌때까지 가지는 방향(힘)
        private AnimationLeg2D[] animationLeg2DArray;//다리오브젝트들//현재컴포넌트가 붙은 오브젝트 안에 동작
        //private int legSetAmount = 0;
        //다리 작동방식
        //UP Move Down => Repeat

        //몸체가 진행방향을 절반을 이동
        //나머지의 다리를 이동하면 절반을 더 이동

        //요약
        //다리의 절반을 움직일시 몸체가 이동함 
        //각각의 다리는 2개가 한세트로 움직임
        //다리의 한세트당 이동속도 절반씩
        //=> 2개의 다리를 가진 개체이면 1의 움직이면 0.5씩 나누어서 이동
        //=> 4개의 다리를 가진 개체이면 1의 움직이면 0.25씩 나누어서 이동
        //언제 이동하는가 한쪽다리의 움직인다음 바닥에 놓았을시 움직임

        //공식
        //(다리 * 2) / 이동속도


        //다리방향은 세팅에 따라 다르지만 업 다운 방식이
        //순방향 0 1 1 0
        //역방향 0 1 0 1

        private int legIndex;//현재 다리번호
        private int legNextIndex;//현재 다리의 다음번호
        //private float maxDistince = 1f;//체크할거리


        private bool isDoneWalkCycle = true;//걷는거의 한사이클이 끝났는가        
        private bool isLegMove = false;//다리움직일차례인가
        private bool isLegDown = false;//다리아래로 내릴 차례인가
        private bool isOtherLeg = false;//반대다리 들이기위한 규칙//한바퀴//다리 업다운 관련
        //public bool[] legUpArray;//다리올리는 규칙//isUp에 따라 반대로 작동

        //작동시간관련
        public float legTimer = 0.5f;
        private float time = 0;
        private bool isRight = false;

        //움직임 관련
        private Vector2 changeBodyPos = Vector2.zero;//바디가 이동할 위치//현재 위치 + 이동할방향 위치
        public float moveSpeed = 1.0f;//이동속도
        private bool isMove = false;//움직이는 중인가 여부

        Vector2 m_Velocity = Vector2.zero;
        [SerializeField] float movementSmoothing = .05f;

        private void Awake()
        {
            animationLeg2DArray = GetComponentsInChildren<AnimationLeg2D>();
            //int index = animationLeg2DArray.Length;
            //legSetAmount = index / 2;
            //legActionArray = new int[index];
            //legUpArray = new bool[index];

            //자동으로 지그재그 세팅하기
            //나올답
            //0 2 4 
            // 1 3 
            //0213 = 4개
            for (int i = 0; i < animationLeg2DArray.Length; i++)
            {
                //if (i == 0)
                //{
                //    legActionArray[i] = i;
                //    legUpArray[i] = false;
                //}
                //else if (i == index - 1)
                //{
                //    legActionArray[i] = i;
                //    legUpArray[i] = true;
                //}
                //else
                //{
                //    if (i % 2 == 1)
                //    {
                //        //홀수이면+1
                //        legActionArray[i] = i + 1;
                //        legUpArray[i] = true;
                //    }
                //    else
                //    {
                //        //짝수이면-1
                //        legActionArray[i] = i - 1;
                //        legUpArray[i] = false;
                //    }
                //}
                animationLeg2DArray[i].SetUnitMoveSpeed(moveSpeed / 2);
            }
            changeBodyPos = transform.position;
            cycleDirection = directionPower;
        }

        // Update is called once per frame
        void Update()
        {
            //시간이 되면
            if (GetLegTime() || !isDoneWalkCycle) 
            {
                //움직일 방향체크 없으면 넘어감
                //한 싸이클이 끝나야지만 작동이 안되게 함
                if (directionPower == Vector2.zero)
                {
                    return;
                }

                //한대의 다리가 걷는사이클 한바퀴가 끝났는지
                if (isDoneWalkCycle)
                {
                    //움직일다리 인덱스체크
                    legIndex++;
                    //짝수인지 체크
                    if (legIndex % 2 == 0)
                    {
                        //짝수이면 다른 다리를 움직이게 작동
                        isOtherLeg = !isOtherLeg;
                    }   
                    //다리싸이클 초기화
                    isDoneWalkCycle = false;
                    isLegMove = false;
                    isLegDown = false;
                }

                //제한점 체크
                if (legIndex >= 2)
                {
                    //모든다리의 한 싸이클이 다 돌았으면
                    legIndex = 0;
                    cycleDirection = directionPower;
                }
                //다음다리의 제한점 체크
                //다리가 여러개인 경우 그에 맞게 더해줘서 움직여야함
                legNextIndex = legIndex + 1;//기본
                if (legNextIndex >= 2)
                {
                    legNextIndex = 0;
                }


                //추가내용
                //다리는 0 1 이렇게 작동되며  0 1 이 하나의 세트라 
                //0 1 이상 +2 를하여 0 2 4 로 각각의 한세트를 잡아둘수 있음
              
                //UP Move Down //원사이클
                if (!isLegMove && !isLegDown)
                {
                    //다리 업순서에 따라 다리를 올림을 작동    
                    //Up                    
                    for (int i = 0; i < animationLeg2DArray.Length; i += 2)
                    {
                        if (isOtherLeg)
                        {
                            animationLeg2DArray[legIndex + i].SetLegUp(true);
                        }
                        else
                        {
                            animationLeg2DArray[legNextIndex + i].SetLegUp(true);
                        }
                    }
                    isLegMove = true;
                }
                else if (isLegMove && !isLegDown)
                {
                    //해당다리가 들어올리는 다리이면 이동
                    //Move
                    for (int i = 0; i < animationLeg2DArray.Length; i += 2)
                    {
                        if (isOtherLeg)
                        {
                            animationLeg2DArray[legIndex + i].SetLegDirection(cycleDirection.normalized);//다리 위치이동
                            animationLeg2DArray[legNextIndex + i].SetLegDirection(-cycleDirection.normalized);//반대쪽다리 위치이동
                        }
                        else
                        {
                            animationLeg2DArray[legIndex + i].SetLegDirection(-cycleDirection.normalized);//다리 위치이동
                            animationLeg2DArray[legNextIndex + i].SetLegDirection(cycleDirection.normalized);//반대쪽다리 위치이동
                        }
                    }
                  

                    isLegDown = true;

                    //이동
                    //다리수의 절반만큼 이동
                    changeBodyPos = (transform.up * (cycleDirection.y * moveSpeed / animationLeg2DArray.Length)) + (transform.right * (cycleDirection.x * moveSpeed / animationLeg2DArray.Length)) + transform.position;
                    isMove = true;
                }
                else if (isLegDown)
                {
                    //Down
                    for (int i = 0; i < animationLeg2DArray.Length; i += 2)
                    {
                        if (isOtherLeg)
                        {
                            animationLeg2DArray[legIndex + i].SetLegUp(false);
                        }
                        else
                        {
                            animationLeg2DArray[legNextIndex + i].SetLegUp(false);
                        }
                    }
                    isDoneWalkCycle = true;
                }



                //원래 내용
                //다리 두개일떄 작동 시키던거였음
                ////UP Move Down //원사이클
                //if (!isLegMove && !isLegDown) 
                //{
                //    //다리 업순서에 따라 다리를 올림을 작동    
                //    //Up                    
                //    if (isOtherLeg)
                //    {
                //        animationLeg2DArray[legIndex].SetLegUp(true);
                //    }
                //    else
                //    {
                //        animationLeg2DArray[legNextIndex].SetLegUp(true);
                //    }

                //    isLegMove = true;
                //}
                //else if (isLegMove && !isLegDown)
                //{
                //    //해당다리가 들어올리는 다리이면 이동
                //    //Move
                //    if (isOtherLeg)
                //    {
                //        animationLeg2DArray[legIndex].SetLegDirection(cycleDirection.normalized);//다리 위치이동
                //        animationLeg2DArray[legNextIndex].SetLegDirection(-cycleDirection.normalized);//반대쪽다리 위치이동
                //    }
                //    else
                //    {
                //        animationLeg2DArray[legIndex].SetLegDirection(-cycleDirection.normalized);//다리 위치이동
                //        animationLeg2DArray[legNextIndex].SetLegDirection(cycleDirection.normalized);//반대쪽다리 위치이동
                //    }

                //    isLegDown = true;

                //    //이동
                //    //다리수의 절반만큼 이동
                //    changeBodyPos = (transform.up * (cycleDirection.y * moveSpeed / animationLeg2DArray.Length)) + (transform.right * (cycleDirection.x * moveSpeed / animationLeg2DArray.Length)) + transform.position;
                //    isMove = true;
                //}
                //else if (isLegDown)
                //{
                //    //Down
                //    if (isOtherLeg)
                //    {
                //        animationLeg2DArray[legIndex].SetLegUp(false);
                //    }
                //    else
                //    {
                //        animationLeg2DArray[legNextIndex].SetLegUp(false);
                //    }

                //    isDoneWalkCycle = true;
                //}

                //해당다리의 거리 체크
                //거리가 너무 멀면 원래위치로 이동
                //if (Vector2.Distance(animationLeg2DArray[legIndex].GetTransform().localPosition, animationLeg2DArray[legIndex].poleLegPos) > maxDistince)
                //{
                //    animationLeg2DArray[legIndex].ResetLegPos();
                //}

            }

            //이동관련
            if (isMove)
            {
                if (lLcroweUtil.CheckDistance((Vector2)transform.position, changeBodyPos, 0.02f))
                {
                    isMove = false;
                }
                else
                {
                    transform.position = Vector2.SmoothDamp(transform.position, changeBodyPos, ref m_Velocity, movementSmoothing);
                }
            }
        }
        private bool GetLegTime()
        {
            isRight = false;
            if (Time.time > legTimer + time)
            {
                isRight = true;
                time = Time.time;
            }
            return isRight;
        }
        private void OnDestroy()
        {
            animationLeg2DArray = null;
        }
    }
}
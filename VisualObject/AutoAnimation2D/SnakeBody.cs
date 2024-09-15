#if MEC
using MEC;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using lLCroweTool.TimerSystem;

namespace lLCroweTool.AutoAnimation2D.SnakeBody
{
    [RequireComponent(typeof(CoroutineTimerModule))]
    public class SnakeBody : MonoBehaviour
    {
        //스네이크바디를 표현하기 위한 스크립트
        //오브젝트를 움직이고 오브젝트를 사용하여 다른 모듈을 추가시켜서 추가적인기능을 노릴수 잇음
        //애니메이션기법에 맞게 Y축만 늘리고 줄이는게 아니라 XY축 스케일을 같이 늘려서 확장 효과를 줄수 있음.


        //20220701
        //현재로는 오브젝트로 사용해서 바디를 표현하는게 맞아보임
        //150개도 괜찮음

        //20220702
        //삼킴표현 스크립트 애니메이션제작
        //맘에 듬//촉수, 뱀류 같은 몬스터에 활용가능

        //20220707
        //편법으로 물결웨이브추가
        //SinWaveObject컴포넌트를 추가하여 웨이브를 줄수 있음
        //파워를 세팅하면 정해진 설정에 맞게 상호작용하여 작동

        //20220720
        //스파인과 연동중인데 Y축이 아닌 X축으로 해야되는 문제가 있다
        //추가설정 => 추가각도, 회전각도 제한
        //



        public float tailSmoothSpeed = 0.002f;//0.002~0.02f 사이//보간스피드
        public float checkDistance = 0.5f;//갭//확인할 거리
        public float addAngle = 90;//추가해서 바꿀 각도
        public float limitAngle = 90;

        //각도제한

        public Transform[] tailObjectArray;//꼬리오브젝트

        //참조체크 속도
        private Vector2[] refBodySpeedArray;
        private Vector2 refBodySpeed;
      

        [Space]
        [Header("원복설정")]
        public float restoreSize = 1f;//원복사이즈//원래사이즈로 돌아가는 값
        public float restoreTime = 0.5f;//원복시간

        //팽창설정을 2초과로 해야지만 삼킨다는 표현을 할수 있는거 같음
        [Space]
        [Header("팽창설정")]
        public float expansionSize = 3f;//팽창사이즈
        public float expansionTime = 0.1f;//팽창시간

        [Space]
        [Header("수축설정")]
        public float contractionSize = 0.7f;//수축사이즈//원래몸통크기보다는 작아야됨
        public float contractionTime = 0.3f;//수축시간

        [Space]
        [Header("꼬리표현설정")]
        public float nextTailActionTime = 0.05f;//다음 꼬리표현까지의 시간//팽창 + 수축 + 원복 시간을 합친것보다 작은걸로 값을 집어넣어야됨//그래야지만삼키는느낌이 좋음
        public int voreTailCount = 10;//삼킴을 표현할 꼬리수//꼬리기준으로 3/1지점까지가 적당함
        public bool isDecreaseGradual = false;//점점감소하는여부
        public float decreaseSizeValue = 0.1f;//감소하는 값//팽창 수축사이즈로 부터 얼만큼씩 증감할지 체크

        //컴포넌트
        private CoroutineTimerModule coroutineTimer;

        private void Awake()
        {
            //rb2d = GetComponent<Rigidbody2D>();
            //rb2d.gravityScale = 0;

            coroutineTimer = GetComponent<CoroutineTimerModule>();
            coroutineTimer.SetTimer(0.02f);
            coroutineTimer.AddUnityEvent(delegate { SnakeBodyMovement(this); } );
            

            for (int i = 0; i < tailObjectArray.Length; i++)
            {
                tailObjectArray[i].parent = null;
                //tailObjectArray[i].gameObject.hideFlags = HideFlags.HideInHierarchy;
            }
            refBodySpeedArray = new Vector2[tailObjectArray.Length];
        }

        /// <summary>
        /// 스네이크바디 움직임
        /// </summary>
        /// <param name="snakeBody">타겟팅할 스네이크 바디</param>
        private static void SnakeBodyMovement(SnakeBody snakeBody)
        {
            //기준오브젝트와 첫번째 꼬리 거리체크
            if (lLcroweUtil.CheckDistance(snakeBody.transform.position, snakeBody.tailObjectArray[0].position, snakeBody.checkDistance))//연산떄문에 추가시킴
            {
                return;
            }

            //머리와 첫번째 꼬리 움직임
            //MoveTail(transform, tailObjectList[0]);
            MoveTailPos(snakeBody.transform, snakeBody.tailObjectArray[0], ref snakeBody.refBodySpeed, snakeBody);
            snakeBody.tailObjectArray[0].rotation = lLcroweUtil.GetRotation(snakeBody.tailObjectArray[0], snakeBody.transform, snakeBody.limitAngle, snakeBody.addAngle);
            //TestWave(snakeBody.tailObjectArray[0], snakeBody.transform, snakeBody, 1);




            //꼬리오브젝트 거리체크
            if (snakeBody.tailObjectArray.Length > 1)
            {
                for (int i = 1; i < snakeBody.tailObjectArray.Length; i++)
                {
                    if (!lLcroweUtil.CheckDistance(snakeBody.tailObjectArray[i - 1].position, snakeBody.tailObjectArray[i].position, snakeBody.checkDistance))//연산떄문에 추가시킴
                    {
                        //MoveTail(tailObjectList[i - 1], tailObjectList[i]);                        
                        MoveTailPos(snakeBody.tailObjectArray[i - 1], snakeBody.tailObjectArray[i], ref snakeBody.refBodySpeedArray[i], snakeBody);
                        snakeBody.tailObjectArray[i].rotation = lLcroweUtil.GetRotation(snakeBody.tailObjectArray[i], snakeBody.tailObjectArray[i - 1], snakeBody.limitAngle, snakeBody.addAngle);

                        //int index = i;
                        //TestWave(snakeBody.tailObjectArray[i], snakeBody.tailObjectArray[i - 1], snakeBody, index);

                        //float y = snakeBody.amplitude * Mathf.Sin(snakeBody.frequency * Time.time * snakeBody.tailSmoothSpeed) - i;
                        //snakeBody.tailObjectArray[i].position = snakeBody.tailObjectArray[i - 1].right + new Vector3(Mathf.Sin(Time.time), 0.0f, 0.0f);


                        //snakeBody.tailObjectArray[i].position += snakeBody.tailObjectArray[i - 1].up * y;
                    }
                }
            }
        }

        //private static void TestWave(Transform targetObject, Transform frontObject, SnakeBody snakeBody, int index)
        //{
        //    if (snakeBody.isWave)
        //    {
        //        if (index % 2 ==0)
        //        {
        //            targetObject.position += frontObject.up * lLcroweUtil.SinWave(snakeBody.amplitude, snakeBody.frequency, index) * Time.deltaTime;
        //            //targetObject.position += frontObject.right * lLcroweUtil.SinWave(snakeBody.amplitude, snakeBody.frequency, index) * Time.deltaTime;
        //        }
        //    }
        //}

        /// <summary>
        /// 꼬리 움직임 함수
        /// </summary>
        /// <param name="targetPointObject">타겟팅될 기준 오브젝트</param>
        /// <param name="targetTail">꼬리가 될 오브젝트</param>
        /// <param name="refVector">참조할 속도값</param>
        /// <param name="snakeBody">스네이크 바디</param>
        /// <param name="isWave">웨이브 작동여부</param>
        /// <param name="addAngle">추가할 각도</param>
        public static void MoveTail(Transform targetPointObject, Transform targetTail, ref Vector2 refVector, SnakeBody snakeBody, float addAngle)
        {
            //방향체크=> 현재 위치 + 특정방향으로의 고정1거리 * 체크할거리
            Vector3 targetPos = targetPointObject.position + (targetTail.position - targetPointObject.position).normalized * snakeBody.checkDistance;
            lLcroweUtil.MoveSmoothDamp(targetTail, targetPos, ref refVector, snakeBody.tailSmoothSpeed);//이동
            targetTail.rotation = lLcroweUtil.GetRotation(targetTail.position, targetPointObject.position, addAngle);//회전
        }

        public static void MoveTailPos(Transform targetPointObject, Transform targetTail, ref Vector2 refVector, SnakeBody snakeBody)
        {
            //방향체크=> 현재 위치 + 특정방향으로의 고정1거리 * 체크할거리
            Vector3 targetPos = targetPointObject.position + (targetTail.position - targetPointObject.position).normalized * snakeBody.checkDistance;
            lLcroweUtil.MoveSmoothDamp(targetTail, targetPos, ref refVector, snakeBody.tailSmoothSpeed);//이동
        }


        /// <summary>
        /// 스네이크바디의 삼킴 동작표현하는 함수
        /// </summary>
        /// <param name="snakeBody">타겟팅할 스네이크 바디</param>
        public static IEnumerator<float> ActionVore(SnakeBody snakeBody)
        {
            //삼킴을 표현하는 동작
            //율동을 줘야됨//꼬리부분에 대한 사이즈를 전체적으로 커지게하거나 Y축만 커지게 하는 방법
            //Y같은경우 애니메이션기법에 위배되는느낌이라 안함
            //삼킴에 대한 표현을 꼬리 몇번째부분까지할지 에 대한 체크
            //먹었을시 부풀어 오르니까//팽창이 빠르고 수축이 느림

            int tempValue = snakeBody.voreTailCount < snakeBody.tailObjectArray.Length ? snakeBody.voreTailCount : snakeBody.tailObjectArray.Length;

            for (int i = 0; i < tempValue; i++)
            {
                Transform targetTail = snakeBody.tailObjectArray[i];
                int index = i;
                Timing.RunCoroutine(VoreEffect(snakeBody, targetTail, index));
                yield return Timing.WaitForSeconds(snakeBody.nextTailActionTime);
            }
        }

        /// <summary>
        /// 삼킴효과 표현함수
        /// </summary>
        /// <param name="snakeBody">타겟팅할 스네이크 바디</param>
        /// <param name="targetTail">타겟팅할 스네이크 꼬리</param>
        private static IEnumerator<float> VoreEffect(SnakeBody snakeBody, Transform targetTail, int count)
        {
            //삼킨표현
            float value = CalValue(snakeBody, snakeBody.expansionSize, count, true);
            targetTail.DOScale(value, snakeBody.expansionTime);//팽창
            yield return Timing.WaitForSeconds(snakeBody.expansionTime);

            value = CalValue(snakeBody, snakeBody.contractionSize, count, false);
            targetTail.DOScale(value, snakeBody.contractionTime);//수축
            yield return Timing.WaitForSeconds(snakeBody.contractionTime);

            targetTail.DOScale(snakeBody.restoreSize, snakeBody.restoreTime);//원복
        }

        /// <summary>
        /// 증감 작동시 최대치 체크후 돌려주는 함수
        /// </summary>
        /// <param name="snakeBody">스네이크 바디</param>
        /// <param name="targetValue">타겟팅 값</param>
        /// <param name="count">인덱스번호</param>
        /// <param name="isSizeUp">1기준으로 커져야되는 단계 여부</param>
        private static float CalValue(SnakeBody snakeBody, float targetValue, int count, bool isSizeUp)
        {
            if (snakeBody.isDecreaseGradual)
            {
                //1기준으로 커지는 단계인가 작은단계인가 여부
                if (isSizeUp)
                {
                    //커지는단계이니 출력이 작아져야함//최종은 원복사이즈보다커야됨
                    targetValue -= count * snakeBody.decreaseSizeValue;

                    targetValue = snakeBody.restoreSize > targetValue ? snakeBody.restoreSize : targetValue;

                    //if (snakeBody.restoreSize > targetValue)
                    //{
                    //    targetValue = snakeBody.restoreSize;
                    //}
                }
                else
                {
                    //작아지는단계이니 출력이 커져야함//최종은 원복사이즈보다 작아야됨
                    targetValue += count * snakeBody.decreaseSizeValue;

                    targetValue = snakeBody.restoreSize < targetValue ? snakeBody.restoreSize : targetValue;

                    //if (snakeBody.restoreSize < targetValue)
                    //{
                    //    targetValue = snakeBody.restoreSize;
                    //}
                }
            }
            return targetValue;
        }

        public void TestVore()
        {
            Timing.RunCoroutine(SnakeBody.ActionVore(this));
        }
    }
}
#endif
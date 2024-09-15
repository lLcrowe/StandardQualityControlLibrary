#if MEC && Shape
using UnityEngine;
using Shapes;
using lLCroweTool.TimerSystem;

namespace lLCroweTool.AutoAnimation2D
{
    [RequireComponent(typeof(CoroutineTimerModule))]
    public class SnakeBodyPolyLine : MonoBehaviour
    {
        //스네이크바디
        //폴리라인으로만 구현된것

        public float tailSmoothSpeed = 0.002f;//0.002~0.02f 사이//보간스피드
        public float checkDistance = 0.5f;//갭//확인할 거리
        public int tailCount = 5;//꼬리 수량
        public Polyline snakeBodyLine;
        private Vector2[] refBodySpeedArray;
        private Vector2 refBodySpeed;

        public CoroutineTimerModule coroutineTimer;


        private void Awake()
        {
            //rb2d = GetComponent<Rigidbody2D>();
            //rb2d.gravityScale = 0;

            snakeBodyLine.transform.parent = null;
            snakeBodyLine.transform.position = Vector2.zero;

            for (int i = 0; i < tailCount; i++)
            {
                snakeBodyLine.AddPoint(transform.position);
            }

            coroutineTimer = GetComponent<CoroutineTimerModule>();
            coroutineTimer.SetTimer(0.02f);
            coroutineTimer.AddUnityEvent(delegate { SnakeBodyPolyLineMovement(this); });
           
            refBodySpeedArray = new Vector2[snakeBodyLine.points.Count];
        }

        /// <summary>
        /// 스네이크바디 움직임
        /// </summary>
        /// <param name="snakeBodyPolyLine">타겟팅할 스네이크 바디폴리라인</param>
        private static void SnakeBodyPolyLineMovement(SnakeBodyPolyLine snakeBodyPolyLine)
        {
            //기준오브젝트와 첫번째 꼬리 거리체크
            if (lLcroweUtil.CheckDistance(snakeBodyPolyLine.transform.position, snakeBodyPolyLine.snakeBodyLine.points[0].point, snakeBodyPolyLine.checkDistance))
            {
                return;
            }
            snakeBodyPolyLine.snakeBodyLine.SetPointPosition(0, MoveTailPolyLine(snakeBodyPolyLine.snakeBodyLine.points[0].point, snakeBodyPolyLine.transform.position, ref snakeBodyPolyLine.refBodySpeed, snakeBodyPolyLine));

            if (snakeBodyPolyLine.snakeBodyLine.Count > 1)
            {
                for (int i = 1; i < snakeBodyPolyLine.snakeBodyLine.Count; i++)
                {
                    if (!lLcroweUtil.CheckDistance(snakeBodyPolyLine.snakeBodyLine.points[i - 1].point, snakeBodyPolyLine.snakeBodyLine.points[i].point, snakeBodyPolyLine.checkDistance))
                    {
                        snakeBodyPolyLine.snakeBodyLine.SetPointPosition(i, MoveTailPolyLine(snakeBodyPolyLine.snakeBodyLine.points[i].point, snakeBodyPolyLine.snakeBodyLine.points[i - 1].point, ref snakeBodyPolyLine.refBodySpeedArray[i], snakeBodyPolyLine));
                    }
                }
            }
        }

        public static Vector3 MoveTailPolyLine(Vector3 targetPointObject, Vector3 targetTail, ref Vector2 refVector, SnakeBodyPolyLine snakeBodyPolyLine)
        {
            Vector3 targetPos = targetPointObject + (targetTail - targetPointObject).normalized * snakeBodyPolyLine.checkDistance;//방향체크
            return Vector2.SmoothDamp(targetTail, targetPos, ref refVector, snakeBodyPolyLine.tailSmoothSpeed);
        }


    }
}
#endif
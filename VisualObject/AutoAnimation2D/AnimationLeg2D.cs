using System.Collections;
using UnityEngine;

namespace lLCroweTool.AutoAnimation2D
{
    public class AnimationLeg2D : MonoBehaviour
    {
        //다리가 되는 부위이다
        //IK를 메달아둔 끝부분에 달아주는 컴포넌트
        //해당 위치로 다리를 이동

        private Transform tr;

        public Vector2 poleLegPos = Vector2.zero;//몸체로부터 다리가 있어야될 고정위치

        //움직임관련
        private Vector2 curLegPos = Vector2.zero;//다리의 마지막위치
        private Vector2 changeLegPos = Vector2.zero;//다리가 변경될 위치        
        private float moveSpeed = 1.0f;
        private bool isMove = false;

        private Vector2 m_Velocity = Vector2.zero;
        [SerializeField] float movementSmoothing = .05f;

        //크기관련
        //다리 스탭관련
        private bool isChangeSize = false;
        private Vector2 curLegScale = Vector2.one;
        private Vector2 changeLegScale = Vector2.one;

        private Vector2 m_Scale_Velocity = Vector2.zero;
        [SerializeField] float scaleSmoothing = .05f;

       


        private void Awake()
        {
            tr = transform;
            poleLegPos = tr.localPosition;
            curLegPos = poleLegPos;
            changeLegPos = curLegPos;

            curLegScale = tr.localScale;
            changeLegScale = curLegScale;
        }
                
        private void FixedUpdate()
        {                      
            if (isMove)
            {
                if (lLcroweUtil.CheckDistance(changeLegPos, curLegPos, 0.02f))
                {
                    isMove = false;
                }
                else
                {
                    curLegPos = Vector2.SmoothDamp(curLegPos, changeLegPos, ref m_Velocity, movementSmoothing);
                  

                }
                tr.localPosition = curLegPos;
                
            }

            if (isChangeSize)
            {
                if (curLegScale == changeLegPos)
                {
                    isChangeSize = false;
                }
                else
                {
                    curLegScale = Vector2.SmoothDamp(curLegScale, changeLegScale, ref m_Scale_Velocity, scaleSmoothing);
                }
                tr.localScale = curLegScale;
            }
        }

        public Transform GetTransform()
        {
            return tr;
        }

        /// <summary>
        /// 다리 방향 세팅함수
        /// </summary>
        /// <param name="_direction">방향</param>
        public void SetLegDirection(Vector2 _direction)
        {   
            changeLegPos += (_direction * moveSpeed);
            isMove = true;
        }

        /// <summary>
        /// 다리위치 초기화 함수
        /// </summary>
        public void ResetLegPos()
        {
            changeLegPos = poleLegPos;
            isMove = true;
        }

        /// <summary>
        /// 해당 다리를 위로 올리는가의 여부
        /// </summary>
        /// <param name="_isUp">올리는가?</param>
        public void SetLegUp(bool _isUp)
        {
            if (_isUp)
            {
                changeLegScale = Vector2.one * 1.3f;
            }
            else
            {
                changeLegScale = Vector2.one;
            }
            isChangeSize = true;
        }

        /// <summary>
        /// 다리 이동크기 설정함수
        /// </summary>
        /// <param name="_speed"></param>
        public void SetUnitMoveSpeed(float _speed)
        {
            moveSpeed = _speed;
        }

        private void OnDestroy()
        {
            tr = null;
        }
    }
}
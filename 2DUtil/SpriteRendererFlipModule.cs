using UnityEngine;

namespace lLCroweTool
{
    public class SpriteRendererFlipModule : MonoBehaviour
    {
        //특정방향으로 봐라봐서 작동하는방식
        //아니면 이미 지나가온 구역을 통해 작동하는 방식
        //public enum MecanicType
        //{
        //    TargetPos,
        //    PrevPos,
        //}


        //public MecanicType mecanicType;
        public Vector3 targetPos;


        private SpriteRenderer spriteRenderer;
        private bool isShowRight;
        private Vector3 prevPos;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        ////테스트 타입
        //private void Update()
        //{
        //    switch (mecanicType)
        //    {
        //        case MecanicType.TargetPos:
        //            UpdateTargetPosSpriteFlip(targetPos.x);
        //            break;
        //        case MecanicType.PrevPos:
        //            UpdatePrevPosSpriteFlip();
        //            break;
        //    }
        //}

        /// <summary>
        /// 타겟위치로의 스프라이트플립 업데이트
        /// </summary>
        /// <param name="targetPosX">타겟위치x값</param>
        public void UpdateTargetPosSpriteFlip(float targetPosX)
        {
            var pos = targetPos;
            var posX = pos.x;
            var prevPosX = prevPos.x;

            if (prevPosX == posX)
            {
                return;
            }

            var dirX = prevPosX - posX;
            var isShow = dirX > 0 ? true : false;
            if (isShow != isShowRight)
            {
                spriteRenderer.flipX = isShow;
                isShowRight = isShow;
            }
            prevPos = pos;
        }

        /// <summary>
        /// 이전위치로의 스프라이트플립 업데이트
        /// </summary>
        public void UpdatePrevPosSpriteFlip()
        {
            var pos = transform.position;
            var posX = pos.x;
            var prevPosX = prevPos.x;

            if (prevPosX == posX)
            {
                return;
            }

            var dirX = prevPosX - posX;
            var isShow = dirX > 0 ? true : false;
            if (isShow != isShowRight)
            {
                spriteRenderer.flipX = isShow;
                isShowRight = isShow;
            }
            prevPos = pos;
        }
    }
}
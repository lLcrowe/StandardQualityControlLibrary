#if MEC
using UnityEngine;
using DG.Tweening;
using MEC;
using System.Collections.Generic;

namespace lLCroweTool.AutoAnimation2D.GunRecoilAnim
{
    public class GunRecoilAnim : MonoBehaviour
    {
        //건의 주퇴복좌기를 구현하기 위한 클래스
        //현재 오브젝트를 쏘고난뒤 돌아오는 애니메이션
        [System.Serializable]
        public class GunRecoilTarget
        {
            [Header("포 발사시 뒤로 가는 설정")]
            [Tooltip("뒤로 후퇴하는거리")]
            public float backDistnace = 0;//뒤로 후퇴하는거리
            [Tooltip("뒤로 후퇴하는 시간")]
            [Range(0.0f, 2f)]
            public float backIntervalTime = 0;//뒤로 후퇴하는 시간


            [Header("포 발사후 원래되로 가는 설정")]
            [Tooltip("원래 위치")]
            public float originDistance;//원래위치
            [Tooltip("되돌아오는 시간")]
            [Range(0.0f, 2f)]
            public float resetIntervalTime = 0.3f;//되돌아오는 시간

            [Header("포 발사 위치설정")]
            [Tooltip("포 위치는 현스크립트로 설정됩니다.")]
            public Transform gunRecoilTransform;
        }

        [Header("건리코일타겟 설정")]
        public GunRecoilTarget[] gunRecoilTargetArray = new GunRecoilTarget[0];

        [Header("동시발사 설정")]
        [Tooltip("동시발사 여부")]
        public bool isSolvoShot = false;
        [SerializeField] [HideInInspector] private int firePosCount = 0;

        private CoroutineHandle handle;

        private void Awake()
        {
            for (int i = 0; i < gunRecoilTargetArray.Length; i++)
            {
                gunRecoilTargetArray[i].originDistance = gunRecoilTargetArray[i].gunRecoilTransform.localPosition.y;
            }
        }

        /// <summary>
        /// 리코일 액션
        /// </summary>
        [ButtonMethod]
        public void ActionRecoil()
        {
            ActionAnim(this);
        }

        /// <summary>
        /// 리코일 액션
        /// </summary>
        /// <param name="firePosCount">작동될 카운트</param>
        public void ActionRecoil(int firePosCount)
        {
            ActionAnim(this, firePosCount);
        }

        /// <summary>
        /// 리코일타겟 배열의 크기를 가져오는 함수
        /// </summary>
        /// <returns>배열의 크기</returns>
        public int GetgunRecoilTargetLength()
        {
            return gunRecoilTargetArray.Length;
        }

        /// <summary>
        /// 다음 발사위치번호를 가져오는 함수
        /// </summary>
        /// <returns>다음 발사위치번호</returns>
        public int GetNextFirePosCount()
        {
            return firePosCount > gunRecoilTargetArray.Length ? 0 : firePosCount;
        }

        /// <summary>
        /// 건리코일애님 작동
        /// </summary>
        /// <param name="gunRecoilAnim">건리코일애님모듈</param>
        private static void ActionAnim(GunRecoilAnim gunRecoilAnim)
        {
            //작동되고있으면 꺼버리기
            if (gunRecoilAnim.handle.IsRunning)
            {
                Timing.KillCoroutines(gunRecoilAnim.handle);
            }

            //일제사격여부
            if (gunRecoilAnim.isSolvoShot)
            {
                gunRecoilAnim.handle = Timing.RunCoroutine(ActionAnimSolvoShot(gunRecoilAnim));
                //ActionAnimSolvoShot(gunRecoilAnim);
            }
            else
            {
                //한개만작동
                if (gunRecoilAnim.firePosCount >= gunRecoilAnim.gunRecoilTargetArray.Length)
                {
                    gunRecoilAnim.firePosCount = 0;
                }
                gunRecoilAnim.handle = Timing.RunCoroutine(ActionAnimSingleShot(gunRecoilAnim, gunRecoilAnim.firePosCount));
                //ActionAnimSingleShot(gunRecoilAnim, gunRecoilAnim.firePosCount);

                gunRecoilAnim.firePosCount++;
               
            }
        }

        /// <summary>
        /// 건리코일애님 작동
        /// </summary>
        /// <param name="gunRecoilAnim">건리코일애님모듈</param>
        /// <param name="firePosCount">발사위치 번호</param>
        private static void ActionAnim(GunRecoilAnim gunRecoilAnim, int firePosCount)
        {
            //작동되고있으면 꺼버리기
            if (gunRecoilAnim.handle.IsRunning)
            {
                Timing.KillCoroutines(gunRecoilAnim.handle);
            }

            //일제사격여부
            if (gunRecoilAnim.isSolvoShot)
            {
                //ActionAnimSolvoShot(gunRecoilAnim);
                gunRecoilAnim.handle = Timing.RunCoroutine(ActionAnimSolvoShot(gunRecoilAnim));
            }
            else
            {
                //한개만작동
                if (firePosCount >= gunRecoilAnim.gunRecoilTargetArray.Length)
                {
                    firePosCount = 0;
                }

                //ActionAnimSingleShot(gunRecoilAnim, firePosCount);
                gunRecoilAnim.handle = Timing.RunCoroutine(ActionAnimSingleShot(gunRecoilAnim, firePosCount));
            }
        }

        /// <summary>
        /// 일제발사
        /// </summary>
        /// <param name="gunRecoilAnim">건리코일애님모듈</param>
        private static IEnumerator<float> ActionAnimSolvoShot(GunRecoilAnim gunRecoilAnim)
        {
            //코루틴이 아닌 함수로 사용시//원본
            //전체 다 작동
            //for (int i = 0; i < gunRecoilAnim.gunRecoilTargetArray.Length; i++)
            //{
            //    //캐싱
            //    GunRecoilTarget gunRecoilTarget = gunRecoilAnim.gunRecoilTargetArray[i];
            //    Timing.RunCoroutine(ActionAnimRecoil(gunRecoilTarget));
            //}


            float backIntervalTime = 0;
            for (int i = 0; i < gunRecoilAnim.gunRecoilTargetArray.Length; i++)
            {
                //캐싱
                GunRecoilTarget gunRecoilTarget = gunRecoilAnim.gunRecoilTargetArray[i];
                backIntervalTime = gunRecoilTarget.backIntervalTime;
                Transform tr = gunRecoilTarget.gunRecoilTransform;

                tr.DOLocalMoveY(-gunRecoilTarget.backDistnace, backIntervalTime);
            }

            //마지막꺼작동
            yield return Timing.WaitForSeconds(backIntervalTime);
           

            for (int i = 0; i < gunRecoilAnim.gunRecoilTargetArray.Length; i++)
            {
                //캐싱
                GunRecoilTarget gunRecoilTarget = gunRecoilAnim.gunRecoilTargetArray[i];
                Transform tr = gunRecoilTarget.gunRecoilTransform;
                tr.DOLocalMoveY(gunRecoilTarget.originDistance, gunRecoilTarget.resetIntervalTime);
            }
        }

        /// <summary>
        /// 단일발사
        /// </summary>
        /// <param name="gunRecoilAnim">건리코일애님모듈</param>
        /// <param name="firePosCount">발사위치 번호</param>
        private static IEnumerator<float> ActionAnimSingleShot(GunRecoilAnim gunRecoilAnim, int firePosCount)
        {
            //코루틴이 아닌 함수로 사용시//원본
            //단일만 작동
            GunRecoilTarget gunRecoilTarget = gunRecoilAnim.gunRecoilTargetArray[firePosCount];
            //Timing.RunCoroutine(ActionAnimRecoil(gunRecoilTarget));


            float backIntervalTime = gunRecoilTarget.backIntervalTime;
            Transform tr = gunRecoilTarget.gunRecoilTransform;

            tr.DOLocalMoveY(-gunRecoilTarget.backDistnace, backIntervalTime);
            yield return Timing.WaitForSeconds(backIntervalTime);
            tr.DOLocalMoveY(gunRecoilTarget.originDistance, gunRecoilTarget.resetIntervalTime);
        }

        ///// <summary>
        ///// 건리코일애님처리
        ///// </summary>
        ///// <param name="gunRecoilTarget">건리코일애님모듈</param>
        //private static IEnumerator<float> ActionAnimRecoil(GunRecoilTarget gunRecoilTarget)
        //{
        //    float backIntervalTime = gunRecoilTarget.backIntervalTime;
        //    Transform tr = gunRecoilTarget.gunRecoilTransform;

        //    tr.DOLocalMoveY(-gunRecoilTarget.backDistnace, backIntervalTime);
        //    yield return Timing.WaitForSeconds(backIntervalTime);
        //    tr.DOLocalMoveY(gunRecoilTarget.originDistance, gunRecoilTarget.resetIntervalTime);
        //}



        private void OnDrawGizmosSelected()
        {
            for (int i = 0; i < gunRecoilTargetArray.Length; i++)
            {   
                GunRecoilTarget gunRecoilTarget = gunRecoilTargetArray[i];
                Gizmos.color = Color.green;
                Vector3 direction = gunRecoilTarget.gunRecoilTransform.up * gunRecoilTarget.originDistance;
                Vector2 startPos = transform.position + (transform.right * gunRecoilTarget.gunRecoilTransform.localPosition.x) + direction;
                //Vector2 startPos = gunRecoilTarget.gunRecoilTransform.position + direction;//원본
                Gizmos.DrawWireSphere(startPos, 0.2f);


                Vector3 backDirection = gunRecoilTarget.gunRecoilTransform.up * -gunRecoilTarget.backDistnace;
                Vector2 endPos = transform.position + (transform.right * gunRecoilTarget.gunRecoilTransform.localPosition.x) + backDirection;


                //Vector2 endPos = gunRecoilTarget.gunRecoilTransform.position + gunRecoilTarget.gunRecoilTransform.up * -gunRecoilTarget.backDistnace;
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(endPos, 0.2f);
            }
        }
    }
}
#endif
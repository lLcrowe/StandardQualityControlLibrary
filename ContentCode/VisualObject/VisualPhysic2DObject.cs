using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

namespace lLCroweTool.Visual.PhysicObject
{
    public class VisualPhysic2DObject : MonoBehaviour
    {
        //콜라이더는 자율적으로 집어넣기
        //인스팩터에디터에서 처리해주기
        private Rigidbody2D rb2d;
        private Collider2D col2d;
        private SpriteRenderer sr;
        private Transform tr;

        protected void Awake()
        {
            tr = transform;
            rb2d.GetComponent<Rigidbody2D>();
            col2d.GetComponent<Collider2D>();
        }

        /// <summary>
        /// 콜라이더의 트리거 변경함수
        /// </summary>
        /// <param name="_isTrigger">트리거와 콜라이더 변경</param>
        public void SetColliderTrigger(bool _isTrigger)
        {
            col2d.isTrigger = _isTrigger;
        }

        public Transform GetTransform()
        {
            return tr;
        }

        public static void ActionPhysic(VisualPhysic2DObject visualPhysicObject, bool isFadeObject, float tempFadeTime, bool isDisappear, float disableTimer)
        {
            //충돌체 활성화 부모객체변경
            //visualPhysicObject.rb2d.bodyType = RigidbodyType2D.Dynamic;            
            visualPhysicObject.transform.parent = null;
            SpriteRenderer sr = visualPhysicObject.sr;
            Color color = sr.color;
            color.a = 1;
            sr.color = color;

            if (isFadeObject)
            {
                //랜더링 사라지게
                sr.DOFade(0, tempFadeTime);
            }
            if (isDisappear)
            {

                ActionWaitDeActive(visualPhysicObject, disableTimer);
            }
        }

        /// <summary>
        /// 물리 작동
        /// </summary>
        /// <param name="directionPower">방향파워</param>
        /// <param name="rotatePower">회전파워</param>
        /// <param name="forceMode2D">파워종류</param>
        /// <param name="disableTimer">사라지는 시간초</param>
        public static void ActionPhysic(VisualPhysic2DObject visualPhysicObject, Vector2 directionPower, float rotatePower, ForceMode2D forceMode2D, float disableTimer)
        {
            Rigidbody2D rb2d = visualPhysicObject.rb2d;

            //쪼매 문제 될수도
            //힘작동
            rb2d.AddRelativeForce(directionPower, forceMode2D);
            rb2d.AddTorque(rotatePower);

            //충돌체 활성화 부모객체변경
            rb2d.bodyType = RigidbodyType2D.Dynamic;
            visualPhysicObject.col2d.enabled = true;
            visualPhysicObject.transform.parent = null;

            //랜더링 사라지게
            SpriteRenderer sr = visualPhysicObject.sr;
            Color color = sr.color;
            color.a = 1;
            sr.color = color;
            sr.DOFade(0, disableTimer);

            ActionWaitDeActive(visualPhysicObject, disableTimer);
        }



        private static void ActionWaitDeActive(VisualPhysic2DObject visualPhysicObject, float disableTimer)
        {

#if MEC
            //게임오브젝트 비활성화와 되돌리기
            Timing.RunCoroutine(WaitAndDeActive(visualPhysicObject, disableTimer));
#else

            visualPhysicObject.StartCoroutine(WaitAndDeActive(visualPhysicObject, disableTimer));

#endif


        }

        private static IEnumerator WaitAndDeActive(VisualPhysic2DObject visualPhysicObject, float disable)
        {


            float time = Time.time;
            do
            {
                if (time + disable < Time.time) 
                {
                }
                yield return null;
            } while (true);

            visualPhysicObject.gameObject.SetActive(false);
        }

#if MEC
        private static IEnumerator<float> WaitAndDeActive(VisualPhysic2DObject visualPhysicObject, float disable)
        {
            yield return Timing.WaitForSeconds(disable + 0.5f);
            visualPhysicObject.gameObject.SetActive(false);
        }
#endif
        /// <summary>
        /// 빈탄창 초기화
        /// </summary>
        /// <param name="emtpyMagazine">타겟팅할 빈탄창</param>
        public static void InitEmtpyMagazine(VisualPhysic2DObject visualPhysicObject)
        {
            visualPhysicObject.rb2d.bodyType = RigidbodyType2D.Kinematic;
            visualPhysicObject.col2d.enabled = false;
        }

        private void OnDestroy()
        {
            tr = null;
            sr = null;
            rb2d = null;
            col2d = null;
        }
    }
}
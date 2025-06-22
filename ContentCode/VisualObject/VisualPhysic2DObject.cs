using UnityEngine;
using DG.Tweening;

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

        public void ActionPhysic(bool isFadeObject, float tempFadeTime, bool isDisappear, float disableTimer)
        {
            //충돌체 활성화 부모객체변경
            //visualPhysicObject.rb2d.bodyType = RigidbodyType2D.Dynamic;            
            transform.parent = null;            
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
                this.ActionAndDisable(disableTimer, null);
            }
        }

        /// <summary>
        /// 물리 작동
        /// </summary>
        /// <param name="directionPower">방향파워</param>
        /// <param name="rotatePower">회전파워</param>
        /// <param name="forceMode2D">파워종류</param>
        /// <param name="disableTimer">사라지는 시간초</param>
        public void ActionPhysic(Vector2 directionPower, float rotatePower, ForceMode2D forceMode2D, float disableTimer)
        {
            //쪼매 문제 될수도
            //힘작동
            rb2d.AddRelativeForce(directionPower, forceMode2D);
            rb2d.AddTorque(rotatePower);

            //충돌체 활성화 부모객체변경
            rb2d.bodyType = RigidbodyType2D.Dynamic;
            col2d.enabled = true;
            transform.parent = null;

            //랜더링 사라지게
            Color color = sr.color;
            color.a = 1;
            sr.color = color;
            sr.DOFade(0, disableTimer);


            this.ActionAndDisable(disableTimer, null);
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
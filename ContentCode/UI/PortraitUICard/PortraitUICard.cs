#if Doozy
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace lLCroweTool
{
    public class PortraitUICard : MonoBehaviour
    {
        //포트레이트를 담당하는 클래스이다.
        //포트레이트로 사용할 애니메이션 프로그램은 총두가지
        //애니포트레이트와 스파인or 스프라이트 교체

        //포트레이트를 담당하는 오브젝트        

        public AnimamtionPortraitMessageType animamtionPortraitMessageType;

        //세팅해주는게 필요
        public TextMeshProUGUI portraitNameText;//포트레이트 이름 텍스트
        public Image portraitNameBackGroundImageObject;//포트레이트 이름 배경
        public Image borderImageObject;//포트레이트 뒷배경 테두리
        public GameObject targetAnimObject;//초상화
        public Image backGroundImageObject;//포트레이트 뒷배경이름
        public ActorInfoObjectScript actorInfoData;//액터정보데이터 비교용
        private Image targetUIView;

        private void Awake()
        {
        }

        /// <summary>
        /// 초상화UI카드를 세팅해주는 함수_제작중
        /// </summary>
        public void SetPortraitUICard(AnimamtionPortraitMessageType animName = AnimamtionPortraitMessageType.Idle)
        {
            //초기는 Idle로 작동
            
        }

        /// <summary>
        /// 보여주는 효과를 세팅해주는 함수
        /// </summary>
        public void SetShowType()
        {
            //일반적인 보여주기면 테두리 없이 보여줌
            //함선이나 통신이면 테두리와 특수효과로 보여주게
        }

        ///// <summary>
        ///// 초상화를 보여주는 함수
        ///// </summary>
        //public void ShowPortraitUICard()
        //{
        //    targetUIView.Show();
        //}

        ///// <summary>
        ///// 초상화를 닫는 함수
        ///// </summary>
        //public void OffPortraitUICard()
        //{
        //    targetUIView.Hide();
        //}
    }
    /// <summary>
    /// 포트레이트 사용시 포트레이트에게 이벤트메세지를 보내기 위한 변수
    /// </summary>
    public enum AnimamtionPortraitMessageType
    {
        None,
        Idle,
        Smile,


    }

}
#endif
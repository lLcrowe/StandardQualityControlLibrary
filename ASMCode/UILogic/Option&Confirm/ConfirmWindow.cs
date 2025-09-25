using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UILibrary;


namespace lLCroweTool.UI.Confirm
{
    public class ConfirmWindow : MonoBehaviour
    {
        //여기도 좀더 좋게 만들수 있을거 같긴함//20231030

        //확인창
        //버튼 두개 or 확인 버튼 한개
        //Yes//No
        //각각 이벤트설정후 돌려받을수 있음
        //이것저것찾아보니 Doozy PopUP이라는게 현재 이것의 기능을 대체하고 있음 체크해봐야함
        [SerializeField] private TextMeshProUGUI titleTextObject;
        [SerializeField] private Image titleImageObject;
        //밑의 두버튼은 호라이즌레이아웃 하단에 배치하여 확인창과 예아니오창을 겸인하고 작업하게 제작함
        [SerializeField] private CustomButton confirmButtonObject;//확인버튼
        [SerializeField] private CustomButton yesButtonObject;//등록된 이벤트를 작동시키고 동작정지
        [SerializeField] private CustomButton noButtonObject;//아무행동을 안함
                
        private System.Action yesAction;
        private System.Action noAction;
        private System.Action confirmAction;

        protected virtual void Awake()
        {   
            yesButtonObject.onClickAction = ()=> 
            {
                OffConfirmWindowUIView();
                yesAction?.Invoke();
            };
            noButtonObject.onClickAction = ()=> 
            {
                OffConfirmWindowUIView();
                noAction?.Invoke();
            };
            confirmButtonObject.onClickAction = () =>
            {
                OffConfirmWindowUIView();
                confirmAction?.Invoke();
            };
        }

        /// <summary>
        /// 확인창을 세팅해주는 함수
        /// </summary>
        /// <param name="titleText">제목</param>
        /// <param name="yesAction">'예'할시 기능</param>
        /// <param name="noAction">'아니오'할시 기능</param>
        /// <param name="yesButtonText">Yes버튼의 텍스트</param>
        /// <param name="noButtonText">No버튼의 텍스트</param>
        /// <param name="titleSprite">제목스프라이트 이미지</param>
        public void SetConfirmWindow(string titleText, System.Action yesAction, System.Action noAction, string yesButtonText = "Yes", string noButtonText = "No", Sprite titleSprite = null)
        {
            ResetEvent();
            ShowConfirmWindowUIView();

            //YES NO 보여줌
            confirmButtonObject.gameObject.SetActive(false);
            noButtonObject.gameObject.SetActive(true);
            yesButtonObject.gameObject.SetActive(true);

            //titleTextObject.text = LocalizingManager.Instance.GetLocalLizeText(titleText);
            //yesButtonObject.SetLabelText(LocalizingManager.Instance.GetLocalLizeText(yesButtonText));
            //noButtonObject.SetLabelText(LocalizingManager.Instance.GetLocalLizeText(noButtonText));

            titleImageObject.sprite = titleSprite;
            titleTextObject.text = titleText;
            yesButtonObject.SetLabelText(yesButtonText);
            noButtonObject.SetLabelText(noButtonText);
            this.yesAction = yesAction;
            this.noAction = noAction;
        }

        /// <summary>
        /// 확인창을 세팅해주는 함수
        /// </summary>
        /// <param name="titleText">제목</param>
        /// <param name="confirmAction">확인할시 기능</param>
        /// <param name="confirmButtonText">확인버튼의 텍스트</param>
        /// <param name="titleSprite">제목스프라이트 이미지</param>
        public void SetConfirmWindow(string titleText, System.Action confirmAction, string confirmButtonText = "Confirm", Sprite titleSprite = null)
        {
            ResetEvent();
            ShowConfirmWindowUIView();

            //알림창
            //알림창여부
            //확인만 보여줌
            confirmButtonObject.gameObject.SetActive(true);
            noButtonObject.gameObject.SetActive(false);
            yesButtonObject.gameObject.SetActive(false);

            titleImageObject.sprite = titleSprite;
            titleTextObject.text = titleText;
            confirmButtonObject.SetLabelText(confirmButtonText);
            this.confirmAction = confirmAction;
        }

        private void ResetEvent()
        {
            yesAction = null;
            noAction = null;
            confirmAction = null;
        }
    
        /// <summary>
        /// 확인창을 보여주는 함수
        /// </summary>
        public virtual void ShowConfirmWindowUIView()
        {
            this.SetActive(true);
            //if (targetUIView.IsShowing)
            //{
            //    return;
            //}
            //targetUIView.Show();
        }

        /// <summary>
        /// 확인창을 닫아주는 함수
        /// </summary>
        protected virtual void OffConfirmWindowUIView()
        {
            this.SetActive(false);
            //if (targetUIView.IsHiding)
            //{
            //    return;
            //}
            //targetUIView.Hide();
        }
    }
}
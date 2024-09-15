using TMPro;
using lLCroweTool.SingletonUI;

namespace lLCroweTool.NoticeDisplay
{
    //상호작용을 할시 직접적으로 단일텍스트를 보여주는 기능을 가짐
    public class NoticeDirectText : MonoBehaviourSingletonUI<NoticeDirectText>
    {   
        public TextMeshProUGUI targetText;

        protected override void Awake()
        {
            base.Awake();
            targetText = GetComponent<TextMeshProUGUI>();            
        }

        public override void ShowUIView()
        {
            base.ShowUIView();
        }

        public override void OffUIView()
        {
            base.OffUIView();
        }

        /// <summary>
        /// 다이렉트알람을 보여주는 함수
        /// </summary>
        /// <param name="content">내용</param>
        public void ShowNoticeDirectText(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                OffUIView();
                return;
            }

            if (content == targetText.text)
            {
                OffUIView();
                return;
            }
            targetText.text = content;
            ShowUIView();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            targetText = null;
        }
    }
}

#if Doozy
using UnityEngine.UI;
using UnityEngine;
using Doozy.Engine.UI;
using lLCroweTool.UI.Confirm;

namespace lLCroweTool
{
    public class RewardSelectViewUI : MonoBehaviour
    {
        //퀘스트를 클리어후 
        //퀘스트보상을 보여주고 가질수 있게 하는 기능을 가짐
        //선택카드과 전체주는 두개가 있다.


        //퀘스트 보상을 선택할떄 확인을 위한 창
        public ConfirmWindow confirmWindow;
        public HorizontalLayoutGroup horizontalLayout;


        private UIView targetUIView;

        private void Awake()
        {
            targetUIView = GetComponent<UIView>();
        }

        /// <summary>
        /// 창 보여주는 함수
        /// </summary>
        public void ShowQuestRewardViewUI()
        {
            targetUIView.Show();
        }

        /// <summary>
        /// 창 닫는 함수
        /// </summary>
        public void OffQuestRewardViewUI()
        {
            targetUIView.Hide();
        }
    }
}
#endif
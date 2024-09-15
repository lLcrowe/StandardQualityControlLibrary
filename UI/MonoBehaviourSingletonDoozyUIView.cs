//using Doozy.Engine.UI;
//using lLCroweTool.LogSystem;
//using UnityEngine;

//namespace lLCroweTool.SingletonUI
//{
    
//    public class MonoBehaviourSingletonDoozyUIView<T> : MonoBehaviourSingletonUI<T> where T : MonoBehaviour
//    {
//        private UIView targetUIView;//타겟이 될 UIView

//        protected override void Awake()
//        {   
//            targetUIView = GetComponent<UIView>();
//            base.Awake();
//        }

//        /// <summary>
//        /// UI 뷰 숨기기
//        /// </summary>
//        public override void ShowUIView()
//        {
//            if (!targetUIView.IsShowing)
//            {
//                targetUIView.Show();
//            }
//        }

//        /// <summary>
//        /// UI 뷰 숨기기
//        /// </summary>
//        public override void OffUIView()
//        {
//            if (!targetUIView.IsHiding)
//            {
//                targetUIView.Hide();
//            }
//        }
//    }
//}
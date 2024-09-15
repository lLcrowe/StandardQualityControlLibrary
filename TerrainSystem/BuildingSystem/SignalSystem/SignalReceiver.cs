using UnityEngine;
using UnityEngine.Events;

namespace lLCroweTool.BuildingSystem.SignalSystem
{
    public class SignalReceiver : MonoBehaviour
    {   
        //신호 수신기
        //특정신호를 송신기로 부터 받아 툭정 액션을 작동시킴        

        [Header("신호수신기 신호타입설정")]
        /// <summary>
        /// 신호수신기의 신호타입
        /// </summary>
        public ESignalType signalType;//신호작동 타입을 세팅함//신호타입에 따라 연결할수 있는 송신기가 다름
        public SignalActionEvent signalActionEvent;//작동이벤트

        private void Awake()
        {
            if (signalActionEvent == null)
            {
                signalActionEvent = new SignalActionEvent();
            }
        }

        /// <summary>
        /// 신호수신기의 신호타입을 가져옴
        /// </summary>
        /// <returns>신호타입</returns>
        public ESignalType GetSignalType()
        {
            return signalType;
        }

        /// <summary>
        /// 신호수신기의 신호타입을 세팅
        /// </summary>
        /// <param name="targetSignalType">신호타입</param>
        public void SetSignalType(ESignalType targetSignalType)
        {
            signalType = targetSignalType;
        }

        /// <summary>
        /// 신호를 보냈을때 수신기가 작동될 동작을 정의하는 함수
        /// </summary>
        /// <param name="index">작동상태</param>        
        public void RequestSignalReceiverAction(int index)
        {
            signalActionEvent?.Invoke(index);
        }

        /// <summary>
        /// 신호를 보냈을때 수신기가 작동될 동작 이벤트에 등록하는 함수
        /// </summary>
        /// <param name="action">이벤트</param>
        public void SetSignalActionEvent(UnityAction<int> action)
        {
            signalActionEvent.RemoveAllListeners();
            signalActionEvent.AddListener(action);
        }
    }

    [System.Serializable]
    //신호 액션관련
    public class SignalActionEvent : UnityEvent<int> { }
}
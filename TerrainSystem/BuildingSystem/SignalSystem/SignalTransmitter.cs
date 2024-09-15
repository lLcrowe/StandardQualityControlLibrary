using UnityEngine;

namespace lLCroweTool.BuildingSystem.SignalSystem
{
    /// <summary>
    /// 신호 송신기 
    /// </summary>
    public class SignalTransmitter : MonoBehaviour
    {
        //신호 송신기
        //특정신호를 신호 수신기에게 보내주는 역할을 해줌
        //송신기 수신기 1대1로 통신됨//그래서 이컴포넌트를 여러개 연결시켜서 작동
        //transmitter receiver

        /// <summary>
        /// 신호송신기의 신호타입
        /// </summary>
        public ESignalType signalType;//신호작동 타입을 세팅함//신호타입에 따라 연결할수 있는 수신기가 다름

        [SerializeField] private bool isSettingToSignalReceiver;//수신기세팅됫는지 여부
        [SerializeField] private SignalReceiver targetSignalReceiver;//신호를 보내줄 신호수신기
        private bool currentState = false;//신호가 변경됫는지 기억하는 역할
        private int cachingIndex = -1;//전에 작동됫던 인덱스번호를 기억해주는 역할//초기는 신호없음을 기억함

        /// <summary>
        /// 신호송신기의 신호타입을 가져옴
        /// </summary>
        /// <returns>신호타입</returns>
        public ESignalType GetSignalType()
        {
            return signalType;
        }

        /// <summary>
        /// 신호송신기의 신호타입을 세팅
        /// </summary>
        /// <param name="targetSignalType">신호타입</param>
        public void SetSignalType(ESignalType targetSignalType)
        {
            signalType = targetSignalType;
        }

        /// <summary>
        /// 신호송신기에서 수신기에게 신호를 보내주는 함수
        /// </summary>
        /// /// <param name="index">작동번호</param>
        /// <param name="isAction">작동상태</param>
        public void SendInputSignal(int index, bool isAction)
        {
            //if(ReferenceEquals(targetSignalReceiver, null))
            //{
            //    Debug.Log("수신기가 존재하지않음",gameObject);
            //    return;
            //}
            //else
            //{
            //    Debug.Log("수신기가 존재함", gameObject);
            //    Transform tempTr = targetSignalReceiver.GetTransform();
            //    Debug.Log("수신기 오브젝트 이름 : " + tempTr.name, tempTr.gameObject);
            //}

            //작동방식에 따라 작업되게
            switch (signalType)
            {
                case ESignalType.JoyStick:
                    //작동완료
                    //신호캐치
                    //꾸준히 캐치됨
                    if (index > -1 && isAction == true && !currentState)
                    {
                        switch (index) 
                        {
                            case 0:
                                //업
                            case 1:
                                //다운
                            case 2:
                                //우측
                            case 3:
                                //좌측                                
                                Debug.Log("신호송신기 업데이트 신호보냄 (신호" + index + ")");
                                targetSignalReceiver.RequestSignalReceiverAction(index);
                                currentState = !currentState;//false
                                cachingIndex = index;
                                break;
                            default:
                                //아무행동없음
                                //Debug.Log("신호송신기 신호해석없음(" + index + ")");
                                break;
                        }
                    }
                    else if (currentState == true && isAction == false)
                    {
                        //신호가 없으면
                        //되돌림
                        //한번만 작동되게
                        Debug.Log("신호송신기 신호취소 (신호리셋)");
                        targetSignalReceiver.RequestSignalReceiverAction(-1);//-1입력되야됨
                        currentState = !currentState;//true
                        cachingIndex = index;
                    }
                    break;
                case ESignalType.Button:
                    //작동완료
                    //신호캐치
                    //꾸준히 캐치됨
                    if (index == 4 && isAction == true && !currentState)
                    {
                        Debug.Log("신호송신기 업데이트 신호보냄 (신호" + true + ")");
                        targetSignalReceiver.RequestSignalReceiverAction(1);
                        currentState = !currentState;//false
                        cachingIndex = index;
                    }
                    else if (currentState == true && isAction == false)
                    {
                        //신호가 없으면
                        //되돌림
                        //한번만 작동되게
                        Debug.Log("신호송신기 신호취소 (신호" + false + ")");
                        targetSignalReceiver.RequestSignalReceiverAction(0);
                        currentState = !currentState;//true
                        cachingIndex = index;
                    }
                    break;
                case ESignalType.Switch:
                    //작동완료
                    //신호캐치
                    //꾸준히 캐치됨
                    //4번만
                    if (index == 4 && isAction == true && !currentState)
                    {
                        if (cachingIndex == -1)
                        {
                            //무신호였으면 신호가 작동되게
                            Debug.Log("신호송신기 업데이트 신호보냄 (신호 " + true + ")");
                            targetSignalReceiver.RequestSignalReceiverAction(1);
                            cachingIndex = index;
                        }
                        else
                        {
                            //신호가 존재했으면 무신호로 작동되게
                            Debug.Log("신호송신기 업데이트 신호보냄 (신호 " + false + ")");
                            targetSignalReceiver.RequestSignalReceiverAction(0);
                            cachingIndex = -1;
                        }                        
                        currentState = !currentState;//false
                    }
                    else if (currentState == true && isAction == false)
                    {
                        //신호가 없으면                        
                        //한번만 작동되게                        
                        currentState = !currentState;//true                        
                    }
                    break;
                case ESignalType.Slice:
                    //작동완료
                    //신호캐치
                    //꾸준히 캐치됨
                    //0, 1번만
                    if (index > -1 && isAction == true && !currentState)
                    {
                        //0번 업
                        if (index == 0)
                        {
                            Debug.Log("신호송신기 업데이트 신호보냄 (신호 업)");
                            targetSignalReceiver.RequestSignalReceiverAction(1); 
                        }
                        //1번 다운
                        else if(index == 1)
                        {
                            Debug.Log("신호송신기 업데이트 신호보냄 (신호 다운)");
                            targetSignalReceiver.RequestSignalReceiverAction(0);                            
                        }
                        cachingIndex = index;
                        currentState = !currentState;//false
                    }
                    else if (currentState == true && isAction == false)
                    {
                        //신호가 없으면                        
                        //한번만 작동되게                        
                        currentState = !currentState;//true
                        cachingIndex = index;
                    }
                    break;
            }
        }
       
        /// <summary>
        /// 타겟이 될 신호수신기를 세팅하는 함수
        /// </summary>
        /// <param name="signalReceiver">타겟이 될 신호수신기</param>
        public void SetTargetSinalReceiver(SignalReceiver signalReceiver)
        {
            targetSignalReceiver = signalReceiver;
            isSettingToSignalReceiver = !ReferenceEquals(targetSignalReceiver, null);
        }

        /// <summary>
        /// 송신기가 세팅됫는지 여부
        /// </summary>
        /// <returns>세팅이 된 송신기 인가</returns>
        public bool GetIsSetSignalReceiver()
        {
            return isSettingToSignalReceiver; 
        }
    }

    //룰을 써줘야함
    //신호는 한번만 체인지됨
    //메세지는 계속보내줌
    //신호는 6가지로 보내줌
    //-1 : 아무행동없음    //신호가 없는상태X
    //0 : 업              //신호가 있는상태O
    //1 : 다운            //신호가 있는상태O
    //2 : 우측            //신호가 있는상태O
    //3 : 좌측            //신호가 있는상태O
    //4 : 스페이스        //신호가 있는상태O

    //index작동상태 표시법
    //JoyStick => -1,0,1,2,3 로 체크
    //Button, Switch, Slice => bool값으로 체크
    //bool값일시 true 1 , false 0 //작동 온오프 여부와 업다운 여부    
    //나머지 존재없음
    //조건문 처리할떄는 스위치문으로 쓸것
    public enum ESignalType
    {
        JoyStick,//좌우상하 4방향 + 가운데 1개//신호가 없을시 되돌림
        Button,//클릭//-1, 0 => 2개//신호가 없을시 되돌림
        Switch,//딸칵//-1, 0 => 2개//상태변환
        Slice,//딸칵//-1, N => N개/상태변환 위 아래로 슬라이스단계를 조절
        //슬라이스는 수신기에서 별도로 로직이 필요하다
        //여기는 위아래만 체크하여 위쪽으로 아래쪽으로만 신호를 준다
        //수신기에서 신호를 받고 최대 최소를 체크후 변경으로 하는게 맞아보인다
    }
}
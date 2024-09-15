using lLCroweTool.LogSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lLCroweTool.UI.MainMenu
{
    /// <summary>
    /// 키세팅을 바꿔줄때 사용하는 UI클래스
    /// </summary>    
    public class InputKeySettingUI : MonoBehaviour
    {   
        public InputKeySettingButton inputKeySettingButtonPrefab;   //인풋세팅UI카드 프리팹오브젝트
        public Transform[] buttonPosArray = new Transform[2];       //인풋세팅UI카드의 위치들3개
        public Color selectColor = Color.yellow;                    //선택했을시 버튼의 바탕색이 변경될 색

        //[SerializeField] private bool isInit = false;//초기화여부
        private bool isSetting = false;//키변경버튼을 누를시부터 작동되는 세팅중 여부
        private bool isCancel = false;//취소여부
        private KeyCode targetKeyCode;//타겟팅할 키코드        


        protected void Awake()
        {
            LogManager.Register(name, name, true, true);
        }

        private void Start()
        {
            InitInputSettingUI();
        }

        /// <summary>
        /// 인풋키세팅UI를 초기화하는 함수
        /// </summary>
        public void InitInputSettingUI()
        {
            var instance = InPutKeySystem.Instance;
            var normalKeyBible = instance.NormalKeyDataList;
            var secondaryKeyBible = instance.SecondaryKeyDataList;

            foreach (var item in normalKeyBible)
            {
                InputKeySettingButton temp = ObjectPoolManager.Instance.RequestDynamicComponentObject(inputKeySettingButtonPrefab);
                temp.transform.SetParent(buttonPosArray[0]);
                temp.InitInputSettingUICard(item.keyName, item.keyCode, ()=> InputKeySettingButtonFunc(item, temp, normalKeyBible), selectColor);
            }

            foreach (var item in secondaryKeyBible)
            {
                InputKeySettingButton temp = ObjectPoolManager.Instance.RequestDynamicComponentObject(inputKeySettingButtonPrefab);
                temp.transform.SetParent(buttonPosArray[1]);
                temp.InitInputSettingUICard(item.keyName, item.keyCode, ()=> InputKeySettingButtonFunc(item, temp, secondaryKeyBible), selectColor);
            }
        }

        /// <summary>
        /// 인풋키세팅버튼 기능
        /// </summary>
        /// <param name="keyData">세팅당할 키코드</param>
        /// <param name="inputSettingButton">인풋키세팅버튼</param>
        /// <param name="targetKeyBible">키 사전</param>        
        private void InputKeySettingButtonFunc(KeyData keyData, InputKeySettingButton inputSettingButton, List<KeyData> keyDataList)
        {
            if (isSetting)
            {
                return;
            }        
            isSetting = true;
            StartCoroutine(UpdateKeySettingCoroutine(keyData, inputSettingButton, keyDataList));
        }

        private void OnGUI()
        {
            //코루틴으로 인해 작동되고 있으면 그때부터 작동
            if (isSetting)
            {
                Event @event = Event.current;
                if (@event.isKey)
                {
                    targetKeyCode = @event.keyCode;
                    isSetting = false;
                    if (targetKeyCode == KeyCode.Escape)
                    {
                        isCancel = true;
                    }
                }
                else if (@event.isMouse)
                {   
                    targetKeyCode = (KeyCode)@event.button + 323;
                    isSetting = false;
                }
            }
        }

        private IEnumerator UpdateKeySettingCoroutine(KeyData keyData, InputKeySettingButton inputSettingButton,List<KeyData> keyDataList)
        {
            do
            {
                //OnGUI에서 세팅이 되면 넘어감
                //여기서 이벤트받아오니 이벤트가 제대로 안넘어옴
                if (isCancel)
                {
                    isCancel = false;
                    break;
                }


                if (!isSetting)
                {   
                    //해당되는 키 사전에서 중복체크
                    if (CheckOverlap(keyDataList, targetKeyCode))
                    {
                        //중복됨
                        //나중에 컨펌창 띄워주기 구역                    
                        LogManager.Log(typeof(InputKeySettingUI), "중복됩니다", inputSettingButton.gameObject, LogManager.LogType.Info);
                        break;
                    }
                    else
                    {
                        keyData.keyCode = targetKeyCode;
                        inputSettingButton.ChangeButtonText(targetKeyCode.ToString());
                        break;
                    }
                }
                yield return null;
            } while (true);
            inputSettingButton.ChangeButtonColor(Color.white);
        }

        private bool CheckOverlap(List<KeyData> keyDataList, KeyCode keyCode)
        {
            for (int i = 0; i < keyDataList.Count; i++)
            {
                var keyData = keyDataList[i];

                if (keyData.keyCode != keyCode)
                {
                    continue;
                }
                return true;
            }
            return false;
        }
    }
}

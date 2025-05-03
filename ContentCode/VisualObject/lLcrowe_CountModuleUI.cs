using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace lLCroweTool.TimerSystem
{
    public class lLcrowe_CountModuleUI : MonoBehaviour
    {
        //UI모듈을 타입
        public CountUIType uIType;
        public float a;
        //할당시킬때 필요한목적
        Text text;
        Image image;


        private void Awake()
        {
            switch (uIType)
            {
                case CountUIType.text:
                    text = GetComponent<Text>();
                    text.text = "";
                    break;
                case CountUIType.image:
                    image = GetComponent<Image>();
                    image.type = Image.Type.Filled;
                    break;
            }

        }

        //새로갱신할때 소환
        public void CountModuleUIUpdate(int value, int maxValue)
        {
            if (value > maxValue)
            {
                value = maxValue;
            }
            switch (uIType)
            {
                case CountUIType.text:
                    text.text = value.ToString();
                    break;
                case CountUIType.image:
                    //일부값 ÷ 전체값 X 100
                    //예제) 300에서 105는 몇퍼센트?
                    //답: 35 %

                    //전체값 X 퍼센트 ÷ 100
                    //예제) 300의 35퍼센트는 얼마?
                    //답) 105
                    //fillAmount = 1;(MAX)
                    a = value / maxValue * 0.1f;
                    image.fillAmount = a;
                    //image.fillAmount = value / maxValue;




                    //image.fillAmount = value
                    break;
            }
        }

        public void CountModuleUIONOFF(bool onOff)
        {
            switch (uIType)
            {
                case CountUIType.text:
                    text.enabled = onOff;
                    break;
                case CountUIType.image:
                    image.enabled = onOff;
                    break;
            }
        }


        public enum CountUIType
        {
            none,
            text,
            image
        }
        public enum CountImageType
        {

        }
    }

}


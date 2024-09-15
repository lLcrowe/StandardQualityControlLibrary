using UnityEngine;
using TMPro;
using System.Text;

namespace lLCroweTool.UI.NoiseText
{
    public class CustomNoiseText : MonoBehaviour
    {
        //20221128완
        //대화시스템과 연동시켜서 처리할수 있게
        //Zstring과 연동시켜 좀더 괜찮게 처리예정        

        public TextMeshProUGUI targetTextObject;

        private string targetText;
        private int curTextPosNum;

        //다음텍스트문자로 넘어가기 위한 딜레이 타임
        public float nextTextMoveDelayTimer = 0.05f;
        private float nextTextMoveDelaytime;

        //텍스트가 랜덤변환될떄마다의 딜레이 타임
        public float textChangeDelayTimer = 0.05f;
        private float textChangeDelaytime;

        private static StringBuilder stringBuilder = new StringBuilder(200);//나중에 유틸꺼쓰기

        //랜덤으로 보여줄 문자들
        public string randomShowString = "ABCDEFGHIJKLMN!@#$%^&*(){}[]";
        //public string showString = "abcdefghijklmn!@#$%^&*(){}[]";
        //public string showString = "Ⅳ∀∃";

        //대문자로 볼시 길이 바뀌는 폭이 커서 신경쓰이는분들도 있음
        //소문자로 하면 바뀌는 폭이 작아 괜찮아짐
        //길이가 너무길면 지루한 느낌이 느껴지시는분들도 있음


        private bool isShowAllContent = true;

        private void Awake()
        {
            textChangeDelaytime = Time.time;
            nextTextMoveDelaytime = Time.time;
            isShowAllContent = true;
        }

        void Update()
        {
            if (isShowAllContent)
            {
                return;
            }

            //변경 문자위치의 문자를 랜덤으로 변경
            if (Time.time > textChangeDelaytime + textChangeDelayTimer)
            {
                //문자 받은 길이로
                //현재 텍스트위치값부터 타겟팅된 텍스트 길이까지
                for (int i = curTextPosNum; i < targetText.Length; i++)
                {
                    stringBuilder[i] = randomShowString[Random.Range(0, randomShowString.Length)];//기존거에 삽입
                }

                textChangeDelaytime = Time.time;
                targetTextObject.text = stringBuilder.ToString();
            }

            //다음문자가 생성
            if (Time.time > nextTextMoveDelaytime + nextTextMoveDelayTimer)
            {
                stringBuilder[curTextPosNum] = targetText[curTextPosNum];//기존거에 삽입
                curTextPosNum++;
                if (curTextPosNum >= targetText.Length)
                {
                    //다 생성됫으면 정지
                    isShowAllContent = true;
                }

                nextTextMoveDelaytime = Time.time;
                targetTextObject.text = stringBuilder.ToString();
            }
        }


        public void SetText(string text)
        {
            targetText = text;
            curTextPosNum = 0;
            targetTextObject.text = "";
            isShowAllContent = false;
            stringBuilder.Clear();
            for (int i = 0; i < text.Length; i++)
            {
                stringBuilder.Insert(0, "-");
            }
        }
    }


}

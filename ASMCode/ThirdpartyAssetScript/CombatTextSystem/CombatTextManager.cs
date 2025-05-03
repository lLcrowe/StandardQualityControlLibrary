using UnityEngine;
using System.Collections.Generic;
using TMPro;
using lLCroweTool.Singleton;

namespace lLCroweTool.CombatTextSystem.Origin
{
    public class CombatTextManager : MonoBehaviourSingleton<CombatTextManager>
    {
        //컴뱃 테스트매니저
        //원하는 컴뱃 테스트를 표현할때
        //여기를 통해서 가져온다
        //크리티컬같은건 자기자신한테 체크한다음 해당 클래스에서 여기를 호출할떄 정함

        public enum CombatTextActionType{ CustomCombatText, DamageNumbersPro }
        [Header("작동방식")]
        public CombatTextActionType combatTextActionType;
          


        //직접만든것
        [Header("제작한 컴뱃텍스트")]
        public Canvas combatTextCanvas;//캔버스로 고정형 좌표에 옮긴다?
        public CombatText combatTextPrefab;
        public int combatTextNumber;//컴뱃텍스트를 몇개 만들것인지
        private int combatTextCount = 0;//현재 작동되는 컴뱃텍스트 인덱스
                                        //컴뱃텍스트 리스트
        private List<CombatText> combatTexts = new List<CombatText>();

        [Space]
        //랜덤한 텍스트 위치를 해주고 싶을떄 쓰는 변수들
        public bool isRandomDirection = false;//랜덤적인 이동방향을 가질것인가
        public Vector2 minRandomVecter2 = new Vector2(-1, 1);//최소 방향
        public Vector2 maxRandomVecter2 = new Vector2(1, 1);//최대방향

        [Space]
        public float moveTextSpeed;//텍스트가 움직이는 속도
        public Vector3 directionText;//텍스트 방향
        public float fadeTime;//사라지는 시간

        [Space]
        public Vector2 normalTextSize = new Vector2(1, 1);
        public Vector2 critTextSize = new Vector2(1.2f, 1.2f);

        [Space]
        //테스트용
        [Header("테스트용")]
        public string testCharecter = "HI";
        public Transform testTarget;
        public CombatTextType testCombatTextType;
        public CombatTextColor testCombatTextColor;



        protected override void Init()
        {
            //처음시작할떄 컴뱃텍스트 매니저 세팅
            for (int i = 0; i < combatTextNumber; i++)
            {
                //CombatText combatText = Instantiate(combatTextPrefab, combatTextCanvas.transform.position, Quaternion.identity, combatTextCanvas.transform);
                CombatText combatText = Instantiate(combatTextPrefab, transform.position, Quaternion.identity, transform);
                combatTexts.Add(combatText);
                combatText.gameObject.SetActive(false);
            }
        }

        //컴벳텍스트보여주기
        public void showCombatText(Transform _transform, string text, CombatTextColor color, CombatTextType TextType)
        {
            CheckRandomTextDirection();
            combatTexts[combatTextCount].gameObject.SetActive(true);
            combatTexts[combatTextCount].transform.position = Camera.main.WorldToScreenPoint(_transform.position);
            combatTexts[combatTextCount].GetComponent<RectTransform>().localScale = GetTextSizeType(TextType);
            combatTexts[combatTextCount].InitializeCombatText(moveTextSpeed, directionText, fadeTime);
            //combatTexts[combatTextCount].GetComponent<Text>().text = text;
            //combatTexts[combatTextCount].GetComponent<Text>().color = GetTextColor(color);
            //combatTexts[combatTextCount].GetComponent<TextMeshProUGUI>().text = text;
            //combatTexts[combatTextCount].GetComponent<TextMeshProUGUI>().color = GetTextColor(color);
            combatTexts[combatTextCount].textPro.text = text;
            combatTexts[combatTextCount].textPro.color = GetTextColor(color);
            ++combatTextCount;
            //초기화
            //if (combatTextCount >= combatTextNumber)
            if (combatTextCount > combatTexts.Count)
            {
                combatTextCount = 0;
            }
        }

        //타입형 파라미터
        public void showCombatText(Transform _transform, string text, CombatTextCategory combatTextCategory, CombatTextType textType)
        {
            CheckRandomTextDirection();
            combatTexts[combatTextCount].gameObject.SetActive(true);//켜짐
            combatTexts[combatTextCount].transform.position = Camera.main.WorldToScreenPoint(_transform.position);//좌표
            //combatTexts[combatTextCount].GetComponent<Text>().text = text;
            //combatTexts[combatTextCount].GetComponent<TextMeshProUGUI>().text = text;
            combatTexts[combatTextCount].textPro.text = text;
            combatTexts[combatTextCount].InitializeCombatText(moveTextSpeed, directionText, fadeTime);
            switch (combatTextCategory)
            {
                case CombatTextCategory.Alert:
                    //combatTexts[combatTextCount].GetComponent<Text>().color = GetTextColor(CombatTextColor.red);
                    //combatTexts[combatTextCount].GetComponent<TextMeshPro>().color = GetTextColor(CombatTextColor.red);
                    combatTexts[combatTextCount].textPro.color = GetTextColor(CombatTextColor.red);
                    break;
                case CombatTextCategory.Damage:
                    //combatTexts[combatTextCount].GetComponent<Text>().color = GetTextColor(CombatTextColor.yellow);
                    //combatTexts[combatTextCount].GetComponent<TextMeshPro>().color = GetTextColor(CombatTextColor.yellow);
                    combatTexts[combatTextCount].textPro.color = GetTextColor(CombatTextColor.yellow);
                    break;
                case CombatTextCategory.Heal:
                    //combatTexts[combatTextCount].GetComponent<Text>().color = GetTextColor(CombatTextColor.green);
                    //combatTexts[combatTextCount].GetComponent<TextMeshPro>().color = GetTextColor(CombatTextColor.green);
                    combatTexts[combatTextCount].textPro.color = GetTextColor(CombatTextColor.green);
                    break;
                case CombatTextCategory.Normal:
                    //combatTexts[combatTextCount].GetComponent<Text>().color = GetTextColor(CombatTextColor.white);
                    //combatTexts[combatTextCount].GetComponent<TextMeshPro>().color = GetTextColor(CombatTextColor.white);
                    combatTexts[combatTextCount].textPro.color = GetTextColor(CombatTextColor.white);
                    break;
                case CombatTextCategory.Repair:
                    //combatTexts[combatTextCount].GetComponent<Text>().color = GetTextColor(CombatTextColor.green);
                    //combatTexts[combatTextCount].GetComponent<TextMeshPro>().color = GetTextColor(CombatTextColor.green);
                    combatTexts[combatTextCount].textPro.color = GetTextColor(CombatTextColor.green);
                    break;
            }
            combatTexts[combatTextCount].GetComponent<RectTransform>().localScale = GetTextSizeType(textType);

            ++combatTextCount;
            //초기화
            if (combatTextCount >= combatTextNumber)
            {
                combatTextCount = 0;
            }
        }


        //테스트용출력용 함수
        public void TestshowCombatText()
        {
            CheckRandomTextDirection();
            combatTexts[combatTextCount].gameObject.SetActive(true);
            //combatTexts[combatTextCount].transform.parent.SetParent(testTarget, false);
            
            //RectTransform rec;
            //Transform tr;
            combatTexts[combatTextCount].transform.position = Camera.main.WorldToScreenPoint(testTarget.position);
            //combatTexts[combatTextCount].transform.position = Camera.main.ScreenToWorldPoint(testTarget.position);
            //컴뱃텍스트의 좌표가 Rect로 변했음
            //combatTexts[combatTextCount].transform.position = testTarget.position;
            

             


            combatTexts[combatTextCount].GetComponent<RectTransform>().localScale = GetTextSizeType(testCombatTextType);
            combatTexts[combatTextCount].InitializeCombatText(moveTextSpeed, directionText, fadeTime);
            //combatTexts[combatTextCount].GetComponent<Text>().text = testCharecter;
            //combatTexts[combatTextCount].GetComponent<Text>().color = GetTextColor(testCombatTextColor);
            combatTexts[combatTextCount].textPro.text = testCharecter;
            combatTexts[combatTextCount].textPro.color = GetTextColor(testCombatTextColor);
            combatTextCount++;
            //초기화
            if (combatTextCount >= combatTextNumber)
            {
                combatTextCount = 0;
            }
        }
        //테스트용출력용 함수(타겟)
        public void TestshowCombatText_Target(Transform transform)
        {
            CheckRandomTextDirection();
            combatTexts[combatTextCount].gameObject.SetActive(true);
            combatTexts[combatTextCount].transform.position = Camera.main.WorldToScreenPoint(transform.position);
            combatTexts[combatTextCount].GetComponent<RectTransform>().localScale = GetTextSizeType(testCombatTextType);
            combatTexts[combatTextCount].InitializeCombatText(moveTextSpeed, directionText, fadeTime);
            //combatTexts[combatTextCount].GetComponent<Text>().text = testCharecter;
            //combatTexts[combatTextCount].GetComponent<Text>().color = GetTextColor(testCombatTextColor);
            combatTexts[combatTextCount].GetComponent<TextMeshPro>().text = testCharecter;
            combatTexts[combatTextCount].GetComponent<TextMeshPro>().color = GetTextColor(testCombatTextColor);
            combatTextCount++;
            //초기화
            if (combatTextCount >= combatTextNumber)
            {
                combatTextCount = 0;
            }
        }

        //텍스트색깔타입에 맞게 컬러를 반환
        //추가적으로 집어넣을수 있음
        //나중에 컬러셋같은걸 사용해서 집어넣으면 괜찮을듯
        private Color GetTextColor(CombatTextColor combatTextColor)
        {
            Color color;
            switch (combatTextColor)
            {
                case CombatTextColor.white:
                    color = Color.white;
                    break;
                case CombatTextColor.red:
                    color = Color.red;
                    break;
                case CombatTextColor.blue:
                    color = Color.blue;
                    break;
                case CombatTextColor.green:
                    color = Color.green;
                    break;
                case CombatTextColor.yellow:
                    color = Color.yellow;
                    break;
                default:
                    color = Color.white;
                    break;
            }
            return color;
        }

        //컴뱃텍스쳐의 타입을 반환해줌
        private Vector2 GetTextSizeType(CombatTextType combatTextType)
        {
            Vector2 vector2;
            switch (combatTextType)
            {
                case CombatTextType.normal:
                    vector2 = normalTextSize;
                    break;
                case CombatTextType.Crit:
                    vector2 = critTextSize;
                    break;
                default:
                    vector2 = normalTextSize;
                    break;
            }
            return vector2;
        }

        //텍스트의 랜덤방향을 생성해주는 함수
        private void CheckRandomTextDirection()
        {
            if (isRandomDirection)
            {
                directionText.x = Random.Range(minRandomVecter2.x, maxRandomVecter2.x);
                directionText.y = Random.Range(minRandomVecter2.y, maxRandomVecter2.y);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            combatTextCanvas = null;
            combatTextPrefab = null;
            combatTexts.Clear();
            testCharecter = null;
            testTarget = null;
        }

        

#if UNITY_EDITOR

        //[ButtonMethod]
        //RTS셀렉터 제작(클릭한다음 설정하고 프리팹화시키면됨)
        public void CreateCombatText()
        {
            GameObject go = new GameObject();
            go.name = "CombatText";
            go.AddComponent<CombatText>();
            Debug.Log("CombatText가 제작되었습니다 설정하시고 프리팹화 시킨다음 매니저에 집어넣어주세요");
        }
#endif
    }
}

namespace lLCroweTool.CombatTextSystem
{
    //컴뱃텍스트 종류를 열거하는곳
    public enum CombatTextCategory
    {
        Normal,//노말(색깔 : 흰색, 크기 : 일반)
        Alert,//알람(색깔 : 빨간색, 크기 : 일반~커짐)
        Damage,//대미지(색깔 : 빨간색, 크기 : 일반~커짐)
        Repair,//수리(색깔 : 초록색, 크기 : 일반)
        Heal,//힐(색깔 : 초록색, 크기 : 일반)
    }

    //색깔열거형
    public enum CombatTextColor
    {
        white,
        red,
        blue,
        green,
        yellow
    }

    //컴뱃텍스트타입
    public enum CombatTextType
    {
        normal,
        Crit,
    }
}


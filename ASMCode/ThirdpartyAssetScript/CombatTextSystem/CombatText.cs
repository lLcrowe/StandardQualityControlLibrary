using UnityEngine;
using TMPro;
using lLCroweTool.TimerSystem;

namespace lLCroweTool.CombatTextSystem.Origin
{

    [RequireComponent(typeof(UpdateTimerModule))]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class CombatText : MonoBehaviour
    {
        //컴뱃텍스트는
        //3D Object의 TextMeshPro를 쓰면 가능함


        private UpdateTimerModule timerModule;//타이머모듈
        [HideInInspector]public TextMeshProUGUI textPro;//UGUI
        //public TextMeshPro textPro;//게임월드 pro
        private float speed;
        private Vector3 direction;
        private float fadeTime;

        private float time = -1f;
        //초기화
        private void Awake()
        {
            textPro = GetComponent<TextMeshProUGUI>();
            timerModule = GetComponent<UpdateTimerModule>();
            timerModule.AddUnityEvent(delegate { UpdateCombatText(); });
            timerModule.SetTimer(0.01f);
        }

        private void OnEnable()
        {
            timerModule.enabled = true;
            ResetTime();
        }
        private void OnDisable()
        {
            timerModule.enabled = false;
        }

        //컴벳텍스트업데이트
        private void UpdateCombatText()
        {
            float translation = speed * Time.deltaTime;
            transform.Translate(direction * translation);
            FadeOut();
        }
        //천천히 사라지게하기
        public void FadeOut()
        {
            float alpfaColor = textPro.color.a;
            float rate = 1.0f / fadeTime;
            alpfaColor -= rate * (Time.time - time);
            textPro.color = new Color(textPro.color.r, textPro.color.g, textPro.color.b, alpfaColor);
            if (alpfaColor <= 0)
            {
                gameObject.SetActive(false);
            }
            ResetTime();
        }

        //시간초 리셋
        private void ResetTime()
        {
            time = Time.time;
        }

        //컴뱃텍스트초기화
        public void InitializeCombatText(float speed, Vector3 direction, float fadeTime)
        {
            this.speed = speed;
            this.direction = direction;
            this.fadeTime = fadeTime;
        }

        private void OnDestroy()
        {
            timerModule = null;
            textPro = null;
        }

    }

}

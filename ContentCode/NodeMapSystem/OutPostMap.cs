using lLCroweTool.TimerSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace lLCroweTool.NodeMapSystem
{
    [RequireComponent(typeof(UpdateTimerModule))]
    public class OutPostMap : MonoBehaviour
    {
        //맵
        //맵은 전초기지의 맵을 말하며 
        //해당 전초기지의 우주선위치등을 집어넣음
        //이름을 바꿀가능성있음
        //맵이 아닌 전초기지쪽
        //네이밍 변환해야함

        //총두가지
        //전초기지맵 : OutPostMap로 명명
        //우주항해맵 : SpaceVoyageMap로 명명

        //사라지는 시간
        public float fadeTime;
        
        //위치지정
        public Transform spaceShipLandingPos;//착륙할 우주선의 위치지정
        public Transform spaceOutPostMapPos;//전초기지의 착륙장위치 지정


        UpdateTimerModule timerModule;
        //사라지는 함수
        //페이드아웃?

        private void Awake()
        {
            //타이터모듈에 사라지는거 집어넣기
            timerModule = GetComponent<UpdateTimerModule>();
            timerModule.AddUnityEvent(delegate { FadeOutOutPost(); });
        }


        public void StartFadeOutOutPost()
        {
            timerModule.enabled = true;
        }

        //밑으로 가다가 사라지게
        public void FadeOutOutPost()
        {
            float translation = fadeTime * Time.deltaTime;
            transform.Translate(Vector2.down * translation);
        }
    }
}


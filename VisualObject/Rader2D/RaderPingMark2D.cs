using DG.Tweening;
using lLCroweTool.Sound;
using UnityEngine;

namespace lLCroweTool.RaderSystem
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class RaderPingMark2D : MonoBehaviour
    {
        //20240606
        //몇개 로직변경

        //옛날버전 몇년때인지 모름
        //레이더핑
        //레이어를 따로 배정시켜
        //카메라의 컬링시스템을 사용하여 미니맵 or 게임월드상에 보여줄수 있게할수 있음        
        private SpriteRenderer sr;
        private Transform tr;
        private TweenCallback disableAction;
        private static Color zeroAlphaColor = new Color(0,0,0,0);

        private void Awake()
        {
            tr = transform;
            TryGetComponent(out sr);
            disableAction = Disable;
        }

        /// <summary>
        /// 초기화
        /// </summary>
        /// <param name="color">시작컬러</param>
        /// <param name="sprite">세팅할 스프라이트</param>
        /// <param name="time">지속시간</param>
        /// <param name="startScale">시작 스케일</param>
        /// <param name="endScale">끝 스케일</param>
        /// <param name="pos">위치</param>
        /// <param name="audioClip">오디오 클립</param>
        public void Init(Color color, Sprite sprite, float time, float startScale, float endScale, Vector3 pos, AudioClip audioClip)
        {
            gameObject.SetActive(true);
            sr.color = color;
            sr.sprite = sprite;
            
            sr.DOColor(zeroAlphaColor, time);

            tr.localScale = Vector3.one * startScale;
            tr.DOScale(endScale, time).OnComplete(disableAction);            

            tr.InitTrObjPrefab(pos);
            SoundManager.PlaySound3DAtVector3(audioClip, pos);
        }

        private void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}


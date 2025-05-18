
using UnityEngine;
using MEC;
using System.Collections.Generic;

namespace lLCroweTool
{
    /// <summary>
    /// 이팩트매니저에서 사용하여 관리해주는 오브젝트
    /// 이팩트가 다 작동되고 비활성화되어 사용되게함
    /// </summary>
        
    public class EffectObject : MonoBehaviour
    {
        //20230225//GIR프로젝트에서 이식
        //이팩트타입 
        //1.이팩트는 방향성을 가지고 있다
        //2.방향성이 강하지 않으면 특정 위치에서 랜덤or고정으로 생성 

        //1번 이팩트관련해서는 게임오브젝트의 중심점을 히트위치로 지정하여 텍스쳐를 세팅해야됨
        //2번 1번과 다르게 중심에 맞게 처리//랜덤은 텍스쳐이미지 랜덤으로 처리하는게 맞아보임

        //코루틴으로 작업
        [Header("지속 시간")]
        [Tooltip("최소 2초를 추천함")]
        public float fadeTime = 2f;

        /// <summary>
        /// 이팩트오브젝트 작동
        /// </summary>
        /// <param name="curPos">현재위치</param>
        /// <param name="hitPos">맞은위치</param>
        public void Action(Vector2 curPos, Vector2 hitPos)
        {
            transform.position = curPos;
            transform.rotation = lLcroweUtil.GetRotation(curPos, hitPos);
            //transform.parent = null;

            StartCoroutine(ActionEffectCoroutine(this));
        }



        private static IEnumerator<float> ActionEffectCoroutine(EffectObject effectObject)
        {
            float timer = effectObject.fadeTime;
            //이팩트액션작동

            yield return Timing.WaitForSeconds(timer);
            effectObject.gameObject.SetActive(false);
        }
    }
}

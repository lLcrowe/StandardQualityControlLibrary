#if Doozy

using TMPro;
using UnityEngine;

namespace lLCroweTool
{
    /// <summary>
    /// 퀘스트를 진행할 오브젝트들 액터인포에서 이름은 유니크해야됨
    /// </summary>
    public class ActorInfoObjectScript : ScriptableObject
    {
        //배우데이터//배우정보
        public Sprite actorThumbnail;//섬네일용 스프라이트
        public PortraitUICard portraitUICardPrefab;//포트레이트 카드
        public string actorID;//액터아이디=>프로퍼티로 만들기
        [LocalizeGroup] public string actorNameLocalizeID;
        public string actorName;//이름
        public string actorDescription;//설명
        public TMP_FontAsset fontAsset;//배우의 폰트
    }
}
#endif
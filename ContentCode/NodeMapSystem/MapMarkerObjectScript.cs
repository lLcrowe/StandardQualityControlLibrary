using UnityEngine;
using System.Collections;

namespace lLCroweTool.NodeMapSystem
{
    [CreateAssetMenu(fileName = "New MapMarkerObjectData", menuName = "lLcroweTool/New MapMarkerObjectData")]
    public class MapMarkerObjectScript : IconLabelBase
    {
        //맵마커데이터
        //objectSprite;//맵마커의 아이콘 이미지

        //objectName;//맵마커의 이름        

        //objectShortDescription;//맵마커의 설명

        //objectDescription;//없음//사용안함

        [Header("지도데이터의 맵. 게임오브젝트로 만들어서 프리팹상태로 배치")]
        public GameObject mapGameObject;

    }
}


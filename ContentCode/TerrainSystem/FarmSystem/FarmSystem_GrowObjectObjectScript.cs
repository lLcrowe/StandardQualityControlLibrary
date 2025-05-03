using UnityEngine;

namespace lLCroweTool.TerrainSystem.FarmSystem
{
    [CreateAssetMenu(fileName = "New GrowObjectData", menuName = "lLcroweTool/New GrowObjectData")]
    public class FarmSystem_GrowObjectObjectScript : IconLabelBase
    {
        [Header("GrowObjectInfo")]
        public int growObjectMaxAge = 10;//최대나이
        public int growObjectFullyGrowAge;//다 자란 나이
        public float growingAgeTimer;//1살 자라날떄까지 걸리는 시간

        //최대나이가 지나면 랜덤대미지입는 것
        public bool isUseMaxAgeDamage = false;
        public int maxAgeDamage = 100;//최소 1

        public int desireDepletionDamage = 10;//결핍시 대미지량


        //자라나는 오브젝트에서 생산되는 상품(오브젝트 or 아이템)
        //과실이 매치기까지의 데이터들
        //과실이 매칠 확률
        [Header("GrowObjectProductInfo")]
        //여러개를 처리할수 있음
        public GrowObjectProductInfo[] growObjectProductInfoArray = new GrowObjectProductInfo[0];
    }

    /// <summary>
    /// 상품정보데이터. 성장오브젝트 상품에 사용됨
    /// </summary>
    [System.Serializable]
    public class GrowObjectProductInfo
    {
        public int productionPercent = 80;//과실이 자라날확률
        public int productFullGrowAge = 10;//다 자라난 나이

        public bool productMaxAgeToRotton = false;//다 자란후에 썩는 오브젝트인지//썩은후에는 수확해도 얻어가는게 없음
        public int productMaxAge = 10;//최대나이
        public int productRottonPersentValue;//썩을 확률
        public bool isRottenToReset = false;//썩은후 그대로 두기? 아니면 리셋시키기

        //자동으로 수확되는 최종상품인가?
        public bool isAutoHarvest;
        public int autoHarvestPersent = 100;//자동수확 확률

        //어떠한 최종상품을 수확하는가
        //아이템
        //public ItemObjectScript itemData;
        //public int itemHarvestPersent = 100;//수확 확률
        //public int itemMinCount;
        //public int itemMaxCount;

        //게임오브젝트
        public GameObject productObject;
        public int productHarvestPersent = 100;//수확 확률
        public int productMinCount;
        public int productMaxCount;
    }

    /// <summary>
    /// GrowObject가 사용할 상품스탯
    /// </summary>
    [System.Serializable]
    public class GrowObjectProductStatus
    {
        public int productCurAge = 0;//현재상품나이
        public bool isProductionReady = false;//과실이 맽혓는가 여부
        public bool isProductionRotten = false;//과실이 썩었는가
    }

}
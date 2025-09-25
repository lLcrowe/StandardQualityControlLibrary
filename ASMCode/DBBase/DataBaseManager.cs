
using lLCroweTool.Dictionary;
using lLCroweTool.Singleton;
using UnityEngine;

namespace lLCroweTool.DataBase
{
    public class DataBaseManager : MonoBehaviourSingleton<DataBaseManager>
    {
        //온갖게임에 들어갈 데이터를 다 집어넣는 구역
        //오브젝트매니저와는 다른 종류의 기능
        //여기는 데이터만
   
        //가변데이터는 스크립터블로 만들어지지 말아야됨
        //여기자체를 게임데이터매니저를 하는게

        //여기는 말단(고정, 스크립터블)데이터만 넣는 구역

        //모든 불변데이터는 스크립터블로 처리하기
        //그럼 이게 필요한가?


        //여기를 스크립터블 오브젝트로 도배
        public DataBaseInfo_Base dataBaseInfo;


        //아이템정보들을 넣음
       

        //스프라이트들을 넣을 
        [System.Serializable]
        public class SpriteDataBaseBible : CustomDictionary<string, Sprite> { }
        public SpriteDataBaseBible spriteDataBaseBible = new SpriteDataBaseBible();

        protected override void Init()
        {
#if lLcroweLogSystem
            LogManager.Register("DataImport", "DataBaseManager", false, false);

            //데이터베이스에서 가져와서 다 박아버리기
            if (dataBaseInfo == null)
            {
                lLCroweTool.LogSystem.LogManager.Log("DataImport","데이터베이스 정보가 비어있습니다.");
                return;
            }
#endif

            //데이터베이스
            //itemDataBaseBible.AddBibleForInfoList(dataBaseInfo.itemInfoList);
        }

        public Sprite RequestSprite(string id)
        {
            spriteDataBaseBible.TryGetValue(id, out Sprite sprite);
            return sprite;
        }
    }

    [System.Serializable]
    public class TempleteDataBaseBible<T> : CustomDictionary<string, T> { }
    public static class ExtendDataBaseManager
    {
        public static T RequestData<T>(this TempleteDataBaseBible<T> dataBaseBible, string id) where T : LabelBase
        {
            dataBaseBible.TryGetValue(id, out T itemData);
            return itemData;
        }
    }
}
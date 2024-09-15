using UnityEngine;
using lLCroweTool.Dictionary;

namespace lLCroweTool.Localize
{
    [CreateAssetMenu(fileName = "New LocalizeDBData", menuName = "lLcroweTool/LocalizeDBData")]
    public class LocalizeDBObjectScript : DBObjectScriptBase
    {
        //로컬라이징데이터들이 들어가 있는 데이터
        [SerializeField]public LocalizeDataBible localizeDataBible = new LocalizeDataBible();//번역정보가 들어가있는 데이터시트
    }

    [System.Serializable]
    public class LocalizeDataBible : CustomDictionary<string, LocalizeData>{}

    [System.Serializable]
    public class LocalizeData
    {
        public string string_ID;
        public string kor;
        public string eng;
    }
}
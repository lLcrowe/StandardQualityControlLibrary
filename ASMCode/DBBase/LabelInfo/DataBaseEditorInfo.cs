using lLCroweTool.Dictionary;
using System.Collections.Generic;
using UnityEngine;

namespace lLCroweTool.QC.EditorOnly
{
    [System.Serializable]
    public class ResourcesBible : CustomDictionary<string, Object> { }
    public class DataBaseEditorInfo : ScriptableObject
    {
        //Resources 관련//이거 따로빼서 가지고 있는게 맞아보임
        public ResourcesBible resourcesSpriteBible = new();
        public ResourcesBible resourcesObjectBible = new();

        public List<string> allResourcesPathList = new();
    }

    /// <summary>
    /// Resources폴더내에서 특정 클래스타입 찾을 종류들
    /// </summary>
    public enum ResourcesSearchClassType
    {
        Sprite,
        GameObject,
    }

    //리소스관리 관련

    //나중에 처리
    public class CustomResources
    {
        public ResourcesSearchClassType resourcesSearchClassType;
        public string name;
        public int nameHash;//+그룹핑추가
        public Object targetObject;
    }
}
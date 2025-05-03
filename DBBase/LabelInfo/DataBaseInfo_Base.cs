using System.Collections.Generic;
using UnityEngine;

namespace lLCroweTool.DataBase
{
    [CreateAssetMenu(fileName = "New DataBaseInfo_Base", menuName = "lLcroweTool/New DataBaseInfo_Base")]    
    public class DataBaseInfo_Base : ScriptableObject
    {
        //A0/Resources 폴더안에 배치할것
        //CSV텍스트파일//변경될수 있으니 그대로 둠

        //20250504
        //string처리가 기본으로
        
        public TextAsset itemInfoTextSheet;

        public TextAsset achievementInfoTextSheet;
        public TextAsset achievementConditionInfoTextSheet;        
        public TextAsset achievementRewardTextSheet;
        public TextAsset recordActionInfoTextSheet;

        public TextAsset unitObjectTextSheet;

        //UI테마에 맞게 CSV처리대기
        public TextAsset iconPresetTextSheet;
        public TextAsset uiThemaTextSheet;


        
    }
}


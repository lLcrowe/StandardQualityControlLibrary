using lLCroweTool.Achievement;
using System.Collections.Generic;
using UnityEngine;

namespace lLCroweTool.DataBase
{
    [CreateAssetMenu(fileName = "New DataBaseInfo_Base", menuName = "lLcroweTool/New DataBaseInfo_Base")]    
    public class DataBaseInfo_Base : ScriptableObject
    {
        //A0/Resources 폴더안에 배치할것
        //CSV텍스트파일//변경될수 있으니 그대로 둠
        
        public TextAsset itemInfoTextSheet;

        public TextAsset achievementInfoTextSheet;
        public TextAsset achievementConditionInfoTextSheet;        
        public TextAsset achievementRewardTextSheet;
        public TextAsset recordActionInfoTextSheet;

        public TextAsset unitObjectTextSheet;

        //UI테마에 맞게 CSV처리대기
        public TextAsset iconPresetTextSheet;
        public TextAsset uiThemaTextSheet;


        //데이터베이스//리스토로 가지고 동작되면 다른 매니저에서 딕셔너리로 가짐//인포와 연동        
        public List<RecordActionInfo> recordActionInfoList = new List<RecordActionInfo>();
        public List<AchievementInfo> achievementInfoList = new List<AchievementInfo>();
        public List<AchievementConditionInfo> achievementConditionInfoList = new List<AchievementConditionInfo>();        
        public List<RewardInfo> achievementRewardInfoList = new List<RewardInfo>();
    }
}


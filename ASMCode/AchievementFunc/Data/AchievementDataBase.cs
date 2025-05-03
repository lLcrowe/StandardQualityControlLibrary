using lLCroweTool.DataBase;
using System.Collections.Generic;
using UnityEngine;


namespace lLCroweTool.Achievement 
{
    public class AchievementDataBase : DataBaseInfo_Base
    {
        //데이터베이스//리스토로 가지고 동작되면 다른 매니저에서 딕셔너리로 가짐//인포와 연동        
        public List<RecordActionInfo> recordActionInfoList = new List<RecordActionInfo>();
        public List<AchievementInfo> achievementInfoList = new List<AchievementInfo>();
        public List<AchievementConditionInfo> achievementConditionInfoList = new List<AchievementConditionInfo>();
        public List<RewardInfo> achievementRewardInfoList = new List<RewardInfo>();
    }

}



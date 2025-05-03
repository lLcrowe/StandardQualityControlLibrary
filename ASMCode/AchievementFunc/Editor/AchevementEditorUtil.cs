#if Achevement

using lLCroweTool.Achievement;
using System.Collections.Generic;

namespace lLCroweTool.QC.EditorOnly
{
    public static class AchevementEditorUtil
    {
        public static void AchievementInfoImport(Dictionary<string, object> item, AchievementInfo achievementInfo)
        {
            //20240526
            //이함수 어디다 갔다 팔았지?
            //만들수는 있는데 어디다 넣었는지 체크해야됨
            //포트폴리오사용한거 같은데 기억이
            achievementInfo.achievementActionType = item.GetConvertEnum<EAchievementActionType>("achievementActionType");//테스트해보기
            //achievementInfo.achievementActionType = (EAchievementActionTagType)Enum.Parse(typeof(EAchievementActionTagType), (string)item["achievementActionType"]);
            achievementInfo.isAutoUnlock = item.GetConvertBool("isAutoComplete");
            //achievementInfo.uiBarPrefab = item.GetResourcesForComponent<UIBar_Base>("UIBarPrefabName", DataBaseEditorUtil.CATPathType.UIBar);
            
        }
    }
}
#endif
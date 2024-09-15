using lLCroweTool.Singleton;
using System.Collections.Generic;

namespace Assets.StandardQualityControlLibary.TestModule.Ecosystem
{
    public class EcoSystemSpawnAreaGroupManager : MonoBehaviourSingleton<EcoSystemSpawnAreaGroupManager>
    {
        //생태계스폰 영역 그룹 매니저
        //생태계스폰 영역을 업데이트해주는 함수
        private List<EcoSystemSpawnArea> ecoSystemSpawnAreaList = new List<EcoSystemSpawnArea> ();

        protected override void Init(){}

        /// <summary>
        /// 그룹추가
        /// </summary>
        /// <param name="area">영역</param>
        public void AddGroup(EcoSystemSpawnArea area)
        {
            ecoSystemSpawnAreaList.Add(area);
        }

        /// <summary>
        /// 그룹삭제
        /// </summary>
        /// <param name="area">영역</param>
        public void RemoveGroup(EcoSystemSpawnArea area)
        {
            ecoSystemSpawnAreaList.Remove(area);
        }

        private void Update()
        {
            //그룹 업데이트
            for (int i = 0; i < ecoSystemSpawnAreaList.Count; i++)
            {
                var temp  = ecoSystemSpawnAreaList[i];
                temp.UpdateArea();
            }
        }
    }
}
#if Doozy

using lLCroweTool.WorldObjectSystem;
using lLCroweTool.WorldObjectSystem.Structure;
using UnityEngine;
using UnityEngine.Tilemaps;
namespace lLCroweTool.BuildingSystem
{
        
    [CreateAssetMenu(fileName = "New BuildingData", menuName = "lLcroweTool/New BuildingData")]
    public class BuildingObjectScript : IconLabelBase
    {
        //건물타입//빌딩건설을 위한 UI표시를 위한 자리지정 변수
        public BuildingType buildingType = BuildingType.Floor;

        //미리보기 건물이미지
        public Sprite bluePrintBuildingImage;
        //빌딩 오브젝트
        public TestWorldStructureObject buildingObject;
        //빌딩의 임시 작업지역
        public SkeletonStructureObject skeletonStructure;
        //바닥타일//바닥일시 집어넣음
        public Tile buildingTile;
        //바닥골조타일
        public Tile skeletonTile;

        //필요 자원설명
        public IconLabelBase[] resourceDataArray = new IconLabelBase[0];//아이템(자원)데이터
        public int[] resourceNeedAmounts = new int[0];//자원필요량

        //건설중에 필요한 값
        public int buildingWorkNeedValue;//빌딩 타임
        public SkeletonStructureType skeletonStructureType;//작동되는 골조타입

        //해당건물을 고치기위해서는 무엇이 필요한가
        public IconLabelBase[] fixResourceDataArray = new IconLabelBase[0];//아이템(자원)데이터
        public int[] fixResourceNeedAmounts = new int[0];//자원필요량

        //제작하기 전에 해당건물이 지어져있는가 여부으로 필요한 건물
        //건축전에 확인할 빌딩데이터들
        public BuildingObjectScript[] checkBuildings = new BuildingObjectScript[0];//확인할 빌딩데이터
        public ComparisonOperatorType[] checkComparisonOperatorTypes = new ComparisonOperatorType[0];//비교타입
        public int[] checkBuildAmounts = new int[0];//제작된건물의 수
    }
}
#endif
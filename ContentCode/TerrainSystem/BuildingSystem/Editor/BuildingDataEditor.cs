#if UNITY_EDITOR && Doozy
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;
using lLCroweTool.BuildingSystem;
using lLCroweTool.WorldObjectSystem;
using lLCroweTool.WorldObjectSystem.Structure;

using UnityEditor;
#pragma warning disable 0618
namespace lLCroweTool.QC.EditorOnly
{
    public class BuildingDataEditor :  CustomDataWindowEditor<BuildingObjectScript>
    {
        ItemObjectScript buildingItemData;
        int buildingItemCount;

        ItemObjectScript fixItemData;
        int fixItemCount;

        int checkBuildAmount;
        ComparisonOperatorType comparisonOperatorType;        
        BuildingObjectScript checkBuildData;

        [MenuItem("lLcroweTool/BuildingDataEditor")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow editorWindow = GetWindow(typeof(BuildingDataEditor));
            editorWindow.titleContent.text = "건물 생성 관리자";
            editorWindow.minSize = new Vector2(600, 515);
            editorWindow.maxSize = new Vector2(600, 515);
        }

        protected override void SetDataContentName(ref string dataContentName)        
        {
            dataContentName = "건물";
        }

        //protected override void CreateDataAction()
        //{
        //    targetData = new BuildingObjectScript();
        //}

        protected override void SetSaveFileData(ref string labelNameOrTitle, ref string tag, ref string folderName)        
        {
            tag = targetData.buildingType.ToString();
            folderName = "BuildingDataFolder";
        }

        protected override void DataDisplaySection()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            targetData.objectName = EditorGUILayout.TextField(dataContentName + "데이터 이름", targetData.objectName);
            EditorGUILayout.LabelField(dataContentName + "데이터 아이콘");
            targetData.objectSprite = (Sprite)EditorGUILayout.ObjectField(targetData.objectSprite, typeof(Sprite));
            targetData.objectDescription = EditorGUILayout.TextField(dataContentName + "데이터 간단설명", targetData.objectDescription);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(dataContentName + "에 대한 설정", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            targetData.buildingType = (BuildingType)EditorGUILayout.EnumPopup("건물타입", targetData.buildingType);
            EditorGUILayout.LabelField(dataContentName + " 미리보기 이미지");
            targetData.bluePrintBuildingImage = (Sprite)EditorGUILayout.ObjectField(targetData.bluePrintBuildingImage, typeof(Sprite));

            switch (targetData.buildingType)
            {
                case BuildingType.Frame:
                case BuildingType.Floor:
                case BuildingType.Wall:
                    EditorGUILayout.LabelField(dataContentName + " 건물타일");
                    targetData.buildingTile = (Tile)EditorGUILayout.ObjectField(targetData.buildingTile, typeof(Tile));
                    EditorGUILayout.LabelField(dataContentName + " 골조타일");
                    targetData.skeletonTile = (Tile)EditorGUILayout.ObjectField(targetData.skeletonTile, typeof(Tile));
                    break;
                case BuildingType.Door:
                case BuildingType.Building:
                case BuildingType.Pipe:
                    EditorGUILayout.LabelField(dataContentName + " 오브젝트");
                    targetData.buildingObject = (TestWorldStructureObject)EditorGUILayout.ObjectField(targetData.buildingObject, typeof(TestWorldStructureObject));
                    EditorGUILayout.LabelField(dataContentName + " 골조오브젝트");
                    targetData.skeletonStructure = (SkeletonStructureObject)EditorGUILayout.ObjectField(targetData.skeletonStructure, typeof(SkeletonStructureObject));
                    break;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("필요 건설자원 설정", EditorStyles.boldLabel);
            buildingItemData = (ItemObjectScript)EditorGUILayout.ObjectField("필요 아이템", buildingItemData, typeof(ItemObjectScript));
            buildingItemCount = EditorGUILayout.IntField("필요 갯수", buildingItemCount);

            EditorGUILayout.LabelField("=============================================");
            for (int i = 0; i < targetData.resourceDatas.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();

                string content = "";
                if (targetData.resourceDatas[i])
                {
                    content += targetData.resourceDatas[i].objectName.ToString() + "\n";
                }
                else
                {
                    content += "건설자원 is Null \n";
                }
                content += ": " + targetData.resourceNeedAmounts[i];

                EditorGUILayout.HelpBox(content, MessageType.Info);
                if (GUILayout.Button("필요 건설자원 삭제"))
                {
                    List<ItemObjectScript> tempList = new List<ItemObjectScript>();
                    List<int> tempintList = new List<int>();

                    tempList.AddRange(targetData.resourceDatas);
                    tempintList.AddRange(targetData.resourceNeedAmounts);

                    int value = i;
                    tempList.RemoveAt(value);
                    tempintList.RemoveAt(value);

                    targetData.resourceDatas = tempList.ToArray();
                    targetData.resourceNeedAmounts = tempintList.ToArray();
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("필요 건설자원추가"))
            {
                if (targetData.resourceDatas.Contains(buildingItemData))
                {
                    Debug.Log("이미존재하는 자원입니다.");
                    return;
                }

                List<ItemObjectScript> tempList = new List<ItemObjectScript>();
                List<int> tempintList = new List<int>();

                tempList.AddRange(targetData.resourceDatas);
                tempintList.AddRange(targetData.resourceNeedAmounts);

                tempList.Add(buildingItemData);
                tempintList.Add(buildingItemCount);

                targetData.resourceDatas = tempList.ToArray();
                targetData.resourceNeedAmounts = tempintList.ToArray();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("필요 수리자원 설정", EditorStyles.boldLabel);
            fixItemData = (ItemObjectScript)EditorGUILayout.ObjectField("필요 아이템", fixItemData, typeof(ItemObjectScript));
            fixItemCount = EditorGUILayout.IntField("필요 갯수", fixItemCount);

            EditorGUILayout.LabelField("=============================================");
            for (int i = 0; i < targetData.fixResourceDatas.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();

                string content = "";
                if (targetData.fixResourceDatas[i])
                {
                    content += targetData.fixResourceDatas[i].objectName.ToString() + "\n";
                }
                else
                {
                    content += "수리자원 is Null \n";
                }
                content += ": " + targetData.fixResourceNeedAmounts[i];

                EditorGUILayout.HelpBox(content, MessageType.Info);
                if (GUILayout.Button("필요 수리자원 삭제"))
                {
                    List<ItemObjectScript> tempList = new List<ItemObjectScript>();
                    List<int> tempintList = new List<int>();

                    tempList.AddRange(targetData.fixResourceDatas);
                    tempintList.AddRange(targetData.fixResourceNeedAmounts);

                    int value = i;
                    tempList.RemoveAt(value);
                    tempintList.RemoveAt(value);

                    targetData.fixResourceDatas = tempList.ToArray();
                    targetData.fixResourceNeedAmounts = tempintList.ToArray();
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("필요 수리자원추가"))
            {
                if (targetData.fixResourceDatas.Contains(fixItemData))
                {
                    Debug.Log("이미존재하는 자원입니다.");
                    return;
                }

                List<ItemObjectScript> tempList = new List<ItemObjectScript>();
                List<int> tempintList = new List<int>();

                tempList.AddRange(targetData.fixResourceDatas);
                tempintList.AddRange(targetData.fixResourceNeedAmounts);

                tempList.Add(fixItemData);
                tempintList.Add(fixItemCount);

                targetData.fixResourceDatas = tempList.ToArray();
                targetData.fixResourceNeedAmounts = tempintList.ToArray();
            }



            EditorGUILayout.LabelField("=============================================");
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("건설중에 필요한 설정", EditorStyles.boldLabel);
            targetData.buildingWorkNeedValue = EditorGUILayout.IntField("빌딩 타임", targetData.buildingWorkNeedValue);
            targetData.skeletonStructureType = (SkeletonStructureType)EditorGUILayout.EnumPopup("건물타입", targetData.skeletonStructureType);

            EditorGUILayout.LabelField("추가적으로 체크할 건물데이터설정", EditorStyles.boldLabel);
            checkBuildData = (BuildingObjectScript)EditorGUILayout.ObjectField("건물 데이터", checkBuildData, typeof(BuildingObjectScript));
            comparisonOperatorType = (ComparisonOperatorType)EditorGUILayout.EnumPopup("비교타입", comparisonOperatorType);
            checkBuildAmount = EditorGUILayout.IntField("존재할 건물 갯수", checkBuildAmount);

            EditorGUILayout.LabelField("=============================================");
            for (int i = 0; i < targetData.checkBuildAmounts.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                string temp = "";
                switch (targetData.checkComparisonOperatorTypes[i])
                {
                    case ComparisonOperatorType.Greater:
                        temp = "보다 많아야됨";
                        break;
                    case ComparisonOperatorType.Less:
                        temp = "보다 적어야됨";
                        break;
                    case ComparisonOperatorType.Equal:
                        temp = "와 동일해야됨";
                        break;
                }
                EditorGUILayout.LabelField(targetData.checkBuildings[i].objectName.ToString() + "가 " + targetData.checkBuildAmounts[i].ToString() + "개" + temp);
                if (GUILayout.Button("삭제"))
                {
                    List<BuildingObjectScript> tempList = new List<BuildingObjectScript>();
                    List<ComparisonOperatorType> tempTypeList = new List<ComparisonOperatorType>();
                    List<int> tempintList = new List<int>();

                    tempList.AddRange(targetData.checkBuildings);
                    tempTypeList.AddRange(targetData.checkComparisonOperatorTypes);
                    tempintList.AddRange(targetData.checkBuildAmounts);

                    int value = i;
                    tempList.RemoveAt(value);
                    tempTypeList.RemoveAt(value);
                    tempintList.RemoveAt(value);

                    targetData.checkBuildings = tempList.ToArray();
                    targetData.checkComparisonOperatorTypes = tempTypeList.ToArray();
                    targetData.checkBuildAmounts = tempintList.ToArray();
                   
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("체크할 건축물추가"))
            {
                if (targetData.checkBuildings.Contains(checkBuildData))
                {
                    Debug.Log("이미존재하는 건물데이터입니다.");
                    return;
                }

                List<BuildingObjectScript> tempList = new List<BuildingObjectScript>();
                List<ComparisonOperatorType> tempTypeList = new List<ComparisonOperatorType>();
                List<int> tempintList = new List<int>();

                tempList.AddRange(targetData.checkBuildings);
                tempTypeList.AddRange(targetData.checkComparisonOperatorTypes);
                tempintList.AddRange(targetData.checkBuildAmounts);

                tempList.Add(checkBuildData);
                tempTypeList.Add(comparisonOperatorType);
                tempintList.Add(checkBuildAmount);

                targetData.checkBuildings = tempList.ToArray();
                targetData.checkComparisonOperatorTypes = tempTypeList.ToArray();
                targetData.checkBuildAmounts = tempintList.ToArray();
            }

            EditorGUILayout.LabelField("=============================================");
        }
    }
}
#endif


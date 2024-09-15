using UnityEngine;
using System.Collections.Generic;
using lLCroweTool.TerrainSystem.FarmSystem;
#if UNITY_EDITOR && MEC
using UnityEditor;
#pragma warning disable 0618
namespace lLCroweTool.QC.EditorOnly
{
    public class GrowObjectDataEditor : CustomDataWindowEditor<FarmSystem_GrowObjectObjectScript>
    {                                                                                           
        public int productionPercent = 80;//과실이 자라날확률
        public int productFullGrowAge = 10;//다 자라난 나이

        public bool productMaxAgeToRotton = false;//다 자란후에 썩는 오브젝트인지//썩은후에는 수확해도 얻어가는게 없음
        public int productMaxAge = 10;//최대나이
        public int productRottonPersentValue;//썩을 확률
        public bool isRottenToReset = false;//썩은후 그대로 두기? 아니면 리셋시키기

        //자동으로 수확되는 최종상품인가?
        public bool isAutoHarvest;
        public int autoHarvestPersent = 100;//자동수확 확률

        //어떠한 최종상품을 수확하는가
        //아이템
        public ItemObjectScript itemData;
        public int itemHarvestPersent = 100;//수확 확률
        public int itemMinCount;
        public int itemMaxCount;

        //게임오브젝트
        public GameObject productObject;
        public int productHarvestPersent = 100;//수확 확률
        public int productMinCount;
        public int productMaxCount;

        [MenuItem("lLcroweTool/GrowObjectDataEditor")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow editorWindow = GetWindow(typeof(GrowObjectDataEditor));
            editorWindow.titleContent.text = "성장 오브젝트 관리자";
            editorWindow.minSize = new Vector2(600, 515);
            editorWindow.maxSize = new Vector2(600, 515);
        }

        protected override void SetDataContentName(ref string dataContentName)
        {
            dataContentName = "성장오브젝트";
        }

        protected override void SetSaveFileData(ref string labelNameOrTitle, ref string tag, ref string folderName)
        {
            tag = "GrowObject";
            folderName = "GrowObjectDataFolder";
        }

        protected override void DataDisplaySection(ref FarmSystem_GrowObjectObjectScript targetData)
        {
            string dataContentName = "성장오브젝트";
            lLcroweUtilEditor.IconLabelBaseDataShow(dataContentName, targetData);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(dataContentName + " 정보");
            targetData.growObjectMaxAge = EditorGUILayout.IntField("최대나이", targetData.growObjectMaxAge);
            targetData.growObjectFullyGrowAge = EditorGUILayout.IntField("다 자란 나이", targetData.growObjectFullyGrowAge);
            targetData.growingAgeTimer = EditorGUILayout.FloatField("1살 걸리는 시간", targetData.growingAgeTimer);
            targetData.isUseMaxAgeDamage = EditorGUILayout.Toggle("수명이 되면 대미지 여부", targetData.isUseMaxAgeDamage);
            if (targetData.isUseMaxAgeDamage)
            {
                targetData.maxAgeDamage = EditorGUILayout.IntField("최대나이이상 대미지", targetData.maxAgeDamage);
            }
            targetData.desireDepletionDamage = EditorGUILayout.IntField("욕구결핍시 대미지", targetData.desireDepletionDamage);

            EditorGUILayout.LabelField(dataContentName + " 생산품들 정보");
            EditorGUILayout.LabelField("상품 설정", EditorStyles.boldLabel);

            productionPercent = EditorGUILayout.IntField("과실이 자라날확률", productionPercent);
            productFullGrowAge = EditorGUILayout.IntField("다 자라난 나이", productFullGrowAge);

            productMaxAgeToRotton = EditorGUILayout.Toggle("썩는 생산품 여부", productMaxAgeToRotton);
            if (productMaxAgeToRotton)
            {
                productMaxAge = EditorGUILayout.IntField("최대나이", productMaxAge);
                productRottonPersentValue = EditorGUILayout.IntField("썩을 확률", productRottonPersentValue);
                isRottenToReset = EditorGUILayout.Toggle("썩은후 그대로 두기 여부", isRottenToReset);
            }


            isAutoHarvest = EditorGUILayout.Toggle("자동수확 여부", isAutoHarvest);
            if (isAutoHarvest)
            {
                autoHarvestPersent = EditorGUILayout.IntField("자동수확 확률", autoHarvestPersent);
            }

            itemData = (GameObject)EditorGUILayout.ObjectField("수확 아이템", itemData, typeof(GameObject));
            itemHarvestPersent = EditorGUILayout.IntField("아이템수확 확률", itemHarvestPersent);
            itemMinCount = EditorGUILayout.IntField("아이템수확 최소갯수", itemMinCount);
            itemMaxCount = EditorGUILayout.IntField("아이템수확 최대갯수", itemMaxCount);

            productObject = (GameObject)EditorGUILayout.ObjectField("수확 오브젝트", productObject, typeof(GameObject));
            productHarvestPersent = EditorGUILayout.IntField("오브젝트수확 확률", productHarvestPersent);
            productMinCount = EditorGUILayout.IntField("오브젝트수확 최소갯수", productMinCount);
            productMaxCount = EditorGUILayout.IntField("오브젝트수확 최대갯수", productMaxCount);


            EditorGUILayout.LabelField("=============================================");
            for (int i = 0; i < targetData.growObjectProductInfoArray.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                string content = "";

                //if (targetData.growObjectProductInfoArray[i].itemData)
                //{
                //    content += targetData.growObjectProductInfoArray[i].itemData.objectName + "\n";
                //    content += targetData.growObjectProductInfoArray[i].itemHarvestPersent + "\n";
                //    content += targetData.growObjectProductInfoArray[i].itemMinCount + "\n";
                //    content += targetData.growObjectProductInfoArray[i].itemMaxCount + "\n";
                //}
                //else
                //{
                //    content += "Item is Null \n";
                //}

                if (targetData.growObjectProductInfoArray[i].productObject)
                {
                    content += targetData.growObjectProductInfoArray[i].productObject.name + "\n";
                    content += targetData.growObjectProductInfoArray[i].productHarvestPersent + "\n";
                    content += targetData.growObjectProductInfoArray[i].productMinCount + "\n";
                    content += targetData.growObjectProductInfoArray[i].productMaxCount;
                }
                else
                {
                    content += "productObject is Null";
                }


                EditorGUILayout.HelpBox(content, MessageType.Info);
                if (GUILayout.Button("생산품 삭제"))
                {
                    List<GrowObjectProductInfo> tempList = new List<GrowObjectProductInfo>();

                    tempList.AddRange(targetData.growObjectProductInfoArray);

                    int value = i;
                    tempList.RemoveAt(value);

                    targetData.growObjectProductInfoArray = tempList.ToArray();
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("생산품 추가"))
            {
                List<GrowObjectProductInfo> tempList = new List<GrowObjectProductInfo>();

                tempList.AddRange(targetData.growObjectProductInfoArray);

                GrowObjectProductInfo productInfo = new GrowObjectProductInfo();
                productInfo.productionPercent = productionPercent;
                productInfo.productFullGrowAge = productFullGrowAge;

                productInfo.productMaxAgeToRotton = productMaxAgeToRotton;
                productInfo.productMaxAge = productMaxAge;
                productInfo.productRottonPersentValue = productRottonPersentValue;
                productInfo.isRottenToReset = isRottenToReset;

                productInfo.isAutoHarvest = isAutoHarvest;
                productInfo.autoHarvestPersent = autoHarvestPersent;

                //productInfo.itemData = itemData;
                //productInfo.itemHarvestPersent = itemHarvestPersent;
                //productInfo.itemMinCount = itemMinCount;
                //productInfo.itemMaxCount = itemMaxCount;

                productInfo.productObject = productObject;
                productInfo.productHarvestPersent = productHarvestPersent;
                productInfo.productMinCount = productMinCount;
                productInfo.productMaxCount = productMaxCount;

                tempList.Add(productInfo);

                targetData.growObjectProductInfoArray = tempList.ToArray();
            }

            EditorGUILayout.LabelField("=============================================");
        }
    }
}
#endif
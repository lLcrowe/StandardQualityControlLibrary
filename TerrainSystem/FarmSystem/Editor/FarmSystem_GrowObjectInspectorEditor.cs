#if MEC

using lLCroweTool.TerrainSystem.FarmSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(FarmSystem_GrowObject))]
    public class FarmSystem_GrowObjectInspectorEditor : Editor
    {
        //인스팩터창 에디터
        private FarmSystem_GrowObject growObject;

        private void OnEnable()
        {
            growObject = (FarmSystem_GrowObject)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (growObject.growObjectData == null)
            {
                EditorGUILayout.HelpBox("성장오브젝트 데이터가 비었습니다. 집어넣어주세요", MessageType.Warning);
                return;
            }

            //데이터 동일한지
            if (growObject.growObjectData.growObjectProductInfoArray.Length == growObject.growObjectProductStatusArray.Length)
            {
                if (GUILayout.Button("상품 상태정보리셋"))
                {
                    growObject.growObjectProductStatusArray = new GrowObjectProductStatus[0];
                }
                return;
            }

            //다르면 리셋시키고 작동
            if (GUILayout.Button("상품 상태정보 데이터와 동기화하기"))
            {
                //상태처리
                List<GrowObjectProductStatus> growObjectProductStatusList = new List<GrowObjectProductStatus>();

                for (int i = 0; i < growObject.growObjectData.growObjectProductInfoArray.Length; i++)
                {
                    GrowObjectProductStatus productStatus = new GrowObjectProductStatus();
                    growObjectProductStatusList.Add(productStatus);
                }
                growObject.growObjectProductStatusArray = growObjectProductStatusList.ToArray();
            }
        }
    }
}
#endif
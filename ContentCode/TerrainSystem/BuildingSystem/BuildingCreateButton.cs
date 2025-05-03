#if Doozy

using UnityEngine;
using UnityEngine.EventSystems;
using Doozy.Engine.UI;

namespace lLCroweTool.BuildingSystem
{
    public class BuildingCreateButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private BuildingObjectScript targetBuildData;
        [Header("버튼에서 텍스트는 필요없음")]
        [SerializeField] private UIButton button;

        private void Awake()
        {
            if (button == null)
            {
                button = transform.GetComponent<UIButton>();
            }
        }

        /// <summary>
        /// 건물생성버튼 세팅하는 함수
        /// </summary>
        /// <param name="buildingData">세팅할 건물데이터</param>
        /// <param name="buildModeType">건축모드 타입</param>
        public void SetBuildingCreateButton(BuildingObjectScript buildingData, BuildingConstructionType buildModeType)
        {  
            targetBuildData = buildingData;
            button.Button.interactable = true;
            button.Button.onClick.RemoveAllListeners();
            button.Button.onClick.AddListener(delegate 
            {
                BuildingPointer.Instance.ResetBuildingPointer();
                BuildingPointer.Instance.InitBuildingPointer(buildingData, buildModeType);
            });

            switch (buildModeType)
            {
                case BuildingConstructionType.Build:
                    //건설
                    button.Button.image.sprite = buildingData.icon;

                    //건설전에 확인할 건물데이터들 체크후 상호작용여부
                    //잠금상태이면 상호작용안함
                    button.Button.interactable = !BuildingManager.Instance.CheckCreateLockBuildingData(targetBuildData);
                    break;
                case BuildingConstructionType.Fix:
                    //수리
                    button.Button.image.sprite = BuildingManager.Instance.fixImage;
                    break;
                case BuildingConstructionType.BuildingDismantle:
                    //해제
                    button.Button.image.sprite = BuildingManager.Instance.dismantleBuildingImage;
                    break;
                case BuildingConstructionType.FloorDismantle:
                    //해제
                    button.Button.image.sprite = BuildingManager.Instance.dismantleFloorImage;
                    break;
                case BuildingConstructionType.Connect:
                    //연결
                    button.Button.image.sprite = BuildingManager.Instance.connectImage;
                    break;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (targetBuildData == null)
            {
                return;
            }

            string contentText = "";
            for (int i = 0; i < targetBuildData.resourceDataArray.Length; i++)
            {
                contentText += targetBuildData.resourceDataArray[i].labelNameOrTitle;
                contentText += " : " + targetBuildData.resourceNeedAmounts[i].ToString();
                contentText += "\n";
            }
            contentText += targetBuildData.buildingWorkNeedValue.ToString();
            
            if (!button.Button.interactable)//상호작용안하면 해당물체가 잠겨있음
            {
                //잠겨 있는거 관련해서는 다시 체크해서 만들것
                contentText += "\n";
                for (int i = 0; i < targetBuildData.checkBuildings.Length; i++)
                {
                    contentText += targetBuildData.checkBuildings[i].labelNameOrTitle;
                    contentText += "\n";
                    contentText += targetBuildData.checkBuildAmounts[i] + LocalizingManager.Instance.GetLocalLizeText("Count") + LocalizingManager.Instance.GetLocalLizeText(targetBuildData.checkComparisonOperatorTypes[i].ToString());
                    contentText += "\n";
                }

                //contentText += buildData.precadeResearch.gameObject.name;
            }
            BuildingManager.Instance.buildingtoolTipUiView.ShowText(targetBuildData.labelNameOrTitle, targetBuildData.description, targetBuildData.icon, contentText);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            BuildingManager.Instance.buildingtoolTipUiView.ClearText();
            BuildingManager.Instance.buildingtoolTipUiView.OffText();
        }
    }

}
#endif